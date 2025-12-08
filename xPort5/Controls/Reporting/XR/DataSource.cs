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
        /// <summary>
        /// Retrieves invoice details for a specific invoice ID
        /// TODO: Migrate to ViewService when vwInvoiceDetails is converted to LINQ
        /// Note: vwInvoiceDetails may differ from vwInvoiceItemList - verify before migration
        /// </summary>
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
            string whereClause = "[INNumber] = '" + invoiceNumber + "'";
            DataSet ds = ViewService.Default.GetRptInvoiceCharges(whereClause, "");
            return ds.Tables[0];
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
            // Use ViewService instead of direct SQL query
            string whereClause = String.Format("[ArticleId] = '{0}'", productId);
            DataSet ds = ViewService.Default.GetProductList(whereClause, "");
            return ds.Tables[0];
        }
        /// <summary>
        /// Retrieves product packing information for a specific product
        /// TODO: Migrate to ViewService when vwProductPackage is converted to LINQ
        /// </summary>
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
        /// <summary>
        /// Retrieves product supplier information for a specific product
        /// TODO: Migrate to ViewService when vwProductSupplier is converted to LINQ
        /// </summary>
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
            string whereClause = String.Format("[QTNumber] = '{0}'", QTNumber);
            DataSet ds = ViewService.Default.GetRptPriceList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }

        #region SalesContract

        /// <summary>
        /// SalesContract
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable SalesContract(string SCNumber)
        {
            string whereClause = String.Format("[SCNumber] = '{0}'", SCNumber);
            DataSet ds = ViewService.Default.GetRptSalesContractList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }

        /// <summary>
        /// SalesContract
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable SalesContractByLineNumber(string SCNumber)
        {
            string whereClause = String.Format("[SCNumber] = '{0}'", SCNumber);
            DataSet ds = ViewService.Default.GetRptSalesContractList(whereClause, "[SCLineNo]");
            return ds.Tables[0];
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
            string whereClause = String.Format("[SCNumber] = '{0}'", SCNumber);
            DataSet ds = ViewService.Default.GetRptProformaInvoiceList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }

        /// <summary>
        /// ProformaInvoice
        /// </summary>
        /// <param name="SCNumber"></param>
        /// <returns></returns>
        public static DataTable ProformaInvoiceByLineNumber(string SCNumber)
        {
            string whereClause = String.Format("[SCNumber] = '{0}'", SCNumber);
            DataSet ds = ViewService.Default.GetRptProformaInvoiceList(whereClause, "[SCLineNo]");
            return ds.Tables[0];
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
            string whereClause = String.Format("[PCNumber] = '{0}'", PCNumber);
            DataSet ds = ViewService.Default.GetRptPurchaseContractList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
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
            string whereClause = String.Format("[INNumber] in ({0})", INNumber);
            DataSet ds = ViewService.Default.GetRptInvoiceList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
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
            string whereClause = String.Format("[INNumber] in ({0})", INNumber);
            DataSet ds = ViewService.Default.GetRptInvoiceList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }

        #endregion

        #region

        public static DataTable ShipmentAdvise(string INNumber)
        {
            string whereClause = String.Format("[INNumber] in ({0})", INNumber);
            DataSet ds = ViewService.Default.GetRptShipmentAdviseList(whereClause, "[OurRef]");
            return ds.Tables[0];
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
            string whereClause = String.Format("[PLNumber] = '{0}'", PLNumber);
            DataSet ds = ViewService.Default.GetRptPreOrderList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }

        #endregion


        public static DataTable SalesContractShipment(string scNumber,string lineNumber)
        {
            string whereClause = String.Format("[LineNumber] = '{0}' AND SCNumber='{1}'", lineNumber, scNumber);
            DataSet ds = ViewService.Default.GetRptSalesContractShipmentList(whereClause, "[ShippedOn]");
            return ds.Tables[0];
        }

        public static DataTable PurchaseContractShipment(string pcNumber,string lineNumber)
        {
            string whereClause = String.Format("[LineNumber] = '{0}' AND PCNumber='{1}'", lineNumber, pcNumber);
            DataSet ds = ViewService.Default.GetRptPurchaseContractShipmentList(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }

        public static DataTable PreOrderListCustShipment(string plNumber, string lineNumber)
        {
            string whereClause = String.Format("[LineNumber] = '{0}' AND PLNumber='{1}'", lineNumber, plNumber);
            DataSet ds = ViewService.Default.GetRptPreOrderListCustShipment(whereClause, "[ShippedOn]");
            return ds.Tables[0];
        }

        public static DataTable PreOrderListSuppShipment(string plNumber, string lineNumber)
        {
            string whereClause = String.Format("[LineNumber] = '{0}' AND PLNumber='{1}'", lineNumber, plNumber);
            DataSet ds = ViewService.Default.GetRptPreOrderListSuppShipment(whereClause, "[ArticleCode]");
            return ds.Tables[0];
        }
    }
}
