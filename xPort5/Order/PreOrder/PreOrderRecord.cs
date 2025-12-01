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

namespace xPort5.Order.PreOrder
{
    public partial class PreOrderRecord : Form
    {
        public Guid _OrderPLId = System.Guid.Empty;
        public Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private string _PLNumber = String.Empty;
        private Guid _Dup_OrderPLId = System.Guid.Empty;

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
        public Guid OrderPLId
        {
            get
            {
                return _OrderPLId;
            }
            set
            {
                _OrderPLId = value;
            }
        }
        #endregion

        public PreOrderRecord()
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
                    _OrderPLId = System.Guid.NewGuid();
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

            txtPLNumber.MaxLength = Common.Config.MaxLength_QTNumber;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextPLNumber.Visible = true;
                    txtPLNumber.Width = txtPLNumber.Width - 25;
                    itemPane.Enabled = false;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtPLNumber.Enabled = false;
                    itemPane.Enabled = true;
                    break;
            }

            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = Common.DateTimeHelper.GetDateFormat();

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("pre_order")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            lblPreOrderNumber.Text = oDict.GetWordWithColon("pre-order_num");
            lblDate.Text = oDict.GetWordWithColon("date");
            lblCustomer.Text = oDict.GetWordWithColon("customer");
            lblSalesperson.Text = oDict.GetWordWithColon("salesperson");
            lblRemarks.Text = oDict.GetWordWithColon("remarks");

            toolTip1.SetToolTip(cmdNextPLNumber, string.Format(oDict.GetWord("generate_number"), oDict.GetWord("pre-order_num")));
        }


        private void SetDropdowns()
        {
            cboSalesperson.DataSource = null;
            Staff.LoadCombo(ref cboSalesperson, "Alias", false);

            cboCustomer.DataSource = null;
            Customer.LoadCombo(ref cboCustomer, "CustomerName", false);

            cboRemarks.DataSource = null;
            T_Remarks.LoadCombo(ref cboRemarks, "RemarkCode", false);//, true, String.Empty, String.Empty);

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

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Order.PreOrderList");

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
                        attachWizard.OrderId = this._OrderPLId;
                        attachWizard.OrderNumber = this._PLNumber;
                        attachWizard.ShowDialog();
                        break;
                }
            }
        }
        #endregion

        private void LoadChildControls()
        {
            this.itemPane.Controls.Clear();

            Items.ItemsList list = new Items.ItemsList();
            list.Dock = DockStyle.Fill;
            list.EditMode = _EditMode;
            list.OrderPLId = _OrderPLId;

            this.itemPane.Controls.Add(list);
        }

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()

        private void ShowItem()
        {
            xPort5.EF6.OrderPL item = xPort5.EF6.OrderPL.Load(_OrderPLId);
            if (item != null)
            {
                txtPLNumber.Text = item.PLNumber;
                if (item.PLDate.HasValue)
                {
                    dtpDate.Value = item.PLDate.Value;
                }
                txtRemarks.Text = item.Remarks;

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

                _PLNumber = item.PLNumber;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    xPort5.EF6.OrderPL item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new xPort5.EF6.OrderPL();
                            item.OrderPLId = _OrderPLId;

                            item.Status = (int)Common.Enums.Status.Active;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = xPort5.EF6.OrderPL.Load(_OrderPLId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;

                            item.Revision = item.Revision + 1;
                            break;
                    }
                    item.PLNumber = txtPLNumber.Text.Trim();
                    item.PLDate = dtpDate.Value;
                    item.Remarks = txtRemarks.Text.Trim();

                    item.CustomerId = (Guid)cboCustomer.SelectedValue;
                    item.StaffId = (Guid)cboSalesperson.SelectedValue;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.CustomerId != _OrderPLId)
                    {
                        _OrderPLId = item.OrderPLId;
                        _PLNumber = item.PLNumber;
                    }

                    DuplicateDetailsItem();

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
                if (txtPLNumber.Text.Trim() == String.Empty)
                {
                    errMsg += Environment.NewLine + "Pre-Order No. cannot be blank.";
                    result = false;
                }
                else
                {
                    xPort5.EF6.OrderPL orderPL = xPort5.EF6.OrderPL.LoadWhere(String.Format("PLNumber = '{0}'", txtPLNumber.Text.Trim()));
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

        private void DuplicateDetailsItem()
        {
            if (this._Dup_OrderPLId != System.Guid.Empty)
            {
                string sql = "OrderPLId = '" + _Dup_OrderPLId.ToString() + "'";
                OrderPLItemsCollection itemList = OrderPLItems.LoadCollection(sql);
                foreach (OrderPLItems item in itemList)
                {
                    OrderPLItems newItem = new OrderPLItems();
                    newItem.OrderPLId = this._OrderPLId;
                    newItem.LineNumber = item.LineNumber;
                    newItem.OrderQTItemId = item.OrderQTItemId;
                    newItem.Save();
                }
            }
        }

        private bool DeleteItem()
        {
            return xPort5.Controls.Utility.OrderPL.DeleteRec(_OrderPLId);
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
                        MessageBox.Show(String.Format("Pre-Order {0} is saved!", _PLNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextPLNumber.Visible = false;

                            txtPLNumber.Width = txtPLNumber.Width + 25;
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
                        MessageBox.Show(String.Format("Pre-Order {0} is saved!", _PLNumber), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Pre-Order {0} is saved!", _PLNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _Dup_OrderPLId = _OrderPLId;
                            _EditMode = Common.Enums.EditMode.Add;
                            _OrderPLId = System.Guid.NewGuid();

                            cmdNextPLNumber.Visible = true;

                            txtPLNumber.Width = txtPLNumber.Width - 25;
                            txtPLNumber.Enabled = true;
                            itemPane.Enabled = false;
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
                        MessageBox.Show(String.Format("Pre-Order {0} is saved!", _PLNumber), "Save Result");
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
                        MessageBox.Show(String.Format("Pre-Order {0} is deleted.", _PLNumber), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdNextPLNumber_Click(object sender, EventArgs e)
        {
            txtPLNumber.Text = xPort5.Controls.Utility.OrderPL.NextPLNumber();
        }
    }
}
