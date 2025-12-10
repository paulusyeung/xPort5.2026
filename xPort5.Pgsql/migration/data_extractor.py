"""
Extract data from MS SQL Server database.
"""

import pyodbc
import logging
from typing import List, Dict, Iterator, Optional
from .config import DatabaseConfig
from .type_mapper import TypeMapper

logger = logging.getLogger(__name__)


class DataExtractor:
    """Extracts data from MS SQL Server."""
    
    def __init__(self, connection_string: Optional[str] = None):
        """
        Initialize data extractor.
        
        Args:
            connection_string: MS SQL Server connection string.
                              If None, uses config from environment.
        """
        self.connection_string = connection_string or DatabaseConfig.get_mssql_connection_string()
        self.conn = None
    
    def connect(self):
        """Establish connection to MS SQL Server."""
        try:
            self.conn = pyodbc.connect(self.connection_string)
            logger.info("Connected to MS SQL Server")
        except Exception as e:
            logger.error(f"Failed to connect to MS SQL Server: {e}")
            raise
    
    def disconnect(self):
        """Close connection to MS SQL Server."""
        if self.conn:
            self.conn.close()
            logger.info("Disconnected from MS SQL Server")
    
    def get_table_list(self) -> List[str]:
        """
        Get list of all user tables in the database.
        
        Returns:
            List of table names
        """
        if not self.conn:
            self.connect()
        
        cursor = self.conn.cursor()
        cursor.execute("""
            SELECT TABLE_NAME 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE = 'BASE TABLE'
            AND TABLE_SCHEMA = 'dbo'
            ORDER BY TABLE_NAME
        """)
        
        tables = [row[0] for row in cursor.fetchall()]
        cursor.close()
        return tables
    
    def get_table_columns(self, table_name: str) -> List[Dict[str, any]]:
        """
        Get column information for a table.
        
        Args:
            table_name: Name of the table
            
        Returns:
            List of column information dictionaries
        """
        if not self.conn:
            self.connect()
        
        cursor = self.conn.cursor()
        cursor.execute("""
            SELECT 
                COLUMN_NAME,
                DATA_TYPE,
                CHARACTER_MAXIMUM_LENGTH,
                NUMERIC_PRECISION,
                NUMERIC_SCALE,
                IS_NULLABLE,
                COLUMN_DEFAULT
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo'
            AND TABLE_NAME = ?
            ORDER BY ORDINAL_POSITION
        """, table_name)
        
        columns = []
        for row in cursor.fetchall():
            columns.append({
                'name': row[0],
                'data_type': row[1],
                'max_length': row[2],
                'precision': row[3],
                'scale': row[4],
                'is_nullable': row[5] == 'YES',
                'default': row[6]
            })
        
        cursor.close()
        return columns
    
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
        
        cursor = self.conn.cursor()
        cursor.execute(f"SELECT COUNT(*) FROM [dbo].[{table_name}]")
        count = cursor.fetchone()[0]
        cursor.close()
        return count
    
    def extract_table_data(self, table_name: str, batch_size: int = 1000) -> Iterator[List[tuple]]:
        """
        Extract data from a table in batches.
        
        Args:
            table_name: Name of the table
            batch_size: Number of rows per batch
            
        Yields:
            Batches of rows as lists of tuples
        """
        if not self.conn:
            self.connect()
        
        cursor = self.conn.cursor()
        
        # Get column information for type conversion
        columns = self.get_table_columns(table_name)
        
        # Build SELECT query
        column_names = [col['name'] for col in columns]
        query = f"SELECT [{'], ['.join(column_names)}] FROM [dbo].[{table_name}]"
        
        cursor.execute(query)
        
        batch = []
        row_count = 0
        
        while True:
            rows = cursor.fetchmany(batch_size)
            if not rows:
                break
            
            # Convert values according to data types
            converted_rows = []
            for row in rows:
                converted_row = []
                for i, col in enumerate(columns):
                    value = row[i]
                    converted_value = TypeMapper.convert_value(value, col['data_type'])
                    converted_row.append(converted_value)
                converted_rows.append(tuple(converted_row))
            
            batch.extend(converted_rows)
            row_count += len(rows)
            
            if len(batch) >= batch_size:
                yield batch
                batch = []
        
        # Yield remaining rows
        if batch:
            yield batch
        
        cursor.close()
        logger.info(f"Extracted {row_count} rows from {table_name}")
    
    def __enter__(self):
        """Context manager entry."""
        self.connect()
        return self
    
    def __exit__(self, exc_type, exc_val, exc_tb):
        """Context manager exit."""
        self.disconnect()

