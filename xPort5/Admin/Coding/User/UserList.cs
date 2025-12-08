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

namespace xPort5.Admin.Coding.User
{
    public partial class UserList : UserControl
    {
        nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

        private int _ButtonIndex_Workshop = 0;
        private string _BaseSqlSelect = String.Empty;
        private string _BaseSqlWhere = String.Empty;
        private string _BaseSqlOrderBy = String.Empty;
        private string _CurSqlWhere = String.Empty;
        private string _CurSqlOrderBy = String.Empty;
        private string _CurWorkshop = String.Empty;
        private string _CurShortcut = String.Empty;

        public UserList()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttribute();
            SetCaptions();
            SetStaffListAns();
            SetLvwList();

            _BaseSqlSelect = @"
SELECT TOP 100 PERCENT
       [UserSid]        -- 0
      ,[UserType]
      ,[Alias]
      ,[LoginName]
      ,[LoginPassword]
      ,[FullName]       -- 5
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
      ,[Status]         -- 10
FROM [dbo].[vwUserList]
";
            _BaseSqlWhere = String.Format("WHERE [Status] >= {0}", Common.Enums.Status.Active.ToString("d"));
            _BaseSqlOrderBy = "ORDER BY [Alias]";
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;

            BindUserList();
        }

        #region Set Attributes, Themes
        private void SetCaptions()
        {
            lblLookup.Text = oDict.GetWordWithColon("lookup");

            colUserAlias.Text = oDict.GetWord("alias");
            colUserCode.Text = string.Format(oDict.GetWord("code_replace"), oDict.GetWord("logon_user"));
            colDivision.Text = oDict.GetWord("full_name");
            colGroup.Text = oDict.GetWord("usergroup");
            colFirstName.Text = oDict.GetWord("login_name");
            colLastName.Text = oDict.GetWord("login_password");
            colCreatedOn.Text = oDict.GetWord("created_on");
            colCreatedBy.Text = oDict.GetWord("created_by");
            colModifiedOn.Text = oDict.GetWord("modified_on");
            colModifiedBy.Text = oDict.GetWord("modified_by");
        }

