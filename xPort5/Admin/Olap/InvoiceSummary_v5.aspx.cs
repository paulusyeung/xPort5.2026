using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    public partial class InvoiceSummary_v5 : System.Web.UI.Page
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
                    SqlParameter[] param = new SqlParameter[1];

                    param[0] = new SqlParameter("@CustomerId_Array", SqlDbType.NVarChar);
                    param[0].Direction = ParameterDirection.Input;
                    param[0].Value = custList.ToString();

                    dataSource = SqlHelper.Default.ExecuteDataSet("olap_InvoiceSummary", param);
                    #endregion
                }
            }
        }

        void ansReceivablesOlap_ItemClick(object source, DevExpress.Web.MenuItemEventArgs e)
        {
            ExportToExcel();
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
            pvgOlap.Fields["INNumber"].Area = PivotArea.RowArea;
            pvgOlap.Fields["INNumber"].Caption = oDict.GetWord("invoice_no");
            #endregion

            #region Column Area
            var year = new DevExpress.Web.ASPxPivotGrid.PivotGridField("INDate", PivotArea.FilterArea);
            year.GroupInterval = PivotGroupInterval.DateYear;
            year.Caption = oDict.GetWord("year");
            year.AreaIndex = 0;

            pvgOlap.Fields.Add(year);
            #endregion

            #region Data Area
            pvgOlap.Fields["BackLogAmt"].Area = PivotArea.DataArea;
            pvgOlap.Fields["BackLogAmt"].Caption = "Back Log";
            pvgOlap.Fields["BackLogAmt"].AreaIndex = 0;
            pvgOlap.Fields["BackLogAmt"].ValueFormat.FormatString = "{0:n2}";
            pvgOlap.Fields["BackLogAmt"].ValueFormat.FormatType = FormatType.Numeric;
            pvgOlap.Fields["BackLogAmt"].ExportBestFit = false;
            pvgOlap.Fields["BackLogAmt"].Width = 120;

            for (int i = 1; i <= 12; i++)
            {
                var tag = "Amt" + i.ToString();

                pvgOlap.Fields[tag].Area = PivotArea.DataArea;
                pvgOlap.Fields[tag].Caption = DateTime.Now.AddMonths(i - 1).ToString("MMM yyyy");
                pvgOlap.Fields[tag].ValueFormat.FormatString = "{0:n2}";
                pvgOlap.Fields[tag].ValueFormat.FormatType = FormatType.Numeric;
                pvgOlap.Fields[tag].AreaIndex = i;
                pvgOlap.Fields[tag].ExportBestFit = false;
                pvgOlap.Fields[tag].Width = 120;
            }

            pvgOlap.Fields["Total"].Area = PivotArea.DataArea;
            pvgOlap.Fields["Total"].AreaIndex = 13;
            pvgOlap.Fields["Total"].ValueFormat.FormatString = "{0:n2}";
            pvgOlap.Fields["Total"].ValueFormat.FormatType = FormatType.Numeric;
            pvgOlap.Fields["Total"].ExportBestFit = false;
            pvgOlap.Fields["Total"].Width = 120;
            #endregion

            #region Filter Area
            pvgOlap.Fields["Region"].Area = PivotArea.FilterArea;
            pvgOlap.Fields["Region"].Caption = oDict.GetWord("region");
            #endregion

            #region Ignore these fields
            pvgOlap.Fields["INQty"].Visible = false;
            pvgOlap.Fields["UnitAmount"].Visible = false;
            pvgOlap.Fields["ExtAmount"].Visible = false;
            pvgOlap.Fields["ExtHKDAmount"].Visible = false;
            pvgOlap.Fields["ExchangeRate"].Visible = false;
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
