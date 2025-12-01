using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Controls.Reporting.XR
{
    public class DataSource
    {
        public static DataTable Invoice(int invoiceId)
        {
            string sql = @"
SELECT  TOP (100) PERCENT
		[InvoiceId]
        ,[InvoiceNumber]
        ,[InvoiceDate]
        ,[InvoiceAmount]
        ,[OrderID]
        ,ISNULL([PaymentType], 0)
        ,ISNULL([PaidBy], '')
        ,[Remarks]
        ,[Status]
        ,[ClientName]
        ,[OrderedBy]
        ,[ReceivedOn]
        ,[CompletedOn]
        ,[INDetailsId]
        ,[ItemCode]
        ,[ItemDescription]
        ,[ItemQty]
        ,[ItemUoM]
        ,[ItemUnitAmt]
        ,[ItemDiscount]
        ,[ItemAmount]
FROM    [dbo].[vwInvoiceDetails]
WHERE	[InvoiceId] = " + invoiceId.ToString() + @"
ORDER BY [INDetailsId];
";
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
        public static DataTable InvoiceCharges(String invoiceNumber)
        {
            string sql = @"
SELECT TOP 100 PERCENT
       [OrderINId]
      ,[INNumber]
      ,[OrderINChargeId]
      ,[ChargeId]
      ,[Description]
      ,[Amount]
FROM [dbo].[vwRptInvoice_Charges]
WHERE	[INNumber] = '" + invoiceNumber + @"'
;
";
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
        public static DataTable ProductList(string sql)
        {
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
        public static DataTable ProductListGeneral(string productId)
        {
            string sql = String.Format(@"
SELECT TOP 100 PERCENT
       [ArticleId]
      ,[SKU]
      ,[ArticleCode]
      ,[ArticleName]
      ,[ArticleName_Chs]
      ,[ArticleName_Cht]
      ,[CategoryId]
      ,ISNULL([CategoryCode], '') AS 'CategoryCode'
      ,ISNULL([CategoryName], '') AS 'CategoryName'
      ,ISNULL([CategoryName_Chs], '') AS 'CategoryName_Chs'
      ,ISNULL([CategoryName_Cht], '') AS 'CategoryName_Cht'
      ,[AgeGradingId]
      ,ISNULL([AgeGradingCode], '') AS 'AgeGradingCode'
      ,ISNULL([AgeGradingName], '') AS 'AgeGradingName'
      ,ISNULL([AgeGradingName_Chs], '') AS 'AgeGradingName_Chs'
      ,ISNULL([AgeGradingName_Cht], '') AS 'AgeGradingName_Cht'
      ,[OriginId]
      ,ISNULL([OriginCode], '') AS 'OriginCode'
      ,ISNULL([OriginName], '') AS 'OriginName'
      ,ISNULL([OriginName_Chs], '') As 'OriginName_Chs'
      ,ISNULL([OriginName_Cht], '') AS 'OriginName_Cht'
      ,[Remarks]
      ,[ColorPattern]
      ,[Barcode]
      ,[Status]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120) AS 'CreatedOn'
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120) AS 'ModifiedOn'
      ,[ModifiedBy]
FROM [dbo].[vwProductList]
WHERE [ArticleId] = '{0}'
", productId);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
        public static DataTable ProductListPacking(string productId)
        {
            string sql = String.Format(@"
SELECT [ArticleId]
      ,[ArticlePackageId]
      ,[PackageId]
      ,[PackageName]
      ,[DefaultRec]
      ,[UomId]                  -- 5
      ,[UomName]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
      ,[SizeLength_in]          -- 10
      ,[SizeWidth_in]
      ,[SizeHeight_in]
      ,[SizeLength_cm]
      ,[SizeWidth_cm]
      ,[SizeHeight_cm]          -- 15
      ,[WeightGross_lb]
      ,[WeightNet_lb]
      ,[WeightGross_kg]
      ,[WeightNet_kg]
      ,[ContainerQty]           -- 20
      ,[ContainerSize]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
FROM [dbo].[vwProductPackage]
WHERE [ArticleId] = '{0}'
ORDER BY [PackageName], [DefaultRec] DESC
", productId);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
        public static DataTable ProductListSupplier(string productId)
        {
            string sql = String.Format(@"
SELECT [ArticleId]
      ,[ArticleSupplierId]
      ,[SupplierId]
      ,[SupplierName]
      ,[DefaultRec]
      ,[SuppRef]
      ,[CurrencyCode]
      ,[FCLCost]
      ,[LCLCost]
      ,[UnitCost]
      ,[Notes]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
FROM [dbo].[vwProductSupplier]
WHERE [ArticleId] = '{0}'
ORDER BY [SupplierName]
", productId);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        public static DataTable PriceList(string QTNumber)
        {
            string sql = String.Format(@"
SELECT [QTNumber]
      ,[QTDate]
      ,[CustName]
      ,[CustAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
      ,[PayTerms]
      ,[CurrencyUsed]
      ,[Remarks]
      ,[Remarks2]
      ,[Remarks3]
      ,[QTLineNo]
      ,[CustRef]
      ,[ArticleCode]
      ,[ArtName]
      ,[AgeGrading]
      ,[Package]
      ,[Particular]
      ,[Carton]
      ,[Qty]
      ,[Unit]
      ,[UnitAmt]
      ,[InnerBox]
      ,[OuterBox]
      ,[PackageUnit]
      ,[CUFT]
      ,[ShippingMark]
      ,[SampleQty]
      ,[SuppRef]
      ,[SupplierName]
      ,[ArticleId]
      ,[ColorPattern]
  FROM [dbo].[vwRptPriceList]
WHERE [QTNumber] = '{0}'
ORDER BY [ArticleCode]
", QTNumber);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        #region SalesContract

        /// <summary>
        /// SalesContract
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable SalesContract(string SCNumber)
        {
            string sql = String.Format(@"
SELECT DISTINCT [SCNumber]
      ,[SCLineNo]
      ,[QTNumber]
      ,[QTLineNo]
      ,[CustRef]
      ,[ArticleId]
      ,[ArticleCode]
      ,[ArtName]
      ,[AgeGrading]
      ,[ColorPattern]
      ,[Package]
      ,[Particular]
      ,[Carton]
      ,[Qty]
      ,[Unit]
      ,[UnitAmt]
      ,[CurrencyUsed]
      ,[InnerBox]
      ,[OuterBox]
      ,[PackageUnit]
      ,[CUFT]
      ,[SCDate]
      ,[CustName]
      ,[CustAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
      ,[YourOrderNo]
      ,[YourRef]
      ,[Carrier]
      ,[PayTerms]
      ,[LoadingPort]
      ,[Origin]
      ,[Remarks]
      ,[Remarks2]
      ,[Remarks3]
      ,[ShippingMark]
      ,[DischargePort]
      ,[Destination]
      ,[PricingTerms]
  FROM [dbo].[vwRptSalesContractList]
WHERE [SCNumber] = '{0}'
ORDER BY [ArticleCode]
", SCNumber);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        /// <summary>
        /// SalesContract
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable SalesContractByLineNumber(string SCNumber)
        {
            string sql = String.Format(@"
SELECT DISTINCT [SCNumber]
      ,[SCLineNo]
      ,[QTNumber]
      ,[QTLineNo]
      ,[CustRef]
      ,[ArticleId]
      ,[ArticleCode]
      ,[ArtName]
      ,[AgeGrading]
      ,[ColorPattern]
      ,[Package]
      ,[Particular]
      ,[Carton]
      ,[Qty]
      ,[Unit]
      ,[UnitAmt]
      ,[CurrencyUsed]
      ,[InnerBox]
      ,[OuterBox]
      ,[PackageUnit]
      ,[CUFT]
      ,[SCDate]
      ,[CustName]
      ,[CustAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
      ,[YourOrderNo]
      ,[YourRef]
      ,[Carrier]
      ,[PayTerms]
      ,[LoadingPort]
      ,[Origin]
      ,[Remarks]
      ,[Remarks2]
      ,[Remarks3]
      ,[ShippingMark]
      ,[DischargePort]
      ,[Destination]
      ,[PricingTerms]
  FROM [dbo].[vwRptSalesContractList]
WHERE [SCNumber] = '{0}'
ORDER BY [SCLineNo]
", SCNumber);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        #endregion

        #region ProformaInvoice

        /// <summary>
        /// ProformaInvoice
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable ProformaInvoice(string SCNumber)
        {
            string sql = String.Format(@"
SELECT DISTINCT [SCNumber]
      ,[SCLineNo]
      ,[CustRef]
      ,[ArticleCode]
      ,[ArtName]
      ,[Package]
      ,[Carton]
      ,[Qty]
      ,[Unit]
      ,[UnitAmt]
      ,[CurrencyUsed]
      ,[InnerBox]
      ,[OuterBox]
      ,[PackageUnit]
      ,[CUFT]
      ,[SCDate]
      ,[CustName]
      ,[CustAddr]
      ,[YourOrderNo]
      ,[YourRef]
      ,[Carrier]
      ,[PayTerms]
      ,[LoadingPort]
      ,[Destination]
      ,[Origin]
      ,[PricingTerms]
      ,[DischargePort]
      ,[ShippingMark]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
  FROM [dbo].[vwRptProformaInvoiceList]
WHERE [SCNumber] = '{0}'
ORDER BY [ArticleCode]
", SCNumber);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        /// <summary>
        /// ProformaInvoice
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable ProformaInvoiceByLineNumber(string SCNumber)
        {
            string sql = String.Format(@"
SELECT DISTINCT [SCNumber]
      ,[SCLineNo]
      ,[CustRef]
      ,[ArticleCode]
      ,[ArtName]
      ,[Package]
      ,[Carton]
      ,[Qty]
      ,[Unit]
      ,[UnitAmt]
      ,[CurrencyUsed]
      ,[InnerBox]
      ,[OuterBox]
      ,[PackageUnit]
      ,[CUFT]
      ,[SCDate]
      ,[CustName]
      ,[CustAddr]
      ,[YourOrderNo]
      ,[YourRef]
      ,[Carrier]
      ,[PayTerms]
      ,[LoadingPort]
      ,[Destination]
      ,[Origin]
      ,[PricingTerms]
      ,[DischargePort]
      ,[ShippingMark]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
  FROM [dbo].[vwRptProformaInvoiceList]
WHERE [SCNumber] = '{0}'
ORDER BY [SCLineNo]
", SCNumber);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
        #endregion

        #region PurchaseContract

        /// <summary>
        /// PurchaseContract
        /// </summary>
        /// <param name="PCNumber"></param>
        /// <returns></returns>
        public static DataTable PurchaseContract(string PCNumber)
        {
        string sql = String.Format(@"
SELECT DISTINCT [PCNumber]
      ,[PCLineNo]
      ,[PCDate]
      ,[QTNumber]
      ,[QTLineNo]
      ,[ArticleId]
      ,[ArticleCode]
      ,[CustRef]
      ,[SuppRef]
      ,[ArtName]
      ,[Package]
      ,[AgeGrading]
      ,[ColorPattern]
      ,[Particular]
      ,[Carton]
      ,[OrderedQty]
      ,[OrderedUnit]
      ,[UnitCost]
      ,[CostCny]
      ,[SuppName]
      ,[SuppAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
      ,[YourRef]
      ,[Carrier]
      ,[PayTerms]
      ,[PricingTerms]
      ,[LoadingPort]
      ,[DischargePort]
      ,[Destination]
      ,[Origin]
      ,[Remarks]
      ,[Remarks2]
      ,[Remarks3]
      ,[PackUnit]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
      ,[ShippingMark]
  FROM [dbo].[vwRptPurchaseContractList]
WHERE [PCNumber] = '{0}'
ORDER BY [ArticleCode]
", PCNumber);
            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        #endregion

        #region Invoice

        /// <summary>
        /// Invoice
        /// </summary>
        /// <param name="INNumber"></param>
        /// <returns></returns>
        public static DataTable Invoice(string INNumber)
        {
            string sql = String.Format(@"
SELECT DISTINCT [INNumber]
      ,[INLineNo]
      ,[QTNumber]
      ,[QTLineNo]
      ,[YourOrderNo]
      ,[SCNumber]
      ,[CustRef]
      ,[ArticleId]
      ,[ArticleCode]
      ,[ArtName]
      ,[AgeGrading]
      ,[ColorPattern]
      ,[Particular]
      ,[Package]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
      ,[UoM]
      ,[InvoiceQty]
      ,[Unit]
      ,[UnitAmount]
      ,[CurrencyUsed]
      ,[INDate]
      ,[CustName]
      ,[CustAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
      ,[YourRef]
      ,[Carrier]
      ,[DepartureDate]
      ,[Remarks]
      ,[Remarks2]
      ,[Remarks3]
      ,[PayTerms]
      ,[PricingTerms]
      ,[LoadingPort]
      ,[DischargePort]
      ,[Destination]
      ,[Origin]
  FROM [dbo].[vwRptInvoiceList]
WHERE [INNumber] in ({0})
ORDER BY [ArticleCode]
", INNumber);

            DataSet ds = new DataSet();
            using (ds = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return ds.Tables[0];
            }
        }

        #endregion

        #region PackingList

        /// <summary>
        /// PackingList
        /// </summary>
        /// <param name="INNumber"></param>
        /// <returns></returns>
        public static DataTable PackingList(string INNumber)
        { 
            string sql = String.Format(@"
SELECT [INNumber]
      ,[INLineNo]
      ,[QTNumber]
      ,[QTLineNo]
      ,[YourOrderNo]
      ,[SCNumber]
      ,[CustRef]
      ,[ArticleCode]
      ,[ArtName]
      ,[AgeGrading]
      ,[ColorPattern]
      ,[Particular]
      ,[Package]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
      ,[UoM]
      ,[InvoiceQty]
      ,[Unit]
      ,[UnitAmount]
      ,[CurrencyUsed]
      ,[INDate]
      ,[CustName]
      ,[CustAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
      ,[YourRef]
      ,[Carrier]
      ,[DepartureDate]
      ,[Remarks]
      ,[Remarks2]
      ,[Remarks3]
      ,[PayTerms]
      ,[PricingTerms]
      ,[LoadingPort]
      ,[DischargePort]
      ,[Destination]
      ,[Origin]
      ,[SizeLength_in]
      ,[SizeWidth_in]
      ,[SizeHeight_in]
      ,[SizeLength_cm]
      ,[SizeWidth_cm]
      ,[SizeHeight_cm]
      ,[WeightNet_kg]
      ,[WeightGross_kg]
      ,[ShippingMark]
  FROM [dbo].[vwRptInvoiceList]
WHERE [INNumber] in ({0})
ORDER BY [ArticleCode]
", INNumber);
            DataSet ds = new DataSet();
            using (ds = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return ds.Tables[0];
            }
        }

        #endregion

        #region

        public static DataTable ShipmentAdvise(string INNumber)
        {
            string sql = String.Format(@"
SELECT [CustName]
      ,[ShipmentDate]
      ,[Carrier]
      ,[FromPort]
      ,[ToPort]
      ,[INNumber]
      ,[INLine]
      ,[SCNumber]
      ,[CustRef]
      ,[OurRef]
      ,[InvoicedQty]
      ,[UoM]
      ,[Carton]
      ,[Amount]
      ,[CurrencyUsed]
      ,[PricingTerms]
      ,[FactoryUnit]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
  FROM [dbo].[vwRptShipmentAdviseList]
WHERE [INNumber] in ({0})
ORDER BY [OurRef]
", INNumber);

            DataSet ds = new DataSet();
            using (ds = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return ds.Tables[0];
            }
        }

        #endregion

        #region PreOrderList

        /// <summary>
        /// PreOrderList
        /// </summary>
        /// <param name="PLNumber"></param>
        /// <returns></returns>
        public static DataTable PreOrderList(string PLNumber)
        {
            string sql = String.Format(@"
SELECT [PLNumber]
      ,[PLDate]
      ,[Revision]
      ,[CustName]
      ,[PLLineNo]
      ,[QTNumber]
      ,[QTLineNo]
      ,[SKU]
      ,[ArticleId]
      ,[ArticleCode]
      ,[SupplierCode]
      ,[SuppName]
      ,[Package]
      ,[ArtName]
      ,[AgeGrading]
      ,[ColorPattern]
      ,[Particular]
      ,[CustRef]
      ,[OrderedQty]
      ,[OrderedCny]
      ,[OrderedUnit]
      ,[QuotedUnitAmt]
      ,[FactoryCost]
      ,[Margin]
      ,[FCL]
      ,[LCL]
      ,[SuppRef]
      ,[FactoryCny]
      ,[FCLCost]
      ,[LCLCost]
      ,[InnerBox]
      ,[OuterBox]
      ,[PackingUnit]
      ,[CUFT]
      ,[CustAddr]
      ,[Phone1_Label]
      ,[Phone1_Text]
      ,[Phone2_Label]
      ,[Phone2_Text]
      ,[Phone3_Label]
      ,[Phone3_Text]
      ,[Phone4_Label]
      ,[Phone4_Text]
      ,[Phone5_Label]
      ,[Phone5_Text]
  FROM [dbo].[vwRptPreOrderList]
WHERE [PLNumber] = '{0}'
ORDER BY [ArticleCode]
", PLNumber);

            DataSet ds = new DataSet();
            using (ds = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return ds.Tables[0];
            }
        }

        #endregion


        public static DataTable SalesContractShipment(string scNumber,string lineNumber)
        {
            string sql = String.Format(@"
SELECT TOP 1000 [SCNumber]
      ,[LineNumber]
      ,[ArticleCode]
      ,[ArticleName]
      ,[ShippedOn]
      ,[QtyOrdered]
      ,[QtyShipped]
      ,[Status]
FROM [dbo].[vwRptSalesContractShipmentList]
WHERE [LineNumber] = '{0}' AND SCNumber='{1}'
ORDER BY [ShippedOn]", lineNumber, scNumber);

            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        public static DataTable PurchaseContractShipment(string pcNumber,string lineNumber)
        {
            string sql = String.Format(@"
SELECT TOP 1000 [PCNumber]
      ,[LineNumber]
      ,[ArticleCode]
      ,[ArticleName]
      ,[DateShipped]
      ,[QtyOrdered]
      ,[QtyShipped]
      ,[Status]
FROM [dbo].[vwRptPurchaseContractShipmentList]
WHERE [LineNumber] = '{0}' AND PCNumber='{1}'
ORDER BY [ArticleCode]", lineNumber, pcNumber);

            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        public static DataTable PreOrderListCustShipment(string plNumber, string lineNumber)
        {
            string sql = String.Format(@"
SELECT TOP 1000 [PLNumber]
      ,[LineNumber]
      ,[ArticleCode]
      ,[ArticleName]
      ,[ShippedOn]
      ,[QtyOrdered]
      ,[QtyShipped]
      ,[Status]
FROM [dbo].[vwRptPreOrderList_CustShipment]
WHERE [LineNumber] = '{0}' AND PLNumber='{1}'
ORDER BY [ShippedOn]", lineNumber, plNumber);

            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }

        public static DataTable PreOrderListSuppShipment(string plNumber, string lineNumber)
        {
            string sql = String.Format(@"
SELECT TOP 1000 [PLNumber]
      ,[LineNumber]
      ,[ArticleCode]
      ,[ArticleName]
      ,[DateShipped]
      ,[QtyOrdered]
      ,[QtyShipped]
      ,[Status]
FROM [dbo].[vwRptPreOrderList_SuppShipment]
WHERE [LineNumber] = '{0}' AND PLNumber='{1}'
ORDER BY [ArticleCode]", lineNumber, plNumber);

            DataSet dataset = new DataSet();
            using (dataset = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql))
            {
                return dataset.Tables[0];
            }
        }
    }
}
