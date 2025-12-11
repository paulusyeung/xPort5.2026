-- dbo.OLAP source

CREATE OR ALTER VIEW [dbo].[OLAP]
AS 
SELECT     T_Region.RegionName AS Region, Customer.CustomerName AS CustName, T_PaymentTerms.TermsName AS PricingTerms, OrderIN.INDate, 
			  CAST(YEAR(OrderIN.INDate) AS VARCHAR(4)) AS [Year], CAST(MONTH(OrderIN.INDate) AS VARCHAR(2)) AS [Month], 'Q' + CAST(DATEPART(Quarter, OrderIN.INDate) AS VARCHAR(1)) AS [Quarter], OrderIN.INNumber, ISNULL
				  ((SELECT     CurrencyCode
					  FROM         T_Currency
					  WHERE     CurrencyId = OrderQT.CurrencyId), '') AS Currency, ISNULL((((OrderINItems.Qty * OrderQTItems.Amount))), 0) AS ExtAmount, 
			  ISNULL((((OrderINItems.Qty * OrderQTItems.Amount * OrderQT.ExchangeRate))), 0) AS ExtHKDAmount
FROM         (((((Customer RIGHT JOIN
			  (OrderIN LEFT JOIN
			  OrderINItems ON OrderIN.OrderINId = OrderINItems.OrderINId) ON Customer.CustomerId = OrderIN.CustomerId) LEFT JOIN
			  OrderSCItems ON (OrderINItems.LineNumber = OrderSCItems.LineNumber) AND (OrderINItems.OrderSCItemsId = OrderSCItems.OrderSCItemsId)) LEFT JOIN
			  OrderQTItems ON (OrderSCItems.LineNumber = OrderQTItems.LineNumber) AND (OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId)) LEFT JOIN
			  OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId) LEFT JOIN
			  T_Region ON Customer.RegionId = T_Region.RegionId) LEFT JOIN
			  T_PaymentTerms ON OrderIN.PricingTerms = T_PaymentTerms.TermsId INNER JOIN
			  T_Currency ON Customer.CurrencyId = T_Currency.CurrencyId;


-- dbo.vwCategoryList source

CREATE OR ALTER VIEW [vwCategoryList]
AS
SELECT     dbo.T_Dept.DeptId, dbo.T_Dept.DeptCode, dbo.T_Dept.DeptName, dbo.T_Dept.DeptName_Chs, dbo.T_Dept.DeptName_Cht, dbo.T_Class.ClassId, 
                      dbo.T_Class.ClassCode, dbo.T_Class.ClassName, dbo.T_Class.ClassName_Chs, dbo.T_Class.ClassName_Cht, dbo.T_Category.CategoryId, 
                      dbo.T_Category.CategoryCode, dbo.T_Category.CategoryName, dbo.T_Category.CategoryName_Chs, dbo.T_Category.CategoryName_Cht, 
                      dbo.T_Category.CostingMethod, dbo.T_Category.InventoryMethod, dbo.T_Category.TaxMethod
FROM         dbo.T_Class RIGHT OUTER JOIN
                      dbo.T_Category ON dbo.T_Class.ClassId = dbo.T_Category.ClassId LEFT OUTER JOIN
                      dbo.T_Dept ON dbo.T_Category.DeptId = dbo.T_Dept.DeptId
WHERE     (dbo.T_Dept.DeptId IS NULL) AND (dbo.T_Class.ClassId IS NULL);


-- dbo.vwCityList source

CREATE OR ALTER VIEW [dbo].[vwCityList]
AS
SELECT     TOP (100) PERCENT co.CountryId, co.CountryCode, co.CountryName, co.CountryName_Chs, co.CountryName_Cht, co.CountryPhoneCode, 
                      pr.ProvinceId, pr.ProvinceCode, pr.ProvinceName, pr.ProvinceName_Chs, pr.ProvinceName_Cht, ci.CityId, ci.CityCode, ci.CityPhoneCode, ci.CityName, 
                      ci.CityName_Chs, ci.CityName_Cht
FROM         dbo.T_City AS ci INNER JOIN
                      dbo.T_Country AS co ON ci.CountryId = co.CountryId LEFT OUTER JOIN
                      dbo.T_Province AS pr ON ci.ProvinceId = pr.ProvinceId
ORDER BY co.CountryName, pr.ProvinceName, ci.CityName;


-- dbo.vwCustomerAddressList source

CREATE OR ALTER VIEW [dbo].[vwCustomerAddressList]
AS
SELECT     ca.CustomerAddressId, ca.CustomerId, a.AddressName, ca.DefaultRec, ca.AddrText, ca.AddrIsMailing, p1.PhoneName AS PhoneLabel1, 
                      ca.Phone1_Text, p2.PhoneName AS PhoneLabel2, ca.Phone2_Text, p3.PhoneName AS PhoneLabel3, ca.Phone3_Text, p4.PhoneName AS PhoneLabel4,
                       ca.Phone4_Text, p5.PhoneName AS PhoneLabel5, ca.Phone5_Text, ca.Notes, ca.CreatedOn, s1.Alias AS CreatedBy, ca.ModifiedOn, 
                      s2.Alias AS ModifiedBy, ca.Retired
FROM         dbo.CustomerAddress AS ca INNER JOIN
                      dbo.Z_Address AS a ON ca.AddressId = a.AddressId INNER JOIN
                      dbo.Z_Phone AS p1 ON ca.Phone1_Label = p1.PhoneId INNER JOIN
                      dbo.Z_Phone AS p2 ON ca.Phone2_Label = p2.PhoneId INNER JOIN
                      dbo.Z_Phone AS p3 ON ca.Phone3_Label = p3.PhoneId INNER JOIN
                      dbo.Z_Phone AS p4 ON ca.Phone4_Label = p4.PhoneId INNER JOIN
                      dbo.Z_Phone AS p5 ON ca.Phone5_Label = p5.PhoneId INNER JOIN
                      dbo.Staff AS s1 ON ca.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON ca.ModifiedBy = s2.StaffId;


-- dbo.vwCustomerContactList source

CREATE OR ALTER VIEW [dbo].[vwCustomerContactList]
AS
SELECT     cc.CustomerContactId, cc.CustomerId, cc.DefaultRec, zs.SalutationName, cc.FullName, cc.FirstName, cc.LastName, zj.JobTitleName, 
                      p1.PhoneName AS PhoneLabel1, cc.Phone1_Text, p2.PhoneName AS PhoneLabel2, cc.Phone2_Text, p3.PhoneName AS PhoneLabel3, cc.Phone3_Text, 
                      p4.PhoneName AS PhoneLabel4, cc.Phone4_Text, p5.PhoneName AS PhoneLabel5, cc.Phone5_Text, cc.Notes, cc.CreatedOn, s1.Alias AS CreatedBy, 
                      cc.ModifiedOn, s2.Alias AS ModifiedBy, cc.Retired
FROM         dbo.CustomerContact AS cc INNER JOIN
                      dbo.Z_Salutation AS zs ON cc.SalutationId = zs.SalutationId INNER JOIN
                      dbo.Z_JobTitle AS zj ON cc.JobTitleId = zj.JobTitleId INNER JOIN
                      dbo.Z_Phone AS p1 ON cc.Phone1_Label = p1.PhoneId INNER JOIN
                      dbo.Z_Phone AS p2 ON cc.Phone2_Label = p2.PhoneId INNER JOIN
                      dbo.Z_Phone AS p3 ON cc.Phone3_Label = p3.PhoneId INNER JOIN
                      dbo.Z_Phone AS p4 ON cc.Phone4_Label = p4.PhoneId INNER JOIN
                      dbo.Z_Phone AS p5 ON cc.Phone5_Label = p5.PhoneId INNER JOIN
                      dbo.Staff AS s1 ON cc.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON cc.ModifiedBy = s2.StaffId;


-- dbo.vwCustomerList source

CREATE OR ALTER VIEW [dbo].[vwCustomerList]
AS
SELECT     TOP (100) PERCENT c.CustomerId, c.CustomerCode, c.ACNumber, c.CustomerName, c.CustomerName_Chs, c.CustomerName_Cht, c.RegionId, 
                      r.RegionName, c.TermsId, t.TermsName, c.CurrencyId, cny.CurrencyName, c.CreditLimit, c.ProfitMargin, c.BlackListed, c.Remarks, c.Status, 
                      c.CreatedOn, s1.Alias AS CreatedBy, c.ModifiedOn, s2.Alias AS ModifiedBy, c.Retired
FROM         dbo.Customer AS c INNER JOIN
                      dbo.T_Region AS r ON c.RegionId = r.RegionId INNER JOIN
                      dbo.T_PaymentTerms AS t ON c.TermsId = t.TermsId INNER JOIN
                      dbo.T_Currency AS cny ON c.CurrencyId = cny.CurrencyId INNER JOIN
                      dbo.Staff AS s1 ON c.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON c.ModifiedBy = s2.StaffId
ORDER BY c.CustomerName;


-- dbo.vwInvoiceItemList source

CREATE OR ALTER VIEW dbo.vwInvoiceItemList
AS
SELECT        TOP (100) PERCENT m.OrderINId, m.INNumber, m.INDate, i.OrderINItemsId, i.LineNumber, a.ArticleId, a.SKU, a.ArticleCode, a.ArticleName, a.ArticleName_Chs, a.ArticleName_Cht, p.PackageId, p.PackageCode, p.PackageName, 
                         p.PackageName_Chs, p.PackageName_Cht, s.SupplierId, s.SupplierCode, s.SupplierName, s.SupplierName_Chs, s.SupplierName_Cht, qi.Particular, qi.CustRef, qi.PriceType, qi.FactoryCost, qi.Margin, qi.FCL, qi.LCL, 
                         qi.SampleQty, qi.Qty, qi.Unit, qi.Amount, qi.Carton, qi.GLAccount, qi.RefDocNo, qi.ShippingMark, qi.QtyIN, qi.QtyOUT, ps.SuppRef, c.CurrencyCode, ps.FCLCost, ps.LCLCost, i.Qty AS Inv_Qty,
                             (SELECT        SCNumber
                               FROM            dbo.OrderSC
                               WHERE        (OrderSCId = sci.OrderSCId)) AS SCNUmber, ap.InnerBox, ap.OuterBox, ap.CUFT
FROM            dbo.OrderIN AS m INNER JOIN
                         dbo.Supplier AS s INNER JOIN
                         dbo.ArticleSupplier AS ps INNER JOIN
                         dbo.OrderINItems AS i LEFT OUTER JOIN
                         dbo.OrderSCItems AS sci ON i.OrderSCItemsId = sci.OrderSCItemsId LEFT OUTER JOIN
                         dbo.OrderQTItems AS qi ON sci.OrderQTItemId = qi.OrderQTItemId INNER JOIN
                         dbo.Article AS a ON qi.ArticleId = a.ArticleId ON ps.ArticleId = qi.ArticleId AND ps.SupplierId = qi.SupplierId ON s.SupplierId = ps.SupplierId INNER JOIN
                         dbo.T_Currency AS c ON ps.CurrencyId = c.CurrencyId ON m.OrderINId = i.OrderINId INNER JOIN
                         dbo.T_Package AS p ON qi.PackageId = p.PackageId INNER JOIN
                         dbo.OrderQTPackage AS ap ON qi.OrderQTItemId = ap.OrderQTItemId
ORDER BY m.INNumber, i.LineNumber DESC;


-- dbo.vwInvoiceList source

CREATE OR ALTER VIEW [dbo].[vwInvoiceList]
AS
SELECT     [in].OrderINId, [in].INDate, [in].InUse, [in].Status, [in].INNumber, c.CustomerName, [in].Remarks, [in].CreatedOn, s1.Alias AS CreatedBy, [in].ModifiedOn, 
                      s2.Alias AS ModifiedBy, [in].Revision, [in].SendFrom, [in].SendTo, c.CustomerId
FROM         dbo.OrderIN AS [in] LEFT OUTER JOIN
                      dbo.Staff AS s ON [in].StaffId = s.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s2 ON [in].ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON [in].CreatedBy = s1.StaffId LEFT OUTER JOIN
                      dbo.Customer AS c ON [in].CustomerId = c.CustomerId;


