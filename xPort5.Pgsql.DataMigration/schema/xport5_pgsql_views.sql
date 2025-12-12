-- OLAP source
DROP VIEW IF EXISTS olap CASCADE;
CREATE OR REPLACE VIEW olap AS
SELECT
  T_Region.RegionName AS Region,
  Customer.CustomerName AS CustName,
  T_PaymentTerms.TermsName AS PricingTerms,
  OrderIN.INDate,
  EXTRACT(YEAR FROM OrderIN.INDate)::text AS year,
  EXTRACT(MONTH FROM OrderIN.INDate)::text AS month,
  'Q' || EXTRACT(QUARTER FROM OrderIN.INDate)::text AS quarter,
  OrderIN.INNumber,
  COALESCE(
    (SELECT CurrencyCode FROM T_Currency WHERE CurrencyId = OrderQT.CurrencyId),
    ''
  ) AS Currency,
  COALESCE((OrderINItems.Qty * OrderQTItems.Amount), 0) AS ExtAmount,
  COALESCE((OrderINItems.Qty * OrderQTItems.Amount * OrderQT.ExchangeRate), 0) AS ExtHKDAmount
FROM
  (((((Customer RIGHT JOIN
    (OrderIN LEFT JOIN OrderINItems ON OrderIN.OrderINId = OrderINItems.OrderINId)
    ON Customer.CustomerId = OrderIN.CustomerId)
    LEFT JOIN OrderSCItems ON (OrderINItems.LineNumber = OrderSCItems.LineNumber) AND (OrderINItems.OrderSCItemsId = OrderSCItems.OrderSCItemsId))
    LEFT JOIN OrderQTItems ON (OrderSCItems.LineNumber = OrderQTItems.LineNumber) AND (OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId))
    LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId)
    LEFT JOIN T_Region ON Customer.RegionId = T_Region.RegionId)
    LEFT JOIN T_PaymentTerms ON OrderIN.PricingTerms = T_PaymentTerms.TermsId
    INNER JOIN T_Currency ON Customer.CurrencyId = T_Currency.CurrencyId;

-- vwCategoryList
DROP VIEW IF EXISTS vwCategoryList CASCADE;
CREATE OR REPLACE VIEW vwCategoryList AS
SELECT
  T_Dept.DeptId,
  T_Dept.DeptCode,
  T_Dept.DeptName,
  T_Dept.DeptName_Chs,
  T_Dept.DeptName_Cht,
  T_Class.ClassId,
  T_Class.ClassCode,
  T_Class.ClassName,
  T_Class.ClassName_Chs,
  T_Class.ClassName_Cht,
  T_Category.CategoryId,
  T_Category.CategoryCode,
  T_Category.CategoryName,
  T_Category.CategoryName_Chs,
  T_Category.CategoryName_Cht,
  T_Category.CostingMethod,
  T_Category.InventoryMethod,
  T_Category.TaxMethod
FROM T_Class RIGHT JOIN T_Category ON T_Class.ClassId = T_Category.ClassId
  LEFT JOIN T_Dept ON T_Category.DeptId = T_Dept.DeptId
WHERE T_Dept.DeptId IS NULL AND T_Class.ClassId IS NULL;

-- vwCityList
DROP VIEW IF EXISTS vwCityList CASCADE;
CREATE OR REPLACE VIEW vwCityList AS
SELECT
  co.CountryId,
  co.CountryCode,
  co.CountryName,
  co.CountryName_Chs,
  co.CountryName_Cht,
  co.CountryPhoneCode,
  pr.ProvinceId,
  pr.ProvinceCode,
  pr.ProvinceName,
  pr.ProvinceName_Chs,
  pr.ProvinceName_Cht,
  ci.CityId,
  ci.CityCode,
  ci.CityPhoneCode,
  ci.CityName,
  ci.CityName_Chs,
  ci.CityName_Cht
FROM T_City AS ci
  INNER JOIN T_Country AS co ON ci.CountryId = co.CountryId
  LEFT JOIN T_Province AS pr ON ci.ProvinceId = pr.ProvinceId;

-- vwCustomerAddressList
DROP VIEW IF EXISTS vwCustomerAddressList CASCADE;
CREATE OR REPLACE VIEW vwCustomerAddressList AS
SELECT
  ca.CustomerAddressId,
  ca.CustomerId,
  a.AddressName,
  ca.DefaultRec,
  ca.AddrText,
  ca.AddrIsMailing,
  p1.PhoneName AS PhoneLabel1,
  ca.Phone1_Text,
  p2.PhoneName AS PhoneLabel2,
  ca.Phone2_Text,
  p3.PhoneName AS PhoneLabel3,
  ca.Phone3_Text,
  p4.PhoneName AS PhoneLabel4,
  ca.Phone4_Text,
  p5.PhoneName AS PhoneLabel5,
  ca.Phone5_Text,
  ca.Notes,
  ca.CreatedOn,
  s1.Alias AS CreatedBy,
  ca.ModifiedOn,
  s2.Alias AS ModifiedBy,
  ca.Retired
FROM CustomerAddress AS ca
  INNER JOIN Z_Address AS a ON ca.AddressId = a.AddressId
  INNER JOIN Z_Phone AS p1 ON ca.Phone1_Label = p1.PhoneId
  INNER JOIN Z_Phone AS p2 ON ca.Phone2_Label = p2.PhoneId
  INNER JOIN Z_Phone AS p3 ON ca.Phone3_Label = p3.PhoneId
  INNER JOIN Z_Phone AS p4 ON ca.Phone4_Label = p4.PhoneId
  INNER JOIN Z_Phone AS p5 ON ca.Phone5_Label = p5.PhoneId
  INNER JOIN Staff AS s1 ON ca.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON ca.ModifiedBy = s2.StaffId;

-- vwCustomerContactList
DROP VIEW IF EXISTS vwCustomerContactList CASCADE;
CREATE OR REPLACE VIEW vwCustomerContactList AS
SELECT
  cc.CustomerContactId,
  cc.CustomerId,
  cc.DefaultRec,
  zs.SalutationName,
  cc.FullName,
  cc.FirstName,
  cc.LastName,
  zj.JobTitleName,
  p1.PhoneName AS PhoneLabel1,
  cc.Phone1_Text,
  p2.PhoneName AS PhoneLabel2,
  cc.Phone2_Text,
  p3.PhoneName AS PhoneLabel3,
  cc.Phone3_Text,
  p4.PhoneName AS PhoneLabel4,
  cc.Phone4_Text,
  p5.PhoneName AS PhoneLabel5,
  cc.Phone5_Text,
  cc.Notes,
  cc.CreatedOn,
  s1.Alias AS CreatedBy,
  cc.ModifiedOn,
  s2.Alias AS ModifiedBy,
  cc.Retired
FROM CustomerContact AS cc
  INNER JOIN Z_Salutation AS zs ON cc.SalutationId = zs.SalutationId
  INNER JOIN Z_JobTitle AS zj ON cc.JobTitleId = zj.JobTitleId
  INNER JOIN Z_Phone AS p1 ON cc.Phone1_Label = p1.PhoneId
  INNER JOIN Z_Phone AS p2 ON cc.Phone2_Label = p2.PhoneId
  INNER JOIN Z_Phone AS p3 ON cc.Phone3_Label = p3.PhoneId
  INNER JOIN Z_Phone AS p4 ON cc.Phone4_Label = p4.PhoneId
  INNER JOIN Z_Phone AS p5 ON cc.Phone5_Label = p5.PhoneId
  INNER JOIN Staff AS s1 ON cc.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON cc.ModifiedBy = s2.StaffId;

-- vwCustomerList
DROP VIEW IF EXISTS vwCustomerList CASCADE;
CREATE OR REPLACE VIEW vwCustomerList AS
SELECT
  c.CustomerId,
  c.CustomerCode,
  c.ACNumber,
  c.CustomerName,
  c.CustomerName_Chs,
  c.CustomerName_Cht,
  c.RegionId,
  r.RegionName,
  c.TermsId,
  t.TermsName,
  c.CurrencyId,
  cny.CurrencyName,
  c.CreditLimit,
  c.ProfitMargin,
  c.BlackListed,
  c.Remarks,
  c.Status,
  c.CreatedOn,
  s1.Alias AS CreatedBy,
  c.ModifiedOn,
  s2.Alias AS ModifiedBy,
  c.Retired
FROM Customer AS c
  INNER JOIN T_Region AS r ON c.RegionId = r.RegionId
  INNER JOIN T_PaymentTerms AS t ON c.TermsId = t.TermsId
  INNER JOIN T_Currency AS cny ON c.CurrencyId = cny.CurrencyId
  INNER JOIN Staff AS s1 ON c.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON c.ModifiedBy = s2.StaffId;

-- vwInvoiceItemList
DROP VIEW IF EXISTS vwInvoiceItemList CASCADE;
CREATE OR REPLACE VIEW vwInvoiceItemList AS
SELECT
  m.OrderINId,
  m.INNumber,
  m.INDate,
  i.OrderINItemsId,
  i.LineNumber,
  a.ArticleId,
  a.SKU,
  a.ArticleCode,
  a.ArticleName,
  a.ArticleName_Chs,
  a.ArticleName_Cht,
  p.PackageId,
  p.PackageCode,
  p.PackageName,
  p.PackageName_Chs,
  p.PackageName_Cht,
  s.SupplierId,
  s.SupplierCode,
  s.SupplierName,
  s.SupplierName_Chs,
  s.SupplierName_Cht,
  qi.Particular,
  qi.CustRef,
  qi.PriceType,
  qi.FactoryCost,
  qi.Margin,
  qi.FCL,
  qi.LCL,
  qi.SampleQty,
  qi.Qty,
  qi.Unit,
  qi.Amount,
  qi.Carton,
  qi.GLAccount,
  qi.RefDocNo,
  qi.ShippingMark,
  qi.QtyIN,
  qi.QtyOUT,
  ps.SuppRef,
  c.CurrencyCode,
  ps.FCLCost,
  ps.LCLCost,
  i.Qty AS Inv_Qty,
  (SELECT SCNumber FROM OrderSC WHERE OrderSCId = sci.OrderSCId) AS SCNumber,
  ap.InnerBox,
  ap.OuterBox,
  ap.CUFT
FROM OrderIN AS m
  INNER JOIN Supplier AS s
    INNER JOIN ArticleSupplier AS ps
      INNER JOIN OrderINItems AS i
        LEFT JOIN OrderSCItems AS sci ON i.OrderSCItemsId = sci.OrderSCItemsId
        LEFT JOIN OrderQTItems AS qi ON sci.OrderQTItemId = qi.OrderQTItemId
      INNER JOIN Article AS a ON qi.ArticleId = a.ArticleId
    ON ps.ArticleId = qi.ArticleId AND ps.SupplierId = qi.SupplierId
  ON s.SupplierId = ps.SupplierId
  INNER JOIN T_Currency AS c ON ps.CurrencyId = c.CurrencyId
ON m.OrderINId = i.OrderINId
  INNER JOIN T_Package AS p ON qi.PackageId = p.PackageId
  INNER JOIN OrderQTPackage AS ap ON qi.OrderQTItemId = ap.OrderQTItemId;

-- vwInvoiceList
DROP VIEW IF EXISTS vwInvoiceList CASCADE;
CREATE OR REPLACE VIEW vwInvoiceList AS
SELECT
  in_.OrderINId,
  in_.INDate,
  in_.InUse,
  in_.Status,
  in_.INNumber,
  c.CustomerName,
  in_.Remarks,
  in_.CreatedOn,
  s1.Alias AS CreatedBy,
  in_.ModifiedOn,
  s2.Alias AS ModifiedBy,
  in_.Revision,
  in_.SendFrom,
  in_.SendTo,
  c.CustomerId
FROM OrderIN AS in_
  LEFT JOIN Staff AS s ON in_.StaffId = s.StaffId
  LEFT JOIN Staff AS s2 ON in_.ModifiedBy = s2.StaffId
  LEFT JOIN Staff AS s1 ON in_.CreatedBy = s1.StaffId
  LEFT JOIN Customer AS c ON in_.CustomerId = c.CustomerId;

