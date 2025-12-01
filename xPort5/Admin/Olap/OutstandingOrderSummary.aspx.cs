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
    public partial class OutstandingOrderSummary : System.Web.UI.Page
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
            this.olapOSOrder.Fields.Clear();

            #region Row Area
            //Customer Name
            PivotGridField customerFields = new PivotGridField("CustName", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            customerFields.Caption = "Customer Nam";
            customerFields.AreaIndex = 0;

            if (!olapOSOrder.Fields.Contains(customerFields))
            {
                olapOSOrder.Fields.Add(customerFields);
            }

            //Contract Number
            PivotGridField contractFields = new PivotGridField("SCNumber", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            contractFields.Caption = "Contract Number";
            contractFields.AreaIndex = 1;

            if (!olapOSOrder.Fields.Contains(contractFields))
            {
                olapOSOrder.Fields.Add(contractFields);
            }

            //Article Code
            PivotGridField articleCodeFields = new PivotGridField("ArticleCode", DevExpress.XtraPivotGrid.PivotArea.RowArea);
            articleCodeFields.Caption = "Article Code";
            articleCodeFields.AreaIndex = 2;

            if (!olapOSOrder.Fields.Contains(articleCodeFields))
            {
                olapOSOrder.Fields.Add(articleCodeFields);
            }
            #endregion

            #region Filter Area
            //Region
            PivotGridField regionFields=new PivotGridField("Region",DevExpress.XtraPivotGrid.PivotArea.FilterArea);
            regionFields.Caption="Region";

            if(!olapOSOrder.Fields.Contains(regionFields))
            {
                olapOSOrder.Fields.Add(regionFields);
            }
            #endregion

            #region Data Area
            PivotGridField backLogFields = new PivotGridField("BackLogAmt", DevExpress.XtraPivotGrid.PivotArea.DataArea);
            backLogFields.Caption = "Back Log";
            backLogFields.AreaIndex = 0;
            backLogFields.CellFormat.FormatString = "{0:n2}";
            backLogFields.CellFormat.FormatType = FormatType.Numeric;

            if (!olapOSOrder.Fields.Contains(backLogFields))
            {
                olapOSOrder.Fields.Add(backLogFields);
            }

            for (int i = 1; i <= 12; i++)
            {
                PivotGridField amtFields = new PivotGridField("Amt" + i.ToString(), DevExpress.XtraPivotGrid.PivotArea.DataArea);

                amtFields.Caption = DateTime.Now.AddMonths(i-1).ToString("MMM yyyy");
                amtFields.CellFormat.FormatString ="{0:n2}";
                amtFields.CellFormat.FormatType = FormatType.Numeric;
                amtFields.AreaIndex = i ;

                if (!olapOSOrder.Fields.Contains(amtFields))
                {
                    olapOSOrder.Fields.Add(amtFields);
                }
            }

            PivotGridField totalFields = new PivotGridField("Total", DevExpress.XtraPivotGrid.PivotArea.DataArea);
            totalFields.AreaIndex = 13;
            totalFields.CellFormat.FormatString = "{0:n2}";
            totalFields.CellFormat.FormatType = FormatType.Numeric;

            if (!olapOSOrder.Fields.Contains(totalFields))
            {
                olapOSOrder.Fields.Add(totalFields);
            }
            #endregion
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
            }
            if (custList.Length > 0)
            {
               this.CustomerId.Value = custList.ToString();
            }
        }

        private void Export(bool saveAs)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                /* 解決唔倒。暫停
                olapOSOrderExporter.OptionsPrint.PrintHeadersOnEveryPage = checkPrintHeadersOnEveryPage.Checked;
                olapOSOrderExporter.OptionsPrint.PrintFilterHeaders = checkPrintFilterHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapOSOrderExporter.OptionsPrint.PrintColumnHeaders = checkPrintColumnHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapOSOrderExporter.OptionsPrint.PrintRowHeaders = checkPrintRowHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
                olapOSOrderExporter.OptionsPrint.PrintDataHeaders = checkPrintDataHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;

                string contentType = string.Empty;
                string fileName = string.Empty;
                switch (listExportFormat.SelectedIndex)
                {
                    case 0:
                        contentType = "application/pdf";
                        fileName = "PivotGrid.pdf";
                        this.olapOSOrderExporter.ExportToPdf(stream);
                        break;
                    case 1:
                        contentType = "application/ms-excel";
                        fileName = "PivotGrid.xls";
                        this.olapOSOrderExporter.ExportToXls(stream);
                        break;
                    case 2:
                        contentType = "text/enriched";
                        fileName = "PivotGrid.rtf";
                        this.olapOSOrderExporter.ExportToRtf(stream);
                        break;
                    case 3:
                        contentType = "text/plain";
                        fileName = "PivotGrid.txt";
                        this.olapOSOrderExporter.ExportToText(stream);
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
