#!/usr/bin/env python3
"""Test the schema converter"""

import sys
sys.path.insert(0, 'c:/Projects/xPort5.2026/xPort5.Pgsql/scripts')

from convert_schema import SchemaConverter

def test_basic_conversions():
    converter = SchemaConverter()
    
    # Test USE statement
    result = converter.convert_line('USE [xPort5]\n')
    print(f"USE statement: '{result}'")
    assert result == '', f"Expected empty, got: {result}"
    
    # Test CREATE TABLE
    result = converter.convert_line('CREATE TABLE [dbo].[Test](\n')
    print(f"CREATE TABLE: '{result}'")
    assert 'CREATE TABLE "Test"' in result
    
    # Test data type conversion
    result = converter.convert_line('    [Id] [uniqueidentifier] NOT NULL,\n')
    print(f"Data type: '{result}'")
    assert 'uuid' in result
    
    # Test GO statement
    result = converter.convert_line('GO\n')
    print(f"GO statement: '{result}'")
    assert result == ''
    
    print("\nAll tests passed!")

if __name__ == '__main__':
    test_basic_conversions()
