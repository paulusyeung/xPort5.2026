using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gizmox.WebGUI.Forms;
using System.Drawing;
using xPort5.EF6;
using xPort5.Common;
using System.IO;

namespace xPort5.Controls.Product
{
    public class ImagePanel : Panel
    {
        public new event EventHandler DoubleClick;
        public event EventHandler KeyPictureBoxClick;

        public Size ImageSize { get; set; }
        public Guid ProductId { get; set; }
        public bool ShowDetailInfo { get; set; }
        public Guid OrderItemId { get; set; }
        public Guid ResourceId { get; set; }
        public string FileName { get; set; }
        public bool Selection { get; set; }
        public bool KeyPicture { get; set; }

        private ToolTip toolTip1;

        public ImagePanel(Size imageSize, Guid productId, bool inDetail, Guid orderItemId) :
            this(imageSize, productId, inDetail, orderItemId, System.Guid.Empty, string.Empty, false, false)
        {
        }

        public ImagePanel(Size imageSize, Guid productId, bool inDetail, Guid orderItemId, Guid resourceId, string fileName, bool selection, bool keyPicture)
        {
            this.ImageSize = imageSize;
            this.ProductId = productId;
            this.ShowDetailInfo = inDetail;
            this.OrderItemId = orderItemId;
            this.ResourceId = resourceId;
            this.FileName = fileName;
            this.Selection = selection;
            this.KeyPicture = keyPicture;

            this.DrawPanel();
        }

        private void DrawPanel()
        {
            this.Size = new Size(this.ImageSize.Width + 20, this.ImageSize.Height + 40);

            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);
            this.toolTip1 = new ToolTip();

            if (this.KeyPicture)
            {
                this.Size = new Size(this.Size.Width, this.Size.Height + 20);
            }

            //this.Margin = new Padding(3);
            //this.BorderColor = Color.Gray;
            //this.BorderStyle = BorderStyle.Clear;
            //this.BorderWidth = 1;

            Article product = Article.Load(this.ProductId);

            #region CheckBox for selection

            CheckBox chkImage = new CheckBox();
            chkImage.Location = new Point(10, 7);
            chkImage.Size = new Size(20, 20);
            chkImage.CheckAlign = ContentAlignment.MiddleLeft;
            chkImage.Visible = this.Selection;
            chkImage.Tag = "SelectedImage";
            this.toolTip1.SetToolTip(chkImage, oDict.GetWord("msg_click_to_select"));
            this.Controls.Add(chkImage);

            #endregion

            #region Add Product Image

