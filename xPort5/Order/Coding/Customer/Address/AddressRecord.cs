#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;
using System.Data.SqlClient;

#endregion

namespace xPort5.Order.Coding.Customer.Address
{
    public partial class AddressRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _CustomerId = System.Guid.Empty;
        private string _AddressName = String.Empty;
        private Guid _CustomerAddressId = System.Guid.Empty;

        public AddressRecord()
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
        public Guid CustomerAddressId
        {
            get
            {
                return _CustomerAddressId;
            }
            set
            {
                _CustomerAddressId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("address")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.lblCustomerCode.Text = oDict.GetWordWithColon("customer_code");
            this.lblCustomerName.Text = oDict.GetWordWithColon("customer_name");
            this.lblAddressName.Text = oDict.GetWordWithColon("addresstype");
            this.lblAddress.Text = oDict.GetWordWithColon("address");
            this.chkIsMailing.Text = oDict.GetWord("mailing_address");
            this.lblPhone1.Text = oDict.GetWordWithColon("business");
            this.lblPhone2.Text = oDict.GetWord("business")+"2��";
            this.lblPhone3.Text = oDict.GetWordWithColon("fax");
            this.lblPhone4.Text = oDict.GetWordWithColon("website");
            this.lblPhone5.Text = oDict.GetWordWithColon("other");
            this.lblNotes.Text = oDict.GetWordWithColon("note");
            this.boxLogs.Text = oDict.GetWord("logs");
            this.lblCreatedOn.Text = oDict.GetWordWithColon("created_on");
            this.lblCreatedBy.Text = oDict.GetWordWithColon("created_by");
            this.lblModifiedOb.Text = oDict.GetWordWithColon("modified_on");
            this.lblModifiedBy.Text = oDict.GetWordWithColon("modified_by");
        }

        private void SetAttributes()
        {
            string[] phoneLabels = xPort5.Controls.Utility.Default.PhoneLabels4Customer();
            lblPhone1.Text = phoneLabels[0] + ":";
            lblPhone2.Text = phoneLabels[1] + ":";
            lblPhone3.Text = phoneLabels[2] + ":";
            lblPhone4.Text = phoneLabels[3] + ":";
            lblPhone5.Text = phoneLabels[4] + ":";

            //toolTip1.SetToolTip(txtAddress, "Numeric only");

            txtUserCode.Enabled = false;
            txtUserAlias.Enabled = false;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    break;
            }
        }

        private void SetDropdowns()
        {
            xPort5.EF6.Z_Address.LoadCombo(ref cboAddress, "AddressName", false);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Coding.Customer");

            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
        }
        #endregion

