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
using xPort5.Controls;

#endregion

namespace xPort5.Order.PurchaseContract
{
    public partial class PurchaseContractRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _OrderPCId = System.Guid.Empty;
        private string _PCNumber = string.Empty;

        public PurchaseContractRecord()
        {
            InitializeComponent();
        }

        private void PurchaseContractRecord_Load(object sender, EventArgs e)
        {
            SetAttributes();
            SetAnsToolbar();
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _OrderPCId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowItem();
                    break;
            }

            LoadChildControls();
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
        public Guid OrderPCId
        {
            get
            {
                return _OrderPCId;
            }
            set
            {
                _OrderPCId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load.
        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("purchase_contract")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.lblPCNumber.Text = oDict.GetWordWithColon("purchase_contract_num");

            toolTip1.SetToolTip(cmdNextPCNumber, string.Format(oDict.GetWord("generate_number"), oDict.GetWord("purchase_contract_num")));
            this.lblDate.Text = oDict.GetWordWithColon("date");
            this.lblSupplier.Text = oDict.GetWordWithColon("supplier");
            this.lblPurchsedBy.Text = oDict.GetWordWithColon("purchased_by");
            this.lblRemarks.Text = oDict.GetWordWithColon("remarks");
            this.lblTerms.Text = oDict.GetWordWithColon("payment_terms");
            this.lblPriceTerms.Text = oDict.GetWordWithColon("price_terms");
            this.lblYourRef.Text = oDict.GetWordWithColon("ref#");
            this.lblCarrier.Text = oDict.GetWordWithColon("carrier");
            this.lblLoadingPort.Text = oDict.GetWordWithColon("loading_port");
            this.lblDischargePort.Text = oDict.GetWordWithColon("discharge_port");
            this.lblDestination.Text = oDict.GetWordWithColon("dest");
            this.lblOrigin.Text = oDict.GetWordWithColon("origin");
            this.lblRemarks.Text = oDict.GetWordWithColon("remarks");
            this.lblRemarks_2.Text = string.Format("{0} 2{1}", oDict.GetWord("remarks"), oDict.GetColon());
            this.lblRemarks_3.Text = string.Format("{0} 3{1}", oDict.GetWord("remarks"), oDict.GetColon());

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextPCNumber.Visible = true;
                    txtPCNumber.Width = txtPCNumber.Width - 25;
                    wspBottom.Enabled = false;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtPCNumber.Enabled = false;
                    wspBottom.Enabled = true;
                    break;
            }
        }

        private void SetDropdowns()
        {
            Supplier.LoadCombo(ref cboSupplier, "SupplierName", false);
            Staff.LoadCombo(ref cboPurchasedBy, "Alias", false);
            T_Port.LoadCombo(ref cboLoadingPort, "PortName", false);
            T_Port.LoadCombo(ref cboDiscPort, "PortName", false);
            T_Port.LoadCombo(ref cboDestination, "PortName", false);
            T_Origin.LoadCombo(ref cboOrigin, "OriginName", false);
            T_PaymentTerms.LoadCombo(ref cboPaymentTerms, "TermsName", false, false, String.Empty, "TermsType = 'P'");
            T_PaymentTerms.LoadCombo(ref cboPricingTerms, "TermsName", false, false, String.Empty, "TermsType = 'R'");
            T_Remarks.LoadCombo(ref cboRemarks, "RemarkCode", false);
            T_Remarks.LoadCombo(ref cboRemarks2, "RemarkCode", false);
            T_Remarks.LoadCombo(ref cboRemarks3, "RemarkCode", false);
        }

        private void SetAnsToolbar()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Order.PurchaseContract");

            // cmdAttach
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

