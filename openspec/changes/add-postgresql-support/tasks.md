## 1. Schema Analysis and DDL Generation
- [x] 1.1 Analyze MS SQL Server schema from `database/xPort5_MSSQL_script.sql`
- [x] 1.2 Identify all tables, columns, data types, constraints, and indexes
- [x] 1.3 Map MS SQL Server data types to PostgreSQL equivalents
  - [x] uniqueidentifier → uuid
  - [x] datetime → timestamp
  - [x] nvarchar → varchar (with proper encoding)
  - [x] varchar → varchar
  - [x] decimal → numeric
  - [x] money → numeric(19,4)
  - [x] bit → boolean
  - [x] int → integer
- [x] 1.4 Generate PostgreSQL CREATE TABLE statements preserving table and column names
- [x] 1.5 Generate PostgreSQL CREATE INDEX statements
- [x] 1.6 Generate PostgreSQL FOREIGN KEY constraints
- [x] 1.7 Generate PostgreSQL PRIMARY KEY constraints
- [x] 1.8 Generate PostgreSQL CHECK constraints
- [x] 1.9 Generate PostgreSQL DEFAULT constraints

## 2. View Migration
- [x] 2.1 Extract all CREATE VIEW statements from MS SQL Server script
- [x] 2.2 Identify views that use MS SQL Server-specific syntax
- [x] 2.3 Convert compatible views to PostgreSQL syntax
  - [x] Replace MS SQL Server functions with PostgreSQL equivalents
  - [x] Handle TOP → LIMIT conversions
  - [x] Handle ISNULL → COALESCE conversions
  - [x] Handle GETDATE() → CURRENT_TIMESTAMP conversions
- [x] 2.4 Document views that cannot be migrated (if any)
- [x] 2.5 Generate PostgreSQL CREATE VIEW statements

## 3. Python Data Migration Tool
- [x] 3.1 Create `xPort5.Pgsql` project directory structure
- [x] 3.2 Set up Python project with requirements.txt
  - [x] pyodbc (MS SQL Server connection)
  - [x] psycopg2 (PostgreSQL connection)
  - [x] python-dotenv (configuration management)
- [x] 3.3 Create configuration module for database connections
- [x] 3.4 Create schema extraction module (MS SQL Server → PostgreSQL DDL)
- [x] 3.5 Create data extraction module (read from MS SQL Server)
- [x] 3.6 Create data transformation module (type conversions, data cleaning)
- [x] 3.7 Create data import module (write to PostgreSQL)
- [x] 3.8 Implement referential integrity handling (foreign key order)
- [x] 3.9 Implement batch processing for large tables
- [x] 3.10 Implement progress reporting and logging
- [x] 3.11 Implement data validation (row counts, checksums)
- [x] 3.12 Create main migration script with command-line interface

## 4. Entity Framework Configuration
- [x] 4.1 Add Npgsql.EntityFramework NuGet package to xPort5.EF6 project (documented in POSTGRESQL_CONFIGURATION.md)
- [x] 4.2 Update `App.config` and `Web.config` with Npgsql provider configuration
- [x] 4.3 Update connection string format for PostgreSQL
- [ ] 4.4 Test EF6 DbContext initialization with PostgreSQL connection
- [ ] 4.5 Verify entity mapping works correctly with PostgreSQL

## 5. Application Configuration Updates
- [ ] 5.1 Update `xPort5/Web.config` connection strings to support PostgreSQL
- [ ] 5.2 Update `xPort5.Bot/Web.config` connection strings to support PostgreSQL
- [ ] 5.3 Update `xPort5.EF6/App.config` connection strings to support PostgreSQL
- [ ] 5.4 Update `xPort5.Common/Config.cs` to handle PostgreSQL connection strings
- [ ] 5.5 Ensure connection string switching mechanism (MS SQL vs PostgreSQL)

## 6. Testing and Validation
- [ ] 6.1 Create test PostgreSQL database
- [ ] 6.2 Run schema migration scripts
- [ ] 6.3 Run Python data migration tool on test database
- [ ] 6.4 Verify table row counts match between MS SQL and PostgreSQL
- [ ] 6.5 Test CRUD operations with xPort5.EF6 against PostgreSQL
- [ ] 6.6 Test view queries against PostgreSQL
- [ ] 6.7 Test complex queries and joins
- [ ] 6.8 Test foreign key constraints and referential integrity
- [ ] 6.9 Performance testing (compare MS SQL vs PostgreSQL query performance)
- [ ] 6.10 Test xPort5 application with PostgreSQL backend
- [ ] 6.11 Test xPort5.Bot with PostgreSQL backend
- [ ] 6.12 Verify xPort5.Common works with PostgreSQL connection strings

## 7. Documentation
- [ ] 7.1 Document PostgreSQL setup requirements
- [ ] 7.2 Document connection string configuration
- [ ] 7.3 Document data migration process and steps
- [ ] 7.4 Document known limitations and differences
- [ ] 7.5 Create migration runbook for production deployment