        private void SetLvwList()
        {
            this.lvwUserList.ListViewItemSorter = new ListViewItemSorter(this.lvwUserList);
            this.lvwUserList.Dock = DockStyle.Fill;
            this.lvwUserList.GridLines = true;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            this.lvwUserList.Tag = new Guid("AD8E71FC-2476-4ca3-AFE0-EAA38B2422BE");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwUserList);
        }

        private void SetAttribute()
        {
            this.lvwUserList.ListViewItemSorter = new ListViewItemSorter(this.lvwUserList);
            this.lvwUserList.Dock = DockStyle.Fill;
            lvwUserList.Margin = new Padding(0, 24, 0, 24);

            toolTip1.SetToolTip(txtLookup, String.Format("Look for targets:{0}Login Name and Full Name", Environment.NewLine));
            toolTip1.SetToolTip(cmdLookup, String.Format("Look for targets:{0}Login Name and Full Name", Environment.NewLine));
            toolTip1.SetToolTip(lvwUserList, oDict.GetWord("double_click_to_open_record"));
        }
        #endregion

        #region Set Action Strip
        private void SetStaffListAns()
        {
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

        #region Bind User List
        private void BindUserList()
        {
            this.lvwUserList.Items.Clear();

            int iCount = 1;
            
            // Use ViewService instead of direct SQL query
            string whereClause = BuildWhereClause();
            string orderBy = BuildOrderByClause();
            DataSet ds = ViewService.Default.GetUserList(whereClause, orderBy);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                string alias = row["Alias"] != DBNull.Value ? row["Alias"].ToString() : "";
                int userType = row["UserType"] != DBNull.Value ? Convert.ToInt32(row["UserType"]) : 0;
                
                ListViewItem objItem = this.lvwUserList.Items.Add(alias);  // User Alias
                #region User Icon
                switch (userType)
                {
                    case (int)Common.Enums.UserType.Supplier:
                        objItem.SmallImage = new IconResourceHandle("16x16.suppliersingle_16.gif");
                        objItem.LargeImage = new IconResourceHandle("32x32.suppliersingle_32.png");
                        break;
                    case (int)Common.Enums.UserType.Customer:
                        objItem.SmallImage = new IconResourceHandle("16x16.customersingle_16.png");
                        objItem.LargeImage = new IconResourceHandle("32x32.customersingle_32.png");
                        break;
                    case (int)Common.Enums.UserType.Staff:
                    default:
                        string createdBy = row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : "";
                        if ((createdBy == String.Empty) || (createdBy == System.Guid.Empty.ToString()))
                        {
                            objItem.SmallImage = new IconResourceHandle("16x16.staff16_key.png");
                            objItem.LargeImage = new IconResourceHandle("32x32.staff32_key.png");
                        }
                        else
                        {
                            objItem.SmallImage = new IconResourceHandle("16x16.staffSingle_16.png");
                            objItem.LargeImage = new IconResourceHandle("32x32.staffSingle_32.png");
                        }
                        break;
                }
                #endregion
                objItem.SubItems.Add(row["UserSid"].ToString());     // UserId
                objItem.SubItems.Add(iCount.ToString());                // Line Number
                objItem.SubItems.Add(userType.ToString());
                #region User Type
                switch (userType)
                {
                    case (int)Common.Enums.UserType.Supplier:
                        objItem.SubItems.Add(oDict.GetWord(Common.Enums.UserType.Supplier.ToString("g")));
                        break;
                    case (int)Common.Enums.UserType.Customer:
                        objItem.SubItems.Add(oDict.GetWord(Common.Enums.UserType.Customer.ToString("g")));
                        break;
                    case (int)Common.Enums.UserType.Staff:
                    default:
                        objItem.SubItems.Add(oDict.GetWord(Common.Enums.UserType.Staff.ToString("g")));
                        break;
                }
                #endregion
                objItem.SubItems.Add(row["LoginName"] != DBNull.Value ? row["LoginName"].ToString() : "");              // Login Name
                objItem.SubItems.Add(row["LoginPassword"] != DBNull.Value ? row["LoginPassword"].ToString() : "");              // Login Password
                objItem.SubItems.Add(row["FullName"] != DBNull.Value ? row["FullName"].ToString() : "");              // User Full Name
                objItem.SubItems.Add(row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // Created On
                objItem.SubItems.Add(row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : "");              // Created By
                objItem.SubItems.Add(row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // Modified On
                objItem.SubItems.Add(row["ModifiedBy"] != DBNull.Value ? row["ModifiedBy"].ToString() : "");              // Modified By

                iCount++;
            }

            this.lvwUserList.Sort();
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
                        _CurSqlWhere = _BaseSqlWhere + " AND ( SUBSTRING([Alias], 1, 1) NOT BETWEEN N'A' AND N'Z' )";
                        break;
                    case "All":
                        _CurSqlWhere = _BaseSqlWhere;
                        break;
                    default:
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ( SUBSTRING([Alias], 1, 1) = N'{0}' )", _CurShortcut);
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
                        _CurSqlWhere = _BaseSqlWhere + " AND ( SUBSTRING([Alias], 1, 1) NOT BETWEEN N'A' AND N'Z' )";
                        break;
                    case "All":
                        _CurSqlWhere = _BaseSqlWhere;
                        break;
                    default:
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND ( SUBSTRING([Alias], 1, 1) = N'{0}' )", _CurShortcut);
                        break;
                }
            }
            #endregion

            sql.Append(_BaseSqlSelect + Environment.NewLine);
            sql.Append(_CurSqlWhere + Environment.NewLine);
            sql.Append(_CurSqlOrderBy);

            return sql.ToString();
        }
        #endregion

        private void ResetForm()
        {
            txtLookup.Text = String.Empty;
            //            ansToolbar.Buttons[_ButtonIndex_Workshop].Text = "Branch";
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
            _CurWorkshop = String.Empty;
            _CurShortcut = String.Empty;
        }

        private void RefreshForm()
        {
            txtLookup.Text = String.Empty;
            //            ansToolbar.Buttons[_ButtonIndex_Workshop].Text = "Branch";
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
            _CurWorkshop = String.Empty;
            //            _CurShortcut = String.Empty;
        }

        private void DoLookup()
        {
            string target = txtLookup.Text.Trim();
            if (!(String.IsNullOrEmpty(target)))
            {
                ResetForm();
                txtLookup.Text = target;
                _CurSqlWhere = _BaseSqlWhere + " AND " +
                    String.Format("( ([FullName] LIKE N'%{0}%') OR ([Alias] LIKE N'%{0}%') OR ([LoginName] LIKE N'%{0}%') OR ([LoginPassword] LIKE '%{0}%') )", target);
                BindUserList();
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
                        BindUserList();
                        this.Update();
                        break;
                    case "columns":
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwUserList);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        ListViewSortingOptions objListViewSortingOptions = new ListViewSortingOptions(this.lvwUserList);
                        objListViewSortingOptions.ShowDialog();
                        break;
                    case "checkbox":
                        this.lvwUserList.CheckBoxes = !this.lvwUserList.CheckBoxes;
                        break;
                    case "multiselect":
                        this.lvwUserList.MultiSelect = !this.lvwUserList.MultiSelect;
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
            if (lvwUserList.SelectedItem != null)
            {
                Guid userSid = new Guid(lvwUserList.SelectedItem.SubItems[1].Text);
                int userType = Convert.ToInt32(lvwUserList.SelectedItem.SubItems[3].Text);
                switch (userType)
                {
                    case (int)Common.Enums.UserType.Staff:
                        Staff.StaffRecord staff = new Staff.StaffRecord();
                        staff.UserId = userSid;
                        staff.EditMode = Common.Enums.EditMode.Edit;
                        staff.Show();
                        break;
                    case (int)Common.Enums.UserType.Supplier:
                        SupplierContact sContact = SupplierContact.Load(userSid);
                        if (sContact != null)
                        {
                            xPort5.Order.Coding.Supplier.Contact.ContactRecord supplier = new xPort5.Order.Coding.Supplier.Contact.ContactRecord();
                            supplier.SupplierId = sContact.SupplierId;
                            supplier.SupplierContactId = sContact.SupplierContactId;
                            supplier.EditMode = Common.Enums.EditMode.Edit;
                            supplier.Show();
                        }
                        break;
                    case (int)Common.Enums.UserType.Customer:
                        CustomerContact cContact = CustomerContact.Load(userSid);
                        if (cContact != null)
                        {
                            xPort5.Order.Coding.Customer.Contact.ContactRecord customer = new xPort5.Order.Coding.Customer.Contact.ContactRecord();
                            customer.CustomerId = cContact.CustomerId;
                            customer.CustomerContactId = cContact.CustomerContactId;
                            customer.EditMode = Common.Enums.EditMode.Edit;
                            customer.Show();
                        }
                        break;
                }
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwUserList);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwUserList);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            switch (e.MenuItem.Text)
            {
                case "Icon":
                    this.lvwUserList.View = View.SmallIcon;
                    break;
                case "Tile":
                    this.lvwUserList.View = View.LargeIcon;
                    break;
                case "List":
                    this.lvwUserList.View = View.List;
                    break;
                case "Details":
                    this.lvwUserList.View = View.Details;
                    break;
            }
        }

        private void ansWorkshop_MenuClick(object sender, MenuItemEventArgs e)
        {
            // show he selected Workshop as Ans Button text
            ToolBarButton oSender = (ToolBarButton)sender;
            oSender.Text = e.MenuItem.Text;
            _CurWorkshop = e.MenuItem.Text;
            BindUserList();
            this.Update();
        }

        private void ShortcutButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            _CurShortcut = button.Tag.ToString();
            BindUserList();
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