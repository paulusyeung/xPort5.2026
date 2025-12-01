# Change: Migrate Data Access from xPort5.DAL to xPort5.EF6

## Why

The xPort5 application currently uses two different data access layers:
- **xPort5.DAL**: Legacy custom DAL using stored procedures (used by the main xPort5 web application)
- **xPort5.EF6**: Modern Entity Framework 6 implementation (used by xPort5.Bot)

This dual approach creates several problems:
1. **Maintainability**: Two separate codebases for data access require duplicate effort for schema changes
2. **Stored Procedure Dependency**: xPort5.DAL relies heavily on stored procedures, making business logic harder to test and maintain
3. **Inconsistency**: Different patterns across the solution lead to confusion and potential bugs
4. **Technical Debt**: The legacy DAL prevents modernization efforts and makes future migration to .NET Core/EF Core more difficult

The objective is to standardize on xPort5.EF6 across the entire solution, eliminating stored procedure dependencies and improving code maintainability.

## What Changes

- Replace all `using xPort5.DAL` references with `using xPort5.EF6` across the xPort5 main application
- Update all entity instantiation and data access patterns from DAL's Active Record pattern to EF6's DbContext pattern
- Migrate business logic currently embedded in stored procedures to application code
- Remove dependency on stored procedures for CRUD operations (SELECT, INSERT, UPDATE, DELETE)
- Maintain backward compatibility with existing VWG forms during migration
- **BREAKING**: Remove xPort5.DAL project from the solution after migration is complete
- **BREAKING**: Stored procedures will no longer be used for data access (may be deprecated in database)

## Impact

### Affected Specs
- `data-access`: Core data access patterns and entity management
- `order-management`: Order-related entities (PreOrder, Quotation, SalesContract, PurchaseContract, Sample, Invoice)
- `partner-management`: Customer and Supplier entities
- `product-management`: Article and related entities
- `settings-management`: System configuration and coding tables

### Affected Code
- **Main Application**: 119+ files in `xPort5` project using `xPort5.DAL`
- **Data Access Layers**: 
  - `xPort5.DAL` (73 entity files) - to be deprecated
  - `xPort5.EF6` (79 entity files) - to be extended with compatibility layer
- **Key Modules**:
  - `xPort5/Order/*` - All order management forms
  - `xPort5/Admin/*` - Partner and staff management
  - `xPort5/Factory/*` - Factory and production management
  - `xPort5/Settings/*` - System settings and coding tables
  - `xPort5/Public/*` - Authentication and public forms

### Migration Strategy
The migration will be performed incrementally by module to minimize risk:
1. **Phase 0**: Proof-of-Concept (validate assumptions, build confidence)
   - Create prototype compatibility layer for 2-3 representative entities
   - Performance benchmarking (DAL vs EF6)
   - Test Dynamic LINQ with complex where clauses
   - Extract and test one complex stored procedure
   - Create integration test suite
2. **Phase 1**: Settings and coding tables (lowest risk, no complex business logic)
3. **Phase 2**: Partner management (Customer, Supplier, Staff)
4. **Phase 3**: Product management (Article and related entities)
5. **Phase 4**: Order management (highest complexity, most business logic)

Each phase will be tested thoroughly before proceeding to the next. Phase 0 serves as a validation gate - if the proof-of-concept reveals fundamental issues with the approach, we can adjust the strategy before committing to the full migration.
