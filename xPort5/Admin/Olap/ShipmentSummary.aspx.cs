using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gizmox.WebGUI.Forms;
using DevExpress.Web.ASPxPivotGrid;
using System.IO;
using DevExpress.Utils;
using System.Data;
using System.Text;
using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Admin.Olap
{
    public partial class ShipmentSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.divOptions.Visible = false;
                InitialValues();
                BuildFields();
            }
        }

        /// <summary>
        /// Builds the Field.
        /// </summary>
        private void BuildFields()
        {
            this.olapShipmentSummary.Fields.Clear();

            #region Row Area
            //Customer Name
            PivotGridField customerFields = new PivotGridField("CustName", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            customerFields.Caption = "Customer Name";
            customerFields.AreaIndex = 0;
            customerFields.MinWidth = 40;
            customerFields.Width = 80;

            if (!olapShipmentSummary.Fields.Contains(customerFields))
            {
                olapShipmentSummary.Fields.Add(customerFields);
            }

            //Contract Number
            PivotGridField contractFields = new PivotGridField("SCNumber", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            contractFields.Caption = "Contract Number";
            contractFields.AreaIndex = 1;
            contractFields.MinWidth = 40;
            contractFields.Width = 80;

            if (!olapShipmentSummary.Fields.Contains(contractFields))
            {
                olapShipmentSummary.Fields.Add(contractFields);
            }

            //Article Code
            PivotGridField articleCodeFields = new PivotGridField("ArticleCode", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            articleCodeFields.Caption = "Article Code";
            articleCodeFields.AreaIndex = 2;
            articleCodeFields.MinWidth = 20;
            articleCodeFields.Width = 60;

            if (!olapShipmentSummary.Fields.Contains(articleCodeFields))
            {
                olapShipmentSummary.Fields.Add(articleCodeFields);
            }
            #endregion

            #region Filter Area
            //Region
            PivotGridField regionFields = new PivotGridField("Region", DevExpress.XtraPivotGrid.PivotArea.FilterArea);
            regionFields.Caption = "Region";

            if (!olapShipmentSummary.Fields.Contains(regionFields))
            {
                olapShipmentSummary.Fields.Add(regionFields);
            }
            #endregion

            #region Data Area
            PivotGridField backLogFields = new PivotGridField("BackLogAmt", DevExpress.XtraPivotGrid.PivotArea.DataArea);
            backLogFields.Caption = "Back Log";
            backLogFields.AreaIndex = 0;
            backLogFields.CellFormat.FormatString = "{0:C}";
            backLogFields.CellFormat.FormatType = FormatType.Numeric;

            if (!olapShipmentSummary.Fields.Contains(backLogFields))
            {
                olapShipmentSummary.Fields.Add(backLogFields);
            }

            for (int i = 1; i <= 12; i++)
            {
                PivotGridField amtFields = new PivotGridField("Amt" + i.ToString(), DevExpress.XtraPivotGrid.PivotArea.DataArea);

                amtFields.Caption = DateTime.Now.AddMonths(i - 1).ToString("MMM yyyy");
                amtFields.CellFormat.FormatString = "{0:C}";
                amtFields.CellFormat.FormatType = FormatType.Numeric;
                amtFields.AreaIndex = i;

                if (!olapShipmentSummary.Fields.Contains(amtFields))
                {
                    olapShipmentSummary.Fields.Add(amtFields);
                }
            }

            PivotGridField totalFields = new PivotGridField("Total", DevExpress.XtraPivotGrid.PivotArea.DataArea);
            totalFields.AreaIndex = 13;
            totalFields.CellFormat.FormatString = "{0:C}";
            totalFields.CellFormat.FormatType = FormatType.Numeric;

            if (!olapShipmentSummary.Fields.Contains(totalFields))
            {
                olapShipmentSummary.Fields.Add(totalFields);
            }
            #endregion
        }

        private void InitialValues()
        {
            string[] period = xPort5.Controls.Utility.OlapAdmin.DatePeriod.Split(',');

            if (period.Length > 0)
            {
                string currency = xPort5.Controls.Utility.OlapAdmin.SelectedCurrency;

                StringBuilder custList = new StringBuilder();

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
                if (custList.Length > 0)
                {
                    CustomerId.Value = custList.ToString();
                    FromDate.Value = period[0] + " 00:00:00";
                    Currency.Value = currency;
                }
            }
        }

        private void Export(bool saveAs)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                /* 解決唔倒。暫停
                olapShipmentSummaryExporter.OptionsPrint.PrintHeadersOnEveryPage = checkPrintHeadersOnEveryPage.Checked;
                olapShipmentSummaryExporter.OptionsPrint.PrintFilterHeaders = checkPrintFilterHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapShipmentSummaryExporter.OptionsPrint.PrintColumnHeaders = checkPrintColumnHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapShipmentSummaryExporter.OptionsPrint.PrintRowHeaders = checkPrintRowHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapShipmentSummaryExporter.OptionsPrint.PrintDataHeaders = checkPrintDataHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;

                string contentType = string.Empty;
                string fileName = string.Empty;
                switch (listExportFormat.SelectedIndex)
                {
                    case 0:
                        contentType = "application/pdf";
                        fileName = "PivotGrid.pdf";
                        this.olapShipmentSummaryExporter.ExportToPdf(stream);
                        break;
                    case 1:
                        contentType = "application/ms-excel";
                        fileName = "PivotGrid.xls";
                        this.olapShipmentSummaryExporter.ExportToXls(stream);
                        break;
                    case 2:
                        contentType = "text/enriched";
                        fileName = "PivotGrid.rtf";
                        this.olapShipmentSummaryExporter.ExportToRtf(stream);
                        break;
                    case 3:
                        contentType = "text/plain";
                        fileName = "PivotGrid.txt";
                        this.olapShipmentSummaryExporter.ExportToText(stream);
                        break;
                }

                byte[] buffer = stream.GetBuffer();

                string disposition = saveAs ? "attachment" : "inline";
                Response.Clear();
                Response.Buffer = false;
                Response.AppendHeader("Content-Type", contentType);
                Response.AppendHeader("Content-Transfer-Encoding", "binary");
                Response.AppendHeader("Content-Disposition", disposition + ";filename=" + fileName);
                Response.BinaryWrite(buffer);

                Response.End();
                */
            }
        }

        protected void btnSaveAs_Click(object sender, ImageClickEventArgs e)
        {
            this.Export(true);
        }

        protected void btOpen_Click(object sender, ImageClickEventArgs e)
        {
            this.Export(false);
        }
    }
}
