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
using xPort5.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using xPort5.Helper;

#endregion

namespace xPort5.Order.Quotation
{
    public partial class QuotationList : UserControl
    {
        private string _BaseSqlSelect = String.Empty;
        private string _BaseSqlWhere = String.Empty;
        private string _BaseSqlOrderBy = String.Empty;
        private string _CurSqlWhere = String.Empty;
        private string _CurSqlOrderBy = String.Empty;
        private string _CustomerName = String.Empty;
        private Guid _CustomerId = System.Guid.Empty;

        #region Properties
        private static string quotationNum;
        /// <summary>
        /// Get ListView selectItem (SubItems[0])-QuotationNo
        /// </summary>
        public static string QuotationNum
        {
            get
            {
                return quotationNum;
            }
            set
            {
                quotationNum = value;
            }
        }
        #endregion

        public QuotationList()
        {
            InitializeComponent();
        }

        private void QuotationList_Load(object sender, EventArgs e)
        {
            SetAttributes();
            LoadComboBox();
            SetAnsTree();
            SetTvwCustomer();
            SetAnsList();
            SetLvwList();

            _BaseSqlSelect = @"
SELECT TOP 100 PERCENT
       [OrderQTId]
      ,[QTNumber]
      ,CONVERT(NVARCHAR(10), [QTDate], 20)
      ,[CustomerName]
      ,[SalePerson]
      ,[Remarks]
      ,[Status]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
      ,[InUse]
      ,[Revision]
      ,[SampleQty]
FROM [dbo].[vwOrderQTList]
";
            _BaseSqlWhere = String.Format("WHERE [Status] = {0}", Common.Enums.Status.Active.ToString("d"));
            _BaseSqlOrderBy = "ORDER BY [QTNumber] DESC";
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
        }

        #region SetTheme...
        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            toolTip1.SetToolTip(txtLookup, String.Format(oDict.GetWord("look_for_targets"), "; ", "QT Number", "Customer Code and Customer Name"));
            toolTip1.SetToolTip(cmdLookup, String.Format(oDict.GetWord("look_for_targets"), "; ", "QT Number", "Customer Code and Customer Name"));
            toolTip1.SetToolTip(lvwList, oDict.GetWord("double_click_to_open_record"));

            lblLookup.Text = oDict.GetWordWithColon("look_for");
            lblViews.Text = oDict.GetWordWithColon("view");

            colItemNumber.Text = oDict.GetWord("qt_number");
            colDate.Text = oDict.GetWord("qt_date");
            colCustomer.Text = oDict.GetWord("customer");
            colRemarks.Text = oDict.GetWord("remarks");
            colCreatedOn.Text = oDict.GetWord("created_on");
            colCreatedBy.Text = oDict.GetWord("created_by");
            colModifiedBy.Text = oDict.GetWord("modified_by");
            colModifiedOn.Text = oDict.GetWord("modified_on");
        }

        private void LoadComboBox()
        {
            xPort5.Controls.Utility.ComboBox.LoadViews(ref cboViews);
        }

        private void SetTvwCustomer()
        {
            this.tvwCustomer.Dock = DockStyle.Fill;
            xPort5.Controls.Utility.TreeViewControl.LoadCustomer(this.tvwCustomer.Nodes);
        }

        private void SetAnsTree()
        {
            this.ansTree.MenuHandle = false;
            this.ansTree.DragHandle = false;
            this.ansTree.TextAlign = ToolBarTextAlign.Right;
        }

        private void SetAnsList()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.ansList.MenuHandle = false;
            this.ansList.DragHandle = false;
            this.ansList.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdButtons   - Buttons [0~3]
            this.ansList.Buttons.Add(new ToolBarButton("Columns", string.Empty));
            this.ansList.Buttons[0].Image = new IconResourceHandle("16x16.listview_columns.gif");
            this.ansList.Buttons[0].ToolTipText = oDict.GetWord(@"Hide/Unhide Columns");
            this.ansList.Buttons.Add(new ToolBarButton("Sorting", String.Empty));
            this.ansList.Buttons[1].Image = new IconResourceHandle("16x16.listview_sorting.gif");
            this.ansList.Buttons[1].ToolTipText = oDict.GetWord(@"Sorting");
            this.ansList.Buttons.Add(new ToolBarButton("Checkbox", String.Empty));
            this.ansList.Buttons[2].Image = new IconResourceHandle("16x16.listview_checkbox.gif");
            this.ansList.Buttons[2].ToolTipText = oDict.GetWord(@"Toggle Checkbox");
            this.ansList.Buttons.Add(new ToolBarButton("MultiSelect", String.Empty));
            this.ansList.Buttons[3].Image = new IconResourceHandle("16x16.listview_multiselect.gif");
            this.ansList.Buttons[3].ToolTipText = oDict.GetWord(@"Toggle Multi-Select");
            this.ansList.Buttons[3].Visible = false;
            #endregion

