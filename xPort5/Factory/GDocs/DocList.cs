#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;

using Microsoft.Win32;

using MarkPasternak.Utility;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Interfaces;
using Gizmox.WebGUI.Common.Gateways;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Forms.Dialogs;

using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;

using xPort5.DAL;

#endregion

namespace xPort5.Factory.GDocs
{
    public partial class DocList : UserControl, IGatewayComponent
    {
        private int _ClientId = 0;
        private string _filename = String.Empty;
        private DocumentsFeed _FolderList = null;
        private string _CurFolderText = String.Empty;
        private string _CurFolderUri = String.Empty;

        public DocList()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();
            SetAttribute();
            ResetToolbar();
            BuildFolderTree(tvwClient.Nodes);
        }

        #region IGatewayControl Members
        void IGatewayComponent.ProcessRequest(IContext objContext, string strAction)
        {
            // Trt to get the gateway handler
            IGatewayHandler objGatewayHandler = ProcessGatewayRequest(objContext.HttpContext, strAction);

            if (objGatewayHandler != null)
            {
                objGatewayHandler.ProcessGatewayRequest(objContext, this);
            }
        }

        protected override IGatewayHandler ProcessGatewayRequest(HttpContext objContext, string strAction)
        {
            IGatewayHandler objGH = null;

            string filepath = String.Format(@"{0}\{1}", Common.Client.DropBox(_ClientId), _filename);

            if (File.Exists(filepath))
            {
                FileInfo oFile = new FileInfo(filepath);
                if (oFile.Length > 1024 * 1024 * 32)
                {
                    // use this method for file size over 32MB 
                    WriteFileHelper oWriteFile = new WriteFileHelper();
                    oWriteFile.BufferSize = 65536;
                    oWriteFile.WriteFileToResponseStreamWithForceDownloadHeaders(filepath, _filename);
                }
                else
                {
                    HttpResponse response = objContext.Response;    // prefer to use Gizmox instead of: this.Context.HttpContext.Response;

                    response.Buffer = true;
                    response.Clear();
                    response.ClearHeaders();
                    response.ContentType = "application/octet-stream";
                    response.AddHeader("content-disposition", "attachment; filename=" + _filename);
                    response.WriteFile(filepath);
                    response.Flush();
                    response.End();
                }
            }
            else
            {
                objContext.Response.Write(String.Format("<html><body><h>File: {0}, file not found!</h></body></html>", _filename));
            }

            return objGH;
        }
        #endregion