-- dbo.vwOSSample source

CREATE OR ALTER VIEW [dbo].[vwOSSample]
AS
SELECT     dbo.Customer.CustomerId, dbo.Customer.CustomerName AS CustName, dbo.OrderQTItems.CustRef, dbo.Article.ArticleCode, dbo.Article.ArticleName AS ArtName, 
                      dbo.OrderQTItems.Amount, dbo.OrderQTItems.SampleQty, dbo.OrderQT.Unit, dbo.Supplier.SupplierName, dbo.OrderQT.QTNumber, dbo.OrderQTItems.QtyOUT, 
                      dbo.T_Package.PackageCode, dbo.T_Package.PackageName, dbo.Supplier.SupplierCode, dbo.OrderQT.QTDate, dbo.OrderQTItems.OrderQTItemId
FROM         dbo.T_Package RIGHT OUTER JOIN
                      dbo.OrderQTItems ON dbo.T_Package.PackageId = dbo.OrderQTItems.PackageId RIGHT OUTER JOIN
                      dbo.Article ON dbo.OrderQTItems.ArticleId = dbo.Article.ArticleId LEFT OUTER JOIN
                      dbo.OrderQT ON dbo.OrderQTItems.OrderQTId = dbo.OrderQT.OrderQTId LEFT OUTER JOIN
                      dbo.Supplier ON dbo.OrderQTItems.SupplierId = dbo.Supplier.SupplierId LEFT OUTER JOIN
                      dbo.Customer ON dbo.OrderQT.CustomerId = dbo.Customer.CustomerId
WHERE     (dbo.OrderQTItems.OrderQTItemId IS NOT NULL);


-- dbo.vwOSShipment source

CREATE OR ALTER VIEW [dbo].[vwOSShipment]
AS
SELECT	Customer.CustomerId,Supplier.SupplierId,Customer.CustomerName AS CustName, OrderQTItems.CustRef, Article.ArticleCode, Article.ArticleName AS ArtName, 
		(SELECT CurrencyCode FROM T_Currency WHERE OrderQT.CurrencyId = T_Currency.CurrencyId)AS CurrencyCode, 
		OrderQTItems.Amount, OrderQTItems.Qty, OrderQTItems.Unit,
		OrderQTCustShipping.ShippedOn AS ShipmentDate, OrderQTCustShipping.QtyOrdered, OrderQTCustShipping.QtyShipped, 
		(OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped) AS OSQty, 
		((OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped) * OrderQTItems.Amount) AS OSAmount, 
		Supplier.SupplierName AS SuppName, OrderSC.SCNumber
FROM	OrderSCItems LEFT JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId 
		LEFT JOIN Customer ON OrderSC.CustomerId = Customer.CustomerId
		RIGHT JOIN (Article RIGHT JOIN OrderQTItems ON Article.ArticleId = OrderQTItems.ArticleId) 
			ON OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId 
		RIGHT JOIN OrderQTCustShipping ON OrderQTItems.OrderQTItemId = OrderQTCustShipping.OrderQTItemId 
		LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId 
		LEFT JOIN OrderQT ON OrderQT.OrderQTId = OrderQTItems.OrderQTId
WHERE	Customer.CustomerName IS NOT NULL;


-- dbo.vwOrderINQTList source

CREATE OR ALTER VIEW dbo.vwOrderINQTList
AS
SELECT sci.LineNumber AS SCLineNo, art.ArticleId, art.ArticleCode, 
      ISNULL(dbo.T_Color.ColorName, art.ColorPattern) AS Color, qti.CustRef, 
      qti.Qty - sci.QtyOUT AS OSQty, 0 AS InvoicedQty, qt.QTNumber, 
      qti.LineNumber AS QTLineNo, qti.OrderQTItemId, sci.OrderSCItemsId, 
      sc.SCNumber
FROM dbo.OrderQT AS qt RIGHT OUTER JOIN
      dbo.Article AS art LEFT OUTER JOIN
      dbo.T_Color ON art.ColorId = dbo.T_Color.ColorId RIGHT OUTER JOIN
      dbo.OrderQTItems AS qti ON art.ArticleId = qti.ArticleId ON 
      qt.OrderQTId = qti.OrderQTId RIGHT OUTER JOIN
      dbo.OrderSC AS sc RIGHT OUTER JOIN
      dbo.OrderSCItems AS sci ON sc.OrderSCId = sci.OrderSCId ON 
      qti.OrderQTItemId = sci.OrderQTItemId;


-- dbo.vwOrderINShipmentList source

CREATE OR ALTER VIEW [dbo].[vwOrderINShipmentList]
AS
SELECT     dbo.OrderQTCustShipping.ShippedOn, dbo.OrderQTCustShipping.QtyOrdered - dbo.OrderQTCustShipping.QtyShipped AS OSQty, 0 AS ThisShipment, 
                      dbo.OrderQTCustShipping.OrderQTItemId, dbo.OrderSCItems.OrderSCItemsId
FROM         dbo.OrderQTCustShipping LEFT OUTER JOIN
                      dbo.OrderSCItems ON dbo.OrderQTCustShipping.OrderQTItemId = dbo.OrderSCItems.OrderQTItemId;


-- dbo.vwOrderQTItemList source

CREATE OR ALTER VIEW dbo.vwOrderQTItemList
AS
SELECT        TOP (100) PERCENT m.OrderQTId, m.QTNumber, m.QTDate, i.OrderQTItemId, i.LineNumber, a.ArticleId, a.SKU, a.ArticleCode, a.ArticleName, a.ArticleName_Chs, a.ArticleName_Cht, p.PackageId, p.PackageCode, p.PackageName, 
                         p.PackageName_Chs, p.PackageName_Cht, s.SupplierId, s.SupplierCode, s.SupplierName, s.SupplierName_Chs, s.SupplierName_Cht, i.Particular, i.CustRef, i.PriceType, i.FactoryCost, i.Margin, i.FCL, i.LCL, i.SampleQty, i.Qty, 
                         i.Unit, i.Amount, i.Carton, i.GLAccount, i.RefDocNo, i.ShippingMark, i.QtyIN, i.QtyOUT, ps.SuppRef, c.CurrencyCode, ps.FCLCost, ps.LCLCost, ap.InnerBox, ap.OuterBox, ap.CUFT
FROM            dbo.T_Currency AS c INNER JOIN
                         dbo.Supplier AS s INNER JOIN
                         dbo.ArticleSupplier AS ps INNER JOIN
                         dbo.OrderQTItems AS i INNER JOIN
                         dbo.Article AS a ON i.ArticleId = a.ArticleId INNER JOIN
                         dbo.OrderQT AS m ON i.OrderQTId = m.OrderQTId ON ps.ArticleId = i.ArticleId AND ps.SupplierId = i.SupplierId ON s.SupplierId = ps.SupplierId ON c.CurrencyId = ps.CurrencyId INNER JOIN
                         dbo.T_Package AS p ON i.PackageId = p.PackageId INNER JOIN
                         dbo.OrderQTPackage AS ap ON i.OrderQTItemId = ap.OrderQTItemId
ORDER BY m.QTNumber, i.LineNumber DESC;


-- dbo.vwOrderQTList source

CREATE OR ALTER VIEW [dbo].[vwOrderQTList]
AS
SELECT     TOP (100) PERCENT q.OrderQTId, q.QTNumber, q.QTDate, q.CustomerId, c.CustomerCode, c.CustomerName, c.CustomerName_Chs, c.CustomerName_Cht, q.StaffId, 
                      s.Alias AS SalePerson, q.PriceMethod, q.FCLFactor, q.LCLFactor, q.Unit, q.Measurement, q.SampleQty, q.InputMask, tc.CurrencyCode, q.ExchangeRate, 
                      tp.TermsName, q.Repayment, q.SendFrom, q.SendTo, q.TotalQty, q.TotalQtyIN, q.TotalQtyOUT, q.TotalAmount, q.Remarks, q.Remarks2, q.Remarks3, q.Revision, 
                      q.InUse, q.Status, q.CreatedOn, s1.Alias AS CreatedBy, q.ModifiedOn, s2.Alias AS ModifiedBy, q.AccessedOn, s3.Alias AS AccessedBy, q.Retired
FROM         dbo.OrderQT AS q LEFT OUTER JOIN
                      dbo.Staff AS s3 ON q.AccessedBy = s3.StaffId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS tp ON q.TermsId = tp.TermsId LEFT OUTER JOIN
                      dbo.Staff AS s2 ON q.ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.T_Currency AS tc ON q.CurrencyId = tc.CurrencyId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON q.CreatedBy = s1.StaffId LEFT OUTER JOIN
                      dbo.Customer AS c ON q.CustomerId = c.CustomerId LEFT OUTER JOIN
                      dbo.Staff AS s ON q.StaffId = s.StaffId
ORDER BY q.QTNumber;


-- dbo.vwOrderSPItemList source

CREATE OR ALTER VIEW [dbo].[vwOrderSPItemList]
AS
SELECT     TOP (100) PERCENT m.OrderSPId, m.SPNumber, m.SPDate, i.OrderSPItemsId, i.LineNumber, a.ArticleId, a.SKU, a.ArticleCode, a.ArticleName, a.ArticleName_Chs, 
                      a.ArticleName_Cht, p.PackageId, p.PackageCode, p.PackageName, p.PackageName_Chs, p.PackageName_Cht, s.SupplierId, s.SupplierCode, s.SupplierName, 
                      s.SupplierName_Chs, s.SupplierName_Cht, qi.Particular, qi.CustRef, qi.PriceType, qi.FactoryCost, qi.Margin, qi.FCL, qi.LCL, qi.SampleQty, i.Qty, i.Unit, qi.Amount, 
                      qi.Carton, qi.GLAccount, qi.RefDocNo, qi.ShippingMark, qi.QtyIN, qi.QtyOUT, ps.SuppRef, c.CurrencyCode, ps.FCLCost, ps.LCLCost, ap.InnerBox, ap.OuterBox, 
                      ap.CUFT, qi.LineNumber AS QTLineNumber,
                          (SELECT     QTNumber
                            FROM          dbo.OrderQT
                            WHERE      (OrderQTId = qi.OrderQTId)) AS QTNumber
FROM         dbo.Supplier AS s INNER JOIN
                      dbo.OrderSPItems AS i LEFT OUTER JOIN
                      dbo.OrderQTItems AS qi ON i.OrderQTItemId = qi.OrderQTItemId INNER JOIN
                      dbo.Article AS a ON qi.ArticleId = a.ArticleId INNER JOIN
                      dbo.ArticlePackage AS ap INNER JOIN
                      dbo.T_Package AS p ON ap.PackageId = p.PackageId ON qi.ArticleId = ap.ArticleId AND qi.PackageId = ap.PackageId INNER JOIN
                      dbo.ArticleSupplier AS ps ON qi.ArticleId = ps.ArticleId AND qi.SupplierId = ps.SupplierId ON s.SupplierId = ps.SupplierId INNER JOIN
                      dbo.T_Currency AS c ON ps.CurrencyId = c.CurrencyId INNER JOIN
                      dbo.OrderSP AS m ON i.OrderSPId = m.OrderSPId
ORDER BY m.SPNumber, i.LineNumber DESC;


-- dbo.vwOrderSPList source

CREATE OR ALTER VIEW [dbo].[vwOrderSPList]
AS
SELECT     sp.OrderSPId, sp.SPDate, sp.InUse, sp.Status, sp.SPNumber, c.CustomerName, s.Alias AS Salesperson, sp.Remarks, sp.CreatedOn, s1.Alias AS CreatedBy, 
                      sp.ModifiedOn, s2.Alias AS ModifiedBy, sp.Revision, sp.SendFrom, sp.SendTo, c.CustomerId, c.CustomerCode, c.CustomerName_Chs, c.CustomerName_Cht
FROM         dbo.OrderSP AS sp LEFT OUTER JOIN
                      dbo.Staff AS s ON sp.StaffId = s.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s2 ON sp.ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON sp.CreatedBy = s1.StaffId LEFT OUTER JOIN
                      dbo.Customer AS c ON sp.CustomerId = c.CustomerId;


-- dbo.vwPreOrderItemList source