-- vwOSSample
DROP VIEW IF EXISTS vwOSSample CASCADE;
CREATE OR REPLACE VIEW vwOSSample AS
SELECT
  Customer.CustomerId,
  Customer.CustomerName AS CustName,
  OrderQTItems.CustRef,
  Article.ArticleCode,
  Article.ArticleName AS ArtName,
  OrderQTItems.Amount,
  OrderQTItems.SampleQty,
  OrderQT.Unit,
  Supplier.SupplierName,
  OrderQT.QTNumber,
  OrderQTItems.QtyOUT,
  T_Package.PackageCode,
  T_Package.PackageName,
  Supplier.SupplierCode,
  OrderQT.QTDate,
  OrderQTItems.OrderQTItemId
FROM T_Package
  RIGHT JOIN OrderQTItems ON T_Package.PackageId = OrderQTItems.PackageId
  RIGHT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId
WHERE OrderQTItems.OrderQTItemId IS NOT NULL;

-- vwOSShipment
DROP VIEW IF EXISTS vwOSShipment CASCADE;
CREATE OR REPLACE VIEW vwOSShipment AS
SELECT
  Customer.CustomerId,
  Supplier.SupplierId,
  Customer.CustomerName AS CustName,
  OrderQTItems.CustRef,
  Article.ArticleCode,
  Article.ArticleName AS ArtName,
  (SELECT CurrencyCode FROM T_Currency WHERE OrderQT.CurrencyId = T_Currency.CurrencyId) AS CurrencyCode,
  OrderQTItems.Amount,
  OrderQTItems.Qty,
  OrderQTItems.Unit,
  OrderQTCustShipping.ShippedOn AS ShipmentDate,
  OrderQTCustShipping.QtyOrdered,
  OrderQTCustShipping.QtyShipped,
  (OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped) AS OSQty,
  ((OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped) * OrderQTItems.Amount) AS OSAmount,
  Supplier.SupplierName AS SuppName,
  OrderSC.SCNumber
FROM OrderSCItems
  LEFT JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId
  LEFT JOIN Customer ON OrderSC.CustomerId = Customer.CustomerId
  RIGHT JOIN Article RIGHT JOIN OrderQTItems ON Article.ArticleId = OrderQTItems.ArticleId
    ON OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId
  RIGHT JOIN OrderQTCustShipping ON OrderQTItems.OrderQTItemId = OrderQTCustShipping.OrderQTItemId
  LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  LEFT JOIN OrderQT ON OrderQT.OrderQTId = OrderQTItems.OrderQTId
WHERE Customer.CustomerName IS NOT NULL;

-- vwOrderINQTList
DROP VIEW IF EXISTS vwOrderINQTList CASCADE;
CREATE OR REPLACE VIEW vwOrderINQTList AS
SELECT
  sci.LineNumber AS SCLineNo,
  art.ArticleId,
  art.ArticleCode,
  COALESCE(T_Color.ColorName, art.ColorPattern) AS Color,
  qti.CustRef,
  qti.Qty - sci.QtyOUT AS OSQty,
  0 AS InvoicedQty,
  qt.QTNumber,
  qti.LineNumber AS QTLineNo,
  qti.OrderQTItemId,
  sci.OrderSCItemsId,
  sc.SCNumber
FROM OrderSCItems AS sci
  LEFT JOIN OrderSC AS sc ON sci.OrderSCId = sc.OrderSCId
  LEFT JOIN OrderQTItems AS qti ON sci.OrderQTItemId = qti.OrderQTItemId
  LEFT JOIN Article AS art ON qti.ArticleId = art.ArticleId
  LEFT JOIN T_Color ON art.ColorId = T_Color.ColorId
  LEFT JOIN OrderQT AS qt ON qti.OrderQTId = qt.OrderQTId;

-- vwOrderINShipmentList
DROP VIEW IF EXISTS vwOrderINShipmentList CASCADE;
CREATE OR REPLACE VIEW vwOrderINShipmentList AS
SELECT
  OrderQTCustShipping.ShippedOn,
  OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped AS OSQty,
  0 AS ThisShipment,
  OrderQTCustShipping.OrderQTItemId,
  OrderSCItems.OrderSCItemsId
FROM OrderQTCustShipping
  LEFT JOIN OrderSCItems ON OrderQTCustShipping.OrderQTItemId = OrderSCItems.OrderQTItemId;

-- vwOrderQTItemList
DROP VIEW IF EXISTS vwOrderQTItemList CASCADE;
CREATE OR REPLACE VIEW vwOrderQTItemList AS
SELECT
  m.OrderQTId,
  m.QTNumber,
  m.QTDate,
  i.OrderQTItemId,
  i.LineNumber,
  a.ArticleId,
  a.SKU,
  a.ArticleCode,
  a.ArticleName,
  a.ArticleName_Chs,
  a.ArticleName_Cht,
  p.PackageId,
  p.PackageCode,
  p.PackageName,
  p.PackageName_Chs,
  p.PackageName_Cht,
  s.SupplierId,
  s.SupplierCode,
  s.SupplierName,
  s.SupplierName_Chs,
  s.SupplierName_Cht,
  i.Particular,
  i.CustRef,
  i.PriceType,
  i.FactoryCost,
  i.Margin,
  i.FCL,
  i.LCL,
  i.SampleQty,
  i.Qty,
  i.Unit,
  i.Amount,
  i.Carton,
  i.GLAccount,
  i.RefDocNo,
  i.ShippingMark,
  i.QtyIN,
  i.QtyOUT,
  ps.SuppRef,
  c.CurrencyCode,
  ps.FCLCost,
  ps.LCLCost,
  ap.InnerBox,
  ap.OuterBox,
  ap.CUFT
FROM OrderQTItems AS i
  INNER JOIN OrderQT AS m ON i.OrderQTId = m.OrderQTId
  INNER JOIN Article AS a ON i.ArticleId = a.ArticleId
  INNER JOIN ArticleSupplier AS ps ON ps.ArticleId = i.ArticleId AND ps.SupplierId = i.SupplierId
  INNER JOIN Supplier AS s ON s.SupplierId = ps.SupplierId
  INNER JOIN T_Currency AS c ON c.CurrencyId = ps.CurrencyId
  INNER JOIN T_Package AS p ON i.PackageId = p.PackageId
  INNER JOIN OrderQTPackage AS ap ON i.OrderQTItemId = ap.OrderQTItemId;

-- vwOrderQTList
DROP VIEW IF EXISTS vwOrderQTList CASCADE;
CREATE OR REPLACE VIEW vwOrderQTList AS
SELECT
  q.OrderQTId,
  q.QTNumber,
  q.QTDate,
  q.CustomerId,
  c.CustomerCode,
  c.CustomerName,
  c.CustomerName_Chs,
  c.CustomerName_Cht,
  q.StaffId,
  s.Alias AS SalePerson,
  q.PriceMethod,
  q.FCLFactor,
  q.LCLFactor,
  q.Unit,
  q.Measurement,
  q.SampleQty,
  q.InputMask,
  tc.CurrencyCode,
  q.ExchangeRate,
  tp.TermsName,
  q.Repayment,
  q.SendFrom,
  q.SendTo,
  q.TotalQty,
  q.TotalQtyIN,
  q.TotalQtyOUT,
  q.TotalAmount,
  q.Remarks,
  q.Remarks2,
  q.Remarks3,
  q.Revision,
  q.InUse,
  q.Status,
  q.CreatedOn,
  s1.Alias AS CreatedBy,
  q.ModifiedOn,
  s2.Alias AS ModifiedBy,
  q.AccessedOn,
  s3.Alias AS AccessedBy,
  q.Retired
FROM OrderQT AS q
  LEFT JOIN Staff AS s3 ON q.AccessedBy = s3.StaffId
  LEFT JOIN T_PaymentTerms AS tp ON q.TermsId = tp.TermsId
  LEFT JOIN Staff AS s2 ON q.ModifiedBy = s2.StaffId
  LEFT JOIN T_Currency AS tc ON q.CurrencyId = tc.CurrencyId
  LEFT JOIN Staff AS s1 ON q.CreatedBy = s1.StaffId
  LEFT JOIN Customer AS c ON q.CustomerId = c.CustomerId
  LEFT JOIN Staff AS s ON q.StaffId = s.StaffId;

-- vwOrderSPItemList
DROP VIEW IF EXISTS vwOrderSPItemList CASCADE;
-- CREATE OR REPLACE VIEW vwOrderSPItemList AS
-- SELECT
--   m.OrderSPId,
--   m.SPNumber,
--   m.SPDate,
--   i.OrderSPItemsId,
--   i.LineNumber,
--   a.ArticleId,
--   a.SKU,
--   a.ArticleCode,
--   a.ArticleName,
--   a.ArticleName_Chs,
--   a.ArticleName_Cht,
--   p.PackageId,
--   p.PackageCode,
--   p.PackageName,
--   p.PackageName_Chs,
--   p.PackageName_Cht,
--   s.SupplierId,
--   s.SupplierCode,
--   s.SupplierName,
--   s.SupplierName_Chs,
--   s.SupplierName_Cht,
--   qi.Particular,
--   qi.CustRef,
--   qi.PriceType,
--   qi.FactoryCost,
--   qi.Margin,
--   qi.FCL,
--   qi.LCL,
--   qi.SampleQty,
--   i.Qty,
--   i.Unit,
--   qi.Amount,
--   qi.Carton,
--   qi.GLAccount,
--   qi.RefDocNo,
--   qi.ShippingMark,
--   qi.QtyIN,
--   qi.QtyOUT,
--   ps.SuppRef,
--   c.CurrencyCode,
--   ps.FCLCost,
--   ps.LCLCost,
--   ap.InnerBox,
--   ap.OuterBox,
--   ap.CUFT,
--   qi.LineNumber AS QTLineNumber,
--   (SELECT QTNumber FROM OrderQT WHERE OrderQTId = qi.OrderQTId) AS QTNumber
-- FROM Supplier AS s
--   INNER JOIN OrderSPItems AS i
--     LEFT JOIN OrderQTItems AS qi ON i.OrderQTItemId = qi.OrderQTItemId
--   INNER JOIN Article AS a ON qi.ArticleId = a.ArticleId
--   INNER JOIN ArticlePackage AS ap
--     INNER JOIN T_Package AS p ON ap.PackageId = p.PackageId
--   ON qi.ArticleId = ap.ArticleId AND qi.PackageId = ap.PackageId
--   INNER JOIN ArticleSupplier AS ps ON qi.ArticleId = ps.ArticleId AND qi.SupplierId = ps.SupplierId
--   INNER JOIN T_Currency AS c ON ps.CurrencyId = c.CurrencyId
--   INNER JOIN OrderSP AS m ON i.OrderSPId = m.OrderSPId;
CREATE OR REPLACE VIEW vworderspitemlist AS
SELECT
    m.orderspid,
    m.spnumber,
    m.spdate,
    i.orderspitemsid,
    i.linenumber,
    a.articleid,
    a.sku,
    a.articlecode,
    a.articlename,
    a.articlename_chs,
    a.articlename_cht,
    p.packageid,
    p.packagecode,
    p.packagename,
    p.packagename_chs,
    p.packagename_cht,
    s.supplierid,
    s.suppliercode,
    s.suppliername,
    s.suppliername_chs,
    s.suppliername_cht,
    qi.particular,
    qi.custref,
    qi.pricetype,
    qi.factorycost,
    qi.margin,
    qi.fcl,
    qi.lcl,
    qi.sampleqty,
    i.qty,
    i.unit,
    qi.amount,
    qi.carton,
    qi.glaccount,
    qi.refdocno,
    qi.shippingmark,
    qi.qtyin,
    qi.qtyout,
    ps.suppref,
    c.currencycode,
    ps.fclcost,
    ps.lclcost,
    ap.innerbox,
    ap.outerbox,
    ap.cuft,
    qi.linenumber AS qtlinenumber,
    (SELECT qtnumber
     FROM orderqt
     WHERE orderqtid = qi.orderqtid) AS qtnumber
