# Database Migration to PostgreSQL

## ADDED Requirements

### Requirement: PostgreSQL Schema Support
The system SHALL support PostgreSQL as a database backend, preserving all table names and column names from the MS SQL Server schema.

#### Scenario: Schema creation
- **WHEN** PostgreSQL DDL scripts are executed
- **THEN** all tables are created with identical names and column names as MS SQL Server
- **AND** all primary keys, foreign keys, indexes, and constraints are created
- **AND** data types are mapped appropriately (uniqueidentifier → uuid, datetime → timestamp, etc.)

#### Scenario: Table name preservation
- **WHEN** a table named `OrderQT` exists in MS SQL Server
- **THEN** the PostgreSQL table is also named `OrderQT` (preserving exact case)
- **AND** all column names match exactly (e.g., `OrderQTId`, `QTNumber`, `QTDate`)

### Requirement: Database View Migration
The system SHALL migrate database views from MS SQL Server to PostgreSQL where PostgreSQL-compatible syntax can be achieved.

#### Scenario: Compatible view migration
- **WHEN** a view uses standard SQL syntax (SELECT, JOIN, WHERE, etc.)
- **THEN** the view is migrated to PostgreSQL with equivalent syntax
- **AND** MS SQL Server-specific functions are converted to PostgreSQL equivalents (ISNULL → COALESCE, GETDATE() → CURRENT_TIMESTAMP)

#### Scenario: Incompatible view handling
- **WHEN** a view uses MS SQL Server-specific syntax that cannot be converted
- **THEN** the view migration is documented as incompatible
- **AND** alternative solutions are provided (application-level queries or manual conversion)

### Requirement: Data Migration Tool
The system SHALL provide a Python program to extract data from MS SQL Server and import it into PostgreSQL.

#### Scenario: Data extraction from MS SQL Server
- **WHEN** the Python migration script is executed with MS SQL Server connection parameters
- **THEN** all table data is extracted from MS SQL Server
- **AND** data types are transformed appropriately (uniqueidentifier → uuid, datetime → timestamp, etc.)
- **AND** progress is reported for large tables

#### Scenario: Data import to PostgreSQL
- **WHEN** extracted data is imported into PostgreSQL
- **THEN** all rows are inserted maintaining referential integrity
- **AND** foreign key constraints are respected (parent tables before child tables)
- **AND** data validation is performed (row counts, checksums)

#### Scenario: Batch processing
- **WHEN** migrating large tables (e.g., > 100,000 rows)
- **THEN** data is processed in batches to avoid memory issues
- **AND** progress is reported periodically
- **AND** errors are logged without stopping the entire migration

#### Scenario: Data validation
- **WHEN** migration completes
- **THEN** row counts match between MS SQL Server and PostgreSQL for all tables
- **AND** sample data is verified for correctness
- **AND** foreign key relationships are validated

### Requirement: Entity Framework 6 PostgreSQL Support
The system SHALL configure Entity Framework 6 to work with PostgreSQL using the Npgsql provider.

#### Scenario: EF6 provider configuration
- **WHEN** Npgsql.EntityFramework package is installed
- **THEN** EF6 DbContext can connect to PostgreSQL database
- **AND** entity mappings work correctly with PostgreSQL
- **AND** CRUD operations (Create, Read, Update, Delete) function properly

#### Scenario: Connection string configuration
- **WHEN** PostgreSQL connection string is provided
- **THEN** EF6 uses Npgsql provider to connect
- **AND** connection string format follows PostgreSQL conventions (Host, Port, Database, User Id, Password)

### Requirement: Application Compatibility
The system SHALL allow xPort5, xPort5.EF6, xPort5.Common, and xPort5.Bot to run with PostgreSQL with minimal code changes.

#### Scenario: xPort5 application with PostgreSQL
- **WHEN** xPort5 application is configured with PostgreSQL connection string
- **THEN** all existing forms and controls work without modification
- **AND** data access operations (Load, Save, Delete) function correctly
- **AND** queries and views return expected results

#### Scenario: xPort5.EF6 with PostgreSQL
- **WHEN** xPort5.EF6 is configured for PostgreSQL
- **THEN** all entity classes work without modification
- **AND** Active Record compatibility layer functions correctly
- **AND** LINQ queries execute properly

#### Scenario: xPort5.Common with PostgreSQL
- **WHEN** xPort5.Common utilities access database
- **THEN** connection string handling supports PostgreSQL format
- **AND** database operations work correctly

#### Scenario: xPort5.Bot with PostgreSQL
- **WHEN** xPort5.Bot is configured with PostgreSQL connection string
- **THEN** bot functionality works without modification
- **AND** database queries execute correctly

### Requirement: Dual Database Support
The system SHALL support both MS SQL Server and PostgreSQL backends, selectable via configuration.

#### Scenario: MS SQL Server backend
- **WHEN** MS SQL Server connection string is configured
- **THEN** application uses System.Data.SqlClient provider
- **AND** all functionality works as before

#### Scenario: PostgreSQL backend
- **WHEN** PostgreSQL connection string is configured
- **THEN** application uses Npgsql provider
- **AND** all functionality works identically to MS SQL Server

#### Scenario: Connection string switching
- **WHEN** connection string is changed from MS SQL to PostgreSQL (or vice versa)
- **THEN** application reconnects to the new database
- **AND** no code changes are required

### Requirement: Stored Procedure Exclusion
The system SHALL NOT migrate stored procedures from MS SQL Server to PostgreSQL.

#### Scenario: Stored procedure handling
- **WHEN** stored procedures exist in MS SQL Server database
- **THEN** they are not included in PostgreSQL migration scripts
- **AND** application code does not depend on stored procedures for data access
- **AND** any business logic in stored procedures is handled at application level