CREATE OR ALTER VIEW dbo.vwPreOrderItemList
AS
SELECT        TOP (100) PERCENT m.OrderPLId, m.PLNumber, m.PLDate, i.OrderPLItemsId, i.LineNumber, a.ArticleId, a.SKU, a.ArticleCode, a.ArticleName, a.ArticleName_Chs, a.ArticleName_Cht, p.PackageId, p.PackageCode, 
                         p.PackageName, p.PackageName_Chs, p.PackageName_Cht, s.SupplierId, s.SupplierCode, s.SupplierName, s.SupplierName_Chs, s.SupplierName_Cht, qi.Particular, qi.CustRef, qi.PriceType, qi.FactoryCost, qi.Margin, qi.FCL, 
                         qi.LCL, qi.SampleQty, qi.Qty, qi.Unit, qi.Amount, qi.Carton, qi.GLAccount, qi.RefDocNo, qi.ShippingMark, qi.QtyIN, qi.QtyOUT, ps.SuppRef, c.CurrencyCode, ps.FCLCost, ps.LCLCost, ap.InnerBox, ap.OuterBox, ap.CUFT
FROM            dbo.OrderPL AS m INNER JOIN
                         dbo.Supplier AS s INNER JOIN
                         dbo.ArticleSupplier AS ps INNER JOIN
                         dbo.OrderPLItems AS i LEFT OUTER JOIN
                         dbo.OrderQTItems AS qi ON i.OrderQTItemId = qi.OrderQTItemId INNER JOIN
                         dbo.Article AS a ON qi.ArticleId = a.ArticleId ON ps.ArticleId = qi.ArticleId AND ps.SupplierId = qi.SupplierId ON s.SupplierId = ps.SupplierId INNER JOIN
                         dbo.T_Currency AS c ON ps.CurrencyId = c.CurrencyId ON m.OrderPLId = i.OrderPLId INNER JOIN
                         dbo.T_Package AS p ON qi.PackageId = p.PackageId INNER JOIN
                         dbo.OrderQTPackage AS ap ON qi.OrderQTItemId = ap.OrderQTItemId
ORDER BY m.PLNumber, i.LineNumber DESC;


-- dbo.vwPreOrderList source

CREATE OR ALTER VIEW [dbo].[vwPreOrderList]
AS
SELECT     pl.OrderPLId, pl.PLDate, pl.InUse, pl.Status, pl.PLNumber, c.CustomerName, pl.Remarks, pl.CreatedOn, s1.Alias AS CreatedBy, pl.ModifiedOn, s2.Alias AS ModifiedBy, 
                      pl.Revision, pl.TotalQty, pl.TotalQtyIN, pl.TotalQtyOUT, pl.TotalAmount, pl.SendFrom, pl.SendTo, c.CustomerId
FROM         dbo.OrderPL AS pl LEFT OUTER JOIN
                      dbo.Staff AS s ON pl.StaffId = s.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s2 ON pl.ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON pl.CreatedBy = s1.StaffId LEFT OUTER JOIN
                      dbo.Customer AS c ON pl.CustomerId = c.CustomerId;


-- dbo.vwPriceDetailList source

CREATE OR ALTER VIEW [dbo].[vwPriceDetailList]
AS
SELECT     TOP (100) PERCENT  m.QTNumber, m.QTDate, i.LineNumber, a.SKU, a.ArticleCode, a.ArticleName, p.PackageCode, p.PackageName,
					  (SELECT AgeGradingName FROM T_AgeGrading WHERE T_AgeGrading.AgeGradingId = a.AgeGradingId ) AS AgeGrading,
					  (SELECT OriginName FROM T_Origin WHERE T_Origin.OriginId = a.OriginId) AS Origin,
                      s.SupplierCode, s.SupplierName, i.Particular, i.CustRef, i.FactoryCost, i.Margin, i.FCL, i.LCL, i.Qty, i.Unit, 
                      i.Amount,  ps.SuppRef, c.CurrencyCode, ps.FCLCost, ps.LCLCost, ap.InnerBox, ap.OuterBox, ap.CUFT                     
FROM         dbo.ArticleSupplier AS ps INNER JOIN
                      dbo.OrderQTItems AS i INNER JOIN
                      dbo.Article AS a ON i.ArticleId = a.ArticleId INNER JOIN
                      dbo.OrderQT AS m ON i.OrderQTId = m.OrderQTId INNER JOIN
                      dbo.ArticlePackage AS ap INNER JOIN
                      dbo.T_Package AS p ON ap.PackageId = p.PackageId ON i.ArticleId = ap.ArticleId AND i.PackageId = ap.PackageId ON ps.ArticleId = i.ArticleId AND 
                      ps.SupplierId = i.SupplierId INNER JOIN
                      dbo.Supplier AS s ON ps.SupplierId = s.SupplierId INNER JOIN
                      dbo.T_Currency AS c ON ps.CurrencyId = c.CurrencyId
ORDER BY m.QTNumber, i.LineNumber DESC;


-- dbo.vwPrint_CompletedPriceList source

CREATE OR ALTER VIEW [dbo].[vwPrint_CompletedPriceList]
AS
SELECT		OrderQT.QTNumber, OrderQTItems.LineNumber, Article.SKU, Article.ArticleCode,  
			Supplier.SupplierCode, T_Package.PackageName, Article.ArticleName AS ArtName, OrderQTItems.Particular,  
			OrderQTItems.CustRef, OrderQTItems.Qty, OrderQTItems.Unit AS QuotedUnit, OrderQTItems.FactoryCost, 
			OrderQTItems.Margin, OrderQTItems.FCL, OrderQTItems.LCL, OrderQTItems.Amount, OrderQTSupplier.SuppRef,
			(SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId) AS SuppCurrency,
			OrderQTSupplier.FCLCost, OrderQTSupplier.LCLCost, OrderQTPackage.Unit, OrderQTPackage.InnerBox, 
			OrderQTPackage.OuterBox, OrderQTPackage.CUFT, Supplier.SupplierName AS SuppName, T_AgeGrading.AgeGradingName AS AgeGrading,
			T_Origin.OriginName AS Origin, OrderQT.QTDate, Customer.CustomerName AS CustName,
			CustomerAddress.DefaultRec, CustomerAddress.AddrText, CustomerAddress.Phone1_Label, CustomerAddress.Phone1_Text,
			CustomerAddress.Phone2_Label, CustomerAddress.Phone2_Text, CustomerAddress.Phone3_Label, CustomerAddress.Phone3_Text,
			CustomerAddress.Phone4_Label, CustomerAddress.Phone4_Text, CustomerAddress.Phone5_Label, CustomerAddress.Phone5_Text, 
			(SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId) AS CurrencyUsed,
			OrderQT.PriceMethod, T_PaymentTerms.TermsName AS Terms, OrderQT.Remarks
FROM	((((((((((OrderQTItems LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId) 
			LEFT JOIN OrderQTSupplier ON OrderQTItems.OrderQTItemId = OrderQTSupplier.OrderQTItemId) 
			LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId) 
			LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId) 
			LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId) 
			LEFT JOIN T_AgeGrading ON Article.AgeGradingId = T_AgeGrading.AgeGradingId) 
			LEFT JOIN T_Origin ON Article.OriginId = T_Origin.OriginId) 
			LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId) 
			LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId) 
			LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId) 
			LEFT JOIN T_PaymentTerms ON OrderQT.TermsId = T_PaymentTerms.TermsId
WHERE		(CustomerAddress.DefaultRec =1 OR CustomerAddress.DefaultRec=-1);


-- dbo.vwPrint_PriceList source

CREATE OR ALTER VIEW [dbo].[vwPrint_PriceList]
AS
SELECT OrderQT.QTNumber, OrderQT.QTDate, Customer.CustomerName As CustName, CustomerAddress.AddrText, 
	CustomerAddress.Phone1_Label, CustomerAddress.Phone1_Text, CustomerAddress.Phone2_Label, 
	CustomerAddress.Phone2_Text, CustomerAddress.Phone3_Label, CustomerAddress.Phone3_Text, 
	CustomerAddress.Phone4_Label, CustomerAddress.Phone4_Text, CustomerAddress.Phone5_Label, 
	CustomerAddress.Phone5_Text, OrderQT.Unit, OrderQT.PriceMethod,
	(SELECT T_Currency.CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId) AS CurrencyUsed,
	T_PaymentTerms.TermsName As Terms, OrderQT.Remarks
FROM (OrderQT LEFT JOIN (Customer LEFT JOIN CustomerAddress ON Customer.CustomerId = CustomerAddress.CustomerId) 
				ON OrderQT.CustomerId = Customer.CustomerId)	
			  LEFT JOIN T_PaymentTerms ON OrderQT.TermsId = T_PaymentTerms.TermsId;


-- dbo.vwPrint_QuotationList source

CREATE OR ALTER VIEW dbo.vwPrint_QuotationList
AS
SELECT     dbo.OrderQT.QTNumber, dbo.OrderQT.QTDate, dbo.OrderQTItems.LineNumber, dbo.OrderQTItems.CustRef, dbo.OrderQTItems.Particular, dbo.OrderQTItems.Margin, 
                      dbo.OrderQTItems.FactoryCost, dbo.OrderQTItems.LCL, dbo.OrderQTItems.FCL, dbo.OrderQTItems.Amount, dbo.Customer.CustomerName AS CustName, 
                      dbo.Article.ArticleId, dbo.Article.ArticleCode, dbo.Article.ArticleName AS ArtName, dbo.Article.ColorPattern, dbo.Supplier.SupplierCode, 
                      dbo.Supplier.SupplierName AS SuppName, dbo.OrderQTSupplier.SuppRef, dbo.T_Origin.OriginName AS Origin, dbo.T_AgeGrading.AgeGradingName AS AgeGrading, 
                      dbo.OrderQTPackage.InnerBox, dbo.OrderQTPackage.OuterBox, dbo.OrderQT.Unit, dbo.OrderQTPackage.CUFT, dbo.T_Package.PackageName, 
                      dbo.T_PaymentTerms.TermsName AS Terms,
                          (SELECT     CurrencyCode
                            FROM          dbo.T_Currency
                            WHERE      (CurrencyId = dbo.OrderQTSupplier.CurrencyId)) AS SuppCurrency,
                          (SELECT     CurrencyCode
                            FROM          dbo.T_Currency AS T_Currency_1
                            WHERE      (CurrencyId = dbo.Customer.CurrencyId)) AS CurrencyUsed, dbo.CustomerAddress.DefaultRec, dbo.CustomerAddress.AddrText, 
                      dbo.CustomerAddress.Phone1_Text, dbo.CustomerAddress.Phone2_Text, dbo.CustomerAddress.Phone3_Text, dbo.CustomerAddress.Phone4_Text, 
                      dbo.CustomerAddress.Phone5_Text
FROM         dbo.OrderQTItems LEFT OUTER JOIN
                      dbo.OrderQTPackage ON dbo.OrderQTItems.OrderQTItemId = dbo.OrderQTPackage.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderQTSupplier ON dbo.OrderQTItems.OrderQTItemId = dbo.OrderQTSupplier.OrderQTItemId LEFT OUTER JOIN
                      dbo.Article ON dbo.OrderQTItems.ArticleId = dbo.Article.ArticleId LEFT OUTER JOIN
                      dbo.Supplier ON dbo.OrderQTItems.SupplierId = dbo.Supplier.SupplierId LEFT OUTER JOIN
                      dbo.T_Package ON dbo.OrderQTItems.PackageId = dbo.T_Package.PackageId LEFT OUTER JOIN
                      dbo.T_AgeGrading ON dbo.Article.AgeGradingId = dbo.T_AgeGrading.AgeGradingId LEFT OUTER JOIN
                      dbo.T_Origin ON dbo.Article.OriginId = dbo.T_Origin.OriginId LEFT OUTER JOIN
                      dbo.OrderQT ON dbo.OrderQTItems.OrderQTId = dbo.OrderQT.OrderQTId LEFT OUTER JOIN
                      dbo.Customer ON dbo.OrderQT.CustomerId = dbo.Customer.CustomerId LEFT OUTER JOIN
                      dbo.CustomerAddress ON dbo.Customer.CustomerId = dbo.CustomerAddress.CustomerId LEFT OUTER JOIN
                      dbo.T_PaymentTerms ON dbo.OrderQT.TermsId = dbo.T_PaymentTerms.TermsId;