FROM ordersp AS m
    INNER JOIN orderspitems AS i ON i.orderspid = m.orderspid
    LEFT JOIN orderqtitems AS qi ON i.orderqtitemid = qi.orderqtitemid
    INNER JOIN article AS a ON qi.articleid = a.articleid
    INNER JOIN articlepackage AS ap 
        INNER JOIN t_package AS p ON ap.packageid = p.packageid
        ON qi.articleid = ap.articleid AND qi.packageid = ap.packageid
    INNER JOIN articlesupplier AS ps ON qi.articleid = ps.articleid AND qi.supplierid = ps.supplierid
    INNER JOIN supplier AS s ON ps.supplierid = s.supplierid
    INNER JOIN t_currency AS c ON ps.currencyid = c.currencyid;
/* Grok 4.1
Why this works and matches the original exactly:

Starts with ordersp m → orderspitems i (header to detail)
Then LEFT JOIN orderqtitems qi on the item link
Then article a from qi
Then articlepackage ap + t_package p (nested inner join, just like original)
Then articlesupplier ps using bothqi.articleid and qi.supplierid — this is critical and was in the original
Then supplier s from ps.supplierid
Finally t_currency c from ps

This preserves the exact join logic of your T-SQL, including:

Supplier comes through qi.supplierid → articlesupplier → supplier
No premature reference to undefined aliases
All ON clauses are properly placed right after their JOIN

No more errors about undefined tables or columns.
This should now create successfully in PostgreSQL.

Grok: The version I provided last is logically identical to your original T-SQL view — 
I only restructured the FROM clause to satisfy PostgreSQL's stricter syntax rules around join ordering and ON clause placement. 
The joins, conditions, left/right behavior, and selected columns are all exactly the same, 
so the result set should match your MS SQL version row-for-row (assuming the data is the same).

Using it is perfectly safe. When you get more data and have a chance to compare results side-by-side, you'll likely see they are equivalent.
 */

-- vwOrderSPList
DROP VIEW IF EXISTS vwOrderSPList CASCADE;
CREATE OR REPLACE VIEW vwOrderSPList AS
SELECT
  sp.OrderSPId,
  sp.SPDate,
  sp.InUse,
  sp.Status,
  sp.SPNumber,
  c.CustomerName,
  s.Alias AS Salesperson,
  sp.Remarks,
  sp.CreatedOn,
  s1.Alias AS CreatedBy,
  sp.ModifiedOn,
  s2.Alias AS ModifiedBy,
  sp.Revision,
  sp.SendFrom,
  sp.SendTo,
  c.CustomerId,
  c.CustomerCode,
  c.CustomerName_Chs,
  c.CustomerName_Cht
FROM OrderSP AS sp
  LEFT JOIN Staff AS s ON sp.StaffId = s.StaffId
  LEFT JOIN Staff AS s2 ON sp.ModifiedBy = s2.StaffId
  LEFT JOIN Staff AS s1 ON sp.CreatedBy = s1.StaffId
  LEFT JOIN Customer AS c ON sp.CustomerId = c.CustomerId;

-- vwPreOrderItemList
DROP VIEW IF EXISTS vwPreOrderItemList CASCADE;
CREATE OR REPLACE VIEW vwPreOrderItemList AS
SELECT
  m.OrderPLId,
  m.PLNumber,
  m.PLDate,
  i.OrderPLItemsId,
  i.LineNumber,
  a.ArticleId,
  a.SKU,
  a.ArticleCode,
  a.ArticleName,
  a.ArticleName_Chs,
  a.ArticleName_Cht,
  p.PackageId,
  p.PackageCode,
  p.PackageName,
  p.PackageName_Chs,
  p.PackageName_Cht,
  s.SupplierId,
  s.SupplierCode,
  s.SupplierName,
  s.SupplierName_Chs,
  s.SupplierName_Cht,
  qi.Particular,
  qi.CustRef,
  qi.PriceType,
  qi.FactoryCost,
  qi.Margin,
  qi.FCL,
  qi.LCL,
  qi.SampleQty,
  qi.Qty,
  qi.Unit,
  qi.Amount,
  qi.Carton,
  qi.GLAccount,
  qi.RefDocNo,
  qi.ShippingMark,
  qi.QtyIN,
  qi.QtyOUT,
  ps.SuppRef,
  c.CurrencyCode,
  ps.FCLCost,
  ps.LCLCost,
  ap.InnerBox,
  ap.OuterBox,
  ap.CUFT
FROM OrderPL AS m
  INNER JOIN Supplier AS s
    INNER JOIN ArticleSupplier AS ps
      INNER JOIN OrderPLItems AS i
        LEFT JOIN OrderQTItems AS qi ON i.OrderQTItemId = qi.OrderQTItemId
      INNER JOIN Article AS a ON qi.ArticleId = a.ArticleId
    ON ps.ArticleId = qi.ArticleId AND ps.SupplierId = qi.SupplierId
  ON s.SupplierId = ps.SupplierId
  INNER JOIN T_Currency AS c ON ps.CurrencyId = c.CurrencyId
ON m.OrderPLId = i.OrderPLId
  INNER JOIN T_Package AS p ON qi.PackageId = p.PackageId
  INNER JOIN OrderQTPackage AS ap ON qi.OrderQTItemId = ap.OrderQTItemId;

-- vwPreOrderList
DROP VIEW IF EXISTS vwPreOrderList CASCADE;
CREATE OR REPLACE VIEW vwPreOrderList AS
SELECT
  pl.OrderPLId,
  pl.PLDate,
  pl.InUse,
  pl.Status,
  pl.PLNumber,
  c.CustomerName,
  pl.Remarks,
  pl.CreatedOn,
  s1.Alias AS CreatedBy,
  pl.ModifiedOn,
  s2.Alias AS ModifiedBy,
  pl.Revision,
  pl.TotalQty,
  pl.TotalQtyIN,
  pl.TotalQtyOUT,
  pl.TotalAmount,
  pl.SendFrom,
  pl.SendTo,
  c.CustomerId
FROM OrderPL AS pl
  LEFT JOIN Staff AS s ON pl.StaffId = s.StaffId
  LEFT JOIN Staff AS s2 ON pl.ModifiedBy = s2.StaffId
  LEFT JOIN Staff AS s1 ON pl.CreatedBy = s1.StaffId
  LEFT JOIN Customer AS c ON pl.CustomerId = c.CustomerId;

-- vwPriceDetailList
DROP VIEW IF EXISTS vwPriceDetailList CASCADE;
CREATE OR REPLACE VIEW vwPriceDetailList AS
SELECT
  m.QTNumber,
  m.QTDate,
  i.LineNumber,
  a.SKU,
  a.ArticleCode,
  a.ArticleName,
  p.PackageCode,
  p.PackageName,
  (SELECT AgeGradingName FROM T_AgeGrading WHERE T_AgeGrading.AgeGradingId = a.AgeGradingId) AS AgeGrading,
  (SELECT OriginName FROM T_Origin WHERE T_Origin.OriginId = a.OriginId) AS Origin,
  s.SupplierCode,
  s.SupplierName,
  i.Particular,
  i.CustRef,
  i.FactoryCost,
  i.Margin,
  i.FCL,
  i.LCL,
  i.Qty,
  i.Unit,
  i.Amount,
  ps.SuppRef,
  c.CurrencyCode,
  ps.FCLCost,
  ps.LCLCost,
  ap.InnerBox,
  ap.OuterBox,
  ap.CUFT
FROM ArticleSupplier AS ps
  INNER JOIN OrderQTItems AS i
    INNER JOIN Article AS a ON i.ArticleId = a.ArticleId
    INNER JOIN OrderQT AS m ON i.OrderQTId = m.OrderQTId
  ON ps.ArticleId = i.ArticleId AND ps.SupplierId = i.SupplierId
  INNER JOIN Supplier AS s ON ps.SupplierId = s.SupplierId
  INNER JOIN T_Currency AS c ON ps.CurrencyId = c.CurrencyId
  INNER JOIN ArticlePackage AS ap
  INNER JOIN T_Package AS p ON ap.PackageId = p.PackageId
  ON i.ArticleId = ap.ArticleId AND i.PackageId = ap.PackageId;

-- vwPrint_CompletedPriceList
DROP VIEW IF EXISTS vwPrint_CompletedPriceList CASCADE;
CREATE OR REPLACE VIEW vwPrint_CompletedPriceList AS
SELECT
  OrderQT.QTNumber,
  OrderQTItems.LineNumber,
  Article.SKU,
  Article.ArticleCode,
  Supplier.SupplierCode,
  T_Package.PackageName,
  Article.ArticleName AS ArtName,
  OrderQTItems.Particular,
  OrderQTItems.CustRef,
  OrderQTItems.Qty,
  OrderQTItems.Unit AS QuotedUnit,
  OrderQTItems.FactoryCost,
  OrderQTItems.Margin,
  OrderQTItems.FCL,
  OrderQTItems.LCL,
  OrderQTItems.Amount,
  OrderQTSupplier.SuppRef,
  (SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId) AS SuppCurrency,
  OrderQTSupplier.FCLCost,
  OrderQTSupplier.LCLCost,
  OrderQTPackage.Unit,
  OrderQTPackage.InnerBox,
  OrderQTPackage.OuterBox,
  OrderQTPackage.CUFT,
  Supplier.SupplierName AS SuppName,
  T_AgeGrading.AgeGradingName AS AgeGrading,
  T_Origin.OriginName AS Origin,
  OrderQT.QTDate,
  Customer.CustomerName AS CustName,
  CustomerAddress.DefaultRec,
  CustomerAddress.AddrText,
  CustomerAddress.Phone1_Label,
  CustomerAddress.Phone1_Text,
  CustomerAddress.Phone2_Label,
  CustomerAddress.Phone2_Text,
  CustomerAddress.Phone3_Label,
  CustomerAddress.Phone3_Text,
  CustomerAddress.Phone4_Label,
  CustomerAddress.Phone4_Text,
  CustomerAddress.Phone5_Label,
  CustomerAddress.Phone5_Text,
  (SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId) AS CurrencyUsed,
  OrderQT.PriceMethod,
  T_PaymentTerms.TermsName AS Terms,
  OrderQT.Remarks
FROM OrderQTItems
  LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN OrderQTSupplier ON OrderQTItems.OrderQTItemId = OrderQTSupplier.OrderQTItemId
  LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId
  LEFT JOIN T_AgeGrading ON Article.AgeGradingId = T_AgeGrading.AgeGradingId
  LEFT JOIN T_Origin ON Article.OriginId = T_Origin.OriginId
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId
  LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId
  LEFT JOIN T_PaymentTerms ON OrderQT.TermsId = T_PaymentTerms.TermsId;

-- vwPrint_PriceList
DROP VIEW IF EXISTS vwPrint_PriceList CASCADE;
CREATE OR REPLACE VIEW vwPrint_PriceList AS
SELECT
  OrderQT.QTNumber,
  OrderQT.QTDate,
  Customer.CustomerName AS CustName,
  CustomerAddress.AddrText,
  CustomerAddress.Phone1_Label,
  CustomerAddress.Phone1_Text,
  CustomerAddress.Phone2_Label,
  CustomerAddress.Phone2_Text,
  CustomerAddress.Phone3_Label,
  CustomerAddress.Phone3_Text,
  CustomerAddress.Phone4_Label,
  CustomerAddress.Phone4_Text,
  CustomerAddress.Phone5_Label,
  CustomerAddress.Phone5_Text,
  OrderQT.Unit,
  OrderQT.PriceMethod,
  (SELECT T_Currency.CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId) AS CurrencyUsed,
  T_PaymentTerms.TermsName AS Terms,
  OrderQT.Remarks
FROM OrderQT
  LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId
  LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId
  LEFT JOIN T_PaymentTerms ON OrderQT.TermsId = T_PaymentTerms.TermsId;

