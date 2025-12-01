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
    public partial class SalesContract : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.divOptions.Visible = false;
                InitialValues();
            }
        }

        private void InitialValues()
        {
            string[] period = xPort5.Controls.Utility.OlapAdmin.DatePeriod.Split(',');
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
                if (custList.Length > 0)
                {
                    CustomerId.Value = custList.ToString();
                    FromDate.Value = period[0] + " 00:00:00";
                    ToDate.Value = period[1] + " 23:59:59";
                }
            }
        }

        private void Export(bool saveAs)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                /* 解決唔倒。暫停
                olapSalesContractExporter.OptionsPrint.PrintHeadersOnEveryPage = checkPrintHeadersOnEveryPage.Checked;
                olapSalesContractExporter.OptionsPrint.PrintFilterHeaders = checkPrintFilterHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapSalesContractExporter.OptionsPrint.PrintColumnHeaders = checkPrintColumnHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapSalesContractExporter.OptionsPrint.PrintRowHeaders = checkPrintRowHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapSalesContractExporter.OptionsPrint.PrintDataHeaders = checkPrintDataHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;

                string contentType = string.Empty;
                string fileName = string.Empty;
                switch (listExportFormat.SelectedIndex)
                {
                    case 0:
                        contentType = "application/pdf";
                        fileName = "PivotGrid.pdf";
                        this.olapSalesContractExporter.ExportToPdf(stream);
                        break;
                    case 1:
                        contentType = "application/ms-excel";
                        fileName = "PivotGrid.xls";
                        this.olapSalesContractExporter.ExportToXls(stream);
                        break;
                    case 2:
                        contentType = "text/enriched";
                        fileName = "PivotGrid.rtf";
                        this.olapSalesContractExporter.ExportToRtf(stream);
                        break;
                    case 3:
                        contentType = "text/plain";
                        fileName = "PivotGrid.txt";
                        this.olapSalesContractExporter.ExportToText(stream);
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

        protected void olapSalesContract_PreRender(object sender, EventArgs e)
        {
            foreach (DevExpress.Web.ASPxPivotGrid.PivotGridField field in olapSalesContract.Fields)
            {
                field.CollapseAll();
            }
        }
    }
}
