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
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Forms.Dialogs;

using xPort5.EF6;
using xPort5.Common;
#endregion

namespace xPort5.Order.Coding.Supplier
{
    public partial class SupplierList : UserControl
    {
        private int _ButtonIndex_Workshop = 0;
        private string _BaseSqlSelect = String.Empty;
        private string _BaseSqlWhere = String.Empty;
        private string _BaseSqlOrderBy = String.Empty;
        private string _CurSqlWhere = String.Empty;
        private string _CurSqlOrderBy = String.Empty;
        private string _CurWorkshop = String.Empty;
        private string _CurShortcut = String.Empty;

        public SupplierList()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttribute();
            SetCaptions();
            SetListAns();
            SetLvwList();

            _BaseSqlSelect = @"
SELECT TOP 100 PERCENT
       [SupplierId]
      ,[SupplierCode]
      ,[SupplierName]
      ,[SupplierName_Chs]
      ,[SupplierName_Cht]
      ,[RegionName]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
      ,[Status]
  FROM [dbo].[vwSupplierList]
";
            _BaseSqlWhere = String.Format("WHERE [Status] >= {0}", Common.Enums.Status.Draft.ToString("d"));
            _BaseSqlOrderBy = "ORDER BY [SupplierName]";
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;

            // do not load all of the records when form loaded
            //BindList();
        }

        #region Set Attributes, Themes
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            lblLookup.Text = oDict.GetWordWithColon("lookup");