-- vwPrint_QuotationList
DROP VIEW IF EXISTS vwPrint_QuotationList CASCADE;
CREATE OR REPLACE VIEW vwPrint_QuotationList AS
SELECT
  OrderQT.QTNumber,
  OrderQT.QTDate,
  OrderQTItems.LineNumber,
  OrderQTItems.CustRef,
  OrderQTItems.Particular,
  OrderQTItems.Margin,
  OrderQTItems.FactoryCost,
  OrderQTItems.LCL,
  OrderQTItems.FCL,
  OrderQTItems.Amount,
  Customer.CustomerName AS CustName,
  Article.ArticleId,
  Article.ArticleCode,
  Article.ArticleName AS ArtName,
  Article.ColorPattern,
  Supplier.SupplierCode,
  Supplier.SupplierName AS SuppName,
  OrderQTSupplier.SuppRef,
  T_Origin.OriginName AS Origin,
  T_AgeGrading.AgeGradingName AS AgeGrading,
  OrderQTPackage.InnerBox,
  OrderQTPackage.OuterBox,
  OrderQT.Unit,
  OrderQTPackage.CUFT,
  T_Package.PackageName,
  T_PaymentTerms.TermsName AS Terms,
  (SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId) AS SuppCurrency,
  (SELECT CurrencyCode FROM T_Currency AS T_Currency_1 WHERE T_Currency_1.CurrencyId = Customer.CurrencyId) AS CurrencyUsed,
  CustomerAddress.DefaultRec,
  CustomerAddress.AddrText,
  CustomerAddress.Phone1_Text,
  CustomerAddress.Phone2_Text,
  CustomerAddress.Phone3_Text,
  CustomerAddress.Phone4_Text,
  CustomerAddress.Phone5_Text
FROM OrderQTItems
  LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN OrderQTSupplier ON OrderQTItems.OrderQTItemId = OrderQTSupplier.OrderQTItemId
  LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId
  LEFT JOIN T_AgeGrading ON Article.AgeGradingId = T_AgeGrading.AgeGradingId
  LEFT JOIN T_Origin ON Article.OriginId = T_Origin.OriginId
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId
  LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId
  LEFT JOIN T_PaymentTerms ON OrderQT.TermsId = T_PaymentTerms.TermsId;

-- vwProductList
DROP VIEW IF EXISTS vwProductList CASCADE;
CREATE OR REPLACE VIEW vwProductList AS
SELECT
  a.ArticleId,
  a.SKU,
  a.ArticleCode,
  a.ArticleName,
  a.ArticleName_Chs,
  a.ArticleName_Cht,
  c.CategoryId,
  c.CategoryCode,
  c.CategoryName,
  c.CategoryName_Chs,
  c.CategoryName_Cht,
  ag.AgeGradingId,
  ag.AgeGradingCode,
  ag.AgeGradingName,
  ag.AgeGradingName_Chs,
  ag.AgeGradingName_Cht,
  o.OriginId,
  o.OriginCode,
  o.OriginName,
  o.OriginName_Chs,
  o.OriginName_Cht,
  a.Remarks,
  a.ColorPattern,
  a.Barcode,
  a.Status,
  a.CreatedOn,
  s.Alias AS CreatedBy,
  a.ModifiedOn,
  s1.Alias AS ModifiedBy,
  cl.ColorId,
  cl.ColorCode,
  cl.ColorName,
  cl.ColorName_Chs,
  cl.ColorName_Cht
FROM Staff AS s1
  INNER JOIN Article AS a
    INNER JOIN T_Category AS c ON a.CategoryId = c.CategoryId
    INNER JOIN T_Origin AS o ON a.OriginId = o.OriginId
  ON s1.StaffId = a.ModifiedBy
  LEFT JOIN T_AgeGrading AS ag ON a.AgeGradingId = ag.AgeGradingId
  LEFT JOIN T_Color AS cl ON a.ColorId = cl.ColorId
  INNER JOIN Staff AS s ON a.CreatedBy = s.StaffId;

-- vwProductPackage
DROP VIEW IF EXISTS vwProductPackage CASCADE;
CREATE OR REPLACE VIEW vwProductPackage AS
SELECT
  ap.ArticleId,
  ap.ArticlePackageId,
  ap.PackageId,
  p.PackageCode,
  p.PackageName,
  ap.DefaultRec,
  ap.UomId,
  u.UomName,
  ap.InnerBox,
  ap.OuterBox,
  ap.CUFT,
  ap.SizeLength_in,
  ap.SizeWidth_in,
  ap.SizeHeight_in,
  ap.SizeLength_cm,
  ap.SizeWidth_cm,
  ap.SizeHeight_cm,
  ap.WeightGross_lb,
  ap.WeightNet_lb,
  ap.WeightGross_kg,
  ap.WeightNet_kg,
  ap.ContainerQty,
  ap.ContainerSize,
  ap.CreatedOn,
  s1.Alias AS CreatedBy,
  ap.ModifiedOn,
  s2.Alias AS ModifiedBy
FROM ArticlePackage AS ap
  INNER JOIN T_UnitOfMeasures AS u ON ap.UomId = u.UomId
  INNER JOIN T_Package AS p ON ap.PackageId = p.PackageId
  INNER JOIN Staff AS s1 ON ap.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON ap.ModifiedBy = s2.StaffId;

-- vwProductSupplier
DROP VIEW IF EXISTS vwProductSupplier CASCADE;
CREATE OR REPLACE VIEW vwProductSupplier AS
SELECT
  a.ArticleId,
  a.ArticleSupplierId,
  a.SupplierId,
  s.SupplierCode,
  s.SupplierName,
  a.DefaultRec,
  a.SuppRef,
  c.CurrencyCode,
  a.FCLCost,
  a.LCLCost,
  a.UnitCost,
  a.Notes,
  a.CreatedOn,
  s1.Alias AS CreatedBy,
  a.ModifiedOn,
  s1.Alias AS ModifiedBy
FROM ArticleSupplier AS a
  INNER JOIN Supplier AS s ON a.SupplierId = s.SupplierId
  INNER JOIN Staff AS s1 ON a.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON a.ModifiedBy = s2.StaffId
  INNER JOIN T_Currency AS c ON a.CurrencyId = c.CurrencyId;

-- vwProductWithSupplierAndPackage
DROP VIEW IF EXISTS vwProductWithSupplierAndPackage CASCADE;
CREATE OR REPLACE VIEW vwProductWithSupplierAndPackage AS
SELECT
  p.ArticleId,
  p.SKU,
  p.ArticleCode,
  p.ArticleName,
  p.ArticleName_Chs,
  p.ArticleName_Cht,
  COALESCE(pp.PackageCode, '') AS PackageCode,
  COALESCE(pp.PackageName, '') AS PackageName,
  COALESCE(ps.SupplierName, '') AS SupplierName,
  COALESCE(ps.SuppRef, '') AS SuppRef,
  COALESCE(ps.SupplierCode, '') AS SupplierCode,
  p.ColorPattern,
  COALESCE(pp.UomName, '') AS Unit,
  COALESCE(pp.InnerBox, 0) AS InnerBox,
  COALESCE(pp.OuterBox, 0) AS OuterBox,
  COALESCE(pp.CUFT, 0) AS CUFT
FROM vwProductList AS p
  LEFT JOIN vwProductPackage AS pp ON p.ArticleId = pp.ArticleId
  LEFT JOIN vwProductSupplier AS ps ON p.ArticleId = ps.ArticleId;

-- vwPurchaseContractItemList
DROP VIEW IF EXISTS vwPurchaseContractItemList CASCADE;
CREATE OR REPLACE VIEW vwPurchaseContractItemList AS
SELECT
  OrderPC.OrderPCId,
  OrderPCItems.OrderPCItemsId,
  OrderQT.OrderQTId,
  OrderPC.PCDate,
  OrderPCItems.LineNumber,
  OrderPC.PCNumber,
  Article.ArticleCode,
  Article.ArticleId,
  Article.ArticleName,
  T_Package.PackageId,
  T_Package.PackageCode,
  T_Package.PackageName,
  T_Package.PackageName_Chs,
  T_Package.PackageName_Cht,
  OrderQTItems.CustRef,
  ArticleSupplier.SuppRef,
  OrderQTItems.Qty,
  OrderQTItems.Unit,
  OrderQTItems.PriceType,
  OrderQTItems.FactoryCost,
  T_Currency.CurrencyCode,
  ArticleSupplier.FCLCost,
  ArticleSupplier.LCLCost,
  ArticleSupplier.UnitCost,
  OrderQTItems.ShippingMark
FROM T_Package
  INNER JOIN T_Currency
  INNER JOIN ArticleSupplier
  INNER JOIN OrderPC
    INNER JOIN OrderPCItems ON OrderPC.OrderPCId = OrderPCItems.OrderPCId
    INNER JOIN OrderSCItems ON OrderSCItems.OrderSCItemsId = OrderPCItems.OrderSCItemsId
    INNER JOIN OrderQTItems ON OrderQTItems.OrderQTItemId = OrderSCItems.OrderQTItemId
    INNER JOIN OrderQT ON OrderQT.OrderQTId = OrderQTItems.OrderQTId
    INNER JOIN Article ON Article.ArticleId = OrderQTItems.ArticleId
  ON ArticleSupplier.ArticleId = OrderQTItems.ArticleId
  ON T_Currency.CurrencyId = ArticleSupplier.CurrencyId
ON T_Package.PackageId = OrderQTItems.PackageId;

-- vwPurchaseContractList
DROP VIEW IF EXISTS vwPurchaseContractList CASCADE;
CREATE OR REPLACE VIEW vwPurchaseContractList AS
SELECT DISTINCT
  pc.OrderPCId,
  pc.PCNumber,
  pc.PCDate,
  supp.SupplierId,
  supp.SupplierCode,
  supp.SupplierName,
  supp.SupplierName_Chs,
  supp.SupplierName_Cht,
  pc.StaffId,
  S.Alias AS SalePerson,
  pc.Remarks,
  pc.Remarks2,
  pc.Remarks3,
  pc.Revision,
  pc.InUse,
  pc.Status,
  pc.CreatedOn,
  S1.Alias AS CreatedBy,
  pc.ModifiedOn,
  S2.Alias AS ModifiedBy,
  pc.AccessedOn,
  S3.Alias AS AccessedBy,
  pc.Retired,
  COALESCE(qt.SampleQty, 0) AS SampleQty
FROM OrderQT AS qt
  INNER JOIN OrderQTItems AS qti ON qt.OrderQTId = qti.OrderQTId
  RIGHT JOIN OrderSCItems AS sci ON qti.OrderQTItemId = sci.OrderQTItemId
  RIGHT JOIN OrderPCItems AS pci ON sci.OrderSCItemsId = pci.OrderSCItemsId
  RIGHT JOIN OrderPC AS pc ON pci.OrderPCId = pc.OrderPCId
  LEFT JOIN Staff AS S2 ON S2.StaffId = pc.ModifiedBy
  LEFT JOIN Staff AS S3 ON S3.StaffId = pc.AccessedBy
  LEFT JOIN Staff AS S1 ON S1.StaffId = pc.CreatedBy
  LEFT JOIN Supplier AS supp ON pc.SupplierId = supp.SupplierId
  LEFT JOIN Staff AS S ON S.StaffId = pc.StaffId;

-- vwPurchaseHistory
DROP VIEW IF EXISTS vwPurchaseHistory CASCADE;
CREATE OR REPLACE VIEW vwPurchaseHistory AS
SELECT
  Customer.CustomerId,
  Supplier.SupplierId,
  Supplier.SupplierName AS SuppName,
  OrderPC.PCNumber,
  Customer.CustomerName AS CustName,
  OrderQTItems.CustRef,
  Article.ArticleCode,
  Article.ArticleName AS ArtName,
  (T_Package.PackageName || ' ' || OrderQTPackage.InnerBox::text || ' ' || OrderQTPackage.Unit || '/ ' || OrderQTPackage.OuterBox::text || ' ' || OrderQTPackage.Unit || '/ ' || OrderQTPackage.CUFT::text || ' CUFT.') AS Packing,
  OrderQTSuppShipping.QtyOrdered AS ScheduledQty,
  COALESCE((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId), '') AS OrderedCny,
  OrderQTItems.Amount AS OrderedPrice,
  OrderQTItems.Unit AS OrderedUnit,
  COALESCE((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId), '') AS FactoryCny,
  OrderQTItems.FactoryCost,
  OrderQTPackage.Unit AS FactoryUnit,
  OrderQTSuppShipping.DateShipped AS ScheduledShipmentDate
