using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Interfaces;
using Gizmox.WebGUI.Common.Gateways;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Forms.Dialogs;

using MarkPasternak.Utility;

using xPort5.EF6;
using xPort5.Common;
using xPort5.Controls.Product;
using xPort5.Controls;
using xPort5.Helper;

namespace xPort5.Factory.Product
{
    public partial class ProductList : UserControl
    {
        nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

        private bool _IsPageLoaded = false;
        private int _ButtonIndex_Workshop = 0;
        private int _ButtonIndex_OrderType = 0;

        private enum PanelView { ListView, ImageView };

        private string _BaseSqlSelect = String.Empty;
        private string _BaseSqlWhere = String.Empty;
        private string _BaseSqlOrderBy = String.Empty;
        private string _CurSqlWhere = String.Empty;
        private string _CurSqlOrderBy = String.Empty;
        private string _CurWorkshop = String.Empty;
        private string _CurOrderType = String.Empty;

        public ProductList(Control toolBar)
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();
            SetAttribute();
            LoadComboBox();
            SetProductListAns();
            SetLvwList();

            _BaseSqlSelect = @"
SELECT TOP 100 PERCENT
       [ArticleId]
      ,[SKU]
      ,[ArticleCode]
      ,[ArticleName]
      ,[ArticleName_Chs]
      ,[ArticleName_Cht]
      ,[CategoryId]
      ,ISNULL([CategoryCode], '') AS 'CategoryCode'
      ,ISNULL([CategoryName], '') AS 'CategoryName'
      ,ISNULL([CategoryName_Chs], '') AS 'CategoryName_Chs'
      ,ISNULL([CategoryName_Cht], '') AS 'CategoryName_Cht'
      ,[ColorId]
      ,ISNULL([ColorCode], '') AS 'ColorCode'
      ,ISNULL([ColorName], '') AS 'ColorName'
      ,ISNULL([ColorName_Chs], '') AS 'ColorName_Chs'
      ,ISNULL([ColorName_Cht], '') AS 'ColorName_Cht'
      ,[OriginId]
      ,ISNULL([OriginCode], '') AS 'OriginCode'
      ,ISNULL([OriginName], '') AS 'OriginName'
      ,ISNULL([OriginName_Chs], '') As 'OriginName_Chs'
      ,ISNULL([OriginName_Cht], '') AS 'OriginName_Cht'
      ,[Remarks]
      ,ISNULL([ColorPattern], '') AS 'ColorPattern'
      ,[Barcode]
      ,[Status]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120) AS 'CreatedOn'
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120) AS 'ModifiedOn'
      ,[ModifiedBy]
  FROM [dbo].[vwProductList]
