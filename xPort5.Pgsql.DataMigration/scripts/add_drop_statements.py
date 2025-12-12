"""
Add DROP VIEW IF EXISTS before each CREATE OR REPLACE VIEW statement
"""

import re
from pathlib import Path

def add_drop_statements(sql_file_path: str):
    """Add DROP VIEW IF EXISTS before each CREATE OR REPLACE VIEW."""
    
    print(f"Processing: {sql_file_path}")
    
    # Read the file
    with open(sql_file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Pattern to match CREATE OR REPLACE VIEW statements
    # Captures the view name
    pattern = r'-- (vw\w+|olap)\s*\nCREATE OR REPLACE VIEW (\w+) AS'
    
    def add_drop(match):
        comment = match.group(1)
        view_name = match.group(2)
        return f'-- {comment}\nDROP VIEW IF EXISTS {view_name} CASCADE;\nCREATE OR REPLACE VIEW {view_name} AS'
    
    # Replace all occurrences
    new_content = re.sub(pattern, add_drop, content)
    
    # Count how many replacements were made
    original_count = len(re.findall(pattern, content))
    
    # Write back to file
    with open(sql_file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    
    print(f"✅ Added DROP VIEW IF EXISTS to {original_count} views")
    return original_count

if __name__ == '__main__':
    script_dir = Path(__file__).parent
    sql_file = script_dir.parent / 'schema' / 'xport5_pgsql_views.sql'
    
    count = add_drop_statements(str(sql_file))
    print(f"\n✅ Successfully processed {count} views")
