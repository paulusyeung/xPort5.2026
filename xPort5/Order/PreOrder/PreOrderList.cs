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
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms.Dialogs;
using System.Data.SqlClient;
using xPort5.Controls;
using xPort5.Helper;

#endregion

namespace xPort5.Order.PreOrder
{
    public partial class PreOrderList : UserControl
    {
        private string baseSqlSelect = string.Empty;
        private string baseSqlWhere = string.Empty;
        private string curSqlWhere = string.Empty;
        private string customerId = string.Empty;

        private static string plNumber;

        public static string PLNumber
        {
            get
            {
                return plNumber;
            }
            set
            {
                plNumber = value;
            }
        }

        public PreOrderList()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttributes();
            LoadComboBox();
            SetLvwList();
            SetTvwList();
            SetAnsList();

            baseSqlSelect = @"
SELECT TOP 100 PERCENT
       [OrderPLId]
      ,[PLNumber]
      ,CONVERT(NVARCHAR(10), [PLDate], 20)
      ,[CustomerName]
      ,ISNULL([Remarks],'')
      ,[Status]     --5
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
      ,[InUse]     --10
      ,[Revision]
      ,[TotalQty]
FROM [dbo].[vwPreOrderList]
";
            baseSqlWhere = string.Format("WHERE [Status] = {0}", Common.Enums.Status.Active.ToString("d"));
            curSqlWhere = baseSqlWhere;
        }

        #region SetTheme...
        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            toolTip1.SetToolTip(txtPreOrderNo, String.Format(oDict.GetWord("look_for_targets"), "; ", "PL Number", "Customer Code"));
            toolTip1.SetToolTip(btnFind, String.Format(oDict.GetWord("Look_for_targets"), "; ", "PL Number", "Customer Code"));
            toolTip1.SetToolTip(lvwList, oDict.GetWord("double_click_to_open_record"));

            lblLookup.Text = oDict.GetWordWithColon("look_for");
            lblView.Text = oDict.GetWordWithColon("view");

            colItemNumber.Text = oDict.GetWord("pre-order_num");
            colCustomer.Text = oDict.GetWord("customer");
            colRemarks.Text = oDict.GetWord("remarks");
            colCreatedOn.Text = oDict.GetWord("created_on");
            colCreatedBy.Text = oDict.GetWord("created_by");
            colModifiedBy.Text = oDict.GetWord("modified_by");
            colModifiedOn.Text = oDict.GetWord("modified_on");

