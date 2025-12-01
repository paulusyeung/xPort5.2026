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

namespace xPort5.Order.Invoice.Items
{
    public partial class ShippingInfo : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _OrderINId = System.Guid.Empty;
        private Guid _OrderINItemId = System.Guid.Empty;

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
        public Guid OrderINItemId
        {
            get
            {
                return _OrderINItemId;
            }
            set
            {
                _OrderINItemId = value;
            }
        }

        public int LineNumber { get; set; }
        #endregion

        public ShippingInfo()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCaptions();
            SetAttributes();
            SetCombo();
            SetAnsToolbar();

            if (_EditMode == Common.Enums.EditMode.Edit)
            {
                ShowPackage();
            }
        }

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = oDict.GetWord("shipping_info");

            gbShippingInfo.Text = oDict.GetWord("shipping_info");
            lblLineNumber.Text = oDict.GetWordWithColon("line_no");
            lblShippingMark.Text = oDict.GetWordWithColon("shipping_mark");

            gbPacking.Text = oDict.GetWord("packing");
            lblInnerBox.Text = oDict.GetWordWithColon("inner_box");
            lblOuterBox.Text = oDict.GetWordWithColon("outer_box");

            boxDimension.Text = oDict.GetWord("dimension");
            lblLength.Text = oDict.GetWord("length");
            lblWidth.Text = oDict.GetWord("width");
            lblHeight.Text = oDict.GetWord("height");
            lblInInch.Text = oDict.GetWordWithColon("inch");
            lblInCm.Text = oDict.GetWordWithColon("cm");
            lblVolum.Text = oDict.GetWordWithColon("volumn");
            lblCUFT.Text = oDict.GetWordWithBracket("cuft");

            boxWeight.Text = oDict.GetWord("weight");
            lblGross.Text = oDict.GetWord("gross_weight");
            lblNet.Text = oDict.GetWord("net_weight");
            lblInLB.Text = oDict.GetWordWithColon("pound");
            lblInKG.Text = oDict.GetWordWithColon("kilogram");
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(cmdConvertToInch, "Convert CM to Inch");
            toolTip1.SetToolTip(cmdConvertToCm, "Convert Inch to CM");
            toolTip1.SetToolTip(cmdConvertToVolumn, "Calculate CUFT");
            toolTip1.SetToolTip(cmdConvertToLb, "Convert KG to LB");
            toolTip1.SetToolTip(cmdConvertToKg, "Convert LB to KG");

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

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    break;
            }
        }

        private void SetCombo()
        {
            T_ShippingMark.LoadCombo(ref cboShippingMark, "ShippingMarkCode", false);
        }

        private void SetAnsToolbar()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            // cmdSaveClose
            ToolBarButton cmdSaveClose = new ToolBarButton("Save & Close", System.Web.HttpUtility.UrlDecode(oDict.GetWord("save_close")));
            cmdSaveClose.Tag = "Save & Close";
            cmdSaveClose.Image = new IconResourceHandle("16x16.16_saveClose.gif");

            if (_EditMode != Common.Enums.EditMode.Read)
            {
                if (xPort5.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.Invoice.Create") ||
                        xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.Invoice.Update"))
                    this.ansToolbar.Buttons.Add(cmdSaveClose);
                }
                else
                {
                    this.ansToolbar.Buttons.Add(cmdSaveClose);
                }
            }

            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
        }

        private void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString().ToLower())
                {
                    case "save & close":
                        MessageBox.Show("Save Item And Close?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveClose_Click));
                        break;
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
                        MessageBox.Show("Package info is saved!", "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdCloseForm(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ShowPackage(), SaveItem(), VerifyItem(), DeleteItem(), CalcTotalQty()

        private void ShowPackage()
        {
            OrderINItems item = OrderINItems.Load(this._OrderINItemId);
            if (item != null)
            {
                txtLineNumber.Text = item.LineNumber.HasValue ? item.LineNumber.Value.ToString() : "";
                txtShippingMark.Text = item.ShippingMark;

                string sql = "OrderQTItemId = '" + xPort5.Controls.Utility.OrderSC.OrderQTItemId(item.OrderSCItemsId).ToString() + "'";
                OrderQTPackage qtPackage = OrderQTPackage.LoadWhere(sql);
                if (qtPackage != null)
                {
                    lblUnitOfInnerBox.Text = qtPackage.Unit;
                    lblUnitOfOuterBox.Text = qtPackage.Unit;

                    txtInnerBox.Text = qtPackage.InnerBox.ToString("##0.00");
                    txtOuterBox.Text = qtPackage.OuterBox.ToString("##0.00");

                    ShowDimensions(qtPackage.SizeLength_in, qtPackage.SizeWidth_in, qtPackage.SizeHeight_in, qtPackage.SizeLength_cm, qtPackage.SizeWidth_cm, qtPackage.SizeHeight_cm);

                    txtVolumn.Text = qtPackage.CUFT.ToString("##0.00");

                    ShowWeight(qtPackage.WeightGross_lb, qtPackage.WeightNet_lb, qtPackage.WeightGross_kg, qtPackage.WeightNet_kg);
                }
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
                    OrderINItems item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = OrderINItems.Load(_OrderINItemId);
                            break;
                    }

                    item.ShippingMark = txtShippingMark.Text;
                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    string sql = "OrderQTItemId = '" + xPort5.Controls.Utility.OrderSC.OrderQTItemId(item.OrderSCItemsId).ToString() + "'";
                    OrderQTPackage qtPackage = OrderQTPackage.LoadWhere(sql);
                    if (qtPackage != null)
                    {
                        qtPackage.InnerBox = Convert.ToDecimal(txtInnerBox.Text.Trim());
                        qtPackage.OuterBox = Convert.ToDecimal(txtOuterBox.Text.Trim());

                        qtPackage.SizeLength_in = Convert.ToDecimal(txtLengthInch.Text.Trim());
                        qtPackage.SizeWidth_in = Convert.ToDecimal(txtWidthInch.Text.Trim());
                        qtPackage.SizeHeight_in = Convert.ToDecimal(txtHeightInch.Text.Trim());
                        qtPackage.SizeLength_cm = Convert.ToDecimal(txtLengthCm.Text.Trim());
                        qtPackage.SizeWidth_cm = Convert.ToDecimal(txtWidthCm.Text.Trim());
                        qtPackage.SizeHeight_cm = Convert.ToDecimal(txtHeightCm.Text.Trim());
                        qtPackage.CUFT = Convert.ToDecimal(txtVolumn.Text.Trim());

                        qtPackage.WeightGross_lb = Convert.ToDecimal(txtGrossLB.Text.Trim());
                        qtPackage.WeightNet_lb = Convert.ToDecimal(txtNetLB.Text.Trim());
                        qtPackage.WeightGross_kg = Convert.ToDecimal(txtGrossKG.Text.Trim());
                        qtPackage.WeightNet_kg = Convert.ToDecimal(txtNetKG.Text.Trim());

                        qtPackage.Save();
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

            if (!(result))
            {
                MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        private bool DeleteItem()
        {
            return xPort5.Controls.Utility.OrderIN.DeleteItem(this._OrderINItemId);
        }
        #endregion

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

        private void cboShippingMark_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            try
            {
                T_ShippingMark mark = T_ShippingMark.Load((Guid)s.SelectedValue);
                if (mark != null)
                {
                    if (txtShippingMark.Text.Trim().Length > 0)
                    {
                        txtShippingMark.Text = Environment.NewLine;
                    }

                    txtShippingMark.Text += mark.ShippingMarkName;
                }
            }
            catch { }
        }
    }
}
