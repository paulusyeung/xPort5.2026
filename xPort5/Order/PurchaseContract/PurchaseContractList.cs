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

namespace xPort5.Order.PurchaseContract
{
    public partial class PurchaseContractList : UserControl
    {
        private string baseSqlSelect = string.Empty;
        private string baseSqlWhere = string.Empty;
        private string curSqlWhere = string.Empty;
        private string supplierId = string.Empty;

        #region Properties
        private static string contractNum;
        /// <summary>
        /// Get ListView selectItem (SubItems[0])-SalesContractNum
        /// </summary>
        public static string PurchaseContractNum
        {
            get
            {
                return contractNum;
            }
            set
            {
                contractNum = value;
            }
        }
        #endregion

        public PurchaseContractList()
        {
            InitializeComponent();
        }

        private void PurchaseContractList_Load(object sender, EventArgs e)
        {
            SetAttributes();
            LoadComboBox();
            SetLvwList();
            SetTvwList();
            SetAnsList();

            baseSqlSelect = @"
SELECT TOP 100 PERCENT
       [OrderPCId]
      ,[PCNumber]
      ,CONVERT(NVARCHAR(10), [PCDate], 20)
      ,[SupplierId]
      ,[SalePerson]
      ,ISNULL([Remarks],'')
      ,[Status]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
      ,[InUse]
      ,[Revision]
      ,ISNULL([SampleQty], 0)
FROM [dbo].[vwPurchaseContractList]
";
            baseSqlWhere = string.Format("WHERE [Status] = {0}", Common.Enums.Status.Active.ToString("d"));
            curSqlWhere = baseSqlWhere;
        }

        #region SetTheme...
        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            toolTip1.SetToolTip(txtContractNo, String.Format(oDict.GetWord("look_for_targets"), "; ", "PC Number", "Customer Code"));
            toolTip1.SetToolTip(btnFind, String.Format(oDict.GetWord("look_for_targets"), "; ", "PC Number", "Customer Code"));
            toolTip1.SetToolTip(lvwList, oDict.GetWord("double_click_to_open_record"));

            lblLookup.Text = oDict.GetWordWithColon("look_for");
            lblView.Text = oDict.GetWordWithColon("view");

            colItemNumber.Text = oDict.GetWord("purchase_contract_num");
            colSupplier.Text = oDict.GetWord("supplier");
            colRemarks.Text = oDict.GetWord("remarks");
            colCreatedOn.Text = oDict.GetWord("created_on");
            colCreatedBy.Text = oDict.GetWord("created_by");
            colModifiedBy.Text = oDict.GetWord("modified_by");
            colModifiedOn.Text = oDict.GetWord("modified_on");

            ansTree.TextAlign = ToolBarTextAlign.Right;
            ansTree.Height = 24;
            ansList.TextAlign = ToolBarTextAlign.Right;
            ansList.Height = 24;
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
            lvwList.Tag = new Guid("D9848AFD-E06B-4323-B833-78E5B6F82007");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwList);
        }

        private void SetTvwList()
        {
            xPort5.Controls.Utility.TreeViewControl.Load<Supplier>(this.tvList.Nodes);
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

            #region Attachment
            this.ansList.Buttons.Add(new ToolBarButton("Attachment", oDict.GetWord("Attachment")));
            this.ansList.Buttons[12].Image = new IconResourceHandle("16x16.ico_16_1001_d.gif");
            this.ansList.Buttons[12].Enabled = false;
            #endregion

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansList.Buttons.Add(cmdPopup);
            #endregion
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
                System.Guid pcId = new Guid(this.lvwList.SelectedItem.SubItems[1].Text);
                PurchaseContractRecord purchaseContract = new PurchaseContractRecord();
                purchaseContract.EditMode = Common.Enums.EditMode.Edit;
                purchaseContract.OrderPCId = pcId;
                purchaseContract.Show();

                purchaseContract.Closed += new EventHandler(purchaseContract_Closed);
            }
        }

        private void ResetForm()
        {
            txtContractNo.Text = string.Empty;
            curSqlWhere = baseSqlWhere;
        }

        private void RefreshForm()
        {
            this.txtContractNo.Text = string.Empty;
        }

        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            
            // Use ViewService instead of direct SQL query
            string whereClause = BuildWhereClause();
            DataSet ds = ViewService.Default.GetPurchaseContractList(whereClause, "");
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem objItem = this.lvwList.Items.Add(row["PCNumber"].ToString());    //PCNumber
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
                objItem.SubItems.Add(row["OrderPCId"].ToString());     //Id
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
                if (xPort5.Controls.Utility.Resources.HasAttachment(row["PCNumber"].ToString())) // Attachment
                {
                    objItem.SubItems.Add(new IconResourceHandle("16x16.ico_16_1001_d.gif").ToString());
                }
                else
                {
                    objItem.SubItems.Add(String.Empty);
                }
                #endregion
                objItem.SubItems.Add(iCount.ToString());                // Line Number
                objItem.SubItems.Add(row["SupplierId"] != DBNull.Value ? xPort5.Controls.Utility.Supplier.GetSupplierName((Guid)row["SupplierId"]) : "");              // SupplierName
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
            string target = txtContractNo.Text;
            if (!(string.IsNullOrEmpty(target)))
            {
                ResetForm();
                txtContractNo.Text = target;
                cboViews.SelectedIndex = 0;
                curSqlWhere = baseSqlWhere +
                              String.Format(" AND ([PCNumber] LIKE '%{0}%') ", target);
                BindList();
                this.Update();
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            DoLookup();
        }

        private void txtContractNo_EnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            this.ansList.Buttons[10].Enabled = false;

            DoLookup();
        }

        private void txtContractNo_GotFocus(object sender, EventArgs e)
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
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) <= '{0}')", today);
                        break;
                    case 2:         // 30 days
                        filter = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) <= '{0}')", today);
                        break;
                    case 3:         // 60 days
                        filter = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) <= '{0}')", today);
                        break;
                    case 4:         // 90 days
                        filter = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                        curSqlWhere = baseSqlWhere + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) >= '{0}')", filter) + String.Format(" AND (CONVERT(NVARCHAR(10), [PCDate], 20) <= '{0}')", today);
                        break;
                    case 0:
                    default:
                        curSqlWhere = baseSqlWhere;
                        break;
                }

                BindList();
                txtContractNo.Text = String.Empty;
                cboViews.Text = cboViews.SelectedItem.ToString();
                this.Update();
            }
        }

        private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;

            if (!(e.Node.HasNodes))
            {
                supplierId = e.Node.Tag.ToString();
                curSqlWhere = baseSqlWhere + string.Format(@" AND ([SupplierId] = '{0}') ", supplierId);
                BindList();
            }
        }

        private void cboViews_GotFocus(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = false;
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }

        void purchaseContract_Closed(object sender, EventArgs e)
        {
            this.ansList.Buttons[10].Enabled = true;
            this.ansList.Buttons[12].Enabled = true;
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedIndex != null)
            {
                this.ansList.Buttons[10].Enabled = true;
                this.ansList.Buttons[12].Enabled = true;
                contractNum = lvwList.SelectedItem.Text;
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
                string pcNumber = this.lvwList.SelectedItem.Text;
                Guid itemId = new Guid(lvwList.SelectedItem.SubItems[1].Text);

                AttachmentManager attachWizard = new AttachmentManager();
                attachWizard.OrderId = itemId;
                attachWizard.OrderNumber = pcNumber;
                attachWizard.ShowDialog();
            }
        }
    }
}