-- dbo.vwProductList source

CREATE OR ALTER VIEW dbo.vwProductList
AS
SELECT     a.ArticleId, a.SKU, a.ArticleCode, a.ArticleName, a.ArticleName_Chs, a.ArticleName_Cht, c.CategoryId, c.CategoryCode, c.CategoryName, c.CategoryName_Chs, 
                      c.CategoryName_Cht, ag.AgeGradingId, ag.AgeGradingCode, ag.AgeGradingName, ag.AgeGradingName_Chs, ag.AgeGradingName_Cht, o.OriginId, o.OriginCode, 
                      o.OriginName, o.OriginName_Chs, o.OriginName_Cht, a.Remarks, a.ColorPattern, a.Barcode, a.Status, a.CreatedOn, s.Alias AS CreatedBy, a.ModifiedOn, 
                      s1.Alias AS ModifiedBy, cl.ColorId, cl.ColorCode, cl.ColorName, cl.ColorName_Chs, cl.ColorName_Cht
FROM         dbo.Staff AS s1 INNER JOIN
                      dbo.Article AS a INNER JOIN
                      dbo.T_Category AS c ON a.CategoryId = c.CategoryId INNER JOIN
                      dbo.T_Origin AS o ON a.OriginId = o.OriginId INNER JOIN
                      dbo.Staff AS s ON a.CreatedBy = s.StaffId ON s1.StaffId = a.ModifiedBy LEFT OUTER JOIN
                      dbo.T_AgeGrading AS ag ON a.AgeGradingId = ag.AgeGradingId LEFT OUTER JOIN
                      dbo.T_Color AS cl ON a.ColorId = cl.ColorId;


-- dbo.vwProductPackage source

CREATE OR ALTER VIEW [vwProductPackage]
AS
SELECT     ap.ArticleId, ap.ArticlePackageId, ap.PackageId, p.PackageCode, p.PackageName, ap.DefaultRec, ap.UomId, u.UomName, ap.InnerBox, ap.OuterBox, ap.CUFT, 
                      ap.SizeLength_in, ap.SizeWidth_in, ap.SizeHeight_in, ap.SizeLength_cm, ap.SizeWidth_cm, ap.SizeHeight_cm, ap.WeightGross_lb, ap.WeightNet_lb, 
                      ap.WeightGross_kg, ap.WeightNet_kg, ap.ContainerQty, ap.ContainerSize, ap.CreatedOn, s1.Alias AS CreatedBy, ap.ModifiedOn, s2.Alias AS ModifiedBy
FROM         dbo.ArticlePackage AS ap INNER JOIN
                      dbo.T_UnitOfMeasures AS u ON ap.UomId = u.UomId INNER JOIN
                      dbo.T_Package AS p ON ap.PackageId = p.PackageId INNER JOIN
                      dbo.Staff AS s1 ON ap.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON ap.ModifiedBy = s2.StaffId;


-- dbo.vwProductSupplier source

CREATE OR ALTER VIEW [vwProductSupplier]
AS
SELECT     a.ArticleId, a.ArticleSupplierId, a.SupplierId, s.SupplierCode, s.SupplierName, a.DefaultRec, a.SuppRef, c.CurrencyCode, a.FCLCost, a.LCLCost, a.UnitCost, a.Notes, 
                      a.CreatedOn, s1.Alias AS CreatedBy, a.ModifiedOn, s1.Alias AS ModifiedBy
FROM         dbo.ArticleSupplier AS a INNER JOIN
                      dbo.Supplier AS s ON a.SupplierId = s.SupplierId INNER JOIN
                      dbo.Staff AS s1 ON a.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON a.ModifiedBy = s2.StaffId INNER JOIN
                      dbo.T_Currency AS c ON a.CurrencyId = c.CurrencyId;


-- dbo.vwProductWithSupplierAndPackage source

CREATE OR ALTER VIEW dbo.vwProductWithSupplierAndPackage
AS
SELECT        p.ArticleId, p.SKU, p.ArticleCode, p.ArticleName, p.ArticleName_Chs, p.ArticleName_Cht, ISNULL(pp.PackageCode, '') AS PackageCode, ISNULL(pp.PackageName, '') AS PackageName, ISNULL(ps.SupplierName, '') 
                         AS SupplierName, ISNULL(ps.SuppRef, '') AS SuppRef, ISNULL(ps.SupplierCode, '') AS SupplierCode, p.ColorPattern, ISNULL(pp.UomName, N'') AS Unit, ISNULL(pp.InnerBox, 0) AS InnerBox, ISNULL(pp.OuterBox, 0) 
                         AS OuterBox, ISNULL(pp.CUFT, 0) AS CUFT
FROM            dbo.vwProductList AS p LEFT OUTER JOIN
                         dbo.vwProductPackage AS pp ON p.ArticleId = pp.ArticleId LEFT OUTER JOIN
                         dbo.vwProductSupplier AS ps ON p.ArticleId = ps.ArticleId;


-- dbo.vwPurchaseContractItemList source

CREATE OR ALTER VIEW dbo.vwPurchaseContractItemList
AS
SELECT        TOP (100) PERCENT dbo.OrderPC.OrderPCId, dbo.OrderPCItems.OrderPCItemsId, dbo.OrderQT.OrderQTId, dbo.OrderPC.PCDate, dbo.OrderPCItems.LineNumber, dbo.OrderPC.PCNumber, dbo.Article.ArticleCode, 
                         dbo.Article.ArticleId, dbo.Article.ArticleName, dbo.T_Package.PackageId, dbo.T_Package.PackageCode, dbo.T_Package.PackageName, dbo.T_Package.PackageName_Chs, dbo.T_Package.PackageName_Cht, 
                         dbo.OrderQTItems.CustRef, dbo.ArticleSupplier.SuppRef, dbo.OrderQTItems.Qty, dbo.OrderQTItems.Unit, dbo.OrderQTItems.PriceType, dbo.OrderQTItems.FactoryCost, dbo.T_Currency.CurrencyCode, dbo.ArticleSupplier.FCLCost, 
                         dbo.ArticleSupplier.LCLCost, dbo.ArticleSupplier.UnitCost, dbo.OrderQTItems.ShippingMark
FROM            dbo.T_Package INNER JOIN
                         dbo.T_Currency INNER JOIN
                         dbo.ArticleSupplier INNER JOIN
                         dbo.OrderPC INNER JOIN
                         dbo.OrderPCItems ON dbo.OrderPC.OrderPCId = dbo.OrderPCItems.OrderPCId INNER JOIN
                         dbo.OrderSCItems ON dbo.OrderSCItems.OrderSCItemsId = dbo.OrderPCItems.OrderSCItemsId INNER JOIN
                         dbo.OrderQTItems ON dbo.OrderQTItems.OrderQTItemId = dbo.OrderSCItems.OrderQTItemId INNER JOIN
                         dbo.OrderQT ON dbo.OrderQT.OrderQTId = dbo.OrderQTItems.OrderQTId INNER JOIN
                         dbo.Article ON dbo.Article.ArticleId = dbo.OrderQTItems.ArticleId ON dbo.ArticleSupplier.ArticleId = dbo.OrderQTItems.ArticleId ON dbo.T_Currency.CurrencyId = dbo.ArticleSupplier.CurrencyId ON 
                         dbo.T_Package.PackageId = dbo.OrderQTItems.PackageId
ORDER BY dbo.OrderPC.PCNumber, dbo.Article.ArticleCode;


-- dbo.vwPurchaseContractList source

CREATE OR ALTER VIEW [dbo].[vwPurchaseContractList]
AS
SELECT DISTINCT 
                      TOP (100) PERCENT pc.OrderPCId, pc.PCNumber, pc.PCDate, supp.SupplierId, supp.SupplierCode, supp.SupplierName, supp.SupplierName_Chs, 
                      supp.SupplierName_Cht, pc.StaffId, S.Alias AS SalePerson, pc.Remarks, pc.Remarks2, pc.Remarks3, pc.Revision, pc.InUse, pc.Status, pc.CreatedOn, 
                      S1.Alias AS CreatedBy, pc.ModifiedOn, S2.Alias AS ModifiedBy, pc.AccessedOn, S3.Alias AS AccessedBy, pc.Retired, ISNULL(qt.SampleQty, 0) AS SampleQty
FROM         dbo.OrderQT AS qt INNER JOIN
                      dbo.OrderQTItems AS qti ON qt.OrderQTId = qti.OrderQTId RIGHT OUTER JOIN
                      dbo.OrderSCItems AS sci ON qti.OrderQTItemId = sci.OrderQTItemId RIGHT OUTER JOIN
                      dbo.OrderPCItems AS pci ON sci.OrderSCItemsId = pci.OrderSCItemsId RIGHT OUTER JOIN
                      dbo.OrderPC AS pc ON pci.OrderPCId = pc.OrderPCId LEFT OUTER JOIN
                      dbo.Staff AS S2 ON S2.StaffId = pc.ModifiedBy LEFT OUTER JOIN
                      dbo.Staff AS S3 ON S3.StaffId = pc.AccessedBy LEFT OUTER JOIN
                      dbo.Staff AS S1 ON S1.StaffId = pc.CreatedBy LEFT OUTER JOIN
                      dbo.Supplier AS supp ON pc.SupplierId = supp.SupplierId LEFT OUTER JOIN
                      dbo.Staff AS S ON S.StaffId = pc.StaffId
ORDER BY pc.PCNumber;


-- dbo.vwPurchaseHistory source

CREATE OR ALTER VIEW [dbo].[vwPurchaseHistory]
AS	
SELECT Customer.CustomerId, Supplier.SupplierId, Supplier.SupplierName AS SuppName, OrderPC.PCNumber, Customer.CustomerName AS CustName, OrderQTItems.CustRef, 
		Article.ArticleCode, Article.ArticleName AS ArtName, 
		(T_Package.PackageName + ' ' + CAST(OrderQTPackage.InnerBox AS NVARCHAR(10)) + ' ' + OrderQTPackage.Unit + '/ ' + cast(OrderQTPackage.OuterBox AS NVARCHAR(10)) + ' ' + OrderQTPackage.Unit + '/ ' + CAST(OrderQTPackage.CUFT AS NVARCHAR(10)) + ' CUFT.') AS Packing, 
		OrderQTSuppShipping.QtyOrdered AS ScheduledQty,
		ISNULL((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId),'') AS OrderedCny,
		OrderQTItems.Amount AS OrderedPrice, OrderQTItems.Unit AS OrderedUnit,
		ISNULL((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId),'') AS FactoryCny,
		OrderQTItems.FactoryCost, OrderQTPackage.Unit AS FactoryUnit, OrderQTSuppShipping.DateShipped AS ScheduledShipmentDate
FROM (((((((OrderQTSuppShipping LEFT JOIN OrderQTItems ON OrderQTSuppShipping.OrderQTItemId = OrderQTItems.OrderQTItemId) 
			LEFT JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId) 
			LEFT JOIN OrderQTSupplier ON (OrderQTPackage.OrderQTItemId = OrderQTSupplier.OrderQTItemId)) 
			LEFT JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId) 
			LEFT JOIN (OrderSCItems LEFT JOIN OrderPCItems ON (OrderSCItems.OrderSCItemsId = OrderPCItems.OrderSCItemsId))
			LEFT JOIN OrderPC ON OrderPC.OrderPCId =OrderPCItems.OrderPCId  
				ON (OrderQTSuppShipping.OrderQTItemId = OrderSCItems.OrderQTItemId)) 
			LEFT JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId) 
			LEFT JOIN (OrderQT LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId)
			    ON OrderQT.OrderQTId = OrderQTItems.OrderQTId) 
			LEFT JOIN T_Package ON OrderQTItems.PackageId = T_Package.PackageId;


-- dbo.vwQuoteHistory source

