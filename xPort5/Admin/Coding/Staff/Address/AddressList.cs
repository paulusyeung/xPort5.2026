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

namespace xPort5.Admin.Coding.Staff.Address
{
    public partial class AddressList : UserControl
    {
        nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _UserId = System.Guid.Empty;
        private string _UserCode = String.Empty;

        public AddressList()
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
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttribute();
            SetPackageAns();
            SetCaptions();
            SetLvwList();

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                BindAddressList();
            }
        }

        private void SetCaptions()
        {
            colAddress.Text = oDict.GetWord("address");
            colAddressName.Text = oDict.GetWord("addresstype");
            colPhone1.Text = oDict.GetWord("direct_line");
            colPhone2.Text = oDict.GetWord("home");
            colPhone3.Text = oDict.GetWord("mobile");
            colPhone4.Text = oDict.GetWord("email");
            colPhone5.Text = oDict.GetWord("other");
            colNotes.Text = oDict.GetWord("note");
            colModifiedOn.Text = oDict.GetWord("modified_on");
            colModifiedBy.Text = oDict.GetWord("modified_by");
        }

        private void SetLvwList()
        {
            this.lvwAddress.ListViewItemSorter = new ListViewItemSorter(this.lvwAddress);
            this.lvwAddress.Dock = DockStyle.Fill;
            this.lvwAddress.GridLines = true;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            this.lvwAddress.Tag = new Guid("DC74B674-DB08-4d61-92D2-10AD8D44073E");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwAddress);
        }

        private void SetAttribute()
        {
            this.lvwAddress.ListViewItemSorter = new ListViewItemSorter(this.lvwAddress);

            string[] phones = xPort5.Controls.Utility.Default.PhoneLabels4User();
            colPhone1.Text = phones[0];
            colPhone2.Text = phones[1];
            colPhone3.Text = phones[2];
            colPhone4.Text = phones[3];
            colPhone5.Text = phones[4];

            toolTip1.SetToolTip(this.lvwAddress, oDict.GetWord("double_click_to_open_record"));
        }

        private void SetPackageAns()
        {
            this.ansAddress.MenuHandle = false;
            this.ansAddress.DragHandle = false;
            this.ansAddress.TextAlign = ToolBarTextAlign.Right;

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

            if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Admin", "Coding.Staff.Create"))
                {
                    this.ansAddress.Buttons.Add(cmdNew);
                }
            }
            else
            {
                this.ansAddress.Buttons.Add(cmdNew);
            }

            this.ansAddress.Buttons.Add(cmdRefresh);

            #region cmdPreference 
            ContextMenu ddlPreference = new ContextMenu();
            Common.Data.AppendMenuItem_AppPref(ref ddlPreference);
            ToolBarButton cmdPreference = new ToolBarButton("Preference", oDict.GetWord("Preference"));
            cmdPreference.Style = ToolBarButtonStyle.DropDownButton;
            cmdPreference.Image = new IconResourceHandle("16x16.ico_16_1039_default.gif");
            cmdPreference.DropDownMenu = ddlPreference;
            this.ansAddress.Buttons.Add(cmdPreference);
            cmdPreference.MenuClick += new MenuEventHandler(ansPreference_MenuClick);
            #endregion

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Admin", "Coding.Staff.Delete"))
                    {
                        this.ansAddress.Buttons.Add(cmdDelete);
                    }
                }
                else
                {
                    this.ansAddress.Buttons.Add(cmdDelete);
                }
            }

            #region cmdPopup
            ToolBarButton cmdPopup = new ToolBarButton("Popup", oDict.GetWord("popup"));
            cmdPopup.Image = new IconResourceHandle("16x16.popup_16x16.gif");

            this.ansAddress.Buttons.Add(cmdPopup);
            #endregion

            this.ansAddress.ButtonClick += new ToolBarButtonClickEventHandler(ansPackage_ButtonClick);
        }

        private void BindAddressList()
        {
            this.lvwAddress.Items.Clear();

            int iCount = 1;
            string sql = String.Format(@"
SELECT TOP 100 PERCENT
       [StaffAddressId]         -- 0
      ,[AddressName]
      ,[DefaultRec]
      ,[AddrText]
      ,[AddrIsMailing]
      ,[PhoneLable1]            -- 5
      ,[Phone1_Text]
      ,[PhoneLabel2]
      ,[Phone2_Text]
      ,[PhoneLabel3]
      ,[Phone3_Text]            -- 10
      ,[PhoneLabel4]
      ,[Phone4_Text]
      ,[PhoneLabel5]
      ,[Phone5_Text]
      ,[Notes]                  -- 15
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,ISNULL([CreatedBy], '')
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,ISNULL([ModifiedBy], '')
      ,[Retired]                -- 20
  FROM [dbo].[vwStaffAddressList]
WHERE [StaffId] = '{0}'
ORDER BY [AddressName]
", _UserId.ToString());
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                Guid uAddressId = reader.GetGuid(0);
                StaffAddress aAddress = StaffAddress.Load(uAddressId);

                ListViewItem objItem = this.lvwAddress.Items.Add(iCount.ToString());  // Line Number
                #region Address Image
                if (aAddress.DefaultRec)
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.vcard_key_16.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.vcard_key_16.png");
                }
                else
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.vcard.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.vcard.png");
                }
                #endregion
                objItem.SubItems.Add(reader.GetGuid(0).ToString());     // User Address Id
                objItem.SubItems.Add(reader.GetString(1));              // Address Name
                #region Mailing Image
                if (aAddress.AddrIsMailing)
                {
                    objItem.SubItems.Add(new IconResourceHandle("16x16.mailing_on_16.png").ToString());
                }
                else
                {
                    objItem.SubItems.Add(String.Empty);
                }
                #endregion
                objItem.SubItems.Add(reader.GetString(3));              // Address
                objItem.SubItems.Add(reader.GetString(6));              // Phone 1
                objItem.SubItems.Add(reader.GetString(8));              // Phone 2
                objItem.SubItems.Add(reader.GetString(10));             // Phone 3
                objItem.SubItems.Add(reader.GetString(12));             // Phone 4
                objItem.SubItems.Add(reader.GetString(14));             // Phone 5
                objItem.SubItems.Add(reader.GetString(15));             // Notes
                objItem.SubItems.Add(reader.GetString(18));             // Modified On
                objItem.SubItems.Add(reader.GetString(19));             // Modified By

                iCount++;
            }
            reader.Close();

            this.lvwAddress.Sort();
        }

        private void ansPackage_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "refresh":
                        BindAddressList();
                        this.Update();
                        break;
                    case "new":
                        AddressRecord address = new AddressRecord();
                        address.EditMode = Common.Enums.EditMode.Add;
                        address.UserId = _UserId;
                        address.ShowDialog();
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
            if (lvwAddress.SelectedItem != null)
            {
                string productCode = lvwAddress.SelectedItem.Text;
                if (Common.Utility.IsGUID(lvwAddress.SelectedItem.SubItems[1].Text))
                {
                    Guid uAddressId = new Guid(lvwAddress.SelectedItem.SubItems[1].Text);

                    AddressRecord address = new AddressRecord();

                    address.EditMode = Common.Enums.EditMode.Edit;
                    address.UserId = _UserId;
                    address.UserAddressId = uAddressId;
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
                    xPort5.Controls.Utility.DisplayPreference.Save(lvwAddress);
                    break;
                case "Reset":
                    xPort5.Controls.Utility.DisplayPreference.Delete(lvwAddress);
                    break;
            }
            MessageBox.Show(oDict.GetWord("finish"));
        }

        #region ans Button Clicks: Delete
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (lvwAddress.CheckBoxes && lvwAddress.CheckedIndices.Count > 0)
                {
                    foreach (ListViewItem item in lvwAddress.CheckedItems)
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
                    if (lvwAddress.SelectedIndex >= 0)
                    {
                        if (Common.Utility.IsGUID(lvwAddress.SelectedItem.SubItems[1].Text))
                        {
                            Guid aSupplierId = new Guid(lvwAddress.SelectedItem.SubItems[1].Text);

                            if (xPort5.Controls.Utility.ProductSupplier.DeleteRec(aSupplierId))
                            {
                                lvwAddress.Items[lvwAddress.SelectedIndex].Remove();
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