        #region Set Attributes, Themes
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            colFileName.Text = oDict.GetWord("file_name");
            colFileSize.Text = oDict.GetWord("author");
            colFileType.Text = oDict.GetWord("file_type");
            colModifiedOn.Text = oDict.GetWord("modified_on");
        }

        private void SetAttribute()
        {
            this.lvwFileExplorer.ListViewItemSorter = new ListViewItemSorter(this.lvwFileExplorer);

            toolTip1.SetToolTip(lvwFileExplorer, "Double click to download file");
        }
        #endregion

        #region build Folder tree
        private void BuildFolderTree(TreeNodeCollection root)
        {
            DocumentsService docService = xPort5.Controls.GData.GDocs.GetService();

            FolderQuery folderQuery = new FolderQuery();
            _FolderList = docService.Query(folderQuery);

            foreach (DocumentEntry entry in _FolderList.Entries)
            {
                string title = (entry.Title.Text);

                if (entry.ParentFolders.Count == 0)
                {
                    #region Add Root Nodes
                    TreeNode oNode = new TreeNode();
                    oNode.Image = new IconResourceHandle("16x16.folder_close.png");
                    oNode.ExpandedImage = new IconResourceHandle("16x16.folder_open.png");
                    oNode.IsExpanded = true;
                    oNode.Label = entry.Title.Text;
                    oNode.Tag = entry.SelfUri.ToString();

                    root.Add(oNode);
                    #endregion

                    AddChildFolders(oNode.Nodes, entry.Title.Text);
                }
            }
        }

        private void AddChildFolders(TreeNodeCollection oNodes, string title)
        {
            foreach (DocumentEntry entry in _FolderList.Entries)
            {
                foreach (AtomLink item in entry.ParentFolders)
                {
                    if (item.Title == title)
                    {
                        TreeNode oNode = new TreeNode();
                        oNode.Image = new IconResourceHandle("16x16.folder_close.png");
                        oNode.ExpandedImage = new IconResourceHandle("16x16.folder_open.png");
                        oNode.IsExpanded = true;
                        oNode.Label = entry.Title.Text;
                        oNode.Tag = entry.SelfUri.ToString();
                        oNodes.Add(oNode);

                        AddChildFolders(oNode.Nodes, entry.Title.Text);
                    }
                }
            }
        }
        #endregion

        #region SetToolbar(), ResetToolbar(), SetfileExplorerAns();
        private void ResetToolbar()
        {
            ResetClientTreeAns();
            ResetFileExplorerAns();
        }

        private void ResetClientTreeAns()
        {
            this.ansClientTree.MenuHandle = false;
            this.ansClientTree.DragHandle = false;
            this.ansClientTree.TextAlign = ToolBarTextAlign.Right;
        }

        private void ResetFileExplorerAns()
        {
            this.ansFileExplorer.MenuHandle = false;
            this.ansFileExplorer.DragHandle = false;
            this.ansFileExplorer.TextAlign = ToolBarTextAlign.Right;
        }

        private void SetFileExplorerAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansFileExplorer.MenuHandle = false;
            this.ansFileExplorer.DragHandle = false;
            this.ansFileExplorer.TextAlign = ToolBarTextAlign.Right;
            this.ansFileExplorer.Buttons.Clear();

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdButtons   - Buttons [0~3]
            this.ansFileExplorer.Buttons.Add(new ToolBarButton("Columns", String.Empty));
            this.ansFileExplorer.Buttons[0].Image = new IconResourceHandle("16x16.listview_columns.gif");
            this.ansFileExplorer.Buttons[0].ToolTipText = @"Hide/Unhide Columns";
            this.ansFileExplorer.Buttons.Add(new ToolBarButton("Sorting", String.Empty));
            this.ansFileExplorer.Buttons[1].Image = new IconResourceHandle("16x16.listview_sorting.gif");
            this.ansFileExplorer.Buttons[1].ToolTipText = @"Sorting";
            this.ansFileExplorer.Buttons.Add(new ToolBarButton("Checkbox", String.Empty));
            this.ansFileExplorer.Buttons[2].Image = new IconResourceHandle("16x16.listview_checkbox.gif");
            this.ansFileExplorer.Buttons[2].ToolTipText = @"Toggle Checkbox";
            this.ansFileExplorer.Buttons.Add(new ToolBarButton("MultiSelect", String.Empty));
            this.ansFileExplorer.Buttons[3].Image = new IconResourceHandle("16x16.listview_multiselect.gif");
            this.ansFileExplorer.Buttons[3].ToolTipText = @"Toggle Multi-Select";
            this.ansFileExplorer.Buttons[3].Visible = false;
            #endregion

            this.ansFileExplorer.Buttons.Add(sep);

            #region cmdViews    - Buttons[5]
            ContextMenu ddlViews = new ContextMenu();
            Common.Data.AppendMenuItem_AppViews(ref ddlViews);
            ToolBarButton cmdViews = new ToolBarButton("Views", oDict.GetWord("views"));
            cmdViews.Style = ToolBarButtonStyle.DropDownButton;
            cmdViews.Image = new IconResourceHandle("16x16.appView_xp.png");
            cmdViews.DropDownMenu = ddlViews;
            this.ansFileExplorer.Buttons.Add(cmdViews);
            cmdViews.MenuClick += new MenuEventHandler(ansViews_MenuClick);
            #endregion

            this.ansFileExplorer.Buttons.Add(sep);

            #region cmdRefresh, cmdPreference       - Buttons[7~8]
            this.ansFileExplorer.Buttons.Add(new ToolBarButton("Refresh", oDict.GetWord("refresh")));
            this.ansFileExplorer.Buttons[7].Image = new IconResourceHandle("16x16.16_L_refresh.gif");
            this.ansFileExplorer.Buttons.Add(new ToolBarButton("Preference", oDict.GetWord("preference")));
            this.ansFileExplorer.Buttons[8].Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            this.ansFileExplorer.Buttons[8].Enabled = false;
            this.ansFileExplorer.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
            #endregion

            this.ansFileExplorer.Buttons.Add(sep);

            //ToolBarButton cmdClientInfo = new ToolBarButton("ClientInfo", oDict.GetWord("client_info"));
            //cmdClientInfo.Image = new IconResourceHandle("16x16.group.png");

            ToolBarButton cmdUpload = new ToolBarButton("DropFile", oDict.GetWord("upload_file"));
            cmdUpload.Image = new IconResourceHandle("16x16.dropbox_out_16x16.png");

            ToolBarButton cmdDownload = new ToolBarButton("RetrieveFile", oDict.GetWord("download_file"));
            cmdDownload.Image = new IconResourceHandle("16x16.dropbox_in_16x16.png");

            ToolBarButton cmdDelete = new ToolBarButton("DeleteFile", oDict.GetWord("delete_file"));
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

