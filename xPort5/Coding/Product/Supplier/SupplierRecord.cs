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

namespace xPort5.Coding.Product.Supplier
{
    public partial class SupplierRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _ProductId = System.Guid.Empty;
        private string _SupplierName = String.Empty;
        private Guid _ProdSupplierId = System.Guid.Empty;

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
        public Guid ProductId
        {
            get
            {
                return _ProductId;
            }
            set
            {
                _ProductId = value;
            }
        }
        public Guid ProdSupplierId
        {
            get
            {
                return _ProdSupplierId;
            }
            set
            {
                _ProdSupplierId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            lblProductCode.Text = oDict.GetWordWithColon("product_code");
            lblProductName.Text = oDict.GetWordWithColon("product_description");
            lblSupplier.Text = oDict.GetWordWithColon("supplier");
            lblRefNumber.Text = oDict.GetWordWithColon("ref_number");
            lblFCL.Text = oDict.GetWordWithColon("fcl_cost");
            lblLCL.Text = oDict.GetWordWithColon("lcl_cost");
            lblUnitCost.Text = oDict.GetWordWithColon("unit_cost");
            lblCurrency.Text = oDict.GetWordWithColon("currency");
            lblNotes.Text = oDict.GetWordWithColon("note");

            boxLogs.Text = oDict.GetWord("logs");
            lblCreatedOn.Text = oDict.GetWordWithColon("created_on");
            lblCreatedBy.Text = oDict.GetWordWithColon("created_by");
            lblModifiedOn.Text = oDict.GetWordWithColon("modified_on");
            lblModifiedBy.Text = oDict.GetWordWithColon("modified_by");
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(txtDescription, "Numeric only");

            txtItemSKU.Enabled = false;
            txtItemSKU.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;
            txtItemCode.Enabled = false;
            txtItemCode.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;
            txtDescription.Enabled = false;
            txtDescription.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;

            txtFCL.Validator = TextBoxValidation.FloatValidator;
            txtLCL.Validator = TextBoxValidation.FloatValidator;
            txtUnitCost.Validator = TextBoxValidation.FloatValidator;

            txtFCL.Text = "0.0000";
            txtLCL.Text = "0.0000";
            txtUnitCost.Text = "0.0000";

            if (!(xPort5.Controls.Utility.Staff.IsAccessAllowed(Common.Enums.UserGroup.Senior)))
            {
                cboSupplier.Enabled = false;
            }

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
            cboSupplier.DataSource = null;
            xPort5.EF6.Supplier.LoadCombo(ref cboSupplier, "SupplierName", false);

            cboCurrency.DataSource = null;
            T_Currency.LoadCombo(ref cboCurrency, "CurrencyCode", false);

            xPort5.Controls.Utility.Default.Currency(ref cboCurrency);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.Buttons.Clear();

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Factory", "Product");

            this.ansToolbar.ButtonClick -= new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
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

        #region ShowProduct(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowProduct()
        {
            Article product = Article.Load(_ProductId);
            if (product != null)
            {
                txtItemSKU.Text = product.SKU;
                txtItemCode.Text = product.ArticleCode;
                txtDescription.Text = product.ArticleName;
            }
        }

        private void ShowProdSupplier()
        {
            ArticleSupplier prodSupplier = ArticleSupplier.Load(_ProdSupplierId);
            if (prodSupplier != null)
            {
                txtRefNumber.Text = prodSupplier.SuppRef;
                txtFCL.Text = prodSupplier.FCLCost.ToString("##0.0000");
                txtLCL.Text = prodSupplier.LCLCost.ToString("##0.0000");
                txtUnitCost.Text = prodSupplier.UnitCost.ToString("##0.0000");
                txtNotes.Text = prodSupplier.Notes;

                xPort5.EF6.Supplier supplier = xPort5.EF6.Supplier.Load(prodSupplier.SupplierId);
                if (supplier != null)
                {
                    cboSupplier.Text = supplier.SupplierName;
                    cboSupplier.SelectedValue = supplier.SupplierId;
                }
                T_Currency currency = T_Currency.Load(prodSupplier.CurrencyId);
                if (currency != null)
                {
                    cboCurrency.Text = currency.CurrencyCode;
                    cboCurrency.SelectedValue = currency.CurrencyId;
                }

                Staff s1 = Staff.Load(prodSupplier.CreatedBy);
                Staff s2 = Staff.Load(prodSupplier.ModifiedBy);
                txtCreatedOn.Text = prodSupplier.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                txtCreatedBy.Text = s1.Alias;
                txtModifiedOn.Text = prodSupplier.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
                txtModifiedBy.Text = s2.Alias;
                boxLogs.Visible = true;

                _SupplierName = supplier.SupplierName;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    ArticleSupplier prodSupplier = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            prodSupplier = new ArticleSupplier();
                            prodSupplier.ArticleId = _ProductId;
                            prodSupplier.CreatedOn = DateTime.Now;
                            prodSupplier.CreatedBy = Common.Config.CurrentUserId;
                            prodSupplier.ModifiedOn = DateTime.Now;
                            prodSupplier.ModifiedBy = Common.Config.CurrentUserId;
                            prodSupplier.Retired = false;
                            if (xPort5.Controls.Utility.ProductSupplier.Count(_ProductId) == 0)
                            {
                                prodSupplier.DefaultRec = true;
                            }
                            else
                            {
                                prodSupplier.DefaultRec = false;
                            }
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            prodSupplier = ArticleSupplier.Load(_ProdSupplierId);
                            prodSupplier.ModifiedOn = DateTime.Now;
                            prodSupplier.ModifiedBy = Common.Config.CurrentUserId;
                            break;
                    }
                    prodSupplier.SupplierId = (Guid)cboSupplier.SelectedValue;
                    prodSupplier.SuppRef = txtRefNumber.Text.Trim();
                    prodSupplier.CurrencyId = (Guid)cboCurrency.SelectedValue;
                    prodSupplier.FCLCost = Convert.ToDecimal(txtFCL.Text.Trim());
                    prodSupplier.LCLCost = Convert.ToDecimal(txtLCL.Text.Trim());
                    prodSupplier.UnitCost = Convert.ToDecimal(txtUnitCost.Text.Trim());
                    prodSupplier.Notes = txtNotes.Text.Trim();

                    prodSupplier.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, prodSupplier.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, prodSupplier.ToString());
                    #endregion

                    if (prodSupplier.ArticleSupplierId != _ProdSupplierId)
                    {
                        _ProdSupplierId = prodSupplier.ArticleSupplierId;
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
                #region validate Supplier
                try
                {
                    sql = String.Format("ArticleId = '{0}' AND SupplierId = '{1}'",
                        _ProductId.ToString(),
                        ((Guid)cboSupplier.SelectedValue).ToString());
                    ArticleSupplier aSupplier = ArticleSupplier.LoadWhere(sql);
                    if (aSupplier != null)
                    {
                        errMsg += Environment.NewLine + "Supplier is in use.";
                        result = false;
                    }
                }
                catch
                {
                    errMsg += Environment.NewLine + "Article + Supplier is invalid.";
                    result = false;
                }
                #endregion
            }
            #region validate FCL
            try
            {
                decimal fcl = Convert.ToDecimal(txtFCL.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "FCL must be decimal.";
                result = false;
            }
            #endregion

            #region validate LCL
            try
            {
                decimal lcl = Convert.ToDecimal(txtLCL.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "LCL must be decimal.";
                result = false;
            }
            #endregion

            #region validate Unit Cost
            try
            {
                decimal unitcost = Convert.ToDecimal(txtUnitCost.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Unit Cost must be decimal.";
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
            return xPort5.Controls.Utility.ProductSupplier.DeleteRec(_ProdSupplierId);
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
                        MessageBox.Show(String.Format("Supplier {0} is saved!", _SupplierName), "Save Result");
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
                        MessageBox.Show(String.Format("Supplier {0} is saved!", _SupplierName), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Supplier {0} is saved!", _SupplierName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _ProdSupplierId = System.Guid.NewGuid();
                            _SupplierName = string.Empty;

                            SetAnsToolbar();
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
                        MessageBox.Show(String.Format("Supplier {0} is saved!", _SupplierName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _ProdSupplierId = System.Guid.NewGuid();

                            SetAnsToolbar();
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
                        MessageBox.Show(String.Format("Supplier {0} is deleted.", _SupplierName), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
            SetCaptions();
            SetAttributes();
            SetDropdowns();
            ShowProduct();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _ProdSupplierId = System.Guid.NewGuid();

                    if (!(xPort5.Controls.Utility.Staff.IsAccessAllowed(Common.Enums.UserGroup.Senior)))
                    {
                        xPort5.EF6.Supplier supp = xPort5.EF6.Supplier.Load(xPort5.Controls.Utility.Staff.GetSupplierId(Common.Config.CurrentUserId));
                        if (supp != null)
                        {
                            cboSupplier.SelectedValue = supp.SupplierId;
                            cboSupplier.Text = supp.SupplierName;
                        }
                    }
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowProdSupplier();
                    break;
            }
        }

        private void txtFCL_GotFocus(object sender, EventArgs e)
        {
            txtFCL.SelectAll();
        }

        private void txtLCL_GotFocus(object sender, EventArgs e)
        {
            txtLCL.SelectAll();
        }

        private void txtUnitCost_GotFocus(object sender, EventArgs e)
        {
            txtUnitCost.SelectAll();
        }
    }
}