        void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Name != null)
            {
                switch (e.Button.Name.ToLower())
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
                        attachWizard.OrderId = this._OrderPCId;
                        attachWizard.OrderNumber = this._PCNumber;
                        attachWizard.ShowDialog();
                        break;
                }
            }
        }

        private void LoadChildControls()
        {
            this.wspBottom.Controls.Clear();

            Items.ItemsList list = new xPort5.Order.PurchaseContract.Items.ItemsList();
            list.Dock = DockStyle.Fill;
            list.EditMode = _EditMode;
            list.OrderPCId = _OrderPCId;

            this.wspBottom.Controls.Add(list);
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowItem()
        {
            xPort5.EF6.OrderPC item = xPort5.EF6.OrderPC.Load(_OrderPCId);
            if (item != null)
            {
                txtPCNumber.Text = item.PCNumber;
                if (item.PCDate.HasValue)
                {
                    datPCDate.Value = item.PCDate.Value;
                }
                txtYourRef.Text = item.YourRef;
                txtCarrier.Text = item.Carrier;
                txtRemarks.Text = item.Remarks;
                txtRemarks2.Text = item.Remarks2;
                txtRemarks3.Text = item.Remarks3;

                Supplier suppName = Supplier.Load(item.SupplierId);
                if (suppName != null)
                {
                    cboSupplier.Text = suppName.SupplierName;
                    cboSupplier.SelectedValue = suppName.SupplierId;
                }
                Staff purchaseBy = Staff.Load(item.StaffId);
                if (purchaseBy != null)
                {
                    cboPurchasedBy.Text = purchaseBy.Alias;
                    cboPurchasedBy.SelectedValue = purchaseBy.StaffId;
                }
                if (item.LoadingPort.HasValue)
                {
                    T_Port loadingPort = T_Port.Load(item.LoadingPort.Value);
                    if (loadingPort != null)
                    {
                        cboLoadingPort.Text = loadingPort.PortName;
                        cboLoadingPort.SelectedValue = loadingPort.PortId;
                    }
                }
                if (item.DischargePort.HasValue)
                {
                    T_Port discPort = T_Port.Load(item.DischargePort.Value);
                    if (discPort != null)
                    {
                        cboDiscPort.Text = discPort.PortName;
                        cboDiscPort.SelectedValue = discPort.PortId;
                    }
                }
                if (item.Destination.HasValue)
                {
                    T_Port destination = T_Port.Load(item.Destination.Value);
                    if (destination != null)
                    {
                        cboDestination.Text = destination.PortName;
                        cboDestination.SelectedValue = destination.PortId;
                    }
                }
                if (item.PaymentTerms.HasValue)
                {
                    T_PaymentTerms pTerms = T_PaymentTerms.Load(item.PaymentTerms.Value);
                    if (pTerms != null)
                    {
                        cboPaymentTerms.Text = pTerms.TermsName;
                        cboPaymentTerms.SelectedValue = pTerms.TermsId;
                    }
                }
                if (item.PricingTerms.HasValue)
                {
                    T_PaymentTerms rTerms = T_PaymentTerms.Load(item.PricingTerms.Value);
                    if (rTerms != null)
                    {
                        cboPricingTerms.Text = rTerms.TermsName;
                        cboPricingTerms.SelectedValue = rTerms.TermsId;
                    }
                }

                this._PCNumber = item.PCNumber;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    xPort5.EF6.OrderPC item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new xPort5.EF6.OrderPC();
                            item.OrderPCId = _OrderPCId;

                            item.Status = (int)Common.Enums.Status.Active;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = xPort5.EF6.OrderPC.Load(_OrderPCId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;

                            item.Revision = item.Revision + 1;
                            break;
                    } 
                    item.PCNumber = txtPCNumber.Text.Trim();
                    item.PCDate = datPCDate.Value;
                    if (Common.Utility.IsGUID(cboSupplier.SelectedValue.ToString()))
                    {
                        item.SupplierId = (Guid)cboSupplier.SelectedValue;
                    }
                    else
                    {
                        item.SupplierId = System.Guid.Empty;
                    }

                    if (Common.Utility.IsGUID(cboPurchasedBy.SelectedValue.ToString()))
                    {
                        item.StaffId = (Guid)cboPurchasedBy.SelectedValue;
                    }
                    else
                    {
                        item.StaffId = System.Guid.Empty;
                    }

                    item.YourRef = txtYourRef.Text.Trim();
                    item.Carrier = txtCarrier.Text.Trim();

                    if (Common.Utility.IsGUID(cboLoadingPort.SelectedValue.ToString()))
                    {
                        item.LoadingPort = (Guid)cboLoadingPort.SelectedValue;
                    }
                    else
                    {
                        item.LoadingPort = System.Guid.Empty;
                    }

                    if (Common.Utility.IsGUID(cboDiscPort.SelectedValue.ToString()))
                    {
                        item.DischargePort = (Guid)cboDiscPort.SelectedValue;
                    }
                    else
                    {
                        item.DischargePort = System.Guid.Empty;
                    }

                    if (Common.Utility.IsGUID(cboDestination.SelectedValue.ToString()))
                    {
                        item.Destination = (Guid)cboDestination.SelectedValue;
                    }
                    else
                    {
                        item.Destination = System.Guid.Empty;
                    }

                    if (Common.Utility.IsGUID(cboOrigin.SelectedValue.ToString()))
                    {
                        item.OriginId = (Guid)cboOrigin.SelectedValue;
                    }
                    else
                    {
                        item.OriginId = System.Guid.Empty;
                    }

                    if (Common.Utility.IsGUID(cboPaymentTerms.SelectedValue.ToString()))
                    {
                        item.PaymentTerms = (Guid)cboPaymentTerms.SelectedValue;
                    }
                    else
                    {
                        item.PaymentTerms = System.Guid.Empty;
                    }

                    if (Common.Utility.IsGUID(cboPricingTerms.SelectedValue.ToString()))
                    {
                        item.PricingTerms = (Guid)cboPricingTerms.SelectedValue;
                    }
                    else
                    {
                        item.PricingTerms = System.Guid.Empty;
                    }
                    item.Remarks = txtRemarks.Text.Trim();
                    item.Remarks2 = txtRemarks2.Text.Trim();
                    item.Remarks3 = txtRemarks3.Text.Trim();

                    _PCNumber = item.PCNumber;
                    item.Save();

                    #region loag activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    result = true;
                }
                catch
                { }
            }

            return result;
        }

        private bool VerifyItem()
        {
            bool result = true;
            string errMsg = string.Empty;

            if (_EditMode == Common.Enums.EditMode.Add)
            {
                #region Validate PC Number
                if (this.txtPCNumber.Text == string.Empty)
                {
                    errMsg += Environment.NewLine + "PurchaseContract No. cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.OrderPC orderPC = xPort5.EF6.OrderPC.LoadWhere(String.Format("PCNumber = '{0}'", txtPCNumber.Text.Trim()));
                    if (orderPC != null)
                    {
                        errMsg += Environment.NewLine + "Quotation No. is in use.";
                        result = false;
                    }
                }
                #endregion

                if (!(result))
                {
                    MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return result;
            }

            return result;
        }

        private bool DeleteItem()
        {
            return xPort5.Controls.Utility.OrderPC.DeleteRec(_OrderPCId);
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
                        MessageBox.Show(string.Format("Purchase Contract {0} is saved!", _PCNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextPCNumber.Visible = false;

                            txtPCNumber.Width = txtPCNumber.Width + 25;
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
                        MessageBox.Show(String.Format("Purchase Contract {0} is saved!", _PCNumber), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Purchase Contract {0} is saved!", _PCNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _OrderPCId = System.Guid.NewGuid();

                            cmdNextPCNumber.Visible = true;

                            txtPCNumber.Width = txtPCNumber.Width - 25;
                            txtPCNumber.Enabled = true;
                            wspBottom.Enabled = false;
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
                        MessageBox.Show(String.Format("Purchase Contract {0} is saved!", _PCNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _OrderPCId = System.Guid.NewGuid();

                            cmdNextPCNumber.Visible = true;

                            txtPCNumber.Width = txtPCNumber.Width - 25;
                            txtPCNumber.Enabled = true;
                            wspBottom.Enabled = false;
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
                        MessageBox.Show(String.Format("Purchase Contract {0} is deleted.", _PCNumber), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdNextPCNumber_Click(object sender, EventArgs e)
        {
            txtPCNumber.Text = xPort5.Controls.Utility.OrderPC.NextPCNumber();
        }

        private void cboRemarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            try
            {
                T_Remarks remark = T_Remarks.Load((Guid)s.SelectedValue);
                if (remark != null)
                {
                    switch (s.Name.ToLower())
                    {
                        case "cboremarks":
                            txtRemarks.Text = remark.RemarkName;
                            break;
                        case "cboremarks2":
                            txtRemarks2.Text = remark.RemarkName;
                            break;
                        case "cboremarks3":
                            txtRemarks3.Text = remark.RemarkName;
                            break;
                    }
                }
            }
            catch { }
        }
    }
}
