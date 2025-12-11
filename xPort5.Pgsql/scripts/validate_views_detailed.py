"""
Enhanced PostgreSQL Views Validation Script with detailed error reporting
"""

import psycopg2
import sys
import os
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from migration.config import DatabaseConfig


def validate_views_detailed(sql_file_path: str):
    """Validate PostgreSQL views with detailed error reporting."""
    
    db_config = DatabaseConfig.get_postgresql_connection_string()
    
    print("=" * 80)
    print("PostgreSQL Views Detailed Validation")
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
    except Exception as e:
        print(f"❌ ERROR reading SQL file: {e}")
        return False
    
    # Connect to PostgreSQL
    try:
        conn = psycopg2.connect(**db_config)
        conn.autocommit = False
        cursor = conn.cursor()
        print("✅ Connected to PostgreSQL database\n")
    except Exception as e:
        print(f"❌ ERROR connecting to database: {e}")
        return False
    
    # Split SQL into individual view statements
    view_statements = []
    current_statement = []
    in_view = False
    view_name = None
    line_number = 0
    start_line = 0
    
    for line_number, line in enumerate(sql_content.split('\n'), 1):
        stripped = line.strip()
        
        if stripped.upper().startswith('CREATE OR REPLACE VIEW'):
            in_view = True
            current_statement = [line]
            start_line = line_number
            # Extract view name
            parts = stripped.split()
            if len(parts) >= 5:
                view_name = parts[4].replace('AS', '').strip()
        elif in_view:
            current_statement.append(line)
            if stripped.endswith(';'):
                view_statements.append({
                    'name': view_name,
                    'sql': '\n'.join(current_statement),
                    'start_line': start_line,
                    'end_line': line_number
                })
                in_view = False
                current_statement = []
                view_name = None
    
    print(f"Found {len(view_statements)} view definitions\n")
    
    # Validate each view
    success_count = 0
    error_count = 0
    errors = []
    
    for idx, view_info in enumerate(view_statements, 1):
        view_name = view_info['name']
        view_sql = view_info['sql']
        start_line = view_info['start_line']
        end_line = view_info['end_line']
        
        try:
            cursor.execute(view_sql)
            print(f"✅ [{idx:2d}/{len(view_statements)}] {view_name:<40} (lines {start_line}-{end_line})")
            success_count += 1
            conn.rollback()  # Rollback after each successful test
        except Exception as e:
            error_msg = str(e)
            print(f"❌ [{idx:2d}/{len(view_statements)}] {view_name:<40} (lines {start_line}-{end_line})")
            print(f"   Error: {error_msg[:200]}")
            errors.append({
                'view': view_name,
                'error': error_msg,
                'sql': view_sql,
                'start_line': start_line,
                'end_line': end_line
            })
            error_count += 1
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
            print(f"\n❌ View: {err['view']} (lines {err['start_line']}-{err['end_line']})")
            print(f"Error: {err['error']}")
            print("-" * 80)
    
    return error_count == 0


def main():
    script_dir = Path(__file__).parent
    sql_file = script_dir.parent / 'schema' / 'xport5_pgsql_views.sql'
    
    if not sql_file.exists():
        print(f"❌ ERROR: SQL file not found: {sql_file}")
        sys.exit(1)
    
    success = validate_views_detailed(str(sql_file))
    
    if success:
        print("\n✅ All views validated successfully!")
        sys.exit(0)
    else:
        print("\n❌ Validation failed. Please fix the errors above.")
        sys.exit(1)


if __name__ == '__main__':
    main()
