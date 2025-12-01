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
    public partial class InvoiceSummaryByMonth : System.Web.UI.Page
    {
        string[] period = xPort5.Controls.Utility.OlapAdmin.DatePeriod.Split(',');

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.divOptions.Visible = false;
                InitialValues();
                BuildFields();
            }
        }

        private void InitialValues()
        {
            StringBuilder custList = new StringBuilder();

            if(VWGContext.Current.Session["CustomerList"] is HashSet<Guid>)
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
                ToDate.Value = period[1] + " 23:59:59";
            }
        }

        /// <summary>
        /// Builds the Field.
        /// </summary>
        private void BuildFields()
        {
            this.olapInvoiceByMonth.Fields.Clear();

            #region Row Area
            //Customer Name
            PivotGridField custFields = new PivotGridField("CustName", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            custFields.Caption = "Customer Name";
            custFields.AreaIndex = 0;

            if (!olapInvoiceByMonth.Fields.Contains(custFields))
            {
                olapInvoiceByMonth.Fields.Add(custFields);
            }

            //Contract Number
            PivotGridField contractFields = new PivotGridField("INNumber", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            contractFields.Caption = "Contract Number";
            contractFields.AreaIndex = 1;

            if (!olapInvoiceByMonth.Fields.Contains(contractFields))
            {
                olapInvoiceByMonth.Fields.Add(contractFields);
            }
            #endregion

            #region Filter Area
            PivotGridField regionFields = new PivotGridField("Region", DevExpress.XtraPivotGrid.PivotArea.FilterArea);
            regionFields.Caption = "Region";
            regionFields.AreaIndex = 0;

            if (!olapInvoiceByMonth.Fields.Contains(regionFields))
            {
                olapInvoiceByMonth.Fields.Add(regionFields);
            }
            #endregion

            #region Data Area
            PivotGridField backLogFields = new PivotGridField("BackLogAmt", DevExpress.XtraPivotGrid.PivotArea.DataArea);
            backLogFields.Caption = "Back Log";
            backLogFields.AreaIndex = 0;
            backLogFields.CellFormat.FormatString = "{0:n2}";
            backLogFields.CellFormat.FormatType = FormatType.Numeric;

            if (!olapInvoiceByMonth.Fields.Contains(backLogFields))
            {
                olapInvoiceByMonth.Fields.Add(backLogFields);
            }

            for (int i = 1; i <= 12; i++)
            {
                PivotGridField amtFields = new PivotGridField("Amt" + i.ToString(), DevExpress.XtraPivotGrid.PivotArea.DataArea);
                amtFields.Caption = DateTime.Parse(period[1]).AddMonths(i - 12).ToString("MMM yyyy");
                amtFields.CellFormat.FormatString = "{0:n2}";
                amtFields.CellFormat.FormatType = FormatType.Numeric;
                amtFields.AreaIndex = i;

                if (!olapInvoiceByMonth.Fields.Contains(amtFields))
                {
                    olapInvoiceByMonth.Fields.Add(amtFields);
                }
            }

            PivotGridField totalFields = new PivotGridField("Total", DevExpress.XtraPivotGrid.PivotArea.DataArea);
            totalFields.Caption = "Total";
            totalFields.AreaIndex = 13;
            totalFields.CellFormat.FormatString = "{0:n2}";
            totalFields.CellFormat.FormatType = FormatType.Numeric;

            if (!olapInvoiceByMonth.Fields.Contains(totalFields))
            {
                olapInvoiceByMonth.Fields.Add(totalFields);
            }
            #endregion
        }

        private void Export(bool saveAs)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                /* 解決唔倒。暫停
                olapInvoiceByMonthExporter.OptionsPrint.PrintHeadersOnEveryPage = checkPrintHeadersOnEveryPage.Checked;
                olapInvoiceByMonthExporter.OptionsPrint.PrintFilterHeaders = checkPrintFilterHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapInvoiceByMonthExporter.OptionsPrint.PrintColumnHeaders = checkPrintColumnHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapInvoiceByMonthExporter.OptionsPrint.PrintRowHeaders = checkPrintRowHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapInvoiceByMonthExporter.OptionsPrint.PrintDataHeaders = checkPrintDataHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;

                string contentType = string.Empty;
                string fileName = string.Empty;
                switch (listExportFormat.SelectedIndex)
                {
                    case 0:
                        contentType = "application/pdf";
                        fileName = "PivotGrid.pdf";
                        this.olapInvoiceByMonthExporter.ExportToPdf(stream);
                        break;
                    case 1:
                        contentType = "application/ms-excel";
                        fileName = "PivotGrid.xls";
                        this.olapInvoiceByMonthExporter.ExportToXls(stream);
                        break;
                    case 2:
                        contentType = "text/enriched";
                        fileName = "PivotGrid.rtf";
                        this.olapInvoiceByMonthExporter.ExportToRtf(stream);
                        break;
                    case 3:
                        contentType = "text/plain";
                        fileName = "PivotGrid.txt";
                        this.olapInvoiceByMonthExporter.ExportToText(stream);
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
