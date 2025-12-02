# Change: Convert OLAP Stored Procedures to LINQ Queries

## Why

The xPort5 application currently uses 8 OLAP stored procedures (`olap_*`) for reporting functionality. These stored procedures are critical blockers for PostgreSQL migration, as stored procedures will not be migrated to PostgreSQL. Converting these procedures to application-level LINQ queries will:

1. **Enable PostgreSQL Migration**: Remove dependency on database-specific stored procedures
2. **Improve Maintainability**: LINQ queries are easier to understand, test, and maintain than T-SQL stored procedures
3. **Better Compatibility**: Application-level queries work identically with both MS SQL Server and PostgreSQL via EF6
4. **Align with Architecture**: Supports the ongoing migration from DAL stored procedures to EF6 LINQ queries

## What Changes

- **MODIFIED**: Replace stored procedure calls with LINQ queries in 8 OLAP reporting pages
- **NEW**: Create `OlapService` class in `xPort5.EF6` project to encapsulate OLAP query logic
- **MODIFIED**: Update OLAP reporting pages (`*_v5.aspx.cs`) to use LINQ queries instead of stored procedures
- **PRESERVED**: All report functionality, data structure, and user interface remain unchanged
- **REMOVED**: Dependency on 8 OLAP stored procedures (`olap_InvoiceSummary`, `olap_MonthlyInvoiceSummary`, `olap_OSOrder`, `olap_OSProfit`, `olap_SalesContract`, `olap_SalesTurnover`, `olap_ShipmentSummary`, `olap_Top10Supplier`)

## Impact

### Affected Specs
- `olap-reporting`: Modified capability for OLAP reporting using LINQ queries instead of stored procedures

### Affected Code
- **New Service Class**: `xPort5.EF6/OlapService.cs` - Contains LINQ query methods for all OLAP reports
- **Modified Pages**: 
  - `xPort5/Admin/Olap/InvoiceSummary_v5.aspx.cs`
  - `xPort5/Admin/Olap/InvoiceSummaryByMonth_v5.aspx.cs`
  - `xPort5/Admin/Olap/OutstandingOrderSummary_v5.aspx.cs`
  - `xPort5/Admin/Olap/OutstandingProfitSummary_v5.aspx.cs`
  - `xPort5/Admin/Olap/SalesContract_v5.aspx.cs`
  - `xPort5/Admin/Olap/SalesTurnover_v5.aspx.cs`
  - `xPort5/Admin/Olap/ShipmentSummary_v5.aspx.cs`
  - `xPort5/Admin/Olap/Top10Suppliers_v5.aspx.cs`
- **Helper Infrastructure**: 
  - `xPort5.EF6/SqlHelper.Compatibility.cs` - May need updates for DataSet creation from LINQ results

### Migration Strategy

1. **Phase 1**: Create OLAP Service Infrastructure
   - Create `OlapService` class with EF6 DbContext access
   - Implement helper methods for common OLAP query patterns
   - Create DataSet conversion utilities for backward compatibility

2. **Phase 2**: Convert Stored Procedures to LINQ
   - Convert each of the 8 OLAP procedures one by one
   - Test each conversion with sample data
   - Ensure data structure matches stored procedure output exactly

3. **Phase 3**: Update ASPX Pages
   - Replace `SqlHelper.Default.ExecuteDataSet("olap_*", param)` calls
   - Update to use `OlapService` methods
   - Maintain existing parameter handling and DataSet structure

4. **Phase 4**: Testing and Validation
   - Compare LINQ query results with stored procedure results
   - Test all OLAP reports with production data samples
   - Verify performance is acceptable (target: <10% slower than stored procedures)
   - Test with both MS SQL Server and PostgreSQL (when available)
