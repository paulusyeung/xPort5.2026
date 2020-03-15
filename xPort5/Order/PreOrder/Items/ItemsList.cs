#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using xPort5.DAL;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms.Dialogs;
using System.Data.SqlClient;
using xPort5.Controls.Product;
using xPort5.Controls;

#endregion

namespace xPort5.Order.PreOrder.Items
{
    public partial class ItemsList : UserControl
    {
        private enum PanelView { ListView, ImageView };

        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _OrderPLId = System.Guid.Empty;
        private string _PLNumber = string.Empty;

        #region public properties
        public Common.Enums.EditMode EditMode
        {
            get
            {
                return _EditMode;
            }
            set
            {
                _EditMode = value;
            }
        }

        public Guid OrderPLId
        {
            get
            {
                return _OrderPLId;
            }
            set
            {
                _OrderPLId = value;
            }
        }
        #endregion

        public ItemsList()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttribute();
            SetListAns();
            SetLvwList();

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                BindList();
            }
        }

        private void SetAttribute()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.DAL.Common.Config.CurrentWordDict, xPort5.DAL.Common.Config.CurrentLanguageId);

            this.lvwItems.ListViewItemSorter = new ListViewItemSorter(this.lvwItems);
            this.lvwItems.Dock = DockStyle.Fill;

            toolTip1.SetToolTip(this.lvwItems, oDict.GetWord("double_click_to_open_record"));

            this.colItemCode.Text = oDict.GetWord("product_code");
            this.colSupplier.Text = oDict.GetWord("supplier");
            this.colPackage.Text = oDict.GetWord("package");
            this.colDescription.Text = oDict.GetWord("product_description");
            this.colParticular.Text = oDict.GetWord("particular");
            this.colCustomerRef.Text = oDict.GetWord("customer_ref");
            this.colFactoryCost.Text = oDict.GetWord("factory_cost");
            this.colMargin.Text = oDict.GetWord("margin");
            this.colFCLAmount.Text = oDict.GetWord("FCL Amount");
            this.colLCLAmount.Text = oDict.GetWord("LCL Amount");
            this.colSupplierRef.Text = oDict.GetWord("supplier_ref");
            this.colCurrency.Text = oDict.GetWord("currency");
            this.colFCLCost.Text = oDict.GetWord("FCL cost");
            this.colLCLCost.Text = oDict.GetWord("LCL Cost");
            this.colInnerBox.Text = oDict.GetWord("Inner Box");
            this.colOuterBox.Text = oDict.GetWord("Outer Box");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colAmount.Text = oDict.GetWord("amount");
        }

        private void SetLvwList()
        {
            this.lvwItems.ListViewItemSorter = new ListViewItemSorter(this.lvwItems);
            this.lvwItems.Dock = DockStyle.Fill;
            this.lvwItems.GridLines = true;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwItems.Tag = new Guid("8F9BB42A-05B5-49c5-B91A-D26EFFB551E1");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwItems);
        }

        private void SetListAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.DAL.Common.Config.CurrentWordDict, xPort5.DAL.Common.Config.CurrentLanguageId);

            this.ansItems.MenuHandle = false;
            this.ansItems.DragHandle = false;
            this.ansItems.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdButtons   - Buttons [0~3]
            this.ansItems.Buttons.Add(new ToolBarButton("Columns", String.Empty));
            this.ansItems.Buttons[0].Image = new IconResourceHandle("16x16.listview_columns.gif");
            this.ansItems.Buttons[0].ToolTipText = oDict.GetWord(@"Hide/Unhide Columns");
            this.ansItems.Buttons.Add(new ToolBarButton("Sorting", String.Empty));
            this.ansItems.Buttons[1].Image = new IconResourceHandle("16x16.listview_sorting.gif");
            this.ansItems.Buttons[1].ToolTipText = oDict.GetWord(@"Sorting");
            this.ansItems.Buttons.Add(new ToolBarButton("Checkbox", String.Empty));
            this.ansItems.Buttons[2].Image = new IconResourceHandle("16x16.listview_checkbox.gif");
            this.ansItems.Buttons[2].ToolTipText = oDict.GetWord(@"Toggle Checkbox");
            this.ansItems.Buttons.Add(new ToolBarButton("MultiSelect", String.Empty));
            this.ansItems.Buttons[3].Image = new IconResourceHandle("16x16.listview_multiselect.gif");
            this.ansItems.Buttons[3].ToolTipText = oDict.GetWord(@"Toggle Multi-Select");
            this.ansItems.Buttons[3].Visible = false;
            #endregion

            this.ansItems.Buttons.Add(sep);

            #region cmdViews    - Buttons[5]
            ContextMenu ddlViews = new ContextMenu();
            Common.Data.AppendMenuItem_AppViews(ref ddlViews);
            ToolBarButton cmdViews = new ToolBarButton("Views", oDict.GetWord("View"));
            cmdViews.Style = ToolBarButtonStyle.DropDownButton;
            cmdViews.Image = new IconResourceHandle("16x16.appView_xp.png");
            cmdViews.DropDownMenu = ddlViews;
            this.ansItems.Buttons.Add(cmdViews);
            cmdViews.MenuClick += new MenuEventHandler(ansViews_MenuClick);
            #endregion

            #region cmdImageList    - Buttons[6]
            ContextMenu ddlImageList = new ContextMenu();
            Common.Data.AppendMenuItem_AppImageList(ref ddlImageList);
            ToolBarButton cmdImageList = new ToolBarButton("Images", oDict.GetWord("Images"));
            cmdImageList.Style = ToolBarButtonStyle.DropDownButton;
            cmdImageList.Image = new IconResourceHandle("16x16.imagelist_duo_on_16.png");
            cmdImageList.DropDownMenu = ddlImageList;
            this.ansItems.Buttons.Add(cmdImageList);
            cmdImageList.MenuClick += new MenuEventHandler(ansImageList_MenuClick);
            #endregion

            this.ansItems.Buttons.Add(sep);

            #region cmdRefresh, cmdPreference       - Buttons[7~8]
            this.ansItems.Buttons.Add(new ToolBarButton("Refresh", oDict.GetWord("Refresh")));
            this.ansItems.Buttons[8].Image = new IconResourceHandle("16x16.16_L_refresh.gif");
            this.ansItems.ButtonClick += new ToolBarButtonClickEventHandler(ansItems_ButtonClick);

            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansItems.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            this.ansItems.Buttons.Add(sep);

            // cmdNew
            ToolBarButton cmdNew = new ToolBarButton("New", oDict.GetWord("New"));
            cmdNew.Tag = "New";
            cmdNew.Image = new IconResourceHandle("16x16.ico_16_3.gif");
            if (_EditMode == Common.Enums.EditMode.Read || _EditMode == Common.Enums.EditMode.Add)
                cmdNew.Enabled = false;

            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("Delete"));
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            #region add cmdNew
            if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PreOrderList.Create"))
                {
                    this.ansItems.Buttons.Add(cmdNew);
                }
            }
            else
            {
                this.ansItems.Buttons.Add(cmdNew);
            }
            #endregion

            #region add cmdDelete
            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PreOrderList.Delete"))
                    {
                        this.ansItems.Buttons.Add(cmdDelete);
                    }
                }
                else
                {
                    this.ansItems.Buttons.Add(cmdDelete);
                }
            }
            #endregion

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansItems.Buttons.Add(cmdPopup);
            #endregion

            //            this.ansItems.ButtonClick += new ToolBarButtonClickEventHandler(ansItem_ButtonClick);
        }

        private string BuildSql()
        {
            string sql = String.Format(@"
SELECT TOP 100 PERCENT
       [OrderPLId]          -- 0
      ,[PLNumber]
      ,[PLDate]
      ,[OrderPLItemsId]
      ,[LineNumber]
      ,[ArticleId]          -- 5
      ,[SKU]
      ,[ArticleCode]
      ,[ArticleName]
      ,[ArticleName_Chs]
      ,[ArticleName_Cht]    -- 10
      ,[PackageId]
      ,[PackageCode]
      ,[PackageName]
      ,[PackageName_Chs]
      ,[PackageName_Cht]    -- 15
      ,[SupplierId]
      ,[SupplierCode]
      ,[SupplierName]
      ,[SupplierName_Chs]
      ,[SupplierName_Cht]   -- 20
      ,[Particular]
      ,[CustRef]
      ,[PriceType]
      ,[FactoryCost]
      ,[Margin]             -- 25
      ,[FCL]
      ,[LCL]
      ,[SampleQty]
      ,[Qty]
      ,[Unit]               -- 30
      ,[Amount]
      ,[Carton]
      ,[GLAccount]
      ,[RefDocNo]
      ,[ShippingMark]       -- 35
      ,[QtyIN]
      ,[QtyOUT]
      ,[SuppRef]
      ,[CurrencyCode]
      ,[FCLCost]            -- 40
      ,[LCLCost]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
FROM [dbo].[vwPreOrderItemList]
WHERE [OrderPLId] = '{0}'
ORDER BY [LineNumber]
", _OrderPLId.ToString());

            return sql;
        }

        private void BindList()
        {
            this.lvwItems.Items.Clear();

            int iCount = 1;

            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, BuildSql());

            while (reader.Read())
            {
                Guid itemId = reader.GetGuid(3);
                OrderQTItems item = OrderQTItems.Load(itemId);

                ListViewItem objItem = this.lvwItems.Items.Add(reader.GetString(7));  // Product Code
                #region Product Image
                //if (cAddress.DefaultRec)
                //{
                objItem.SmallImage = new IconResourceHandle("16x16.pumpkin_16.png");
                objItem.LargeImage = new IconResourceHandle("32x32.pumpkin_32.png");
                //}
                //else
                //{
                //    objItem.SmallImage = new IconResourceHandle("16x16.addresssingle_16.png");
                //    objItem.LargeImage = new IconResourceHandle("16x16.addresssingle_16.png");
                //}
                #endregion
                objItem.SubItems.Add(reader.GetGuid(3).ToString());     // Items Id
                objItem.SubItems.Add(iCount.ToString());                // Line Number
                objItem.SubItems.Add(xPort5.Controls.Utility.Supplier.GetSupplierName(reader.GetGuid(16)));             // Supplier
                objItem.SubItems.Add(reader.GetString(13));             // Package
                objItem.SubItems.Add(reader.GetString(8));              // Product Name
                objItem.SubItems.Add(reader.GetString(21));             // Particular
                objItem.SubItems.Add(reader.GetString(22));             // Cust. Ref.
                objItem.SubItems.Add(reader.GetDecimal(28).ToString("##0"));                // Sample Qty
                objItem.SubItems.Add(reader.GetDecimal(24).ToString("#,##0.0000"));         // Factory Cost
                objItem.SubItems.Add(reader.GetDecimal(25).ToString("##0.00"));             // Margin
                objItem.SubItems.Add(reader.GetDecimal(26).ToString("#,##0.0000"));         // FCL Amount
                objItem.SubItems.Add(reader.GetDecimal(27).ToString("#,##0.0000"));         // LCL Amount
                objItem.SubItems.Add(reader.GetString(38));                                 // Supplier Ref.
                objItem.SubItems.Add(reader.GetString(39));                                 // Currency
                objItem.SubItems.Add(reader.GetDecimal(40).ToString("#,##0.0000"));         // FCL Cost
                objItem.SubItems.Add(reader.GetDecimal(41).ToString("#,##0.0000"));         // LCL Cost
                objItem.SubItems.Add(reader.GetDecimal(42).ToString("##0.00"));             // Inner Box
                objItem.SubItems.Add(reader.GetDecimal(43).ToString("##0.00"));             // Outer Box
                objItem.SubItems.Add(reader.GetString(30));                                 // Unit
                objItem.SubItems.Add(reader.GetDecimal(44).ToString("##0.00"));             // CUFT
                objItem.SubItems.Add(reader.GetDecimal(31).ToString("#,##0.0000"));         // Amount

                iCount++;
            }
            reader.Close();

            this.lvwItems.Sort();
        }


        private void BindImageList(Size imageSize, bool inDetail)
        {
            this.flpImageList.Controls.Clear();

            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, BuildSql());

            while (reader.Read())
            {
                Guid itemId = reader.GetGuid(3);
                Guid articleId = reader.GetGuid(5);

                ImagePanel imgItem = new ImagePanel(imageSize, articleId, inDetail, itemId, System.Guid.Empty, string.Empty, lvwItems.CheckBoxes, false);
                imgItem.DoubleClick += new EventHandler(imgItem_DoubleClick);
                flpImageList.Controls.Add(imgItem);
            }
            reader.Close();
        }

        void imgItem_DoubleClick(object sender, EventArgs e)
        {
            xPort5.Controls.ProductImage prodImage = (xPort5.Controls.ProductImage)sender;

            OrderPLItems item = OrderPLItems.LoadWhere(String.Format("OrderPLItemsId = '{0}'", prodImage.Name));
            if (item != null)
            {
                EditItemRecord itemRecord = new EditItemRecord();

                itemRecord.EditMode = Common.Enums.EditMode.Edit;
                itemRecord.OrderPLId = item.OrderPLId;
                itemRecord.OrderItemId = item.OrderPLItemsId;
                itemRecord.ArticleCode = prodImage.Tag.ToString();
                itemRecord.ShowDialog();
            }
        }

        #region Action Strip Clicks
        private void ansItems_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        BindList();
                        this.Update();
                        break;
                    case "columns":
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwItems);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        ListViewSortingOptions objListViewSortingOptions = new ListViewSortingOptions(this.lvwItems);
                        objListViewSortingOptions.ShowDialog();
                        break;
                    case "checkbox":
                        this.lvwItems.CheckBoxes = !this.lvwItems.CheckBoxes;

                        if (!lvwItems.Visible)
                        {
                            BindImageList(Utility.Resources.ImageSize.Medium, false);
                        }
                        break;
                    case "multiselect":
                        this.lvwItems.MultiSelect = !this.lvwItems.MultiSelect;
                        e.Button.Pushed = true;
                        break;
                    case "delete":
                        MessageBox.Show("Are you sure to delete the selected records?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                    case "new":
                        AddItemRecord item = new AddItemRecord();
                        item.EditMode = Common.Enums.EditMode.Add;
                        item.OrderId = _OrderPLId;
                        item.LineNumber = lvwItems.Items.Count + 1;
                        item.Closed += new EventHandler(item_Closed);
                        item.ShowDialog();
                        break;
                    case "popup":
                        ShowRecord();
                        break;
                }
            }
        }

        private void ShowRecord()
        {
            if (lvwItems.SelectedItem != null)
            {
                string productCode = lvwItems.SelectedItem.Text;
                if (Common.Utility.IsGUID(lvwItems.SelectedItem.SubItems[1].Text))
                {
                    Guid itemId = new Guid(lvwItems.SelectedItem.SubItems[1].Text);

                    EditItemRecord item = new EditItemRecord();

                    item.EditMode = Common.Enums.EditMode.Edit;
                    item.OrderPLId = _OrderPLId;
                    item.OrderItemId = itemId;
                    item.ArticleCode = lvwItems.SelectedItem.Text;
                    item.ShowDialog();
                }
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwItems);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwItems);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        void item_Closed(object sender, EventArgs e)
        {
            if (sender is AddItemRecord)
            {
                BindList();
                this.Update();
            }
        }

        #region ans Button Clicks: Delete, Approve
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (lvwItems.Visible)
                {
                    if (lvwItems.CheckBoxes && lvwItems.CheckedIndices.Count > 0)
                    {
                        foreach (ListViewItem item in lvwItems.CheckedItems)
                        {
                            if (Common.Utility.IsGUID(item.SubItems[1].Text))
                            {
                                Guid itemId = new Guid(item.SubItems[1].Text);

                                if (xPort5.Controls.Utility.OrderPL.DeleteItem(itemId))
                                {
                                    item.Remove();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lvwItems.SelectedIndex >= 0)
                        {
                            if (Common.Utility.IsGUID(lvwItems.SelectedItem.SubItems[1].Text))
                            {
                                Guid itemId = new Guid(lvwItems.SelectedItem.SubItems[1].Text);

                                if (xPort5.Controls.Utility.OrderPL.DeleteItem(itemId))
                                {
                                    lvwItems.Items[lvwItems.SelectedIndex].Remove();
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<Guid> selectedList = Utility.ImagePanel.GetCheckedItems(flpImageList, Utility.ImagePanel.CheckedType.Order);
                    if (selectedList.Count > 0)
                    {
                        foreach (System.Guid plItemId in selectedList)
                        {
                            xPort5.Controls.Utility.OrderPL.DeleteItem(plItemId);
                        }
                    }

                    BindImageList(Utility.Resources.ImageSize.Medium, false);
                }
            }
        }
        #endregion

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            SwitchPanel(PanelView.ListView);
            BindList();

            switch (e.MenuItem.Tag.ToString())
            {
                case "Icon":
                    this.lvwItems.View = View.SmallIcon;
                    break;
                case "Tile":
                    this.lvwItems.View = View.LargeIcon;
                    break;
                case "List":
                    this.lvwItems.View = View.List;
                    break;
                case "Details":
                    this.lvwItems.View = View.Details;
                    break;
            }
        }

        private void ansImageList_MenuClick(object sender, MenuItemEventArgs e)
        {
            SwitchPanel(PanelView.ImageView);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Small":
                    BindImageList(Utility.Resources.ImageSize.Small, false);
                    break;
                case "Medium":
                    BindImageList(Utility.Resources.ImageSize.Medium, false);
                    break;
                case "Large":
                    BindImageList(Utility.Resources.ImageSize.Large, false);
                    break;
                case "Details":
                    BindImageList(Utility.Resources.ImageSize.XLarge, true);
                    break;
            }
        }

        private void SwitchPanel(PanelView view)
        {
            switch ((int)view)
            {
                case (int)PanelView.ListView:
                    flpImageList.Controls.Clear();
                    flpImageList.Dock = DockStyle.None;
                    flpImageList.Visible = false;

                    lvwItems.Dock = DockStyle.Fill;
                    lvwItems.Visible = true;
                    break;
                case (int)PanelView.ImageView:
                    lvwItems.Items.Clear();
                    lvwItems.Dock = DockStyle.None;
                    lvwItems.Visible = false;

                    flpImageList.Controls.Clear();
                    flpImageList.AutoScroll = true;
                    flpImageList.Dock = DockStyle.Fill;
                    flpImageList.BackColor = Color.White;
                    flpImageList.FlowDirection = FlowDirection.LeftToRight;
                    flpImageList.Visible = true;
                    break;
            }
        }
        #endregion

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }
    }
}