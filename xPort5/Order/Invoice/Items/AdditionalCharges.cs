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
using xPort5.Controls;

#endregion

namespace xPort5.Order.Invoice.Items
{
    public partial class AdditionalCharges : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _OrderINId = System.Guid.Empty;
        private Guid _OrderINChargeId = System.Guid.Empty;

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
        public Guid OrderINId
        {
            get
            {
                return _OrderINId;
            }
            set
            {
                _OrderINId = value;
            }
        }
        #endregion

        public AdditionalCharges()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttributes();
            SetCombo();

            BindList();
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = oDict.GetWord("additional_charge");
            this.lblCharge.Text = string.Format(oDict.GetWordWithColon("code_replace"), oDict.GetWord("charge"));
            this.lblDescription.Text = oDict.GetWordWithColon("description");
            this.lblAmount.Text = oDict.GetWordWithColon("amount");
            this.btnAdd.Text = oDict.GetWord("add");
            this.btnEdit.Text = oDict.GetWord("edit");
            this.cmdDelete.Text = oDict.GetWord("delete");
            this.colChargeCode.Text = string.Format(oDict.GetWord("code_replace"), oDict.GetWord("charge"));
            this.colDescription.Text = oDict.GetWord("description");
            this.colAmount.Text = oDict.GetWord("amount");

            this.btnEdit.Enabled = false;
            this.cmdDelete.Enabled = false;
        }

        private void SetCombo()
        {
            cboCharge.DataSource = null;
            T_Charge.LoadCombo(ref cboCharge, "ChargeCode", false, true, string.Empty, string.Empty);
        }

        private void BindList()
        {
            lvwChargeList.Items.Clear();

            string sql = "OrderINId = '" + this.OrderINId.ToString() + "'";
            OrderINChargesCollection inChargeList = OrderINCharges.LoadCollection(sql);
            foreach (OrderINCharges inCharge in inChargeList)
            {
                ListViewItem lvItem = lvwChargeList.Items.Add(inCharge.OrderINChargeId.ToString());
                lvItem.SubItems.Add(GetChargeCode(inCharge.ChargeId));
                lvItem.SubItems.Add(inCharge.Description);
                lvItem.SubItems.Add(inCharge.Amount.ToString("#,###0.00"));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (VerifyItem())
            {
                OrderINCharges inCharge = new OrderINCharges();
                inCharge.OrderINId = this.OrderINId;
                inCharge.ChargeId = new Guid(cboCharge.SelectedValue.ToString());
                inCharge.Description = txtDescription.Text;
                inCharge.Amount = Convert.ToDecimal(txtAmount.Text.Trim());

                inCharge.Save();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, inCharge.ToString());

                BindList();
                Clear();
            }
        }

        private bool VerifyItem()
        {
            bool result = true;
            string errMsg = String.Empty;

            #region validate Charge
            if (cboCharge.SelectedValue == null)
            {
                errMsg += Environment.NewLine + " Charge cannot be blank.";
                result = false;
            }
            else
            {
                if (!Common.Utility.IsGUID(cboCharge.SelectedValue.ToString()))
                {
                    errMsg += Environment.NewLine + " Make sure you've chosen a correct Charge record.";
                    result = false;
                }
            }
            #endregion

            #region validate Amount
            if (txtAmount.Text.Trim() == String.Empty)
            {
                errMsg += Environment.NewLine + "Amount cannot be blank.";
                result = false;
            }
            else
            {
                decimal amount = 0;

                if (!decimal.TryParse(txtAmount.Text.Trim(), out amount))
                {
                    errMsg += Environment.NewLine + "The format in Amount box is not correct.";
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

        private string GetChargeCode(Guid chargeId)
        {
            T_Charge charge = T_Charge.Load(chargeId);
            if (charge != null)
            {
                return charge.ChargeCode;
            }

            return string.Empty;
        }

        private void cboCharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCharge.SelectedValue != null)
            {
                if (Common.Utility.IsGUID(cboCharge.SelectedValue.ToString()))
                {
                    T_Charge charge = T_Charge.Load(new Guid(cboCharge.SelectedValue.ToString()));
                    if (charge != null)
                    {
                        txtDescription.Text = charge.ChargeName;
                    }
                }
            }
        }

        private void lvwChargeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwChargeList.SelectedItem != null)
            {
                if (Common.Utility.IsGUID(lvwChargeList.SelectedItem.Text))
                {
                    this.btnAdd.Enabled = false;
                    this.btnEdit.Enabled = true;
                    this.cmdDelete.Enabled = true;
                    this.cboCharge.Enabled = false;

                    _OrderINChargeId = new Guid(lvwChargeList.SelectedItem.Text);

                    T_Charge charge = T_Charge.LoadWhere("ChargeCode = '" + lvwChargeList.SelectedItem.SubItems[1].Text + "'");
                    if (charge != null)
                    {
                        cboCharge.Text = charge.ChargeCode;
                        cboCharge.SelectedValue = charge.ChargeId;
                    }

                    txtDescription.Text = lvwChargeList.SelectedItem.SubItems[2].Text;
                    txtAmount.Text = lvwChargeList.SelectedItem.SubItems[3].Text;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (VerifyItem())
            {
                OrderINCharges inCharge = OrderINCharges.Load(this._OrderINChargeId);
                if (inCharge != null)
                {
                    inCharge.Description = txtDescription.Text;
                    inCharge.Amount = Convert.ToDecimal(txtAmount.Text.Trim());

                    inCharge.Save();

                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, inCharge.ToString());
                }

                BindList();
                Clear();
            }
        }

        private void Clear()
        {
            this.SetCombo();

            txtDescription.Clear();
            txtAmount.Clear();

            this.btnAdd.Enabled = true;
            this.btnEdit.Enabled = false;
            this.cmdDelete.Enabled = false;
            this.cboCharge.Enabled = true;
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            OrderINCharges inCharge = OrderINCharges.Load(this._OrderINChargeId);
            if (inCharge != null)
            {
                inCharge.Delete();
            }

            BindList();
            Clear();
        }
    }
}
