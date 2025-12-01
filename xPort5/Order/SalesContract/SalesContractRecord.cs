#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using xPort5.EF6;
using xPort5.Common;
using Gizmox.WebGUI.Common.Resources;
using xPort5.Controls;

#endregion

namespace xPort5.Order.SalesContract
{
    public partial class SalesContractRecord : Form
    {
        private Guid _ContractId = System.Guid.Empty;
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private string _SCNumber = string.Empty;

        #region Public Properties

        public Guid SalesContractId
        {
            get
            {
                return _ContractId;
            }
            set
            {
                _ContractId = value;
            }
        }

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

        #endregion

        public SalesContractRecord()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAnsToolbar();
            SetAttributes();
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowItem();
                    break;
            }

            LoadChildControls();
        }

        #region Configure Controls on Form Load
        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("sales_contract")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.lblSCNumber.Text = oDict.GetWordWithColon("sales_contract_num");
            this.lblDate.Text = oDict.GetWordWithColon("date");
            this.lblCustomer.Text = oDict.GetWordWithColon("customer");
            this.lblSalesperson.Text = oDict.GetWordWithColon("salesperson");
            this.lblRemarks.Text = oDict.GetWordWithColon("remarks");
            this.lblTerms.Text = oDict.GetWordWithColon("payment_terms");
            this.lblPriceTerms.Text = oDict.GetWordWithColon("price_terms");
            this.lblOrderNumber.Text = oDict.GetWordWithColon("order#");
            this.lblYourReference.Text = oDict.GetWordWithColon("ref#");
            this.lblCarrier.Text = oDict.GetWordWithColon("carrier");
            this.lblLoadingPort.Text = oDict.GetWordWithColon("loading_port");
            this.lblDischargePort.Text = oDict.GetWordWithColon("discharge_port");
            this.lblDestination.Text = oDict.GetWordWithColon("dest");
            this.lblOrigin.Text = oDict.GetWordWithColon("origin");

            toolTip1.SetToolTip(cmdNextSCNumber, string.Format(oDict.GetWord("generate_number"), oDict.GetWord("sales_contract_num")));

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextSCNumber.Visible = true;
                    txtSCNumber.Width = txtSCNumber.Width - 25;
                    wspBottom.Enabled = false;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtSCNumber.Enabled = false;
                    wspBottom.Enabled = true;
                    break;
            }

            datSCDate.Format = DateTimePickerFormat.Custom;
            datSCDate.CustomFormat = Common.DateTimeHelper.GetDateFormat();
        }

        private void SetDropdowns()
        {
            Staff.LoadCombo(ref cboSalesperson, "Alias", false);
            Customer.LoadCombo(ref cboCustomer, "CustomerName", false);
            T_PaymentTerms.LoadCombo(ref cboTerms, "TermsName", false, false, String.Empty, "TermsType = 'P'");
            T_PaymentTerms.LoadCombo(ref cboPriceTerms, "TermsName", false, false, String.Empty, "TermsType = 'R'");
            T_Remarks.LoadCombo(ref cboRemarks, "RemarkCode", false);//, true, String.Empty, String.Empty);
            T_Origin.LoadCombo(ref cboOrigin, "OriginName", false);
            T_Port.LoadCombo(ref cboLoadingPort, "PortName", false);
            T_Port.LoadCombo(ref cboDischargePort, "PortName", false);
            T_Port.LoadCombo(ref cboDestination, "PortName", false);

            xPort5.Controls.Utility.Default.Salesperson(ref cboSalesperson);
        }

        private void SetAnsToolbar()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Order.SalesContract");

            // cmdApprove
            ToolBarButton cmdAttach = new ToolBarButton("Upload", oDict.GetWord("attachment"));
            cmdAttach.Tag = "Upload";
            cmdAttach.Image = new IconResourceHandle("16x16.ico_16_1001_d.gif");

            if (_EditMode != Common.Enums.EditMode.Read)
            {
                if (_EditMode != Common.Enums.EditMode.Add)
                {
                    this.ansToolbar.Buttons.Add(sep);
                    this.ansToolbar.Buttons.Add(cmdAttach);
                }
            }

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
                    case "delete":
                        MessageBox.Show("Delete Item?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                    case "upload":
                        AttachmentManager attachWizard = new AttachmentManager();
                        attachWizard.OrderId = this._ContractId;
                        attachWizard.OrderNumber = this._SCNumber;
                        attachWizard.ShowDialog();
                        break;
                }
            }
        }

        private void LoadChildControls()
        {
            this.wspBottom.Controls.Clear();

            Items.ItemList list = new Items.ItemList();
            list.Dock = DockStyle.Fill;
            list.EditMode = _EditMode;
            list.SalesContractId = _ContractId;

            this.wspBottom.Controls.Add(list);
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowItem()
        {
            xPort5.EF6.OrderSC item = xPort5.EF6.OrderSC.Load(_ContractId);
            if (item != null)
            {
                txtSCNumber.Text = item.SCNumber;
                if (item.SCDate.HasValue)
                {
                    datSCDate.Value = item.SCDate.Value;
                }
                txtRemarks.Text = item.Remarks;
                txtYourOrderNumber.Text = item.YourOrderNo;
                txtYourReference.Text = item.YourRef;
                txtCarrier.Text = item.Carrier;

                Customer customer = Customer.Load(item.CustomerId);
                if (customer != null)
                {
                    cboCustomer.Text = customer.CustomerName;
                    cboCustomer.SelectedValue = customer.CustomerId;
                }
                Staff salesperson = Staff.Load(item.StaffId);
                if (salesperson != null)
                {
                    cboSalesperson.Text = salesperson.Alias;
                    cboSalesperson.SelectedValue = salesperson.StaffId;
                }
                if (item.PaymentTerms.HasValue)
                {
                    T_PaymentTerms oTerms = T_PaymentTerms.Load(item.PaymentTerms.Value);
                    if (oTerms != null)
                    {
                        cboTerms.Text = oTerms.TermsName;
                        cboTerms.SelectedValue = oTerms.TermsId;
                    }
                }
                if (item.PricingTerms.HasValue)
                {
                    T_PaymentTerms oPriceTerms = T_PaymentTerms.Load(item.PricingTerms.Value);
                    if (oPriceTerms != null)
                    {
                        cboPriceTerms.Text = oPriceTerms.TermsName;
                        cboPriceTerms.SelectedValue = oPriceTerms.TermsId;
                    }
                }
                if (item.LoadingPort.HasValue)
                {
                    T_Port oLoadingPort = T_Port.Load(item.LoadingPort.Value);
                    if (oLoadingPort != null)
                    {
                        cboLoadingPort.Text = oLoadingPort.PortName;
                        cboLoadingPort.SelectedValue = oLoadingPort.PortId;
                    }
                }
                if (item.DischargePort.HasValue)
                {
                    T_Port oDischargePort = T_Port.Load(item.DischargePort.Value);
                    if (oDischargePort != null)
                    {
                        cboDischargePort.Text = oDischargePort.PortName;
                        cboDischargePort.SelectedValue = oDischargePort.PortId;
                    }
                }
                if (item.Destination.HasValue)
                {
                    T_Port oDestinationPort = T_Port.Load(item.Destination.Value);
                    if (oDestinationPort != null)
                    {
                        cboDestination.Text = oDestinationPort.PortName;
                        cboDestination.SelectedValue = oDestinationPort.PortId;
                    }
                }
                if (item.OriginId.HasValue)
                {
                    T_Origin oOrigin = T_Origin.Load(item.OriginId.Value);
                    if (oOrigin != null)
                    {
                        cboOrigin.Text = oOrigin.OriginName;
                        cboOrigin.SelectedValue = oOrigin.OriginId;
                    }
                }

                _SCNumber = item.SCNumber;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    xPort5.EF6.OrderSC item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new xPort5.EF6.OrderSC();

                            item.Status = (int)Common.Enums.Status.Active;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = xPort5.EF6.OrderSC.Load(_ContractId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;

                            item.Revision = item.Revision + 1;
                            break;
                    }
                    item.SCNumber = txtSCNumber.Text.Trim();
                    item.SCDate = datSCDate.Value;
                    item.Remarks = txtRemarks.Text.Trim();
                    item.YourOrderNo = txtYourOrderNumber.Text.Trim();
                    item.YourRef = txtYourReference.Text.Trim();
                    item.Carrier = txtCarrier.Text.Trim();

                    item.CustomerId = (Guid)cboCustomer.SelectedValue;
                    item.StaffId = (Guid)cboSalesperson.SelectedValue;
                    item.PaymentTerms = (Guid)cboTerms.SelectedValue;
                    item.PricingTerms = (Guid)cboPriceTerms.SelectedValue;

                    item.LoadingPort = (Guid)cboLoadingPort.SelectedValue;
                    item.DischargePort = (Guid)cboDischargePort.SelectedValue;
                    item.Destination = (Guid)cboDestination.SelectedValue;
                    item.OriginId = (Guid)cboOrigin.SelectedValue;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.CustomerId != _ContractId)
                    {
                        _ContractId = item.OrderSCId;
                        _SCNumber = item.SCNumber;
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
                #region validate Sales Contract Number
                if (txtSCNumber.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Sales Contract No. cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.OrderSC order = xPort5.EF6.OrderSC.LoadWhere(String.Format("SCNumber = '{0}'", txtSCNumber.Text.Trim()));
                    if (order != null)
                    {
                        errMsg += Environment.NewLine + "Sales Contract No. is in use.";
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
            return xPort5.Controls.Utility.OrderSC.DeleteRec(_ContractId);
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _SCNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextSCNumber.Visible = false;

                            txtSCNumber.Width = txtSCNumber.Width + 25;
                            wspBottom.Enabled = true;

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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _SCNumber), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (DeleteItem())
                    {
                        MessageBox.Show(String.Format("Customer Code {0} is deleted.", _SCNumber), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cboRemarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            try
            {
                T_Remarks remark = T_Remarks.Load((Guid)s.SelectedValue);
                if (remark != null)
                {
                    txtRemarks.Text = remark.RemarkName;
                }
            }
            catch { }
        }

        private void cmdNextSCNumber_Click(object sender, EventArgs e)
        {
            txtSCNumber.Text = xPort5.Controls.Utility.OrderSC.NextSCNumber();
        }
    }
}
