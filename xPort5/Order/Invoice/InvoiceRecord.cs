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

namespace xPort5.Order.Invoice
{
    public partial class InvoiceRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _InvoiceId = System.Guid.Empty;
        private string _InvoiceNumber = string.Empty;

        #region Public Properties

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

        public Guid InvoiceId
        {
            get
            {
                return _InvoiceId;
            }
            set
            {
                _InvoiceId = value;
            }
        }

        #endregion

        public InvoiceRecord()
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
                    _InvoiceId = System.Guid.NewGuid();
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

            txtINNumber.MaxLength = Common.Config.MaxLength_QTNumber;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextINNumber.Visible = true;
                    txtINNumber.Width = txtINNumber.Width - 25;
                    itemPane.Enabled = false;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtINNumber.Enabled = false;
                    cmdNextINNumber.Visible = false;
                    itemPane.Enabled = true;
                    break;
            }

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("invoice")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.dtpDate.Format = DateTimePickerFormat.Custom;
            this.dtpDate.CustomFormat = Common.DateTimeHelper.GetDateFormat();

            this.dtpDepartureDate.Format = DateTimePickerFormat.Custom;
            this.dtpDepartureDate.CustomFormat = Common.DateTimeHelper.GetDateFormat();

            this.lblInvoiceNumber.Text = oDict.GetWordWithColon("invoice_no");
            this.lblDate.Text = oDict.GetWordWithColon("date");
            this.lblCustomer.Text = oDict.GetWordWithColon("customer");
            this.lblSalesperson.Text = oDict.GetWordWithColon("salesperson");
            this.lblRemarks.Text = oDict.GetWordWithColon("remarks");
            this.lblRemarks_2.Text = string.Format("{0} 2{1}", oDict.GetWord("remarks"), oDict.GetColon());
            this.lblRemarks_3.Text = string.Format("{0} 3{1}", oDict.GetWord("remarks"), oDict.GetColon());
            this.lblSalesperson.Text = oDict.GetWordWithColon("salesperson");
            this.lblRemarks.Text = oDict.GetWordWithColon("remarks");
            this.lblPaymentTerms.Text = oDict.GetWordWithColon("payment_terms");
            this.lblPriceTerms.Text = oDict.GetWordWithColon("price_terms");
            this.lblYourReference.Text = oDict.GetWordWithColon("ref#");
            this.lblCarrier.Text = oDict.GetWordWithColon("carrier");
            this.lblLoadingPort.Text = oDict.GetWordWithColon("loading_port");
            this.lblDischargePort.Text = oDict.GetWordWithColon("discharge_port");
            this.lblDestination.Text = oDict.GetWordWithColon("dest");
            this.lblOrigin.Text = oDict.GetWordWithColon("origin");
            this.lblDepartureDate.Text = oDict.GetWordWithColon("departure_date");

            toolTip1.SetToolTip(cmdNextINNumber, string.Format(oDict.GetWord("generate_number"), oDict.GetWord("invoice_no")));
        }


        private void SetDropdowns()
        {
            Staff.LoadCombo(ref cboSalesperson, "Alias", false);
            Customer.LoadCombo(ref cboCustomer, "CustomerName", false);
            T_Remarks.LoadCombo(ref cboRemarks, "RemarkCode", false);//, true, String.Empty, String.Empty);
            T_Remarks.LoadCombo(ref cboRemarks_2, "RemarkCode", false);//, true, String.Empty, String.Empty);
            T_Remarks.LoadCombo(ref cboRemarks_3, "RemarkCode", false);//, true, String.Empty, String.Empty);

            T_PaymentTerms.LoadCombo(ref cboPaymentTerms, "TermsName", false, false, String.Empty, "TermsType = 'P'");
            T_PaymentTerms.LoadCombo(ref cboPriceTerms, "TermsName", false, false, String.Empty, "TermsType = 'R'");
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

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Order.Invoice");

            // cmdAttach
            ToolBarButton cmdAttach = new ToolBarButton("Upload", oDict.GetWord("attachment"));
            cmdAttach.Tag = "Upload";
            cmdAttach.Image = new IconResourceHandle("16x16.ico_16_1001_d.gif");

            // cmdAdditionalCharges
            ToolBarButton cmdAdditionalCharges = new ToolBarButton("AdditionalCharges", oDict.GetWord("additional_charge"));
            cmdAdditionalCharges.Tag = "AdditionalCharges";
            cmdAdditionalCharges.Image = new IconResourceHandle("16x16.ico_18_9105.gif");

            if (_EditMode != Common.Enums.EditMode.Read)
            {
                if (_EditMode != Common.Enums.EditMode.Add)
                {
                    this.ansToolbar.Buttons.Add(sep);
                    this.ansToolbar.Buttons.Add(cmdAttach);
                    this.ansToolbar.Buttons.Add(cmdAdditionalCharges); 
                }
            }

            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
        }

        void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
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
                    case "upload":
                        AttachmentManager attachWizard = new AttachmentManager();
                        attachWizard.OrderId = this._InvoiceId;
                        attachWizard.OrderNumber = this._InvoiceNumber;
                        attachWizard.ShowDialog();
                        break;
                    case "additionalcharges":
                        Items.AdditionalCharges additionalCharges = new Items.AdditionalCharges();
                        additionalCharges.OrderINId = this._InvoiceId;
                        additionalCharges.ShowDialog();
                        break;
                }
            }
        }
        #endregion

        private void LoadChildControls()
        {
            this.itemPane.Controls.Clear();

            Items.ItemList list = new Items.ItemList();
            list.Dock = DockStyle.Fill;
            list.EditMode = _EditMode;
            list.InvoiceId = _InvoiceId;

            this.itemPane.Controls.Add(list);
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()

        private void ShowItem()
        {
            xPort5.EF6.OrderIN item = xPort5.EF6.OrderIN.Load(_InvoiceId);
            if (item != null)
            {
                txtINNumber.Text = item.INNumber;
                if (item.INDate.HasValue)
                {
                    dtpDate.Value = item.INDate.Value;
                }
                txtRemarks.Text = item.Remarks;
                txtRemarks_2.Text = item.Remarks2;
                txtRemarks_3.Text = item.Remarks3;
                txtYourRef.Text = item.YourRef;
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
                        cboPaymentTerms.Text = oTerms.TermsName;
                        cboPaymentTerms.SelectedValue = oTerms.TermsId;
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

                if (item.ShipmentDate.HasValue)
                {
                    dtpDepartureDate.Value = item.ShipmentDate.Value;
                }

                _InvoiceNumber = item.INNumber;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    xPort5.EF6.OrderIN item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new xPort5.EF6.OrderIN();
                            item.OrderINId = _InvoiceId;

                            item.Status = (int)Common.Enums.Status.Active;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = xPort5.EF6.OrderIN.Load(_InvoiceId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;

                            item.Revision = item.Revision + 1;
                            break;
                    }

                    item.INNumber = txtINNumber.Text.Trim();
                    item.INDate = dtpDate.Value;
                    item.Remarks = txtRemarks.Text.Trim();
                    item.Remarks2 = txtRemarks_2.Text.Trim();
                    item.Remarks3 = txtRemarks_3.Text.Trim();

                    item.CustomerId = (Guid)cboCustomer.SelectedValue;
                    item.StaffId = (Guid)cboSalesperson.SelectedValue;
                    item.PaymentTerms = (Guid)cboPaymentTerms.SelectedValue;
                    item.PricingTerms = (Guid)cboPriceTerms.SelectedValue;
                    item.LoadingPort = (Guid)cboLoadingPort.SelectedValue;
                    item.DischargePort = (Guid)cboDischargePort.SelectedValue;
                    item.Destination = (Guid)cboDestination.SelectedValue;
                    item.OriginId = (Guid)cboOrigin.SelectedValue;

                    item.YourRef = txtYourRef.Text;
                    item.Carrier = txtCarrier.Text;

                    item.ShipmentDate = dtpDepartureDate.Value;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.CustomerId != _InvoiceId)
                    {
                        _InvoiceId = item.OrderINId;
                        _InvoiceNumber = item.INNumber;
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
                #region validate PL Number
                if (txtINNumber.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Pre-Order No. cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.OrderPL orderPL = xPort5.EF6.OrderPL.LoadWhere(String.Format("PLNumber = '{0}'", txtINNumber.Text.Trim()));
                    if (orderPL != null)
                    {
                        errMsg += Environment.NewLine + "Pre-Order No. is in use.";
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
            return xPort5.Controls.Utility.OrderIN.DeleteRec(_InvoiceId);
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
                        MessageBox.Show(String.Format("Pre-Order {0} is saved!", _InvoiceNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextINNumber.Visible = false;

                            txtINNumber.Width = txtINNumber.Width + 25;
                            itemPane.Enabled = true;

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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _InvoiceNumber), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _InvoiceNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _InvoiceId = System.Guid.NewGuid();

                            //cmdNextQTNumber.Visible = true;

                            //txtQTNumber.Width = txtQTNumber.Width - 25;
                            //txtQTNumber.Enabled = true;
                            //wspBottom.Enabled = false;
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
                        MessageBox.Show(String.Format("Customer Code {0} is saved!", _InvoiceNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            //_QuotationId = System.Guid.NewGuid();

                            //cmdNextQTNumber.Visible = true;

                            //txtQTNumber.Width = txtQTNumber.Width - 25;
                            //txtQTNumber.Enabled = true;
                            //wspBottom.Enabled = false;
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
                        MessageBox.Show(String.Format("Customer Code {0} is deleted.", _InvoiceNumber), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdNextINNumber_Click(object sender, EventArgs e)
        {
            txtINNumber.Text = xPort5.Controls.Utility.OrderIN.NextINNumber();
        }

        private void cboRemarks_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            try
            {
                T_Remarks remark = T_Remarks.Load((Guid)s.SelectedValue);
                if (remark != null)
                {
                    txtRemarks_2.Text = remark.RemarkName;
                }
            }
            catch { }
        }

        private void cboRemarks_3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            try
            {
                T_Remarks remark = T_Remarks.Load((Guid)s.SelectedValue);
                if (remark != null)
                {
                    txtRemarks_3.Text = remark.RemarkName;
                }
            }
            catch { }
        }
    }
}
