# Design: PostgreSQL Database Migration

## Context

xPort5 is a legacy .NET Framework 4.5.2 web application using Entity Framework 6 with MS SQL Server. The application needs to support PostgreSQL to enable cross-platform deployment and reduce licensing costs. The migration must preserve table and column names to minimize application code changes.

### Current Architecture
- **Database**: MS SQL Server
- **ORM**: Entity Framework 6 (Database-First with EDMX)
- **Data Access**: xPort5.EF6 project with Active Record compatibility layer
- **Projects**: xPort5 (main app), xPort5.EF6 (data access), xPort5.Common (shared), xPort5.Bot (bot service)

### Constraints
- Must preserve table names and column names
- Must maintain compatibility with existing EF6 code
- Must support both MS SQL Server and PostgreSQL (during transition)
- Cannot break existing functionality
- .NET Framework 4.5.2 limitation (no async/await in many places)

## Goals / Non-Goals

### Goals
- **PostgreSQL Support**: Enable xPort5 to run on PostgreSQL database
- **Schema Preservation**: Keep table and column names unchanged
- **View Migration**: Migrate database views where PostgreSQL-compatible
- **Data Migration**: Provide tooling to migrate data from MS SQL to PostgreSQL
- **Minimal Code Changes**: xPort5, xPort5.EF6, xPort5.Common, xPort5.Bot should work with minimal modifications
- **Dual Support**: Support both MS SQL Server and PostgreSQL during transition period

### Non-Goals
- **Stored Procedure Migration**: Stored procedures will not be migrated (as per requirements)
- **Schema Redesign**: Not changing database schema structure or relationships
- **Application Rewrite**: Not rewriting application code, only database backend changes
- **Performance Optimization**: Focus on compatibility first, optimization can come later
- **EF Core Migration**: Staying on EF6 for .NET Framework 4.5.2 compatibility

## Decisions

### Decision: Use Npgsql Provider for EF6
**What**: Use Npgsql.EntityFramework NuGet package to enable EF6 support for PostgreSQL.

**Why**: 
- Npgsql is the official .NET data provider for PostgreSQL
- Provides full EF6 support including Code-First and Database-First
- Well-maintained and widely used
- Compatible with .NET Framework 4.5.2

**Alternatives Considered**:
- Devart dotConnect for PostgreSQL (commercial, expensive)
- Custom provider (too complex, not worth the effort)

### Decision: Python for Data Migration Tool
**What**: Create a Python script to extract data from MS SQL Server and import into PostgreSQL.

**Why**:
- Python has excellent libraries for both MS SQL (pyodbc) and PostgreSQL (psycopg2)
- Easier to handle data transformations and error handling
- Can be run independently of the .NET application
- Better for batch processing and progress reporting

**Alternatives Considered**:
- C# console application (more complex, requires .NET runtime)
- SQL Server Integration Services (SSIS) (requires SQL Server, Windows-only)
- Third-party migration tools (may not preserve exact schema)

### Decision: Preserve Table and Column Names
**What**: Keep all table names and column names exactly as they are in MS SQL Server.

**Why**:
- Minimizes application code changes
- EF6 entity classes don't need modification
- Reduces risk of breaking existing queries
- Easier to maintain dual database support

**Trade-offs**:
- PostgreSQL naming conventions (lowercase, snake_case) are not followed
- May require quoted identifiers in some cases
- Slightly less "PostgreSQL-native" but acceptable for compatibility

### Decision: Migrate Views Conditionally
**What**: Migrate database views only if they can be converted to PostgreSQL-compatible syntax.

**Why**:
- Views provide important abstractions for reporting and queries
- Some views may use MS SQL Server-specific functions that don't translate directly
- Better to document limitations than break functionality

**Migration Strategy**:
- Convert common MS SQL functions to PostgreSQL equivalents:
  - `ISNULL()` → `COALESCE()`
  - `GETDATE()` → `CURRENT_TIMESTAMP`
  - `TOP N` → `LIMIT N`
  - `LEN()` → `LENGTH()`