//            this.ansFileExplorer.Buttons.Add(cmdClientInfo);
            this.ansFileExplorer.Buttons.Add(cmdUpload);
            this.ansFileExplorer.Buttons.Add(cmdDownload);
            this.ansFileExplorer.Buttons.Add(cmdDelete);
            this.ansFileExplorer.Buttons.Add(sep);
        }
        #endregion

        private void BindFileExplorer()
        {
            RegistryKey rootKey = Registry.ClassesRoot;
            lvwFileExplorer.Items.Clear();
            DocumentsFeed docList = xPort5.Controls.GData.GDocs.GetDocsInFolder(_CurFolderText);
            foreach (DocumentEntry fileinfo in docList.Entries)
            {
                string suffix = fileinfo.Title.Text.Substring(fileinfo.Title.Text.Length - 4, 4).ToLower();
                string contentType = String.Empty;

                #region retrieve Content Type
                try
                {
                    RegistryKey subKey = rootKey.OpenSubKey(suffix);
                    contentType = (string)subKey.GetValue("Content Type");
                }
                catch
                {
                    contentType = String.Empty;
                }
                #endregion

                ListViewItem listitem = this.lvwFileExplorer.Items.Add(fileinfo.Title.Text);            // File Name
                #region File Icon
                switch (suffix)
                {
                    case ".ai":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.ai16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.ai32.png");
                        break;
                    case ".cdr":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.cdr16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.cdr32.png");
                        break;
                    case ".dwt":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.dwt16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.dwt32.png");
                        break;
                    case ".doc":
                    case ".docx":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.doc16.gif");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.doc32.gif");
                        break;
                    case ".fh8":
                    case ".fh9":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.fh16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.fh32.png");
                        break;
                    case ".ind":
                    case ".indd":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.iddd16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.indd32.png");
                        break;
                    case ".jpg":
                    case ".jpeg":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.jpg16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.jpg32.png");
                        break;
                    case ".mdb":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.mdb16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.mdb32.png");
                        break;
                    case ".pdf":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.pdf16.gif");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.pdf32.gif");
                        break;
                    case ".pm":
                    case ".pm6":
                    case ".pmd":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.pmd16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.pmd32.png");
                        break;
                    case ".png":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.png16.gif");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.png32.gif");
                        break;
                    case ".psd":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.psd16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.psd32.png");
                        break;
                    case ".ps":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.ps16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.ps32.png");
                        break;
                    case ".rar":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.rar16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.rar32.png");
                        break;
                    case ".sit":
                    case ".sitx":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.sit16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.sit32.png");
                        break;
                    case ".tif":
                    case ".tiff":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.tif16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.tif32.png");
                        break;
                    case ".txt":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.txt16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.txt32.png");
                        break;
                    case ".xls":
                    case ".xlsx":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.xls16.gif");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.xls32.gif");
                        break;
                    case ".wma":
                    case ".wmv":
                    case ".avi":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.wma16.gif");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.wma32.gif");
                        break;
                    case ".zip":
                        listitem.SmallImage = new IconResourceHandle("Icons.FileType.zip16.png");
                        listitem.LargeImage = new IconResourceHandle("Icons.FileType.zip32.png");
                        break;
                    default:
                        if (fileinfo.IsPDF)
                        {
                            listitem.SmallImage = new IconResourceHandle("Icons.FileType.pdf16.gif");
                            listitem.LargeImage = new IconResourceHandle("Icons.FileType.pdf32.gif");
                        }
                        else if (fileinfo.IsDocument)
                        {
                            listitem.SmallImage = new IconResourceHandle("Icons.FileType.doc16.gif");
                            listitem.LargeImage = new IconResourceHandle("Icons.FileType.doc32.gif");
                        }
                        else if (fileinfo.IsPresentation)
                        {
                            listitem.SmallImage = new IconResourceHandle("Icons.FileType.wma16.gif");
                            listitem.LargeImage = new IconResourceHandle("Icons.FileType.wma32.gif");
                        }
                        else if (fileinfo.IsSpreadsheet)
                        {
                            listitem.SmallImage = new IconResourceHandle("Icons.FileType.xls16.gif");
                            listitem.LargeImage = new IconResourceHandle("Icons.FileType.xls32.gif");
                        }
                        else
                        {
                            listitem.SmallImage = new IconResourceHandle("Icons.FileType.Unknown16.png");
                            listitem.LargeImage = new IconResourceHandle("Icons.FileType.Unknown32.png");
                        }
                        break;
                }
                #endregion

                listitem.SubItems.Add(fileinfo.Authors[0].Name);                                        // File Size
                listitem.SubItems.Add(contentType);                                                     // File Type
                listitem.SubItems.Add(fileinfo.Updated.ToString("yyyy-MM-dd HH:mm"));                   // Modified On
            }
        }

        private void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(DAL.Common.Config.CurrentWordDict, DAL.Common.Config.CurrentLanguageId);

            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        BindFileExplorer();
                        this.Update();
                        break;
                    case "columns":
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwFileExplorer);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        ListViewSortingOptions objListViewSortingOptions = new ListViewSortingOptions(this.lvwFileExplorer);
                        objListViewSortingOptions.ShowDialog();
                        break;
                    case "checkbox":
                        this.lvwFileExplorer.CheckBoxes = !this.lvwFileExplorer.CheckBoxes;
                        this.lvwFileExplorer.MultiSelect = this.lvwFileExplorer.CheckBoxes;
                        break;
                    case "multiselect":
                        this.lvwFileExplorer.MultiSelect = !this.lvwFileExplorer.MultiSelect;
                        e.Button.Pushed = true;
                        break;
                    case "dropfile":
                        #region upload file
                        fileUpload.Title = oDict.GetWord("upload_file");
                        //fileUpload.DefaultExt = "pdf";
                        //fileUpload.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                        //fileUpload.DefaultExt = "pdf";
                        fileUpload.Filter = "All files (*.*)|*.*";
                        fileUpload.MaxFileSize = Common.Config.MaxFileSize;
                        fileUpload.Multiselect = true;
                        fileUpload.ShowDialog();
                        #endregion
                        break;
                    case "retrievefile":
                        #region download file
                        if (lvwFileExplorer.CheckBoxes && lvwFileExplorer.CheckedIndices.Count > 0)
                        {
                            foreach (ListViewItem item in lvwFileExplorer.CheckedItems)
                            {
                                _filename = item.Text;
                                //Link.Open(new GatewayReference(this, "RetrieveFile"));
                                xPort5.Controls.GData.GDocs.DownloadFile(_filename);
                                string filePath = Path.Combine(DAL.Common.Config.OutBox, _filename);
                                if (File.Exists(filePath))
                                {
                                    xPort5.Controls.FileDownloadGateway dl = new xPort5.Controls.FileDownloadGateway();
                                    dl.Filename = _filename;
                                    dl.SetContentType(xPort5.Controls.DownloadContentType.OctetStream);
                                    dl.StartFileDownload(this, filePath);
                                }
                            }
                        }
                        else
                        {
                            if (lvwFileExplorer.SelectedIndex >= 0)
                            {
                                _filename = lvwFileExplorer.SelectedItem.Text;
                                //Link.Open(new GatewayReference(this, "RetrieveFile"));
                                xPort5.Controls.GData.GDocs.DownloadFile(_filename);
                                string filePath = Path.Combine(DAL.Common.Config.OutBox, _filename);
                                if (File.Exists(filePath))
                                {
                                    xPort5.Controls.FileDownloadGateway dl = new xPort5.Controls.FileDownloadGateway();
                                    dl.Filename = _filename;
                                    dl.SetContentType(xPort5.Controls.DownloadContentType.OctetStream);
                                    dl.StartFileDownload(this, filePath);
                                }
                            }
                        }
                        #endregion
                        break;
                    case "deletefile":
                        MessageBox.Show("Delete file?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                }
            }
        }

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            switch ((string)e.MenuItem.Tag)
            {
                case "Icon":
                    this.lvwFileExplorer.View = View.SmallIcon;
                    break;
                case "Tile":
                    this.lvwFileExplorer.View = View.LargeIcon;
                    break;
                case "List":
                    this.lvwFileExplorer.View = View.List;
                    break;
                case "Details":
                    this.lvwFileExplorer.View = View.Details;
                    break;
            }
        }

        #region ans Button Clicks: cmdDelete
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (DeleteFile())
                    {
                        MessageBox.Show("File deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("This file is protected...You can not delete this file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("File is Read Only...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region DeleteFile() Download()
        private bool DeleteFile()
        {
            string filename = String.Empty;
            string filepath = String.Empty;
            bool result = false;

            if (lvwFileExplorer.CheckBoxes && lvwFileExplorer.CheckedIndices.Count > 0)
            {
                foreach (ListViewItem item in lvwFileExplorer.CheckedItems)
                {
                    filename = item.Text;
                    xPort5.Controls.GData.GDocs.DeleteFile(filename);
                    item.Remove();
                    result = true;
                }
            }
            else
            {
                if (lvwFileExplorer.SelectedIndex >= 0)
                {
                    filename = lvwFileExplorer.SelectedItem.Text;
                    xPort5.Controls.GData.GDocs.DeleteFile(filename);
                    lvwFileExplorer.SelectedItem.Remove();
                    result = true;
                }
            }

            return result;
        }
        #endregion

        private void ShowDocuments()
        {
            if (ansFileExplorer.Buttons.Count == 0)
            {
                SetFileExplorerAns();
            }
            BindFileExplorer();
        }

        private void tvwClient_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Label.Length > 1)
            {
                _CurFolderText = e.Node.Text;
                _CurFolderUri = (string)e.Node.Tag;

                if (_CurFolderText != String.Empty)
                {
                    ShowDocuments();
                }
            }
        }

        private void lvwFileExplorer_DoubleClick(object sender, EventArgs e)
        {
            if (lvwFileExplorer.SelectedItem != null)
            {
                _filename = lvwFileExplorer.SelectedItem.Text;
                Link.Open(new GatewayReference(this, "RerieveFile"));
            }
        }

        private void fileUpload_FileOk(object sender, CancelEventArgs e)
        {
            string FileName = string.Empty;
            string FullName = string.Empty;
            string inbox = DAL.Common.Config.InBox;

            OpenFileDialog oFileDialog = sender as OpenFileDialog;

            switch (oFileDialog.DialogResult)
            {
                case DialogResult.OK:
                    for (int i = 0; i < oFileDialog.Files.Count; i++)
                    {
                        HttpPostedFileHandle file = oFileDialog.Files[i] as HttpPostedFileHandle;
                        if (file.ContentLength > 0)
                        {
                            FileName = Path.GetFileName(file.PostedFileName);
                            FullName = Path.Combine(inbox, FileName);
                            file.SaveAs(FullName);

//                            DocumentEntry entry = xPort5.Controls.GData.UploadFile3(FullName, FileName);
                            DocumentEntry entry = xPort5.Controls.GData.GDocs.UploadFile3(FullName, FileName, _CurFolderUri.Replace("documents", "folders"));
                        }
                    }
                    BindFileExplorer();
                    this.Update();
                    break;
            }
        }

        private void lvwFileExplorer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwFileExplorer.MultiSelect && lvwFileExplorer.CheckBoxes)
            {
                foreach (ListViewItem item in lvwFileExplorer.SelectedItems)
                {
                    item.Checked = true;
                }
            }

        }
    }
}