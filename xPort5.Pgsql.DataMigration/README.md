# xPort5.Pgsql.DataMigration - PostgreSQL Migration Tools

This project contains tools and scripts to migrate the xPort5 database from MS SQL Server to PostgreSQL.

## Project Structure

```
xPort5.Pgsql.DataMigration/
├── migration/          # Python data migration scripts
├── schema/             # PostgreSQL DDL scripts
├── scripts/            # Utility scripts
└── README.md           # This file
```

## Requirements

### Python Dependencies
- Python 3.8+
- pyodbc (MS SQL Server connection)
- psycopg2-binary (PostgreSQL connection)
- python-dotenv (configuration management)
- tqdm (progress bars)

### Database Requirements
- MS SQL Server (source database)
- PostgreSQL 12+ (target database)
- ODBC Driver 17 for SQL Server (for pyodbc on Windows)

## Installation

### Install Python Dependencies

```bash
pip install -r requirements.txt
```

**Note for Windows users**: If you encounter an error installing `psycopg2-binary`, try:
```bash
pip install psycopg2-binary --only-binary :all:
```

This ensures pip uses pre-built wheels instead of trying to build from source. The `requirements.txt` file has been configured to avoid version 2.9.10 which doesn't have Windows wheels for Python 3.8.

## Usage

### 1. Schema Migration
Generate PostgreSQL DDL scripts from MS SQL Server schema:
```bash
python scripts/convert_schema.py database/xPort5_MSSQL_script.sql schema/xPort5_PostgreSQL.sql
```

### 2. Data Migration
The migration script has been enhanced to handle schema case-sensitivity differences (MSSQL CamelCase -> PostgreSQL lowercase) and provides options for handling table dependencies.

**Basic Usage:**
Navigate to the `xPort5.Pgsql.DataMigration` directory:
```bash
cd xPort5.Pgsql.DataMigration
```

Migrate all tables (converting names to lowercase):
```bash
python -m migration.migrate_data
```

**Advanced Usage:**
Control the migration order and filtering using wildcards, and get a detailed summary report:
```bash
python -m migration.migrate_data --include "Article,Customer*" --exclude "Hangfire*" --priority "T_*"
```

**Arguments:**
- `--include`: Comma-separated list of wildcard patterns for tables to *migrate* (whitelist). If omitted, all tables are included.
- `--exclude`: Comma-separated list of wildcard patterns for tables to *skip* (blacklist). Applied after inclusion.
- `--priority`: Comma-separated list of wildcard patterns for tables to migrate *first* (high priority).

**Output:**
The script now produces a formatted summary table at the end of execution:
```
====================================================================================================
| Table Name                     | Rows       | Started              | Finished             | Duration (s) |
|--------------------------------|------------|----------------------|----------------------|------------|
| T_Currency                     | 15         | 2025-12-10 07:00:00  | 2025-12-10 07:00:01  | 0.52       |
| ...
====================================================================================================
```

**Note:** The script automatically handles foreign key constraints by temporarily disabling them (setting `session_replication_role` to `replica`), allowing for circular dependencies to be loaded without error.

## Configuration

Create a `.env` file in the project root:
```env
MSSQL_SERVER=192.168.12.144
MSSQL_DATABASE=xPort3_Newish
MSSQL_USER=sa
MSSQL_PASSWORD=nx-9602

POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DATABASE=xport5
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
```

## Data Type Mappings

| MS SQL Server | PostgreSQL |
|--------------|------------|
| uniqueidentifier | uuid |
| datetime | timestamp |
| nvarchar(n) | varchar(n) |
| varchar(n) | varchar(n) |
| decimal(p,s) | numeric(p,s) |
| money | numeric(19,4) |
| bit | boolean |
| int | integer |

## Notes

- Table names and column names are preserved exactly as in MS SQL Server
- Views are migrated where PostgreSQL-compatible syntax can be achieved
- Stored procedures are NOT migrated (as per requirements)

