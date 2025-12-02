# Change: Add PostgreSQL Database Support

## Why

The xPort5 application currently uses MS SQL Server as its database backend. To enable cross-platform deployment, reduce licensing costs, and align with modern open-source technology stacks, we need to add PostgreSQL support. This change will allow the application to run on PostgreSQL while maintaining compatibility with existing MS SQL Server deployments during a transition period.

The migration will:
1. **Enable Platform Flexibility**: PostgreSQL runs on Linux, Windows, and macOS, enabling broader deployment options
2. **Reduce Licensing Costs**: PostgreSQL is open-source and free, reducing database licensing expenses
3. **Modernize Technology Stack**: Aligns with the long-term vision of migrating to Python/FastAPI backend with PostgreSQL
4. **Maintain Compatibility**: Existing projects (xPort5, xPort5.EF6, xPort5.Common, xPort5.Bot) will continue to work with minimal changes

## What Changes

- **NEW**: Create `xPort5.Pgsql` project containing PostgreSQL-specific database migration tools and scripts
- **NEW**: Create Python data migration program to extract data from MS SQL Server and import into PostgreSQL
- **NEW**: Generate PostgreSQL DDL scripts from existing MS SQL Server schema (tables, views, constraints, indexes)
- **MODIFIED**: Update Entity Framework 6 configuration to support PostgreSQL via Npgsql provider
- **MODIFIED**: Update connection string configurations to support PostgreSQL connection strings
- **PRESERVED**: Table names and column names remain unchanged to minimize application code changes
- **NOT MIGRATED**: Stored procedures will not be migrated (as per requirements)
- **CONDITIONAL**: Database views will be migrated if PostgreSQL-compatible syntax can be achieved

## Impact

### Affected Specs
- `database-migration`: New capability for PostgreSQL database migration and support

### Affected Code
- **New Project**: `xPort5.Pgsql` - PostgreSQL migration utilities and scripts
- **Configuration Files**: 
  - `xPort5/Web.config` - Connection string updates
  - `xPort5.EF6/App.config` - EF6 provider configuration
  - `xPort5.Bot/Web.config` - Connection string updates
  - `xPort5.Common/Config.cs` - Connection string handling
- **Entity Framework**: 
  - `xPort5.EF6/xPort5Model.edmx` - May require provider-specific adjustments
  - `xPort5.EF6/xPort5Model.Context.cs` - Connection string handling
- **Data Access**: Minimal changes expected due to EF6 abstraction layer

### Migration Strategy
1. **Phase 1**: Schema Migration
   - Extract MS SQL Server schema (tables, columns, constraints, indexes)
   - Convert to PostgreSQL-compatible DDL
   - Preserve table and column names
   - Handle data type mappings (uniqueidentifier → uuid, datetime → timestamp, etc.)
   - Migrate views where PostgreSQL syntax is compatible

2. **Phase 2**: Data Migration Tool
   - Develop Python script to extract data from MS SQL Server
   - Transform data types as needed
   - Import data into PostgreSQL
   - Handle referential integrity and foreign key constraints
   - Support incremental migration and data validation

3. **Phase 3**: Application Configuration
   - Add Npgsql provider to EF6 configuration
   - Update connection string handling to support both MS SQL and PostgreSQL
   - Test with existing application code (xPort5, xPort5.EF6, xPort5.Common, xPort5.Bot)

4. **Phase 4**: Testing and Validation
   - Verify all CRUD operations work with PostgreSQL
   - Test view queries and complex queries
   - Validate data integrity and referential constraints
   - Performance benchmarking (MS SQL vs PostgreSQL)

