## MODIFIED Requirements

### Requirement: OLAP Reporting Data Access
The system SHALL provide OLAP reporting functionality using LINQ queries instead of stored procedures.

#### Scenario: Invoice Summary Report
- **WHEN** the user requests an Invoice Summary report with customer filter
- **THEN** the system SHALL execute a LINQ query using EF6 DbContext
- **AND** the query SHALL filter invoices by customer IDs from the provided array
- **AND** the query SHALL calculate monthly amounts (BackLogAmt, Amt1-12, Total) based on invoice dates
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_InvoiceSummary` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Monthly Invoice Summary Report
- **WHEN** the user requests a Monthly Invoice Summary report with date range
- **THEN** the system SHALL execute a LINQ query filtering invoices by date range
- **AND** the query SHALL group results by month
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_MonthlyInvoiceSummary` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Outstanding Order Summary Report
- **WHEN** the user requests an Outstanding Order Summary report
- **THEN** the system SHALL execute a LINQ query to retrieve outstanding orders
- **AND** the query SHALL filter by customer IDs from the provided array
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_OSOrder` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Outstanding Profit Summary Report
- **WHEN** the user requests an Outstanding Profit Summary report
- **THEN** the system SHALL execute a LINQ query to calculate profit metrics
- **AND** the query SHALL filter by customer IDs from the provided array
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_OSProfit` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Sales Contract Summary Report
- **WHEN** the user requests a Sales Contract Summary report with date range
- **THEN** the system SHALL execute a LINQ query filtering sales contracts by date range
- **AND** the query SHALL filter by customer IDs from the provided array
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_SalesContract` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Sales Turnover Report
- **WHEN** the user requests a Sales Turnover report with date range
- **THEN** the system SHALL execute a LINQ query filtering sales by date range
- **AND** the query SHALL filter by customer IDs from the provided array
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_SalesTurnover` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Shipment Summary Report
- **WHEN** the user requests a Shipment Summary report with currency filter
- **THEN** the system SHALL execute a LINQ query filtering shipments by currency code
- **AND** the query SHALL filter by customer IDs and date range from provided parameters
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_ShipmentSummary` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

#### Scenario: Top 10 Suppliers Report
- **WHEN** the user requests a Top 10 Suppliers report with date range
- **THEN** the system SHALL execute a LINQ query ordering suppliers by purchase amount
- **AND** the query SHALL filter by supplier IDs and date range from provided parameters
- **AND** the query SHALL return the top 10 suppliers
- **AND** the query SHALL return a DataSet matching the structure previously returned by `olap_Top10Supplier` stored procedure
- **AND** the system SHALL NOT use stored procedures for this report

## ADDED Requirements

### Requirement: OLAP Service Class
The system SHALL provide an `OlapService` class in the `xPort5.EF6` project to encapsulate OLAP query logic.

#### Scenario: Service instantiation
- **WHEN** application code needs to execute OLAP queries
- **THEN** the system SHALL provide `OlapService.Default` static property for singleton access
- **AND** the service SHALL use EF6 DbContext for database access
- **AND** the service SHALL create a new DbContext instance for each query operation

#### Scenario: DataSet conversion
- **WHEN** a LINQ query returns entity results
- **THEN** the system SHALL provide a method to convert LINQ results to DataSet format
- **AND** the DataSet structure SHALL match the structure previously returned by stored procedures
- **AND** column names and data types SHALL be preserved exactly

#### Scenario: Parameter handling
- **WHEN** OLAP queries require customer ID arrays
- **THEN** the service SHALL parse comma-separated GUID strings into collections
- **AND** the service SHALL handle date range parameters (FromDate, ToDate)
- **AND** the service SHALL handle optional currency code filters

### Requirement: Backward Compatibility
The system SHALL maintain backward compatibility with existing OLAP reporting pages.

#### Scenario: Existing page code compatibility
- **WHEN** an OLAP reporting page calls the service
- **THEN** the page code SHALL continue to work with minimal changes
- **AND** the DataSet structure returned SHALL match stored procedure output exactly
- **AND** PivotGrid binding SHALL work without modification
- **AND** Excel export functionality SHALL continue to work

#### Scenario: Performance compatibility
- **WHEN** LINQ queries are executed
- **THEN** query performance SHALL be within 10% of stored procedure performance
- **AND** if performance degrades beyond 10%, optimizations SHALL be applied (compiled queries, eager loading, etc.)

## REMOVED Requirements

### Requirement: OLAP Stored Procedures
**Reason**: Stored procedures are database-specific and cannot be migrated to PostgreSQL. Application-level LINQ queries provide better compatibility and maintainability.

**Migration**: All 8 OLAP stored procedures (`olap_InvoiceSummary`, `olap_MonthlyInvoiceSummary`, `olap_OSOrder`, `olap_OSProfit`, `olap_SalesContract`, `olap_SalesTurnover`, `olap_ShipmentSummary`, `olap_Top10Supplier`) are replaced with LINQ queries in the `OlapService` class.
