# Implementation Tasks

## 1. Phase 0: Proof-of-Concept (Validation & Confidence Building)
- [x] 1.1 Select three representative entities for prototype (T_Category, Customer, Article)
- [x] 1.2 Add System.Linq.Dynamic.Core NuGet package to xPort5.EF6 project
- [x] 1.3 Create prototype compatibility layer for T_Category entity
  - [x] 1.3.1 Implement static `Load(Guid id)` method
  - [x] 1.3.2 Implement instance `Save()` method
  - [x] 1.3.3 Implement static `Delete(Guid id)` method
  - [x] 1.3.4 Implement static `LoadCollection(string whereClause)` method
  - [x] 1.3.5 Implement static `LoadCombo()` method with locale support
  - [x] 1.3.6 Create `T_CategoryCollection : BindingList<T_Category>` wrapper
- [x] 1.4 Create prototype compatibility layer for Customer entity
  - [x] 1.4.1 Implement all Active Record methods (Load, Save, Delete, LoadCollection)
  - [x] 1.4.2 Test with CustomerAddress and CustomerContact relationships
  - [x] 1.4.3 Create `CustomerCollection` wrapper
- [x] 1.5 Create prototype compatibility layer for Article entity
  - [x] 1.5.1 Implement all Active Record methods
  - [x] 1.5.2 Implement complex `LoadCombo()` with multiple overloads
  - [x] 1.5.3 Test locale switching (_Chs, _Cht)
  - [x] 1.5.4 Create `ArticleCollection` wrapper
- [/] 1.6 Performance benchmarking (infrastructure complete, needs Enterprise Library config)
  - [x] 1.6.1 Create benchmark test project with all test cases
  - [x] 1.6.2 Build successfully
  - [ ] 1.6.3 Fix Enterprise Library configuration for legacy DAL
  - [ ] 1.6.4 Run benchmarks and measure DAL vs EF6 performance
  - [ ] 1.6.5 Document results in spreadsheet (target: <10% regression)
  - [ ] 1.6.6 Optimize any operations >10% slower (compiled queries, eager loading, etc.)
- [x] 1.7 Dynamic LINQ validation
  - [x] 1.7.1 Search codebase for complex where clauses
  - [x] 1.7.2 Collect 10 most complex examples
  - [x] 1.7.3 Test each with Dynamic LINQ
  - [x] 1.7.4 Document results (100% success rate - no failures)
  - [x] 1.7.5 Verified 100% success rate (exceeded 90% target)
- [ ] 1.8 Stored procedure business logic extraction
  - [ ] 1.8.1 Select one complex stored procedure (e.g., `spArticle_ValidateForOrder` or `spOrderQT_CalcTotal`)
  - [ ] 1.8.2 Document all logic in the stored procedure (flowchart or pseudocode)
  - [ ] 1.8.3 Extract logic to C# method or service class
  - [ ] 1.8.4 Create unit tests for extracted logic
  - [ ] 1.8.5 Run tests with production data samples
  - [ ] 1.8.6 Verify results match stored procedure output exactly
- [ ] 1.9 Integration test suite
  - [ ] 1.9.1 Create test project: `xPort5.EF6.IntegrationTests`
  - [ ] 1.9.2 Create tests comparing DAL vs EF6 for T_Category
  - [ ] 1.9.3 Create tests comparing DAL vs EF6 for Customer
  - [ ] 1.9.4 Create tests comparing DAL vs EF6 for Article
  - [ ] 1.9.5 Test edge cases: null values, Guid.Empty, DateTime defaults
  - [ ] 1.9.6 Ensure all tests pass (100% match between DAL and EF6)
- [ ] 1.10 Stored procedure documentation
  - [ ] 1.10.1 Create spreadsheet: "Stored Procedure Inventory"
  - [ ] 1.10.2 List all sp*_SelRec, sp*_SelAll, sp*_InsRec, sp*_UpdRec, sp*_DelRec procedures
  - [ ] 1.10.3 Document business logic in each (validation, calculations, workflows)
  - [ ] 1.10.4 Rate complexity (Low/Medium/High)
  - [ ] 1.10.5 Identify high-risk procedures requiring special attention
- [ ] 1.11 Test with actual VWG forms
  - [ ] 1.11.1 Update one Settings form to use EF6 T_Category
  - [ ] 1.11.2 Update one Admin form to use EF6 Customer
  - [ ] 1.11.3 Update one Product form to use EF6 Article
  - [ ] 1.11.4 Test all CRUD operations in each form
  - [ ] 1.11.5 Test dropdown loading and filtering
  - [ ] 1.11.6 Verify UI behavior identical to DAL version
