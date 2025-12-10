"""
Configuration module for database migration.
Handles connection strings and environment variables.
"""

import os
from dotenv import load_dotenv
from typing import Dict, Optional

# Load environment variables from .env file
load_dotenv()


class DatabaseConfig:
    """Configuration for database connections."""
    
    @staticmethod
    def get_mssql_connection_string() -> str:
        """Get MS SQL Server connection string from environment variables."""
        server = os.getenv('MSSQL_SERVER', 'localhost')
        database = os.getenv('MSSQL_DATABASE', 'xPort3_Newish')
        user = os.getenv('MSSQL_USER', 'sa')
        password = os.getenv('MSSQL_PASSWORD', '')
        
        return (
            f"DRIVER={{ODBC Driver 17 for SQL Server}};"
            f"SERVER={server};"
            f"DATABASE={database};"
            f"UID={user};"
            f"PWD={password};"
            f"TrustServerCertificate=yes;"
        )
    
    @staticmethod
    def get_postgresql_connection_string() -> Dict[str, str]:
        """Get PostgreSQL connection parameters from environment variables."""
        return {
            'host': os.getenv('POSTGRES_HOST', 'localhost'),
            'port': int(os.getenv('POSTGRES_PORT', '5432')),
            'database': os.getenv('POSTGRES_DATABASE', 'xport5'),
            'user': os.getenv('POSTGRES_USER', 'postgres'),
            'password': os.getenv('POSTGRES_PASSWORD', '')
        }
    
    @staticmethod
    def get_batch_size() -> int:
        """Get batch size for data migration."""
        return int(os.getenv('BATCH_SIZE', '1000'))
    
    @staticmethod
    def get_log_level() -> str:
        """Get log level for migration."""
        return os.getenv('LOG_LEVEL', 'INFO')