FROM OrderQTSuppShipping
  LEFT JOIN OrderQTItems ON OrderQTSuppShipping.OrderQTItemId = OrderQTItems.OrderQTItemId
  LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN OrderQTSupplier ON OrderQTPackage.OrderQTItemId = OrderQTSupplier.OrderQTItemId
  LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  LEFT JOIN OrderSCItems LEFT JOIN OrderPCItems ON OrderSCItems.OrderSCItemsId = OrderPCItems.OrderSCItemsId
  LEFT JOIN OrderPC ON OrderPC.OrderPCId = OrderPCItems.OrderPCId
  ON OrderQTSuppShipping.OrderQTItemId = OrderSCItems.OrderQTItemId
  LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  LEFT JOIN OrderQT LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId
  ON OrderQT.OrderQTId = OrderQTItems.OrderQTId
  LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId;

-- vwQuoteHistory
DROP VIEW IF EXISTS vwQuoteHistory CASCADE;
CREATE OR REPLACE VIEW vwQuoteHistory AS
SELECT
  OrderQTItems.OrderQTItemId,
  COALESCE(Article.ArticleCode, '') AS ArticleCode,
  COALESCE(Supplier.SupplierCode, '') AS SupplierCode,
  COALESCE(T_Package.PackageCode, '') AS PackageCode,
  COALESCE(Customer.CustomerName, '') AS CustomerName,
  COALESCE(OrderQTItems.CustRef, '') AS CustRef,
  OrderQT.QTDate,
  COALESCE(OrderQT.QTNumber, '') AS QTNumber,
  COALESCE(OrderQTItems.Margin, 0) AS Margin,
  COALESCE(CASE
    WHEN OrderQTItems.PriceType = 0 THEN 'C'
    WHEN OrderQTItems.PriceType = 1 THEN 'F'
    WHEN OrderQTItems.PriceType = 2 THEN 'L'
    ELSE '' END, '') AS PriceType,
  OrderQTItems.Amount,
  COALESCE((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId), '') AS CurrencyCode,
  COALESCE(OrderQTItems.FactoryCost, 0) AS FactoryCost,
  COALESCE((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId), '') AS CurrencyUsed,
  COALESCE(OrderQTPackage.InnerBox, 0) AS InnerBox,
  COALESCE(OrderQTPackage.OuterBox, 0) AS OuterBox,
  COALESCE(OrderQTPackage.CUFT, 0) AS CUFT,
  COALESCE(OrderQTItems.Unit, '') AS Unit,
  COALESCE(Article.SKU, '') AS SKU
FROM OrderQTItems
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId
  LEFT JOIN OrderQTSupplier ON OrderQTItems.OrderQTItemId = OrderQTSupplier.OrderQTItemId
  LEFT JOIN OrderQTPackage ON OrderQTSupplier.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN Article ON Article.ArticleId = OrderQTItems.ArticleId
  LEFT JOIN T_Package ON T_Package.PackageId = OrderQTItems.PackageId
  LEFT JOIN Supplier ON Supplier.SupplierId = OrderQTItems.SupplierId;

-- vwRptInvoiceList
DROP VIEW IF EXISTS vwRptInvoiceList CASCADE;
CREATE OR REPLACE VIEW vwRptInvoiceList AS
SELECT
  OrderIN.INNumber,
  OrderINItems.LineNumber AS INLineNo,
  OrderQT.QTNumber,
  OrderQTItems.LineNumber AS QTLineNo,
  OrderSC.YourOrderNo,
  OrderSC.SCNumber,
  OrderQTItems.CustRef,
  Article.ArticleId,
  Article.ArticleCode,
  Article.ArticleName AS ArtName,
  T_AgeGrading.AgeGradingName AS AgeGrading,
  Article.ColorPattern,
  OrderQTItems.Particular,
  T_Package.PackageName AS Package,
  OrderQTPackage.InnerBox,
  OrderQTPackage.OuterBox,
  OrderQTPackage.CUFT,
  OrderQTPackage.Unit AS UoM,
  OrderINItems.Qty AS InvoiceQty,
  OrderQTItems.Unit,
  OrderQTItems.Amount AS UnitAmount,
  T_Currency.CurrencyCode AS CurrencyUsed,
  OrderIN.INDate,
  Customer.CustomerName AS CustName,
  CustomerAddress.AddrText AS CustAddr,
  CustomerAddress.Phone1_Label,
  CustomerAddress.Phone1_Text,
  CustomerAddress.Phone2_Label,
  CustomerAddress.Phone2_Text,
  CustomerAddress.Phone3_Label,
  CustomerAddress.Phone3_Text,
  CustomerAddress.Phone4_Label,
  CustomerAddress.Phone4_Text,
  CustomerAddress.Phone5_Label,
  CustomerAddress.Phone5_Text,
  OrderIN.YourRef,
  OrderIN.Carrier,
  OrderIN.ShipmentDate AS DepartureDate,
  OrderIN.Remarks,
  OrderIN.Remarks2,
  OrderIN.Remarks3,
  T_PaymentTerms.TermsName AS PayTerms,
  T_PaymentTerms_1.TermsName AS PricingTerms,
  T_Port_2.PortName AS LoadingPort,
  T_Port_1.PortName AS DischargePort,
  T_Port.PortName AS Destination,
  T_Origin.OriginName AS Origin,
  OrderQTPackage.SizeLength_cm,
  OrderQTPackage.SizeWidth_cm,
  OrderQTPackage.SizeHeight_cm,
  OrderQTPackage.WeightNet_kg,
  OrderQTPackage.WeightGross_kg,
  OrderQTItems.ShippingMark,
  OrderQTPackage.SizeLength_in,
  OrderQTPackage.SizeWidth_in,
  OrderQTPackage.SizeHeight_in
FROM OrderIN
  LEFT JOIN T_Port ON OrderIN.Destination = T_Port.PortId
  LEFT JOIN Customer ON OrderIN.CustomerId = Customer.CustomerId
  LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId
  LEFT JOIN T_PaymentTerms ON OrderIN.PaymentTerms = T_PaymentTerms.TermsId
  LEFT JOIN T_PaymentTerms AS T_PaymentTerms_1 ON OrderIN.PricingTerms = T_PaymentTerms_1.TermsId
  LEFT JOIN T_Port AS T_Port_2 ON OrderIN.LoadingPort = T_Port_2.PortId
  LEFT JOIN T_Port AS T_Port_1 ON OrderIN.DischargePort = T_Port_1.PortId
  LEFT JOIN T_Origin ON OrderIN.OriginId = T_Origin.OriginId
  RIGHT JOIN OrderINItems ON OrderIN.OrderINId = OrderINItems.OrderINId
  LEFT JOIN OrderSCItems ON OrderINItems.OrderSCItemsId = OrderSCItems.OrderSCItemsId
  LEFT JOIN OrderQTItems ON OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId
  LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId
  LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN T_AgeGrading ON Article.AgeGradingId = T_AgeGrading.AgeGradingId
  LEFT JOIN T_Currency ON OrderQT.CurrencyId = T_Currency.CurrencyId;

-- vwRptInvoice_Charges
DROP VIEW IF EXISTS vwRptInvoice_Charges CASCADE;
CREATE OR REPLACE VIEW vwRptInvoice_Charges AS
SELECT
  OrderIN.OrderINId,
  OrderIN.INNumber,
  OrderINCharges.OrderINChargeId,
  OrderINCharges.ChargeId,
  OrderINCharges.Description,
  OrderINCharges.Amount
FROM OrderIN
  INNER JOIN OrderINCharges ON OrderIN.OrderINId = OrderINCharges.OrderINId;

-- vwRptPreOrderList
DROP VIEW IF EXISTS vwRptPreOrderList CASCADE;
CREATE OR REPLACE VIEW vwRptPreOrderList AS
SELECT
  OrderPL.PLNumber,
  OrderPL.PLDate,
  OrderPL.Revision,
  Customer.CustomerName AS CustName,
  OrderPLItems.LineNumber AS PLLineNo,
  OrderQT.QTNumber,
  OrderQTItems.LineNumber AS QTLineNo,
  Article.SKU,
  Article.ArticleId,
  Article.ArticleCode,
  Supplier.SupplierCode,
  Supplier.SupplierName AS SuppName,
  T_Package.PackageName AS Package,
  Article.ArticleName AS ArtName,
  T_AgeGrading.AgeGradingName AS AgeGrading,
  Article.ColorPattern,
  OrderQTItems.Particular,
  OrderQTItems.CustRef,
  OrderQTItems.Qty AS OrderedQty,
  ocny.CurrencyCode AS OrderedCny,
  OrderQTItems.Unit AS OrderedUnit,
  OrderQTItems.Amount AS QuotedUnitAmt,
  OrderQTItems.FactoryCost,
  OrderQTItems.Margin,
  OrderQTItems.FCL,
  OrderQTItems.LCL,
  OrderQTSupplier.SuppRef,
  T_Currency.CurrencyCode AS FactoryCny,
  OrderQTSupplier.FCLCost,
  OrderQTSupplier.LCLCost,
  OrderQTPackage.InnerBox,
  OrderQTPackage.OuterBox,
  OrderQTPackage.Unit AS PackingUnit,
  OrderQTPackage.CUFT,
  CustomerAddress.AddrText AS CustAddr,
  CustomerAddress.Phone1_Label,
  CustomerAddress.Phone1_Text,
  CustomerAddress.Phone2_Label,
  CustomerAddress.Phone2_Text,
  CustomerAddress.Phone3_Label,
  CustomerAddress.Phone3_Text,
  CustomerAddress.Phone4_Label,
  CustomerAddress.Phone4_Text,
  CustomerAddress.Phone5_Label,
  CustomerAddress.Phone5_Text
FROM OrderPLItems
  LEFT JOIN OrderPL ON OrderPLItems.OrderPLId = OrderPL.OrderPLId
  LEFT JOIN OrderQTItems ON OrderPLItems.OrderQTItemId = OrderQTItems.OrderQTItemId
  LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN OrderQTSupplier ON OrderQTPackage.OrderQTItemId = OrderQTSupplier.OrderQTItemId
  LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  LEFT JOIN Customer ON OrderPL.CustomerId = Customer.CustomerId
  LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId
  LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId
  LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  LEFT JOIN T_AgeGrading ON Article.AgeGradingId = T_AgeGrading.AgeGradingId
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN T_Currency ON OrderQTSupplier.CurrencyId = T_Currency.CurrencyId
  LEFT JOIN T_Currency AS ocny ON OrderQT.CurrencyId = ocny.CurrencyId;

-- vwRptPreOrderList_CustShipment
DROP VIEW IF EXISTS vwRptPreOrderList_CustShipment CASCADE;
CREATE OR REPLACE VIEW vwRptPreOrderList_CustShipment AS
SELECT
  OrderPL.PLNumber,
  OrderPLItems.LineNumber,
  Article.ArticleCode,
  Article.ArticleName,
  OrderQTCustShipping.ShippedOn,
  OrderQTCustShipping.QtyOrdered,
  OrderQTCustShipping.QtyShipped,
  OrderQTCustShipping.Status
FROM OrderPLItems
  LEFT JOIN OrderPL ON OrderPLItems.OrderPLId = OrderPL.OrderPLId
  RIGHT JOIN OrderQTCustShipping ON OrderPLItems.OrderQTItemId = OrderQTCustShipping.OrderQTItemId
  LEFT JOIN Article RIGHT JOIN OrderQTItems ON Article.ArticleId = OrderQTItems.ArticleId
  ON OrderQTCustShipping.OrderQTItemId = OrderQTItems.OrderQTItemId;

-- vwRptPreOrderList_SuppShipment
DROP VIEW IF EXISTS vwRptPreOrderList_SuppShipment CASCADE;
CREATE OR REPLACE VIEW vwRptPreOrderList_SuppShipment AS
SELECT
  OrderPL.PLNumber,
  OrderPLItems.LineNumber,
  Article.ArticleCode,
  Article.ArticleName,
  OrderQTSuppShipping.DateShipped,
  OrderQTSuppShipping.QtyOrdered,
  OrderQTSuppShipping.QtyShipped,
  OrderQTSuppShipping.Status