            splitContainer1.SplitterDistance = 200;
            ansTree.Height = 24;
            ansTree.Dock = DockStyle.Top;
            ansList.Height = 24;
            ansList.Dock = DockStyle.Top;
            tvList.Dock = DockStyle.Fill;
        }

        private void LoadComboBox()
        {
            xPort5.Controls.Utility.ComboBox.LoadViews(ref cboViews);
        }

        private void SetLvwList()
        {
            //this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.ListViewItemSorter = new Sorter();   // 參考：https://stackoverflow.com/a/1214333

            this.lvwList.Dock = DockStyle.Fill;
            this.lvwList.GridLines = true;

            // 2012.04.18 paulus: 提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwList.Tag = new Guid("53000103-0112-4190-8300-14AF067E7EC2");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwList);
        }

        private void SetTvwList()
        {
            xPort5.Controls.Utility.TreeViewControl.Load<Customer>(this.tvList.Nodes);
        }

        private void SetAnsList()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

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
            #endregion

            this.ansList.Buttons.Add(sep);

            #region cmdGenOrder [12]
            this.ansList.Buttons.Add(new ToolBarButton("GenerateOrder", oDict.GetWord("Generate Order")));
            this.ansList.Buttons[12].Image = new IconResourceHandle("16x16.ico_16_3.gif");
            this.ansList.Buttons[12].Visible = false;
            if (xPort5.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PreOrderList.GenOrders"))
                    this.ansList.Buttons[12].Enabled = true;
                else
                    this.ansList.Buttons[12].Enabled = false;
            }
            #endregion

            #region Attachment
            this.ansList.Buttons.Add(new ToolBarButton("Attachment", oDict.GetWord("Attachment")));
            this.ansList.Buttons[13].Image = new IconResourceHandle("16x16.ico_16_1001_d.gif");
            this.ansList.Buttons[13].Enabled = false;
            #endregion

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansList.Buttons.Add(cmdPopup);
            #endregion

            ansList.TextAlign = ToolBarTextAlign.Right;
        }

        #endregion

        private void ansViews_MenuClick(object sender, MenuItemEventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            switch (e.MenuItem.Text)
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
                    case "columns":
                        this.ansList.Buttons[10].Enabled = false;
                        ListViewColumnOptions objListViewColumnOptions = new ListViewColumnOptions(this.lvwList);
                        objListViewColumnOptions.ShowDialog();
                        break;
                    case "sorting":
                        this.ansList.Buttons[10].Enabled = false;
                        ListViewSortingOptions objListViewSortingOption = new ListViewSortingOptions(this.lvwList);
                        objListViewSortingOption.ShowDialog();
                        break;
                    case "checkbox":
                        this.ansList.Buttons[10].Enabled = false;
                        this.lvwList.CheckBoxes = !this.lvwList.CheckBoxes;

                        if (this.lvwList.CheckBoxes)
                        {
                            this.ansList.Buttons[12].Visible = false;
                        }
                        break;
                    case "multiselect":
                        this.ansList.Buttons[10].Enabled = false;
                        this.lvwList.MultiSelect = !this.lvwList.MultiSelect;
                        e.Button.Pushed = true;
                        break;
                    case "refresh":
                        this.ansList.Buttons[10].Enabled = false;
                        RefreshForm();
                        BindList();
                        this.Update();
                        break;
                    case "generateorder":
                        GenerateOrder();
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
            this.ansList.Buttons[13].Enabled = false;

            if (lvwList.SelectedItem != null)
            {
                System.Guid plId = new Guid(this.lvwList.SelectedItem.SubItems[1].Text);
                PreOrderRecord preOrderRecord = new PreOrderRecord();
                preOrderRecord.EditMode = Common.Enums.EditMode.Edit;
                preOrderRecord.OrderPLId = plId;
                preOrderRecord.Show();

                preOrderRecord.Closed += new EventHandler(preOrderRecord_Closed);
            }
        }

        private void ResetForm()
        {
            txtPreOrderNo.Text = string.Empty;
            curSqlWhere = baseSqlWhere;
        }

        private void RefreshForm()
        {
            this.txtPreOrderNo.Text = string.Empty;
        }

        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            
            // Use ViewService instead of direct SQL query
            string whereClause = BuildWhereClause();
            DataSet ds = ViewService.Default.GetPreOrderList(whereClause, "");
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem objItem = this.lvwList.Items.Add(row["PLNumber"].ToString());    //PLNumber
                objItem.SubItems.Add(row["OrderPLId"].ToString());     //Id
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
                decimal totalQty = row["TotalQty"] != DBNull.Value ? Convert.ToDecimal(row["TotalQty"]) : 0;
                switch (Convert.ToInt32(totalQty))
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
                if (xPort5.Controls.Utility.Resources.HasAttachment(row["PLNumber"].ToString())) // Attachment
                {
                    objItem.SubItems.Add(new IconResourceHandle("16x16.ico_16_1001_d.gif").ToString());
                }
                else
                {
                    objItem.SubItems.Add(String.Empty);
                }
                #endregion
                objItem.SubItems.Add(iCount.ToString());                // Line Number
                objItem.SubItems.Add(row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : "");              // CustomerName
                objItem.SubItems.Add(row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : "");              // Remarks
                objItem.SubItems.Add(row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // CreatedOn
                objItem.SubItems.Add(row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : "");              // CreatedBy
                objItem.SubItems.Add(row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]).ToString("yyyy-MM-dd HH:mm") : "");              // ModifiedOn
                objItem.SubItems.Add(row["ModifiedBy"] != DBNull.Value ? row["ModifiedBy"].ToString() : "");             // ModifiedBy

                iCount++;
            }
            
            this.lvwList.Sort();        // 2012.04.18 paulus: 依照當前的 ListView.SortOrder 和 ListView.SortPosition 排序，使 UserPreference 有效
        }

        // Deprecated: Replaced by BuildWhereClause() for ViewService
        private string BuildSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(baseSqlSelect + Environment.NewLine);
            sql.Append(curSqlWhere + Environment.NewLine);

            return sql.ToString();
        }

        /// <summary>
        /// Builds WHERE clause for ViewService (without "WHERE" keyword)
        /// </summary>
        private string BuildWhereClause()
        {
            string whereClause = curSqlWhere;
            
            // Remove "WHERE" keyword if present
            if (whereClause.TrimStart().StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
            {
                whereClause = whereClause.Substring(whereClause.IndexOf("WHERE ", StringComparison.OrdinalIgnoreCase) + 6);
            }
            
            return whereClause.Trim();
        }

        private void DoLookup()
        {
            string target = txtPreOrderNo.Text;
            if (!(string.IsNullOrEmpty(target)))
            {
                ResetForm();
                txtPreOrderNo.Text = target;
                cboViews.SelectedIndex = 0;
                curSqlWhere = baseSqlWhere +
                              String.Format(" AND ([PLNumber] LIKE '%{0}%') ", target);
                BindList();
                this.Update();
            }
        }

        private void GenerateOrder()
        {
            if (lvwList.SelectedItem != null)
            {
                if (Common.Utility.IsGUID(lvwList.SelectedItem.SubItems[1].Text))
                {
                    Order.PreOrder.GenerateOrder orderGenerator = new GenerateOrder();
                    orderGenerator.OrderPLId = new Guid(lvwList.SelectedItem.SubItems[1].Text);
                    orderGenerator.ShowDialog();
                }
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            DoLookup();
        }

        private void cboViews_GotFocus(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;
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
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) <= '{0}')", today);
                        break;
                    case 2:         // 30 days
                        filter = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) <= '{0}')", today);
                        break;
                    case 3:         // 60 days
                        filter = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) <= '{0}')", today);
                        break;
                    case 4:         // 90 days
                        filter = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PLDate], 20) <= '{0}')", today);
                        break;
                    case 0:
                    default:
                        curSqlWhere = baseSqlWhere;
                        break;
                }

                BindList();
                txtPreOrderNo.Text = String.Empty;
                cboViews.Text = cboViews.SelectedItem.ToString();
                this.Update();
            }
        }

        private void txtPreOrderNo_GotFocus(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;
        }

        private void txtPreOrderNo_EnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            this.ansList.Buttons[10].Enabled = false;

            DoLookup();
        }

        private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            if (!(e.Node.HasNodes))
            {
                customerId = e.Node.Tag.ToString();
                curSqlWhere = baseSqlWhere + string.Format(@" AND ([CustomerId] = '{0}') ", customerId);
                BindList();

                this.ansList.Buttons[12].Visible = (lvwList.SelectedItem != null);
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItem != null)
            {
                this.ansList.Buttons[10].Enabled = true;

                if (!lvwList.CheckBoxes)
                {
                    this.ansList.Buttons[12].Visible = true;
                }
                this.ansList.Buttons[13].Enabled = true;

                plNumber = lvwList.SelectedItem.Text;
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
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

        void preOrderRecord_Closed(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = true;
            this.ansList.Buttons[13].Enabled = true;
        }

        private void ShowAttachmentManager()
        {
            if (lvwList.SelectedItem != null)
            {
                string preOrderNumber = this.lvwList.SelectedItem.Text;
                Guid itemId = new Guid(lvwList.SelectedItem.SubItems[1].Text);

                AttachmentManager attachWizard = new AttachmentManager();
                attachWizard.OrderId = itemId;
                attachWizard.OrderNumber = preOrderNumber;
                attachWizard.ShowDialog();
            }
        }
    }
}
