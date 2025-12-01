using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using DevExpress.XtraReports.UI;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Coding.Product.Report
{
    public partial class ProductListXrGeneral : DevExpress.XtraReports.UI.XtraReport
    {
        private bool _ToggleBackColor = false;

        public bool ToggleBackColor
        {
            set
            {
                _ToggleBackColor = value;
            }
        }

        public ProductListXrGeneral()
        {
            InitializeComponent();

            #region item databindings
            this.txtSKU.DataBindings.Add("Text", DataSource, "SKU");
            this.txtProductCode.DataBindings.Add("Text", DataSource, "ArticleCode");
            this.txtColor.DataBindings.Add("Text", DataSource, "ColorPattern");
            this.txtDescription.DataBindings.Add("Text", DataSource, "ArticleName");
            this.txtBarcode.DataBindings.Add("Text", DataSource, "Barcode");
            this.txtCategory.DataBindings.Add("Text", DataSource, "CategoryName");
            this.txtOrigin.DataBindings.Add("Text", DataSource, "OriginName");
            this.txtRemarks.DataBindings.Add("Text", DataSource, "Remarks");
            #endregion
        }

        private void ProductListXrGeneral_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            #region toggle background color
            if (_ToggleBackColor)
            {
//                this.paneDetail.BackColor = Color.WhiteSmoke;
            }
            else
            {
                this.paneDetail.BackColor = Color.Transparent;
            }
            #endregion

            #region set product picture
            string prodId = GetCurrentColumnValue("ArticleId").ToString();
            Article prod = Article.Load(new Guid(prodId));
            if (prod != null)
            {
                string pictureFile = xPort5.Controls.Utility.Resources.PictureFilePath(prod.ArticleId, xPort5.Controls.Utility.Product.KeyPicture(prod.ArticleId));
                if (!(File.Exists(pictureFile)))
                {
                    pictureFile = Path.Combine(VWGContext.Current.Config.GetDirectory("Images"), "no_photo.jpg");
                }
//                this.picProduct.ImageUrl = pictureFile;
                this.picProduct.Image = xPort5.Controls.Utility.Resources.GetPicture(pictureFile, this.picProduct.Width, this.picProduct.Height, true);
            }
            #endregion
        }

    }
}
