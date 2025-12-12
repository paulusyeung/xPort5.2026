# Quick Start Guide - PostgreSQL Migration

This guide provides step-by-step instructions for migrating xPort5 from MS SQL Server to PostgreSQL.

## Prerequisites

1. **Python 3.8+** installed
2. **PostgreSQL 12+** installed and running
3. **MS SQL Server** access (source database)
4. **ODBC Driver 17 for SQL Server** installed (for pyodbc)

## Step 1: Install Python Dependencies

```bash
cd xPort5.Pgsql
pip install -r requirements.txt
```

## Step 2: Configure Database Connections

Copy the example environment file and edit it:

```bash
cp .env.example .env
# Edit .env with your database credentials
```

Edit `.env` with your actual database connection details:

```env
MSSQL_SERVER=192.168.12.144
MSSQL_DATABASE=xPort3_Newish
MSSQL_USER=sa
MSSQL_PASSWORD=your_mssql_password

POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DATABASE=xport5
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_postgres_password
```

## Step 3: Convert Schema

Convert MS SQL Server DDL script to PostgreSQL:

```bash
python scripts/convert_schema.py database/xPort5_MSSQL_script.sql schema/xPort5_PostgreSQL.sql
```

Review the generated PostgreSQL script in `schema/xPort5_PostgreSQL.sql` and make any necessary adjustments.

## Step 4: Create PostgreSQL Database and Schema

```bash
# Create database
createdb -U postgres xport5

# Run schema script
psql -U postgres -d xport5 -f schema/xPort5_PostgreSQL.sql
```

## Step 5: Migrate Data

Run the Python migration tool:

```bash
python migrate.py
```

Or with specific options:

```bash
python migrate.py --batch-size 500 --tables Article Customer OrderQT
```

## Step 6: Verify Migration

Check row counts:

```bash
# In PostgreSQL
psql -U postgres -d xport5 -c "SELECT COUNT(*) FROM \"Article\";"
psql -U postgres -d xport5 -c "SELECT COUNT(*) FROM \"Customer\";"
```

Compare with MS SQL Server:

```sql
-- In MS SQL Server
SELECT COUNT(*) FROM Article;
SELECT COUNT(*) FROM Customer;
```

## Step 7: Configure Application

See `POSTGRESQL_CONFIGURATION.md` for detailed application configuration instructions.

## Troubleshooting

### Connection Errors

**MS SQL Server connection fails:**
- Verify ODBC Driver 17 for SQL Server is installed
- Check firewall settings
- Verify credentials in `.env`

**PostgreSQL connection fails:**
- Verify PostgreSQL is running: `pg_isready`
- Check `pg_hba.conf` allows connections
- Verify credentials in `.env`

### Data Type Conversion Errors

If you encounter data type conversion errors:
1. Check `migration/type_mapper.py` for type mappings
2. Review error logs for specific column/table issues
3. Manually adjust problematic data before migration

### Foreign Key Violations

If foreign key violations occur:
1. Check table dependency order in `migration/migrate_data.py`
2. Ensure parent tables are migrated before child tables
3. Temporarily disable foreign key constraints if needed

## Next Steps

After successful migration:

1. Install Npgsql.EntityFramework NuGet package in xPort5.EF6
2. Update connection strings in Web.config/App.config
3. Test application with PostgreSQL backend
4. Update application code if needed (minimal changes expected)

## Support

For detailed information, see:
- `README.md` - Project overview
- `POSTGRESQL_CONFIGURATION.md` - Application configuration
- `openspec/changes/add-postgresql-support/` - Full specification

