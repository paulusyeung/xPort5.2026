#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms.Dialogs;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;
using xPort5.Controls.Product;
using xPort5.Controls;
using xPort5.Helper;

#endregion

namespace xPort5.Order.PurchaseContract.Items
{
    public partial class ItemsList : UserControl
    {
        private enum PanelView { ListView, ImageView };

        private Common.Enums.EditMode editMode = Common.Enums.EditMode.Read;
        private Guid orderPCId = System.Guid.Empty;
        private string pcNumber = string.Empty;

        public ItemsList()
        {
            InitializeComponent();
        }

        #region public properties
        public Common.Enums.EditMode EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                editMode = value;
            }
        }

        public Guid OrderPCId
        {
            get
            {
                return orderPCId;
            }
            set
            {
                orderPCId = value;
            }
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttribute();
            SetListAns();
            SetLvwList();

            if (editMode == Common.Enums.EditMode.Edit)
            {
                BindList();
            }
        }

        private void SetAttribute()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.lvwItems.ListViewItemSorter = new ListViewItemSorter(this.lvwItems);
            this.lvwItems.Dock = DockStyle.Fill;

            this.colArticleCode.Text = oDict.GetWord("product_code");
            this.colSupplierRef.Text = oDict.GetWord("supplier_ref");
            this.colPackageCode.Text = oDict.GetWord("package");
            this.colCustRef.Text = oDict.GetWord("customer_ref");
            this.colDescription.Text = oDict.GetWord("product_description");
            this.colQty.Text = oDict.GetWord("qty");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colAmount.Text = oDict.GetWord("amount");
            this.colCurrency.Text = oDict.GetWord("currency");

            toolTip1.SetToolTip(this.lvwItems, oDict.GetWord("double_click_to_open_record"));
        }

        private void SetLvwList()
        {
            //this.lvwItems.ListViewItemSorter = new ListViewItemSorter(this.lvwItems);
            this.lvwItems.ListViewItemSorter = new Sorter();   // 參考：https://stackoverflow.com/a/1214333

            this.lvwItems.Dock = DockStyle.Fill;
            this.lvwItems.GridLines = true;

            //Ìá¹©Ò»‚€¹Ì¶¨µÄ Guid tag£¬ ÔÚ UserPreference ÖÐÓÃ×÷ß@‚€ ListView µÄ unique key
            lvwItems.Tag = new Guid("EBA2BBB9-AF61-4a93-A32E-D6479075FAFA");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwItems);
        }

        private void SetListAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

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
            ToolBarButton cmdViews = new ToolBarButton("Views", oDict.GetWord("Views"));
            cmdViews.Style = ToolBarButtonStyle.DropDownButton;
            cmdViews.Image = new IconResourceHandle("16x16.appView_xp.png");
            cmdViews.DropDownMenu = ddlViews;
            this.ansItems.Buttons.Add(cmdViews);
            cmdViews.MenuClick += new MenuEventHandler(cmdViews_MenuClick);
            #endregion

            #region cmdImageList    - Buttons[6]
            ContextMenu ddlImageList = new ContextMenu();
            Common.Data.AppendMenuItem_AppImageList(ref ddlImageList);
            ToolBarButton cmdImageList = new ToolBarButton("Images", oDict.GetWord("Images"));
            cmdImageList.Style = ToolBarButtonStyle.DropDownButton;
            cmdImageList.Image = new IconResourceHandle("16x16.imagelist_duo_on_16.png");
            cmdImageList.DropDownMenu = ddlImageList;
            this.ansItems.Buttons.Add(cmdImageList);
            cmdImageList.MenuClick += new MenuEventHandler(cmdImageList_MenuClick);
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
            if (editMode == Common.Enums.EditMode.Read || editMode == Common.Enums.EditMode.Add)
                cmdNew.Enabled = false;

            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("Delete"));
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            #region add cndNew
            if (xPort5.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PurchaseContract.Create"))
                {
                    this.ansItems.Buttons.Add(cmdNew);
                }
            }
            else
            {
                this.ansItems.Buttons.Add(cmdNew);
            }
            #endregion

            #region cmdDelete
            if (editMode == Common.Enums.EditMode.Edit)
            {
                if (xPort5.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PurchaseContract.Delete"))
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

            //this.ansItems.ButtonClick += new ToolBarButtonClickEventHandler(ansItem_ButtonClick);
        }

        private string BuildSql()
        {
            string sql = string.Format(@"
SELECT TOP 100 PERCENT
       [OrderPCId]          
      ,[PCNumber]
      ,[OrderPCItemsId]
      ,[PCDate]
      ,[LineNumber]
      ,[ArticleId]         
      ,[ArticleCode]
      ,[ArticleName]
      ,[PackageId]
      ,[PackageCode]
      ,[PackageName]
      ,[CustRef]
      ,[PriceType]
      ,[FactoryCost]
      ,[Qty]
      ,[Unit]               
      ,[SuppRef]
      ,[CurrencyCode]
FROM [dbo].[vwPurchaseContractItemList]
WHERE [OrderPCId] = '{0}'
ORDER BY [LineNumber]
", orderPCId.ToString());

            return sql;
        }

        private void BindList()
        {
            this.lvwItems.Items.Clear();

            int iCount = 1;

            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, BuildSql());

            while (reader.Read())
            {
                Guid itemId = reader.GetGuid(2);
                OrderPCItems item = OrderPCItems.Load(itemId);

                ListViewItem objItem = this.lvwItems.Items.Add(reader.GetString(6));   //Article Code
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
                objItem.SubItems.Add(reader.GetGuid(2).ToString()); //OrderPCItemsId
                //objItem.SubItems.Add(reader.GetGuid(18).ToString());
                //objItem.SubItems.Add(reader.GetGuid(19).ToString());
                objItem.SubItems.Add(iCount.ToString());            //Line Number
                objItem.SubItems.Add(reader.GetString(10));          //PackageCode
                objItem.SubItems.Add(reader.GetString(11));         //CustRef
                objItem.SubItems.Add(reader.GetString(16));         //SuppRef
                objItem.SubItems.Add(reader.GetString(7));          //ArticleName
                objItem.SubItems.Add(reader.GetDecimal(14).ToString("#,##0"));         //Qty
                objItem.SubItems.Add(reader.GetString(15));         //Unit
                objItem.SubItems.Add("$" + reader.GetDecimal(13).ToString("#,##0.0000"));        //FactoryCost
                objItem.SubItems.Add(reader.GetString(17));         //CurrencyCode

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
                Guid itemId = reader.GetGuid(2);
                Guid articleId = reader.GetGuid(5);

                ImagePanel imgItem = new ImagePanel(imageSize, articleId, inDetail, itemId, System.Guid.Empty, string.Empty, lvwItems.CheckBoxes, false);
                imgItem.DoubleClick += new EventHandler(imgPane_DoubleClick);

                flpImageList.Controls.Add(imgItem);
            }
            reader.Close();
        }

        void imgPane_DoubleClick(object sender, EventArgs e)
        {
            xPort5.Controls.ProductImage prodImage = (xPort5.Controls.ProductImage)sender;

            OrderPCItems item = OrderPCItems.LoadWhere(String.Format("OrderPCItemsId = '{0}'", prodImage.Name));
            if (item != null)
            {
                ItemRecord itemRecord = new ItemRecord();

                itemRecord.EditMode = Common.Enums.EditMode.Edit;
                itemRecord.OrderPCId = item.OrderPCId;
                itemRecord.OrderPCItemsId = item.OrderPCItemsId;

                OrderSCItems scItem = OrderSCItems.Load(item.OrderSCItemsId);
                if (scItem != null)
                {
                    string sql = "OrderQTItemId = '" + scItem.OrderQTItemId.ToString() + "'";
                    OrderQTCustShipping qtCustShipping = OrderQTCustShipping.LoadWhere(sql);
                    if (qtCustShipping != null)
                    {
                        itemRecord.OrderQTCustShippingId = qtCustShipping.OrderQTCustShippingId;
                    }

                    OrderQTSuppShipping qtSuppShipping = OrderQTSuppShipping.LoadWhere(sql);
                    if (qtSuppShipping != null)
                    {
                        itemRecord.OrderQTSuppShippingId = qtSuppShipping.OrderQTSuppShippingId;
                    }
                }

                itemRecord.ShowDialog();
            }
        }

        #region Action Strip Clicks
        void ansItems_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
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
                            BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
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
                        item.LineNumber = this.lvwItems.Items.Count + 1;
                        item.OrderId = orderPCId;
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
                    Guid pcItemsId = new Guid(lvwItems.SelectedItem.SubItems[1].Text);
                    //Guid suppShipId = new Guid(lvwItems.SelectedItem.SubItems[2].Text);
                    //Guid custShipId = new Guid(lvwItems.SelectedItem.SubItems[3].Text);

                    ItemRecord address = new ItemRecord();

                    address.EditMode = Common.Enums.EditMode.Edit;
                    address.OrderPCId = orderPCId;
                    address.OrderPCItemsId = pcItemsId;
                    //address.OrderQTSuppShippingId = suppShipId;
                    //address.OrderQTCustShippingId = custShipId;
                    address.ShowDialog();
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

                                if (xPort5.Controls.Utility.OrderPC.DeleteItem(itemId))
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

                                if (xPort5.Controls.Utility.OrderPC.DeleteItem(itemId))
                                {
                                    lvwItems.Items[lvwItems.SelectedIndex].Remove();
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<Guid> selectedList = xPort5.Controls.Utility.ImagePanel.GetCheckedItems(flpImageList, xPort5.Controls.Utility.ImagePanel.CheckedType.Order);
                    if (selectedList.Count > 0)
                    {
                        foreach (System.Guid itemId in selectedList)
                        {
                            xPort5.Controls.Utility.OrderPC.DeleteItem(itemId);
                        }
                    }

                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                }
            }
        }
        #endregion

        void cmdImageList_MenuClick(object sender, MenuItemEventArgs e)
        {
            SwitchPanel(PanelView.ImageView);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Small":
                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Small, false);
                    break;
                case "Medium":
                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Medium, false);
                    break;
                case "Large":
                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.Large, false);
                    break;
                case "Details":
                    BindImageList(xPort5.Controls.Utility.Resources.ImageSize.XLarge, true);
                    break;
            }
        }

        void cmdViews_MenuClick(object sender, MenuItemEventArgs e)
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

        private void lvwItems_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }

        private void lvwItems_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 參考：https://stackoverflow.com/a/1214333
            Sorter s = (Sorter)lvwItems.ListViewItemSorter;
            s.Column = e.Column;

            if (s.Order == Gizmox.WebGUI.Forms.SortOrder.Ascending)
            {
                s.Order = Gizmox.WebGUI.Forms.SortOrder.Descending;
            }
            else
            {
                s.Order = Gizmox.WebGUI.Forms.SortOrder.Ascending;
            }
            lvwItems.Sort();
        }
    }
}
