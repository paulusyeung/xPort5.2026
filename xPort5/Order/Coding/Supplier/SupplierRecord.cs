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

namespace xPort5.Order.Coding.Supplier
{
    public partial class SupplierRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _SupplierId = System.Guid.Empty;
        private string _SupplierCode = String.Empty;

        public SupplierRecord()
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
        public Guid SupplierId
        {
            get
            {
                return _SupplierId;
            }
            set
            {
                _SupplierId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("supplier")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.lblCode.Text = oDict.GetWordWithColon("supplier_code");
            this.lblName.Text = oDict.GetWordWithColon("supplier_name");
            this.lblName_Chs.Text = oDict.GetWordWithColon("supplier_name_chs");
            this.lblName_Cht.Text = oDict.GetWordWithColon("supplier_name_cht");
            this.lblAccountName.Text = oDict.GetWordWithColon("account_number");
            this.lblRegion.Text = oDict.GetWordWithColon("customer_region");
            this.lblTerms.Text = oDict.GetWordWithColon("payment_terms");
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
            toolTip1.SetToolTip(cmdNextSuplierCode, "Next Supplier Code");

            txtSupplierCode.MaxLength = Common.Config.MaxLength_SupplierCode;
            txtACNumber.MaxLength = Common.Config.MaxLength_SupplierCode;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextSuplierCode.Visible = true;
                    txtSupplierCode.Width = txtSupplierCode.Width - 25;
                    tabSupplier.Enabled = false;

                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtSupplierCode.Enabled = false;
                    tabSupplier.Enabled = true;
                    break;
            }
        }

        private void SetDropdowns()
        {
            T_PaymentTerms.LoadCombo(ref cboTerms, "TermsName", false);
            T_Region.LoadCombo(ref cboRegion, "RegionName", false);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Coding.Supplier");

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
            this.tabContact.Controls.Clear();

            Contact.ContactList contact = new Contact.ContactList();
            contact.Dock = DockStyle.Fill;
            contact.EditMode = _EditMode;
            contact.SupplierId = _SupplierId;

            this.tabContact.Controls.Add(contact);

            this.tabAddress.Controls.Clear();

            Address.AddressList address = new Address.AddressList();
            address.Dock = DockStyle.Fill;
            address.EditMode = _EditMode;
            address.SupplierId = _SupplierId;

            this.tabAddress.Controls.Add(address);
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowSupplier()
        {
            xPort5.EF6.Supplier item = xPort5.EF6.Supplier.Load(_SupplierId);
            if (item != null)
            {
                txtSupplierCode.Text = item.SupplierCode;
                txtName.Text = item.SupplierName;
                txtName_Chs.Text = item.SupplierName_Chs;
                txtName_Cht.Text = item.SupplierName_Cht;
                txtACNumber.Text = item.ACNumber;
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

                Staff s1 = Staff.Load(item.CreatedBy);
                Staff s2 = Staff.Load(item.ModifiedBy);
                txtCreatedOn.Text = item.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                txtCreatedBy.Text = s1.Alias;
                txtModifiedOn.Text = item.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
                txtModifiedBy.Text = s2.Alias;
                boxLogs.Visible = true;

                _SupplierCode = item.SupplierCode;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    xPort5.EF6.Supplier item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new xPort5.EF6.Supplier();
                            item.SupplierId = _SupplierId;
                            item.SupplierCode = txtSupplierCode.Text.Trim();

                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = xPort5.EF6.Supplier.Load(_SupplierId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            break;
                    }
                    item.ACNumber = txtACNumber.Text.Trim();
                    item.SupplierName = txtName.Text.Trim();
                    item.SupplierName_Chs = txtName_Chs.Text.Trim();
                    item.SupplierName_Cht = txtName_Cht.Text.Trim();
                    item.Remarks = txtRemarks.Text.Trim();
                    item.TermsId = (Guid)cboTerms.SelectedValue;
                    item.RegionId = (Guid)cboRegion.SelectedValue;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.SupplierId != _SupplierId)
                    {
                        _SupplierId = item.SupplierId;
                        _SupplierCode = item.SupplierCode;
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
                #region validate Supplier Code
                if (txtSupplierCode.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Supplier Code cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.Supplier customer = xPort5.EF6.Supplier.LoadWhere(String.Format("SupplierCode = '{0}'", txtSupplierCode.Text.Trim()));
                    if (customer != null)
                    {
                        errMsg += Environment.NewLine + "Supplier Code is in use.";
                        result = false;
                    }
                }
                #endregion

                #region validate Supplier Name
                if (txtName.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Supplier Name cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.Supplier customer = xPort5.EF6.Supplier.LoadWhere(String.Format("SupplierName = N'{0}'", txtName.Text.Trim()));
                    if (customer != null)
                    {
                        errMsg += Environment.NewLine + "Supplier Name is in use.";
                        result = false;
                    }
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
            return xPort5.Controls.Utility.Supplier.DeleteRec(_SupplierId);
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
                        MessageBox.Show(String.Format("Supplier Code {0} is saved!", _SupplierCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextSuplierCode.Visible = false;

                            txtSupplierCode.Width = txtSupplierCode.Width + 25;
                            tabSupplier.Enabled = true;

                            LoadChildControls();

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
                        MessageBox.Show(String.Format("Supplier Code {0} is saved!", _SupplierCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Supplier Code {0} is saved!", _SupplierCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _SupplierId = System.Guid.NewGuid();

                            cmdNextSuplierCode.Visible = true;

                            txtSupplierCode.Width = txtSupplierCode.Width - 25;
                            txtSupplierCode.Enabled = true;
                            tabSupplier.Enabled = false;
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
                        MessageBox.Show(String.Format("Supplier Code {0} is saved!", _SupplierCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _SupplierId = System.Guid.NewGuid();

                            cmdNextSuplierCode.Visible = true;

                            txtSupplierCode.Width = txtSupplierCode.Width - 25;
                            txtSupplierCode.Enabled = true;
                            tabSupplier.Enabled = false;
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
                        MessageBox.Show(String.Format("Supplier Code {0} is deleted.", _SupplierCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
            SetAttributes();
            SetCaptions();
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _SupplierId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowSupplier();
                    break;
            }

            LoadChildControls();
        }

        private void cmdNextSupplierCode_Click(object sender, EventArgs e)
        {
            txtSupplierCode.Text = xPort5.Controls.Utility.Supplier.NextSupplierCode();
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