CREATE OR ALTER VIEW [dbo].[vwQuoteHistory]
AS
SELECT  OrderQTItems.OrderQTItemId, ISNULL(Article.ArticleCode,'') AS ArticleCode , ISNULL(Supplier.SupplierCode,'') AS SupplierCode, ISNULL(T_Package.PackageCode,'') AS PackageCode, 
		ISNULL(Customer.CustomerName,'') AS CustomerName, ISNULL(OrderQTItems.CustRef,'') AS CustRef, 
		OrderQT.QTDate, ISNULL(OrderQT.QTNumber,0) AS QTNumber, ISNULL(OrderQTItems.Margin,0) AS Margin,
		ISNULL((CASE WHEN OrderQTItems.PriceType = 0 THEN 'C'
			  WHEN OrderQTItems.PriceType = 1 THEN 'F'
			  WHEN OrderQTItems.PriceType = 2 THEN 'L' ELSE '' END),'') AS PriceType, OrderQTItems.Amount, 
		ISNULL((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQT.CurrencyId ),'') AS CurrencyCode,
		ISNULL(OrderQTItems.FactoryCost,0) AS FactoryCost, 
		ISNULL((SELECT CurrencyCode FROM T_Currency WHERE T_Currency.CurrencyId = OrderQTSupplier.CurrencyId ),'') AS CurrencyUsed,
		ISNULL(OrderQTPackage.InnerBox,0) AS InnerBox, ISNULL(OrderQTPackage.OuterBox,0) AS OuterBox, 
		ISNULL(OrderQTPackage.CUFT,0) AS CUFT, ISNULL(OrderQTItems.Unit,'') AS Unit, ISNULL(Article.SKU,'') AS SKU
FROM	OrderQTItems LEFT JOIN OrderQT ON OrderQTItems.OrderQTId = OrderQT.OrderQTId
	    LEFT JOIN Customer ON OrderQT.CustomerId = Customer.CustomerId 
	    LEFT JOIN OrderQTSupplier ON OrderQTItems.OrderQTItemId = OrderQTSupplier.OrderQTItemId 
	    LEFT JOIN OrderQTPackage ON OrderQTSupplier.OrderQTItemId = OrderQTPackage.OrderQTItemId
	    LEFT JOIN Article ON Article.ArticleId = OrderQTItems.ArticleId
	    LEFT JOIN T_Package ON T_Package.PackageId = OrderQTItems.PackageId
	    LEFT JOIN Supplier ON Supplier.SupplierId = OrderQTItems.SupplierId;


-- dbo.vwRptInvoiceList source

CREATE OR ALTER VIEW [dbo].[vwRptInvoiceList]
AS
SELECT     dbo.OrderIN.INNumber, dbo.OrderINItems.LineNumber AS INLineNo, dbo.OrderQT.QTNumber, dbo.OrderQTItems.LineNumber AS QTLineNo, 
                      dbo.OrderSC.YourOrderNo, dbo.OrderSC.SCNumber, dbo.OrderQTItems.CustRef, dbo.Article.ArticleId, dbo.Article.ArticleCode, dbo.Article.ArticleName AS ArtName, 
                      dbo.T_AgeGrading.AgeGradingName AS AgeGrading, dbo.Article.ColorPattern, dbo.OrderQTItems.Particular, dbo.T_Package.PackageName AS Package, 
                      dbo.OrderQTPackage.InnerBox, dbo.OrderQTPackage.OuterBox, dbo.OrderQTPackage.CUFT, dbo.OrderQTPackage.Unit AS UoM, dbo.OrderINItems.Qty AS InvoiceQty, 
                      dbo.OrderQTItems.Unit, dbo.OrderQTItems.Amount AS UnitAmount, dbo.T_Currency.CurrencyCode AS CurrencyUsed, dbo.OrderIN.INDate, 
                      dbo.Customer.CustomerName AS CustName, dbo.CustomerAddress.AddrText AS CustAddr, dbo.CustomerAddress.Phone1_Label, dbo.CustomerAddress.Phone1_Text, 
                      dbo.CustomerAddress.Phone2_Label, dbo.CustomerAddress.Phone2_Text, dbo.CustomerAddress.Phone3_Label, dbo.CustomerAddress.Phone3_Text, 
                      dbo.CustomerAddress.Phone4_Label, dbo.CustomerAddress.Phone4_Text, dbo.CustomerAddress.Phone5_Label, dbo.CustomerAddress.Phone5_Text, 
                      dbo.OrderIN.YourRef, dbo.OrderIN.Carrier, dbo.OrderIN.ShipmentDate AS DepartureDate, dbo.OrderIN.Remarks, dbo.OrderIN.Remarks2, dbo.OrderIN.Remarks3, 
                      dbo.T_PaymentTerms.TermsName AS PayTerms, T_PaymentTerms_1.TermsName AS PricingTerms, T_Port_2.PortName AS LoadingPort, 
                      T_Port_1.PortName AS DischargePort, dbo.T_Port.PortName AS Destination, dbo.T_Origin.OriginName AS Origin, dbo.OrderQTPackage.SizeLength_cm, 
                      dbo.OrderQTPackage.SizeWidth_cm, dbo.OrderQTPackage.SizeHeight_cm, dbo.OrderQTPackage.WeightNet_kg, dbo.OrderQTPackage.WeightGross_kg, 
                      dbo.OrderQTItems.ShippingMark, dbo.OrderQTPackage.SizeLength_in, dbo.OrderQTPackage.SizeWidth_in, dbo.OrderQTPackage.SizeHeight_in
FROM         dbo.OrderIN LEFT OUTER JOIN
                      dbo.T_Port ON dbo.OrderIN.Destination = dbo.T_Port.PortId LEFT OUTER JOIN
                      dbo.Customer ON dbo.OrderIN.CustomerId = dbo.Customer.CustomerId LEFT OUTER JOIN
                      dbo.CustomerAddress ON dbo.Customer.CustomerId = dbo.CustomerAddress.CustomerId LEFT OUTER JOIN
                      dbo.T_PaymentTerms ON dbo.OrderIN.PaymentTerms = dbo.T_PaymentTerms.TermsId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS T_PaymentTerms_1 ON dbo.OrderIN.PricingTerms = T_PaymentTerms_1.TermsId LEFT OUTER JOIN
                      dbo.T_Port AS T_Port_2 ON dbo.OrderIN.LoadingPort = T_Port_2.PortId LEFT OUTER JOIN
                      dbo.T_Port AS T_Port_1 ON dbo.OrderIN.DischargePort = T_Port_1.PortId LEFT OUTER JOIN
                      dbo.T_Origin ON dbo.OrderIN.OriginId = dbo.T_Origin.OriginId RIGHT OUTER JOIN
                      dbo.OrderINItems ON dbo.OrderIN.OrderINId = dbo.OrderINItems.OrderINId LEFT OUTER JOIN
                      dbo.OrderSCItems ON dbo.OrderINItems.OrderSCItemsId = dbo.OrderSCItems.OrderSCItemsId LEFT OUTER JOIN
                      dbo.OrderQTItems ON dbo.OrderSCItems.OrderQTItemId = dbo.OrderQTItems.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderQT ON dbo.OrderQTItems.OrderQTId = dbo.OrderQT.OrderQTId LEFT OUTER JOIN
                      dbo.OrderSC ON dbo.OrderSCItems.OrderSCId = dbo.OrderSC.OrderSCId LEFT OUTER JOIN
                      dbo.Article ON dbo.OrderQTItems.ArticleId = dbo.Article.ArticleId LEFT OUTER JOIN
                      dbo.T_Package ON dbo.OrderQTItems.PackageId = dbo.T_Package.PackageId LEFT OUTER JOIN
                      dbo.OrderQTPackage ON dbo.OrderQTItems.OrderQTItemId = dbo.OrderQTPackage.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_AgeGrading ON dbo.Article.AgeGradingId = dbo.T_AgeGrading.AgeGradingId LEFT OUTER JOIN
                      dbo.T_Currency ON dbo.OrderQT.CurrencyId = dbo.T_Currency.CurrencyId;


-- dbo.vwRptInvoice_Charges source

CREATE OR ALTER VIEW [dbo].[vwRptInvoice_Charges]
AS
SELECT     dbo.OrderIN.OrderINId, dbo.OrderIN.INNumber, dbo.OrderINCharges.OrderINChargeId, dbo.OrderINCharges.ChargeId, dbo.OrderINCharges.Description, 
                      dbo.OrderINCharges.Amount
FROM         dbo.OrderIN INNER JOIN
                      dbo.OrderINCharges ON dbo.OrderIN.OrderINId = dbo.OrderINCharges.OrderINId;


-- dbo.vwRptPreOrderList source

CREATE OR ALTER VIEW [dbo].[vwRptPreOrderList]
AS
SELECT     dbo.OrderPL.PLNumber, dbo.OrderPL.PLDate, dbo.OrderPL.Revision, dbo.Customer.CustomerName AS CustName, dbo.OrderPLItems.LineNumber AS PLLineNo, 
                      dbo.OrderQT.QTNumber, dbo.OrderQTItems.LineNumber AS QTLineNo, dbo.Article.SKU, dbo.Article.ArticleId, dbo.Article.ArticleCode, dbo.Supplier.SupplierCode, 
                      dbo.Supplier.SupplierName AS SuppName, dbo.T_Package.PackageName AS Package, dbo.Article.ArticleName AS ArtName, 
                      dbo.T_AgeGrading.AgeGradingName AS AgeGrading, dbo.Article.ColorPattern, dbo.OrderQTItems.Particular, dbo.OrderQTItems.CustRef, 
                      dbo.OrderQTItems.Qty AS OrderedQty, ocny.CurrencyCode AS OrderedCny, dbo.OrderQTItems.Unit AS OrderedUnit, dbo.OrderQTItems.Amount AS QuotedUnitAmt, 
                      dbo.OrderQTItems.FactoryCost, dbo.OrderQTItems.Margin, dbo.OrderQTItems.FCL, dbo.OrderQTItems.LCL, dbo.OrderQTSupplier.SuppRef, 
                      dbo.T_Currency.CurrencyCode AS FactoryCny, dbo.OrderQTSupplier.FCLCost, dbo.OrderQTSupplier.LCLCost, dbo.OrderQTPackage.InnerBox, 
                      dbo.OrderQTPackage.OuterBox, dbo.OrderQTPackage.Unit AS PackingUnit, dbo.OrderQTPackage.CUFT, dbo.CustomerAddress.AddrText AS CustAddr, 
                      dbo.CustomerAddress.Phone1_Label, dbo.CustomerAddress.Phone1_Text, dbo.CustomerAddress.Phone2_Label, dbo.CustomerAddress.Phone2_Text, 
                      dbo.CustomerAddress.Phone3_Label, dbo.CustomerAddress.Phone3_Text, dbo.CustomerAddress.Phone4_Label, dbo.CustomerAddress.Phone4_Text, 
                      dbo.CustomerAddress.Phone5_Label, dbo.CustomerAddress.Phone5_Text
FROM         dbo.CustomerAddress INNER JOIN
                      dbo.Customer ON dbo.CustomerAddress.CustomerId = dbo.Customer.CustomerId RIGHT OUTER JOIN
                      dbo.OrderPLItems LEFT OUTER JOIN
                      dbo.OrderQTItems ON dbo.OrderPLItems.OrderQTItemId = dbo.OrderQTItems.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderQTPackage ON dbo.OrderQTItems.OrderQTItemId = dbo.OrderQTPackage.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderQTSupplier ON dbo.OrderQTPackage.OrderQTItemId = dbo.OrderQTSupplier.OrderQTItemId LEFT OUTER JOIN
                      dbo.Article ON dbo.OrderQTItems.ArticleId = dbo.Article.ArticleId LEFT OUTER JOIN
                      dbo.OrderPL ON dbo.OrderPLItems.OrderPLId = dbo.OrderPL.OrderPLId ON dbo.Customer.CustomerId = dbo.OrderPL.CustomerId LEFT OUTER JOIN
                      dbo.T_Package ON dbo.OrderQTItems.PackageId = dbo.T_Package.PackageId LEFT OUTER JOIN
                      dbo.Supplier ON dbo.OrderQTItems.SupplierId = dbo.Supplier.SupplierId LEFT OUTER JOIN
                      dbo.T_AgeGrading ON dbo.Article.AgeGradingId = dbo.T_AgeGrading.AgeGradingId LEFT OUTER JOIN
                      dbo.OrderQT ON dbo.OrderQTItems.OrderQTId = dbo.OrderQT.OrderQTId LEFT OUTER JOIN
                      dbo.T_Currency ON dbo.OrderQTSupplier.CurrencyId = dbo.T_Currency.CurrencyId LEFT OUTER JOIN
                      dbo.T_Currency AS ocny ON dbo.OrderQT.CurrencyId = ocny.CurrencyId;


