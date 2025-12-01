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

namespace xPort5.Order.Quotation
{
    public partial class PrintManager : Form
    {
        string xmlSource = System.Web.HttpContext.Current.Server.MapPath("~/Resources/List/ReportList_Quotation.xml");

        public PrintManager()
        {
            InitializeComponent();
        }

        private void PrintManager_Load(object sender, EventArgs e)
        {
            Binding();
        }

        /// <summary>
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
                case "price_list":
                    xPort5.Controls.Reporting.XR.Loader.PriceList(QuotationList.QuotationNum, tag.ToLower());
                    break;
                case "price_list_xls":
                    xPort5.Controls.Reporting.XR.Loader.PriceListXls(QuotationList.QuotationNum, tag.ToLower());
                    break;
                case "sample_request":
                    xPort5.Controls.Reporting.XR.Loader.SampleRequest(QuotationList.QuotationNum, tag.ToLower());
                    break;
                case "quotation":
                    xPort5.Controls.Reporting.XR.Loader.Quotation(QuotationList.QuotationNum);
                    break;
                case "quotation_with_photo":
                    xPort5.Controls.Reporting.XR.Loader.QuotationWithPicture(QuotationList.QuotationNum);
                    break;
                case "price_list_faster_version_sortby_article":
                    string[] orderBy2 = { "QTNumber", "ArticleCode" };
                    string reportName2 = "xPort5.Order.Quotation.Reports.PriceListFasterVersionByArticleRdl.rdlc";
                    ShowReport(orderBy2, reportName2);
                    break;
                case "price_list_sortby_articlecode":
                    string[] orderBy3 = { "QTNumber", "ArticleCode" };
                    string reportName3 = "xPort5.Order.Quotation.Reports.PriceListByArticleRdl.rdlc";
                    ShowSubReport(orderBy3, reportName3);
                    break;
                case "price_list_with_particular":
                    string[] orderBy4 = { "QTNumber", "LineNumber" };
                    string reportName4 = "xPort5.Order.Quotation.Reports.PriceListWithParticularRdl.rdlc";
                    ShowSubReport(orderBy4, reportName4);
                    break;
                case "price_list_without_suppliername":
                    string[] orderBy5 = { "QTNumber", "ArticleCode" };
                    string reportName5 = "xPort5.Order.Quotation.Reports.PriceListWithoutSuppNameRdl.rdlc";
                    ShowSubReport(orderBy5, reportName5);
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
        private void ShowReport(string[] orderBy, string reportName)
        {
            string sql = BuildSql(orderBy);
            DataTable table = BindDataToReport(sql);
            string reportDataSourceName = "xPort3_Controls_Reporting_RS_DataSource_vwPrint_CompletedPriceList";
            PreviewReport(table, reportDataSourceName, reportName);
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

        private string BuildSql(string[] orderByColumns)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT TOP 100 PERCENT *  FROM vwPrint_CompletedPriceList");
            sql.Append(" WHERE QTNumber ='" + QuotationList.QuotationNum + "'");
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
                            WHERE QTNumber='" + QuotationList.QuotationNum + "'";

            return xPort5.EF6.SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql).Tables[0];
        }

        private string BuildDetailSql(string[] orderByColumns)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT TOP 100 PERCENT *  FROM vwPriceDetailList");
            sql.Append(" WHERE QTNumber ='" + QuotationList.QuotationNum + "'");
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

        private void PreviewReport(DataTable table, string reportDSName, string reportName)
        {
            if (table.Rows.Count > 0)
            {
                //string imagePath = "file:///" + System.Web.HttpContext.Current.Server.MapPath("~/Resources/Images/logo_companydetail.jpg");
                string[,] param = { 
                {"CustName",table.Rows[0]["CustName"].ToString()},
                {"AddrText",table.Rows[0]["AddrText"].ToString()},
                {"QTNumber",table.Rows[0]["QTNumber"].ToString()},
                {"QTDate",table.Rows[0]["QTDate"].ToString()},
                {"Tel",table.Rows[0]["Phone1_Text"].ToString()},
                {"Fax",table.Rows[0]["Phone3_Text"].ToString()},
                {"SuppCurrency",table.Rows[0]["SuppCurrency"].ToString()}
                };
                xPort5.Controls.Reporting.RS.Viewer view = new xPort5.Controls.Reporting.RS.Viewer();

                view.Datasource = table;
                view.ReportDatasourceName = reportDSName;
                view.ReportName = reportName;
                view.Parameters = param;

                view.Show();
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
                view.Parameters = param;

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
                           WHERE QTNumber ='" + QuotationList.QuotationNum + @"'
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
                view.Parameters = param;
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
                           WHERE QTNumber ='" + QuotationList.QuotationNum + @"'
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
                view.Parameters = param;
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
