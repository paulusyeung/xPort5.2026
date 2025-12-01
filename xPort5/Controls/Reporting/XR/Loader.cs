using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Controls.Reporting.XR
{
    public class Loader
    {
        public static void ProductList(string sql)
        {
            if (sql != String.Empty)
            {
                DataTable dt = DataSource.ProductList(sql);

                xPort5.Factory.Product.Report.ProductListXr report = new xPort5.Factory.Product.Report.ProductListXr();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "Product List";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, sql));
            }
        }

        public static void PriceList(string qtNumber, string parameters)
        {
            if (qtNumber != String.Empty)
            {
                DataTable dt = DataSource.PriceList(qtNumber);

                xPort5.Order.Quotation.Reports.PriceListXr_Pdf report = new xPort5.Order.Quotation.Reports.PriceListXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "PriceList";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, qtNumber));
            }
        }

        public static void PriceListXls(string qtNumber,string parameters)
        { 
            if(qtNumber != string.Empty)
            {
                DataTable dt = DataSource.PriceList(qtNumber);

                xPort5.Order.Quotation.Reports.PriceListXr_Xls report = new xPort5.Order.Quotation.Reports.PriceListXr_Xls(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PriceList.xls";
                view.BinarySource = memStream;
                view.ReportFormat = "excel";
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, qtNumber));
            }
        }

        public static void SampleRequest(string qtNumber, string parameters)
        {
            if (qtNumber != String.Empty)
            {
                DataTable dt = DataSource.PriceList(qtNumber);

                xPort5.Order.Quotation.Reports.SampleRequestXr_Pdf report = new xPort5.Order.Quotation.Reports.SampleRequestXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "PriceListSample";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, qtNumber));
            }
        }

        public static void Quotation(string qtNumber)
        {
            if (qtNumber != String.Empty)
            {
                DataTable dt = DataSource.PriceList(qtNumber);

                xPort5.Order.Quotation.Reports.QuotationXr_Pdf report = new xPort5.Order.Quotation.Reports.QuotationXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                xPort5.Order.Quotation.Reports.ProductThumbnailXr_Pdf appendix = new xPort5.Order.Quotation.Reports.ProductThumbnailXr_Pdf();
                appendix.DataSource = dt;
                appendix.CreateDocument();

                report.Pages.AddRange(appendix.Pages);
                report.PrintingSystem.ContinuousPageNumbering = false;

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "Quotation";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, qtNumber));
            }
        }

        public static void QuotationWithPicture(string qtNumber)
        {
            if (qtNumber != String.Empty)
            {
                DataTable dt = DataSource.PriceList(qtNumber);

                xPort5.Order.Quotation.Reports.QuotationXr_Pdf report = new xPort5.Order.Quotation.Reports.QuotationXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                xPort5.Order.Quotation.Reports.ProductPictureXr_Pdf appendix = new xPort5.Order.Quotation.Reports.ProductPictureXr_Pdf();
                appendix.DataSource = dt;
                appendix.CreateDocument();

                report.Pages.AddRange(appendix.Pages);
                report.PrintingSystem.ContinuousPageNumbering = false;

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "QuotationWithPicture";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, qtNumber));
            }
        }

        public static void SalesContract(string scNumber,string parameters)
        {
            if (scNumber != String.Empty)
            {
                DataTable dt = DataSource.SalesContract(scNumber);

                xPort5.Order.SalesContract.Reports.SalesContractXr_Pdf report = new xPort5.Order.SalesContract.Reports.SalesContractXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "SalesContract";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, scNumber));
            }
        }

        public static void SalesContractByLineNumber(string scNumber, string parameters)
        {
            if (scNumber != String.Empty)
            {
                DataTable dt = DataSource.SalesContractByLineNumber(scNumber);

                xPort5.Order.SalesContract.Reports.SalesContractXr_Pdf report = new xPort5.Order.SalesContract.Reports.SalesContractXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "SalesContract";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, scNumber));
            }
        }

        public static void SalesContractXls(string scNumber)
        {
            if (scNumber != String.Empty)
            {
                DataTable dt = DataSource.SalesContract(scNumber);

                xPort5.Order.SalesContract.Reports.SalesContractXr_Xls report = new xPort5.Order.SalesContract.Reports.SalesContractXr_Xls();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "SalesContract.xls";
                viewer.BinarySource = memStream;
                viewer.ReportFormat = "excel";
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, scNumber));
            }
        }

        public static void ProformaInvoice(string scNumber,string parameters)
        {
            if (scNumber != String.Empty)
            {
                DataTable dt = DataSource.ProformaInvoice(scNumber);

                xPort5.Order.SalesContract.Reports.ProformaInvoiceXr_Pdf report = new xPort5.Order.SalesContract.Reports.ProformaInvoiceXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "ProformaInvoice";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, scNumber));
            } 
        }

        public static void ProformaInvoiceByLineNumber(string scNumber, string parameters)
        {
            if (scNumber != String.Empty)
            {
                DataTable dt = DataSource.ProformaInvoiceByLineNumber(scNumber);

                xPort5.Order.SalesContract.Reports.ProformaInvoiceXr_Pdf report = new xPort5.Order.SalesContract.Reports.ProformaInvoiceXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
                viewer.ReportName = "ProformaInvoice";
                viewer.BinarySource = memStream;
                viewer.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, scNumber));
            }
        }

        public static void PurchaseContract(string PCNumber,string parameters)
        { 
            if(PCNumber != string.Empty)
            {
                DataTable dt = DataSource.PurchaseContract(PCNumber);

                xPort5.Order.PurchaseContract.Reports.PurchaseContractXr_Pdf report = new xPort5.Order.PurchaseContract.Reports.PurchaseContractXr_Pdf(parameters);
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PurchaseContract";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PCNumber));
            }
        }

        public static void PurchaseContractXls(string PCNumber)
        {
            if (PCNumber != string.Empty)
            {
                DataTable dt = DataSource.PurchaseContract(PCNumber);

                xPort5.Order.PurchaseContract.Reports.PurchaseContractXr_Xls report = new xPort5.Order.PurchaseContract.Reports.PurchaseContractXr_Xls();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PurchaseContract.xls";
                view.BinarySource = memStream;
                view.ReportFormat = "excel";
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PCNumber));
            }
        }

        public static void PurchaseContractMark(string PCNumber,string parameters)
        {
            if (PCNumber != string.Empty)
            {
                DataTable dt = DataSource.PurchaseContract(PCNumber);

                xPort5.Order.PurchaseContract.Reports.PurchaseContractMarkXr_Pdf report = new xPort5.Order.PurchaseContract.Reports.PurchaseContractMarkXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PurchaseContractMark";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PCNumber));
            }
        }

        public static void Invoice(string INNumber)
        { 
            if(INNumber != string.Empty)
            {
                DataTable dt = DataSource.Invoice(INNumber);

                xPort5.Order.Invoice.Reports.InvoiceXr_Pdf report = new xPort5.Order.Invoice.Reports.InvoiceXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "Invoice";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void Invoice(int invoiceId)
        {
            //Acct_INMaster invoice = Acct_INMaster.Load(invoiceId);
            //if (invoice != null)
            //{
            //    DataTable dt = DataSource.Invoice(invoiceId);

            //    xPort5.Accounting.Reports.Invoice report1 = new xPort5.Accounting.Reports.Invoice();
            //    report1.InvoiceId = invoiceId;
            //    report1.PageTitle = "CUSTOMER COPY";
            //    report1.DataSource = dt;
            //    report1.CreateDocument();

            //    xPort5.Accounting.Reports.Invoice report2 = new xPort5.Accounting.Reports.Invoice();
            //    report2.InvoiceId = invoiceId;
            //    report2.PageTitle = "ADMIN COPY";
            //    report2.DataSource = dt;
            //    report2.CreateDocument();

            //    report1.Pages.AddRange(report2.Pages);
            //    System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            //    report1.ExportToPdf(memStream);

            //    xPort5.Controls.Reporting.XR.Viewer viewer = new xPort5.Controls.Reporting.XR.Viewer();
            //    viewer.ReportName = "invoice";
            //    viewer.BinarySource = memStream;
            //    viewer.Show();
            //}
        }

        public static void InvoiceXls(string INNumber)
        {
            if (INNumber != string.Empty)
            {
                DataTable dt = DataSource.Invoice(INNumber);

                xPort5.Order.Invoice.Reports.InvoiceXr_Xls report = new xPort5.Order.Invoice.Reports.InvoiceXr_Xls();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "Invoice.xls";
                view.BinarySource = memStream;
                view.ReportFormat = "excel";
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PackingList(string INNumber)
        {
            if (INNumber != string.Empty)
            {
                DataTable dt = DataSource.PackingList(INNumber);

                xPort5.Order.Invoice.Reports.PackingListXr_Pdf report = new xPort5.Order.Invoice.Reports.PackingListXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new Viewer();
                view.ReportName = "PackingList";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PackingListMeasurementInInch(string INNumber)
        { 
            if(INNumber != string.Empty)
            {
                DataTable dt = DataSource.PackingList(INNumber);

                xPort5.Order.Invoice.Reports.PackingListMeasurementInInchXr_Pdf report = new xPort5.Order.Invoice.Reports.PackingListMeasurementInInchXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PackingListMeasurementInInch";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PackingListForCF(string INNumber)
        { 
            if(INNumber != string.Empty)
            {
                DataTable dt = DataSource.PackingList(INNumber);

                xPort5.Order.Invoice.Reports.PackingListForCFXr_Pdf report = new xPort5.Order.Invoice.Reports.PackingListForCFXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PackingListForCF";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PackingListWithOneShippingMark(string INNumber)
        {
            if (INNumber != string.Empty)
            {
                DataTable dt = DataSource.PackingList(INNumber);

                xPort5.Order.Invoice.Reports.PackingListWithOneShippingMarkXr_Pdf report = new xPort5.Order.Invoice.Reports.PackingListWithOneShippingMarkXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PackingListWithOneShippingMark";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PackingListXls(string INNumber)
        {
            if (INNumber != string.Empty)
            {
                DataTable dt = DataSource.PackingList(INNumber);

                xPort5.Order.Invoice.Reports.PackingListXr_Xls report = new xPort5.Order.Invoice.Reports.PackingListXr_Xls();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new Viewer();
                view.ReportName = "PackingList.xls";
                view.BinarySource = memStream;
                view.ReportFormat = "excel";
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PackingListWithTotalQtyXls(string INNumber)
        {
            if (INNumber != string.Empty)
            {
                DataTable dt = DataSource.PackingList(INNumber);

                xPort5.Order.Invoice.Reports.PackingListWithTotalQtyXr_Xls report = new xPort5.Order.Invoice.Reports.PackingListWithTotalQtyXr_Xls();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new Viewer();
                view.ReportName = "PackingListWithTotalQty.xls";
                view.BinarySource = memStream;
                view.ReportFormat = "excel";
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void ShipmentAdvise(string INNumber)
        {
            if (INNumber != string.Empty)
            {
                DataTable dt = DataSource.ShipmentAdvise(INNumber);

                xPort5.Order.Invoice.Reports.ShipmentAdviseXr_Pdf report = new xPort5.Order.Invoice.Reports.ShipmentAdviseXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();
                
                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "ShipmentAdvise";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, INNumber));
            }
        }

        public static void PreOrderList(string PLNumber)
        {
            if (PLNumber != string.Empty)
            {
                DataTable dt = DataSource.PreOrderList(PLNumber);

                xPort5.Order.PreOrder.Reports.PreOrderListXr_Pdf report = new xPort5.Order.PreOrder.Reports.PreOrderListXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PreOrderList";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PLNumber));
            }
        }

        public static void PreOrderListXls(string PLNumber)
        {
            if (PLNumber != string.Empty)
            {
                DataTable dt = DataSource.PreOrderList(PLNumber);

                xPort5.Order.PreOrder.Reports.PreOrderListXr_Xls report = new xPort5.Order.PreOrder.Reports.PreOrderListXr_Xls();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToXls(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PreOrderList.xls";
                view.BinarySource = memStream;
                view.ReportFormat = "excel";
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PLNumber));
            }
        }

        public static void PreOrderListForCustomer(string PLNumber)
        {
            if (PLNumber != string.Empty)
            {
                DataTable dt = DataSource.PreOrderList(PLNumber);

                xPort5.Order.PreOrder.Reports.PreOrderListForCustomerXr_Pdf report = new xPort5.Order.PreOrder.Reports.PreOrderListForCustomerXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PreOrderListForCustomer";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PLNumber));
            }
        }

        public static void PreOrderListForSupplier(string PLNumber)
        {
            if (PLNumber != string.Empty)
            {
                DataTable dt = DataSource.PreOrderList(PLNumber);

                xPort5.Order.PreOrder.Reports.PreOrderListForSupplierXr_Pdf report = new xPort5.Order.PreOrder.Reports.PreOrderListForSupplierXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PreOrderListForSupplier";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PLNumber));
            }
        }

        public static void PreOrderListForSuppcode(string PLNumber)
        {
            if (PLNumber != string.Empty)
            {
                DataTable dt = DataSource.PreOrderList(PLNumber);

                xPort5.Order.PreOrder.Reports.PreOrderListForSuppcodeXr_Pdf report = new xPort5.Order.PreOrder.Reports.PreOrderListForSuppcodeXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "PreOrderListForSuppcode";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PLNumber));
            }
        }

        public static void ConformationSheet(string PLNumber)
        {
            if (PLNumber != string.Empty)
            {
                DataTable dt = DataSource.PreOrderList(PLNumber);

                xPort5.Order.PreOrder.Reports.ConFirmationSheetXr_Pdf report = new xPort5.Order.PreOrder.Reports.ConFirmationSheetXr_Pdf();
                report.DataSource = dt;
                report.CreateDocument();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                report.ExportToPdf(memStream);

                xPort5.Controls.Reporting.XR.Viewer view = new xPort5.Controls.Reporting.XR.Viewer();
                view.ReportName = "ConformationSheet";
                view.BinarySource = memStream;
                view.Show();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Print, String.Format("{0} {1}", MethodBase.GetCurrentMethod().Name, PLNumber));
            }
        }
    }
}
