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

namespace xPort5.Order.Coding.Customer
{
    public partial class CustomerRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _CustomerId = System.Guid.Empty;
        private string _CustomerCode = String.Empty;

        public CustomerRecord()
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

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict,Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("customer")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.lblCode.Text = oDict.GetWordWithColon("customer_code");
            this.lblName.Text = oDict.GetWordWithColon("customer_name");
            this.lblName_Chs.Text = oDict.GetWordWithColon("customer_name_chs");
            this.lblName_Cht.Text = oDict.GetWordWithColon("customer_name_cht");
            this.lblAccountName.Text = oDict.GetWordWithColon("account_number");
            this.lblRegion.Text = oDict.GetWordWithColon("customer_region");
            this.lblTerms.Text = oDict.GetWordWithColon("payment_terms");
            this.lblCurrency.Text = oDict.GetWordWithColon("currency");
            this.lblCreditLimit.Text = oDict.GetWordWithColon("credit_limit");
            this.lblProfitMargin.Text = oDict.GetWordWithColon("margin");
            this.lblBlacklisted.Text = oDict.GetWordWithColon("blacklisted");
            this.lblRemarks.Text = oDict.GetWordWithColon("remarks");
            this.lblCreatedOn.Text = oDict.GetWordWithColon("created_on");
            this.lblCreatedBy.Text = oDict.GetWordWithColon("created_by");
            this.lblModifiedOb.Text = oDict.GetWordWithColon("modified_on");
            this.lblModifiedBy.Text = oDict.GetWordWithColon("modified_by");
            this.boxLogs.Text = oDict.GetWord("logs");
            this.tabContact.Text = oDict.GetWord("contact");
            this.tabAddress.Text = oDict.GetWord("address");
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(cmdConvertToChs, "Convert to Chs");
            toolTip1.SetToolTip(cmdConvertToCht, "Convert to Cht");
            toolTip1.SetToolTip(cmdNextCustomerCode, "Next Customer Code");

            txtCreditLimit.Validator = TextBoxValidation.FloatMaskValidator;
            txtProfitMargin.Validator = TextBoxValidation.IntegerValidator;

            txtCustomerCode.MaxLength = Common.Config.MaxLength_CustomerCode;
            txtACNumber.MaxLength = Common.Config.MaxLength_CustomerCode;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextCustomerCode.Visible = true;
                    txtCustomerCode.Width = txtCustomerCode.Width - 25;
                    tabCustomer.Enabled = false;