- [ ] 1.12 Go/No-Go decision
  - [ ] 1.12.1 Review all Phase 0 results
  - [ ] 1.12.2 Document any issues or concerns
  - [ ] 1.12.3 Adjust migration strategy if needed
  - [ ] 1.12.4 Get stakeholder approval to proceed to Phase 1

## 2. Phase 1: Preparation and Infrastructure
- [x] 2.1 Generalize compatibility layer based on Phase 0 learnings
- [x] 2.2 Create base classes for common Active Record patterns
- [x] 2.3 Create collection wrapper base class
- [ ] 2.4 Set up integration test infrastructure for all entities (deferred to Phase 2+)
- [x] 2.5 Document lessons learned from Phase 0

## 3. Phase 2: Settings and Coding Tables Migration
- [x] 3.1 Add compatibility methods to T_Category entity (Load, Save, Delete, LoadCollection, LoadCombo)
- [x] 3.2 Add compatibility methods to T_Color entity
- [x] 3.3 Add compatibility methods to T_Currency entity
- [x] 3.4 Add compatibility methods to remaining T_* entities (AgeGrading, Charge, City, Class, Country, Dept, Division, Group, Origin, Package, PaymentTerms, Port, Province, Region, Remarks, ShippingMark, UnitOfMeasures)
- [ ] 3.5 Update Settings forms to use `xPort5.EF6` instead of `xPort5.DAL` (deferred to actual migration)
- [ ] 3.6 Test all dropdown loading in Settings module (deferred to actual migration)
- [ ] 3.7 Test CRUD operations for all coding tables (deferred to actual migration)
- [ ] 3.8 Deploy to test environment and validate (deferred to actual migration)

## 4. Phase 3: Partner Management Migration
- [x] 4.1 Add compatibility methods to Customer entity
- [x] 4.2 Add compatibility methods to CustomerAddress entity
- [x] 4.3 Add compatibility methods to CustomerContact entity
- [x] 4.4 Add compatibility methods to Supplier entity
- [x] 4.5 Add compatibility methods to SupplierAddress entity
- [x] 4.6 Add compatibility methods to SupplierContact entity
- [x] 4.7 Add compatibility methods to Staff entity
- [x] 4.8 Add compatibility methods to StaffAddress entity
- [ ] 4.9 Update Admin/Partner forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 4.10 Test customer management workflows (deferred to actual migration)
- [ ] 4.11 Test supplier management workflows (deferred to actual migration)
- [ ] 4.12 Test staff management workflows (deferred to actual migration)
- [ ] 4.13 Deploy to test environment and validate (deferred to actual migration)

## 5. Phase 4: Product Management Migration
- [x] 5.1 Add compatibility methods to Article entity
- [x] 5.2 Add compatibility methods to ArticleCustomer entity
- [x] 5.3 Add compatibility methods to ArticleSupplier entity
- [x] 5.4 Add compatibility methods to ArticlePrice entity
- [x] 5.5 Add compatibility methods to ArticlePackage entity
- [x] 5.6 Add compatibility methods to ArticleKeyPicture entity
- [ ] 5.7 Extract pricing and package logic from stored procedures (deferred)
- [ ] 5.8 Update Product forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 5.9 Test product catalog operations (deferred to actual migration)
- [ ] 5.10 Test article pricing and package management (deferred to actual migration)
- [ ] 5.11 Deploy to test environment and validate (deferred to actual migration)

## 6. Phase 5: Order Management Migration
- [x] 6.1 Add compatibility methods to OrderPL (Pre-Order) entity
- [x] 6.2 Add compatibility methods to OrderPLItems entity
- [x] 6.3 Add compatibility methods to OrderQT (Quotation) entity
- [x] 6.4 Add compatibility methods to OrderQTItems entity
- [x] 6.5 Add compatibility methods to OrderQTPackage, OrderQTSupplier, OrderQTCustShipping, OrderQTSuppShipping entities
- [x] 6.6 Add compatibility methods to OrderSC (Sales Contract) entity
- [x] 6.7 Add compatibility methods to OrderSCItems entity
- [x] 6.8 Add compatibility methods to OrderPC (Purchase Contract) entity
- [x] 6.9 Add compatibility methods to OrderPCItems entity
- [x] 6.10 Add compatibility methods to OrderSP (Sample) entity
- [x] 6.11 Add compatibility methods to OrderSPItems entity
- [x] 6.12 Add compatibility methods to OrderIN (Invoice) entity
- [x] 6.13 Add compatibility methods to OrderINItems, OrderINCharges, OrderINShipment entities
- [x] 6.14 Add compatibility methods to OrderPK (Packing) entity
- [x] 6.15 Add compatibility methods to OrderPKItems entity
- [ ] 6.16 Extract order processing business logic from stored procedures (deferred)
- [ ] 6.17 Extract order validation logic from stored procedures (deferred)
- [ ] 6.18 Extract order calculation logic (totals, charges, etc.) from stored procedures (deferred)
- [ ] 6.19 Update PreOrder forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.20 Update Quotation forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.21 Update SalesContract forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.22 Update PurchaseContract forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.23 Update Sample forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.24 Update Invoice forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.25 Test complete order lifecycle (PreOrder → Quotation → SalesContract → Invoice) (deferred to actual migration)
- [ ] 6.26 Test purchase order workflow (deferred to actual migration)
- [ ] 6.27 Test sample order workflow (deferred to actual migration)
- [ ] 6.28 Test order item re-sequencing (verify fix from previous change) (deferred to actual migration)
- [ ] 6.29 Deploy to test environment and validate (deferred to actual migration)

