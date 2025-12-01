## ADDED Requirements

### Requirement: Entity Framework 6 Data Access
The system SHALL use Entity Framework 6 (EF6) as the primary data access layer for all database operations.

#### Scenario: Load entity by primary key
- **WHEN** the application needs to retrieve a single entity by its primary key
- **THEN** the system SHALL use EF6's `DbContext.Find()` method or equivalent LINQ query
- **AND** the system SHALL NOT use stored procedures for simple SELECT operations

#### Scenario: Save new entity
- **WHEN** the application needs to create a new entity
- **THEN** the system SHALL use EF6's `DbContext.Add()` method
- **AND** the system SHALL call `DbContext.SaveChanges()` to persist the entity
- **AND** the system SHALL NOT use stored procedures for INSERT operations

#### Scenario: Update existing entity
- **WHEN** the application needs to modify an existing entity
- **THEN** the system SHALL attach the entity to the DbContext if detached
- **AND** the system SHALL mark the entity state as Modified
- **AND** the system SHALL call `DbContext.SaveChanges()` to persist changes
- **AND** the system SHALL NOT use stored procedures for UPDATE operations

#### Scenario: Delete entity
- **WHEN** the application needs to remove an entity
- **THEN** the system SHALL use EF6's `DbContext.Remove()` method
- **AND** the system SHALL call `DbContext.SaveChanges()` to persist the deletion
- **AND** the system SHALL NOT use stored procedures for DELETE operations

#### Scenario: Query entities with filter
- **WHEN** the application needs to retrieve multiple entities with filtering criteria
- **THEN** the system SHALL use LINQ queries against the DbContext
- **AND** the system SHALL support string-based where clauses for backward compatibility using Dynamic LINQ
- **AND** the system SHALL NOT use stored procedures for SELECT queries

### Requirement: Active Record Compatibility Layer
The system SHALL provide a compatibility layer in xPort5.EF6 that mimics the Active Record pattern of the legacy xPort5.DAL.

#### Scenario: Static Load method
- **WHEN** existing code calls `Entity.Load(id)`
- **THEN** the EF6 entity SHALL provide a static `Load(id)` method
- **AND** the method SHALL create a new DbContext instance
- **AND** the method SHALL return the entity using `DbContext.Find(id)`
- **AND** the method SHALL dispose the DbContext after use

#### Scenario: Instance Save method
- **WHEN** existing code calls `entity.Save()`
- **THEN** the EF6 entity SHALL provide an instance `Save()` method
- **AND** the method SHALL create a new DbContext instance
- **AND** the method SHALL determine if the entity is new or existing
- **AND** the method SHALL add or update the entity accordingly
- **AND** the method SHALL call `SaveChanges()` and dispose the DbContext

#### Scenario: Static Delete method
- **WHEN** existing code calls `Entity.Delete(id)`
- **THEN** the EF6 entity SHALL provide a static `Delete(id)` method
- **AND** the method SHALL create a new DbContext instance
- **AND** the method SHALL find and remove the entity
- **AND** the method SHALL call `SaveChanges()` and dispose the DbContext

#### Scenario: LoadCollection with where clause
- **WHEN** existing code calls `Entity.LoadCollection(whereClause)`
- **THEN** the EF6 entity SHALL provide a static `LoadCollection(whereClause)` method
- **AND** the method SHALL support string-based where clauses using Dynamic LINQ
- **AND** the method SHALL return a collection compatible with `BindingList<T>`

#### Scenario: LoadCombo for dropdown controls
- **WHEN** existing code calls `Entity.LoadCombo(ref comboBox, ...)`
- **THEN** the EF6 entity SHALL provide a static `LoadCombo()` method
- **AND** the method SHALL populate the ComboBox with data from EF6
- **AND** the method SHALL support locale switching (e.g., _Chs, _Cht suffixes)
- **AND** the method SHALL filter retired records by default

### Requirement: Business Logic Extraction
The system SHALL extract business logic from stored procedures and implement it in application code.

#### Scenario: Validation logic extraction
- **WHEN** a stored procedure contains validation logic
- **THEN** the validation logic SHALL be extracted to entity methods or service classes
- **AND** the validation SHALL be executed before saving entities
- **AND** the validation SHALL throw appropriate exceptions for invalid data

#### Scenario: Calculation logic extraction
- **WHEN** a stored procedure contains calculation logic (e.g., totals, pricing)
- **THEN** the calculation logic SHALL be extracted to entity methods
- **AND** the calculations SHALL be performed in C# code
- **AND** the results SHALL match the previous stored procedure behavior

#### Scenario: Complex business workflow extraction
- **WHEN** a stored procedure orchestrates complex business workflows
- **THEN** the workflow logic SHALL be extracted to service classes
- **AND** the service classes SHALL use EF6 entities for data access
- **AND** the workflow SHALL maintain transactional integrity using `TransactionScope` or `DbContext.Database.BeginTransaction()`

### Requirement: Backward Compatibility
The system SHALL maintain backward compatibility with existing Visual WebGui forms during the migration.

#### Scenario: Existing form code unchanged
- **WHEN** a form uses the legacy DAL API (e.g., `Article.Load(id)`)
- **THEN** the form code SHALL continue to work without modification after changing `using xPort5.DAL` to `using xPort5.EF6`
- **AND** the behavior SHALL be functionally equivalent to the legacy DAL

#### Scenario: Collection binding compatibility
- **WHEN** a form binds a collection to a grid or list control
- **THEN** the EF6 collection SHALL support `BindingList<T>` interface
- **AND** the collection SHALL support change notifications for UI updates

#### Scenario: Null value handling
- **WHEN** the legacy DAL uses `Guid.Empty` or `DateTime.Parse("1900-1-1")` for null values
- **THEN** the EF6 compatibility layer SHALL convert between nullable types and legacy default values
- **AND** the conversion SHALL be transparent to existing code

## REMOVED Requirements

### Requirement: Stored Procedure Data Access
**Reason**: Migrating to Entity Framework 6 eliminates the need for stored procedures for CRUD operations. Stored procedures create maintenance overhead, make business logic harder to test, and prevent effective use of ORM features.

**Migration**: All stored procedure calls for CRUD operations will be replaced with EF6 LINQ queries. Business logic embedded in stored procedures will be extracted to C# code. Stored procedures will be marked as deprecated in the database but not immediately deleted to allow for rollback if needed.

The following stored procedures will no longer be used:
- `sp[Entity]_SelRec` - Replaced by `DbContext.Find()` or LINQ queries
- `sp[Entity]_SelAll` - Replaced by LINQ queries with Dynamic LINQ for where clauses
- `sp[Entity]_InsRec` - Replaced by `DbContext.Add()` and `SaveChanges()`
- `sp[Entity]_UpdRec` - Replaced by entity state management and `SaveChanges()`
- `sp[Entity]_DelRec` - Replaced by `DbContext.Remove()` and `SaveChanges()`

### Requirement: xPort5.DAL Project Dependency
**Reason**: The xPort5.DAL project will be completely replaced by xPort5.EF6. Maintaining two data access layers creates duplication and confusion.

**Migration**: All references to xPort5.DAL will be removed from the xPort5 main application. The xPort5.DAL project will be removed from the solution after all code has been migrated to xPort5.EF6. The DAL code will be archived for reference but will no longer be part of the active codebase.