-- dbo.vwRptPreOrderList_CustShipment source

CREATE OR ALTER VIEW [dbo].[vwRptPreOrderList_CustShipment]
AS
SELECT     TOP (100) PERCENT dbo.OrderPL.PLNumber, dbo.OrderPLItems.LineNumber, dbo.Article.ArticleCode, dbo.Article.ArticleName, dbo.OrderQTCustShipping.ShippedOn, 
                      dbo.OrderQTCustShipping.QtyOrdered, dbo.OrderQTCustShipping.QtyShipped, dbo.OrderQTCustShipping.Status
FROM         dbo.OrderPLItems LEFT OUTER JOIN
                      dbo.OrderPL ON dbo.OrderPLItems.OrderPLId = dbo.OrderPL.OrderPLId RIGHT OUTER JOIN
                      dbo.OrderQTCustShipping ON dbo.OrderPLItems.OrderQTItemId = dbo.OrderQTCustShipping.OrderQTItemId LEFT OUTER JOIN
                      dbo.Article RIGHT OUTER JOIN
                      dbo.OrderQTItems ON dbo.Article.ArticleId = dbo.OrderQTItems.ArticleId ON dbo.OrderQTCustShipping.OrderQTItemId = dbo.OrderQTItems.OrderQTItemId;


-- dbo.vwRptPreOrderList_SuppShipment source

CREATE OR ALTER VIEW [dbo].[vwRptPreOrderList_SuppShipment]
AS
SELECT     dbo.OrderPL.PLNumber, dbo.OrderPLItems.LineNumber, dbo.Article.ArticleCode, dbo.Article.ArticleName, dbo.OrderQTSuppShipping.DateShipped, 
                      dbo.OrderQTSuppShipping.QtyOrdered, dbo.OrderQTSuppShipping.QtyShipped, dbo.OrderQTSuppShipping.Status
FROM         dbo.OrderPLItems LEFT OUTER JOIN
                      dbo.OrderPL ON dbo.OrderPLItems.OrderPLId = dbo.OrderPL.OrderPLId RIGHT OUTER JOIN
                      dbo.OrderQTSuppShipping ON dbo.OrderPLItems.OrderQTItemId = dbo.OrderQTSuppShipping.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderQTItems LEFT OUTER JOIN
                      dbo.Article ON dbo.OrderQTItems.ArticleId = dbo.Article.ArticleId ON dbo.OrderQTSuppShipping.OrderQTItemId = dbo.OrderQTItems.OrderQTItemId;


-- dbo.vwRptPriceList source

CREATE OR ALTER VIEW [dbo].[vwRptPriceList]
AS
SELECT     TOP (100) PERCENT qt.QTNumber, qt.QTDate, cust.CustomerName AS CustName, custad.AddrText AS CustAddr, custad.Phone1_Label, custad.Phone1_Text, 
                      custad.Phone2_Label, custad.Phone2_Text, custad.Phone3_Label, custad.Phone3_Text, custad.Phone4_Label, custad.Phone4_Text, custad.Phone5_Label, 
                      custad.Phone5_Text, pyt.TermsName AS PayTerms, curr.CurrencyCode AS CurrencyUsed, qt.Remarks, qt.Remarks2, qt.Remarks3, qti.LineNumber AS QTLineNo, 
                      qti.CustRef, a.ArticleCode, a.ArticleName AS ArtName, ag.AgeGradingName AS AgeGrading, p.PackageName AS Package, qti.Particular, qti.Carton, qti.Qty, qti.Unit, 
                      qti.Amount AS UnitAmt, qtp.InnerBox, qtp.OuterBox, qtp.Unit AS PackageUnit, qtp.CUFT, qti.ShippingMark, qti.SampleQty, dbo.OrderQTSupplier.SuppRef, 
                      dbo.Supplier.SupplierName, a.ArticleId, a.ColorPattern
FROM         dbo.T_Currency AS curr FULL OUTER JOIN
                      dbo.T_AgeGrading AS ag RIGHT OUTER JOIN
                      dbo.Article AS a ON ag.AgeGradingId = a.AgeGradingId RIGHT OUTER JOIN
                      dbo.OrderQTPackage AS qtp RIGHT OUTER JOIN
                      dbo.OrderQTSupplier RIGHT OUTER JOIN
                      dbo.OrderQTItems AS qti INNER JOIN
                      dbo.Supplier ON qti.SupplierId = dbo.Supplier.SupplierId ON dbo.OrderQTSupplier.OrderQTItemId = qti.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Package AS p ON qti.PackageId = p.PackageId ON qtp.OrderQTItemId = qti.OrderQTItemId ON a.ArticleId = qti.ArticleId FULL OUTER JOIN
                      dbo.T_PaymentTerms AS pyt FULL OUTER JOIN
                      dbo.CustomerAddress AS custad RIGHT OUTER JOIN
                      dbo.OrderQT AS qt LEFT OUTER JOIN
                      dbo.Customer AS cust ON qt.CustomerId = cust.CustomerId ON custad.CustomerId = cust.CustomerId ON pyt.TermsId = qt.TermsId ON qti.OrderQTId = qt.OrderQTId ON 
                      curr.CurrencyId = qt.CurrencyId
WHERE     (qt.QTNumber IS NOT NULL);


-- dbo.vwRptProformaInvoiceList source

CREATE OR ALTER VIEW [dbo].[vwRptProformaInvoiceList]
AS
SELECT     sc.SCNumber, sci.LineNumber AS SCLineNo, qti.CustRef, a.ArticleCode, a.ArticleName AS ArtName, p.PackageName AS Package, qti.Carton, qti.Qty, qt.Unit, 
                      qti.Amount AS UnitAmt, cny.CurrencyCode AS CurrencyUsed, qtp.InnerBox, qtp.OuterBox, qtp.Unit AS PackageUnit, qtp.CUFT, sc.SCDate, 
                      cust.CustomerName AS CustName, custa.AddrText AS CustAddr, sc.YourOrderNo, sc.YourRef, sc.Carrier, tpt.TermsName AS PayTerms, lp.PortName AS LoadingPort, 
                      dsp.PortName AS Destination, o.OriginName AS Origin, tpy.TermsName AS PricingTerms, dp.PortName AS DischargePort, qti.ShippingMark, qtcs.ShippedOn, 
                      qtcs.QtyShipped, custa.Phone1_Label, custa.Phone1_Text, custa.Phone2_Label, custa.Phone2_Text, custa.Phone3_Label, custa.Phone3_Text, custa.Phone4_Label, 
                      custa.Phone4_Text, custa.Phone5_Label, custa.Phone5_Text
FROM         dbo.T_Port AS dp RIGHT OUTER JOIN
                      dbo.OrderSC AS sc LEFT OUTER JOIN
                      dbo.T_Port AS dsp ON sc.Destination = dsp.PortId LEFT OUTER JOIN
                      dbo.Customer AS cust ON sc.CustomerId = cust.CustomerId LEFT OUTER JOIN
                      dbo.CustomerAddress AS custa ON cust.CustomerId = custa.CustomerId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS tpt ON sc.PaymentTerms = tpt.TermsId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS tpy ON sc.PricingTerms = tpy.TermsId LEFT OUTER JOIN
                      dbo.T_Port AS lp ON sc.LoadingPort = lp.PortId ON dp.PortId = sc.DischargePort LEFT OUTER JOIN
                      dbo.T_Origin AS o ON sc.OriginId = o.OriginId RIGHT OUTER JOIN
                      dbo.OrderSCItems AS sci ON sc.OrderSCId = sci.OrderSCId LEFT OUTER JOIN
                      dbo.Article AS a RIGHT OUTER JOIN
                      dbo.OrderQTItems AS qti ON a.ArticleId = qti.ArticleId ON sci.OrderQTItemId = qti.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Package AS p ON qti.PackageId = p.PackageId LEFT OUTER JOIN
                      dbo.OrderQT AS qt ON qti.OrderQTId = qt.OrderQTId LEFT OUTER JOIN
                      dbo.OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Currency AS cny ON qt.CurrencyId = cny.CurrencyId LEFT OUTER JOIN
                      dbo.OrderQTCustShipping AS qtcs ON qtcs.OrderQTItemId = qti.OrderQTItemId;


-- dbo.vwRptPurchaseContractList source

CREATE OR ALTER VIEW [dbo].[vwRptPurchaseContractList]
AS
SELECT     pc.PCNumber, pci.LineNumber AS PCLineNo, pc.PCDate, qt.QTNumber, qti.LineNumber AS QTLineNo, a.ArticleCode, qti.CustRef, qts.SuppRef, a.ArticleId, 
                      a.ArticleName AS ArtName, p.PackageName AS Package, ag.AgeGradingName AS AgeGrading, a.ColorPattern, qti.Particular, qti.Carton, qti.Qty AS OrderedQty, 
                      qti.Unit AS OrderedUnit, qti.FactoryCost AS UnitCost, cny.CurrencyCode AS CostCny, supp.SupplierName AS SuppName, suppa.AddrText AS SuppAddr, 
                      suppa.Phone1_Label, suppa.Phone1_Text, suppa.Phone2_Label, suppa.Phone2_Text, suppa.Phone3_Label, suppa.Phone3_Text, suppa.Phone4_Label, 
                      suppa.Phone4_Text, suppa.Phone5_Label, suppa.Phone5_Text, pc.YourRef, pc.Carrier, pyt.TermsName AS PayTerms, pyp.TermsName AS PricingTerms, 
                      lp.PortName AS LoadingPort, dp.PortName AS DischargePort, c.CountryName AS Destination, o.OriginName AS Origin, pc.Remarks, pc.Remarks2, pc.Remarks3, 
                      qtp.Unit AS PackUnit, qtp.InnerBox, qtp.OuterBox, qtp.CUFT, qti.ShippingMark, qtss.DateShipped, qtss.QtyShipped
FROM         dbo.OrderPCItems AS pci LEFT OUTER JOIN
                      dbo.OrderPC AS pc ON pci.OrderPCId = pc.OrderPCId LEFT OUTER JOIN
                      dbo.Supplier AS supp LEFT OUTER JOIN
                      dbo.SupplierAddress AS suppa ON supp.SupplierId = suppa.SupplierId ON pc.SupplierId = supp.SupplierId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS pyt ON pc.PaymentTerms = pyt.TermsId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS pyp ON pc.PricingTerms = pyp.TermsId LEFT OUTER JOIN
                      dbo.T_Port AS lp ON pc.LoadingPort = lp.PortId LEFT OUTER JOIN
                      dbo.T_Port AS dp ON pc.DischargePort = dp.PortId LEFT OUTER JOIN
                      dbo.T_Country AS c ON pc.Destination = c.CountryId LEFT OUTER JOIN
                      dbo.T_Origin AS o ON pc.OriginId = o.OriginId LEFT OUTER JOIN
                      dbo.OrderSCItems AS sci ON pci.OrderSCItemsId = sci.OrderSCItemsId LEFT OUTER JOIN
                      dbo.OrderQTItems AS qti ON sci.OrderQTItemId = qti.OrderQTItemId LEFT OUTER JOIN
                      dbo.Article AS a ON qti.ArticleId = a.ArticleId LEFT OUTER JOIN
                      dbo.T_Package AS p ON qti.PackageId = p.PackageId LEFT OUTER JOIN
                      dbo.OrderQTSupplier AS qts ON qti.OrderQTItemId = qts.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Currency AS cny ON qts.CurrencyId = cny.CurrencyId LEFT OUTER JOIN
                      dbo.OrderQT AS qt ON qti.OrderQTItemId = qt.OrderQTId LEFT OUTER JOIN
                      dbo.OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_AgeGrading AS ag ON a.AgeGradingId = ag.AgeGradingId LEFT OUTER JOIN
                      dbo.OrderQTSuppShipping AS qtss ON qti.OrderQTItemId = qtss.OrderQTItemId;