## 7. Phase 6: Remaining Entities Migration
- [x] 7.1 Add compatibility methods to Resources entity
- [x] 7.2 Add compatibility methods to SystemInfo entity
- [x] 7.3 Add compatibility methods to UserProfile entity
- [x] 7.4 Add compatibility methods to UserDisplayPreference entity
- [x] 7.5 Add compatibility methods to X_* entities (AppPath, Counter, ErrorLog, EventLog)
# Implementation Tasks

## 1. Phase 0: Proof-of-Concept (Validation & Confidence Building)
- [x] 1.1 Select three representative entities for prototype (T_Category, Customer, Article)
- [x] 1.2 Add System.Linq.Dynamic.Core NuGet package to xPort5.EF6 project
- [x] 1.3 Create prototype compatibility layer for T_Category entity
  - [x] 1.3.1 Implement static `Load(Guid id)` method
  - [x] 1.3.2 Implement instance `Save()` method
  - [x] 1.3.3 Implement static `Delete(Guid id)` method
  - [x] 1.3.4 Implement static `LoadCollection(string whereClause)` method
  - [x] 1.3.5 Implement static `LoadCombo()` method with locale support
  - [x] 1.3.6 Create `T_CategoryCollection : BindingList<T_Category>` wrapper
- [x] 1.4 Create prototype compatibility layer for Customer entity
  - [x] 1.4.1 Implement all Active Record methods (Load, Save, Delete, LoadCollection)
  - [x] 1.4.2 Test with CustomerAddress and CustomerContact relationships
  - [x] 1.4.3 Create `CustomerCollection` wrapper
- [x] 1.5 Create prototype compatibility layer for Article entity
  - [x] 1.5.1 Implement all Active Record methods
  - [x] 1.5.2 Implement complex `LoadCombo()` with multiple overloads
  - [x] 1.5.3 Test locale switching (_Chs, _Cht)
  - [x] 1.5.4 Create `ArticleCollection` wrapper
- [/] 1.6 Performance benchmarking (infrastructure complete, needs Enterprise Library config)
  - [x] 1.6.1 Create benchmark test project with all test cases
  - [x] 1.6.2 Build successfully
  - [ ] 1.6.3 Fix Enterprise Library configuration for legacy DAL
  - [ ] 1.6.4 Run benchmarks and measure DAL vs EF6 performance
  - [ ] 1.6.5 Document results in spreadsheet (target: <10% regression)
  - [ ] 1.6.6 Optimize any operations >10% slower (compiled queries, eager loading, etc.)
- [x] 1.7 Dynamic LINQ validation
  - [x] 1.7.1 Search codebase for complex where clauses
  - [x] 1.7.2 Collect 10 most complex examples
  - [x] 1.7.3 Test each with Dynamic LINQ
  - [x] 1.7.4 Document results (100% success rate - no failures)
  - [x] 1.7.5 Verified 100% success rate (exceeded 90% target)
- [ ] 1.8 Stored procedure business logic extraction
  - [ ] 1.8.1 Select one complex stored procedure (e.g., `spArticle_ValidateForOrder` or `spOrderQT_CalcTotal`)
  - [ ] 1.8.2 Document all logic in the stored procedure (flowchart or pseudocode)
  - [ ] 1.8.3 Extract logic to C# method or service class
  - [ ] 1.8.4 Create unit tests for extracted logic
  - [ ] 1.8.5 Run tests with production data samples
  - [ ] 1.8.6 Verify results match stored procedure output exactly
