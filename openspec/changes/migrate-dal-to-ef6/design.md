# Design: DAL to EF6 Migration

## Context

xPort5 is a legacy .NET 4.5.2 web application built with Visual WebGui. The application currently has two data access implementations:

1. **xPort5.DAL** (Legacy):
   - Custom Active Record pattern with static `Load()`, `Save()`, `Delete()` methods
   - Uses stored procedures for all CRUD operations (e.g., `spArticle_SelRec`, `spArticle_InsRec`, `spArticle_UpdRec`, `spArticle_DelRec`)
   - Direct SqlDataReader usage for data hydration
   - Business logic often embedded in stored procedures
   - ~70 entity classes, each ~800-1000 lines of code

2. **xPort5.EF6** (Modern):
   - Entity Framework 6 with Database-First approach
   - Auto-generated entity classes from EDMX model
   - DbContext pattern for data access
   - Navigation properties for relationships
   - ~70 entity classes, each ~50-150 lines of code
   - Currently only used by xPort5.Bot

### Constraints
- Must maintain backward compatibility with existing VWG forms
- Cannot break existing functionality during migration
- Must preserve business logic currently in stored procedures
- Database schema cannot change during migration
- .NET Framework 4.5.2 limitation (no async/await in many places)

## Goals / Non-Goals

### Goals
- **Eliminate stored procedure dependency**: Move all CRUD operations to EF6
- **Standardize data access**: Single pattern across entire solution
- **Improve maintainability**: Reduce code duplication and complexity
- **Preserve business logic**: Extract and port logic from stored procedures to application code
- **Enable future migration**: Make eventual move to .NET Core/EF Core easier

### Non-Goals
- **Not changing database schema**: This is a code-only migration
- **Not rewriting UI**: VWG forms remain unchanged
- **Not adding new features**: Focus solely on migration
- **Not optimizing performance**: Performance should remain equivalent (optimization can come later)
- **Not migrating to EF Core**: Staying on EF6 for .NET Framework 4.5.2 compatibility

## Decisions

### Decision 1: Compatibility Layer Approach

**Choice**: Create a compatibility layer in xPort5.EF6 that mimics the DAL's Active Record API

**Rationale**:
- Minimizes changes to existing VWG forms
- Allows gradual migration without breaking existing code
- Reduces risk by keeping the same public API surface

**Implementation**:
```csharp
// xPort5.EF6/Article.cs (partial class extension)
public partial class Article
{
    // Static methods to mimic DAL API
    public static Article Load(Guid articleId)
    {
        using (var context = new xPort5Entities())
        {
            return context.Article.Find(articleId);
        }
    }
    
    public void Save()
    {
        using (var context = new xPort5Entities())
        {
            if (context.Entry(this).State == EntityState.Detached)
                context.Article.Add(this);
            else
                context.Entry(this).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
    
    public static void Delete(Guid articleId)
    {
        using (var context = new xPort5Entities())
        {
            var entity = context.Article.Find(articleId);
            if (entity != null)
            {
                context.Article.Remove(entity);
                context.SaveChanges();
            }
        }
    }
    
    public static ArticleCollection LoadCollection(string whereClause = null)
    {
        using (var context = new xPort5Entities())
        {
            IQueryable<Article> query = context.Article;
            if (!string.IsNullOrEmpty(whereClause))
            {
                // Parse and apply where clause using Dynamic LINQ or similar
                query = query.Where(whereClause);
            }
            return new ArticleCollection(query.ToList());
        }
    }
}
```

**Alternatives Considered**:
1. **Direct EF6 DbContext usage**: Would require rewriting all forms - too risky
2. **Repository pattern**: Adds unnecessary abstraction layer for this legacy app
3. **Keep both DAL and EF6**: Perpetuates the problem, increases maintenance burden

### Decision 2: Phased Migration by Module

**Choice**: Migrate incrementally by functional module, not by entity

**Rationale**:
- Allows thorough testing of each module before moving to next
- Reduces blast radius if issues are discovered
- Enables rollback of specific modules if needed
- Matches the application's natural boundaries

**Migration Order**:
1. Settings/Coding tables (T_* entities) - Simple, low risk
2. Partner management (Customer, Supplier, Staff) - Medium complexity
3. Product management (Article and related) - Medium complexity
4. Order management (Order*, Invoice) - High complexity, most business logic

