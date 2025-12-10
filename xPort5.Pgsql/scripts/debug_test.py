#!/usr/bin/env python3
import sys
try:
    from convert_schema import SchemaConverter
    print("Import successful")
    
    converter = SchemaConverter()
    print("Converter created")
    
    result = converter.convert_line('USE [xPort5]\n')
    print(f"Result: '{result}'")
    
    with open('debug_output.txt', 'w') as f:
        f.write(f"Test completed: {result}\n")
        
except Exception as e:
    with open('debug_error.txt', 'w') as f:
        f.write(f"Error: {e}\n")
        f.write(f"Type: {type(e)}\n")
        import traceback
        traceback.print_exc(file=f)
    print(f"Error: {e}")
    raise
