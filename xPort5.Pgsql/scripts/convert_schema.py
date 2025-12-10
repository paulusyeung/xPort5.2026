#!/usr/bin/env python3
"""
Convert MS SQL Server DDL script to PostgreSQL DDL script.
Fixed version handling all edge cases.
"""

import re
import argparse
import sys
import chardet


class SchemaConverter:
    """Converts MS SQL Server DDL to PostgreSQL DDL."""
    
    # Data type mappings (order matters - more specific first)
    TYPE_MAPPINGS = {
        r'\buniqueidentifier\b': 'uuid',
        r'\bdatetime2\b': 'timestamp',
        r'\bsmalldatetime\b': 'timestamp',
        r'\bdatetime\b': 'timestamp',
        r'\bnvarchar\s*\(\s*max\s*\)': 'text',
        r'\bnvarchar\b': 'varchar',
        r'\bnchar\b': 'char',
        r'\bntext\b': 'text',
        r'\bvarchar\s*\(\s*max\s*\)': 'text',
        r'\bvarchar\b': 'varchar',
        r'\bdecimal\s*\((\d+)\s*,\s*(\d+)\)': r'numeric(\1,\2)',
        r'\bdecimal\b': 'numeric',
        r'\bmoney\b': 'numeric(19,4)',
        r'\bbit\b': 'boolean',
        r'\bint\b': 'integer',
        r'\bbigint\b': 'bigint',
        r'\bsmallint\b': 'smallint',
        r'\btinyint\b': 'smallint',
        r'\bfloat\b': 'double precision',
        r'\breal\b': 'real',
        r'\btext\b': 'text',
        r'\bimage\b': 'bytea',
        r'\bbinary\b': 'bytea',
        r'\bvarbinary\b': 'bytea',
        r'\bchar\b': 'char',
        r'\bdate\b': 'date',
        r'\btime\b': 'time',
    }
    
    # Function mappings
    FUNCTION_MAPPINGS = {
        r'\bISNULL\s*\(': 'COALESCE(',
        r'\bGETDATE\s*\(\)': 'CURRENT_TIMESTAMP',
        r'\bLEN\s*\(': 'LENGTH(',
        r'\bNEWID\s*\(\)': 'gen_random_uuid()',
        r'\bNEWSEQUENTIALID\s*\(\)': 'gen_random_uuid()',
    }
    
    def __init__(self):
        self.in_create_table = False
        self.in_view = False
        self.in_procedure = False
        self.skip_until_go = False
        self.paren_depth = 0
    
    def convert_line(self, line: str) -> str:
        """
        Convert a single line of MS SQL Server DDL to PostgreSQL.
        
        Args:
            line: Input line
            
        Returns:
            Converted line or empty string to skip
        """
        # Remove BOM if present
        if line.startswith('\ufeff'):
            line = line[1:]
        
        line_upper = line.upper().strip()
        
        # Skip until next GO if we're in a DEFAULT block
        if self.skip_until_go:
            if line_upper == 'GO':
                self.skip_until_go = False
            return ''
        
        # Skip these entirely
        if line_upper.startswith('USE ') or line_upper.startswith('USE['):
            return ''
        
        if line_upper == 'GO':
            if self.in_create_table:
                self.in_create_table = False
                self.paren_depth = 0
            if self.in_view:
                self.in_view = False  
            if self.in_procedure:
                self.in_procedure = False
            return ''
        
        if 'SET ANSI_NULLS' in line_upper or 'SET QUOTED_IDENTIFIER' in line_upper:
            return ''
        
        if 'EXEC SYS.SP_ADDEXTENDEDPROPERTY' in line_upper or 'SP_ADDEXTENDEDPROPERTY' in line_upper:
            return ''
        
        # Skip CHECK CONSTRAINT statements (not WITH CHECK ADD)
        if (line_upper.startswith('ALTER TABLE') and 
            'CHECK CONSTRAINT' in line_upper and 
            'ADD' not in line_upper and
            'WITH' not in line_upper):
            return ''
        
        # Handle CREATE DEFAULT - skip the whole block until GO
        if 'CREATE DEFAULT' in line_upper:
            self.skip_until_go = True
            return '-- ' + line.rstrip() + ' (PostgreSQL: use DEFAULT in column definition)'
        
        # Handle CREATE PROCEDURE - comment out for manual review
        if 'CREATE PROCEDURE' in line_upper or 'CREATE PROC' in line_upper:
            self.in_procedure = True
            return ('-- TODO: STORED PROCEDURE - Requires manual conversion to PostgreSQL function\n' +
                    '-- ' + line.rstrip())
        
        if self.in_procedure:
            return '-- ' + line.rstrip()
        
        # Start conversion
        converted = line
        
        # Convert brackets to quotes
        converted = re.sub(r'\[(\w+)\]', r'"\1"', converted)
        
        # Data type conversions (apply in order)
        for pattern, replacement in self.TYPE_MAPPINGS.items():
            converted = re.sub(pattern, replacement, converted, flags=re.IGNORECASE)
        
        # Function conversions
        for pattern, replacement in self.FUNCTION_MAPPINGS.items():
            converted = re.sub(pattern, replacement, converted, flags=re.IGNORECASE)
        
        # Remove CLUSTERED/NONCLUSTERED
        converted = re.sub(r'\b(NON)?CLUSTERED\b', '', converted, flags=re.IGNORECASE)
        
        # Remove ASC/DESC keywords everywhere (not just in constraints)
        converted = re.sub(r'\bASC\b', '', converted, flags=re.IGNORECASE)
        converted = re.sub(r'\bDESC\b', '', converted, flags=re.IGNORECASE)
        
        # Track CREATE TABLE
        if 'CREATE TABLE' in line_upper:
            self.in_create_table = True
            self.paren_depth = 0
        
        # Track parentheses in CREATE TABLE
        if self.in_create_table:
            self.paren_depth += converted.count('(') - converted.count(')')
        
        # Remove WITH clause - handle line that has )WITH (...)
        if self.in_create_table and ')WITH' in converted.upper():
            # Remove the WITH clause entirely
            converted = re.sub(
                r'\)\s*WITH\s*\([^)]+\)',
                ')',
                converted,
                flags=re.IGNORECASE
            )
        
        # Remove ON "PRIMARY"
        converted = re.sub(r'\s+ON\s+"PRIMARY"', '', converted, flags=re.IGNORECASE)
        
        # Handle end of CREATE TABLE
        if self.in_create_table:
            stripped = converted.strip()
            # If line is just ) and we're at depth 0 or -1, this is the final closing paren
            if stripped == ')' and self.paren_depth <= 0:
                converted = ');'
                self.in_create_table = False
                self.paren_depth = 0
            # If line ends with ); we're done
            elif stripped.endswith(');'):
                self.in_create_table = False
                self.paren_depth = 0
        
        # ALTER TABLE fixes
        if line_upper.startswith('ALTER TABLE'):
            # WITH CHECK ADD -> ADD
            converted = re.sub(r'\bWITH\s+CHECK\s+ADD\b', 'ADD', converted, flags=re.IGNORECASE)
            
            # Add semicolon to ALTER TABLE if missing
            if not converted.rstrip().endswith(';'):
                # Check if it's a constraint definition that continues on next line
                if ('FOREIGN KEY' in line_upper or 'DEFAULT' in line_upper or 
                    'CONSTRAINT' in line_upper):
                    pass  # Might continue, don't add semicolon yet
                else:
                    converted = converted.rstrip() + ';'
        
        # REFERENCES lines need semicolons
        if ('REFERENCES' in line_upper and 
            not line_upper.startswith('--') and
            not converted.rstrip().endswith(';')):
            converted = converted.rstrip() + ';'
        
        # Handle CREATE VIEW
        if 'CREATE VIEW' in line_upper:
            self.in_view = True
            converted = re.sub(
                r'CREATE\s+VIEW',
                'CREATE OR REPLACE VIEW',
                converted,
                flags=re.IGNORECASE
            )
        
        # View-specific conversions
        if self.in_view:
            # Comment out TOP clauses for manual review
            if 'TOP' in line_upper and 'SELECT' in line_upper:
                converted = re.sub(
                    r'\bTOP\s+\(?\d+\)?\s+PERCENT\b',
                    '-- TODO: Remove TOP X PERCENT',
                    converted,
                    flags=re.IGNORECASE
                )
                converted = re.sub(
                    r'\bTOP\s+\((\d+)\)',
                    r'-- TODO: Add LIMIT \1 at end',
                    converted,
                    flags=re.IGNORECASE
                )
        
        # Clean up extra spaces
        converted = re.sub(r'\s+', ' ', converted)
        converted = re.sub(r'\s+\)', ')', converted)
        converted = re.sub(r'\(\s+', '(', converted)
        
        return converted.rstrip()
    
    def detect_encoding(self, file_path: str) -> str:
        """Detect file encoding."""
        with open(file_path, 'rb') as f:
            raw_data = f.read(10000)
            
            # Check for BOM
            if raw_data.startswith(b'\xff\xfe'):
                return 'utf-16-le'
            elif raw_data.startswith(b'\xfe\xff'):
                return 'utf-16-be'
            elif raw_data.startswith(b'\xef\xbb\xbf'):
                return 'utf-8-sig'
            
            # Use chardet
            result = chardet.detect(raw_data)
            return result.get('encoding', 'utf-8') or 'utf-8'
    
    def convert_file(self, input_file: str, output_file: str):
        """Convert entire SQL file."""
        try:
            # Detect encoding
            encoding = self.detect_encoding(input_file)
            print(f"Detected encoding: {encoding}", file=sys.stderr)
            
            # Try common encodings
            encodings_to_try = [encoding, 'utf-16-le', 'utf-16', 'utf-8', 'utf-8-sig', 'cp1252', 'latin1']
            lines = None
            
            for enc in encodings_to_try:
                try:
                    with open(input_file, 'r', encoding=enc) as f_in:
                        lines = f_in.readlines()
                    print(f"Successfully read file with encoding: {enc}", file=sys.stderr)
                    break
                except (UnicodeDecodeError, UnicodeError):
                    continue
            
            if lines is None:
                raise ValueError(f"Could not decode file with any encoding")
            
            # Convert
            converted_lines = []
            converted_lines.append('-- PostgreSQL DDL converted from MS SQL Server\n')
            converted_lines.append('-- Generated by xPort5.Pgsql schema converter\n')
            converted_lines.append('-- NOTE: Stored procedures require manual review and conversion\n')
            converted_lines.append('-- NOTE: Review TOP clauses in views and convert to LIMIT\n\n')
            converted_lines.append('BEGIN;\n\n')
            
            for line in lines:
                converted = self.convert_line(line)
                if converted:  # Only add non-empty lines
                    converted_lines.append(converted + '\n')
            
            converted_lines.append('\nCOMMIT;\n')
            
            # Write
            with open(output_file, 'w', encoding='utf-8') as f_out:
                f_out.writelines(converted_lines)
            
            print(f"Conversion complete: {output_file}", file=sys.stderr)
            
        except Exception as e:
            print(f"Error converting file: {e}", file=sys.stderr)
            import traceback
            traceback.print_exc(file=sys.stderr)
            sys.exit(1)


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description='Convert MS SQL Server DDL to PostgreSQL DDL'
    )
    parser.add_argument('input', help='Input MS SQL Server script file')
    parser.add_argument('output', help='Output PostgreSQL script file')
    
    args = parser.parse_args()
    
    converter = SchemaConverter()
    converter.convert_file(args.input, args.output)


if __name__ == '__main__':
    main()