-- dbo.vwRptPurchaseContractShipmentList source

CREATE OR ALTER VIEW [dbo].[vwRptPurchaseContractShipmentList]
AS
SELECT     dbo.OrderPC.PCNumber, dbo.OrderPCItems.LineNumber, dbo.Article.ArticleCode, dbo.Article.ArticleName, dbo.OrderQTSuppShipping.DateShipped, 
                      dbo.OrderQTSuppShipping.QtyOrdered, dbo.OrderQTSuppShipping.QtyShipped, dbo.OrderQTSuppShipping.Status
FROM         dbo.OrderPC RIGHT OUTER JOIN
                      dbo.OrderPCItems ON dbo.OrderPC.OrderPCId = dbo.OrderPCItems.OrderPCId RIGHT OUTER JOIN
                      dbo.OrderSCItems RIGHT OUTER JOIN
                      dbo.OrderQTSuppShipping ON dbo.OrderSCItems.OrderQTItemId = dbo.OrderQTSuppShipping.OrderQTItemId LEFT OUTER JOIN
                      dbo.Article RIGHT OUTER JOIN
                      dbo.OrderQTItems ON dbo.Article.ArticleId = dbo.OrderQTItems.ArticleId ON dbo.OrderQTSuppShipping.OrderQTItemId = dbo.OrderQTItems.OrderQTItemId ON 
                      dbo.OrderPCItems.OrderSCItemsId = dbo.OrderSCItems.OrderSCItemsId;


-- dbo.vwRptSalesContractList source

CREATE OR ALTER VIEW [dbo].[vwRptSalesContractList]
AS
SELECT     TOP (100) PERCENT sc.SCNumber, sci.LineNumber AS SCLineNo, qt.QTNumber, qti.LineNumber AS QTLineNo, qti.CustRef, a.ArticleId, a.ArticleCode, 
                      a.ArticleName AS ArtName, ag.AgeGradingName AS AgeGrading, a.ColorPattern, p.PackageName AS Package, qti.Particular, qti.Carton, qti.Qty, qti.Unit, 
                      qti.Amount AS UnitAmt, curr.CurrencyCode AS CurrencyUsed, qtp.InnerBox, qtp.OuterBox, qtp.Unit AS PackageUnit, qtp.CUFT, sc.SCDate, 
                      cust.CustomerName AS CustName, custad.AddrText AS CustAddr, custad.Phone1_Label, custad.Phone1_Text, custad.Phone2_Label, custad.Phone2_Text, 
                      custad.Phone3_Label, custad.Phone3_Text, custad.Phone4_Label, custad.Phone4_Text, custad.Phone5_Label, custad.Phone5_Text, sc.YourOrderNo, sc.YourRef, 
                      sc.Carrier, pyt.TermsName AS PayTerms, lp.PortName AS LoadingPort, o.OriginName AS Origin, sc.Remarks, sc.Remarks2, sc.Remarks3, qti.ShippingMark, 
                      qtcs.ShippedOn, qtcs.QtyShipped, dp.PortName AS DischargePort, Dest.PortName AS Destination, pct.TermsName AS PricingTerms
FROM         dbo.OrderSC AS sc LEFT OUTER JOIN
                      dbo.Customer AS cust ON sc.CustomerId = cust.CustomerId LEFT OUTER JOIN
                      dbo.CustomerAddress AS custad ON cust.CustomerId = custad.CustomerId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS pyt ON sc.PaymentTerms = pyt.TermsId LEFT OUTER JOIN
                      dbo.T_PaymentTerms AS pct ON sc.PricingTerms = pct.TermsId LEFT OUTER JOIN
                      dbo.T_Port AS lp ON sc.LoadingPort = lp.PortId LEFT OUTER JOIN
                      dbo.T_Port AS dp ON sc.DischargePort = dp.PortId LEFT OUTER JOIN
                      dbo.T_Port AS Dest ON sc.Destination = Dest.PortId LEFT OUTER JOIN
                      dbo.T_Origin AS o ON sc.OriginId = o.OriginId LEFT OUTER JOIN
                      dbo.OrderSCItems AS sci ON sc.OrderSCId = sci.OrderSCId LEFT OUTER JOIN
                      dbo.T_AgeGrading AS ag RIGHT OUTER JOIN
                      dbo.Article AS a ON ag.AgeGradingId = a.AgeGradingId RIGHT OUTER JOIN
                      dbo.OrderQTItems AS qti LEFT OUTER JOIN
                      dbo.OrderQTCustShipping AS qtcs ON qti.OrderQTItemId = qtcs.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Package AS p ON qti.PackageId = p.PackageId LEFT OUTER JOIN
                      dbo.OrderQTPackage AS qtp ON qti.OrderQTItemId = qtp.OrderQTItemId ON a.ArticleId = qti.ArticleId ON sci.OrderQTItemId = qti.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Currency AS curr RIGHT OUTER JOIN
                      dbo.OrderQT AS qt ON curr.CurrencyId = qt.CurrencyId ON qti.OrderQTId = qt.OrderQTId
WHERE     (qt.QTNumber IS NOT NULL);


-- dbo.vwRptSalesContractShipmentList source

CREATE OR ALTER VIEW [dbo].[vwRptSalesContractShipmentList]
AS
SELECT     TOP (100) PERCENT dbo.OrderSC.SCNumber, dbo.OrderSCItems.LineNumber, dbo.Article.ArticleCode, dbo.Article.ArticleName, dbo.OrderQTCustShipping.ShippedOn, 
                      dbo.OrderQTCustShipping.QtyOrdered, dbo.OrderQTCustShipping.QtyShipped, dbo.OrderQTCustShipping.Status
FROM         dbo.Article RIGHT OUTER JOIN
                      dbo.OrderQTItems ON dbo.Article.ArticleId = dbo.OrderQTItems.ArticleId RIGHT OUTER JOIN
                      dbo.OrderQTCustShipping ON dbo.OrderQTItems.OrderQTItemId = dbo.OrderQTCustShipping.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderSCItems ON dbo.OrderQTCustShipping.OrderQTItemId = dbo.OrderSCItems.OrderQTItemId LEFT OUTER JOIN
                      dbo.OrderSC ON dbo.OrderSCItems.OrderSCId = dbo.OrderSC.OrderSCId;


-- dbo.vwRptShipmentAdviseList source

CREATE OR ALTER VIEW [dbo].[vwRptShipmentAdviseList]
AS
SELECT     dbo.Customer.CustomerName AS CustName, dbo.OrderIN.ShipmentDate, dbo.OrderIN.Carrier, dbo.T_Port.PortName AS FromPort, T_Port_1.PortName AS ToPort, 
                      dbo.OrderIN.INNumber, dbo.OrderINItems.LineNumber AS INLine, dbo.OrderSC.SCNumber, dbo.OrderQTItems.CustRef, dbo.Article.ArticleCode AS OurRef, 
                      dbo.OrderINItems.Qty AS InvoicedQty, dbo.OrderQTItems.Unit AS UoM, dbo.OrderINItems.Qty / dbo.OrderQTItems.Qty * dbo.OrderQTItems.Carton AS Carton, 
                      dbo.OrderINItems.Qty * dbo.OrderQTItems.Amount AS Amount, dbo.T_Currency.CurrencyCode AS CurrencyUsed, dbo.T_PaymentTerms.TermsName AS PricingTerms, 
                      dbo.OrderQTPackage.Unit AS FactoryUnit, dbo.OrderQTPackage.InnerBox, dbo.OrderQTPackage.OuterBox, dbo.OrderQTPackage.CUFT
FROM         dbo.OrderIN LEFT OUTER JOIN
                      dbo.OrderINItems ON dbo.OrderIN.OrderINId = dbo.OrderINItems.OrderINId LEFT OUTER JOIN
                      dbo.Customer ON dbo.OrderIN.CustomerId = dbo.Customer.CustomerId LEFT OUTER JOIN
                      dbo.OrderSCItems ON dbo.OrderINItems.OrderSCItemsId = dbo.OrderSCItems.OrderSCItemsId LEFT OUTER JOIN
                      dbo.OrderSC ON dbo.OrderSCItems.OrderSCId = dbo.OrderSC.OrderSCId LEFT OUTER JOIN
                      dbo.OrderQTItems ON dbo.OrderSCItems.OrderQTItemId = dbo.OrderQTItems.OrderQTItemId LEFT OUTER JOIN
                      dbo.T_Port ON dbo.OrderIN.LoadingPort = dbo.T_Port.PortId LEFT OUTER JOIN
                      dbo.T_Port AS T_Port_1 ON dbo.OrderIN.DischargePort = T_Port_1.PortId LEFT OUTER JOIN
                      dbo.T_PaymentTerms ON dbo.OrderIN.PricingTerms = dbo.T_PaymentTerms.TermsId LEFT OUTER JOIN
                      dbo.OrderQT ON dbo.OrderQTItems.OrderQTId = dbo.OrderQT.OrderQTId LEFT OUTER JOIN
                      dbo.OrderQTPackage ON dbo.OrderSCItems.OrderQTItemId = dbo.OrderQTPackage.OrderQTItemId LEFT OUTER JOIN
                      dbo.Article ON dbo.Article.ArticleId = dbo.OrderQTItems.ArticleId LEFT OUTER JOIN
                      dbo.T_Currency ON dbo.T_Currency.CurrencyId = dbo.OrderQT.CurrencyId;


-- dbo.vwSalesContractItemList source

CREATE OR ALTER VIEW dbo.vwSalesContractItemList
AS
SELECT        TOP (100) PERCENT m.OrderSCId, m.SCNumber, m.SCDate, i.OrderSCItemsId, i.LineNumber, a.ArticleId, a.SKU, a.ArticleCode, a.ArticleName, a.ArticleName_Chs, a.ArticleName_Cht, p.PackageId, p.PackageCode, 
                         p.PackageName, p.PackageName_Chs, p.PackageName_Cht, s.SupplierId, s.SupplierCode, s.SupplierName, s.SupplierName_Chs, s.SupplierName_Cht, qi.Particular, qi.CustRef, qi.PriceType, qi.FactoryCost, qi.Margin, qi.FCL, 
                         qi.LCL, qi.SampleQty, qi.Qty, qi.Unit, qi.Amount, qi.Carton, qi.GLAccount, qi.RefDocNo, qi.ShippingMark, qi.QtyIN, qi.QtyOUT, ps.SuppRef, c.CurrencyCode, ps.FCLCost, ps.LCLCost, qi.OrderQTItemId, ap.InnerBox, ap.OuterBox, 
                         ap.CUFT
FROM            dbo.OrderSC AS m INNER JOIN
                         dbo.Supplier AS s INNER JOIN
                         dbo.ArticleSupplier AS ps INNER JOIN
                         dbo.OrderSCItems AS i LEFT OUTER JOIN
                         dbo.OrderQTItems AS qi ON i.OrderQTItemId = qi.OrderQTItemId INNER JOIN
                         dbo.Article AS a ON qi.ArticleId = a.ArticleId ON ps.ArticleId = qi.ArticleId AND ps.SupplierId = qi.SupplierId ON s.SupplierId = ps.SupplierId INNER JOIN
                         dbo.T_Currency AS c ON ps.CurrencyId = c.CurrencyId ON m.OrderSCId = i.OrderSCId INNER JOIN
                         dbo.T_Package AS p ON qi.PackageId = p.PackageId INNER JOIN
                         dbo.OrderQTPackage AS ap ON qi.OrderQTItemId = ap.OrderQTItemId
ORDER BY m.SCNumber, i.LineNumber DESC;


-- dbo.vwSalesContractList source

CREATE OR ALTER VIEW [dbo].[vwSalesContractList]
AS
SELECT     TOP (100) PERCENT sc.OrderSCId, sc.SCNumber, sc.SCDate, sc.CustomerId, c.CustomerCode, c.CustomerName, c.CustomerName_Chs, c.CustomerName_Cht, 
                      sc.StaffId, s.Alias AS SalePerson, sc.SendFrom, sc.SendTo, sc.Remarks, sc.Remarks2, sc.Remarks3, sc.Revision, sc.InUse, sc.Status, sc.CreatedOn, 
                      s1.Alias AS CreatedBy, sc.ModifiedOn, s2.Alias AS ModifiedBy, sc.AccessedOn, s3.Alias AS AccessedBy, sc.Retired