            xPort5.Controls.ProductImage prodImage = new xPort5.Controls.ProductImage();
            prodImage.Name = this.OrderItemId.ToString();
            prodImage.ImageName = string.IsNullOrEmpty(this.FileName) ? xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, xPort5.Controls.Utility.Product.KeyPicture(this.ProductId)) : xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, this.FileName);
            prodImage.BorderStyle = BorderStyle.FixedSingle;
            prodImage.BorderWidth = 1;
            prodImage.BorderColor = Color.Gainsboro;
            prodImage.Cursor = Cursors.Hand;
            prodImage.Location = new Point(10, 10);
            prodImage.Size = this.ImageSize;
            prodImage.SizeMode = PictureBoxSizeMode.CenterImage;
            prodImage.Tag = product.ArticleCode;
            prodImage.ProductId = product.ArticleId;
            prodImage.Click += new EventHandler(prodImage_Click);
            prodImage.DoubleClick += new EventHandler(prodImage_DoubleClick);
            this.toolTip1.SetToolTip(prodImage, oDict.GetWord("msg_click_to_select"));

            this.Controls.Add(prodImage);

            #endregion

            if (!(this.ShowDetailInfo))
            {
                #region Add Product Code
                Label prodCode = new Label();
                prodCode.Location = new Point(10, this.ImageSize.Height + 10);
                prodCode.Size = new Size(this.ImageSize.Width, 20);
                prodCode.TextAlign = ContentAlignment.MiddleCenter;
                prodCode.Text = string.IsNullOrEmpty(this.FileName) ? product.ArticleCode : Path.GetFileNameWithoutExtension(this.FileName);
                this.Controls.Add(prodCode);
                #endregion

                #region CheckBox - Primary Picture

                if (this.KeyPicture)
                {
                    List<Guid> idList = new List<Guid>();
                    idList.Add(this.ProductId);
                    idList.Add(this.ResourceId);

                    CheckBox chkPrimaryImage = new CheckBox();
                    chkPrimaryImage.Location = new Point(10, this.ImageSize.Height + 30);
                    chkPrimaryImage.Size = new Size(this.ImageSize.Width, 20);
                    chkPrimaryImage.Text = oDict.GetWord("Primary_picture");
                    chkPrimaryImage.CheckAlign = ContentAlignment.MiddleLeft;
                    chkPrimaryImage.Tag = idList;
                    this.toolTip1.SetToolTip(chkPrimaryImage, oDict.GetWord("Primary_picture"));

                    if (Utility.Product.HasKeyPicture(this.ProductId))
                    {
                        bool isKeyPicture = Utility.Product.IsKeyPicture(this.ProductId, this.ResourceId);
                        chkPrimaryImage.Checked = isKeyPicture;
                    }

                    chkPrimaryImage.Click += new EventHandler(chkPrimaryImage_Click);
                    chkPrimaryImage.Visible = this.KeyPicture;
                    this.Controls.Add(chkPrimaryImage);
                }

                #endregion
            }
            else
            {
                #region Add Product Code, Description, Category, Color, Origin, Remarks

                #region Add Product Code
                Label prodCode = new Label();
                prodCode.Location = new Point(20, this.ImageSize.Height + 10);
                prodCode.Size = new Size(this.ImageSize.Width, 20);
                prodCode.TextAlign = ContentAlignment.TopLeft;
                prodCode.Text = product.ArticleCode;
                this.Controls.Add(prodCode);
                #endregion

                #region Add Description
                TextBox description = new TextBox();
                //Label description = new Label();
                description.Location = new Point(20, this.ImageSize.Height + 30);
                description.Size = new Size(this.ImageSize.Width - 20, 60);
                description.Multiline = true;
                description.ScrollBars = ScrollBars.Vertical;
                description.BorderColor = Color.Gainsboro;
                description.BorderStyle = BorderStyle.FixedSingle;
                description.BorderWidth = 1;
                //description.AutoSize = true;
                //description.MaximumSize = new Size(this.ImageSize.Width, 90);
                //description.TextAlign = ContentAlignment.TopLeft;
                description.Text = product.ArticleName;
                this.Controls.Add(description);
                #endregion

                #region Add Category
                T_Category cat = T_Category.Load(product.CategoryId);
                Label category = new Label();
                category.Location = new Point(20, this.ImageSize.Height + 100);
                category.Size = new Size(this.ImageSize.Width, 20);
                category.TextAlign = ContentAlignment.TopLeft;
                category.Text = cat.CategoryName;
                this.Controls.Add(category);
                #endregion

                #region Add Color
                T_AgeGrading color = T_AgeGrading.Load(product.AgeGradingId);
                Label lblColor = new Label();
                lblColor.Location = new Point(20, this.ImageSize.Height + 120);
                lblColor.Size = new Size(this.ImageSize.Width, 20);
                lblColor.TextAlign = ContentAlignment.TopLeft;
                lblColor.Text = color.AgeGradingName;
                this.Controls.Add(lblColor);
                #endregion

                #region Add Origin
                T_Origin origin = T_Origin.Load(product.OriginId);
                Label lblOrigin = new Label();
                lblOrigin.Location = new Point(20, this.ImageSize.Height + 140);
                lblOrigin.Size = new Size(this.ImageSize.Width, 20);
                lblOrigin.TextAlign = ContentAlignment.TopLeft;
                lblOrigin.Text = origin.OriginName;
                this.Controls.Add(lblOrigin);
                #endregion

                #endregion

                this.Height = lblOrigin.Location.Y + 20;
            }
        }

        void chkPrimaryImage_Click(object sender, EventArgs e)
        {
            if (KeyPictureBoxClick != null)
                KeyPictureBoxClick(sender, e);
        }

        void prodImage_Click(object sender, EventArgs e)
        {
            ProductImage productImage = sender as ProductImage;
            if (productImage != null)
            {
                for (int i = 0; i < productImage.Parent.Controls.Count; i++)
                {
                    Control ctrl = productImage.Parent.Controls[i];
                    if (ctrl is CheckBox)
                    {
                        CheckBox chkCtrl = ctrl as CheckBox;
                        if (chkCtrl != null)
                        {
                            if (chkCtrl.Tag != null && chkCtrl.Tag.ToString().ToLower() == "selectedimage")
                            {
                                chkCtrl.Checked = !chkCtrl.Checked;
                            }
                        }
                    }
                }
            }
        }

        void prodImage_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(sender, e);
        }
    }
}
