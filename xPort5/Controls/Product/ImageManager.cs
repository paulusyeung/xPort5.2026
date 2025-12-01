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
    public partial class ImageManager : Form
    {
        private string _MsgCaption = string.Empty;
        private string _DeleteMsg = string.Empty;
        private string _ErrCaption = string.Empty;
        private string _ErrMsg = string.Empty;
        private string _GDocFileName = "{0} {1} {2}"; // ArticleCode + " " + FileType + " " + FileName

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

        public ImageManager()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();
            SetAnsToolbar();

            PrepareGWebAlbum();

            BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
        }

        #region Set Captions, AnsToolbar, Combo, PrintFormDefault

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = oDict.GetWord("product_image_manager");

            _MsgCaption = oDict.GetWord("message");
            _ErrCaption = oDict.GetWord("warning");
            _ErrMsg = oDict.GetWord("err_cannot_be_blank");
            _DeleteMsg = oDict.GetWord("delete_picture");
        }

        private void SetAnsToolbar()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdImageList    - Buttons[0]

            ContextMenu ddlImageList = new ContextMenu();
            Common.Data.AppendMenuItem_AppImageList(ref ddlImageList);
            ToolBarButton cmdImageList = new ToolBarButton("Images", oDict.GetWord("images"));
            cmdImageList.Style = ToolBarButtonStyle.DropDownButton;
            cmdImageList.Image = new IconResourceHandle("16x16.imagelist_duo_on_16.png");
            cmdImageList.DropDownMenu = ddlImageList;

            this.ansToolbar.Buttons.Add(cmdImageList);

            cmdImageList.MenuClick += new MenuEventHandler(cmdImageList_MenuClick);

            #endregion

            this.ansToolbar.Buttons.Add(sep);

            ToolBarButton cmdRefresh = new ToolBarButton("Refresh", oDict.GetWord("Refresh"));
            cmdRefresh.Tag = "Refresh";
            cmdRefresh.Image = new IconResourceHandle("16x16.16_L_refresh.gif");
            this.ansToolbar.Buttons.Add(cmdRefresh);

            this.ansToolbar.Buttons.Add(sep);

            #region Upload/Download

            ToolBarButton cmdUpload = new ToolBarButton("Upload", oDict.GetWord("upload"));
            cmdUpload.Tag = "Upload";
            cmdUpload.Image = new IconResourceHandle("16x16.dropbox_out_16x16.png");
            this.ansToolbar.Buttons.Add(cmdUpload);

            ToolBarButton cmdDownload = new ToolBarButton("Download", oDict.GetWord("Download"));
            cmdDownload.Tag = "Download";
            cmdDownload.Image = new IconResourceHandle("16x16.dropbox_in_16x16.png");
            this.ansToolbar.Buttons.Add(cmdDownload);

            this.ansToolbar.Buttons.Add(sep);

            #endregion

            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("delete"));
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            this.ansToolbar.Buttons.Add(cmdDelete);

            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
        }

        void cmdImageList_MenuClick(object sender, MenuItemEventArgs e)
        {
            if (e.MenuItem.Tag != null)
            {
                switch (((string)e.MenuItem.Tag).ToLower())
                {
                    case "small":
                        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Small, false);
                        break;
                    case "medium":
                        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                        break;
                    case "large":
                        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Large, false);
                        break;
                    case "details":
                        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.XLarge, false);
                        break;
                }
            }
        }

        private void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString().ToLower())
                {
                    case "refresh":
                        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                        break;
                    case "upload":
                        UploadFile();
                        break;
                    case "download":
                        DownloadFile();
                        break;
                    case "delete":
                        MessageBox.Show(_DeleteMsg + " ?", _ErrCaption, MessageBoxButtons.YesNo, new EventHandler(DeleteFile));
                        break;
                }
            }
        }

        #endregion

        #region Image List

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
            this.paneFileList.Controls.Clear();

            if (this.ProductId != null)
            {
                string sql = "Keyword = '" + Utility.Product.ProductCode(this.ProductId) + "'";
                string[] orderBy = new string[] { "OriginalFileName" };
                ResourcesCollection rescList = Resources.LoadCollection(sql, orderBy, true);
                foreach (Resources resc in rescList)
                {
                    if (!string.IsNullOrEmpty(resc.OriginalFileName))
                    {
                        ImagePanel imgPane = new ImagePanel(imageSize, this.ProductId, inDetail, resc.ResourcesId, resc.ResourcesId, resc.OriginalFileName, true, true);
                        imgPane.KeyPictureBoxClick += new EventHandler(imgPane_KeyPictureBoxClick);

                        paneFileList.Controls.Add(imgPane);
                    }
                }
            }
        }

        void imgPane_KeyPictureBoxClick(object sender, EventArgs e)
        {
            CheckBox chkCtrl = sender as CheckBox;
            if (chkCtrl != null)
            {
                if (chkCtrl.Tag != null)
                {
                    if (chkCtrl.Tag is List<Guid>)
                    {
                        List<Guid> idList = chkCtrl.Tag as List<Guid>;
                        if (idList != null && idList.Count > 1)
                        {
                            System.Guid productId = idList[0];
                            System.Guid resourceId = idList[1];

                            if (Utility.Product.IsKeyPicture(productId, resourceId))
                            {
                                Utility.Product.DeleteKeyPicture(this.ProductId, resourceId);
                            }
                            else
                            {
                                if (Utility.Product.HasKeyPicture(productId))
                                {
                                    Utility.Product.DeleteKeyPicture(productId);
                                }
                            }

                            Utility.Product.SaveKeyPicture(productId, resourceId);

                            BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                        }
                    }
                }
            }
        }

        #endregion

        #region Upload, Download

        private List<string> GetCheckedItems()
        {
            List<string> checkedItems = new List<string>();

            for (int i = 0; i < paneFileList.Controls.Count; i++)
            {
                Control ctrl = paneFileList.Controls[i];
                if (ctrl is Panel)
                {
                    for (int j = 0; j < ctrl.Controls.Count; j++)
                    {
                        Control pCtrl = ctrl.Controls[j];
                        if (pCtrl is ProductImage)
                        {
                            ProductImage imgCtrl = pCtrl as ProductImage;
                            if (imgCtrl != null)
                            {
                                if (IsChecked(imgCtrl.Parent))
                                {
                                    checkedItems.Add(Path.GetFileName(imgCtrl.ImageName));
                                }
                            }
                        }
                    }
                }
            }

            return checkedItems;
        }

        private bool IsChecked(Control panel)
        {
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                Control ctrl = panel.Controls[i];
                if (ctrl is CheckBox)
                {
                    CheckBox chkCtrl = ctrl as CheckBox;
                    if (chkCtrl != null)
                    {
                        if (chkCtrl.Tag != null && chkCtrl.Tag.ToString().ToLower() == "selectedimage")
                        {
                            return chkCtrl.Checked;
                        }
                    }
                }
            }

            return false;
        }

        private void DeleteFile(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                string sql = "Keyword = '" + Utility.Product.ProductCode(this.ProductId) + "'";

                foreach (string item in GetCheckedItems())
                {
                    string fileName = xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, item);

                    xPort5.EF6.ResourcesCollection deleteList = xPort5.EF6.Resources.LoadCollection(sql);
                    foreach (xPort5.EF6.Resources resc in deleteList)
                    {
                        if (Utility.Product.IsKeyPicture(this.ProductId, resc.ResourcesId))
                        {
                            Utility.Product.DeleteKeyPicture(this.ProductId, resc.ResourcesId);
                        }

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

                BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
            }
        }

        private void DownloadFile()
        {
            foreach (string item in GetCheckedItems())
            {
                string fileName = xPort5.Controls.Utility.Resources.PictureFilePath(this.ProductId, item);

                FileDownloadGateway downloadGateway = new FileDownloadGateway();
                downloadGateway.Filename = Path.GetFileName(fileName);
                downloadGateway.StartFileDownload(this, fileName);
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

                BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
            }
        }

        #endregion
    }
}
