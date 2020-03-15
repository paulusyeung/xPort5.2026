#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common.Resources;

using xPort5.DAL;

#endregion

namespace xPort5.Order.Coding.Product.Supplier
{
    public partial class SupplierList : UserControl
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _ProductId = System.Guid.Empty;
        private string _ProductCode = String.Empty;

        public SupplierList()
        {
            InitializeComponent();
        }

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
        public Guid ProductId
        {
            get
            {
                return _ProductId;
            }
            set
            {
                _ProductId = value;
            }
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();
            SetAttribute();
            SetPackageAns();
            SetLvwList();

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                BindSupplierList();
            }
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            colSupplier.Text = oDict.GetWord("supplier");
            colRefNumber.Text = oDict.GetWord("ref_number");
            colFCL.Text = oDict.GetWord("fcl_cost");
            colLCL.Text = oDict.GetWord("lcl_cost");
            colUnitCode.Text = oDict.GetWord("unit_cost");
            colCurrency.Text = oDict.GetWord("currency");
            colNotes.Text = oDict.GetWord("note");
            colModifiedOn.Text = oDict.GetWord("modified_on");
            colModifiedBy.Text = oDict.GetWord("modified_by");

            this.lvwSupplier.Tag = new Guid("529C001B-1BCB-44cc-9480-30F990CA1CEC");
        }

        private void SetLvwList()
        {
            this.lvwSupplier.ListViewItemSorter = new ListViewItemSorter(this.lvwSupplier);
            this.lvwSupplier.Dock = DockStyle.Fill;
            this.lvwSupplier.GridLines = true;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwSupplier.Tag = new Guid("529C001B-1BCB-44cc-9480-30F990CA1CEC");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwSupplier);
        }

        private void SetAttribute()
        {
            this.lvwSupplier.ListViewItemSorter = new ListViewItemSorter(this.lvwSupplier);

            toolTip1.SetToolTip(this.lvwSupplier, "Double click to open record details");
        }

        private void SetPackageAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansSupplier.MenuHandle = false;
            this.ansSupplier.DragHandle = false;
            this.ansSupplier.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            // cmdSave
            ToolBarButton cmdNew = new ToolBarButton("New", oDict.GetWord("New"));
            cmdNew.Tag = "New";
            cmdNew.Image = new IconResourceHandle("16x16.ico_16_3.gif");
            if (_EditMode == Common.Enums.EditMode.Read)
                cmdNew.Enabled = false;

            // cmdRefresh
            ToolBarButton cmdRefresh = new ToolBarButton("Refresh", oDict.GetWord("Refresh"));
            cmdRefresh.Tag = "Refresh";
            cmdRefresh.Image = new IconResourceHandle("16x16.16_L_refresh.gif");

            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("Delete"));
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            this.ansSupplier.Buttons.Add(cmdNew);
            this.ansSupplier.Buttons.Add(cmdRefresh);

            #region cmdPreference
            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansSupplier.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                this.ansSupplier.Buttons.Add(cmdDelete);
            }

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansSupplier.Buttons.Add(cmdPopup);
            #endregion

            this.ansSupplier.ButtonClick += new ToolBarButtonClickEventHandler(ansPackage_ButtonClick);
        }

        private void BindSupplierList()
        {
            this.lvwSupplier.Items.Clear();

            int iCount = 1;
            string sql = String.Format(@"
SELECT [ArticleId]
      ,[ArticleSupplierId]
      ,[SupplierId]
      ,[SupplierName]
      ,[DefaultRec]
      ,[SuppRef]
      ,[CurrencyCode]
      ,[FCLCost]
      ,[LCLCost]
      ,[UnitCost]
      ,[Notes]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
FROM [dbo].[vwProductSupplier]
WHERE [ArticleId] = '{0}'
ORDER BY [SupplierName]
", _ProductId.ToString());
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                Guid aSupplierId = reader.GetGuid(1);
                ArticleSupplier aSupplier = ArticleSupplier.Load(aSupplierId);

                ListViewItem objItem = this.lvwSupplier.Items.Add(iCount.ToString());  // Line Number
                #region Supplier Image
                if (aSupplier.DefaultRec)
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.supplierKey_16.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.supplierKey_16.png");
                }
                else
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.supplierSingle_16.gif");
                    objItem.LargeImage = new IconResourceHandle("16x16.supplierSingle_16.gif");
                }
                #endregion
                objItem.SubItems.Add(reader.GetGuid(1).ToString());    // Supplier Id
                objItem.SubItems.Add(reader.GetString(3));  // Supplier Name
                objItem.SubItems.Add(reader.GetString(5));  // Ref. No.
                objItem.SubItems.Add(reader.GetDecimal(7).ToString("#,##0.0000"));    // FCL
                objItem.SubItems.Add(reader.GetDecimal(8).ToString("#,##0.0000"));    // LCL
                objItem.SubItems.Add(reader.GetDecimal(9).ToString("#,##0.0000"));    // Unit Cost
                objItem.SubItems.Add(reader.GetString(6));  // Unit
                objItem.SubItems.Add(reader.GetString(10)); // Notes
                objItem.SubItems.Add(reader.GetString(13)); // Modified On
                objItem.SubItems.Add(reader.GetString(14)); // Modified By

                iCount++;
            }
            reader.Close();

            this.lvwSupplier.Sort();
        }

        private void ansPackage_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        BindSupplierList();
                        this.Update();
                        break;
                    case "new":
                        SupplierRecord supplier = new SupplierRecord();
                        supplier.EditMode = Common.Enums.EditMode.Add;
                        supplier.ProductId = _ProductId;
                        supplier.ShowDialog();
                        break;
                    case "delete":
                        MessageBox.Show("Are you sure to delete the selected records?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                    case "popup":
                        ShowRecord();
                        break;
                }
            }
        }

        private void ShowRecord()
        {
            if (lvwSupplier.SelectedItem != null)
            {
                string productCode = lvwSupplier.SelectedItem.Text;
                if (Common.Utility.IsGUID(lvwSupplier.SelectedItem.SubItems[1].Text))
                {
                    Guid prodSupplierId = new Guid(lvwSupplier.SelectedItem.SubItems[1].Text);

                    SupplierRecord oSupplierRecord = new SupplierRecord();

                    oSupplierRecord.EditMode = Common.Enums.EditMode.Edit;
                    oSupplierRecord.ProductId = _ProductId;
                    oSupplierRecord.ProdSupplierId = prodSupplierId;
                    oSupplierRecord.ShowDialog();
                }
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwSupplier);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwSupplier);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        #region ans Button Clicks: Delete
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (lvwSupplier.CheckBoxes && lvwSupplier.CheckedIndices.Count > 0)
                {
                    foreach (ListViewItem item in lvwSupplier.CheckedItems)
                    {
                        if (Common.Utility.IsGUID(item.SubItems[1].Text))
                        {
                            Guid aSupplierId = new Guid(item.SubItems[1].Text);

                            if (xPort5.Controls.Utility.ProductSupplier.DeleteRec(aSupplierId))
                            {
                                item.Remove();
                            }
                        }
                    }
                }
                else
                {
                    if (lvwSupplier.SelectedIndex >= 0)
                    {
                        if (Common.Utility.IsGUID(lvwSupplier.SelectedItem.SubItems[1].Text))
                        {
                            Guid aSupplierId = new Guid(lvwSupplier.SelectedItem.SubItems[1].Text);

                            if (xPort5.Controls.Utility.ProductSupplier.DeleteRec(aSupplierId))
                            {
                                lvwSupplier.Items[lvwSupplier.SelectedIndex].Remove();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        private void lvwSupplier_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }
    }
}