FROM OrderPLItems
  LEFT JOIN OrderPL ON OrderPLItems.OrderPLId = OrderPL.OrderPLId
  RIGHT JOIN OrderQTSuppShipping ON OrderPLItems.OrderQTItemId = OrderQTSuppShipping.OrderQTItemId
  LEFT JOIN OrderQTItems LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  ON OrderQTSuppShipping.OrderQTItemId = OrderQTItems.OrderQTItemId;

-- vwRptPriceList
DROP VIEW IF EXISTS vwRptPriceList CASCADE;
CREATE OR REPLACE VIEW vwRptPriceList AS
SELECT
  qt.QTNumber,
  qt.QTDate,
  cust.CustomerName AS CustName,
  custad.AddrText AS CustAddr,
  custad.Phone1_Label,
  custad.Phone1_Text,
  custad.Phone2_Label,
  custad.Phone2_Text,
  custad.Phone3_Label,
  custad.Phone3_Text,
  custad.Phone4_Label,
  custad.Phone4_Text,
  custad.Phone5_Label,
  custad.Phone5_Text,
  pyt.TermsName AS PayTerms,
  curr.CurrencyCode AS CurrencyUsed,
  qt.Remarks,
  qt.Remarks2,
  qt.Remarks3,
  qti.LineNumber AS QTLineNo,
  qti.CustRef,
  a.ArticleCode,
  a.ArticleName AS ArtName,
  ag.AgeGradingName AS AgeGrading,
  p.PackageName AS Package,
  qti.Particular,
  qti.Carton,
  qti.Qty,
  qti.Unit,
  qti.Amount AS UnitAmt,
  qtp.InnerBox,
  qtp.OuterBox,
  qtp.Unit AS PackageUnit,
  qtp.CUFT,
  qti.ShippingMark,
  qti.SampleQty,
  OrderQTSupplier.SuppRef,
  Supplier.SupplierName,
  a.ArticleId,
  a.ColorPattern
FROM OrderQT AS qt
  LEFT JOIN Customer AS cust ON qt.CustomerId = cust.CustomerId
  LEFT JOIN CustomerAddress AS custad ON cust.CustomerId = custad.CustomerId
  LEFT JOIN T_PaymentTerms AS pyt ON qt.TermsId = pyt.TermsId
  LEFT JOIN T_Currency AS curr ON qt.CurrencyId = curr.CurrencyId
  LEFT JOIN OrderQTItems AS qti ON qti.OrderQTId = qt.OrderQTId
  LEFT JOIN Article AS a ON a.ArticleId = qti.ArticleId
  LEFT JOIN T_AgeGrading AS ag ON ag.AgeGradingId = a.AgeGradingId
  LEFT JOIN T_Package AS p ON qti.PackageId = p.PackageId
  LEFT JOIN OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId
  LEFT JOIN OrderQTSupplier ON OrderQTSupplier.OrderQTItemId = qti.OrderQTItemId
  LEFT JOIN Supplier ON qti.SupplierId = Supplier.SupplierId
WHERE qt.QTNumber IS NOT NULL;

-- vwRptProformaInvoiceList
DROP VIEW IF EXISTS vwRptProformaInvoiceList CASCADE;
CREATE OR REPLACE VIEW vwRptProformaInvoiceList AS
SELECT
  sc.SCNumber,
  sci.LineNumber AS SCLineNo,
  qti.CustRef,
  a.ArticleCode,
  a.ArticleName AS ArtName,
  p.PackageName AS Package,
  qti.Carton,
  qti.Qty,
  qt.Unit,
  qti.Amount AS UnitAmt,
  cny.CurrencyCode AS CurrencyUsed,
  qtp.InnerBox,
  qtp.OuterBox,
  qtp.Unit AS PackageUnit,
  qtp.CUFT,
  sc.SCDate,
  cust.CustomerName AS CustName,
  custa.AddrText AS CustAddr,
  sc.YourOrderNo,
  sc.YourRef,
  sc.Carrier,
  tpt.TermsName AS PayTerms,
  lp.PortName AS LoadingPort,
  dsp.PortName AS Destination,
  o.OriginName AS Origin,
  tpy.TermsName AS PricingTerms,
  dp.PortName AS DischargePort,
  qti.ShippingMark,
  qtcs.ShippedOn,
  qtcs.QtyShipped,
  custa.Phone1_Label,
  custa.Phone1_Text,
  custa.Phone2_Label,
  custa.Phone2_Text,
  custa.Phone3_Label,
  custa.Phone3_Text,
  custa.Phone4_Label,
  custa.Phone4_Text,
  custa.Phone5_Label,
  custa.Phone5_Text
FROM T_Port AS dp
  RIGHT JOIN OrderSC AS sc
    LEFT JOIN T_Port AS dsp ON sc.Destination = dsp.PortId
    LEFT JOIN Customer AS cust ON sc.CustomerId = cust.CustomerId
    LEFT JOIN CustomerAddress AS custa ON cust.CustomerId = custa.CustomerId
    LEFT JOIN T_PaymentTerms AS tpt ON sc.PaymentTerms = tpt.TermsId
    LEFT JOIN T_PaymentTerms AS tpy ON sc.PricingTerms = tpy.TermsId
    LEFT JOIN T_Port AS lp ON sc.LoadingPort = lp.PortId
  ON dp.PortId = sc.DischargePort
  LEFT JOIN T_Origin AS o ON sc.OriginId = o.OriginId
  RIGHT JOIN OrderSCItems AS sci ON sc.OrderSCId = sci.OrderSCId
  LEFT JOIN Article AS a RIGHT JOIN OrderQTItems AS qti ON a.ArticleId = qti.ArticleId ON sci.OrderQTItemId = qti.OrderQTItemId
  LEFT JOIN T_Package AS p ON qti.PackageId = p.PackageId
  LEFT JOIN OrderQT AS qt ON qti.OrderQTId = qt.OrderQTId
  LEFT JOIN OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId
  LEFT JOIN T_Currency AS cny ON qt.CurrencyId = cny.CurrencyId
  LEFT JOIN OrderQTCustShipping AS qtcs ON qtcs.OrderQTItemId = qti.OrderQTItemId;

-- vwRptPurchaseContractList
DROP VIEW IF EXISTS vwRptPurchaseContractList CASCADE;
CREATE OR REPLACE VIEW vwRptPurchaseContractList AS
SELECT
  pc.PCNumber,
  pci.LineNumber AS PCLineNo,
  pc.PCDate,
  qt.QTNumber,
  qti.LineNumber AS QTLineNo,
  a.ArticleCode,
  qti.CustRef,
  qts.SuppRef,
  a.ArticleId,
  a.ArticleName AS ArtName,
  p.PackageName AS Package,
  ag.AgeGradingName AS AgeGrading,
  a.ColorPattern,
  qti.Particular,
  qti.Carton,
  qti.Qty AS OrderedQty,
  qti.Unit AS OrderedUnit,
  qti.FactoryCost AS UnitCost,
  cny.CurrencyCode AS CostCny,
  supp.SupplierName AS SuppName,
  suppa.AddrText AS SuppAddr,
  suppa.Phone1_Label,
  suppa.Phone1_Text,
  suppa.Phone2_Label,
  suppa.Phone2_Text,
  suppa.Phone3_Label,
  suppa.Phone3_Text,
  suppa.Phone4_Label,
  suppa.Phone4_Text,
  suppa.Phone5_Label,
  suppa.Phone5_Text,
  pc.YourRef,
  pc.Carrier,
  pyt.TermsName AS PayTerms,
  pyp.TermsName AS PricingTerms,
  lp.PortName AS LoadingPort,
  dp.PortName AS DischargePort,
  c.CountryName AS Destination,
  o.OriginName AS Origin,
  pc.Remarks,
  pc.Remarks2,
  pc.Remarks3,
  qtp.Unit AS PackUnit,
  qtp.InnerBox,
  qtp.OuterBox,
  qtp.CUFT,
  qti.ShippingMark,
  qtss.DateShipped,
  qtss.QtyShipped
FROM OrderPCItems AS pci
  LEFT JOIN OrderPC AS pc ON pci.OrderPCId = pc.OrderPCId
  LEFT JOIN Supplier AS supp
    LEFT JOIN SupplierAddress AS suppa ON supp.SupplierId = suppa.SupplierId
  ON pc.SupplierId = supp.SupplierId
  LEFT JOIN T_PaymentTerms AS pyt ON pc.PaymentTerms = pyt.TermsId
  LEFT JOIN T_PaymentTerms AS pyp ON pc.PricingTerms = pyp.TermsId
  LEFT JOIN T_Port AS lp ON pc.LoadingPort = lp.PortId
  LEFT JOIN T_Port AS dp ON pc.DischargePort = dp.PortId
  LEFT JOIN T_Country AS c ON pc.Destination = c.CountryId
  LEFT JOIN T_Origin AS o ON pc.OriginId = o.OriginId
  LEFT JOIN OrderSCItems AS sci ON pci.OrderSCItemsId = sci.OrderSCItemsId
  LEFT JOIN OrderQTItems AS qti ON sci.OrderQTItemId = qti.OrderQTItemId
  LEFT JOIN Article AS a ON qti.ArticleId = a.ArticleId
  LEFT JOIN T_Package AS p ON qti.PackageId = p.PackageId
  LEFT JOIN OrderQTSupplier AS qts ON qti.OrderQTItemId = qts.OrderQTItemId
  LEFT JOIN T_Currency AS cny ON qts.CurrencyId = cny.CurrencyId
  LEFT JOIN OrderQT AS qt ON qti.OrderQTItemId = qt.OrderQTId
  LEFT JOIN OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId
  LEFT JOIN T_AgeGrading AS ag ON a.AgeGradingId = ag.AgeGradingId
  LEFT JOIN OrderQTSuppShipping AS qtss ON qti.OrderQTItemId = qtss.OrderQTItemId;

-- vwRptPurchaseContractShipmentList
DROP VIEW IF EXISTS vwRptPurchaseContractShipmentList CASCADE;
CREATE OR REPLACE VIEW vwRptPurchaseContractShipmentList AS
SELECT
  OrderPC.PCNumber,
  OrderPCItems.LineNumber,
  Article.ArticleCode,
  Article.ArticleName,
  OrderQTSuppShipping.DateShipped,
  OrderQTSuppShipping.QtyOrdered,
  OrderQTSuppShipping.QtyShipped,
  OrderQTSuppShipping.Status
FROM OrderPC
  RIGHT JOIN OrderPCItems ON OrderPC.OrderPCId = OrderPCItems.OrderPCId
  RIGHT JOIN OrderSCItems
    RIGHT JOIN OrderQTSuppShipping ON OrderSCItems.OrderQTItemId = OrderQTSuppShipping.OrderQTItemId
  LEFT JOIN Article RIGHT JOIN OrderQTItems ON Article.ArticleId = OrderQTItems.ArticleId
  ON OrderQTSuppShipping.OrderQTItemId = OrderQTItems.OrderQTItemId
  ON OrderPCItems.OrderSCItemsId = OrderSCItems.OrderSCItemsId;

