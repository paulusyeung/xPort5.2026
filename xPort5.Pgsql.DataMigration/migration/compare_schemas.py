
import re
import os

def parse_mssql_schema(file_path):
    schema = {}
    current_table = None
    
    with open(file_path, 'r', encoding='utf-8') as f:
        for line in f:
            line = line.strip()
            # Match CREATE TABLE statement
            # Example: CREATE TABLE xPort5.dbo.Log4Net (
            table_match = re.search(r'CREATE\s+TABLE\s+xPort5\.dbo\.(\w+)', line, re.IGNORECASE)
            if table_match:
                current_table = table_match.group(1).lower()
                schema[current_table] = set()
                continue
                
            if current_table and line.startswith(')'):
                current_table = None
                continue
                
            if current_table:
                # Match column definition
                # Example: Id int IDENTITY(1,1) NOT NULL,
                # Taking the first word as column name, removing brackets
                col_match = re.match(r'\[?(\w+)\]?', line)
                if col_match:
                    col_name = col_match.group(1).lower()
                    # Ignore constraints usually at end of table def
                    if col_name not in ['constraint', 'primary', 'foreign', 'unique']:
                        schema[current_table].add(col_name)
    return schema

def parse_pgsql_schema(file_path):
    schema = {}
    current_table = None
    
    with open(file_path, 'r', encoding='utf-8') as f:
        for line in f:
            line = line.strip()
            # Match CREATE TABLE statement
            # Example: CREATE TABLE article(
            table_match = re.search(r'CREATE\s+TABLE\s+(\w+)', line, re.IGNORECASE)
            if table_match:
                current_table = table_match.group(1).lower()
                schema[current_table] = set()
                continue
                
            if current_table and line.startswith(')'):
                current_table = None
                continue
                
            if current_table:
                # Match column definition
                # Example: articleid          uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
                col_match = re.match(r'(\w+)', line)
                if col_match:
                    col_name = col_match.group(1).lower()
                    if col_name not in ['constraint', 'primary', 'foreign', 'unique']:
                        schema[current_table].add(col_name)
    return schema

def compare_schemas(mssql_path, pgsql_path):
    mssql = parse_mssql_schema(mssql_path)
    pgsql = parse_pgsql_schema(pgsql_path)
    
    print(f"Found {len(mssql)} tables in MSSQL")
    print(f"Found {len(pgsql)} tables in PostgreSQL")
    
    # Tables in MSSQL but not in PG
    missing_tables = set(mssql.keys()) - set(pgsql.keys())
    if missing_tables:
        print("\nTables in MSSQL but NOT in PostgreSQL:")
        for t in sorted(missing_tables):
            print(f"  - {t}")
            
    # Tables in PG but not in MSSQL
    extra_tables = set(pgsql.keys()) - set(mssql.keys())
    if extra_tables:
        print("\nTables in PostgreSQL but NOT in MSSQL:")
        for t in sorted(extra_tables):
            print(f"  - {t}")
            
    # Column mismatches
    print("\nColumn Mismatches:")
    common_tables = set(mssql.keys()) & set(pgsql.keys())
    for t in sorted(common_tables):
        ms_cols = mssql[t]
        pg_cols = pgsql[t]
        
        # In MS but not PG
        missing_cols = ms_cols - pg_cols
        # In PG but not MS
        extra_cols = pg_cols - ms_cols
        
        if missing_cols or extra_cols:
            print(f"\nTable: {t}")
            if missing_cols:
                print(f"  Missing in PG: {', '.join(sorted(missing_cols))}")
            if extra_cols:
                print(f"  Extra in PG:   {', '.join(sorted(extra_cols))}")

if __name__ == "__main__":
    mssql_path = r"c:\Projects\xPort5.2026\xPort5.Pgsql\schema\xPort5_MSSQL.DDL.sql"
    pgsql_path = r"c:\Projects\xPort5.2026\xPort5.Pgsql\schema\xport5_pgsql_lowercase.sql"
    compare_schemas(mssql_path, pgsql_path)