        private void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString().ToLower())
                {
                    case "save":
                        MessageBox.Show("Save Item?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSave_Click));
                        break;
                    case "save & close":
                        MessageBox.Show("Save Item And Close?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveClose_Click));
                        break;
                    case "save & dup":
                        MessageBox.Show("Save Item And Dup?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveDup_Click));
                        break;
                    case "save & new":
                        MessageBox.Show("Save Item And New?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveNew_Click));
                        break;
                    case "delete":
                        MessageBox.Show("Delete Item?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                }
            }
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowCustomer()
        {
            xPort5.EF6.Customer customer = xPort5.EF6.Customer.Load(_CustomerId);
            if (customer != null)
            {
                txtUserCode.Text = customer.CustomerCode;
                txtUserAlias.Text = customer.CustomerName;
            }
        }

        private void ShowCustomerAddress()
        {
            CustomerAddress uAddress = CustomerAddress.Load(_CustomerAddressId);
            if (uAddress != null)
            {
                txtAddress.Text = uAddress.AddrText;
                chkIsMailing.Checked = uAddress.AddrIsMailing;
                txtPhone1.Text = uAddress.Phone1_Text;
                txtPhone2.Text = uAddress.Phone2_Text;
                txtPhone3.Text = uAddress.Phone3_Text;
                txtPhone4.Text = uAddress.Phone4_Text;
                txtPhone5.Text = uAddress.Phone5_Text;
                txtNotes.Text = uAddress.Notes;

                Z_Address addr = Z_Address.Load(uAddress.AddressId);
                if (addr != null)
                {
                    cboAddress.Text = addr.AddressName;
                    cboAddress.SelectedValue = addr.AddressId;
                }

                Staff s1 = Staff.Load(uAddress.CreatedBy);
                Staff s2 = Staff.Load(uAddress.ModifiedBy);
                txtCreatedOn.Text = uAddress.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                if (s1 == null)
                    txtCreatedBy.Text = String.Empty;
                else
                    txtCreatedBy.Text = s1.Alias;
                txtModifiedOn.Text = uAddress.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
                if (s2 == null)
                    txtModifiedBy.Text = String.Empty;
                else
                    txtModifiedBy.Text = s2.Alias;
                boxLogs.Visible = true;

                _AddressName = addr.AddressName;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    CustomerAddress uAddress = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            uAddress = new CustomerAddress();
                            uAddress.CustomerId = _CustomerId;
                            uAddress.CreatedOn = DateTime.Now;
                            uAddress.CreatedBy = Common.Config.CurrentUserId;
                            uAddress.ModifiedOn = DateTime.Now;
                            uAddress.ModifiedBy = Common.Config.CurrentUserId;
                            uAddress.Retired = false;
                            if (xPort5.Controls.Utility.CustomerAddress.Count(_CustomerId) == 0)
                            {
                                uAddress.DefaultRec = true;
                            }
                            else
                            {
                                uAddress.DefaultRec = false;
                            }

                            Guid[] labelIds = xPort5.Controls.Utility.Default.PhoneLabelIds4Customer();
                            uAddress.Phone1_Label = labelIds[0];
                            uAddress.Phone2_Label = labelIds[1];
                            uAddress.Phone3_Label = labelIds[2];
                            uAddress.Phone4_Label = labelIds[3];
                            uAddress.Phone5_Label = labelIds[4];
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            uAddress = CustomerAddress.Load(_CustomerAddressId);
                            uAddress.ModifiedOn = DateTime.Now;
                            uAddress.ModifiedBy = Common.Config.CurrentUserId;
                            break;
                    }

                    System.Guid addressId = (Guid)cboAddress.SelectedValue;
                    Z_Address addr = Z_Address.Load(addressId);
                    uAddress.AddressId = addressId;
                    _AddressName = addr.AddressName;

                    uAddress.AddrText = txtAddress.Text.Trim();
                    uAddress.AddrIsMailing = chkIsMailing.Checked;
                    uAddress.Phone1_Text = txtPhone1.Text.Trim();
                    uAddress.Phone2_Text = txtPhone2.Text.Trim();
                    uAddress.Phone3_Text = txtPhone3.Text.Trim();
                    uAddress.Phone4_Text = txtPhone4.Text.Trim();
                    uAddress.Phone5_Text = txtPhone5.Text.Trim();
                    uAddress.Notes = txtNotes.Text.Trim();

                    uAddress.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, uAddress.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, uAddress.ToString());
                    #endregion

                    if (uAddress.CustomerAddressId != _CustomerAddressId)
                    {
                        _CustomerAddressId = uAddress.CustomerAddressId;
                    }
                    result = true;
                }
                catch { }
            }

            return result;
        }

        private bool VerifyItem()
        {
            bool result = true;
            string errMsg = String.Empty;
            string sql = String.Empty;

            if (_EditMode == Common.Enums.EditMode.Add)
            {
                #region validate Address Name
                try
                {
                    Z_Address addr = Z_Address.Load((Guid)cboAddress.SelectedValue);
                    sql = String.Format("CustomerId = '{0}' AND AddressId = '{1}'",
                        _CustomerId.ToString(),
                        addr.AddressId.ToString());
                    xPort5.EF6.CustomerAddress cAddress = xPort5.EF6.CustomerAddress.LoadWhere(sql);
                    if (cAddress != null)
                    {
                        errMsg += Environment.NewLine + "Address is in use.";
                        result = false;
                    }
                }
                catch
                {
                    errMsg += Environment.NewLine + "Customer + Address is invalid.";
                    result = false;
                }
                #endregion
            }

            if (!(result))
            {
                MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        private bool DeleteItem()
        {
            return xPort5.Controls.Utility.CustomerAddress.DeleteRec(_CustomerAddressId);
        }
        #endregion

        #region ans Button Clicks: Save, SaveClose, Delete
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Address {0} is saved!", _AddressName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;

                            this.Update();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is ReadOnly...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdSaveClose_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Address {0} is saved!", _AddressName), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is Read Only...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdSaveDup_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Address {0} is saved!", _AddressName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _CustomerAddressId = System.Guid.NewGuid();
                            _AddressName = string.Empty;

                            this.Update();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is ReadOnly...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdSaveNew_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Address {0} is saved!", _AddressName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _CustomerAddressId = System.Guid.NewGuid();

                            this.Update();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is ReadOnly...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (DeleteItem())
                    {
                        MessageBox.Show(String.Format("Address {0} is deleted.", _AddressName), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
                    }
                    else
                    {
                        MessageBox.Show("This record is protected...You can not cancel this record!\nPlease review the item status.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is Read Only...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdCloseForm(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void AddressRecord_Load(object sender, EventArgs e)
        {
            SetAnsToolbar();
            SetAttributes();
            SetCaptions();
            SetDropdowns();
            ShowCustomer();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _CustomerAddressId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowCustomerAddress();
                    break;
            }
        }
    }
}
