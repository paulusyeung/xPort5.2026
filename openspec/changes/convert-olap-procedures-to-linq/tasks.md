# Implementation Tasks

## 1. Phase 1: Create OLAP Service Infrastructure
- [x] 1.1 Create `OlapService.cs` class in `xPort5.EF6` project
- [x] 1.2 Add static `Default` property for singleton access pattern
- [x] 1.3 Create `GetDbContext()` helper method for EF6 context access
- [x] 1.4 Create `ToDataSet<T>()` extension method to convert LINQ results to DataSet
- [x] 1.5 Create helper methods for common OLAP patterns:
  - [x] 1.5.1 Date range filtering
  - [x] 1.5.2 Customer ID array parsing (from comma-separated GUID string)
  - [x] 1.5.3 Month/Quarter/Year grouping
  - [x] 1.5.4 Currency conversion calculations

## 2. Phase 2: Convert OLAP Stored Procedures to LINQ

### 2.1 Convert olap_InvoiceSummary
- [x] 2.1.1 Analyze stored procedure logic and data flow
- [x] 2.1.2 Create `GetInvoiceSummary()` method in `OlapService`
- [x] 2.1.3 Implement LINQ query matching stored procedure logic
- [x] 2.1.4 Handle customer ID array parameter
- [x] 2.1.5 Implement month-based amount calculations (BackLogAmt, Amt1-12, Total)
- [ ] 2.1.6 Test with sample data and compare results with stored procedure
- [x] 2.1.7 Update `InvoiceSummary_v5.aspx.cs` to use new method

### 2.2 Convert olap_MonthlyInvoiceSummary
- [x] 2.2.1 Analyze stored procedure logic
- [x] 2.2.2 Create `GetMonthlyInvoiceSummary()` method
- [x] 2.2.3 Implement LINQ query with date range filtering
- [x] 2.2.4 Test and update `InvoiceSummaryByMonth_v5.aspx.cs`

### 2.3 Convert olap_OSOrder
- [x] 2.3.1 Analyze stored procedure logic
- [x] 2.3.2 Create `GetOutstandingOrderSummary()` method
- [x] 2.3.3 Implement LINQ query for outstanding orders
- [x] 2.3.4 Test and update `OutstandingOrderSummary_v5.aspx.cs`

### 2.4 Convert olap_OSProfit
- [x] 2.4.1 Analyze stored procedure logic
- [x] 2.4.2 Create `GetOutstandingProfitSummary()` method
- [x] 2.4.3 Implement LINQ query for profit calculations
- [x] 2.4.4 Test and update `OutstandingProfitSummary_v5.aspx.cs`

### 2.5 Convert olap_SalesContract
- [x] 2.5.1 Analyze stored procedure logic
- [x] 2.5.2 Create `GetSalesContractSummary()` method
- [x] 2.5.3 Implement LINQ query with date range filtering
- [x] 2.5.4 Test and update `SalesContract_v5.aspx.cs`

### 2.6 Convert olap_SalesTurnover
- [x] 2.6.1 Analyze stored procedure logic
- [x] 2.6.2 Create `GetSalesTurnover()` method
- [x] 2.6.3 Implement LINQ query with date range filtering
- [x] 2.6.4 Test and update `SalesTurnover_v5.aspx.cs`

### 2.7 Convert olap_ShipmentSummary
- [x] 2.7.1 Analyze stored procedure logic
- [x] 2.7.2 Create `GetShipmentSummary()` method
- [x] 2.7.3 Implement LINQ query with currency filtering
- [x] 2.7.4 Test and update `ShipmentSummary_v5.aspx.cs`

### 2.8 Convert olap_Top10Supplier
- [x] 2.8.1 Analyze stored procedure logic
- [x] 2.8.2 Create `GetTop10Suppliers()` method
- [x] 2.8.3 Implement LINQ query with TOP 10 ordering
- [x] 2.8.4 Test and update `Top10Suppliers_v5.aspx.cs`

## 3. Phase 3: Update ASPX Pages
- [x] 3.1 Update all 8 OLAP pages to use `OlapService` methods
- [x] 3.2 Replace `SqlHelper.Default.ExecuteDataSet()` calls
- [x] 3.3 Maintain existing parameter handling (CustomerId_Array, FromDate, ToDate, etc.)
- [x] 3.4 Ensure DataSet structure matches exactly (column names, data types)
- [x] 3.5 Verify PivotGrid binding still works correctly

## 4. Phase 4: Testing and Validation
- [ ] 4.1 Create test data set matching production scenarios
- [ ] 4.2 Compare LINQ query results with stored procedure results for each report
- [ ] 4.3 Verify row counts match exactly
- [ ] 4.4 Verify calculated amounts match (within rounding tolerance)
- [ ] 4.5 Test with various date ranges and customer filters
- [ ] 4.6 Performance testing:
  - [ ] 4.6.1 Measure LINQ query execution time
  - [ ] 4.6.2 Compare with stored procedure execution time
  - [ ] 4.6.3 Optimize if >10% slower (consider compiled queries, eager loading)
- [ ] 4.7 User acceptance testing:
  - [ ] 4.7.1 Test all 8 OLAP reports in UI
  - [ ] 4.7.2 Verify Excel export functionality
  - [ ] 4.7.3 Verify PivotGrid filtering and grouping
- [ ] 4.8 Documentation:
  - [ ] 4.8.1 Document LINQ query logic for each report
  - [ ] 4.8.2 Create migration notes for future reference

## 5. Cleanup
- [ ] 5.1 Remove unused stored procedure references from code
- [ ] 5.2 Update comments/documentation
- [ ] 5.3 Verify no other code references the 8 OLAP procedures
