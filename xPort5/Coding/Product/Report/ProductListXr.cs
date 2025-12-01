using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;

using DevExpress.XtraReports.UI;
using Gizmox.WebGUI.Forms;

using nxStudio.BaseClass;
using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Coding.Product.Report
{
    public partial class ProductListXr : DevExpress.XtraReports.UI.XtraReport
    {
        private bool odd = true;

        public ProductListXr()
        {
            InitializeComponent();

            #region Set Captions
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            lblProductCode.Text = oDict.GetWord("product_code");
            lblDescription.Text = oDict.GetWord("product_description");
            lblBarcode.Text = oDict.GetWord("barcode");
            lblCategory.Text = oDict.GetWord("category");
            lblOrigin.Text = oDict.GetWord("origin");
            lblColor.Text = oDict.GetWord("color");
            lblRemarks.Text = oDict.GetWord("remarks");
            lblPicture.Text = oDict.GetWord("product_picture");

            lblPacking.Text = oDict.GetWord("package");
            lblUnit.Text = oDict.GetWord("unit");
            lblInnerBox.Text = oDict.GetWord("inner_box");
            lblOuterBox.Text = oDict.GetWord("outer_box");
            lblLength.Text = oDict.GetWord("length");
            lblWidth.Text = oDict.GetWord("width");
            lblHeight.Text = oDict.GetWord("height");
            lblCUFT.Text = oDict.GetWord("cuft");
            lblGross.Text = oDict.GetWord("gross_weight");
            lblNet.Text = oDict.GetWord("net_weight");
            lbltotalQty.Text = oDict.GetWord("total_qty");
            lblContainer.Text = oDict.GetWord("container");

            lblSupplier.Text = oDict.GetWord("supplier");
            lblSuppRef.Text = oDict.GetWord("ref_number");
            lblFCLCost.Text = oDict.GetWord("fcl_cost");
            lblLCLCost.Text = oDict.GetWord("lcl_cost");
            lblUnitCost.Text = oDict.GetWord("unit_cost");
            lblCurrency.Text = oDict.GetWord("currency");

            lblPrintedOn.Text = oDict.GetWordWithColon("printedon");
            lblPrintedBy.Text = oDict.GetWordWithColon("printedby");
            lblPage.Text = oDict.GetWordWithColon("page");
            #endregion
        }

        private void ProductListXr_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //this.txtPrintedOn.Text = DateTime.Now.ToString("yyyy-MM-dd HH:MM");
            Staff user = Staff.Load(xPort5.Common.Config.CurrentUserId);
            this.txtPrintedBy.Text = user.Alias;
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            #region toggle background color
            if (odd)
            {
                this.Detail.BackColor = Color.WhiteSmoke;
            }
            else
            {
                this.Detail.BackColor = Color.Transparent;
            }
            #endregion

            string productId = GetCurrentColumnValue("ArticleId").ToString();

            #region bind subreport General
            ProductListXrGeneral general = new ProductListXrGeneral();
            DataTable dtGeneral = xPort5.Controls.Reporting.XR.DataSource.ProductListGeneral(productId);
            general.DataSource = dtGeneral;
            general.ToggleBackColor = odd;
            this.srpGeneral.ReportSource = general;
            #endregion

            #region bind subreport Packing
            ProductListXrPacking packing = new ProductListXrPacking();
            DataTable dtPacking = xPort5.Controls.Reporting.XR.DataSource.ProductListPacking(productId);
            packing.DataSource = dtPacking;
            packing.ToggleBackColor = odd;
            this.srpPacking.ReportSource = packing;
            #endregion

            #region bind subreport Supplier
            ProductListXrSupplier supplier = new ProductListXrSupplier();
            DataTable dtSupplier = xPort5.Controls.Reporting.XR.DataSource.ProductListSupplier(productId);
            supplier.DataSource = dtSupplier;
            supplier.ToggleBackColor = odd;
            this.srpSupplier.ReportSource = supplier;
            #endregion

            #region toggle background color
            odd = !odd;
            #endregion
        }

    }
}
