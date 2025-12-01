#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using xPort5.EF6;
using xPort5.Common;
using Gizmox.WebGUI.Common.Resources;
using System.Data.SqlClient;
using System.IO;

#endregion

namespace xPort5.Controls.Product
{
    public partial class ImageManager_V2 : Form
    {
        public Guid ProductId { get; set; }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        public ImageManager_V2()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();

            AddProductImageControl();
        }

        private void AddProductImageControl()
        {
            this.imagePane.Controls.Clear();

            ProductImage_AllInOne prodImg = new ProductImage_AllInOne();
            prodImg.ThumbnailSize = xPort5.Controls.Utility.ImagePanel.ThumbnailSize.Medium;
            prodImg.ProductId = this.ProductId;
            prodImg.LoadImagesList();
            prodImg.Dock = DockStyle.Fill;

            this.imagePane.Controls.Add(prodImg);
        }

        #region Set Captions

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = oDict.GetWord("product_image_manager");
        }

        void cmdImageList_MenuClick(object sender, MenuItemEventArgs e)
        {
            if (e.MenuItem.Tag != null)
            {
                //switch (((string)e.MenuItem.Tag).ToLower())
                //{
                //    case "small":
                //        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Small, false);
                //        break;
                //    case "medium":
                //        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                //        break;
                //    case "large":
                //        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Large, false);
                //        break;
                //    case "details":
                //        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.XLarge, false);
                //        break;
                //}
            }
        }

        #endregion
    }
}
