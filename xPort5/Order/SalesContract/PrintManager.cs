#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;

using xPort5.Controls.CustomPanel;
using System.Xml;
#endregion

namespace xPort5.Order.SalesContract
{
    public partial class PrintManager : Form
    {
        string xmlSource = System.Web.HttpContext.Current.Server.MapPath("~/Resources/List/ReportList_SalesContract.xml");

        public PrintManager()
        {
            InitializeComponent();
        }

        private void PrintManager_Load(object sender, EventArgs e)
        {
            Binding();
        }

        /// <summaryArticleCode
        /// Bindings this instance.
        /// </summary>
        private void Binding()
        {
            DataSet ds = new DataSet();
            ds.ReadXml(xmlSource);

            customPanel1.DataSource = ds.Tables["Report"];
        }

        /// <summary>
        /// Handles the DoubleClick event of the customPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void customPanel1_Click(object sender, EventArgs e)
        {
            if (sender is CustomPanel)
            {
                string tag = ((CustomPanel)sender).Tag.ToString();
                this.ViewReports(tag);
            }
        }

        /// <summary>
        /// Views the reports.
        /// </summary>
        /// <param name="tag">The tag.</param>
        private void ViewReports(string tag)
        {
            switch (tag.ToLower())
            {
                case "sales_contract":
                    xPort5.Controls.Reporting.XR.Loader.SalesContract(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "sales_contractbylinenumber":
                    xPort5.Controls.Reporting.XR.Loader.SalesContractByLineNumber(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "sales_contractwithoneshippingmark":
                    xPort5.Controls.Reporting.XR.Loader.SalesContract(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "sales_contractwithoutshippingmark":
                    xPort5.Controls.Reporting.XR.Loader.SalesContract(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "proforma_invoice":
                    xPort5.Controls.Reporting.XR.Loader.ProformaInvoice(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "proforma_invoicewithoutshippingmark":
                    xPort5.Controls.Reporting.XR.Loader.ProformaInvoice(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "proforma_invoicebylinenumber":
                    xPort5.Controls.Reporting.XR.Loader.ProformaInvoiceByLineNumber(SalesContractList.SalesContractNum, tag.ToLower());
                    break;
                case "sales_contract_xls":
                    xPort5.Controls.Reporting.XR.Loader.SalesContractXls(SalesContractList.SalesContractNum);
                    break;
            }

            //UpdateLastAccessedOn(tag);
        }

        /// <summary>
        /// Updates the last printed on.
        /// </summary>
        private void UpdateLastAccessedOn(string tagCode)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlSource);

            XmlNodeList xnList = xml.SelectNodes("root/Reports/Report[Tag='" + tagCode + "']");
            foreach (XmlNode xn in xnList)
            {
                xn.Attributes["LastAccessedOn"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }

            xml.Save(xmlSource);
        }

        #region Report

        /// <summary>
        /// Show haven't SubReport 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="reportName"></param>
        private void ShowReport(string[] orderBy, string reportName, string fromName, string fieldName,string dataSourceName,string parameters)
        {
            string sql = BuildSql(orderBy,fromName,fieldName);
            DataTable table = BindDataToReport(sql);
            string reportDataSourceName = dataSourceName;
            PreviewReport(table, reportDataSourceName, reportName,parameters);
        }

        /// <summary>
        /// Show have SubReport
        /// </summary>
        private void ShowSubReport(string[] orderBy,string reportName)
        {
            string sql = BuildDetailSql(orderBy);
            DataTable table = BindDataToReport();
            DataTable detailTable = BindDataToReport(sql);
            string reportDataSourceName = "xPort3_Controls_Reporting_RS_DataSource_vwPrint_PriceList";
            string reportSubSource = "xPort3_Controls_Reporting_RS_DataSource_vwPriceDetailList";
            PreviewReport(table, detailTable, reportDataSourceName, reportSubSource, reportName);
        }

        private string BuildSql(string[] orderByColumns, string fromName, string fieldName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT TOP 100 PERCENT *  FROM {0}",fromName);
            sql.AppendFormat(" WHERE {0} ='" + SalesContractList.SalesContractNum + "'",fieldName);
            sql.Append(" ORDER BY ");

            for (int i = 0; i < orderByColumns.Length; i++)
            {
                sql.Append(orderByColumns[i]);
                if (i != orderByColumns.Length - 1)
                {
                    sql.Append(",");
                }
            }
            return sql.ToString();
        }

        /// <summary>
        /// Bind Data to Report(Price List)
        /// </summary>
        /// <returns></returns>
        private DataTable BindDataToReport()
        {
            string sql = @" SELECT TOP 100 PERCENT * 
                            FROM vwPrint_PriceList 
                            WHERE QTNumber='" + SalesContractList.SalesContractNum + "'";

            return xPort5.EF6.SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql).Tables[0];
        }

        private string BuildDetailSql(string[] orderByColumns)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT TOP 100 PERCENT *  FROM vwPriceDetailList");
            sql.Append(" WHERE QTNumber ='" + SalesContractList.SalesContractNum + "'");
            sql.Append(" ORDER BY ");

            for (int i = 0; i < orderByColumns.Length; i++)
            {
                sql.Append(orderByColumns[i]);
                if (i != orderByColumns.Length - 1)
                {
                    sql.Append(",");
                }
            }
            return sql.ToString();
        }

        private DataTable BindDataToReport(string sql)
        {
            return xPort5.EF6.SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql).Tables[0];
        }

        private void PreviewReport(DataTable table, string reportDSName, string reportName,string parameters)
        {
            if (table.Rows.Count > 0)
            {
                string imagePath = "file:///" + System.Web.HttpContext.Current.Server.MapPath("~/Resources/Images/ContractHeader.jpg");
                string[,] param = { 
                {"QTNumber",table.Rows[0]["SCNumber"].ToString()},
                {"SCDate",table.Rows[0]["SCDate"].ToString()},
                {"CustName",table.Rows[0]["CustName"].ToString()},
                {"CustAddr",table.Rows[0]["CustAddr"].ToString()},
                {"Tel",table.Rows[0]["Phone1_Text"].ToString()},
                {"Fax",table.Rows[0]["Phone3_Text"].ToString()},
                {"YourOrderNo",table.Rows[0]["YourOrderNo"].ToString()},
                {"YourRef",table.Rows[0]["YourRef"].ToString()},
                {"Carrier",table.Rows[0]["Carrier"].ToString()},
                {"PayTerms",table.Rows[0]["PayTerms"].ToString()},
                {"PriceTerms",table.Rows[0]["PricingTerms"].ToString()},
                {"LoadingPort",table.Rows[0]["LoadingPort"].ToString()},
                {"DischargePort",table.Rows[0]["DischargePort"].ToString()},
                {"Origin",table.Rows[0]["Origin"].ToString()},
                {"Destination",table.Rows[0]["Destination"].ToString()},
                {"CurrencyUsed",table.Rows[0]["CurrencyUsed"].ToString()},
                {"imagePath",imagePath},
                {"Parameters",parameters},
                //{"SuppCurrency",table.Rows[0]["SuppCurrency"].ToString()}
                };




                xPort5.Controls.Reporting.RS.Viewer view = new xPort5.Controls.Reporting.RS.Viewer();
                view.Datasource = table;
                view.ReportDatasourceName = reportDSName;
                view.ReportName = reportName;
                view.Parameters = param;

                view.Show();

                //xPort5.Controls.Reporting.RS.RdlExport export = new xPort5.Controls.Reporting.RS.RdlExport();
                //export.Datasource = table;
                //export.ReportDatasourceName = reportDSName;
                //export.ReportName = reportName;
                //export.Parameters = param;

                //export.ToPdf();
            }
            else
            {
                MessageBox.Show("no detail record!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// View Report(Price List)
        /// </summary>
        private void PreviewReport(DataTable table, DataTable detailTable, string reportDSName, string reportSubSource, string reportName)
        {
            if (table.Rows.Count > 0)
            {
                string[,] param = { 
                {"CustName",table.Rows[0]["CustName"].ToString()},
                {"AddrText",table.Rows[0]["AddrText"].ToString()},
                {"QTNumber",table.Rows[0]["QTNumber"].ToString()},
                {"QTDate",table.Rows[0]["QTDate"].ToString()},
                {"Tel",table.Rows[0]["Phone1_Text"].ToString()},
                {"Fax",table.Rows[0]["Phone3_Text"].ToString()},
                {"CurrencyUsed",table.Rows[0]["CurrencyUsed"].ToString()}
                };

                xPort5.Controls.Reporting.RS.Viewer view = new xPort5.Controls.Reporting.RS.Viewer();

                Dictionary<string, DataTable> subSource = new Dictionary<string, DataTable>();
                subSource.Add(reportSubSource, detailTable);

                view.Datasource = table;
                view.SubReportDataSourceList = subSource;
                view.ReportDatasourceName = reportDSName;
                view.ReportName = reportName;
                //view.Parameters = param;

                view.Show();
            }
            else
            {
                MessageBox.Show("no detail record!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Quotation List Report Preview
        /// </summary>
        private void PreviewReport()
        {
            string sql = @"SELECT TOP 100 PERCENT *  FROM vwPrint_QuotationList
                           WHERE QTNumber ='" + SalesContractList.SalesContractNum + @"'
                           ORDER BY QTNumber, LineNumber";

            DataTable table = BindDataToReport(sql);

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Guid productId = new Guid(row["ArticleId"].ToString());
                    string path = xPort5.Controls.Utility.Resources.PictureFilePath(productId, xPort5.Controls.Utility.Product.KeyPicture(productId));

                    if (!System.IO.File.Exists(path))
                    {
                        row["ColorPattern"] = "file:///" + System.IO.Path.Combine(Context.Config.GetDirectory("Images"), "no_photo.jpg");
                    }
                    else
                    {
                        row["ColorPattern"] = "file:///" + path;
                    }

                    //if (row["PictureFile"].ToString().Length != 0 && string.IsNullOrEmpty(Context.Config.GetDirectory("ProductImage")))
                    //{
                    //    row["PictureFile"] = System.IO.Path.Combine(System.IO.Path.Combine(Context.Config.GetDirectory("ProductImage"), "Product"), row["ProductImage"].ToString());

                    //    if (!System.IO.File.Exists(row["ProductImage"].ToString()))
                    //    {
                    //        row["PictureFile"] = "file:///" + System.IO.Path.Combine(Context.Config.GetDirectory("Images"), "no_photo.jpg");
                    //    }
                    //    else
                    //    {
                    //        row["PictureFile"] = "file:///" + System.IO.Path.Combine(System.IO.Path.Combine(Context.Config.GetDirectory("ProductImage"), "Product"), row["ProductImage"].ToString());
                    //    }
                    //}
                }


                string[,] param = { 
                {"CustName",table.Rows[0]["CustName"].ToString()},
                {"AddrText",table.Rows[0]["AddrText"].ToString()},
                {"QTNumber",table.Rows[0]["QTNumber"].ToString()},
                {"QTDate",table.Rows[0]["QTDate"].ToString()},
                {"Tel",table.Rows[0]["Phone1_Text"].ToString()},
                {"Fax",table.Rows[0]["Phone3_Text"].ToString()},
                {"CurrencyUsed",table.Rows[0]["CurrencyUsed"].ToString()}
                };

                xPort5.Controls.Reporting.RS.Viewer view = new xPort5.Controls.Reporting.RS.Viewer();

                Dictionary<string, DataTable> subDataSource = new Dictionary<string, DataTable>();
                subDataSource.Add("xPort3_Controls_Reporting_RS_DataSource_vwPrint_QuotationList1", table);

                view.SubReportDataSourceList = subDataSource;

                view.Datasource = table;
                view.ReportDatasourceName = "xPort3_Controls_Reporting_RS_DataSource_vwPrint_QuotationList";
                view.ReportName = "xPort5.Order.Quotation.Reports.QuotationRdl.rdlc";
                //view.Parameters = param;
                view.Show();
            }
            else
            {
                MessageBox.Show("no detail record!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Quotation List Report Preview(without photo)
        /// </summary>
        private void PvReport()
        {
            string sql = @"SELECT TOP 100 PERCENT *  FROM vwPrint_QuotationList
                           WHERE QTNumber ='" + SalesContractList.SalesContractNum + @"'
                           ORDER BY QTNumber, LineNumber";

            DataTable table = BindDataToReport(sql);

            if (table.Rows.Count > 0)
            {
                string[,] param = { 
                {"CustName",table.Rows[0]["CustName"].ToString()},
                {"AddrText",table.Rows[0]["AddrText"].ToString()},
                {"QTNumber",table.Rows[0]["QTNumber"].ToString()},
                {"QTDate",table.Rows[0]["QTDate"].ToString()},
                {"Tel",table.Rows[0]["Phone1_Text"].ToString()},
                {"Fax",table.Rows[0]["Phone3_Text"].ToString()},
                {"CurrencyUsed",table.Rows[0]["CurrencyUsed"].ToString()},
                {"Terms",table.Rows[0]["Terms"].ToString()},
                };

                xPort5.Controls.Reporting.RS.Viewer view = new xPort5.Controls.Reporting.RS.Viewer();

                view.Datasource = table;
                view.ReportDatasourceName = "xPort3_Controls_Reporting_RS_DataSource_vwPrint_QuotationList";
                view.ReportName = "xPort5.Order.Quotation.Reports.QuotationWithoutPhotoRdl.rdlc";
                //view.Parameters = param;
                view.Show();
            }
            else
            {
                MessageBox.Show("no detail record!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
    }
}