FROM         dbo.OrderSC AS sc LEFT OUTER JOIN
                      dbo.Staff AS s2 ON sc.ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s3 ON sc.AccessedBy = s3.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON sc.CreatedBy = s1.StaffId LEFT OUTER JOIN
                      dbo.Customer AS c ON sc.CustomerId = c.CustomerId LEFT OUTER JOIN
                      dbo.Staff AS s ON sc.StaffId = s.StaffId
ORDER BY sc.SCNumber;


-- dbo.vwShipmentHistory source

CREATE OR ALTER VIEW [dbo].[vwShipmentHistory]
AS
SELECT	OrderQTItems.OrderQTItemId, Article.ArticleCode, Supplier.SupplierCode,
		(SELECT PackageCode FROM T_Package WHERE OrderQTItems.PackageId = T_Package.PackageId) AS PackageCode, 
		OrderQTItems.CustRef, ISNULL(OrderQTSupplier.SuppRef,'') AS SuppRef, Customer.CustomerName AS CustName, 
		Supplier.SupplierName AS SuppName, OrderSC.SCNumber, OrderQTCustShipping.ShippedOn AS ScheduledDate,
		OrderQTCustShipping.QtyOrdered AS ScheduledQty, OrderQTItems.Unit, OrderQTCustShipping.QtyShipped AS ShippedQty,
		(OrderQTCustShipping.QtyOrdered - OrderQTCustShipping.QtyShipped) AS OSQty, Article.SKU,
		Customer.CustomerId, Supplier.SupplierId
FROM	(((OrderQTCustShipping INNER JOIN ((OrderQTItems INNER JOIN Supplier ON OrderQTItems.SupplierId = Supplier.SupplierId) 
		INNER JOIN Article ON OrderQTItems.ArticleId = Article.ArticleId) ON OrderQTCustShipping.OrderQTItemId = OrderQTItems.OrderQTItemId) 
		INNER JOIN ((OrderSCItems INNER JOIN OrderSC ON OrderSCItems.OrderSCId = OrderSC.OrderSCId) 
		INNER JOIN Customer ON OrderSC.CustomerId = Customer.CustomerId) ON OrderQTCustShipping.OrderQTItemId = OrderSCItems.OrderQTItemId) 
		INNER JOIN OrderQTPackage ON OrderQTItems.OrderQTItemId = OrderQTPackage.OrderQTItemId) 
		INNER JOIN OrderQTSupplier ON OrderQTPackage.OrderQTItemId = OrderQTSupplier.OrderQTItemId;


-- dbo.vwStaffAddressList source

CREATE OR ALTER VIEW [dbo].[vwUserAddressList]
AS
SELECT     sa.StaffId, sa.StaffAddressId, sa.DefaultRec, za.AddressName, sa.AddrText, sa.AddrIsMailing, p1.PhoneName AS PhoneLable1, sa.Phone1_Text, 
                      p2.PhoneName AS PhoneLabel2, sa.Phone2_Text, p3.PhoneName AS PhoneLabel3, sa.Phone3_Text, p4.PhoneName AS PhoneLabel4, 
                      sa.Phone4_Text, p5.PhoneName AS PhoneLabel5, sa.Phone5_Text, ISNULL(sa.Notes, '') AS Notes, sa.CreatedOn, s1.Alias AS CreatedBy, 
                      sa.ModifiedOn, s2.Alias AS ModifiedBy, sa.Retired
FROM         dbo.StaffAddress AS sa INNER JOIN
                      dbo.Z_Phone AS p1 ON sa.Phone1_Label = p1.PhoneId INNER JOIN
                      dbo.Z_Phone AS p2 ON sa.Phone2_Label = p2.PhoneId INNER JOIN
                      dbo.Z_Phone AS p3 ON sa.Phone3_Label = p3.PhoneId INNER JOIN
                      dbo.Z_Phone AS p4 ON sa.Phone4_Label = p4.PhoneId INNER JOIN
                      dbo.Z_Phone AS p5 ON sa.Phone5_Label = p5.PhoneId INNER JOIN
                      dbo.Z_Address AS za ON sa.AddressId = za.AddressId LEFT OUTER JOIN
                      dbo.Staff AS s2 ON sa.ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON sa.CreatedBy = s1.StaffId;


-- dbo.vwStaffList source

CREATE OR ALTER VIEW [dbo].[vwUserList]
AS
SELECT     d.DivisionId, d.DivisionCode, d.DivisionName, d.DivisionName_Chs, d.DivisionName_Cht, g.GroupId, g.GroupCode, g.GroupName, g.GroupName_Chs, 
                      g.GroupName_Cht, u.StaffId, u.StaffCode, u.FullName, u.FirstName, u.LastName, u.Alias, u.Login, u.Password, u.Remarks, u.Status, u.CreatedOn, 
                      s1.Alias AS CreatedBy, u.ModifiedOn, s2.Alias AS ModifiedBy
FROM         dbo.T_Division AS d INNER JOIN
                      dbo.Staff AS u ON d.DivisionId = u.DivisionId INNER JOIN
                      dbo.T_Group AS g ON u.GroupId = g.GroupId LEFT OUTER JOIN
                      dbo.Staff AS s2 ON u.ModifiedBy = s2.StaffId LEFT OUTER JOIN
                      dbo.Staff AS s1 ON u.CreatedBy = s1.StaffId;


-- dbo.vwSupplierAddressList source

CREATE OR ALTER VIEW [dbo].[vwSupplierAddressList]
AS
SELECT     sa.SupplierAddressId, sa.SupplierId, sa.DefaultRec, za.AddressName, sa.AddrText, sa.AddrIsMailing, p1.PhoneName AS PhoneLabel1, 
                      sa.Phone1_Text, p2.PhoneName AS PhoneLabel2, sa.Phone2_Text, p3.PhoneName AS PhoneLabel3, sa.Phone3_Text, 
                      p4.PhoneName AS PhoneLabel4, sa.Phone4_Text, p5.PhoneName AS PhoneLabel5, sa.Phone5_Text, sa.Notes, sa.CreatedOn, s1.Alias AS CreatedBy, 
                      sa.ModifiedOn, s2.Alias AS ModifiedBy, sa.Retired
FROM         dbo.SupplierAddress AS sa INNER JOIN
                      dbo.Z_Address AS za ON sa.AddressId = za.AddressId INNER JOIN
                      dbo.Z_Phone AS p1 ON sa.Phone1_Label = p1.PhoneId INNER JOIN
                      dbo.Z_Phone AS p2 ON sa.Phone2_Label = p2.PhoneId INNER JOIN
                      dbo.Z_Phone AS p3 ON sa.Phone3_Label = p3.PhoneId INNER JOIN
                      dbo.Z_Phone AS p4 ON sa.Phone4_Label = p4.PhoneId INNER JOIN
                      dbo.Z_Phone AS p5 ON sa.Phone5_Label = p5.PhoneId INNER JOIN
                      dbo.Staff AS s1 ON sa.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON sa.ModifiedBy = s2.StaffId;


-- dbo.vwSupplierContactList source

CREATE OR ALTER VIEW [dbo].[vwSupplierContactList]
AS
SELECT     sc.SupplierContactId, sc.SupplierId, sc.DefaultRec, zs.SalutationName, sc.FullName, sc.FirstName, sc.LastName, zj.JobTitleName, 
                      p1.PhoneName AS PhoneLabel1, sc.Phone1_Text, p2.PhoneName AS PhoneLabel2, sc.Phone2_Text, p3.PhoneName AS PhoneLabel3, 
                      sc.Phone3_Text, p4.PhoneName AS PhoneLabel4, sc.Phone4_Text, p5.PhoneName AS PhoneLabel5, sc.Phone5_Text, sc.Notes, sc.CreatedOn, 
                      s1.Alias AS CreatedBy, sc.ModifiedOn, s2.Alias AS ModifiedBy, sc.Retired
FROM         dbo.SupplierContact AS sc INNER JOIN
                      dbo.Z_Salutation AS zs ON sc.SalutationId = zs.SalutationId INNER JOIN
                      dbo.Z_JobTitle AS zj ON sc.JobTitleId = zj.JobTitleId INNER JOIN
                      dbo.Z_Phone AS p1 ON sc.Phone1_Label = p1.PhoneId INNER JOIN
                      dbo.Z_Phone AS p2 ON sc.Phone2_Label = p2.PhoneId INNER JOIN
                      dbo.Z_Phone AS p3 ON sc.Phone3_Label = p3.PhoneId INNER JOIN
                      dbo.Z_Phone AS p4 ON sc.Phone4_Label = p4.PhoneId INNER JOIN
                      dbo.Z_Phone AS p5 ON sc.Phone5_Label = p5.PhoneId INNER JOIN
                      dbo.Staff AS s1 ON sc.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON sc.ModifiedBy = s2.StaffId;


-- dbo.vwSupplierList source

CREATE OR ALTER VIEW [dbo].[vwSupplierList]
AS
SELECT     s.SupplierId, s.SupplierCode, s.SupplierName, s.SupplierName_Chs, s.SupplierName_Cht, s.ACNumber, r.RegionName, p.TermsName, s.Remarks, 
                      s.Status, s.CreatedOn, s1.Alias AS CreatedBy, s.ModifiedOn, s2.Alias AS ModifiedBy, s.Retired
FROM         dbo.Supplier AS s INNER JOIN
                      dbo.T_Region AS r ON s.RegionId = r.RegionId INNER JOIN
                      dbo.T_PaymentTerms AS p ON s.TermsId = p.TermsId INNER JOIN
                      dbo.Staff AS s1 ON s.CreatedBy = s1.StaffId INNER JOIN
                      dbo.Staff AS s2 ON s.ModifiedBy = s2.StaffId;


-- dbo.vwUserList source

CREATE OR ALTER VIEW [dbo].[vwUserList]
AS
SELECT     u.UserSid, u.UserType, u.Alias, u.LoginName, u.LoginPassword, s.FullName, s.CreatedOn, '' AS CreatedBy, s.ModifiedOn, s2.Alias AS ModifiedBy, s.Status
FROM         UserProfile AS u, Staff AS s, Staff AS s1, Staff AS s2
WHERE     u.UserSid = s.StaffId AND s.CreatedBy = '00000000-0000-0000-0000-000000000000' AND s.ModifiedBy = s2.StaffId
UNION
SELECT     u.UserSid, u.UserType, u.Alias, u.LoginName, u.LoginPassword, s.FullName, s.CreatedOn, s1.Alias, s.ModifiedOn, s2.Alias, s.Status
FROM         UserProfile AS u, Staff AS s, Staff AS s1, Staff AS s2
WHERE     u.UserSid = s.StaffId AND s.CreatedBy = s1.StaffId AND s.ModifiedBy = s2.StaffId
UNION
SELECT     u.UserSid, u.UserType, u.Alias, u.LoginName, u.LoginPassword, sc.FullName, sc.CreatedOn, s1.Alias, sc.ModifiedOn, s2.Alias, s.Status
FROM         UserProfile AS u, SupplierContact AS sc, Supplier AS s, Staff AS s1, Staff AS s2
WHERE     u.UserSid = sc.SupplierContactId AND sc.SupplierId = s.SupplierId AND sc.CreatedBy = s1.StaffId AND sc.ModifiedBy = s2.StaffId
UNION
SELECT     u.UserSid, u.UserType, u.Alias, u.LoginName, u.LoginPassword, cc.FullName, cc.CreatedOn, s1.Alias, cc.ModifiedOn, s2.Alias, c.Status
FROM         UserProfile AS u, CustomerContact AS cc, Customer AS c, Staff AS s1, Staff AS s2
WHERE     u.UserSid = cc.CustomerContactId AND cc.CustomerId = c.CustomerId AND cc.CreatedBy = s1.StaffId AND cc.ModifiedBy = s2.StaffId;