                    txtProfitMargin.Text = "0";
                    txtCreditLimit.Text = "0.00";
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtCustomerCode.Enabled = false;
                    tabCustomer.Enabled = true;
                    break;
            }
        }

        private void SetDropdowns()
        {
            T_PaymentTerms.LoadCombo(ref cboTerms, "TermsName", false);
            T_Region.LoadCombo(ref cboRegion, "RegionName", false);
            T_Currency.LoadCombo(ref cboCurrency, "CurrencyCode", false);

            xPort5.Controls.Utility.Default.Currency(ref cboCurrency);
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

        private void LoadChildControls()
        {
            Contact.ContactList contact = new Contact.ContactList();
            contact.Dock = DockStyle.Fill;
            contact.EditMode = _EditMode;
            contact.CustomerId = _CustomerId;

            this.tabContact.Controls.Add(contact);

            Address.AddressList address = new Address.AddressList();
            address.Dock = DockStyle.Fill;
            address.EditMode = _EditMode;
            address.CustomerId = _CustomerId;

            this.tabAddress.Controls.Add(address);
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowCustomer()
        {
            xPort5.EF6.Customer item = xPort5.EF6.Customer.Load(_CustomerId);
            if (item != null)
            {
                txtCustomerCode.Text = item.CustomerCode;
                txtName.Text = item.CustomerName;
                txtName_Chs.Text = item.CustomerName_Chs;
                txtName_Cht.Text = item.CustomerName_Cht;
                txtACNumber.Text = item.ACNumber;
                txtCreditLimit.Text = item.CreditLimit.ToString("###0.00");
                txtProfitMargin.Text = item.ProfitMargin.ToString("##0");
                chkBlacklisted.Checked = item.BlackListed;
                txtRemarks.Text = item.Remarks;

                if (item.RegionId.HasValue)
                {
                    T_Region region = T_Region.Load(item.RegionId.Value);
                    if (region != null)
                    {
                        cboRegion.Text = region.RegionName;
                        cboRegion.SelectedValue = region.RegionId;
                    }
                }
                if (item.TermsId.HasValue)
                {
                    T_PaymentTerms oTerms = T_PaymentTerms.Load(item.TermsId.Value);
                    if (oTerms != null)
                    {
                        cboTerms.Text = oTerms.TermsName;
                        cboTerms.SelectedValue = oTerms.TermsId;
                    }
                }
                if (item.CurrencyId.HasValue)
                {
                    T_Currency oCny = T_Currency.Load(item.CurrencyId.Value);
                    if (oCny != null)
                    {
                        cboCurrency.Text = oCny.CurrencyCode;
                        cboCurrency.SelectedValue = oCny.CurrencyId;
                    }
                }

                Staff s1 = Staff.Load(item.CreatedBy);
                Staff s2 = Staff.Load(item.ModifiedBy);
                txtCreatedOn.Text = item.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                txtCreatedBy.Text = s1.Alias;
                txtModifiedOn.Text = item.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
                txtModifiedBy.Text = s2.Alias;
                boxLogs.Visible = true;

                _CustomerCode = item.CustomerCode;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    xPort5.EF6.Customer item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new xPort5.EF6.Customer();
                            item.CustomerId = _CustomerId;
                            item.CustomerCode = txtCustomerCode.Text.Trim();

                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = xPort5.EF6.Customer.Load(_CustomerId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            break;
                    }
                    item.ACNumber = txtACNumber.Text.Trim();
                    item.CustomerName = txtName.Text.Trim();
                    item.CustomerName_Chs = txtName_Chs.Text.Trim();
                    item.CustomerName_Cht = txtName_Cht.Text.Trim();
                    item.CreditLimit = Convert.ToDecimal(txtCreditLimit.Text.Trim());
                    item.ProfitMargin = Convert.ToInt32(txtProfitMargin.Text.Trim());
                    item.BlackListed = chkBlacklisted.Checked;
                    item.Remarks = txtRemarks.Text.Trim();
                    item.CurrencyId = (Guid)cboCurrency.SelectedValue;
                    item.TermsId = (Guid)cboTerms.SelectedValue;
                    item.RegionId = (Guid)cboRegion.SelectedValue;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.CustomerId != _CustomerId)
                    {
                        _CustomerId = item.CustomerId;
                        _CustomerCode = item.CustomerCode;
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

            if (_EditMode == Common.Enums.EditMode.Add)
            {
                #region validate Customer Code
                if (txtCustomerCode.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Customer Code cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.Customer customer = xPort5.EF6.Customer.LoadWhere(String.Format("CustomerCode = '{0}'", txtCustomerCode.Text.Trim()));
                    if (customer != null)
                    {
                        errMsg += Environment.NewLine + "Customer Code is in use.";
                        result = false;
                    }
                }
                #endregion

                #region validate Article Code
                if (txtName.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Customer Name cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.Customer customer = xPort5.EF6.Customer.LoadWhere(String.Format("CustomerName = N'{0}'", txtName.Text.Trim()));
                    if (customer != null)
                    {
                        errMsg += Environment.NewLine + "Customer Name is in use.";
                        result = false;
                    }
                }
                #endregion
            }

            #region validate Credit Limit
            if (txtCreditLimit.Text.Trim() != String.Empty)
            {
                try
                {
                    Decimal credit = Convert.ToDecimal(txtCreditLimit.Text.Trim());
                }
                catch
                {
                    errMsg += Environment.NewLine + "Credit Limit must be decimal.";
                    result = false;
                }
            }
            else
            {
                errMsg += Environment.NewLine + "Credit Limit cannot be blank.";
                result = false;
            }
            #endregion

            #region validate Profit Margin
            if (txtProfitMargin.Text.Trim() != String.Empty)
            {
                try
                {
                    int credit = Convert.ToInt32(txtProfitMargin.Text.Trim());
                }
                catch
                {
                    errMsg += Environment.NewLine + "Profit Margin must be integer.";
                    result = false;
                }
            }
            else
            {
                errMsg += Environment.NewLine + "Profit Margin cannot be blank.";
                result = false;
            }
            #endregion

            if (!(result))
            {
                MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        private bool DeleteItem()
        {
            return xPort5.Controls.Utility.Customer.DeleteRec(_CustomerId);
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _CustomerCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextCustomerCode.Visible = false;

                            txtCustomerCode.Width = txtCustomerCode.Width + 25;
                            tabCustomer.Enabled = true;
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _CustomerCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _CustomerCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _CustomerId = System.Guid.NewGuid();

                            cmdNextCustomerCode.Visible = true;

                            txtCustomerCode.Width = txtCustomerCode.Width - 25;
                            txtCustomerCode.Enabled = true;
                            tabCustomer.Enabled = false;
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _CustomerCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _CustomerId = System.Guid.NewGuid();

                            cmdNextCustomerCode.Visible = true;

                            txtCustomerCode.Width = txtCustomerCode.Width - 25;
                            txtCustomerCode.Enabled = true;
                            tabCustomer.Enabled = false;
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
                        MessageBox.Show(String.Format("Customer Code {0} is deleted.", _CustomerCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void ArticleRecord_Load(object sender, EventArgs e)
        {
            SetAnsToolbar();
            SetCaptions();
            SetAttributes();
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _CustomerId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowCustomer();
                    break;
            }

            LoadChildControls();
        }

        private void cmdNextCustomerCode_Click(object sender, EventArgs e)
        {
            txtCustomerCode.Text = xPort5.Controls.Utility.Customer.NextCustomerCode();
        }

        private void cmdConvertToChs_Click(object sender, EventArgs e)
        {
            nxStudio.BaseClass.Common.Convert oConvert = new nxStudio.BaseClass.Common.Convert();

            string source = txtName_Cht.Text;
            txtName_Chs.Text = oConvert.TraditionaltoSimplified(source);
        }

        private void cmdConvertToCht_Click(object sender, EventArgs e)
        {
            nxStudio.BaseClass.Common.Convert oConvert = new nxStudio.BaseClass.Common.Convert();

            string source = txtName_Chs.Text;
            txtName_Cht.Text = oConvert.SimplifiedToTraditional(source);
        }
    }
}
