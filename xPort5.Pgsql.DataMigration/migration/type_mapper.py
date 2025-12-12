"""
Data type mapping between MS SQL Server and PostgreSQL.
"""

from typing import Any, Optional
import uuid
from datetime import datetime


class TypeMapper:
    """Maps MS SQL Server data types to PostgreSQL equivalents."""
    
    # Data type mapping dictionary
    TYPE_MAPPING = {
        'uniqueidentifier': 'uuid',
        'datetime': 'timestamp',
        'nvarchar': 'varchar',
        'varchar': 'varchar',
        'decimal': 'numeric',
        'money': 'numeric(19,4)',
        'bit': 'boolean',
        'int': 'integer',
        'bigint': 'bigint',
        'smallint': 'smallint',
        'tinyint': 'smallint',
        'float': 'double precision',
        'real': 'real',
        'text': 'text',
        'ntext': 'text',
        'image': 'bytea',
        'binary': 'bytea',
        'varbinary': 'bytea',
        'char': 'char',
        'nchar': 'char',
        'date': 'date',
        'time': 'time',
        'datetime2': 'timestamp',
        'smalldatetime': 'timestamp',
        'timestamp': 'bytea',  # MS SQL timestamp is different from PostgreSQL
    }
    
    @staticmethod
    def map_sql_type_to_postgres(sql_type: str, max_length: Optional[int] = None, 
                                 precision: Optional[int] = None, 
                                 scale: Optional[int] = None) -> str:
        """
        Map MS SQL Server data type to PostgreSQL equivalent.
        
        Args:
            sql_type: MS SQL Server data type (e.g., 'nvarchar', 'decimal')
            max_length: Maximum length for varchar types
            precision: Precision for decimal/numeric types
            scale: Scale for decimal/numeric types
            
        Returns:
            PostgreSQL data type string
        """
        sql_type_lower = sql_type.lower().strip()
        
        # Handle special cases
        if sql_type_lower == 'money':
            return 'numeric(19,4)'
        
        if sql_type_lower in ['decimal', 'numeric']:
            if precision and scale:
                return f'numeric({precision},{scale})'
            elif precision:
                return f'numeric({precision})'
            else:
                return 'numeric'
        
        if sql_type_lower in ['varchar', 'nvarchar', 'char', 'nchar']:
            base_type = TypeMapper.TYPE_MAPPING.get(sql_type_lower, 'varchar')
            if max_length:
                return f'{base_type}({max_length})'
            else:
                return base_type
        
        # Default mapping
        return TypeMapper.TYPE_MAPPING.get(sql_type_lower, sql_type_lower)
    
    @staticmethod
    def convert_value(value: Any, sql_type: str) -> Any:
        """
        Convert a value from MS SQL Server format to PostgreSQL format.
        
        Args:
            value: The value to convert
            sql_type: MS SQL Server data type
            
        Returns:
            Converted value suitable for PostgreSQL
        """
        if value is None:
            return None
        
        sql_type_lower = sql_type.lower().strip()
        
        # Handle uniqueidentifier (GUID) → uuid
        if sql_type_lower == 'uniqueidentifier':
            if isinstance(value, str):
                return value  # PostgreSQL accepts GUID strings
            elif isinstance(value, uuid.UUID):
                return str(value)
            elif isinstance(value, bytes):
                return str(uuid.UUID(bytes=value))
        
        # Handle bit → boolean
        if sql_type_lower == 'bit':
            if isinstance(value, bool):
                return value
            elif isinstance(value, int):
                return bool(value)
            elif isinstance(value, str):
                return value.lower() in ('true', '1', 'yes')
        
        # Handle datetime → timestamp (PostgreSQL handles datetime the same way)
        if sql_type_lower in ['datetime', 'datetime2', 'smalldatetime']:
            if isinstance(value, datetime):
                return value
            elif isinstance(value, str):
                # Try to parse common datetime formats
                try:
                    return datetime.fromisoformat(value.replace('Z', '+00:00'))
                except:
                    return value  # Let PostgreSQL handle it
        
        # Handle money → numeric
        if sql_type_lower == 'money':
            if isinstance(value, (int, float)):
                return float(value)
            return value
        
        # Default: return as-is
        return value

