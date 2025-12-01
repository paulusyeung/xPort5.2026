#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using xPort5.EF6;
using xPort5.Common;
using System.IO;
using xPort5.Controls.Product;

#endregion

namespace xPort5.Controls
{
    public partial class ProductImage_AllInOne : UserControl
    {
        public Guid ProductId { get; set; }
        public Guid ResourceId { get; set; }
        public string CurrentImageFile { get; set; }
        public xPort5.Controls.Utility.ImagePanel.ThumbnailSize ThumbnailSize { get; set; }

        private Size currentThumbnailSize;
        private Point currentImageLocation = new Point(0, 0);
        private string _MsgCaption = string.Empty;
        private string _DeleteMsg = string.Empty;
        private string _ErrCaption = string.Empty;
        private string _ErrMsg = string.Empty;
        private string _ErrCannotBeDeleted = string.Empty;
        private string _GDocFileName = "{0} {1} {2}"; // ArticleCode + " " + FileType + " " + FileName

        public ProductImage_AllInOne()
        {
            InitializeComponent();

            if (!this.DesignMode)
            {
                SetCaptions();

                this.productImage.SizeMode = PictureBoxSizeMode.CenterImage;
                ReallocateImagePositionAndSize(xPort5.Controls.Utility.Resources.ImageSize.Medium);
            }
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            _MsgCaption = oDict.GetWord("message");
            _ErrCaption = oDict.GetWord("warning");
            _ErrMsg = oDict.GetWord("err_cannot_be_blank");
            _DeleteMsg = oDict.GetWord("delete_picture");

            _ErrCannotBeDeleted = string.Format(oDict.GetWord("err_cannot_be_deleted"), oDict.GetWord("primary_picture"));
        }

        public void LoadImagesList()
        {
            SetThumbnailSize();
            BindImageList(this.currentThumbnailSize, false);
        }

        #region Image List

        private void SetThumbnailSize()
        {
            switch (this.ThumbnailSize)
            {
                case xPort5.Controls.Utility.ImagePanel.ThumbnailSize.Small:
                    this.currentThumbnailSize = new Size(24, 24);
                    break;
                case xPort5.Controls.Utility.ImagePanel.ThumbnailSize.Medium:
                default:
                    this.currentThumbnailSize = new Size(32, 32);
                    break;
                case xPort5.Controls.Utility.ImagePanel.ThumbnailSize.Large:
                    this.currentThumbnailSize = new Size(64, 64);
                    break;
            }

            ResetFlowLayoutPanelHeight();
        }

        private void ResetFlowLayoutPanelHeight()
        {
            int scrollBarHeight = 10;
            int height = this.currentThumbnailSize.Height;

            this.bottomPane.Height = scrollBarHeight + height;

            this.bottomPane.Update();
        }