            colCode.Text = oDict.GetWord("supplier_code");
            colName.Text = oDict.GetWord("supplier_name");
            colName_Chs.Text = oDict.GetWord("supplier_name_chs");
            colName_Cht.Text = oDict.GetWord("supplier_name_cht");
            colRegion.Text = oDict.GetWord("customer_region");
            colCreatedOn.Text = oDict.GetWord("created_on");
            colCreatedBy.Text = oDict.GetWord("created_by");
            colModifiedOn.Text = oDict.GetWord("modified_on");
            colModifiedBy.Text = oDict.GetWord("modified_by");
        }

        private void SetLvwList()
        {
            this.lvwGroupList.ListViewItemSorter = new ListViewItemSorter(this.lvwGroupList);
            this.lvwGroupList.Dock = DockStyle.Fill;
            this.lvwGroupList.GridLines = true;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwGroupList.Tag = new Guid("76615A5F-C28D-4b49-BBB6-E82299C75008");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwGroupList);
        }

        private void SetAttribute()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.lvwGroupList.ListViewItemSorter = new ListViewItemSorter(this.lvwGroupList);
            this.lvwGroupList.Dock = DockStyle.Fill;
            lvwGroupList.Margin = new Padding(0, 0, 0, 24);

            toolTip1.SetToolTip(txtLookup, String.Format("Look for targets:{0}Supplier Code and Customer Name", Environment.NewLine));
            toolTip1.SetToolTip(cmdLookup, String.Format("Look for targets:{0}Supplier Code and Customer Name", Environment.NewLine));
            toolTip1.SetToolTip(lvwGroupList, oDict.GetWord("double_click_to_open_record"));
        }
        #endregion

        #region Set Action Strip
        private void SetListAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdButtons   - Buttons [0~3]
            this.ansToolbar.Buttons.Add(new ToolBarButton("Columns", String.Empty));
            this.ansToolbar.Buttons[0].Image = new IconResourceHandle("16x16.listview_columns.gif");
            this.ansToolbar.Buttons[0].ToolTipText = @"Hide/Unhide Columns";
            this.ansToolbar.Buttons.Add(new ToolBarButton("Sorting", String.Empty));
            this.ansToolbar.Buttons[1].Image = new IconResourceHandle("16x16.listview_sorting.gif");
            this.ansToolbar.Buttons[1].ToolTipText = @"Sorting";
            this.ansToolbar.Buttons.Add(new ToolBarButton("Checkbox", String.Empty));
            this.ansToolbar.Buttons[2].Image = new IconResourceHandle("16x16.listview_checkbox.gif");
            this.ansToolbar.Buttons[2].ToolTipText = @"Toggle Checkbox";
            this.ansToolbar.Buttons.Add(new ToolBarButton("MultiSelect", String.Empty));
            this.ansToolbar.Buttons[3].Image = new IconResourceHandle("16x16.listview_multiselect.gif");
            this.ansToolbar.Buttons[3].ToolTipText = @"Toggle Multi-Select";
            this.ansToolbar.Buttons[3].Visible = false;
            #endregion

            this.ansToolbar.Buttons.Add(sep);

            #region cmdViews    - Buttons[5]
            ContextMenu ddlViews = new ContextMenu();
            Common.Data.AppendMenuItem_AppViews(ref ddlViews);
            ToolBarButton cmdViews = new ToolBarButton("Views", oDict.GetWord("Views"));
            cmdViews.Style = ToolBarButtonStyle.DropDownButton;
            cmdViews.Image = new IconResourceHandle("16x16.appView_xp.png");
            cmdViews.DropDownMenu = ddlViews;
            this.ansToolbar.Buttons.Add(cmdViews);
            cmdViews.MenuClick += new MenuEventHandler(ansViews_MenuClick);
            #endregion

            this.ansToolbar.Buttons.Add(sep);

            #region cmdRefresh, cmdPreference       - Buttons[7~8]
            this.ansToolbar.Buttons.Add(new ToolBarButton("Refresh", oDict.GetWord("Refresh")));
            this.ansToolbar.Buttons[7].Image = new IconResourceHandle("16x16.16_L_refresh.gif");
            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);

            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansToolbar.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            this.ansToolbar.Buttons.Add(sep);

            #region cmdWorkshop     - Buttons [10]
            //ContextMenu ddlWorkshop = new ContextMenu();

            //Client_UserCollection oWorkshop = Common.Data.GetWorkshopList();
            //if (oWorkshop.Count > 0)
            //{
            //    for (int i = 0; i < oWorkshop.Count; i++)
            //    {
            //        ddlWorkshop.MenuItems.Add(new MenuItem(oWorkshop[i].FullName));
            //    }
            //}

            //ToolBarButton cmdWorkshop = new ToolBarButton("Workshop", "Branch");
            //cmdWorkshop.Style = ToolBarButtonStyle.DropDownButton;
            //cmdWorkshop.Image = new IconResourceHandle("16x16.filter_16.png");
            //cmdWorkshop.DropDownMenu = ddlWorkshop;
            //this.ansToolbar.Buttons.Add(cmdWorkshop);
            //cmdWorkshop.MenuClick += new MenuEventHandler(ansWorkshop_MenuClick);
            //_ButtonIndex_Workshop = this.ansToolbar.Buttons.Count - 1;
            #endregion

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansToolbar.Buttons.Add(cmdPopup);
            #endregion

            //this.ansToolbar.Buttons.Add(sep);

        }
        #endregion

        #region Bind List
        private void BindList()
        {
            this.lvwGroupList.Items.Clear();

            int iCount = 1;
            
            // Use ViewService instead of direct SQL query
            string whereClause = BuildWhereClause();
            string orderBy = BuildOrderByClause();
            DataSet ds = ViewService.Default.GetSupplierList(whereClause, orderBy);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                Guid supplierId = (Guid)row["SupplierId"];

                ListViewItem objItem = this.lvwGroupList.Items.Add(row["SupplierCode"].ToString());  // Code
                #region Icon
                int status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                switch (status)
                {
                    case (int)Common.Enums.Status.Draft:
                        objItem.SmallImage = new IconResourceHandle("16x16.supplierSingleGrey_16.gif");
                        objItem.LargeImage = new IconResourceHandle("32x32.supplierSingleGrey_32.png");
                        break;
                    case (int)Common.Enums.Status.Active:
                        objItem.SmallImage = new IconResourceHandle("16x16.supplierSingle_16.gif");
                        objItem.LargeImage = new IconResourceHandle("32x32.suppliersingle_32.png");
                        break;
                }
                #endregion
                objItem.SubItems.Add(supplierId.ToString());     // Id
                #region contact icon
                xPort5.EF6.SupplierContactCollection contacts = xPort5.EF6.SupplierContact.LoadCollection(String.Format("SupplierId = '{0}'", supplierId.ToString()));
                switch (contacts.Count)
                {
                    case 0:
                        objItem.SubItems.Add(String.Empty);
                        break;
                    case 1:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.contactsingle_16.png").ToString());
                        break;
                    default:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.contactdouble_16.png").ToString());
                        break;
                }
                #endregion
                #region address icon
                xPort5.EF6.SupplierAddressCollection address = xPort5.EF6.SupplierAddress.LoadCollection(String.Format("SupplierId = '{0}'", supplierId.ToString()));
                switch (address.Count)
                {
                    case 0:
                        objItem.SubItems.Add(String.Empty);
                        break;
                    case 1:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.addresssingle_16.png").ToString());
                        break;
                    default:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.addressdouble_16.png").ToString());
                        break;
                }
                #endregion
                objItem.SubItems.Add(iCount.ToString());                // Line Number
                objItem.SubItems.Add(row["SupplierName"] != DBNull.Value ? row["SupplierName"].ToString() : "");              // Name
                objItem.SubItems.Add(row["SupplierName_Chs"] != DBNull.Value ? row["SupplierName_Chs"].ToString() : "");              // Name (Chs)
                objItem.SubItems.Add(row["SupplierName_Cht"] != DBNull.Value ? row["SupplierName_Cht"].ToString() : "");              // Name (Cht)
                objItem.SubItems.Add(row["RegionName"] != DBNull.Value ? row["RegionName"].ToString() : "");              // Region
                objItem.SubItems.Add(row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // 
                objItem.SubItems.Add(row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : "");              // 
                objItem.SubItems.Add(row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // 
                objItem.SubItems.Add(row["ModifiedBy"] != DBNull.Value ? row["ModifiedBy"].ToString() : "");              // 

                iCount++;
            }

            this.lvwGroupList.Sort();
        }

        // Deprecated: Replaced by BuildWhereClause() and BuildOrderByClause() for ViewService
        private string BuildSqlQueryString()
        {
            StringBuilder sql = new StringBuilder();

            #region prepare the where clause
            if (!(String.IsNullOrEmpty(_CurShortcut)))
            {
                switch (_CurShortcut)
                {
                    case "9":
                        _CurSqlWhere = _BaseSqlWhere + " AND ( SUBSTRING([SupplierName], 1, 1) NOT BETWEEN N'A' AND N'Z' )";
                        break;
                    case "All":
                        _CurSqlWhere = _BaseSqlWhere;
                        break;
                    default:
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ( SUBSTRING([SupplierName], 1, 1) = N'{0}' )", _CurShortcut);
                        break;
                }
            }
            #endregion

            sql.Append(_BaseSqlSelect + Environment.NewLine);
            sql.Append(_CurSqlWhere + Environment.NewLine);
            sql.Append(_CurSqlOrderBy);

            return sql.ToString();
        }

        /// <summary>
        /// Builds WHERE clause for ViewService (without "WHERE" keyword)
        /// </summary>
        private string BuildWhereClause()
        {
            // Handle shortcut filtering
            if (!(String.IsNullOrEmpty(_CurShortcut)))
            {
                switch (_CurShortcut)
                {
                    case "9":
                        _CurSqlWhere = _BaseSqlWhere + " AND ( SUBSTRING([SupplierName], 1, 1) NOT BETWEEN N'A' AND N'Z' )";
                        break;
                    case "All":
                        _CurSqlWhere = _BaseSqlWhere;
                        break;
                    default:
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ( SUBSTRING([SupplierName], 1, 1) = N'{0}' )", _CurShortcut);
                        break;
                }
            }

            string whereClause = _CurSqlWhere;
            
            // Remove "WHERE" keyword if present
            if (whereClause.TrimStart().StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
            {
                whereClause = whereClause.Substring(whereClause.IndexOf("WHERE ", StringComparison.OrdinalIgnoreCase) + 6);
            }
            
            return whereClause.Trim();
        }

        /// <summary>
        /// Builds ORDER BY clause for ViewService (without "ORDER BY" keyword)
        /// </summary>
        private string BuildOrderByClause()
        {
            string orderBy = _CurSqlOrderBy;
            
            // Remove "ORDER BY" keyword if present
            if (orderBy.TrimStart().StartsWith("ORDER BY ", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = orderBy.Substring(orderBy.IndexOf("ORDER BY ", StringComparison.OrdinalIgnoreCase) + 9);
            }
            
            return orderBy.Trim();
        }
        #endregion

        private void ResetForm()
        {
            txtLookup.Text = String.Empty;
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
            _CurShortcut = String.Empty;
        }

        private void RefreshForm()
        {
            txtLookup.Text = String.Empty;
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
            _CurShortcut = String.Empty;
        }

        private void DoLookup()
        {
            string target = txtLookup.Text.Trim();
            if (!(String.IsNullOrEmpty(target)))
            {
                ResetForm();
                txtLookup.Text = target;
                _CurSqlWhere = _BaseSqlWhere +
                               String.Format(" AND ( ([SupplierCode] LIKE '%{0}%') OR ([SupplierName] LIKE N'%{0}%') OR ([SupplierName_Chs] LIKE '%{0}%') OR ([SupplierName_Cht] LIKE '%{0}%') )", target);
                BindList();
                this.Update();
            }
        }

        private void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        RefreshForm();
                        BindList();
                        this.Update();
                        break;
                    case "columns":
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwGroupList);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        ListViewSortingOptions objListViewSortingOptions = new ListViewSortingOptions(this.lvwGroupList);
                        objListViewSortingOptions.ShowDialog();
                        break;
                    case "checkbox":
                        this.lvwGroupList.CheckBoxes = !this.lvwGroupList.CheckBoxes;
                        break;
                    case "multiselect":
                        this.lvwGroupList.MultiSelect = !this.lvwGroupList.MultiSelect;
                        e.Button.Pushed = true;
                        break;
                    case "popup":
                        ShowRecord();
                        break;
                }
            }
        }

        private void ShowRecord()
        {
            if (lvwGroupList.SelectedItem != null)
            {
                Guid itemId = new Guid(lvwGroupList.SelectedItem.SubItems[1].Text);
                SupplierRecord supplier = new SupplierRecord();
                supplier.SupplierId = itemId;
                supplier.EditMode = Common.Enums.EditMode.Edit;
                supplier.Show();
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwGroupList);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwGroupList);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            switch (e.MenuItem.Text)
            {
                case "Icon":
                    this.lvwGroupList.View = View.SmallIcon;
                    break;
                case "Tile":
                    this.lvwGroupList.View = View.LargeIcon;
                    break;
                case "List":
                    this.lvwGroupList.View = View.List;
                    break;
                case "Details":
                    this.lvwGroupList.View = View.Details;
                    break;
            }
        }

        private void ansWorkshop_MenuClick(object sender, MenuItemEventArgs e)
        {
            // show he selected Workshop as Ans Button text
            ToolBarButton oSender = (ToolBarButton)sender;
            oSender.Text = e.MenuItem.Text;
            _CurWorkshop = e.MenuItem.Text;
            BindList();
            this.Update();
        }

        private void ShortcutButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            _CurShortcut = button.Tag.ToString();
            BindList();
            this.Update();
        }

        private void cmdLookup_Click(object sender, EventArgs e)
        {
            DoLookup();
        }

        private void txtLookup_EnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            DoLookup();
        }

        private void lvwClientList_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }
    }
}
