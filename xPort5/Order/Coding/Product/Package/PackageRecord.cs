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

namespace xPort5.Order.Coding.Product.Package
{
    public partial class PackageRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _ProductId = System.Guid.Empty;
        private string _PackageName = String.Empty;
        private Guid _ProdPackageId = System.Guid.Empty;

        public PackageRecord()
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
        public Guid ProdPackageId
        {
            get
            {
                return _ProdPackageId;
            }
            set
            {
                _ProdPackageId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("address")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            
            this.lblProductCode.Text = oDict.GetWordWithColon("product_code");
            this.lblProductName.Text = oDict.GetWordWithColon("product_description");
            this.lblPackage.Text = oDict.GetWordWithColon("package");
            this.lblUoM.Text = oDict.GetWordWithColon("unit");
            this.lblInnerBox.Text = oDict.GetWordWithColon("inner_box");
            this.lblOuterBox.Text = oDict.GetWordWithColon("outer_box");
            this.lblLength.Text = oDict.GetWord("length");
            this.lblWidth.Text = oDict.GetWord("width");
            this.lblHeight.Text = oDict.GetWord("height");
            this.lblInInch.Text = oDict.GetWordWithColon("inch");
            this.lblInCm.Text = oDict.GetWordWithColon("cm");
            this.lblCUFT.Text = oDict.GetWord("cuft");
            this.lblVolum.Text = oDict.GetWordWithColon("volumn");
            this.lblGross.Text = oDict.GetWord("gross_weight");
            this.lblNet.Text = oDict.GetWord("net_weight");
            this.lblInLB.Text = oDict.GetWordWithColon("pound");
            this.lblInKG.Text = oDict.GetWordWithColon("kilogram");
            this.lblTotalQty.Text = oDict.GetWordWithColon("total_qty");
            this.lblTotalUnit.Text = oDict.GetWordWithColon("units_per");
            this.boxDimension.Text = oDict.GetWord("dimension");
            this.boxWeight.Text = oDict.GetWord("weight");
            this.boxContainer.Text = oDict.GetWord("container");
            this.boxLogs.Text = oDict.GetWord("logs");
            this.lblCreatedOn.Text = oDict.GetWordWithColon("created_on");
            this.lblCreatedBy.Text = oDict.GetWordWithColon("created_by");
            this.lblModifiedOb.Text = oDict.GetWordWithColon("modified_on");
            this.lblModifiedBy.Text = oDict.GetWordWithColon("modified_by");
            this.lblContainer.Text = oDict.GetWord("container");
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(cmdConvertToInch, "Convert CM to Inch");
            toolTip1.SetToolTip(cmdConvertToCm, "Convert Inch to CM");
            toolTip1.SetToolTip(cmdConvertToVolumn, "Calculate CUFT");
            toolTip1.SetToolTip(cmdConvertToLb, "Convert KG to LB");
            toolTip1.SetToolTip(cmdConvertToKg, "Convert LB to KG");

            txtItemSKU.Enabled = false;
            txtItemSKU.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;
            txtItemCode.Enabled = false;
            txtItemCode.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;
            txtDescription.Enabled = false;
            txtDescription.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;

            txtInnerBox.Validator = TextBoxValidation.FloatValidator;
            txtOuterBox.Validator = TextBoxValidation.FloatValidator;
            txtLengthInch.Validator = TextBoxValidation.FloatValidator;
            txtWidthInch.Validator = TextBoxValidation.FloatValidator;
            txtHeightInch.Validator = TextBoxValidation.FloatValidator;
            txtLengthCm.Validator = TextBoxValidation.FloatValidator;
            txtWidthCm.Validator = TextBoxValidation.FloatValidator;
            txtHeightCm.Validator = TextBoxValidation.FloatValidator;
            txtVolumn.Validator = TextBoxValidation.FloatValidator;
            txtGrossLB.Validator = TextBoxValidation.FloatValidator;
            txtNetLB.Validator = TextBoxValidation.FloatValidator;
            txtGrossKG.Validator = TextBoxValidation.FloatValidator;
            txtNetKG.Validator = TextBoxValidation.FloatValidator;
            txtTotalQty.Validator = TextBoxValidation.FloatValidator;
            txtUnitsPerContainer.Validator = TextBoxValidation.FloatValidator;

            txtInnerBox.Text = "0.00";
            txtOuterBox.Text = "0.00";
            txtLengthInch.Text = "0.00";
            txtWidthInch.Text = "0.00";
            txtHeightInch.Text = "0.00";
            txtLengthCm.Text = "0.00";
            txtWidthCm.Text = "0.00";
            txtHeightCm.Text = "0.00";
            txtVolumn.Text = "0.00";
            txtGrossLB.Text = "0.00";
            txtNetLB.Text = "0.00";
            txtGrossKG.Text = "0.00";
            txtNetKG.Text = "0.00";
            txtTotalQty.Text = "0.00";

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
            xPort5.EF6.T_Package.LoadCombo(ref cboPackage, "PackageName", false);
            T_UnitOfMeasures.LoadCombo(ref cboUoM, "UomName", false);

            xPort5.Controls.Utility.Default.UoM(ref cboUoM);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.Buttons.Clear();
            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Order", "Coding.Product");

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

        #region ShowPackage(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowPackage()
        {
            Article product = Article.Load(_ProductId);
            if (product != null)
            {
                txtItemSKU.Text = product.SKU;
                txtItemCode.Text = product.ArticleCode;
                txtDescription.Text = product.ArticleName;
            }
        }

        private void ShowProdPackage()
        {
            ArticlePackage aPackage = ArticlePackage.Load(_ProdPackageId);
            if (aPackage != null)
            {
                xPort5.EF6.T_Package package = xPort5.EF6.T_Package.Load(aPackage.PackageId);
                if (package != null)
                {
                    cboPackage.Text = package.PackageName;
                    cboPackage.SelectedValue = package.PackageId;
                }
                if (aPackage.UomId.HasValue)
                {
                    T_UnitOfMeasures uom = T_UnitOfMeasures.Load(aPackage.UomId.Value);
                    if (uom != null)
                    {
                        cboUoM.Text = uom.UomName;
                        cboUoM.SelectedValue = uom.UomId;
                    }
                }

                txtInnerBox.Text = aPackage.InnerBox.ToString("##0.00");
                txtOuterBox.Text = aPackage.OuterBox.ToString("##0.00");

                ShowDimensions(aPackage.SizeLength_in, aPackage.SizeWidth_in, aPackage.SizeHeight_in, aPackage.SizeLength_cm, aPackage.SizeWidth_cm, aPackage.SizeHeight_cm);

                txtVolumn.Text = aPackage.CUFT.ToString("##0.00");

                ShowWeight(aPackage.WeightGross_lb, aPackage.WeightNet_lb, aPackage.WeightGross_kg, aPackage.WeightNet_kg);

                txtTotalQty.Text = aPackage.ContainerQty.ToString("##0.00");
                txtUnitsPerContainer.Text = aPackage.ContainerSize;

                Staff s1 = Staff.Load(aPackage.CreatedBy);
                Staff s2 = Staff.Load(aPackage.ModifiedBy);
                txtCreatedOn.Text = aPackage.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                txtCreatedBy.Text = s1.Alias;
                txtModifiedOn.Text = aPackage.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
                txtModifiedBy.Text = s2.Alias;
                boxLogs.Visible = true;

                _PackageName = package.PackageName;
            }
        }

        private void ShowDimensions(Decimal lengthInch, Decimal widthInch, Decimal heightInch, Decimal lengthCm, Decimal widthCm, Decimal heightCm)
        {
            txtLengthInch.Text = lengthInch.ToString("###0.00");
            txtWidthInch.Text = widthInch.ToString("###0.00");
            txtHeightInch.Text = heightInch.ToString("###0.00");

            txtLengthCm.Text = lengthCm.ToString("###0.00");
            txtWidthCm.Text = widthCm.ToString("###0.00");
            txtHeightCm.Text = heightCm.ToString("###0.00");
        }

        private void ShowWeight(Decimal grossLb, Decimal netLb, Decimal grossKg, Decimal netKg)
        {
            txtGrossLB.Text = grossLb.ToString("###0.00");
            txtNetLB.Text = netLb.ToString("###0.00");
            txtGrossKG.Text = grossKg.ToString("###0.00");
            txtNetKG.Text = netKg.ToString("###0.00");
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    ArticlePackage prodPackage = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            prodPackage = new ArticlePackage();
                            prodPackage.ArticleId = _ProductId;
                            prodPackage.CreatedOn = DateTime.Now;
                            prodPackage.CreatedBy = Common.Config.CurrentUserId;
                            prodPackage.ModifiedOn = DateTime.Now;
                            prodPackage.ModifiedBy = Common.Config.CurrentUserId;
                            prodPackage.Retired = false;
                            if (xPort5.Controls.Utility.ProductPackage.Count(_ProductId) == 0)
                            {
                                prodPackage.DefaultRec = true;
                            }
                            else
                            {
                                prodPackage.DefaultRec = false;
                            }
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            prodPackage = ArticlePackage.Load(_ProdPackageId);
                            prodPackage.ModifiedOn = DateTime.Now;
                            prodPackage.ModifiedBy = Common.Config.CurrentUserId;
                            break;
                    }

                    prodPackage.PackageId = (Guid)cboPackage.SelectedValue;
                    if (cboUoM.SelectedValue != null)
                    {
                        prodPackage.UomId = (Guid)cboUoM.SelectedValue;
                    }
                    else
                    {
                        prodPackage.UomId = null;
                    }

                    prodPackage.InnerBox = Convert.ToDecimal(txtInnerBox.Text.Trim());
                    prodPackage.OuterBox = Convert.ToDecimal(txtOuterBox.Text.Trim());

                    prodPackage.SizeLength_in = Convert.ToDecimal(txtLengthInch.Text.Trim());
                    prodPackage.SizeWidth_in = Convert.ToDecimal(txtWidthInch.Text.Trim());
                    prodPackage.SizeHeight_in = Convert.ToDecimal(txtHeightInch.Text.Trim());
                    prodPackage.SizeLength_cm = Convert.ToDecimal(txtLengthCm.Text.Trim());
                    prodPackage.SizeWidth_cm = Convert.ToDecimal(txtWidthCm.Text.Trim());
                    prodPackage.SizeHeight_cm = Convert.ToDecimal(txtHeightCm.Text.Trim());
                    prodPackage.CUFT = Convert.ToDecimal(txtVolumn.Text.Trim());

                    prodPackage.WeightGross_lb = Convert.ToDecimal(txtGrossLB.Text.Trim());
                    prodPackage.WeightNet_lb = Convert.ToDecimal(txtNetLB.Text.Trim());
                    prodPackage.WeightGross_kg = Convert.ToDecimal(txtGrossKG.Text.Trim());
                    prodPackage.WeightNet_kg = Convert.ToDecimal(txtNetKG.Text.Trim());

                    prodPackage.ContainerQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                    prodPackage.ContainerSize = txtUnitsPerContainer.Text.Trim();

                    prodPackage.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, prodPackage.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, prodPackage.ToString());
                    #endregion

                    if (prodPackage.ArticlePackageId != _ProdPackageId)
                    {
                        _ProdPackageId = prodPackage.ArticlePackageId;
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
                #region validate Package

                // 2010-09-07 [david] : ȡ��
                //try
                //{
                //    sql = String.Format("ArticleId = '{0}' AND PackageId = '{1}'",
                //        _ProductId.ToString(),
                //        ((Guid)cboPackage.SelectedValue).ToString());
                //    ArticlePackage aPackage = ArticlePackage.LoadWhere(sql);
                //    if (aPackage != null)
                //    {
                //        errMsg += Environment.NewLine + "Package is in use.";
                //        result = false;
                //    }
                //}
                //catch
                //{
                //    errMsg += Environment.NewLine + "Article + Package is invalid.";
                //    result = false;
                //}
                #endregion
            }
            #region validate InnerBox
            try
            {
                decimal innerBox = Convert.ToDecimal(txtInnerBox.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Inner Box must be decimal.";
                result = false;
            }
            #endregion

            #region validate Outer Box
            try
            {
                decimal outerBox = Convert.ToDecimal(txtOuterBox.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Outer Box must be decimal.";
                result = false;
            }
            #endregion

            #region validate Length Inch
            try
            {
                decimal lengthInch = Convert.ToDecimal(txtLengthInch.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Length in Inch must be decimal.";
                result = false;
            }
            #endregion

            #region validate Width Inch
            try
            {
                decimal widthInch = Convert.ToDecimal(txtWidthInch.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Width in Inch must be decimal.";
                result = false;
            }
            #endregion

            #region validate Height Inch
            try
            {
                decimal heightInch = Convert.ToDecimal(txtHeightInch.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Height in Inch must be decimal.";
                result = false;
            }
            #endregion

            #region validate Length Cm
            try
            {
                decimal lengthCm = Convert.ToDecimal(txtLengthCm.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Length in cm must be decimal.";
                result = false;
            }
            #endregion

            #region validate Width Cm
            try
            {
                decimal widthCm = Convert.ToDecimal(txtWidthCm.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Width in cm must be decimal.";
                result = false;
            }
            #endregion

            #region validate Height Cm
            try
            {
                decimal heightCm = Convert.ToDecimal(txtHeightCm.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Height in cm must be decimal.";
                result = false;
            }
            #endregion

            #region validate Gross Lb
            try
            {
                decimal grossLb = Convert.ToDecimal(txtGrossLB.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Gross Weight in Lb must be decimal.";
                result = false;
            }
            #endregion

            #region validate Net Lb
            try
            {
                decimal netLb = Convert.ToDecimal(txtNetLB.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Net Weight in Lb must be decimal.";
                result = false;
            }
            #endregion

            #region validate Gross Kg
            try
            {
                decimal grossKg = Convert.ToDecimal(txtGrossKG.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Gross Weight in Kg must be decimal.";
                result = false;
            }
            #endregion

            #region validate Net Kg
            try
            {
                decimal netKg = Convert.ToDecimal(txtNetKG.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Net Weight in Kg must be decimal.";
                result = false;
            }
            #endregion

            #region validate Total Qty
            try
            {
                decimal totalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
            }
            catch
            {
                errMsg += Environment.NewLine + "Total Qty must be decimal.";
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
            return xPort5.Controls.Utility.ProductPackage.DeleteRec(_ProdPackageId);
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
                        MessageBox.Show(String.Format("Package {0} is saved!", _PackageName), "Save Result");
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
                        MessageBox.Show(String.Format("Package {0} is saved!", _PackageName), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Package {0} is saved!", _PackageName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _ProdPackageId = System.Guid.NewGuid();
                            _PackageName = string.Empty;

                            this.SetAnsToolbar();
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
                        MessageBox.Show(String.Format("Package {0} is saved!", _PackageName), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _ProdPackageId = System.Guid.NewGuid();
                            _PackageName = string.Empty;

                            this.SetAnsToolbar();
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
                        MessageBox.Show(String.Format("Package {0} is deleted.", _PackageName), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void PackageRecord_Load(object sender, EventArgs e)
        {
            SetAnsToolbar();
            SetAttributes();
            SetCaptions();
            SetDropdowns();
            ShowPackage();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _ProdPackageId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowProdPackage();
                    break;
            }
        }

        #region textbox got focus selectall
        private void txtInnerBox_GotFocus(object sender, EventArgs e)
        {
            txtInnerBox.SelectAll();
        }

        private void txtOuterBox_GotFocus(object sender, EventArgs e)
        {
            txtOuterBox.SelectAll();
        }

        private void txtLengthInch_GotFocus(object sender, EventArgs e)
        {
            txtLengthInch.SelectAll();
        }

        private void txtWidthInch_GotFocus(object sender, EventArgs e)
        {
            txtWidthInch.SelectAll();
        }

        private void txtHeightInch_GotFocus(object sender, EventArgs e)
        {
            txtHeightInch.SelectAll();
        }

        private void txtLengthCm_GotFocus(object sender, EventArgs e)
        {
            txtLengthCm.SelectAll();
        }

        private void txtWidthCm_GotFocus(object sender, EventArgs e)
        {
            txtWidthCm.SelectAll();
        }

        private void txtHeightCm_GotFocus(object sender, EventArgs e)
        {
            txtHeightCm.SelectAll();
        }

        private void txtVolumn_GotFocus(object sender, EventArgs e)
        {
            txtVolumn.SelectAll();
        }

        private void txtGrossLB_GotFocus(object sender, EventArgs e)
        {
            txtGrossLB.SelectAll();
        }

        private void txtNetLB_GotFocus(object sender, EventArgs e)
        {
            txtNetLB.SelectAll();
        }

        private void txtGrossKG_GotFocus(object sender, EventArgs e)
        {
            txtGrossKG.SelectAll();
        }

        private void txtNetKG_GotFocus(object sender, EventArgs e)
        {
            txtNetKG.SelectAll();
        }

        private void txtTotalQty_GotFocus(object sender, EventArgs e)
        {
            txtTotalQty.SelectAll();
        }
        #endregion

        #region convertion click
        private void cmdConvertToInch_Click(object sender, EventArgs e)
        {
            Decimal ratio = 0.3937M;
            Decimal lengthInch = 0;
            Decimal widthInch = 0;
            Decimal heightInch = 0;
            Decimal lengthCm = 0;
            Decimal widthCm = 0;
            Decimal heightCm = 0;

            try
            {
                lengthCm = Math.Round(Convert.ToDecimal(txtLengthCm.Text.Trim()), 2);
                widthCm = Math.Round(Convert.ToDecimal(txtWidthCm.Text.Trim()), 2);
                heightCm = Math.Round(Convert.ToDecimal(txtHeightCm.Text.Trim()), 2);
                lengthInch = Math.Round(ratio * lengthCm, 2);
                widthInch = Math.Round(ratio * widthCm, 2);
                heightInch = Math.Round(ratio * heightCm, 2);
            }
            finally
            {
                ShowDimensions(lengthInch, widthInch, heightInch, lengthCm, widthCm, heightCm);
            }
        }

        private void cmdConvertToCm_Click(object sender, EventArgs e)
        {
            Decimal ratio = 2.54M;
            Decimal lengthInch = 0;
            Decimal widthInch = 0;
            Decimal heightInch = 0;
            Decimal lengthCm = 0;
            Decimal widthCm = 0;
            Decimal heightCm = 0;

            try
            {
                lengthInch = Math.Round(Convert.ToDecimal(txtLengthInch.Text.Trim()), 2);
                widthInch = Math.Round(Convert.ToDecimal(txtWidthInch.Text.Trim()), 2);
                heightInch = Math.Round(Convert.ToDecimal(txtHeightInch.Text.Trim()), 2);
                lengthCm = Math.Round(ratio * lengthInch, 2);
                widthCm = Math.Round(ratio * widthInch, 2);
                heightCm = Math.Round(ratio * heightInch, 2);
            }
            finally
            {
                ShowDimensions(lengthInch, widthInch, heightInch, lengthCm, widthCm, heightCm);
            }
        }

        private void cmdConvertToVolumn_Click(object sender, EventArgs e)
        {
            Decimal ratio = 1728M;
            Decimal lengthInch = 0;
            Decimal widthInch = 0;
            Decimal heightInch = 0;
            Decimal volumn = 0;

            try
            {
                lengthInch = Math.Round(Convert.ToDecimal(txtLengthInch.Text.Trim()), 2);
                widthInch = Math.Round(Convert.ToDecimal(txtWidthInch.Text.Trim()), 2);
                heightInch = Math.Round(Convert.ToDecimal(txtHeightInch.Text.Trim()), 2);

                volumn = Math.Round((lengthInch * widthInch * heightInch) / ratio, 2);
            }
            finally
            {
                txtVolumn.Text = volumn.ToString("###0.00");
            }
        }

        private void cmdConvertToLb_Click(object sender, EventArgs e)
        {
            Decimal ratio = 2.2046M;
            Decimal grossLb = 0;
            Decimal netLb = 0;
            Decimal grossKg = 0;
            Decimal netKg = 0;

            try
            {
                grossKg = Math.Round(Convert.ToDecimal(txtGrossKG.Text.Trim()), 2);
                netKg = Math.Round(Convert.ToDecimal(txtNetKG.Text.Trim()), 2);
                grossLb = Math.Round(ratio * grossKg, 2);
                netLb = Math.Round(ratio * netKg, 2);
            }
            finally
            {
                ShowWeight(grossLb, netLb, grossKg, netKg);
            }
        }

        private void cmdConvertToKg_Click(object sender, EventArgs e)
        {
            Decimal ratio = 0.4536M;
            Decimal grossLb = 0;
            Decimal netLb = 0;
            Decimal grossKg = 0;
            Decimal netKg = 0;

            try
            {
                grossLb = Math.Round(Convert.ToDecimal(txtGrossLB.Text.Trim()), 2);
                netLb = Math.Round(Convert.ToDecimal(txtNetLB.Text.Trim()), 2);
                grossKg = Math.Round(ratio * grossLb, 2);
                netKg = Math.Round(ratio * netLb, 2);
            }
            finally
            {
                ShowWeight(grossLb, netLb, grossKg, netKg);
            }
        }
        #endregion
    }
}