- [ ] 1.9 Integration test suite
  - [ ] 1.9.1 Create test project: `xPort5.EF6.IntegrationTests`
  - [ ] 1.9.2 Create tests comparing DAL vs EF6 for T_Category
  - [ ] 1.9.3 Create tests comparing DAL vs EF6 for Customer
  - [ ] 1.9.4 Create tests comparing DAL vs EF6 for Article
  - [ ] 1.9.5 Test edge cases: null values, Guid.Empty, DateTime defaults
  - [ ] 1.9.6 Ensure all tests pass (100% match between DAL and EF6)
- [ ] 1.10 Stored procedure documentation
  - [ ] 1.10.1 Create spreadsheet: "Stored Procedure Inventory"
  - [ ] 1.10.2 List all sp*_SelRec, sp*_SelAll, sp*_InsRec, sp*_UpdRec, sp*_DelRec procedures
  - [ ] 1.10.3 Document business logic in each (validation, calculations, workflows)
  - [ ] 1.10.4 Rate complexity (Low/Medium/High)
  - [ ] 1.10.5 Identify high-risk procedures requiring special attention
- [ ] 1.11 Test with actual VWG forms
  - [ ] 1.11.1 Update one Settings form to use EF6 T_Category
  - [ ] 1.11.2 Update one Admin form to use EF6 Customer
  - [ ] 1.11.3 Update one Product form to use EF6 Article
  - [ ] 1.11.4 Test all CRUD operations in each form
  - [ ] 1.11.5 Test dropdown loading and filtering
  - [ ] 1.11.6 Verify UI behavior identical to DAL version
- [ ] 1.12 Go/No-Go decision
  - [ ] 1.12.1 Review all Phase 0 results
  - [ ] 1.12.2 Document any issues or concerns
  - [ ] 1.12.3 Adjust migration strategy if needed
  - [ ] 1.12.4 Get stakeholder approval to proceed to Phase 1

## 2. Phase 1: Preparation and Infrastructure
- [x] 2.1 Generalize compatibility layer based on Phase 0 learnings
- [x] 2.2 Create base classes for common Active Record patterns
- [x] 2.3 Create collection wrapper base class
- [ ] 2.4 Set up integration test infrastructure for all entities (deferred to Phase 2+)
- [x] 2.5 Document lessons learned from Phase 0

## 3. Phase 2: Settings and Coding Tables Migration
- [x] 3.1 Add compatibility methods to T_Category entity (Load, Save, Delete, LoadCollection, LoadCombo)
- [x] 3.2 Add compatibility methods to T_Color entity
- [x] 3.3 Add compatibility methods to T_Currency entity
- [x] 3.4 Add compatibility methods to remaining T_* entities (AgeGrading, Charge, City, Class, Country, Dept, Division, Group, Origin, Package, PaymentTerms, Port, Province, Region, Remarks, ShippingMark, UnitOfMeasures)
- [ ] 3.5 Update Settings forms to use `xPort5.EF6` instead of `xPort5.DAL` (deferred to actual migration)
- [ ] 3.6 Test all dropdown loading in Settings module (deferred to actual migration)
- [ ] 3.7 Test CRUD operations for all coding tables (deferred to actual migration)
- [ ] 3.8 Deploy to test environment and validate (deferred to actual migration)

## 4. Phase 3: Partner Management Migration
- [x] 4.1 Add compatibility methods to Customer entity
- [x] 4.2 Add compatibility methods to CustomerAddress entity
- [x] 4.3 Add compatibility methods to CustomerContact entity
- [x] 4.4 Add compatibility methods to Supplier entity
- [x] 4.5 Add compatibility methods to SupplierAddress entity
- [x] 4.6 Add compatibility methods to SupplierContact entity
- [x] 4.7 Add compatibility methods to Staff entity
- [x] 4.8 Add compatibility methods to StaffAddress entity
- [ ] 4.9 Update Admin/Partner forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 4.10 Test customer management workflows (deferred to actual migration)
- [ ] 4.11 Test supplier management workflows (deferred to actual migration)
- [ ] 4.12 Test staff management workflows (deferred to actual migration)
- [ ] 4.13 Deploy to test environment and validate (deferred to actual migration)

## 5. Phase 4: Product Management Migration
- [x] 5.1 Add compatibility methods to Article entity
- [x] 5.2 Add compatibility methods to ArticleCustomer entity
- [x] 5.3 Add compatibility methods to ArticleSupplier entity
- [x] 5.4 Add compatibility methods to ArticlePrice entity
- [x] 5.5 Add compatibility methods to ArticlePackage entity
- [x] 5.6 Add compatibility methods to ArticleKeyPicture entity
- [ ] 5.7 Extract pricing and package logic from stored procedures (deferred)
- [ ] 5.8 Update Product forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 5.9 Test product catalog operations (deferred to actual migration)
- [ ] 5.10 Test article pricing and package management (deferred to actual migration)
- [ ] 5.11 Deploy to test environment and validate (deferred to actual migration)