-- vwRptSalesContractList
-- DROP VIEW IF EXISTS vwRptSalesContractList CASCADE;
-- CREATE OR REPLACE VIEW vwRptSalesContractList AS
-- SELECT
--   sc.SCNumber,
--   sci.LineNumber AS SCLineNo,
--   qt.QTNumber,
--   qti.LineNumber AS QTLineNo,
--   qti.CustRef,
--   a.ArticleId,
--   a.ArticleCode,
--   a.ArticleName AS ArtName,
--   ag.AgeGradingName AS AgeGrading,
--   a.ColorPattern,
--   p.PackageName AS Package,
--   qti.Particular,
--   qti.Carton,
--   qti.Qty,
--   qti.Unit,
--   qti.Amount AS UnitAmt,
--   curr.CurrencyCode AS CurrencyUsed,
--   qtp.InnerBox,
--   qtp.OuterBox,
--   qtp.Unit AS PackageUnit,
--   qtp.CUFT,
--   sc.SCDate,
--   cust.CustomerName AS CustName,
--   custad.AddrText AS CustAddr,
--   custad.Phone1_Label,
--   custad.Phone1_Text,
--   custad.Phone2_Label,
--   custad.Phone2_Text,
--   custad.Phone3_Label,
--   custad.Phone3_Text,
--   custad.Phone4_Label,
--   custad.Phone4_Text,
--   custad.Phone5_Label,
--   custad.Phone5_Text,
--   sc.YourOrderNo,
--   sc.YourRef,
--   sc.Carrier,
--   pyt.TermsName AS PayTerms,
--   lp.PortName AS LoadingPort,
--   o.OriginName AS Origin,
--   sc.Remarks,
--   sc.Remarks2,
--   sc.Remarks3,
--   qti.ShippingMark,
--   qtcs.ShippedOn,
--   qtcs.QtyShipped,
--   dp.PortName AS DischargePort,
--   Dest.PortName AS Destination,
--   pct.TermsName AS PricingTerms
-- FROM OrderSC AS sc
--   LEFT JOIN Customer AS cust ON sc.CustomerId = cust.CustomerId
--   LEFT JOIN CustomerAddress AS custad ON cust.CustomerId = custad.CustomerId
--   LEFT JOIN T_PaymentTerms AS pyt ON sc.PaymentTerms = pyt.TermsId
--   LEFT JOIN T_PaymentTerms AS pct ON sc.PricingTerms = pct.TermsId
--   LEFT JOIN T_Port AS lp ON sc.LoadingPort = lp.PortId
--   LEFT JOIN T_Port AS dp ON sc.DischargePort = dp.PortId
--   LEFT JOIN T_Port AS Dest ON sc.Destination = Dest.PortId
--   LEFT JOIN T_Origin AS o ON sc.OriginId = o.OriginId
--   LEFT JOIN OrderSCItems AS sci ON sc.OrderSCId = sci.OrderSCId
--   RIGHT JOIN T_AgeGrading AS ag RIGHT JOIN Article AS a ON ag.AgeGradingId = a.AgeGradingId RIGHT JOIN OrderQTItems AS qti ON a.ArticleId = qti.ArticleId
--   LEFT JOIN OrderQTCustShipping AS qtcs ON qti.OrderQTItemId = qtcs.OrderQTItemId
--   LEFT JOIN T_Package AS p ON qti.PackageId = p.PackageId
--   LEFT JOIN OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId
--   LEFT JOIN T_Currency AS curr RIGHT JOIN OrderQT AS qt ON curr.CurrencyId = qt.CurrencyId ON qti.OrderQTId = qt.OrderQTId
-- WHERE qt.QTNumber IS NOT NULL;
DROP VIEW IF EXISTS vwrptsalescontractlist CASCADE;

CREATE OR REPLACE VIEW vwrptsalescontractlist AS
SELECT
    sc.scnumber,
    sci.linenumber AS sclineno,
    qt.qtnumber,
    qti.linenumber AS qtlineno,
    qti.custref,
    a.articleid,
    a.articlecode,
    a.articlename AS artname,
    ag.agegradingname AS agegrading,
    a.colorpattern,
    p.packagename AS package,
    qti.particular,
    qti.carton,
    qti.qty,
    qti.unit,
    qti.amount AS unitamt,
    curr.currencycode AS currencyused,
    qtp.innerbox,
    qtp.outerbox,
    qtp.unit AS packageunit,
    qtp.cuft,
    sc.scdate,
    cust.customername AS custname,
    custad.addrtext AS custaddr,
    custad.phone1_label,
    custad.phone1_text,
    custad.phone2_label,
    custad.phone2_text,
    custad.phone3_label,
    custad.phone3_text,
    custad.phone4_label,
    custad.phone4_text,
    custad.phone5_label,
    custad.phone5_text,
    sc.yourorderno,
    sc.yourref,
    sc.carrier,
    pyt.termsname AS payterms,
    lp.portname AS loadingport,
    o.originname AS origin,
    sc.remarks,
    sc.remarks2,
    sc.remarks3,
    qti.shippingmark,
    qtcs.shippedon,
    qtcs.qtyshipped,
    dp.portname AS dischargeport,
    dest.portname AS destination,
    pct.termsname AS pricingterms
FROM ordersc AS sc
    LEFT JOIN orderscitems AS sci ON sc.orderscid = sci.orderscid
    LEFT JOIN orderqtitems AS qti ON sci.orderqtitemid = qti.orderqtitemid   -- correct direct link!
    LEFT JOIN article AS a ON qti.articleid = a.articleid
    LEFT JOIN t_agegrading AS ag ON a.agegradingid = ag.agegradingid
    LEFT JOIN t_package AS p ON qti.packageid = p.packageid
    LEFT JOIN orderqtpackage AS qtp ON qti.orderqtitemid = qtp.orderqtitemid
    LEFT JOIN orderqtcustshipping AS qtcs ON qti.orderqtitemid = qtcs.orderqtitemid
    LEFT JOIN orderqt AS qt ON qti.orderqtid = qt.orderqtid
    LEFT JOIN t_currency AS curr ON qt.currencyid = curr.currencyid
    LEFT JOIN customer AS cust ON sc.customerid = cust.customerid
    LEFT JOIN customeraddress AS custad ON cust.customerid = custad.customerid
    LEFT JOIN t_paymentterms AS pyt ON sc.paymentterms = pyt.termsid
    LEFT JOIN t_paymentterms AS pct ON sc.pricingterms = pct.termsid
    LEFT JOIN t_port AS lp ON sc.loadingport = lp.portid
    LEFT JOIN t_port AS dp ON sc.dischargeport = dp.portid
    LEFT JOIN t_port AS dest ON sc.destination = dest.portid
    LEFT JOIN t_origin AS o ON sc.originid = o.originid
WHERE qt.qtnumber IS NOT NULL;
/* Why this is correct

Starts from ordersc (sales contract header)
Joins to its lines sci
Directly joins to the exact quotation line qti via orderqtitemid — precise, no ambiguity, no Cartesian product
Then pulls article, package, shipping, quotation header, currency, customer info, ports, etc.
Keeps the WHERE qt.qtnumber IS NOT NULL to exclude any orphaned/incomplete records (as in your original)

This should now:

Run without errors
Return accurate, non-duplicated rows
Match the logical intent of your original T-SQL view (but much cleaner and safer than the old nested RIGHT JOIN mess)
 */

-- vwRptSalesContractShipmentList
DROP VIEW IF EXISTS vwRptSalesContractShipmentList CASCADE;
CREATE OR REPLACE VIEW vwRptSalesContractShipmentList AS
SELECT
  OrderSC.SCNumber,
  OrderSCItems.LineNumber,
  Article.ArticleCode,
  Article.ArticleName,
  OrderQTCustShipping.ShippedOn,
  OrderQTCustShipping.QtyOrdered,
  OrderQTCustShipping.QtyShipped,
  OrderQTCustShipping.Status
FROM Article RIGHT JOIN OrderQTItems ON Article.ArticleId = OrderQTItems.ArticleId
  RIGHT JOIN OrderQTCustShipping ON OrderQTItems.OrderQTItemId = OrderQTCustShipping.OrderQTItemId
  LEFT JOIN OrderSCItems ON OrderQTCustShipping.OrderQTItemId = OrderSCItems.OrderQTItemId
  LEFT JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId;

-- vwRptShipmentAdviseList
DROP VIEW IF EXISTS vwRptShipmentAdviseList CASCADE;
CREATE OR REPLACE VIEW vwRptShipmentAdviseList AS
SELECT
  Customer.CustomerName AS CustName,
  OrderIN.ShipmentDate,
  OrderIN.Carrier,
  T_Port.PortName AS FromPort,
  T_Port_1.PortName AS ToPort,
  OrderIN.INNumber,
  OrderINItems.LineNumber AS INLine,
  OrderSC.SCNumber,
  OrderQTItems.CustRef,
  Article.ArticleCode AS OurRef,
  OrderINItems.Qty AS InvoicedQty,
  OrderQTItems.Unit AS UoM,
  OrderINItems.Qty / NULLIF(OrderQTItems.Qty, 0) * OrderQTItems.Carton AS Carton,
  OrderINItems.Qty * OrderQTItems.Amount AS Amount,
  T_Currency.CurrencyCode AS CurrencyUsed,
  T_PaymentTerms.TermsName AS PricingTerms,
  OrderQTPackage.Unit AS FactoryUnit,
  OrderQTPackage.InnerBox,
  OrderQTPackage.OuterBox,
  OrderQTPackage.CUFT
FROM OrderIN
  LEFT JOIN OrderINItems ON OrderIN.OrderINId = OrderINItems.OrderINId
  LEFT JOIN Customer ON OrderIN.CustomerId = Customer.CustomerId
  LEFT JOIN OrderSCItems ON OrderINItems.OrderSCItemsId = OrderSCItems.OrderSCItemsId
  LEFT JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId
  LEFT JOIN OrderQTItems ON OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId
  LEFT JOIN T_Port ON OrderIN.LoadingPort = T_Port.PortId
  LEFT JOIN T_Port AS T_Port_1 ON OrderIN.DischargePort = T_Port_1.PortId
  LEFT JOIN T_PaymentTerms ON OrderIN.PricingTerms = T_PaymentTerms.TermsId
  LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
  LEFT JOIN OrderQTPackage ON OrderSCItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  LEFT JOIN Article ON Article.ArticleId = OrderQTItems.ArticleId
  LEFT JOIN T_Currency ON T_Currency.CurrencyId = OrderQT.CurrencyId;

-- vwSalesContractItemList
DROP VIEW IF EXISTS vwSalesContractItemList CASCADE;
CREATE OR REPLACE VIEW vwSalesContractItemList AS
SELECT
  m.OrderSCId,
  m.SCNumber,
  m.SCDate,
  i.OrderSCItemsId,
  i.LineNumber,
  a.ArticleId,
  a.SKU,
  a.ArticleCode,
  a.ArticleName,
  a.ArticleName_Chs,
  a.ArticleName_Cht,
  p.PackageId,
  p.PackageCode,
  p.PackageName,
  p.PackageName_Chs,
  p.PackageName_Cht,
  s.SupplierId,
  s.SupplierCode,
  s.SupplierName,
  s.SupplierName_Chs,
  s.SupplierName_Cht,
  qi.Particular,
  qi.CustRef,
  qi.PriceType,
  qi.FactoryCost,
  qi.Margin,
  qi.FCL,
  qi.LCL,
  qi.SampleQty,
  qi.Qty,
  qi.Unit,
  qi.Amount,
  qi.Carton,
  qi.GLAccount,
  qi.RefDocNo,
  qi.ShippingMark,
  qi.QtyIN,
  qi.QtyOUT,
  ps.SuppRef,
  c.CurrencyCode,
  ps.FCLCost,
  ps.LCLCost,
  qi.OrderQTItemId,
  ap.InnerBox,
  ap.OuterBox,
  ap.CUFT
FROM OrderSC AS m
  INNER JOIN Supplier AS s
    INNER JOIN ArticleSupplier AS ps
      INNER JOIN OrderSCItems AS i
        LEFT JOIN OrderQTItems AS qi ON i.OrderQTItemId = qi.OrderQTItemId
      INNER JOIN Article AS a ON qi.ArticleId = a.ArticleId
    ON ps.ArticleId = qi.ArticleId AND ps.SupplierId = qi.SupplierId
  ON s.SupplierId = ps.SupplierId
  INNER JOIN T_Currency AS c ON ps.CurrencyId = c.CurrencyId
  ON m.OrderSCId = i.OrderSCId
  INNER JOIN T_Package AS p ON qi.PackageId = p.PackageId
  INNER JOIN OrderQTPackage AS ap ON qi.OrderQTItemId = ap.OrderQTItemId;

-- vwSalesContractList
DROP VIEW IF EXISTS vwSalesContractList CASCADE;
CREATE OR REPLACE VIEW vwSalesContractList AS
SELECT
  sc.OrderSCId,
  sc.SCNumber,
  sc.SCDate,
  sc.CustomerId,
  c.CustomerCode,
  c.CustomerName,
  c.CustomerName_Chs,
  c.CustomerName_Cht,
  sc.StaffId,
  s.Alias AS SalePerson,
  sc.SendFrom,
  sc.SendTo,
  sc.Remarks,
  sc.Remarks2,
  sc.Remarks3,
  sc.Revision,
  sc.InUse,
  sc.Status,
  sc.CreatedOn,
  s1.Alias AS CreatedBy,
  sc.ModifiedOn,
  s2.Alias AS ModifiedBy,
  sc.AccessedOn,
  s3.Alias AS AccessedBy,
  sc.Retired
