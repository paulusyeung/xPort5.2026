"""
PostgreSQL Views Validation Script
Validates all views in xport5_pgsql_views.sql against live PostgreSQL database.
"""

import psycopg2
import sys
import os
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from migration.config import DatabaseConfig


def validate_views(sql_file_path: str):
    """
    Validate PostgreSQL views by attempting to create them.
    
    Args:
        sql_file_path: Path to the SQL file containing view definitions
    """
    # Get database connection parameters
    db_config = DatabaseConfig.get_postgresql_connection_string()
    
    print("=" * 80)
    print("PostgreSQL Views Validation")
    print("=" * 80)
    print(f"Database: {db_config['database']}")
    print(f"Host: {db_config['host']}")
    print(f"SQL File: {sql_file_path}")
    print("=" * 80)
    print()
    
    # Read SQL file
    try:
        with open(sql_file_path, 'r', encoding='utf-8') as f:
            sql_content = f.read()
    except FileNotFoundError:
        print(f"❌ ERROR: SQL file not found: {sql_file_path}")
        return False
    except Exception as e:
        print(f"❌ ERROR reading SQL file: {e}")
        return False
    
    # Connect to PostgreSQL
    try:
        conn = psycopg2.connect(**db_config)
        conn.autocommit = False  # Use transaction
        cursor = conn.cursor()
        print("✅ Connected to PostgreSQL database")
        print()
    except Exception as e:
        print(f"❌ ERROR connecting to database: {e}")
        return False
    
    # Split SQL into individual view statements
    view_statements = []
    current_statement = []
    in_view = False
    view_name = None
    
    for line in sql_content.split('\n'):
        stripped = line.strip()
        
        # Detect view start
        if stripped.upper().startswith('CREATE OR REPLACE VIEW'):
            in_view = True
            current_statement = [line]
            # Extract view name
            parts = stripped.split()
            if len(parts) >= 5:
                view_name = parts[4].replace('AS', '').strip()
        elif in_view:
            current_statement.append(line)
            # Detect view end (semicolon at end of line)
            if stripped.endswith(';'):
                view_statements.append({
                    'name': view_name,
                    'sql': '\n'.join(current_statement)
                })
                in_view = False
                current_statement = []
                view_name = None
    
    print(f"Found {len(view_statements)} view definitions")
    print()
    
    # Validate each view
    success_count = 0
    error_count = 0
    errors = []
    
    for idx, view_info in enumerate(view_statements, 1):
        view_name = view_info['name']
        view_sql = view_info['sql']
        
        try:
            # Try to create the view
            cursor.execute(view_sql)
            print(f"✅ [{idx}/{len(view_statements)}] {view_name}")
            success_count += 1
        except Exception as e:
            error_msg = str(e).split('\n')[0]  # Get first line of error
            print(f"❌ [{idx}/{len(view_statements)}] {view_name}")
            print(f"   Error: {error_msg}")
            errors.append({
                'view': view_name,
                'error': error_msg,
                'sql': view_sql
            })
            error_count += 1
            
            # Rollback this statement
            conn.rollback()
    
    # Rollback all changes (we're just validating, not actually creating)
    conn.rollback()
    conn.close()
    
    # Print summary
    print()
    print("=" * 80)
    print("VALIDATION SUMMARY")
    print("=" * 80)
    print(f"Total views: {len(view_statements)}")
    print(f"✅ Successful: {success_count}")
    print(f"❌ Errors: {error_count}")
    print()
    
    if errors:
        print("=" * 80)
        print("DETAILED ERRORS")
        print("=" * 80)
        for err in errors:
            print(f"\n❌ View: {err['view']}")
            print(f"Error: {err['error']}")
            print("-" * 80)
    
    return error_count == 0


def main():
    """Main entry point."""
    # Determine SQL file path
    script_dir = Path(__file__).parent
    sql_file = script_dir.parent / 'schema' / 'xport5_pgsql_views.sql'
    
    if not sql_file.exists():
        print(f"❌ ERROR: SQL file not found: {sql_file}")
        sys.exit(1)
    
    # Run validation
    success = validate_views(str(sql_file))
    
    if success:
        print("\n✅ All views validated successfully!")
        sys.exit(0)
    else:
        print("\n❌ Validation failed. Please fix the errors above.")
        sys.exit(1)


if __name__ == '__main__':
    main()