        private void PrepareGWebAlbum()
        {
            if (this.ProductId != null)
            {
                string sql = "Keyword = '" + Utility.Product.ProductCode(this.ProductId) + "'";
                string[] orderBy = new string[] { "OriginalFileName" };
                ResourcesCollection rescList = Resources.LoadCollection(sql, orderBy, true);
                foreach (Resources resc in rescList)
                {
                    string fileName = resc.OriginalFileName;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (!File.Exists(xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, fileName)))
                        {
                            //string gdocFile = string.Format(_GDocFileName, Utility.Product.ProductCode(this.ProductId), Utility.Product.GetCategoryName(this.ProductId), attached.OriginalFileName);

                            //// Download GDocs to Cache folder
                            //GData.GDocs.DownloadFile(gdocFile,
                            //    Utility.JobOrder.FileFilePath(this.OrderId, this.AttachType.ToString("d"), string.Empty));

                            //gdocFile = Utility.JobOrder.FileFilePath(this.OrderId, this.AttachType.ToString("d"), gdocFile);
                            //if (File.Exists(gdocFile))
                            //{
                            //    File.Copy(gdocFile, Utility.JobOrder.FileFilePath(this.OrderId, this.AttachType.ToString("d"), fileName));

                            //    File.Delete(gdocFile);
                            //}
                        }
                    }
                }
            }
        }

        private void BindImageList(Size imageSize, bool inDetail)
        {
            this.thumbnailsPane.Controls.Clear();

            if (this.ProductId != null)
            {
                string sql = "Keyword = '" + Utility.Product.ProductCode(this.ProductId) + "'";
                string[] orderBy = new string[] { "OriginalFileName" };
                ResourcesCollection rescList = Resources.LoadCollection(sql, orderBy, true);
                foreach (Resources resc in rescList)
                {
                    if (!string.IsNullOrEmpty(resc.OriginalFileName))
                    {
                        Panel imgPane = this.DrawPanel(resc.ResourcesId, resc.OriginalFileName);

                        this.thumbnailsPane.Controls.Add(imgPane);

                        if (Utility.Product.IsKeyPicture(this.ProductId, resc.ResourcesId))
                        {
                            string fileName = resc.OriginalFileName;
                            this.CurrentImageFile = string.IsNullOrEmpty(fileName) ? xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, xPort5.Controls.Utility.Product.KeyPicture(this.ProductId)) : xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, fileName);

                            this.productImage.ImageName = CurrentImageFile;

                            this.lblPictureName.Text = fileName;
                        }
                    }
                }
            }
        }

        #endregion

        #region Image Panel
        private Panel DrawPanel(Guid resId, string fileName)
        {
            Panel imgPane = new Panel();
            imgPane.Margin = new Padding(3);
            imgPane.Size = this.currentThumbnailSize;

            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            Article product = Article.Load(this.ProductId);

            xPort5.Controls.ProductImage prodImage = new xPort5.Controls.ProductImage();
            prodImage.Name = resId.ToString();
            prodImage.ImageName = string.IsNullOrEmpty(fileName) ? xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, xPort5.Controls.Utility.Product.KeyPicture(this.ProductId)) : xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, fileName);
            prodImage.BorderStyle = BorderStyle.FixedSingle;
            prodImage.BorderWidth = 1;
            prodImage.BorderColor = Color.Gainsboro;
            prodImage.Cursor = Cursors.Hand;
            prodImage.Size = this.currentThumbnailSize;
            prodImage.SizeMode = PictureBoxSizeMode.CenterImage;
            prodImage.Tag = product.ArticleCode;
            prodImage.ProductId = product.ArticleId;
            prodImage.Click += new EventHandler(prodImage_Click);
            this.toolTip1.SetToolTip(prodImage, oDict.GetWord("msg_click_to_select"));

            imgPane.Controls.Add(prodImage);

            return imgPane;
        }

        void prodImage_Click(object sender, EventArgs e)
        {
            ReallocateImagePositionAndSize(xPort5.Controls.Utility.Resources.ImageSize.Medium);

            ProductImage prodImage = sender as ProductImage;
            if (prodImage != null)
            {
                this.CurrentImageFile = prodImage.ImageName;

                productImage.ImageName = CurrentImageFile;
                this.lblPictureName.Text = Path.GetFileName(CurrentImageFile);

                this.ResourceId = new Guid(prodImage.Name);
            }
        }

        private void ReallocateImagePositionAndSize(Size originalSize)
        {
            //int x = 0, y = 0;
            //decimal originalX = this.imagePane.Size.Width;
            //decimal originalY = this.imagePane.Size.Height;

            this.currentImageLocation = new Point((this.imagePane.Width / 2) - (originalSize.Width / 2), (this.imagePane.Height / 2) - (originalSize.Height / 2));
            //x = Convert.ToInt32((originalX / 2) - originalSize.Width);
            //y = Convert.ToInt32((originalY / 2) - originalSize.Height);

            //this.currentImageLocation = new Point(x, y);

            this.productImage.Location = this.currentImageLocation;
            this.productImage.Size = originalSize;
        }

        #endregion

        private bool zoomed = false;

        private void btnZoom_Click(object sender, EventArgs e)
        {
            if (!zoomed)
            {
                ReallocateImagePositionAndSize(xPort5.Controls.Utility.Resources.ImageSize.Large);
            }
            else
            {
                ReallocateImagePositionAndSize(xPort5.Controls.Utility.Resources.ImageSize.Medium);
            }

            zoomed = !zoomed;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            UploadFile();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_DeleteMsg + " ?", _ErrCaption, MessageBoxButtons.YesNo, new EventHandler(DeleteFile));
        }

        private void btnPrimary_Click(object sender, EventArgs e)
        {
            if (Utility.Product.IsKeyPicture(this.ProductId, this.ResourceId))
            {
                Utility.Product.DeleteKeyPicture(this.ProductId, this.ResourceId);
            }
            else
            {
                if (Utility.Product.HasKeyPicture(this.ProductId))
                {
                    Utility.Product.DeleteKeyPicture(this.ProductId);
                }
            }

            Utility.Product.SaveKeyPicture(this.ProductId, this.ResourceId);

            BindImageList(this.currentThumbnailSize, false);
        }

        #region Upload, Download

        private void DeleteFile(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                bool cannotDelete = false;
                string sql = "Keyword = '" + Utility.Product.ProductCode(this.ProductId) + "'";

                string fileName = xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, Path.GetFileName(CurrentImageFile));

                xPort5.EF6.ResourcesCollection deleteList = xPort5.EF6.Resources.LoadCollection(sql);
                foreach (xPort5.EF6.Resources resc in deleteList)
                {
                    if (Utility.Product.IsKeyPicture(this.ProductId, resc.ResourcesId))
                    {
                        //Utility.Product.DeleteKeyPicture(this.ProductId, resc.ResourcesId);
                        cannotDelete = true;
                    }

                    if (!cannotDelete)
                    {
                        if (resc.OriginalFileName == Path.GetFileName(fileName))
                        {
                            resc.Delete();

                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }

                            string gdocFile = string.Format(_GDocFileName, Utility.Product.ProductCode(this.ProductId), Utility.Product.GetCategoryName(this.ProductId), resc.OriginalFileName);
                            //if (GData.GDocs.IsFileExist(gdocFile))
                            //{
                            //    GData.GDocs.DeleteFile(gdocFile);
                            //}
                        }
                    }
                }

                xPort5.EF6.ResourcesCollection rescList = xPort5.EF6.Resources.LoadCollection(sql);
                if (rescList.Count > 0)
                {
                    if (!Utility.Product.HasKeyPicture(this.ProductId))
                    {
                        Utility.Product.SaveKeyPicture(this.ProductId, rescList[0].ResourcesId);
                    }
                }

                if (!cannotDelete)
                {
                    BindImageList(this.currentThumbnailSize, false);
                }
                else
                {
                    MessageBox.Show(_ErrCannotBeDeleted, _ErrCaption, MessageBoxButtons.OK,  MessageBoxIcon.Error);
                }
            }
        }

        private void UploadFile()
        {
            openFileDialog.MaxFileSize = Common.Config.MaxFileSize;
            openFileDialog.Title = this.Text;
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            OpenFileDialog objFileDialog = sender as OpenFileDialog;
            if (objFileDialog != null)
            {
                string fileName = xPort5.Controls.Utility.Resources.UploadPicture(openFileDialog, this.ProductId);

                string fullName = xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, fileName);
                fileName = string.Format(_GDocFileName, Utility.Product.ProductCode(this.ProductId), Utility.Product.GetCategoryName(this.ProductId), Path.GetFileName(fileName));

                //if (!GData.GDocs.IsFileExist(fileName))
                //{
                //    GData.GDocs.UploadFile3(fullName, fileName);
                //}

                BindImageList(this.currentThumbnailSize, false);
            }
        }

        #endregion
    }
}
