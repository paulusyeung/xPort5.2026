"""
Import data into PostgreSQL database.
"""

import psycopg2
import psycopg2.extras
import logging
from typing import List, Tuple, Optional, Dict
from .config import DatabaseConfig
from .type_mapper import TypeMapper

logger = logging.getLogger(__name__)


class DataImporter:
    """Imports data into PostgreSQL."""
    
    def __init__(self, connection_params: Optional[Dict[str, str]] = None):
        """
        Initialize data importer.
        
        Args:
            connection_params: PostgreSQL connection parameters dictionary.
                              If None, uses config from environment.
        """
        self.connection_params = connection_params or DatabaseConfig.get_postgresql_connection_string()
        self.conn = None
    
    def connect(self):
        """Establish connection to PostgreSQL."""
        try:
            self.conn = psycopg2.connect(**self.connection_params)
            logger.info("Connected to PostgreSQL")
        except Exception as e:
            logger.error(f"Failed to connect to PostgreSQL: {e}")
            raise
    
    def disconnect(self):
        """Close connection to PostgreSQL."""
        if self.conn:
            self.conn.close()
            logger.info("Disconnected from PostgreSQL")
    
    def execute_sql(self, sql: str):
        """
        Execute raw SQL command.
        
        Args:
            sql: SQL command to execute
        """
        if not self.conn:
            self.connect()
            
        cursor = self.conn.cursor()
        try:
            cursor.execute(sql)
            self.conn.commit()
            logger.info(f"Executed SQL: {sql}")
        except Exception as e:
            self.conn.rollback()
            logger.error(f"Failed to execute SQL: {e}")
            raise
        finally:
            cursor.close()

    def truncate_table(self, table_name: str):
        """
        Truncate a table, removing all rows.
        
        Args:
            table_name: Name of the table to truncate
        """
        if not self.conn:
            self.connect()
            
        cursor = self.conn.cursor()
        target_bbox = table_name.lower()
        
        # Use CASCADE to handle constraint dependencies
        sql = f'TRUNCATE TABLE "{target_bbox}" CASCADE'
        
        try:
            cursor.execute(sql)
            self.conn.commit()
            logger.info(f"Truncated table: {target_bbox}")
        except Exception as e:
            self.conn.rollback()
            logger.error(f"Failed to truncate table {target_bbox}: {e}")
            raise
        finally:
            cursor.close()

    def import_table_data(self, table_name: str, columns: List[Dict[str, any]], 
                         data_batches: List[List[Tuple]]):
        """
        Import data batches into a table.
        
        Args:
            table_name: Name of the table
            columns: List of column information dictionaries
            data_batches: List of data batches (each batch is a list of tuples)
        """
        if not self.conn:
            self.connect()
        
        cursor = self.conn.cursor()
        
        # Ensure lowercase for PostgreSQL compatibility
        target_bbox = table_name.lower()
        
        # Build column names (quoted to preserve logic but lowercased)
        column_names = [f'"{col["name"].lower()}"' for col in columns]
        placeholders = ', '.join(['%s'] * len(columns))
        
        insert_sql = f'''
            INSERT INTO "{target_bbox}" ({', '.join(column_names)})
            VALUES ({placeholders})
        '''
        
        total_rows = 0
        
        try:
            for batch in data_batches:
                psycopg2.extras.execute_batch(cursor, insert_sql, batch)
                total_rows += len(batch)
                logger.debug(f"Imported {len(batch)} rows into {target_bbox} (total: {total_rows})")
            
            self.conn.commit()
            logger.info(f"Imported {total_rows} rows into {target_bbox}")
        except Exception as e:
            self.conn.rollback()
            logger.error(f"Failed to import data into {target_bbox}: {e}")
            raise
        finally:
            cursor.close()
    
    def get_table_row_count(self, table_name: str) -> int:
        """
        Get row count for a table.
        
        Args:
            table_name: Name of the table
            
        Returns:
            Number of rows
        """
        if not self.conn:
            self.connect()
        
        cursor = self.conn.cursor() # Table name lowercased
        cursor.execute(f'SELECT COUNT(*) FROM "{table_name.lower()}"')
        count = cursor.fetchone()[0]
        cursor.close()
        return count
    
    def validate_data(self, table_name: str, expected_count: int) -> bool:
        """
        Validate that imported data matches expected row count.
        
        Args:
            table_name: Name of the table
            expected_count: Expected number of rows
            
        Returns:
            True if count matches, False otherwise
        """
        # Lowercase handled in get_table_row_count
        actual_count = self.get_table_row_count(table_name)
        if actual_count == expected_count:
            logger.info(f"Validation passed for {table_name}: {actual_count} rows")
            return True
        else:
            logger.warning(
                f"Validation failed for {table_name}: "
                f"expected {expected_count}, got {actual_count}"
            )
            return False
    
    def __enter__(self):
        """Context manager entry."""
        self.connect()
        return self
    
    def __exit__(self, exc_type, exc_val, exc_tb):
        """Context manager exit."""
        self.disconnect()