## 6. Phase 5: Order Management Migration
- [x] 6.1 Add compatibility methods to OrderPL (Pre-Order) entity
- [x] 6.2 Add compatibility methods to OrderPLItems entity
- [x] 6.3 Add compatibility methods to OrderQT (Quotation) entity
- [x] 6.4 Add compatibility methods to OrderQTItems entity
- [x] 6.5 Add compatibility methods to OrderQTPackage, OrderQTSupplier, OrderQTCustShipping, OrderQTSuppShipping entities
- [x] 6.6 Add compatibility methods to OrderSC (Sales Contract) entity
- [x] 6.7 Add compatibility methods to OrderSCItems entity
- [x] 6.8 Add compatibility methods to OrderPC (Purchase Contract) entity
- [x] 6.9 Add compatibility methods to OrderPCItems entity
- [x] 6.10 Add compatibility methods to OrderSP (Sample) entity
- [x] 6.11 Add compatibility methods to OrderSPItems entity
- [x] 6.12 Add compatibility methods to OrderIN (Invoice) entity
- [x] 6.13 Add compatibility methods to OrderINItems, OrderINCharges, OrderINShipment entities
- [x] 6.14 Add compatibility methods to OrderPK (Packing) entity
- [x] 6.15 Add compatibility methods to OrderPKItems entity
- [ ] 6.16 Extract order processing business logic from stored procedures (deferred)
- [ ] 6.17 Extract order validation logic from stored procedures (deferred)
- [ ] 6.18 Extract order calculation logic (totals, charges, etc.) from stored procedures (deferred)
- [ ] 6.19 Update PreOrder forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.20 Update Quotation forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.21 Update SalesContract forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.22 Update PurchaseContract forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.23 Update Sample forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.24 Update Invoice forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 6.25 Test complete order lifecycle (PreOrder → Quotation → SalesContract → Invoice) (deferred to actual migration)
- [ ] 6.26 Test purchase order workflow (deferred to actual migration)
- [ ] 6.27 Test sample order workflow (deferred to actual migration)
- [ ] 6.28 Test order item re-sequencing (verify fix from previous change) (deferred to actual migration)
- [ ] 6.29 Deploy to test environment and validate (deferred to actual migration)

## 7. Phase 6: Remaining Entities Migration
- [x] 7.1 Add compatibility methods to Resources entity
- [x] 7.2 Add compatibility methods to SystemInfo entity
- [x] 7.3 Add compatibility methods to UserProfile entity
- [x] 7.4 Add compatibility methods to UserDisplayPreference entity
- [x] 7.5 Add compatibility methods to X_* entities (AppPath, Counter, ErrorLog, EventLog)
- [x] 7.6 Add compatibility methods to Z_* entities (Address, Email, JobTitle, Phone, Salutation)
- [ ] 7.7 Update remaining forms to use `xPort5.EF6` (deferred to actual migration)
- [ ] 7.8 Test authentication and user profile management (deferred to actual migration)
- [ ] 7.9 Test system logging and error handling (deferred to actual migration)

## 8. Phase 7: Final Cleanup and Validation
- [x] 8.1 Create xPort5.Common project to resolve circular dependency
- [x] 8.2 Extract ComboItem and ComboList classes to xPort5.Common
- [x] 8.3 Extract Config.CurrentLanguageId to xPort5.Common
- [x] 8.4 Update xPort5.EF6 to reference xPort5.Common instead of xPort5.DAL.Common
- [x] 8.5 Verify build succeeds with circular dependency resolved
- [x] 8.6 Update 2-3 pilot forms to use xPort5.EF6 (Group, Division, Currency - PASSED)
- [ ] 8.7 Test pilot forms thoroughly
- [x] 8.8 Update remaining Settings forms to use `xPort5.EF6` (5 files - COMPLETED)
- [x] 8.9 Update Admin Forms (xPort5\Admin\Coding) - 50 files COMPLETED
- [x] 8.10 Update Product forms to use `xPort5.EF6` - 16 files COMPLETED
- [x] 8.11 Update Order forms to use `xPort5.EF6` - ALL PHASES COMPLETED
- [ ] 8.12 Remove all `using xPort5.DAL` statements from xPort5 project
- [ ] 8.13 Remove xPort5.DAL project from solution