**Alternatives Considered**:
1. **Big bang migration**: Too risky, hard to test
2. **Entity-by-entity**: Doesn't respect module boundaries, creates partial states
3. **Random order**: Misses opportunity to learn from simpler modules first

### Decision 3: Stored Procedure Business Logic Extraction

**Choice**: Extract business logic from stored procedures into C# extension methods or service classes

**Rationale**:
- Makes logic testable and maintainable
- Keeps logic close to the data model
- Allows gradual refactoring after migration

**Implementation Pattern**:
```csharp
// For simple validation/calculation logic
public partial class Article
{
    public decimal CalculateTotalCost()
    {
        // Logic previously in stored procedure
        return UnitCost * Quantity;
    }
}

// For complex business logic
public class ArticleService
{
    public void ValidateArticleForOrder(Article article, Order order)
    {
        // Complex validation logic from stored procedures
    }
}
```

**Alternatives Considered**:
1. **Keep stored procedures**: Defeats the purpose of migration
2. **Inline all logic**: Makes entities too fat, hard to test
3. **Separate business logic layer**: Over-engineering for this legacy app

### Decision 4: Where Clause Handling

**Choice**: Use Dynamic LINQ or expression trees to support string-based where clauses

**Rationale**:
- DAL extensively uses string-based where clauses (e.g., `"CategoryId = '{guid}' AND Retired = 0"`)
- Rewriting all call sites to use LINQ would be too risky
- Dynamic LINQ provides a bridge between string queries and LINQ

**Implementation**:
```csharp
using System.Linq.Dynamic.Core;

public static ArticleCollection LoadCollection(string whereClause)
{
    using (var context = new xPort5Entities())
    {
        IQueryable<Article> query = context.Article;
        if (!string.IsNullOrEmpty(whereClause))
        {
            query = query.Where(whereClause);
        }
        return new ArticleCollection(query.ToList());
    }
}
```

**Alternatives Considered**:
1. **Rewrite all where clauses to LINQ**: Too many call sites, too risky
2. **Parse SQL manually**: Reinventing the wheel, error-prone
3. **Keep using stored procedures for queries**: Defeats the purpose

## Risks / Trade-offs

### Risk 1: Performance Regression
**Mitigation**:
- Benchmark critical queries before and after migration
- Use SQL Profiler to compare generated SQL vs stored procedures
- Add indexes if EF6 generates less optimal queries
- Consider compiled queries for frequently-used queries

### Risk 2: Business Logic Loss
**Mitigation**:
- Thoroughly document all stored procedures before migration
- Create unit tests for extracted business logic
- Have domain experts review extracted logic
- Keep stored procedures in database (commented) for reference during migration

### Risk 3: Transaction Handling Changes
**Mitigation**:
- Review all multi-step operations that may rely on stored procedure transactions
- Explicitly use `TransactionScope` or `DbContext.Database.BeginTransaction()` where needed
- Test rollback scenarios thoroughly

### Risk 4: Null Handling Differences
**Mitigation**:
- DAL uses `Guid.Empty` and `DateTime.Parse("1900-1-1")` for null values
- EF6 uses `Nullable<Guid>` and `Nullable<DateTime>`
- Add null coalescing in compatibility layer to maintain behavior
- Test edge cases with null/empty values

### Trade-off: DbContext Lifetime Management
**Current Approach**: Using `using` statements for each operation (creates new context per call)
**Pros**: Simple, no state management issues, matches DAL behavior
**Cons**: May have performance impact for multiple operations
**Decision**: Start with simple approach, optimize later if needed

## Migration Plan

### Phase 0: Proof-of-Concept (Week 1)
**Goal**: Validate assumptions and build confidence before full migration

1. **Select representative entities** for prototype:
   - Simple entity: `T_Category` (basic CRUD, no complex logic)
   - Medium entity: `Customer` (has relationships, moderate complexity)
   - Complex entity: `Article` (extensive business logic, ComboBox loading)

2. **Build prototype compatibility layer**:
   - Implement `Load()`, `Save()`, `Delete()`, `LoadCollection()` for all three entities
   - Implement `LoadCombo()` for dropdown support
   - Test with actual VWG forms

