using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;

using Gizmox.WebGUI.Forms;
using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Admin.Olap
{
    public partial class SalesContract_v5 : System.Web.UI.Page
    {
        private DataSet dataSource = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 砌個 Toolbar - Export To Excel
            this.ansOlap.Items.Clear();
            DevExpress.Web.MenuItem item = new DevExpress.Web.MenuItem();
            item.Image.Url = "~//Resources//Icons//16x16//16_Excel.gif";
            item.Text = "Export to Excel"; // PageContext.Text;       //攞 RtfStats.cs 提供嘅名稱（有中英亙捩）

            this.ansOlap.Items.Add(item);
            this.ansOlap.ItemClick += new DevExpress.Web.MenuItemEventHandler(ansReceivablesOlap_ItemClick);
            #endregion

            pvgExporter.ASPxPivotGridID = pvgOlap.ID;

            InitialValues();
            if (dataSource != null)
            {
                BindData(dataSource);
            }
        }

        private void InitialValues()
        {
            string[] period = xPort5.Controls.Utility.OlapAdmin.DatePeriod.Split(',');

            if (period.Length > 1)
            {
                StringBuilder custList = new StringBuilder();

                #region prase Customer List Array
                if (VWGContext.Current.Session["CustomerList"] is HashSet<Guid>)
                {
                    HashSet<Guid> hashedList = VWGContext.Current.Session["CustomerList"] as HashSet<Guid>;
                    if (hashedList != null)
                    {
                        List<Guid> customerList = hashedList.ToList();
                        for (int i = 0; i < customerList.Count; i++)
                        {
                            if (i > 0 && i < customerList.Count)
                            {
                                custList.Append(",");
                            }
                            custList.Append("'").Append(customerList[i].ToString()).Append("'");
                        }
                    }
                }
                #endregion

                if (custList.Length > 0)
                {
                    #region 攞 SQL data
                    // 2025-01-XX Composer: Use LINQ query instead of stored procedure
                    dataSource = OlapService.Default.GetSalesContractSummary(
                        custList.ToString(),
                        period[0] + " 00:00:00",
                        period[1] + " 23:59:59");
                    #endregion
                }
            }
        }

        private void BindData(DataSet ds)
        {
            pvgOlap.DataSource = ds.Tables[0];
            pvgOlap.DataBind();

            if (pvgOlap.Fields.Count != 0) return;      // 如果個 PivotGrid 已經有料，咁就唔使再 set

            #region 砌最初 OLAP 個樣出嚟
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);
            pvgOlap.RetrieveFields();

            pvgOlap.OptionsPager.RowsPerPage = 0;       // hide pager
            ansOlap.Items[0].Text = oDict.GetWord("export_excel");

            #region Row Area
            pvgOlap.Fields["CustName"].Area = PivotArea.RowArea;
            pvgOlap.Fields["CustName"].Caption = oDict.GetWord("customer_name");
            pvgOlap.Fields["SCNumber"].Area = PivotArea.RowArea;
            pvgOlap.Fields["SCNumber"].Caption = oDict.GetWord("sales_contract");
            #endregion

            #region Column Area
            var year = new DevExpress.Web.ASPxPivotGrid.PivotGridField("SCDate", PivotArea.ColumnArea);
            year.GroupInterval = PivotGroupInterval.DateYear;
            year.Caption = oDict.GetWord("year");
            year.AreaIndex = 0;

            pvgOlap.Fields.Add(year);
            #endregion

            #region Data Area
            pvgOlap.Fields["ExtAmount"].Area = PivotArea.DataArea;
            pvgOlap.Fields["ExtAmount"].Caption = oDict.GetWord("amount");
            pvgOlap.Fields["ExtAmount"].ValueFormat.FormatString = "{0:n2}";

            pvgOlap.Fields["ExtHKDAmount"].Area = PivotArea.DataArea;
            pvgOlap.Fields["ExtHKDAmount"].Caption = "HKD " + oDict.GetWord("amount");
            pvgOlap.Fields["ExtHKDAmount"].ValueFormat.FormatString = "{0:n2}";
            #endregion

            #region Filter Area
            pvgOlap.Fields["Region"].Area = PivotArea.FilterArea;
            pvgOlap.Fields["Region"].Caption = oDict.GetWord("region");
            pvgOlap.Fields["SCDate"].Area = PivotArea.FilterArea;
            pvgOlap.Fields["SCDate"].Caption = oDict.GetWord("date");
            pvgOlap.Fields["SCDate"].ValueFormat.FormatType = FormatType.DateTime;
            pvgOlap.Fields["SCDate"].ValueFormat.FormatString = "yyyy-MM-dd";
            pvgOlap.Fields["CurrencyCode"].Area = PivotArea.FilterArea;
            pvgOlap.Fields["CurrencyCode"].Caption = oDict.GetWord("Currency");
            pvgOlap.Fields["ArticleCode"].Area = PivotArea.FilterArea;
            pvgOlap.Fields["ArticleCode"].Caption = oDict.GetWord("article_code");

            var quarter = new DevExpress.Web.ASPxPivotGrid.PivotGridField("SCDate", PivotArea.FilterArea);
            quarter.GroupInterval = PivotGroupInterval.DateQuarter;
            quarter.Caption = oDict.GetWord("Quarter");
            pvgOlap.Fields.Add(quarter);

            var month = new DevExpress.Web.ASPxPivotGrid.PivotGridField("SCDate", PivotArea.FilterArea);
            month.GroupInterval = PivotGroupInterval.DateMonth;
            month.Caption = oDict.GetWord("month");
            month.ValueFormat.FormatType = FormatType.DateTime;
            month.ValueFormat.FormatString = "MM";
            pvgOlap.Fields.Add(month);
            #endregion

            #region Hided fields
            //pvgOlap.Fields["CurrencyCode"].Visible = false;
            #endregion

            pvgOlap.CollapseAllColumns();
            //pvgOlap.Fields[0].CollapseAll();
            pvgOlap.CollapseAllRows();
            #endregion
        }

        public void ExportToExcel()
        {
            String filename = String.Format("SalesTurnover_{0}", DateTime.Now.ToString("yyyyMMddhhmm"));

            DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
            options.ShowGridLines = true;
            options.TextExportMode = DevExpress.XtraPrinting.TextExportMode.Value;

            pvgExporter.ExportXlsxToResponse(filename, options);
        }

        void ansReceivablesOlap_ItemClick(object source, DevExpress.Web.MenuItemEventArgs e)
        {
            ExportToExcel();
        }

        protected void pvgOlap_FieldValueDisplayText(object sender, DevExpress.Web.ASPxPivotGrid.PivotFieldDisplayTextEventArgs e)
        {
            if (e.ValueType == DevExpress.XtraPivotGrid.PivotGridValueType.GrandTotal)
            {
                nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

                if (e.IsColumn)
                    e.DisplayText = e.DisplayText.ToLower() == "grand total" ? oDict.GetWord("grand_total") : e.DisplayText;
                else
                    e.DisplayText = e.DisplayText.ToLower() == "grand total" ? oDict.GetWord("grand_total") : e.DisplayText;
            }
        }
    }
}
