#!/usr/bin/env python3
"""
Main data migration script.
Migrates data from MS SQL Server to PostgreSQL.
"""

import argparse
import logging
import sys
import fnmatch
import time
from datetime import datetime
from typing import List, Optional, NamedTuple
from tqdm import tqdm

from .config import DatabaseConfig
from .data_extractor import DataExtractor
from .data_importer import DataImporter

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)


class TableMigrationStat(NamedTuple):
    table_name: str
    rows_migrated: int
    start_time: str
    end_time: str
    duration_sec: float


class MigrationOrchestrator:
    """Orchestrates the data migration process."""
    
    def __init__(self, mssql_conn_str: Optional[str] = None, 
                 pg_conn_params: Optional[dict] = None,
                 truncate: bool = False):
        """
        Initialize migration orchestrator.
        
        Args:
            mssql_conn_str: MS SQL Server connection string
            mssql_conn_str: MS SQL Server connection string
            pg_conn_params: PostgreSQL connection parameters
            truncate: Whether to truncate target tables before migration
        """
        self.extractor = DataExtractor(mssql_conn_str)
        self.importer = DataImporter(pg_conn_params)
        self.batch_size = DatabaseConfig.get_batch_size()
        self.truncate = truncate
        self.stats: List[TableMigrationStat] = []
    
    def _set_constraints_enabled(self, enabled: bool):
        """
        Disables or enables Foreign Key constraints on the target database 
        by setting session_replication_role.
        """
        role = "origin" if enabled else "replica"
        
        sql = f"SET session_replication_role = '{role}';"
        
        try:
            self.importer.execute_sql(sql)
            logger.info(f"Constraints (session_replication_role) set to: {role}")
        except Exception as e:
            logger.error(f"Failed to set constraints to {role}: {e}")

    def get_ordered_tables(self, all_tables: List[str], 
                          include_patterns: Optional[List[str]],
                          priority_patterns: List[str], 
                          exclude_patterns: List[str]) -> List[str]:
        """
        Order tables based on include, priority and exclusion patterns.
        """
        # 0. Filter by Include patterns (White-listing)
        if include_patterns:
            included_tables = set()
            for pattern in include_patterns:
                matches = fnmatch.filter(all_tables, pattern)
                included_tables.update(matches)
            # If explicit include list provided, we only consider those
            working_set = [t for t in all_tables if t in included_tables]
        else:
            working_set = all_tables

        # 1. Apply Exclusion patterns (Black-listing)
        matches_to_exclude = set()
        for pattern in exclude_patterns:
            matches = fnmatch.filter(working_set, pattern)
            matches_to_exclude.update(matches)
            
        final_set = [t for t in working_set if t not in matches_to_exclude]
        
        # 2. Handle priorities
        if not priority_patterns:
            return sorted(final_set)

        prioritized = []
        remaining = set(final_set)
        
        for pattern in priority_patterns:
            matches = fnmatch.filter(list(remaining), pattern)
            matches.sort()
            for t in matches:
                prioritized.append(t)
                remaining.remove(t)
        
        # 3. Append remaining tables
        ordered = prioritized + sorted(list(remaining))
        
        return ordered
    
    def migrate_table(self, table_name: str, show_progress: bool = True) -> bool:
        """
        Migrate a single table.
        """
        start_ts = time.time()
        start_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        rows_migrated = 0
        success = False
        
        try:
            logger.info(f"Starting migration of table: {table_name}")
            
            # Get column information
            columns = self.extractor.get_table_columns(table_name)
            
            # Get row count
            row_count = self.extractor.get_table_row_count(table_name)
            logger.info(f"Table {table_name} has {row_count} rows")
            
            if row_count == 0:
                logger.info(f"Skipping empty table: {table_name}")
                # Record stat for empty table
                self.stats.append(TableMigrationStat(
                    table_name, 0, start_str, datetime.now().strftime("%Y-%m-%d %H:%M:%S"), 
                    time.time() - start_ts
                ))
                return True
            
            # Truncate if requested
            if self.truncate:
                self.importer.truncate_table(table_name)

            # Extract
            data_batches = []
            extractor_iter = self.extractor.extract_table_data(table_name, self.batch_size)
            
            if show_progress:
                extractor_iter = tqdm(extractor_iter, desc=f"Extracting {table_name}", unit="batch")
            
            for batch in extractor_iter:
                data_batches.append(batch)
            
            # Import
            logger.info(f"Importing {len(data_batches)} batches into PostgreSQL...")
            import_iter = data_batches
            if show_progress:
                import_iter = tqdm(data_batches, desc=f"Importing {table_name}", unit="batch")
            
            self.importer.import_table_data(table_name, columns, import_iter)
            
            # Validate
            if self.importer.validate_data(table_name, row_count):
                logger.info(f"Successfully migrated table: {table_name}")
                rows_migrated = row_count
                success = True
            else:
                logger.error(f"Validation failed for table: {table_name}")
                success = False
                
        except Exception as e:
            logger.error(f"Failed to migrate table {table_name}: {e}", exc_info=True)
            success = False
        
        end_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        duration = time.time() - start_ts
        self.stats.append(TableMigrationStat(
            table_name, rows_migrated if success else 0, start_str, end_str, duration
        ))
        
        return success
    
    def print_summary(self):
        """Prints a tabular summary of the migration."""
        print("\n" + "="*100)
        print(f"| {'Table Name':<30} | {'Rows':<10} | {'Started':<20} | {'Finished':<20} | {'Duration (s)':<10} |")
        print("|" + "-"*32 + "|" + "-"*12 + "|" + "-"*22 + "|" + "-"*22 + "|" + "-"*12 + "|")
        
        total_rows = 0
        total_duration = 0.0
        
        for stat in self.stats:
            print(f"| {stat.table_name:<30} | {stat.rows_migrated:<10} | {stat.start_time:<20} | {stat.end_time:<20} | {stat.duration_sec:<12.2f} |")
            total_rows += stat.rows_migrated
            total_duration += stat.duration_sec
            
        print("|" + "-"*32 + "|" + "-"*12 + "|" + "-"*22 + "|" + "-"*22 + "|" + "-"*12 + "|")
        print(f"| {'TOTAL':<30} | {total_rows:<10} | {'':<20} | {'':<20} | {total_duration:<12.2f} |")
        print("="*100 + "\n")

    def migrate_all(self, include_list: Optional[List[str]] = None,
                   priority_list: Optional[List[str]] = None, 
                   exclude_list: Optional[List[str]] = None) -> bool:
        """
        Migrate tables with include/priority/exclusion logic.
        """
        try:
            # Connect
            self.extractor.connect()
            self.importer.connect()
            
            # Get table list
            all_tables = self.extractor.get_table_list()
            
            # Determine order
            ordered_tables = self.get_ordered_tables(
                all_tables,
                include_list,
                priority_list or [], 
                exclude_list or []
            )
            
            if not ordered_tables:
                logger.warning("No tables matched for migration.")
                return True

            logger.info(f"Migrating {len(ordered_tables)} tables...")
            logger.debug(f"Table Order: {ordered_tables}")
            
            # 1. Disable Constraints
            self._set_constraints_enabled(False)

            # Migrate
            success_count = 0
            failed_tables = []
            
            for table in ordered_tables:
                if self.migrate_table(table):
                    success_count += 1
                else:
                    failed_tables.append(table)
            
            # 2. Print Summary
            self.print_summary()
            
            if failed_tables:
                logger.error(f"Failed tables: {', '.join(failed_tables)}")
                return False
            
            return True
            
        except Exception as e:
            logger.error(f"Migration failed: {e}", exc_info=True)
            return False
        finally:
            try:
                self._set_constraints_enabled(True)
            except:
                pass 
            self.extractor.disconnect()
            self.importer.disconnect()


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(description='Migrate data from MS SQL Server to PostgreSQL')
    
    parser.add_argument('--source', help='MS SQL connection string')
    parser.add_argument('--target-host', help='PG host')
    parser.add_argument('--target-port', type=int, help='PG port')
    parser.add_argument('--target-database', help='PG database')
    parser.add_argument('--target-user', help='PG user')
    parser.add_argument('--target-password', help='PG password')
    
    parser.add_argument(
        '--include', 
        help='Comma-separated list of tables to include (supports wildcards). If omitted, includes all tables.'
    )
    parser.add_argument(
        '--exclude',
        help='Comma-separated list of tables to exclude (supports wildcards).'
    )
    parser.add_argument(
        '--priority',
        help='Comma-separated list of tables to migrate FIRST (supports wildcards).'
    )
    
    parser.add_argument('--batch-size', type=int, default=1000, help='Batch size')
    parser.add_argument('--truncate', action='store_true', help='Truncate target tables before migration')
    
    args = parser.parse_args()
    
    # Build PG connection
    pg_params = DatabaseConfig.get_postgresql_connection_string()
    if args.target_host: pg_params['host'] = args.target_host
    if args.target_port: pg_params['port'] = args.target_port
    if args.target_database: pg_params['database'] = args.target_database
    if args.target_user: pg_params['user'] = args.target_user
    if args.target_password: pg_params['password'] = args.target_password
    
    orchestrator = MigrationOrchestrator(
        mssql_conn_str=args.source, 
        pg_conn_params=pg_params,
        truncate=args.truncate
    )
    if args.batch_size:
        orchestrator.batch_size = args.batch_size
        
    # Process lists
    include_list = [x.strip() for x in args.include.split(',')] if args.include else None
    priority_list = [x.strip() for x in args.priority.split(',')] if args.priority else []
    exclude_list = [x.strip() for x in args.exclude.split(',')] if args.exclude else ['Log4Net']
    
    success = orchestrator.migrate_all(
        include_list=include_list,
        priority_list=priority_list,
        exclude_list=exclude_list
    )
    
    sys.exit(0 if success else 1)


if __name__ == '__main__':
    main()