FROM OrderSC AS sc
  LEFT JOIN Staff AS s2 ON sc.ModifiedBy = s2.StaffId
  LEFT JOIN Staff AS s3 ON sc.AccessedBy = s3.StaffId
  LEFT JOIN Staff AS s1 ON sc.CreatedBy = s1.StaffId
  LEFT JOIN Customer AS c ON sc.CustomerId = c.CustomerId
  LEFT JOIN Staff AS s ON sc.StaffId = s.StaffId;

-- vwShipmentHistory
DROP VIEW IF EXISTS vwShipmentHistory CASCADE;
CREATE OR REPLACE VIEW vwShipmentHistory AS
SELECT
  OrderQTItems.OrderQTItemId,
  Article.ArticleCode,
  Supplier.SupplierCode,
  (SELECT PackageCode FROM T_Package WHERE OrderQTItems.PackageId = T_Package.PackageId) AS PackageCode,
  OrderQTItems.CustRef,
  COALESCE(OrderQTSupplier.SuppRef, '') AS SuppRef,
  Customer.CustomerName AS CustName,
  Supplier.SupplierName AS SuppName,
  OrderSC.SCNumber,
  OrderQTCustShipping.ShippedOn AS ScheduledDate,
  OrderQTCustShipping.QtyOrdered AS ScheduledQty,
  OrderQTItems.Unit,
  OrderQTCustShipping.QtyShipped AS ShippedQty,
  (OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped) AS OSQty,
  Article.SKU,
  Customer.CustomerId,
  Supplier.SupplierId
FROM OrderQTCustShipping
  INNER JOIN OrderQTItems ON OrderQTCustShipping.OrderQTItemId = OrderQTItems.OrderQTItemId
  INNER JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId
  INNER JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId
  INNER JOIN OrderSCItems ON OrderQTCustShipping.OrderQTItemId = OrderSCItems.OrderQTItemId
  INNER JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId
  INNER JOIN Customer ON OrderSC.CustomerId = Customer.CustomerId
  INNER JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId
  INNER JOIN OrderQTSupplier ON OrderQTPackage.OrderQTItemId = OrderQTSupplier.OrderQTItemId;

-- vwUserAddressList (was vwStaffAddressList)
DROP VIEW IF EXISTS vwStaffAddressList CASCADE;
CREATE OR REPLACE VIEW vwStaffAddressList AS
SELECT
  sa.StaffId,
  sa.StaffAddressId,
  sa.DefaultRec,
  za.AddressName,
  sa.AddrText,
  sa.AddrIsMailing,
  p1.PhoneName AS PhoneLabel1,
  sa.Phone1_Text,
  p2.PhoneName AS PhoneLabel2,
  sa.Phone2_Text,
  p3.PhoneName AS PhoneLabel3,
  sa.Phone3_Text,
  p4.PhoneName AS PhoneLabel4,
  sa.Phone4_Text,
  p5.PhoneName AS PhoneLabel5,
  sa.Phone5_Text,
  COALESCE(sa.Notes, '') AS Notes,
  sa.CreatedOn,
  s1.Alias AS CreatedBy,
  sa.ModifiedOn,
  s2.Alias AS ModifiedBy,
  sa.Retired
FROM StaffAddress AS sa
  INNER JOIN Z_Phone AS p1 ON sa.Phone1_Label = p1.PhoneId
  INNER JOIN Z_Phone AS p2 ON sa.Phone2_Label = p2.PhoneId
  INNER JOIN Z_Phone AS p3 ON sa.Phone3_Label = p3.PhoneId
  INNER JOIN Z_Phone AS p4 ON sa.Phone4_Label = p4.PhoneId
  INNER JOIN Z_Phone AS p5 ON sa.Phone5_Label = p5.PhoneId
  INNER JOIN Z_Address AS za ON sa.AddressId = za.AddressId
  LEFT JOIN Staff AS s2 ON sa.ModifiedBy = s2.StaffId
  LEFT JOIN Staff AS s1 ON sa.CreatedBy = s1.StaffId;

-- vwStaffList
DROP VIEW IF EXISTS vwStaffList CASCADE;
CREATE OR REPLACE VIEW vwStaffList AS
SELECT
  d.DivisionId,
  d.DivisionCode,
  d.DivisionName,
  d.DivisionName_Chs,
  d.DivisionName_Cht,
  g.GroupId,
  g.GroupCode,
  g.GroupName,
  g.GroupName_Chs,
  g.GroupName_Cht,
  u.StaffId,
  u.StaffCode,
  u.FullName,
  u.FirstName,
  u.LastName,
  u.Alias,
  u.Login,
  u.Password,
  u.Remarks,
  u.Status,
  u.CreatedOn,
  s1.Alias AS CreatedBy,
  u.ModifiedOn,
  s2.Alias AS ModifiedBy
FROM T_Division AS d
  INNER JOIN Staff AS u ON d.DivisionId = u.DivisionId
  INNER JOIN T_Group AS g ON u.GroupId = g.GroupId
  LEFT JOIN Staff AS s2 ON u.ModifiedBy = s2.StaffId
  LEFT JOIN Staff AS s1 ON u.CreatedBy = s1.StaffId;

-- vwSupplierAddressList
DROP VIEW IF EXISTS vwSupplierAddressList CASCADE;
CREATE OR REPLACE VIEW vwSupplierAddressList AS
SELECT
  sa.SupplierAddressId,
  sa.SupplierId,
  sa.DefaultRec,
  za.AddressName,
  sa.AddrText,
  sa.AddrIsMailing,
  p1.PhoneName AS PhoneLabel1,
  sa.Phone1_Text,
  p2.PhoneName AS PhoneLabel2,
  sa.Phone2_Text,
  p3.PhoneName AS PhoneLabel3,
  sa.Phone3_Text,
  p4.PhoneName AS PhoneLabel4,
  sa.Phone4_Text,
  p5.PhoneName AS PhoneLabel5,
  sa.Phone5_Text,
  sa.Notes,
  sa.CreatedOn,
  s1.Alias AS CreatedBy,
  sa.ModifiedOn,
  s2.Alias AS ModifiedBy,
  sa.Retired
FROM SupplierAddress AS sa
  INNER JOIN Z_Address AS za ON sa.AddressId = za.AddressId
  INNER JOIN Z_Phone AS p1 ON sa.Phone1_Label = p1.PhoneId
  INNER JOIN Z_Phone AS p2 ON sa.Phone2_Label = p2.PhoneId
  INNER JOIN Z_Phone AS p3 ON sa.Phone3_Label = p3.PhoneId
  INNER JOIN Z_Phone AS p4 ON sa.Phone4_Label = p4.PhoneId
  INNER JOIN Z_Phone AS p5 ON sa.Phone5_Label = p5.PhoneId
  INNER JOIN Staff AS s1 ON sa.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON sa.ModifiedBy = s2.StaffId;

-- vwSupplierContactList
DROP VIEW IF EXISTS vwSupplierContactList CASCADE;
CREATE OR REPLACE VIEW vwSupplierContactList AS
SELECT
  sc.SupplierContactId,
  sc.SupplierId,
  sc.DefaultRec,
  zs.SalutationName,
  sc.FullName,
  sc.FirstName,
  sc.LastName,
  zj.JobTitleName,
  p1.PhoneName AS PhoneLabel1,
  sc.Phone1_Text,
  p2.PhoneName AS PhoneLabel2,
  sc.Phone2_Text,
  p3.PhoneName AS PhoneLabel3,
  sc.Phone3_Text,
  p4.PhoneName AS PhoneLabel4,
  sc.Phone4_Text,
  p5.PhoneName AS PhoneLabel5,
  sc.Phone5_Text,
  sc.Notes,
  sc.CreatedOn,
  s1.Alias AS CreatedBy,
  sc.ModifiedOn,
  s2.Alias AS ModifiedBy,
  sc.Retired
FROM SupplierContact AS sc
  INNER JOIN Z_Salutation AS zs ON sc.SalutationId = zs.SalutationId
  INNER JOIN Z_JobTitle AS zj ON sc.JobTitleId = zj.JobTitleId
  INNER JOIN Z_Phone AS p1 ON sc.Phone1_Label = p1.PhoneId
  INNER JOIN Z_Phone AS p2 ON sc.Phone2_Label = p2.PhoneId
  INNER JOIN Z_Phone AS p3 ON sc.Phone3_Label = p3.PhoneId
  INNER JOIN Z_Phone AS p4 ON sc.Phone4_Label = p4.PhoneId
  INNER JOIN Z_Phone AS p5 ON sc.Phone5_Label = p5.PhoneId
  INNER JOIN Staff AS s1 ON sc.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON sc.ModifiedBy = s2.StaffId;

-- vwSupplierList
DROP VIEW IF EXISTS vwSupplierList CASCADE;
CREATE OR REPLACE VIEW vwSupplierList AS
SELECT
  s.SupplierId,
  s.SupplierCode,
  s.SupplierName,
  s.SupplierName_Chs,
  s.SupplierName_Cht,
  s.ACNumber,
  r.RegionName,
  p.TermsName,
  s.Remarks,
  s.Status,
  s.CreatedOn,
  s1.Alias AS CreatedBy,
  s.ModifiedOn,
  s2.Alias AS ModifiedBy,
  s.Retired
FROM Supplier AS s
  INNER JOIN T_Region AS r ON s.RegionId = r.RegionId
  INNER JOIN T_PaymentTerms AS p ON s.TermsId = p.TermsId
  INNER JOIN Staff AS s1 ON s.CreatedBy = s1.StaffId
  INNER JOIN Staff AS s2 ON s.ModifiedBy = s2.StaffId;

-- vwUserList (final user list from UserProfile)
CREATE OR REPLACE VIEW vwUserList AS
SELECT
  u.UserSid,
  u.UserType,
  u.Alias,
  u.LoginName,
  u.LoginPassword,
  s.FullName,
  s.CreatedOn,
  '' AS CreatedBy,
  s.ModifiedOn,
  s2.Alias AS ModifiedBy,
  s.Status
FROM UserProfile AS u, Staff AS s, Staff AS s1, Staff AS s2
WHERE u.UserSid = s.StaffId AND s.CreatedBy = '00000000-0000-0000-0000-000000000000' AND s.ModifiedBy = s2.StaffId
UNION
SELECT
  u.UserSid,
  u.UserType,
  u.Alias,
  u.LoginName,
  u.LoginPassword,
  s.FullName,
  s.CreatedOn,
  s1.Alias,
  s.ModifiedOn,
  s2.Alias,
  s.Status
FROM UserProfile AS u, Staff AS s, Staff AS s1, Staff AS s2
WHERE u.UserSid = s.StaffId AND s.CreatedBy = s1.StaffId AND s.ModifiedBy = s2.StaffId
UNION
SELECT
  u.UserSid,
  u.UserType,
  u.Alias,
  u.LoginName,
  u.LoginPassword,
  sc.FullName,
  sc.CreatedOn,
  s1.Alias,
  sc.ModifiedOn,
  s2.Alias,
  s.Status
FROM UserProfile AS u, SupplierContact AS sc, Supplier AS s, Staff AS s1, Staff AS s2
WHERE u.UserSid = sc.SupplierContactId AND sc.SupplierId = s.SupplierId AND sc.CreatedBy = s1.StaffId AND sc.ModifiedBy = s2.StaffId
UNION
SELECT
  u.UserSid,
  u.UserType,
  u.Alias,
  u.LoginName,
  u.LoginPassword,
  cc.FullName,
  cc.CreatedOn,
  s1.Alias,
  cc.ModifiedOn,
  s2.Alias,
  c.Status
FROM UserProfile AS u, CustomerContact AS cc, Customer AS c, Staff AS s1, Staff AS s2
WHERE u.UserSid = cc.CustomerContactId AND cc.CustomerId = c.CustomerId AND cc.CreatedBy = s1.StaffId AND cc.ModifiedBy = s2.StaffId;