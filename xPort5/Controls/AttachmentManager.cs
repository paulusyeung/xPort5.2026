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
using xPort5.Controls.Product;
using xPort5.Controls.ConvertPdf;

#endregion

namespace xPort5.Controls
{
    public partial class AttachmentManager : Form
    {
        private string _MsgCaption = string.Empty;
        private string _DeleteMsg = string.Empty;
        private string _ErrCaption = string.Empty;
        private string _ErrMsg = string.Empty;
        private string _GDocFileName = "{0} {1} {2}"; // OrderNumber + " " + AttachmentType + " " + FileName

        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }

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

        public AttachmentManager()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();
            SetAnsToolbar();

            PrepareGDocs();

            BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
        }

        #region Set Captions, AnsToolbar, Combo, PrintFormDefault

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = oDict.GetWord("attachment_manager");
            _MsgCaption = oDict.GetWord("message");
            _ErrCaption = oDict.GetWord("warning");
            _ErrMsg = oDict.GetWord("err_cannot_be_blank");
            _DeleteMsg = oDict.GetWord("delete");
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

            #region cmdUPload, cmdDownload

            ToolBarButton cmdUpload = new ToolBarButton("Upload", oDict.GetWord("upload"));
            cmdUpload.Tag = "Upload";
            cmdUpload.Image = new IconResourceHandle("16x16.dropbox_out_16x16.png");
            this.ansToolbar.Buttons.Add(cmdUpload);

            ToolBarButton cmdDownload = new ToolBarButton("Download", oDict.GetWord("download"));
            cmdDownload.Tag = "Download";
            cmdDownload.Image = new IconResourceHandle("16x16.dropbox_in_16x16.png");
            this.ansToolbar.Buttons.Add(cmdDownload);

            #endregion

            // cmdPrint
            ToolBarButton cmdPrint = new ToolBarButton("Delete", oDict.GetWord("delete"));
            cmdPrint.Tag = "Delete";
            cmdPrint.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            this.ansToolbar.Buttons.Add(cmdPrint);
            this.ansToolbar.Buttons.Add(sep);

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
                    case "x-large":
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
                    case "upload":
                        UploadAttachment();
                        break;
                    case "download":
                        DownloadAttachment();
                        break;
                    case "delete":
                        MessageBox.Show(_DeleteMsg + " ?", _ErrCaption, MessageBoxButtons.YesNo, new EventHandler(DeleteAttachment));
                        break;
                }
            }
        }

        #endregion

        #region Attachment List

        private void PrepareGDocs()
        {
            if (this.OrderId != null)
            {
                string sql = "KeyWord = '" + this.OrderNumber + "'";
                ResourcesCollection rescList = Resources.LoadCollection(sql);
                foreach (Resources resc in rescList)
                {
                    string fileName = resc.OriginalFileName;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (!File.Exists(xPort5.Controls.Utility.Resources.PictureFilePath(this.OrderId, fileName)))
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
            this.paneAttachmentList.Controls.Clear();

            if (this.OrderId != null)
            {
                string sql = "KeyWord = '" + this.OrderNumber + "'";
                ResourcesCollection rescList = Resources.LoadCollection(sql);
                foreach (Resources resc in rescList)
                {
                    if (!string.IsNullOrEmpty(resc.OriginalFileName))
                    {
                        paneAttachmentList.Controls.Add(GetImageListItem(imageSize, resc.OriginalFileName, false, (Common.Enums.ContentType)Enum.Parse(typeof(Common.Enums.ContentType), resc.ContentType.ToString())));
                    }
                }
            }
        }

        private Panel GetImageListItem(Size imageSize, string attachedFileName, bool inDetail, Common.Enums.ContentType contentType)
        {
            Panel attach = new Panel();

            attach.Size = new Size(imageSize.Width + 20, imageSize.Height + 40);

            string fileExt = Path.GetExtension(attachedFileName).Remove(0, 1).ToLower();
            string fileName = string.Empty;

            if (contentType == Common.Enums.ContentType.Image)
            {
                fileName = xPort5.Controls.Utility.Resources.ResourceFilePath(this.OrderNumber, attachedFileName);
            }
            else
            {
                if (fileExt.Equals("pdf"))
                {
                    fileName = string.Format("{0}.jpg", xPort5.Controls.Utility.Resources.ResourceFilePath(this.OrderNumber, attachedFileName));
                }
                else
                {
                    fileName = GetContentIcons(contentType, attachedFileName);
                }
            }

            #region CheckBox

            CheckBox chkAttachment = new CheckBox();
            chkAttachment.Location = new Point(10, 7);
            chkAttachment.Size = new Size(20, 20);
            chkAttachment.CheckAlign = ContentAlignment.MiddleLeft;
            attach.Controls.Add(chkAttachment);

            #endregion

            #region Add Product Image

            ProductImage attachedImage = new ProductImage();
            attachedImage.Tag = attachedFileName;
            attachedImage.Name = Path.GetFileNameWithoutExtension(attachedFileName);
            attachedImage.ImageName = fileName;
            attachedImage.BorderStyle = BorderStyle.FixedSingle;
            attachedImage.BorderWidth = 1;
            attachedImage.BorderColor = System.Drawing.Color.Gainsboro;
            attachedImage.Cursor = Cursors.Hand;
            attachedImage.Location = new Point(10, 10);
            attachedImage.Size = imageSize;
            attachedImage.SizeMode = PictureBoxSizeMode.CenterImage;
            attachedImage.Click += new EventHandler(attachedImage_Click);
            attach.Controls.Add(attachedImage);

            #endregion

            if (!(inDetail))
            {
                #region Add Product Code

                Label prodCode = new Label();
                prodCode.Location = new Point(10, imageSize.Height + 10);
                prodCode.Size = new Size(imageSize.Width, 20);
                prodCode.TextAlign = ContentAlignment.MiddleCenter;
                prodCode.Text = Path.GetFileNameWithoutExtension(attachedFileName);
                attach.Controls.Add(prodCode);

                #endregion
            }
            else
            {
                #region Add Product Code, Description, Category, Color, Origin, Remarks

                #region Add Product Code

                //Label prodCode = new Label();
                //prodCode.Location = new Point(20, imageSize.Height + 10);
                //prodCode.Size = new Size(imageSize.Width, 20);
                //prodCode.TextAlign = ContentAlignment.TopLeft;
                //prodCode.Text = product.ArticleCode;
                //prod.Controls.Add(prodCode);

                #endregion

                #region Add Description

                //TextBox description = new TextBox();
                ////Label description = new Label();
                //description.Location = new Point(20, imageSize.Height + 30);
                //description.Size = new Size(imageSize.Width - 20, 60);
                //description.Multiline = true;
                //description.ScrollBars = ScrollBars.Vertical;
                //description.BorderColor = Color.Gainsboro;
                //description.BorderStyle = BorderStyle.FixedSingle;
                //description.BorderWidth = 1;
                ////description.AutoSize = true;
                ////description.MaximumSize = new Size(imageSize.Width, 90);
                ////description.TextAlign = ContentAlignment.TopLeft;
                //description.Text = product.ArticleName;
                //prod.Controls.Add(description);

                #endregion

                #region Add Category

                //T_Category cat = T_Category.Load(product.CategoryId);
                //Label category = new Label();
                //category.Location = new Point(20, imageSize.Height + 100);
                //category.Size = new Size(imageSize.Width, 20);
                //category.TextAlign = ContentAlignment.TopLeft;
                //category.Text = cat.CategoryName;
                //prod.Controls.Add(category);

                #endregion

                #region Add Color

                //if (product.PictureFile != String.Empty)
                //{
                //    Label lblColor = new Label();
                //    lblColor.Location = new Point(20, imageSize.Height + 120);
                //    lblColor.Size = new Size(imageSize.Width, 20);
                //    lblColor.TextAlign = ContentAlignment.TopLeft;
                //    lblColor.Text = product.PictureFile;
                //    prod.Controls.Add(lblColor);
                //}
                //else
                //{
                //    T_AgeGrading color = T_AgeGrading.Load(product.AgeGradingId);
                //    Label lblColor = new Label();
                //    lblColor.Location = new Point(20, imageSize.Height + 120);
                //    lblColor.Size = new Size(imageSize.Width, 20);
                //    lblColor.TextAlign = ContentAlignment.TopLeft;
                //    lblColor.Text = color.AgeGradingName;
                //    prod.Controls.Add(lblColor);
                //}

                #endregion

                #region Add Origin

                //T_Origin origin = T_Origin.Load(product.OriginId);
                //Label lblOrigin = new Label();
                //lblOrigin.Location = new Point(20, imageSize.Height + 140);
                //lblOrigin.Size = new Size(imageSize.Width, 20);
                //lblOrigin.TextAlign = ContentAlignment.TopLeft;
                //lblOrigin.Text = origin.OriginName;
                //prod.Controls.Add(lblOrigin);

                #endregion

                #endregion

                //prod.Height = lblOrigin.Location.Y + 20;
            }

            return attach;
        }

        void attachedImage_Click(object sender, EventArgs e)
        {
            ProductImage attachedImage = sender as ProductImage;
            if (attachedImage != null)
            {
                for (int i = 0; i < attachedImage.Parent.Controls.Count; i++)
                {
                    Control ctrl = attachedImage.Parent.Controls[i];
                    if (ctrl is CheckBox)
                    {
                        CheckBox chkCtrl = ctrl as CheckBox;
                        if (chkCtrl != null)
                        {
                            chkCtrl.Checked = !chkCtrl.Checked;
                        }
                    }
                }
            }
        }

        #endregion

        #region Upload, Download

        private void SaveResource(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Resources resc = new Resources();
                resc.Keyword = this.OrderNumber;
                resc.ContentType = (int)GetContentType(fileName);
                resc.OriginalFileName = fileName;
                resc.SaveAsFileId = this.OrderId.ToString();
                resc.SaveAsFileName = fileName;
                resc.CreatedBy = Common.Config.CurrentUserId;
                resc.CreatedOn = DateTime.Now;
                resc.ModifiedBy = Common.Config.CurrentUserId;
                resc.ModifiedOn = DateTime.Now;

                resc.Save();

                BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
            }
        }

        private Common.Enums.ContentType GetContentType(string fileName)
        {
            string fileExt = Path.GetExtension(fileName).Remove(0, 1).ToLower();

            switch (fileExt)
            {
                case "jpg":
                case "jpeg":
                    return Common.Enums.ContentType.Image;
                case "doc":
                case "docx":
                    return Common.Enums.ContentType.MSWord;
                case "xls":
                case "xlsx":
                    return Common.Enums.ContentType.MSExcel;
                case "pdf":
                    return Common.Enums.ContentType.PdfFile;
                case "mp4":
                    return Common.Enums.ContentType.Video;
                case "txt":
                default:
                    return Common.Enums.ContentType.PlainText;
            }
        }

        private string GetContentIcons(Common.Enums.ContentType contentType, string fileName)
        {
            string result = fileName;

            switch (contentType)
            {
                case Common.Enums.ContentType.Image:
                    result = fileName;
                    break;
                case Common.Enums.ContentType.MSExcel:
                    result = Path.Combine(VWGContext.Current.Config.GetDirectory("Icons"), @"FileType\16_xls.gif");
                    break;
                case Common.Enums.ContentType.MSWord:
                    result = Path.Combine(VWGContext.Current.Config.GetDirectory("Icons"), @"FileType\16_doc.gif");
                    break;
                case Common.Enums.ContentType.PdfFile:
                    result = Path.Combine(VWGContext.Current.Config.GetDirectory("Icons"), @"FileType\16_pdf.gif");
                    break;
                case Common.Enums.ContentType.Video:
                    result = Path.Combine(VWGContext.Current.Config.GetDirectory("Icons"), @"FileType\16_media.gif");
                    break;
                case Common.Enums.ContentType.PlainText:
                default:
                    result = Path.Combine(VWGContext.Current.Config.GetDirectory("Icons"), @"FileType\16_generic.gif");
                    break;
            }

            return result;
        }

        private List<string> GetCheckedItems()
        {
            List<string> checkedItems = new List<string>();

            for (int i = 0; i < paneAttachmentList.Controls.Count; i++)
            {
                Control ctrl = paneAttachmentList.Controls[i];
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
                                    checkedItems.Add(imgCtrl.Tag.ToString());
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
                        return chkCtrl.Checked;
                    }
                }
            }

            return false;
        }

        private void DeleteAttachment(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                foreach (string attachedFile in GetCheckedItems())
                {
                    string sql = "SaveAsFileId = '" + this.OrderId.ToString() + "'";
                    string fileName = xPort5.Controls.Utility.Resources.ResourceFilePath(this.OrderNumber, attachedFile);

                    xPort5.EF6.ResourcesCollection rescList = xPort5.EF6.Resources.LoadCollection(sql);
                    foreach (xPort5.EF6.Resources resc in rescList)
                    {
                        if (resc.OriginalFileName == attachedFile)
                        {
                            resc.Delete();

                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }

                            if (Path.GetExtension(fileName).ToLower() == ".pdf")
                            {
                                string pdfImagefile = string.Format("{0}.jpg", fileName);
                                if (File.Exists(pdfImagefile))
                                {
                                    File.Delete(pdfImagefile);
                                }
                            }

                            //string gdocFile = string.Format(_GDocFileName, Utility.JobOrder.GetOrderNumber(this.OrderId), this.AttachType.ToString("d"), attached.OriginalFileName);
                            //if (GData.GDocs.IsFileExist(gdocFile))
                            //{
                            //    GData.GDocs.DeleteFile(gdocFile);
                            //}
                        }
                    }
                }

                BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
            }
        }

        private void DownloadAttachment()
        {
            foreach (string attachedFile in GetCheckedItems())
            {
                string fileName = xPort5.Controls.Utility.Resources.ResourceFilePath(this.OrderNumber, attachedFile);

                FileDownloadGateway downloadGateway = new FileDownloadGateway();
                downloadGateway.Filename = Path.GetFileName(fileName);
                downloadGateway.StartFileDownload(this, fileName);
            }
        }

        private void UploadAttachment()
        {
            openFileDialog.MaxFileSize = Common.Config.MaxFileSize;
            openFileDialog.Title = this.Text;
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            OpenFileDialog objFileDialog = sender as OpenFileDialog;
            if (objFileDialog != null)
            {
                string fileName = xPort5.Controls.Utility.Resources.UploadResource(openFileDialog, this.OrderNumber);

                CreatePdfPreviewImage(fileName);

                SaveResource(Path.GetFileName(fileName));

                //string fullName = Utility.JobOrder.AttachmentFilePath(this.OrderId, this.AttachType.ToString("d"), fileName);
                //fileName = string.Format(_GDocFileName, Utility.JobOrder.GetOrderNumber(this.OrderId), this.AttachType.ToString("d"), fileName);
                //if (!GData.GDocs.IsFileExist(fileName))
                //{
                //    GData.GDocs.UploadFile3(fullName, fileName);
                //}
            }
        }

        private void CreatePdfPreviewImage(string fileName)
        {
            if (Path.GetExtension(fileName).ToLower() == ".pdf")
            {
                try
                {
                    string fullName = xPort5.Controls.Utility.Resources.UploadResource(openFileDialog, this.OrderNumber);
                    string imgName = string.Format("{0}.jpg", fullName);

                    if (!File.Exists(imgName))
                    {
                        PdfConverter.Convert(fullName, imgName);
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.StackTrace, exc.Message);
                }
            }
        }

        #endregion
    }
}