3. **Performance benchmarking**:
   - Measure DAL baseline: Load, LoadCollection(100), LoadCollection(1000), Save, Delete
   - Measure EF6 equivalent operations
   - Document results and identify any >10% regressions
   - Optimize if needed (compiled queries, eager loading, etc.)

4. **Dynamic LINQ validation**:
   - Collect 10 most complex where clauses from codebase (use `rg "LoadCollection\(\"" -A 1`)
   - Test each with Dynamic LINQ
   - Document any that fail and create fallback strategy

5. **Stored procedure business logic extraction**:
   - Select one complex SP (e.g., `spOrderQT_CalcTotal` or `spArticle_ValidateForOrder`)
   - Document all logic in the SP
   - Extract to C# code
   - Create unit tests to verify behavior matches
   - Compare results with SP output on test data

6. **Integration test suite**:
   - Create tests that run same operations through DAL and EF6
   - Compare results for equality
   - Set up as regression suite for future phases

7. **Documentation**:
   - Create stored procedure inventory spreadsheet
   - Document all business logic found
   - Identify high-risk areas

8. **Go/No-Go decision**:
   - Review findings with stakeholders
   - Adjust approach if needed
   - Get approval to proceed to Phase 1

**Success Criteria**:
- ✓ All three prototype entities work correctly in VWG forms
- ✓ Performance within 10% of DAL baseline
- ✓ 90%+ of where clauses work with Dynamic LINQ
- ✓ Extracted business logic matches SP behavior
- ✓ Integration tests pass

### Phase 1: Preparation (Week 2)
1. Set up xPort5.EF6 compatibility layer infrastructure (based on Phase 0 learnings)
2. Add System.Linq.Dynamic.Core NuGet package
3. Create base classes for common patterns (LoadCollection, Save, Delete)
4. Set up integration tests for each entity type

### Phase 2: Settings Module (Week 3)
1. Migrate T_* entities (Category, Color, Currency, etc.)
2. Update Settings forms to use xPort5.EF6
3. Test all dropdown loading and CRUD operations
4. Deploy to test environment for validation

### Phase 3: Partner Management (Week 4-5)
1. Migrate Customer, Supplier, Staff entities
2. Migrate related entities (Address, Contact)
3. Update Admin forms
4. Test all partner management workflows

### Phase 4: Product Management (Week 6-7)
1. Migrate Article and related entities
2. Extract pricing and package logic from stored procedures
3. Update Product forms
4. Test product catalog operations

### Phase 5: Order Management (Week 8-11)
1. Migrate order entities (PreOrder, Quotation, SalesContract, PurchaseContract, Sample, Invoice)
2. Extract complex order processing logic from stored procedures
3. Update all order forms
4. Extensive testing of order workflows

### Phase 6: Remaining Entities (Week 12)
1. Migrate Resources, SystemInfo, UserProfile entities
2. Migrate X_* and Z_* entities
3. Update remaining forms

### Phase 7: Cleanup (Week 13)
1. Remove xPort5.DAL project from solution
2. Update project references
3. Remove unused stored procedures (or mark as deprecated)
4. Final regression testing

### Rollback Strategy
- Each phase is independent and can be rolled back
- Keep xPort5.DAL in solution until all phases complete
- Use feature flags if needed to toggle between DAL and EF6
- Maintain database stored procedures until migration is fully validated

## Open Questions

1. **Where clause complexity**: Some DAL where clauses may use SQL-specific syntax. How should we handle these?
   - **Proposed**: Start with Dynamic LINQ, fall back to raw SQL queries for complex cases

2. **ComboBox loading**: DAL has extensive ComboBox loading helpers. Should we port these exactly?
   - **Proposed**: Yes, port as-is to maintain compatibility, refactor later if needed

3. **Collection types**: DAL uses `BindingList<T>`, EF6 uses `ICollection<T>`. Do we need custom collection types?
   - **Proposed**: Create `ArticleCollection : BindingList<Article>` wrappers for compatibility

4. **Testing approach**: Should we write unit tests or rely on integration tests?
   - **Proposed**: Integration tests for data access, unit tests for extracted business logic

5. **Performance baseline**: What are acceptable performance thresholds?
   - **Proposed**: No more than 10% regression on critical paths, to be measured during Phase 1
