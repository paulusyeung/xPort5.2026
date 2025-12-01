# Performance Benchmarking Instructions

## Purpose
Compare performance between xPort5.DAL (stored procedures) and xPort5.EF6 (Entity Framework 6) to ensure EF6 is within 10% of DAL performance.

## Setup

### Prerequisites
1. Database must be accessible with test data
2. Connection strings configured in app.config
3. Both xPort5.DAL and xPort5.EF6 projects must compile

### Running the Benchmark

**Option 1: Manual Execution**
```powershell
cd c:\Projects\xPort5.2025\xPort5.EF6.PerformanceTests
dotnet run
```

**Option 2: Build and Run**
```powershell
# Build the project
msbuild xPort5.EF6.PerformanceTests.csproj /p:Configuration=Release

# Run the executable
.\bin\Release\xPort5.EF6.PerformanceTests.exe
```

## What Gets Tested

### Operations
1. **Load** - Single entity by primary key
2. **LoadCollection** - All entities or filtered collection
3. **Save** - Insert new entity (where applicable)

### Entities
1. **T_Category** (Simple) - 10 properties, no complex logic
2. **Customer** (Medium) - 21 properties, audit fields, relationships
3. **Article** (Complex) - 23 properties, full feature set

### Test Parameters
- **Warmup Iterations**: 5 (to prime caches)
- **Test Iterations**: 100 (for statistical significance)
- **Target**: EF6 within ±10% of DAL performance

## Expected Results

### Success Criteria
- ✅ All tests PASS (within 10% threshold)
- ✅ No exceptions or errors
- ✅ Both DAL and EF6 return equivalent data

### Acceptable Performance
| Operation | Expected DAL | Acceptable EF6 Range |
|-----------|--------------|---------------------|
| Load | 5-15ms | 4.5-16.5ms |
| LoadCollection (small) | 20-50ms | 18-55ms |
| LoadCollection (large) | 100-300ms | 90-330ms |
| Save | 10-30ms | 9-33ms |

## Interpreting Results

### Status Indicators
- **PASS** - EF6 within ±10% of DAL (acceptable)
- **FAIL** - EF6 outside ±10% threshold (needs investigation)
- **SKIPPED** - Test couldn't run (no data or missing dependencies)

### Performance Differences

**EF6 Faster (negative %)**
- Good! EF6 is more efficient
- Likely due to better query optimization or caching

**EF6 Slower (positive %)**
- If <10%: Acceptable overhead for ORM benefits
- If >10%: Investigate query optimization
  - Check generated SQL
  - Consider compiled queries
  - Review eager loading strategy

## Troubleshooting

### "No data in database"
- Ensure database has test data
- Run application normally to populate data
- Check connection string

### "Requires valid foreign keys"
- Save tests for Customer/Article need valid CategoryId, RegionId, etc.
- Either skip these tests or create test data with valid FKs

### Performance >10% slower
1. **Check SQL Profiler** - Compare generated queries
2. **Add Indexes** - EF6 may need different indexes than SPs
3. **Use Compiled Queries** - For frequently-used queries
4. **Disable Change Tracking** - Use `.AsNoTracking()` for read-only

### Exceptions
- Check connection strings in app.config
- Ensure both DAL and EF6 projects reference correct assemblies
- Verify database schema matches entity models

## Next Steps After Benchmarking

### If All Tests Pass (✅)
- Document results in phase0-performance-results.md
- Update confidence score
- Proceed to next Phase 0 task

### If Some Tests Fail (⚠️)
- Analyze failed tests
- Optimize EF6 queries
- Re-run benchmarks
- If still failing, reassess migration approach

### If Many Tests Fail (❌)
- Consider alternative approaches:
  - Keep stored procedures for critical paths
  - Use raw SQL for complex queries
  - Hybrid approach (EF6 for simple, SPs for complex)

## Notes

- Performance may vary based on:
  - Database size
  - Server load
  - Network latency
  - SQL Server version
  - Hardware specifications

- First run may be slower due to:
  - EF model compilation
  - SQL Server query plan generation
  - .NET JIT compilation

- Run multiple times for consistent results