            this.ansList.Buttons.Add(sep);

            #region cmdViews    - Buttons[5]
            ContextMenu ddlViews = new ContextMenu();
            Common.Data.AppendMenuItem_AppViews(ref ddlViews);
            ToolBarButton cmdViews = new ToolBarButton("Views", oDict.GetWord("View"));
            cmdViews.Style = ToolBarButtonStyle.DropDownButton;
            cmdViews.Image = new IconResourceHandle("16x16.appView_xp.png");
            cmdViews.DropDownMenu = ddlViews;
            this.ansList.Buttons.Add(cmdViews);
            cmdViews.MenuClick += new MenuEventHandler(ansViews_MenuClick);
            #endregion

            #region cmdPreference    - Buttons[6]
            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansList.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            this.ansList.Buttons.Add(sep);

            #region cmdRefresh cmdPrint       - Buttons[8~10]
            this.ansList.Buttons.Add(new ToolBarButton("Refresh", oDict.GetWord("Refresh")));
            this.ansList.Buttons[8].Image = new IconResourceHandle("16x16.16_L_refresh.gif");

            this.ansList.Buttons.Add(sep);

            this.ansList.Buttons.Add(new ToolBarButton("Print", oDict.GetWord("Print")));
            this.ansList.Buttons[10].Image = new IconResourceHandle("16x16.16_print.gif");
            this.ansList.Buttons[10].Enabled = false;
            this.ansList.ButtonClick += new ToolBarButtonClickEventHandler(ansList_ButtonClick);
            #endregion

            this.ansList.Buttons.Add(sep);

            #region Attachment
            this.ansList.Buttons.Add(new ToolBarButton("Attachment", oDict.GetWord("Attachment")));
            this.ansList.Buttons[12].Image = new IconResourceHandle("16x16.ico_16_1001_d.gif");
            this.ansList.Buttons[12].Enabled = false;
            #endregion

            #region cmdWorkshop     
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
            //this.ansList.Buttons.Add(cmdWorkshop);
            //cmdWorkshop.MenuClick += new MenuEventHandler(ansWorkshop_MenuClick);
            //_ButtonIndex_Workshop = this.ansList.Buttons.Count - 1;
            #endregion

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansList.Buttons.Add(cmdPopup);
            #endregion