- Document views that cannot be migrated with alternatives

### Decision: Support Dual Database Backends
**What**: Configure application to support both MS SQL Server and PostgreSQL via connection strings.

**Why**:
- Enables gradual migration
- Allows testing PostgreSQL without disrupting production
- Provides rollback capability
- Supports different environments (dev/test/prod)

**Implementation**:
- Use connection string name or configuration flag to select database provider
- EF6 provider factory handles the differences
- Application code remains unchanged

## Risks / Trade-offs

### Risk: Data Type Mismatches
**Risk**: Some MS SQL Server data types may not map perfectly to PostgreSQL equivalents.

**Mitigation**: 
- Create comprehensive mapping table
- Test all data types with sample data
- Handle edge cases (e.g., datetime precision, decimal precision)

### Risk: View Migration Failures
**Risk**: Some views may use MS SQL Server-specific syntax that cannot be migrated.

**Mitigation**:
- Document incompatible views
- Provide alternative queries or application-level replacements
- Prioritize critical views for manual conversion

### Risk: Performance Differences
**Risk**: PostgreSQL may have different query performance characteristics than MS SQL Server.

**Mitigation**:
- Benchmark critical queries
- Optimize indexes if needed
- Document performance differences
- Consider query plan analysis

### Risk: EF6 Provider Limitations
**Risk**: Npgsql may not support all EF6 features used by the application.

**Mitigation**:
- Test all EF6 features used in the application
- Check Npgsql documentation for limitations
- Have fallback plan for unsupported features

### Risk: Connection String Complexity
**Risk**: Managing dual database support may complicate configuration.

**Mitigation**:
- Use clear naming conventions for connection strings
- Document configuration requirements
- Provide configuration examples
- Use environment-specific config files

## Migration Plan

### Phase 1: Schema Migration (Week 1-2)
1. Extract MS SQL Server schema
2. Convert to PostgreSQL DDL
3. Test schema creation on empty PostgreSQL database
4. Validate constraints and indexes

### Phase 2: Data Migration Tool (Week 2-3)
1. Develop Python migration script
2. Test on small dataset
3. Test on full dataset (non-production)
4. Validate data integrity

### Phase 3: EF6 Configuration (Week 3-4)
1. Add Npgsql provider
2. Update connection strings
3. Test EF6 operations
4. Verify entity mappings

### Phase 4: Application Testing (Week 4-5)
1. Test xPort5 application with PostgreSQL
2. Test xPort5.Bot with PostgreSQL
3. Test xPort5.Common utilities
4. Performance testing

### Phase 5: Documentation and Deployment (Week 5-6)
1. Document migration process
2. Create deployment runbook
3. Prepare production migration plan
4. Train team on PostgreSQL operations

### Rollback Plan
- Keep MS SQL Server database as backup
- Maintain connection string configuration for MS SQL
- Can switch back to MS SQL by changing connection string
- Data can be re-migrated if needed

## Open Questions

1. **Stored Procedures**: Some business logic may be in stored procedures. How will this be handled?
   - **Answer**: Per requirements, stored procedures will not be migrated. Business logic should be moved to application code if needed.

2. **Sequences vs Identity**: MS SQL Server uses IDENTITY columns. Should we use SERIAL or sequences in PostgreSQL?
   - **Decision**: Use SERIAL/BIGSERIAL for compatibility, but note that GUIDs are used as primary keys, so this may not be relevant.

3. **Case Sensitivity**: PostgreSQL is case-sensitive for identifiers. How to handle?
   - **Decision**: Use quoted identifiers to preserve exact case from MS SQL Server.

4. **Transaction Isolation**: Are there any specific transaction isolation requirements?
   - **Decision**: Use default PostgreSQL isolation level (READ COMMITTED) unless issues arise.

5. **Full-Text Search**: Does the application use full-text search features?
   - **TBD**: Need to check if MS SQL Server full-text search is used and find PostgreSQL equivalent.

