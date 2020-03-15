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

namespace xPort5.Order.Coding.Customer.Contact
{
    public partial class ContactList : UserControl
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _CustomerId = System.Guid.Empty;
        private string _CustomerCode = String.Empty;

        public ContactList()
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
        public Guid CustomerId
        {
            get
            {
                return _CustomerId;
            }
            set
            {
                _CustomerId = value;
            }
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttribute();
            SetCaptions();
            SetPackageAns();
            SetLvwList();

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                BindList();
            }
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.colFullName.Text = oDict.GetWord("contact_name");
            this.colJobTitle.Text = oDict.GetWord("job_title");
            this.colPhone1.Text = oDict.GetWord("direct_line");
            this.colPhone2.Text = oDict.GetWord("home");
            this.colPhone3.Text = oDict.GetWord("mobile");
            this.colPhone4.Text = oDict.GetWord("email");
            this.colPhone5.Text = oDict.GetWord("other");
            this.colNotes.Text = oDict.GetWord("note");
            this.colModifiedOn.Text = oDict.GetWord("modified_on");
            this.colModifiedBy.Text = oDict.GetWord("modified_by");
        }

        private void SetLvwList()
        {
            this.lvwAddress.ListViewItemSorter = new ListViewItemSorter(this.lvwAddress);
            this.lvwAddress.Dock = DockStyle.Fill;
            this.lvwAddress.GridLines = true;

            //提供一個固定的 Guid tag， 在 UserPreference 中用作這個 ListView 的 unique key
            lvwAddress.Tag = new Guid("C02A7941-A449-4182-A7B4-9A3B01B8D082");

            xPort5.Controls.Utility.DisplayPreference.Load(ref lvwAddress);
        }

        private void SetAttribute()
        {
            this.lvwAddress.ListViewItemSorter = new ListViewItemSorter(this.lvwAddress);

            string[] phones = xPort5.Controls.Utility.Default.PhoneLabels4Contact();
            colPhone1.Text = phones[0];
            colPhone2.Text = phones[1];
            colPhone3.Text = phones[2];
            colPhone4.Text = phones[3];
            colPhone5.Text = phones[4];

            toolTip1.SetToolTip(this.lvwAddress, "Double click to open record details");
        }

        private void SetPackageAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict,Common.Config.CurrentLanguageId);

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
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Coding.Customer.Create"))
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
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Coding.Customer.Delete"))
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

        private void BindList()
        {
            this.lvwAddress.Items.Clear();

            int iCount = 1;
            string sql = String.Format(@"
SELECT TOP 100 PERCENT
       [CustomerContactId]                      -- 0
      ,[CustomerId]
      ,[DefaultRec]
      ,[SalutationName]
      ,[FullName]
      ,[FirstName]                              -- 5
      ,[LastName]
      ,[JobTitleName]
      ,[PhoneLabel1]
      ,[Phone1_Text]
      ,[PhoneLabel2]                            -- 10
      ,[Phone2_Text]
      ,[PhoneLabel3]
      ,[Phone3_Text]
      ,[PhoneLabel4]
      ,[Phone4_Text]                            -- 15
      ,[PhoneLabel5]
      ,[Phone5_Text]
      ,ISNULL([Notes], '')
      ,CONVERT(NVARCHAR(16), [CreatedOn], 120)
      ,ISNULL([CreatedBy], '')                  -- 20
      ,CONVERT(NVARCHAR(16), [ModifiedOn], 120)
      ,ISNULL([ModifiedBy], '')
      ,[Retired]
FROM [dbo].[vwCustomerContactList]
WHERE [CustomerId] = '{0}'
ORDER BY [FullName]
", _CustomerId.ToString());
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                Guid cContactId = reader.GetGuid(0);
                CustomerContact cContact = CustomerContact.Load(cContactId);

                ListViewItem objItem = this.lvwAddress.Items.Add(iCount.ToString());  // Line Number
                #region Address Image
                if (cContact.DefaultRec)
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.contactsingle_key_16.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.contactsingle_key_16.png");
                }
                else
                {
                    objItem.SmallImage = new IconResourceHandle("16x16.contactsingle_16.png");
                    objItem.LargeImage = new IconResourceHandle("16x16.contactsingle_16.png");
                }
                #endregion
                objItem.SubItems.Add(reader.GetGuid(0).ToString());     // Customer Contact Id
                objItem.SubItems.Add(reader.GetString(4));              // Full Name
                objItem.SubItems.Add(reader.GetString(7));              // Job Title
                objItem.SubItems.Add(reader.GetString(9));              // Phone 1
                objItem.SubItems.Add(reader.GetString(11));             // Phone 2
                objItem.SubItems.Add(reader.GetString(13));             // Phone 3
                objItem.SubItems.Add(reader.GetString(15));             // Phone 4
                objItem.SubItems.Add(reader.GetString(17));             // Phone 5
                objItem.SubItems.Add(reader.GetString(18));             // Notes
                objItem.SubItems.Add(reader.GetString(21));             // Modified On
                objItem.SubItems.Add(reader.GetString(22));             // Modified By

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
                        BindList();
                        this.Update();
                        break;
                    case "new":
                        ContactRecord contact = new ContactRecord();
                        contact.EditMode = Common.Enums.EditMode.Add;
                        contact.CustomerId = _CustomerId;
                        contact.ShowDialog();
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

                    ContactRecord contact = new ContactRecord();

                    contact.EditMode = Common.Enums.EditMode.Edit;
                    contact.CustomerId = _CustomerId;
                    contact.CustomerContactId = uAddressId;
                    contact.ShowDialog();
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