";
            if (xPort5.Controls.Utility.Staff.IsAccessAllowed(Common.Enums.UserGroup.Senior))
            {
                _BaseSqlWhere = String.Format("([Status] = {0}) ", Common.Enums.Status.Draft.ToString("d"));
            }
            else
            {
                _BaseSqlWhere = String.Format("([Status] = {0} AND [CreatedBy] = N'{1}')", Common.Enums.Status.Draft.ToString("d"), xPort5.Controls.Utility.Staff.GetAlias(Common.Config.CurrentUserId));
            }
            _BaseSqlOrderBy = "[ArticleCode]";
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;

            BindProductList();

            _IsPageLoaded = true;
        }

        #region Set Attributes, Themes
        private void SetCaptions()
        {
            lblLookup.Text = oDict.GetWordWithColon("lookup");
            lblViews.Text = oDict.GetWordWithColon("views");

            colProductCode.Text = oDict.GetWord("product_code");
            colDescription.Text = oDict.GetWord("product_description");
            colCategory.Text = oDict.GetWord("category");
            colColor.Text = oDict.GetWord("color");
            colOrigin.Text = oDict.GetWord("origin");
            colRemarks.Text = oDict.GetWord("remarks");
            colCreatedOn.Text = oDict.GetWord("created_on");
            colCreatedBy.Text = oDict.GetWord("created_by");
            colModifiedOn.Text = oDict.GetWord("modified_on");
            colModifiedBy.Text = oDict.GetWord("modified_by");
        }

        private void SetLvwList()
        {
            //this.lvwProductList.ListViewItemSorter = new ListViewItemSorter(this.lvwProductList);
            this.lvwProductList.ListViewItemSorter = new Sorter();   // 參考：https://stackoverflow.com/a/1214333

            this.lvwProductList.Dock = DockStyle.Fill;
            this.lvwProductList.GridLines = true;
            this.lvwProductList.UseInternalPaging = true;
            this.lvwProductList.ItemsPerPage = 25;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwProductList.Tag = new Guid("E0FB50E9-1C10-4c7a-BEB7-2E2E96C2EFE2");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwProductList);
        }

        private void SetAttribute()
        {
            //this.lvwProductList.ListViewItemSorter = new ListViewItemSorter(this.lvwProductList);
            this.lvwProductList.ListViewItemSorter = new Sorter();   // 參考：https://stackoverflow.com/a/1214333

            this.lvwProductList.Dock = DockStyle.Fill;

            toolTip1.SetToolTip(txtLookup, String.Format("Look for targets:{0}Product Code, Product Name, Remarks,{0}Created By, and Modified By", Environment.NewLine));
            toolTip1.SetToolTip(cmdLookup, String.Format("Look for targets:{0}Product Code, Product Name, Remarks,{0}Created By, and Modified By", Environment.NewLine));
            toolTip1.SetToolTip(lvwProductList, oDict.GetWord("double_click_to_open_record"));
        }

        private void LoadComboBox()
        {
            xPort5.Controls.Utility.ComboBox.LoadViews(ref cboViews);
        }
        #endregion

        #region Set Action Strip
        private void SetProductListAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansProductList.MenuHandle = false;
            this.ansProductList.DragHandle = false;
            this.ansProductList.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdButtons   - Buttons [0~3]
            this.ansProductList.Buttons.Add(new ToolBarButton("Columns", String.Empty));
            this.ansProductList.Buttons[0].Image = new IconResourceHandle("16x16.listview_columns.gif");
            this.ansProductList.Buttons[0].ToolTipText = @"Hide/Unhide Columns";
            this.ansProductList.Buttons.Add(new ToolBarButton("Sorting", String.Empty));
            this.ansProductList.Buttons[1].Image = new IconResourceHandle("16x16.listview_sorting.gif");
            this.ansProductList.Buttons[1].ToolTipText = @"Sorting";
            this.ansProductList.Buttons.Add(new ToolBarButton("Checkbox", String.Empty));
            this.ansProductList.Buttons[2].Image = new IconResourceHandle("16x16.listview_checkbox.gif");
            this.ansProductList.Buttons[2].ToolTipText = @"Toggle Checkbox";
            this.ansProductList.Buttons.Add(new ToolBarButton("MultiSelect", String.Empty));
            this.ansProductList.Buttons[3].Image = new IconResourceHandle("16x16.listview_multiselect.gif");
            this.ansProductList.Buttons[3].ToolTipText = @"Toggle Multi-Select";
            this.ansProductList.Buttons[3].Visible = false;
            #endregion

            this.ansProductList.Buttons.Add(sep);

            #region cmdViews    - Buttons[5]
            ContextMenu ddlViews = new ContextMenu();
            Common.Data.AppendMenuItem_AppViews(ref ddlViews);
            ToolBarButton cmdViews = new ToolBarButton("Views", oDict.GetWord("views"));
            cmdViews.Style = ToolBarButtonStyle.DropDownButton;
            cmdViews.Image = new IconResourceHandle("16x16.appView_xp.png");
            cmdViews.DropDownMenu = ddlViews;
            this.ansProductList.Buttons.Add(cmdViews);
            cmdViews.MenuClick += new MenuEventHandler(ansViews_MenuClick);
            #endregion

            #region cmdImageList    - Buttons[6]
            ContextMenu ddlImageList = new ContextMenu();
            Common.Data.AppendMenuItem_AppImageList(ref ddlImageList);
            ToolBarButton cmdImageList = new ToolBarButton("Images", oDict.GetWord("images"));
            cmdImageList.Style = ToolBarButtonStyle.DropDownButton;
            cmdImageList.Image = new IconResourceHandle("16x16.imagelist_duo_on_16.png");
            cmdImageList.DropDownMenu = ddlImageList;
            this.ansProductList.Buttons.Add(cmdImageList);
            cmdImageList.MenuClick += new MenuEventHandler(ansImageList_MenuClick);
            #endregion

            this.ansProductList.Buttons.Add(sep);

            #region cmdRefresh, cmdPreference       - Buttons[7~8]
            this.ansProductList.Buttons.Add(new ToolBarButton("Refresh", oDict.GetWord("refresh")));
            this.ansProductList.Buttons[8].Image = new IconResourceHandle("16x16.16_L_refresh.gif");
            this.ansProductList.ButtonClick += new ToolBarButtonClickEventHandler(ansProductList_ButtonClick);

            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansProductList.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            this.ansProductList.Buttons.Add(sep);

            #region filters
            /*
            #region cmdWorkshop     - Buttons [11]
            ContextMenu ddlWorkshop = new ContextMenu();

            Client_UserCollection oWorkshop = Common.Data.GetWorkshopList();
            if (oWorkshop.Count > 0)
            {
                for (int i = 0; i < oWorkshop.Count; i++)
                {
                    ddlWorkshop.MenuItems.Add(new MenuItem(oWorkshop[i].FullName));
                }
            }

            ToolBarButton cmdWorkshop = new ToolBarButton("Workshop", "Workshop");
            cmdWorkshop.Style = ToolBarButtonStyle.DropDownButton;
            cmdWorkshop.Image = new IconResourceHandle("16x16.filter_16.png");
            cmdWorkshop.DropDownMenu = ddlWorkshop;
            this.ansProductList.Buttons.Add(cmdWorkshop);
            cmdWorkshop.MenuClick += new MenuEventHandler(ansWorkshop_MenuClick);
            _ButtonIndex_Workshop = this.ansProductList.Buttons.Count - 1;
            #endregion

            #region cmdOrderType      - Button [12]
            ContextMenu ddlOrderType = new ContextMenu();
            Common.Data.AppendMenuItem_OrderType(ref ddlOrderType);
            ToolBarButton cmdOrderType = new ToolBarButton("OrderType", "Order Type");
            cmdOrderType.Style = ToolBarButtonStyle.DropDownButton;
            cmdOrderType.Image = new IconResourceHandle("16x16.filter_16.png");
            cmdOrderType.DropDownMenu = ddlOrderType;
            this.ansProductList.Buttons.Add(cmdOrderType);
            cmdOrderType.MenuClick += new MenuEventHandler(ansOrderType_MenuClick);
            _ButtonIndex_OrderType = this.ansProductList.Buttons.Count - 1;
            #endregion
*/
            #endregion

            //            this.ansProductList.Buttons.Add(sep);

            #region prepare command buttons
            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("delete"));
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            // cmdApprove
            ToolBarButton cmdApprove = new ToolBarButton("Approve", oDict.GetWord("approve"));
            cmdApprove.Tag = "Approve";
            cmdApprove.Image = new IconResourceHandle("16x16.approve.png");

            // cmdPrint
            ToolBarButton cmdPrint = new ToolBarButton("Print", oDict.GetWord("print"));
            cmdPrint.Tag = "Print";
            cmdPrint.Image = new IconResourceHandle("16x16.16_print.gif");

            // cmdUpload
            ToolBarButton cmdUpload = new ToolBarButton("Upload", oDict.GetWord("product_picture"));
            cmdUpload.Tag = "Upload";
            cmdUpload.Image = new IconResourceHandle("16x16.pictureOn16.png");
            //cmdUpload.Visible = false;
            #endregion

            #region add command buttons
            if (xPort5.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Factory", "Product.Delete"))
                    this.ansProductList.Buttons.Add(cmdDelete);
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Factory", "Product.Approve"))
                    this.ansProductList.Buttons.Add(cmdApprove);
            }
            else
            {
                this.ansProductList.Buttons.Add(cmdDelete);

                if (xPort5.Controls.Utility.Staff.IsAccessAllowed(Common.Enums.UserGroup.Senior))
                {
                    this.ansProductList.Buttons.Add(cmdApprove);
                }
            }
            this.ansProductList.Buttons.Add(cmdPrint);

            this.ansProductList.Buttons.Add(sep);
            this.ansProductList.Buttons.Add(cmdUpload);
            #endregion

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansProductList.Buttons.Add(cmdPopup);
            #endregion
        }
        #endregion

        #region Action Strip Clicks
        private void ansProductList_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        ResetForm();
                        BindProductList();
                        this.Update();
                        break;
                    case "columns":
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwProductList);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        ListViewSortingOptions objListViewSortingOptions = new ListViewSortingOptions(this.lvwProductList);
                        objListViewSortingOptions.ShowDialog();
                        break;
                    case "checkbox":
                        this.lvwProductList.CheckBoxes = !this.lvwProductList.CheckBoxes;

                        if (!this.lvwProductList.Visible)
                        {
                            BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                        }
                        break;
                    case "multiselect":
                        this.lvwProductList.MultiSelect = !this.lvwProductList.MultiSelect;
                        e.Button.Pushed = true;
                        break;
                    case "comment":
                        #region popup Order Comment
                        //if (lvwProductList.CheckBoxes && lvwProductList.CheckedIndices.Count > 0)
                        //{
                        //    foreach (ListViewItem item in lvwProductList.CheckedItems)
                        //    {
                        //        Comment oComment = new Comment();
                        //        oComment.OrderId = Convert.ToInt32(item.Text);
                        //        oComment.EditMode = Common.Enums.EditMode.Edit;
                        //        oComment.Show();
                        //    }
                        //}
                        //else
                        //{
                        //    if (lvwProductList.SelectedIndex >= 0)
                        //    {
                        //        Comment oComment = new Comment();
                        //        oComment.OrderId = Convert.ToInt32(lvwProductList.SelectedItem.Text);
                        //        oComment.EditMode = Common.Enums.EditMode.Edit;
                        //        oComment.Show();
                        //    }
                        //}
                        #endregion
                        break;
                    case "download":
                        #region download Attachment
                        if (lvwProductList.CheckBoxes && lvwProductList.CheckedIndices.Count > 0)
                        {
                            foreach (ListViewItem item in lvwProductList.CheckedItems)
                            {
                                Link.Open(new GatewayReference(this, item.Text));
                            }
                        }
                        else
                        {
                            if (lvwProductList.SelectedIndex >= 0)
                            {
                                Link.Open(new GatewayReference(this, lvwProductList.SelectedItem.Text));
                            }
                        }
                        #endregion
                        break;
                    case "billing":
                        #region popup billing form
                        //if (lvwProductList.CheckBoxes && lvwProductList.CheckedIndices.Count > 0)
                        //{
                        //    foreach (ListViewItem item in lvwProductList.CheckedItems)
                        //    {
                        //        Billing billing = new Billing();
                        //        billing.OrderId = Convert.ToInt32(item.Text);
                        //        billing.EditMode = Common.Invoice.GetEditMode(Convert.ToInt32(item.Text));
                        //        billing.Show();
                        //    }
                        //}
                        //else
                        //{
                        //    if (lvwProductList.SelectedIndex >= 0)
                        //    {
                        //        Billing billing = new Billing();
                        //        billing.OrderId = Convert.ToInt32(lvwProductList.SelectedItem.Text);
                        //        billing.EditMode = Common.Invoice.GetEditMode(Convert.ToInt32(lvwProductList.SelectedItem.Text));
                        //        billing.Show();
                        //    }
                        //}
                        #endregion
                        break;
                    case "delete":
                        MessageBox.Show("Are you sure to delete the selected records?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                    case "approve":
                        MessageBox.Show("Are you sure to approve the selected records?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdApprove_Click));
                        break;
                    case "print":
                        // popup Product List
                        xPort5.Controls.Reporting.XR.Loader.ProductList(BuildSqlQueryString());
                        break;
                    case "logfile":
                        #region popup invoice form
                        //if (lvwProductList.CheckBoxes && lvwProductList.CheckedIndices.Count > 0)
                        //{
                        //    foreach (ListViewItem item in lvwProductList.CheckedItems)
                        //    {
                        //        LogFile logfile = new LogFile();
                        //        logfile.OrderId = Convert.ToInt32(item.Text);
                        //        logfile.Show();
                        //    }
                        //}
                        //else
                        //{
                        //    if (lvwProductList.SelectedIndex >= 0)
                        //    {
                        //        LogFile logfile = new LogFile();
                        //        logfile.OrderId = Convert.ToInt32(lvwProductList.SelectedItem.Text);
                        //        logfile.Show();
                        //    }
                        //}
                        #endregion
                        break;
                    case "upload":
                        ShowImageManager();
                        break;
                    case "popup":
                        ShowRecord();
                        break;
                }
            }
        }

        private void ShowRecord()
        {
            if (lvwProductList.SelectedItem != null)
            {
                string productCode = lvwProductList.SelectedItem.Text;
                if (Common.Utility.IsGUID(lvwProductList.SelectedItem.SubItems[1].Text))
                {
                    Guid productId = new Guid(lvwProductList.SelectedItem.SubItems[1].Text);

                    ProductRecord oProductRecord = new ProductRecord();

                    oProductRecord.EditMode = Common.Enums.EditMode.Edit;
                    oProductRecord.ProductId = productId;
                    oProductRecord.ShowDialog();
                }
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwProductList);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwProductList);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        #region ans Button Clicks: Delete, Approve
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (lvwProductList.Visible)
                {
                    if (lvwProductList.CheckBoxes && lvwProductList.CheckedIndices.Count > 0)
                    {
                        foreach (ListViewItem item in lvwProductList.CheckedItems)
                        {
                            if (Common.Utility.IsGUID(item.SubItems[1].Text))
                            {
                                Guid productId = new Guid(item.SubItems[1].Text);

                                if (xPort5.Controls.Utility.Product.DeleteRec(productId))
                                {
                                    item.Remove();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lvwProductList.SelectedIndex >= 0)
                        {
                            if (Common.Utility.IsGUID(lvwProductList.SelectedItem.SubItems[1].Text))
                            {
                                Guid productId = new Guid(lvwProductList.SelectedItem.SubItems[1].Text);

                                if (xPort5.Controls.Utility.Product.DeleteRec(productId))
                                {
                                    lvwProductList.Items[lvwProductList.SelectedIndex].Remove();
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<Guid> selectedList = xPort5.Controls.Utility.ImagePanel.GetCheckedItems(flpImageList, xPort5.Controls.Utility.ImagePanel.CheckedType.Product);
                    if (selectedList.Count > 0)
                    {
                        foreach (System.Guid productId in selectedList)
                        {
                            xPort5.Controls.Utility.Product.DeleteRec(productId);
                        }
                    }

                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                }
            }
        }

        private void cmdApprove_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (lvwProductList.Visible)
                {
                    if (lvwProductList.CheckBoxes && lvwProductList.CheckedIndices.Count > 0)
                    {
                        foreach (ListViewItem item in lvwProductList.CheckedItems)
                        {
                            if (Common.Utility.IsGUID(item.SubItems[1].Text))
                            {
                                Guid productId = new Guid(item.SubItems[1].Text);

                                if (xPort5.Controls.Utility.Product.ApproveRec(productId))
                                {
                                    item.Remove();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lvwProductList.SelectedIndex >= 0)
                        {
                            if (Common.Utility.IsGUID(lvwProductList.SelectedItem.SubItems[1].Text))
                            {
                                Guid productId = new Guid(lvwProductList.SelectedItem.SubItems[1].Text);

                                if (xPort5.Controls.Utility.Product.ApproveRec(productId))
                                {
                                    lvwProductList.Items[lvwProductList.SelectedIndex].Remove();
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<Guid> selectedList = xPort5.Controls.Utility.ImagePanel.GetCheckedItems(flpImageList, xPort5.Controls.Utility.ImagePanel.CheckedType.Product);
                    if (selectedList.Count > 0)
                    {
                        foreach (System.Guid productId in selectedList)
                        {
                            xPort5.Controls.Utility.Product.ApproveRec(productId);
                        }
                    }

                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                }
            }
        }
        #endregion

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            SwitchPanel(PanelView.ListView);

            switch ((string)e.MenuItem.Tag)
            {
                case "Icon":
                    this.lvwProductList.View = View.SmallIcon;
                    break;
                case "Tile":
                    this.lvwProductList.View = View.LargeIcon;
                    break;
                case "List":
                    this.lvwProductList.View = View.List;
                    break;
                case "Details":
                    this.lvwProductList.View = View.Details;
                    break;
            }
        }

        private void ansImageList_MenuClick(object sender, MenuItemEventArgs e)
        {
            SwitchPanel(PanelView.ImageView);

            if (e.MenuItem.Tag != null)
            {
                switch ((string)e.MenuItem.Tag.ToString().ToLower())
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
                        BindImageList(xPort5.Controls.Utility.Resources.ImageSize.XLarge, true);
                        break;
                }
            }
            else
            {
                BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
            }
        }

        #region retired code
        /*
        private void ansWorkshop_MenuClick(object sender, MenuItemEventArgs e)
        {
            // show he selected Workshop as Ans Button text
            ToolBarButton oSender = (ToolBarButton)sender;
            oSender.Text = e.MenuItem.Text;
            _CurWorkshop = e.MenuItem.Text;
            if (String.IsNullOrEmpty(_CurOrderType))
            {
                _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ([Workshop] = N'{0}')", _CurWorkshop);
            }
            else
            {
                _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ([Workshop] = N'{0}' AND [OrderType] = '{1}')", _CurWorkshop, _CurOrderType);
            }
            BindProductList();
            this.Update();
        }

        private void ansOrderType_MenuClick(object sender, MenuItemEventArgs e)
        {
            // show he selected Workshop as Ans Button text
            ToolBarButton oSender = (ToolBarButton)sender;
            oSender.Text = e.MenuItem.Text;
            _CurOrderType = e.MenuItem.Text;
            if (String.IsNullOrEmpty(_CurWorkshop))
            {
                _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ([OrderType] = '{0}')", _CurOrderType);
            }
            else
            {
                _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ([Workshop] = N'{0}' AND [OrderType] = '{1}')", _CurWorkshop, _CurOrderType);
            }
            BindProductList();
            this.Update();
        }
        */
        #endregion
        #endregion

        #region Bind Product List, Bind Image List
        private void BindProductList()
        {
            this.lvwProductList.Items.Clear();

            int iCount = 1;

            string whereClause = _CurSqlWhere;
            string orderBy = _CurSqlOrderBy.Replace("ORDER BY ", "");

            DataSet ds = ViewService.Default.GetProductList(whereClause, orderBy);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                Guid productId = (Guid)row["ArticleId"];

                ListViewItem objItem = this.lvwProductList.Items.Add(row["ArticleCode"].ToString());  // Product Code
                #region Product Image
                if (File.Exists(xPort5.Controls.Utility.Resources.PictureFilePath(productId, xPort5.Controls.Utility.Product.KeyPicture(productId))))
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.pictureOn16.png");
                    objItem.LargeImage = new IconResourceHandle("32x32.pictureOn32.png");
                }
                else
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.pictureOff16.png");
                    objItem.LargeImage = new IconResourceHandle("32x32.pictureOff32.png");
                }
                #endregion
                objItem.SubItems.Add(productId.ToString()); // Product Id (hidden column)
                #region Status Icon
                switch (Convert.ToInt32(row["Status"]))
                {
                    case (int)Common.Enums.Status.Draft:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.flag_lightgrey.png").ToString());
                        break;
                    case (int)Common.Enums.Status.Active:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.flag_green.png").ToString());
                        break;
                    case (int)Common.Enums.Status.Inactive:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.flag_dark.png").ToString());
                        break;
                }
                #endregion
                #region Supplier Icon
                ArticleSupplierCollection suppliers = ArticleSupplier.LoadCollection(String.Format("ArticleId = '{0}'", productId.ToString()));
                switch (suppliers.Count)
                {
                    case 0:
                        //                        objItem.SubItems.Add(new IconResourceHandle("16x16.supplierSingleLightGrey_16.gif").ToString());
                        objItem.SubItems.Add(String.Empty);
                        break;
                    case 1:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.supplierSingle_16.gif").ToString());
                        break;
                    default:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.supplierDouble_16.gif").ToString());
                        break;
                }
                #endregion
                #region Package Icon
                ArticlePackageCollection packages = ArticlePackage.LoadCollection(String.Format("ArticleId = '{0}'", productId.ToString()));
                switch (packages.Count)
                {
                    case 0:
                        //                        objItem.SubItems.Add(new IconResourceHandle("16x16.packageLightGrey_16.png").ToString());
                        objItem.SubItems.Add(String.Empty);
                        break;
                    case 1:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.package_16.png").ToString());
                        break;
                    default:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.packageDouble_16.png").ToString());
                        break;
                }
                #endregion
                objItem.SubItems.Add(iCount.ToString());    // Line Number
                objItem.SubItems.Add(row["ArticleName"].ToString());  // Product Name
                objItem.SubItems.Add(row["CategoryName"].ToString());  // Category
                
                string colorPattern = row["ColorPattern"].ToString();
                if (!string.IsNullOrEmpty(colorPattern))
                    objItem.SubItems.Add(colorPattern); // Multi-Colors
                else
                    objItem.SubItems.Add(row["ColorName"].ToString()); // Single Color
                    
                objItem.SubItems.Add(row["OriginName"].ToString()); // Origin
                objItem.SubItems.Add(row["Remarks"].ToString()); // Remarks
                objItem.SubItems.Add(row["CreatedOn"].ToString()); // Created On
                objItem.SubItems.Add(row["CreatedBy"].ToString()); // Created By
                objItem.SubItems.Add(row["ModifiedOn"].ToString()); // Modified On
                objItem.SubItems.Add(row["ModifiedBy"].ToString()); // Modified By

                iCount++;
            }

            this.lvwProductList.Sort();
        }

        private string BuildSqlQueryString()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"
SELECT TOP 100 PERCENT
       [ArticleId]
       ,[SKU]
       ,[ArticleCode]
       ,[ArticleName]
       ,[ArticleName_Chs]
       ,[ArticleName_Cht]
       ,[CategoryId]
       ,ISNULL([CategoryCode], '') AS 'CategoryCode'
       ,ISNULL([CategoryName], '') AS 'CategoryName'
       ,ISNULL([CategoryName_Chs], '') AS 'CategoryName_Chs'
       ,ISNULL([CategoryName_Cht], '') AS 'CategoryName_Cht'
       ,[ColorId]
       ,ISNULL([ColorCode], '') AS 'ColorCode'
       ,ISNULL([ColorName], '') AS 'ColorName'
       ,ISNULL([ColorName_Chs], '') AS 'ColorName_Chs'
       ,ISNULL([ColorName_Cht], '') AS 'ColorName_Cht'
       ,[OriginId]
       ,ISNULL([OriginCode], '') AS 'OriginCode'
       ,ISNULL([OriginName], '') AS 'OriginName'
       ,ISNULL([OriginName_Chs], '') As 'OriginName_Chs'
       ,ISNULL([OriginName_Cht], '') AS 'OriginName_Cht'
       ,[Remarks]
       ,ISNULL([ColorPattern], '') AS 'ColorPattern'
       ,[Barcode]
       ,[Status]
       ,CONVERT(NVARCHAR(16), [CreatedOn], 120) AS 'CreatedOn'
       ,[CreatedBy]
       ,CONVERT(NVARCHAR(16), [ModifiedOn], 120) AS 'ModifiedOn'
       ,[ModifiedBy]
  FROM [dbo].[vwProductList]
" + Environment.NewLine);

            string where = _CurSqlWhere;
            if (!string.IsNullOrEmpty(where) && !where.TrimStart().StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
            {
                where = "WHERE " + where;
            }
            sql.Append(where + Environment.NewLine);

            string orderBy = _CurSqlOrderBy;
            if (!string.IsNullOrEmpty(orderBy) && !orderBy.TrimStart().StartsWith("ORDER BY", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "ORDER BY " + orderBy;
            }
            sql.Append(orderBy);

            return sql.ToString();
        }

        private void BindImageList(Size imageSize, bool inDetail)
        {
            this.flpImageList.Controls.Clear();

            string whereClause = _CurSqlWhere;
            string orderBy = _CurSqlOrderBy.Replace("ORDER BY ", "");

            DataSet ds = ViewService.Default.GetProductList(whereClause, orderBy);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                Guid productId = (Guid)row["ArticleId"];

                ImagePanel imgItem = new ImagePanel(imageSize, productId, inDetail, System.Guid.Empty, System.Guid.Empty, string.Empty, this.lvwProductList.CheckBoxes, false);
                imgItem.DoubleClick += new EventHandler(imgItem_DoubleClick);

                flpImageList.Controls.Add(imgItem);
            }
        }

        void imgItem_DoubleClick(object sender, EventArgs e)
        {
            xPort5.Controls.ProductImage prodImage = (xPort5.Controls.ProductImage)sender;

            ProductRecord oProductRecord = new ProductRecord();

            oProductRecord.EditMode = Common.Enums.EditMode.Edit;
            oProductRecord.ProductId = prodImage.ProductId;
            oProductRecord.ShowDialog();
        }

        #endregion

        private void SwitchPanel(PanelView view)
        {
            switch ((int)view)
            {
                case (int)PanelView.ListView:
                    flpImageList.Controls.Clear();
                    flpImageList.Dock = DockStyle.None;
                    flpImageList.Visible = false;

                    lvwProductList.Dock = DockStyle.Fill;
                    lvwProductList.Visible = true;
                    BindProductList();
                    break;
                case (int)PanelView.ImageView:
                    lvwProductList.Items.Clear();
                    lvwProductList.Dock = DockStyle.None;
                    lvwProductList.Visible = false;

                    flpImageList.Controls.Clear();
                    flpImageList.AutoScroll = true;
                    flpImageList.Dock = DockStyle.Fill;
                    flpImageList.BackColor = Color.White;
                    flpImageList.FlowDirection = FlowDirection.LeftToRight;
                    flpImageList.Visible = true;
                    break;
            }
        }

        private void ResetForm()
        {
            txtLookup.Text = String.Empty;
            cboViews.SelectedIndex = 0;

            //ansProductList.Buttons[_ButtonIndex_Workshop].Text = "Workshop";
            //ansProductList.Buttons[_ButtonIndex_OrderType].Text = "Order Type";

            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
            _CurWorkshop = String.Empty;
            _CurOrderType = String.Empty;
        }

        private void DoLookup()
        {
            string target = txtLookup.Text.Trim();
            if (!(String.IsNullOrEmpty(target)))
            {
                ResetForm();
                txtLookup.Text = target;
                _CurSqlWhere = _BaseSqlWhere +
                               String.Format(" AND ([ArticleCode] LIKE '%{0}%' OR [ArticleName] LIKE N'%{0}%' OR [Remarks] LIKE N'%{0}%' OR [CreatedBy] LIKE '%{0}%' OR [ModifiedBy] LIKE N'%{0}%')", target);
                BindProductList();
                this.Update();
            }
        }

        private void cboViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboViews.SelectedIndex >= 0 && _IsPageLoaded)
            {
                string filter = String.Empty;
                switch (cboViews.SelectedIndex)
                {
                    case 1:         // 7 dyas
                        filter = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [ModifiedOn], 120) >= '{0}')", filter);
                        break;
                    case 2:         // 30 days
                        filter = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [ModifiedOn], 120) >= '{0}')", filter);
                        break;
                    case 3:         // 60 days
                        filter = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [ModifiedOn], 120) >= '{0}')", filter);
                        break;
                    case 4:         // 90 days
                        filter = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [ModifiedOn], 120) >= '{0}')", filter);
                        break;
                    case 0:
                    default:
                        _CurSqlWhere = _BaseSqlWhere;
                        break;
                }
                BindProductList();
                txtLookup.Text = String.Empty;
                this.Update();
            }
        }

        private void cmdLookup_Click(object sender, EventArgs e)
        {
            DoLookup();
        }

        private void txtLookup_EnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            DoLookup();
        }

        private void lvwProductList_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }

        private void lvwProductList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 參考：https://stackoverflow.com/a/1214333
            Sorter s = (Sorter)lvwProductList.ListViewItemSorter;
            s.Column = e.Column;

            if (s.Order == Gizmox.WebGUI.Forms.SortOrder.Ascending)
            {
                s.Order = Gizmox.WebGUI.Forms.SortOrder.Descending;
            }
            else
            {
                s.Order = Gizmox.WebGUI.Forms.SortOrder.Ascending;
            }
            lvwProductList.Sort();
        }

        private void ShowImageManager()
        {
            Guid productId = System.Guid.Empty;

            if (lvwProductList.Visible)
            {
                if (lvwProductList.SelectedItem != null)
                {
                    if (Common.Utility.IsGUID(lvwProductList.SelectedItem.SubItems[1].Text))
                    {
                        productId = new Guid(lvwProductList.SelectedItem.SubItems[1].Text);

                    }
                }
            }
            else
            {
                List<Guid> selectedList = xPort5.Controls.Utility.ImagePanel.GetCheckedItems(flpImageList, xPort5.Controls.Utility.ImagePanel.CheckedType.Product);
                if (selectedList.Count > 0 && selectedList.Count <= 1)
                {
                    productId = selectedList[0];
                }
            }

            if (productId != System.Guid.Empty)
            {
                ImageManager imageMan = new ImageManager();
                imageMan.ProductId = productId;
                imageMan.ShowDialog();
            }
        }
    }
}