            //this.ansList.Buttons.Add(sep);
        }

        private void SetLvwList()
        {
            //this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.ListViewItemSorter = new Sorter();   // 參考：https://stackoverflow.com/a/1214333

            this.lvwList.Dock = DockStyle.Fill;
            this.lvwList.GridLines = true;

            // 2012.04.18 paulus: 提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwList.Tag = new Guid("A9EF6373-E50A-46f9-96C5-76F0CC6D48E4");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwList);
        }
        #endregion

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            switch (e.MenuItem.Tag.ToString())
            {
                case "Icon":
                    this.lvwList.View = View.SmallIcon;
                    break;
                case "Tile":
                    this.lvwList.View = View.LargeIcon;
                    break;
                case "List":
                    this.lvwList.View = View.List;
                    break;
                case "Details":
                    this.lvwList.View = View.Details;
                    break;
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwList);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwList);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        private void ansList_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        this.ansList.Buttons[10].Enabled = false;
                        RefreshForm();
                        BindList();
                        this.Update();
                        break;
                    case "columns":
                        this.ansList.Buttons[10].Enabled = false;
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwList);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        this.ansList.Buttons[10].Enabled = false;
                        ListViewSortingOptions objListViewSortingOptions = new ListViewSortingOptions(this.lvwList);
                        objListViewSortingOptions.ShowDialog();
                        break;
                    case "checkbox":
                        this.ansList.Buttons[10].Enabled = false;
                        this.lvwList.CheckBoxes = !this.lvwList.CheckBoxes;
                        break;
                    case "multiselect":
                        this.ansList.Buttons[10].Enabled = false;
                        this.lvwList.MultiSelect = !this.lvwList.MultiSelect;
                        e.Button.Pushed = true;
                        break;
                    case "print":
                        PrintManager print = new PrintManager();
                        print.ShowDialog();
                        break;
                    case "attachment":
                        this.ShowAttachmentManager();
                        break;
                    case "popup":
                        ShowRecord();
                        break;
                }
            }
        }

        private void ShowRecord()
        {
            this.ansList.Buttons[10].Enabled = false;
            this.ansList.Buttons[12].Enabled = false;

            if (lvwList.SelectedItem != null)
            {
                Guid itemId = new Guid(lvwList.SelectedItem.SubItems[1].Text);
                QuotationRecord quotation = new QuotationRecord();
                quotation.QuotationId = itemId;
                quotation.EditMode = Common.Enums.EditMode.Edit;
                quotation.Show();

                quotation.Closed += new EventHandler(quotation_Closed);
            }
        }

        private void ResetForm()
        {
            txtLookup.Text = String.Empty;
            _CurSqlWhere = _BaseSqlWhere;
            _CurSqlOrderBy = _BaseSqlOrderBy;
        }

        private void RefreshForm()
        {
            txtLookup.Text = String.Empty;
        }

        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            
            // Use ViewService instead of direct SQL query
            string whereClause = BuildWhereClause();
            string orderBy = BuildOrderByClause();
            DataSet ds = ViewService.Default.GetQuotationList(whereClause, orderBy);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                Guid orderId = (Guid)row["OrderQTId"];

                ListViewItem objItem = this.lvwList.Items.Add(row["QTNumber"].ToString());  // Number
                #region Icon
                int revision = Convert.ToInt32(row["Revision"]);
                switch (revision)
                {
                    case 0:
                        objItem.SmallImage = new IconResourceHandle("16x16.invoice16_0.png");
                        objItem.LargeImage = new IconResourceHandle("32x32.invoice32_0.png");
                        break;
                    case 1:
                        objItem.SmallImage = new IconResourceHandle("16x16.invoice16_1.png");
                        objItem.LargeImage = new IconResourceHandle("32x32.invoice32_1.png");
                        break;
                    case 2:
                        objItem.SmallImage = new IconResourceHandle("16x16.invoice16_2.png");
                        objItem.LargeImage = new IconResourceHandle("32x32.invoice32_2.png");
                        break;
                    default:
                        objItem.SmallImage = new IconResourceHandle("16x16.invoice16_3x.png");
                        objItem.LargeImage = new IconResourceHandle("32x32.invoice32_3x.png");
                        break;
                }
                #endregion
                objItem.SubItems.Add(orderId.ToString());     // Id
                #region Status icon
                int status = Convert.ToInt32(row["Status"]);
                switch (status)
                {
                    case 0:
                        objItem.SubItems.Add(String.Empty);
                        break;
                    case 1:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.flag_green.png").ToString());
                        break;
                    default:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.flag_dark.png").ToString());
                        break;
                }
                #endregion
                #region Sample icon
                decimal sampleQty = row["SampleQty"] != DBNull.Value ? Convert.ToDecimal(row["SampleQty"]) : 0;
                switch (Convert.ToInt32(sampleQty))
                {
                    case 0:
                        objItem.SubItems.Add(String.Empty);
                        break;
                    default:
                        objItem.SubItems.Add(new IconResourceHandle("16x16.flower_16.png").ToString());
                        break;
                }
                #endregion
                #region Attachment
                if (xPort5.Controls.Utility.Resources.HasAttachment(row["QTNumber"].ToString())) // Attachment
                {
                    objItem.SubItems.Add(new IconResourceHandle("16x16.ico_16_1001_d.gif").ToString());
                }
                else
                {
                    objItem.SubItems.Add(String.Empty);
                }
                #endregion
                objItem.SubItems.Add(iCount.ToString());                // Line Number
                objItem.SubItems.Add(row["QTDate"] != DBNull.Value ? Convert.ToDateTime(row["QTDate"]).ToString("yyyy-MM-dd") : "");              // QT Date
                objItem.SubItems.Add(row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : "");              // Customer
                objItem.SubItems.Add(row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : "");              // Remarks

                objItem.SubItems.Add(row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // 
                objItem.SubItems.Add(row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : "");              // 
                objItem.SubItems.Add(row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // 
                objItem.SubItems.Add(row["ModifiedBy"] != DBNull.Value ? row["ModifiedBy"].ToString() : "");             // 

                iCount++;
            }
            
            this.lvwList.Sort();        // 2012.04.18 paulus: 依照當前的 ListView.SortOrder 和 ListView.SortPosition 排序，使 UserPreference 有效
        }

        // Deprecated: Replaced by BuildWhereClause() and BuildOrderByClause() for ViewService
        private string BuildSql()
        {
            StringBuilder sql = new StringBuilder();

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

        private void DoLookup()
        {
            string target = txtLookup.Text.Trim();
            if (!(String.IsNullOrEmpty(target)))
            {
                ResetForm();
                txtLookup.Text = target;
                cboViews.SelectedIndex = 0;
                _CurSqlWhere = _BaseSqlWhere +
                               String.Format(" AND ( ([QTNumber] LIKE '%{0}%') OR ([CustomerCode] LIKE '%{0}%') OR ([CustomerName] LIKE N'%{0}%') OR ([CustomerName_Chs] LIKE '%{0}%') OR ([CustomerName_Cht] LIKE '%{0}%') )", target);
                BindList();
                this.Update();
            }
        }

        private void cmdLookup_Click(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            DoLookup();
        }

        private void txtLookup_EnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            this.ansList.Buttons[10].Enabled = false;

            DoLookup();
        }

        private void cboViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter = String.Empty;
            String today = DateTime.Now.ToString("yyyy-MM-dd");
            int selectedIndex = cboViews.SelectedIndex;

            if (selectedIndex >= 0)
            {
                switch (selectedIndex)
                {
                    case 1:         // 7 dyas
                        filter = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) <= '{0}')", today);
                        break;
                    case 2:         // 30 days
                        filter = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) <= '{0}')", today);
                        break;
                    case 3:         // 60 days
                        filter = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) <= '{0}')", today);
                        break;
                    case 4:         // 90 days
                        filter = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        _CurSqlWhere = _BaseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [QTDate], 20) <= '{0}')", today);
                        break;
                    case 0:
                    default:
                        _CurSqlWhere = _BaseSqlWhere;
                        break;
                }

                BindList();
                txtLookup.Text = String.Empty;
                cboViews.Text = cboViews.SelectedItem.ToString();
                this.Update();
            }
        }

        private void tvwCustomer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;
            this.ansList.Buttons[12].Enabled = false;

            if (!(e.Node.HasNodes))
            {
                _CustomerName = e.Node.Text;
                _CustomerId = (Guid)e.Node.Tag;
                _CurSqlWhere = _BaseSqlWhere + String.Format(@" AND ( [CustomerId] = '{0}' )", _CustomerId.ToString());
                BindList();
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }

        void quotation_Closed(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = true;
            this.ansList.Buttons[12].Enabled = true;
        }

        private void cboViews_GotFocus(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;
            this.ansList.Buttons[12].Enabled = false;
        }

        private void txtLookup_GotFocus(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItem != null)
            {
                this.ansList.Buttons[10].Enabled = true;
                this.ansList.Buttons[12].Enabled = true;
                quotationNum = this.lvwList.SelectedItem.Text;
            }
        }

        private void lvwList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 參考：https://stackoverflow.com/a/1214333
            Sorter s = (Sorter)lvwList.ListViewItemSorter;
            s.Column = e.Column;

            if (s.Order == Gizmox.WebGUI.Forms.SortOrder.Ascending)
            {
                s.Order = Gizmox.WebGUI.Forms.SortOrder.Descending;
            }
            else
            {
                s.Order = Gizmox.WebGUI.Forms.SortOrder.Ascending;
            }
            lvwList.Sort();
        }

        private void ShowAttachmentManager()
        {
            if (lvwList.SelectedItem != null)
            {
                quotationNum = this.lvwList.SelectedItem.Text;
                Guid itemId = new Guid(lvwList.SelectedItem.SubItems[1].Text);

                AttachmentManager attachWizard = new AttachmentManager();
                attachWizard.OrderId = itemId;
                attachWizard.OrderNumber = quotationNum;
                attachWizard.ShowDialog();
            }
        }
    }
}
