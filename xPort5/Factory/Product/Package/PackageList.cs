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

namespace xPort5.Factory.Product.Package
{
    public partial class PackageList : UserControl
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _ProductId = System.Guid.Empty;
        private string _ProductCode = String.Empty;

        public PackageList()
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
                BindPackageList();
            }
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            colDescription.Text = oDict.GetWord("product_description");
            colUnit.Text = oDict.GetWord("unit");
            colInnerBox.Text = oDict.GetWord("inner_box");
            colOuterBox.Text = oDict.GetWord("outer_box");
            colVolumn.Text = oDict.GetWord("volumn");
            colGross.Text = oDict.GetWord("gross_weight");
            colNet.Text = oDict.GetWord("net_weight");
            colModifiedOn.Text = oDict.GetWord("modified_on");
            colModifiedBy.Text = oDict.GetWord("modified_by");
        }

        private void SetLvwList()
        {
            this.lvwPackage.ListViewItemSorter = new ListViewItemSorter(this.lvwPackage);
            this.lvwPackage.Dock = DockStyle.Fill;
            this.lvwPackage.GridLines = true;

            //�ṩһ���̶��� Guid tag�� �� UserPreference �������@�� ListView �� unique key
            lvwPackage.Tag = new Guid("8800BDDE-516A-417b-ADBE-4385B49E397C");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwPackage);
        }

        private void SetAttribute()
        {
            this.lvwPackage.ListViewItemSorter = new ListViewItemSorter(this.lvwPackage);

            toolTip1.SetToolTip(this.lvwPackage, "Double click to open record details");
        }

        private void SetPackageAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansPackage.MenuHandle = false;
            this.ansPackage.DragHandle = false;
            this.ansPackage.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            // cmdNew
            ToolBarButton cmdNew = new ToolBarButton("New", oDict.GetWord("addnew"));
            cmdNew.Tag = "New";
            cmdNew.Image = new IconResourceHandle("16x16.ico_16_3.gif");
            if (_EditMode == Common.Enums.EditMode.Read)
                cmdNew.Enabled = false;

            // cmdRefresh
            ToolBarButton cmdRefresh = new ToolBarButton("Refresh", oDict.GetWord("refresh"));
            cmdRefresh.Tag = "Refresh";
            cmdRefresh.Image = new IconResourceHandle("16x16.16_L_refresh.gif");

            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("delete"));
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Factory", "Product.Create"))
                {
                    this.ansPackage.Buttons.Add(cmdNew);
                }
            }
            else
            {
                this.ansPackage.Buttons.Add(cmdNew);
            }

            this.ansPackage.Buttons.Add(cmdRefresh);

            #region cmdPreference    - Buttons[6]
            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansPackage.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Factory", "Product.Delete"))
                    {
                        this.ansPackage.Buttons.Add(cmdDelete);
                    }
                }
                else
                {
                    this.ansPackage.Buttons.Add(cmdDelete);
                }
            }

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansPackage.Buttons.Add(cmdPopup);
            #endregion

            this.ansPackage.ButtonClick += new ToolBarButtonClickEventHandler(ansPackage_ButtonClick);
        }

        private void BindPackageList()
        {
            this.lvwPackage.Items.Clear();

            int iCount = 1;
            string sql = String.Format(@"
SELECT [ArticleId]
      ,[ArticlePackageId]
      ,[PackageId]
      ,[PackageName]
      ,[DefaultRec]
      ,[UomId]                  -- 5
      ,[UomName]
      ,[InnerBox]
      ,[OuterBox]
      ,[CUFT]
      ,[SizeLength_in]          -- 10
      ,[SizeWidth_in]
      ,[SizeHeight_in]
      ,[SizeLength_cm]
      ,[SizeWidth_cm]
      ,[SizeHeight_cm]          -- 15
      ,[WeightGross_lb]
      ,[WeightNet_lb]
      ,[WeightGross_kg]
      ,[WeightNet_kg]
      ,[ContainerQty]           -- 20
      ,[ContainerSize]
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,[CreatedBy]
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,[ModifiedBy]
FROM [dbo].[vwProductPackage]
WHERE [ArticleId] = '{0}'
ORDER BY [PackageName], [DefaultRec] DESC
", _ProductId.ToString());
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                Guid aPackageId = reader.GetGuid(1);
                ArticlePackage aPackage = ArticlePackage.Load(aPackageId);

                ListViewItem objItem = this.lvwPackage.Items.Add(iCount.ToString());  // Line Number
                #region Package Image
                if (aPackage.DefaultRec)
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.packageKey_16.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.packageKey_16.png");
                }
                else
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.package_16.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.package_16.png");
                }
                #endregion
                objItem.SubItems.Add(reader.GetGuid(1).ToString());    // Package Id
                objItem.SubItems.Add(reader.GetString(3));  // Description
                objItem.SubItems.Add(reader.GetString(6));  // Unit
                objItem.SubItems.Add(reader.GetDecimal(7).ToString("#,##0.00"));    // Inner Box
                objItem.SubItems.Add(reader.GetDecimal(8).ToString("#,##0.00"));    // Outer Box
                objItem.SubItems.Add(reader.GetDecimal(9).ToString("#,##0.00"));    // Volumn
                objItem.SubItems.Add(reader.GetDecimal(18).ToString("#,##0.000"));  // Gross
                objItem.SubItems.Add(reader.GetDecimal(19).ToString("#,##0.000"));  // Net
                objItem.SubItems.Add(reader.GetString(24));  // Modified On
                objItem.SubItems.Add(reader.GetString(25));  // Modified By

                iCount++;
            }
            reader.Close();

            this.lvwPackage.Sort();
        }

        private void ansPackage_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        BindPackageList();
                        this.Update();
                        break;
                    case "new":
                        PackageRecord supplier = new PackageRecord();
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
            if (lvwPackage.SelectedItem != null)
            {
                string productCode = lvwPackage.SelectedItem.Text;
                if (Common.Utility.IsGUID(lvwPackage.SelectedItem.SubItems[1].Text))
                {
                    Guid prodPackageId = new Guid(lvwPackage.SelectedItem.SubItems[1].Text);

                    PackageRecord oPackageRecord = new PackageRecord();

                    oPackageRecord.EditMode = Common.Enums.EditMode.Edit;
                    oPackageRecord.ProductId = _ProductId;
                    oPackageRecord.ProdPackageId = prodPackageId;
                    oPackageRecord.ShowDialog();
                }
            }
        }

        private void ansPreference_MenuClick(object sender, MenuItemEventArgs e)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            switch (e.MenuItem.Tag.ToString())
            {
                case "Save":
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwPackage);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwPackage);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        #region ans Button Clicks: Delete
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (lvwPackage.CheckBoxes && lvwPackage.CheckedIndices.Count > 0)
                {
                    foreach (ListViewItem item in lvwPackage.CheckedItems)
                    {
                        if (Common.Utility.IsGUID(item.SubItems[1].Text))
                        {
                            Guid aPackageId = new Guid(item.SubItems[1].Text);

                            if (xPort5.Controls.Utility.ProductPackage.DeleteRec(aPackageId))
                            {
                                item.Remove();
                            }
                        }
                    }
                }
                else
                {
                    if (lvwPackage.SelectedIndex >= 0)
                    {
                        if (Common.Utility.IsGUID(lvwPackage.SelectedItem.SubItems[1].Text))
                        {
                            Guid aPackageId = new Guid(lvwPackage.SelectedItem.SubItems[1].Text);

                            if (xPort5.Controls.Utility.ProductPackage.DeleteRec(aPackageId))
                            {
                                lvwPackage.Items[lvwPackage.SelectedIndex].Remove();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        private void lvwPackage_DoubleClick(object sender, EventArgs e)
        {
            ShowRecord();
        }
    }
}