#!/usr/bin/env python3
"""
Entry point for xPort5 PostgreSQL migration.
"""

import sys
import os

# Add migration directory to path
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from migration.migrate_data import main

if __name__ == '__main__':
    main()

