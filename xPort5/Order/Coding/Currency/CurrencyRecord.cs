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

namespace xPort5.Order.Coding.Currency
{
    public partial class CurrencyRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _CurrencyId = System.Guid.Empty;
        private string _CurrencyCode = String.Empty;

        public CurrencyRecord()
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
        public Guid CurrencyId
        {
            get
            {
                return _CurrencyId;
            }
            set
            {
                _CurrencyId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("customer")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            this.lblCode.Text = oDict.GetWordWithColon("currency");
            this.lblName.Text = oDict.GetWordWithColon("currency_name");
            this.lblName_Chs.Text = oDict.GetWordWithColon("currency_name_chs");
            this.lblName_Cht.Text = oDict.GetWordWithColon("currency_name_cht");
            this.lblXchgRate.Text = oDict.GetWordWithColon("xchg_rate");
            this.lblLocalCurrency.Text = oDict.GetWordWithColon("is_local_currency");
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(cmdConvertToChs, "Convert to Chs");
            toolTip1.SetToolTip(cmdConvertToCht, "Convert to Cht");
            toolTip1.SetToolTip(cmdXchgRate, "Calculate Exchange Rate");

            txtLocalCurrency.Validator = TextBoxValidation.FloatValidator;
            txtForeignCurrency.Validator = TextBoxValidation.FloatValidator;
            txtXchgRate.Validator = TextBoxValidation.FloatValidator;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    txtLocalCurrency.Text = "1.0000";
                    txtForeignCurrency.Text = "1.0000";
                    txtXchgRate.Text = "1.0000";
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    break;
            }
        }

        private void SetDropdowns()
        {
            Common.Data.LoadCombo_XchgBase(ref cboXchgBase);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Coding.Currency");

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
        private void ShowItem()
        {
            T_Currency item = T_Currency.Load(_CurrencyId);
            if (item != null)
            {
                txtCode.Text = item.CurrencyCode;
                txtName.Text = item.CurrencyName;
                txtName_Chs.Text = item.CurrencyName_Chs;
                txtName_Cht.Text = item.CurrencyName_Cht;
                txtLocalCurrency.Text = item.LocalCny.ToString("###0.0000");
                txtForeignCurrency.Text = item.ForeignCny.ToString("###0.0000");
                txtXchgRate.Text = item.XchgRate.ToString("###0.0000");
                cboXchgBase.SelectedIndex = item.XchgBase;
                chkLocalCurrency.Checked = item.LocalCurrency;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    if (chkLocalCurrency.Checked)
                    {
                        xPort5.Controls.Utility.Currency.ResetLocalCurrency();
                    }

                    T_Currency item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new T_Currency();
                            item.CurrencyId = _CurrencyId;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = T_Currency.Load(_CurrencyId);
                            break;
                    }
                    item.CurrencyCode = txtCode.Text.Trim();
                    item.CurrencyName = txtName.Text.Trim();
                    item.CurrencyName_Chs = txtName_Chs.Text.Trim();
                    item.CurrencyName_Cht = txtName_Cht.Text.Trim();
                    item.LocalCny = Convert.ToDecimal(txtLocalCurrency.Text);
                    item.ForeignCny = Convert.ToDecimal(txtForeignCurrency.Text);
                    item.XchgBase = cboXchgBase.SelectedIndex;
                    item.XchgRate = Convert.ToDecimal(txtXchgRate.Text);
                    item.LocalCurrency = chkLocalCurrency.Checked;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.CurrencyId != _CurrencyId)
                    {
                        _CurrencyId = item.CurrencyId;
                    }
                    _CurrencyCode = item.CurrencyCode;
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

            #region validate Code
            try
            {
                sql = String.Format("CurrencyCode = '{0}'", txtCode.Text.Trim());
                T_Currency cny = T_Currency.LoadWhere(sql);
                if (cny != null)
                {
                    if (cny.CurrencyId != _CurrencyId)
                    {
                        errMsg += Environment.NewLine + "Code is in use.";
                        result = false;
                    }
                }
            }
            catch
            {
                errMsg += Environment.NewLine + "Code is invalid.";
                result = false;
            }
            #endregion

            #region validate Exchange Rate
            try
            {
                Decimal rate = Convert.ToDecimal(txtXchgRate.Text);
                if (rate == 0)
                {
                    errMsg += Environment.NewLine + "Exchange Rate cannot be 0.";
                    result = false;
                }
            }
            catch
            {
                errMsg += Environment.NewLine + "Exchange Rate must be decimal.";
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
            bool result = false;

            try
            {
                // log activity
                T_Currency cny = T_Currency.Load(_CurrencyId);
                if (cny != null)
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, cny.ToString());

                T_Currency.Delete(_CurrencyId);
                result = true;
            }
            catch { }
            return result;
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CurrencyCode), "Save Result");
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CurrencyCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CurrencyCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _CurrencyId = System.Guid.NewGuid();
                            _CurrencyCode = string.Empty;

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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CurrencyCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _CurrencyId = System.Guid.NewGuid();

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
                        MessageBox.Show(String.Format("Item {0} is deleted.", _CurrencyCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void SupplierRecord_Load(object sender, EventArgs e)
        {
            SetAnsToolbar();
            SetAttributes();
            SetCaptions();
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _CurrencyId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowItem();
                    break;
            }
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

        private void cmdXchgRate_Click(object sender, EventArgs e)
        {
            Decimal rate = 0;

            rate = xPort5.Controls.Utility.Currency.XchgRate(txtLocalCurrency.Text.Trim(), txtForeignCurrency.Text.Trim(), cboXchgBase.SelectedIndex);

            txtXchgRate.Text = rate.ToString("###0.0000");
        }
    }
}