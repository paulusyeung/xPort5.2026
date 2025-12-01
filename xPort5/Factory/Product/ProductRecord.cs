#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;
using xPort5.Controls;
using xPort5.Controls.Product;

#endregion

namespace xPort5.Factory.Product
{
    public partial class ProductRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _ProductId = System.Guid.Empty;
        private string _ProductCode = String.Empty;
        private Guid _Dup_ProductId = System.Guid.Empty;

        public ProductRecord()
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
        #endregion

        private void ArticleRecord_Load(object sender, EventArgs e)
        {
            this.Init();
        }

        private void Init()
        {
            SetAnsToolbar();
            SetCaptions();
            SetAttributes();
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _ProductId = System.Guid.NewGuid();
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    ShowProduct();
                    break;
            }
            LoadChildControls();
        }

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format(oDict.GetWord("record"), oDict.GetWord("product"));

            lblProductCode.Text = oDict.GetWordWithColon("product_code");
            lblProductName.Text = oDict.GetWordWithColon("product_description");
            lblBarcode.Text = oDict.GetWordWithColon("barcode");
            lblCategory.Text = oDict.GetWordWithColon("category");
            lblOrigin.Text = oDict.GetWordWithColon("origin");
            lblColor.Text = oDict.GetWordWithColon("color");
            lblRemarks.Text = oDict.GetWordWithColon("remarks");
            boxPicture.Text = oDict.GetWord("product_picture");
            boxLogs.Text = oDict.GetWord("logs");
            lblCreatedOn.Text = oDict.GetWordWithColon("created_on");
            lblCreatedBy.Text = oDict.GetWordWithColon("created_by");
            lblModifiedOn.Text = oDict.GetWordWithColon("modified_on");
            lblModifiedBy.Text = oDict.GetWordWithColon("modified_by");
            tabSupplier.Text = oDict.GetWord("supplier");
            tabPackage.Text = oDict.GetWord("package");
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(txtDescription, "Numeric only");
            toolTip1.SetToolTip(cmdSmartSKU, "Next SKU");
            toolTip1.SetToolTip(cmdSmartProductCode, "Next Product Code");

            txtItemSKU.Validator = TextBoxValidation.IntegerValidator;
            txtItemSKU.MaxLength = Common.Config.MaxLength_SKU;
            txtItemCode.MaxLength = Common.Config.MaxLength_ProductCode;

            imgProduct.SizeMode = PictureBoxSizeMode.CenterImage;

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdSmartSKU.Visible = true;
                    cmdSmartProductCode.Visible = true;
                    txtItemSKU.Width = txtItemSKU.Width - 25;
                    txtItemCode.Width = txtItemCode.Width - 25;
                    txtItemSKU.BackColor = Color.White;
                    txtItemCode.BackColor = Color.White;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    txtItemSKU.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;
                    txtItemCode.BackColor = xPort5.Controls.Utility.TextBoxControl.ColorReadOnly;
                    break;
            }

            txtItemSKU.Enabled = this._EditMode == Common.Enums.EditMode.Add;
            txtItemCode.Enabled = this._EditMode == Common.Enums.EditMode.Add;
            tabArticle.Enabled = this._EditMode == Common.Enums.EditMode.Edit;
        }

        private void SetDropdowns()
        {
            cboOrigin.DataSource = null;
            cboOrigin.Items.Clear();
            T_Origin.LoadCombo(ref cboOrigin, "OriginName", false);

            cboColor.DataSource = null;
            xPort5.Controls.Utility.CheckedComboBox.LoadColor(cboColor);
            xPort5.Controls.Utility.Default.Origin(ref cboOrigin);

            cboCategory.DataSource = null;
            xPort5.Controls.Utility.ComboBox.LoadCategory(ref cboCategory);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.Buttons.Clear();

            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Factory", "Product");

            // cmdApprove
            ToolBarButton cmdApprove = new ToolBarButton("Approve", oDict.GetWord("approve"));
            cmdApprove.Tag = "Approve";
            cmdApprove.Image = new IconResourceHandle("16x16.approve.png");

            // cmdUpload
            ToolBarButton cmdUpload = new ToolBarButton("Upload", oDict.GetWord("product_picture"));
            cmdUpload.Tag = "Upload";
            cmdUpload.Image = new IconResourceHandle("16x16.pictureOn16.png");

            if (_EditMode != Common.Enums.EditMode.Read)
            {
                if (_EditMode != Common.Enums.EditMode.Add)
                {
                    if (xPort5.Common.Config.UseNetSqlAzMan)
                    {
                        if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Factory", "Product.Approve"))
                        {
                            this.ansToolbar.Buttons.Add(cmdApprove);
                        }
                    }
                    else
                    {
                        if (xPort5.Controls.Utility.Staff.IsAccessAllowed(Common.Enums.UserGroup.Senior))
                        {
                            this.ansToolbar.Buttons.Add(cmdApprove);
                        }
                    }
                    this.ansToolbar.Buttons.Add(sep);
                    this.ansToolbar.Buttons.Add(cmdUpload);
                }
            }

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
                    case "approve":
                        MessageBox.Show("Approve Item?", "Approve Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdApprove_Click));
                        break;
                    case "upload":
                        ImageManager imageMan = new ImageManager();
                        imageMan.ProductId = this._ProductId;
                        imageMan.Closed += new EventHandler(imageMan_Closed);
                        imageMan.ShowDialog();
                        break;
                }
            }
        }

        void imageMan_Closed(object sender, EventArgs e)
        {
            ImageManager imageMan = sender as ImageManager;
            if (imageMan != null)
            {
                imgProduct.ImageName = xPort5.Controls.Utility.Resources.PictureFilePath(_ProductId, xPort5.Controls.Utility.Product.KeyPicture(_ProductId));
            }
        }

        private void LoadChildControls()
        {
            Package.PackageList package = new Package.PackageList();
            package.Dock = DockStyle.Fill;
            package.EditMode = this.EditMode;
            package.ProductId = this.ProductId;

            this.tabPackage.Controls.Clear();
            this.tabPackage.Controls.Add(package);

            Supplier.SupplierList supplier = new Supplier.SupplierList();
            supplier.Dock = DockStyle.Fill;
            supplier.EditMode = _EditMode;
            supplier.ProductId = _ProductId;

            this.tabSupplier.Controls.Clear();
            this.tabSupplier.Controls.Add(supplier);
        }

        #region ShowProduct(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowProduct()
        {
            Article item = Article.Load(_ProductId);
            if (item != null)
            {
                txtItemSKU.Text = item.SKU;
                txtItemCode.Text = item.ArticleCode;
                txtDescription.Text = item.ArticleName;
                txtBarcode.Text = item.Barcode;
                txtRemarks.Text = item.Remarks;
                cboColor.Text = item.ColorPattern;

                xPort5.Controls.CheckedComboBoxView list = cboColor.GetCheckedComboView();

                #region Show Colors
                if (item.ColorPattern == String.Empty)
                {
                    // single color
                    T_Color oColor = T_Color.Load(item.ColorId);
                    if (oColor != null)
                    {
                        xPort5.Controls.CheckedComboBoxView colorList = cboColor.GetCheckedComboView();
                        for (int i = 0; i < colorList.Items.Count - 1; i++)
                        {
                            if (colorList.Items[i].ToString().Trim() == oColor.ColorName.Trim())
                            {
                                colorList.SetItemChecked(i, true);
                                colorList.SetSelected(i, true);
                            }
                        }
                    }
                }
                else
                {
                    // multi-color
                    string[] colors = item.ColorPattern.Split(';');
                    foreach (string color in colors)
                    {
                        xPort5.Controls.CheckedComboBoxView colorList = cboColor.GetCheckedComboView();
                        for (int i = 0; i < colorList.Items.Count - 1; i++)
                        {
                            if (colorList.Items[i].ToString().Trim() == color.Trim())
                            {
                                colorList.SetItemChecked(i, true);
                                colorList.SetSelected(i, true);
                            }
                        }
                    }
                }
                #endregion

                T_Origin oOrigin = T_Origin.Load(item.OriginId);
                if (oOrigin != null)
                {
                    cboOrigin.Text = oOrigin.OriginName;
                    cboOrigin.SelectedValue = oOrigin.OriginId;
                }
                T_Category oCategory = T_Category.Load(item.CategoryId);
                if (oCategory != null)
                {
                    cboCategory.Text = oCategory.CategoryName;
                    cboCategory.SelectedValue = oCategory.CategoryId;
                }
                imgProduct.ImageName = xPort5.Controls.Utility.Resources.PictureFilePath(_ProductId, xPort5.Controls.Utility.Product.KeyPicture(_ProductId));

                Staff s1 = Staff.Load(item.CreatedBy);
                Staff s2 = Staff.Load(item.ModifiedBy);
                txtCreatedOn.Text = item.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                txtCreatedBy.Text = s1.Alias;
                txtModifiedOn.Text = item.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
                txtModifiedBy.Text = s2.Alias;
                boxLogs.Visible = true;

                _ProductCode = item.ArticleCode;
                _ProductId = item.ArticleId;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    Article item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new Article();
                            item.ArticleId = _ProductId;
                            item.SKU = txtItemSKU.Text.Trim();
                            item.ArticleCode = txtItemCode.Text.Trim();
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = Common.Config.CurrentUserId;
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            item.Retired = false;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = Article.Load(_ProductId);
                            item.ModifiedOn = DateTime.Now;
                            item.ModifiedBy = Common.Config.CurrentUserId;
                            break;
                    }
                    item.ArticleName = txtDescription.Text.Trim();
                    item.ArticleName_Chs = txtDescription.Text.Trim();
                    item.ArticleName_Cht = txtDescription.Text.Trim();
                    item.Barcode = txtBarcode.Text.Trim();
                    item.Remarks = txtRemarks.Text.Trim();
                    item.CategoryId = (Guid)cboCategory.SelectedValue;
                    item.ColorPattern = cboColor.Text;
                    item.OriginId = (Guid)cboOrigin.SelectedValue;

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.ArticleId != _ProductId)
                    {
                        _ProductCode = item.ArticleCode;
                        _ProductId = item.ArticleId;
                    }

                    if (_Dup_ProductId != System.Guid.Empty)
                    {
                        xPort5.Controls.Utility.Resources.RenamePicture(_Dup_ProductId, item.ArticleId);

                        string sql = "ArticleId = '" + _Dup_ProductId.ToString() + "'";

                        DuplicatePackage(sql, item.ArticleId);
                        DuplicateSuppier(sql, item.ArticleId);
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    MessageBox.Show(ex.Message, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return result;
        }

        private bool VerifyItem()
        {
            bool result = true;
            string errMsg = String.Empty;

            if (_EditMode == Common.Enums.EditMode.Add)
            {
                #region validate SKU
                if (String.IsNullOrEmpty(txtItemSKU.Text))
                {
                    errMsg += Environment.NewLine + "SKU cannot be blank.";
                    result = false;
                }
                else
                {
                    Article product = Article.LoadWhere(String.Format("SKU = '{0}'", txtItemSKU.Text.Trim()));
                    if (product != null)
                    {
                        errMsg += Environment.NewLine + "SKU is in use.";
                        result = false;
                    }
                }
                #endregion

                #region validate Article Code
                if (String.IsNullOrEmpty(txtItemCode.Text))
                {
                    errMsg += Environment.NewLine + "SKU cannot be blank.";
                    result = false;
                }
                //else
                //{
                //    Article product = Article.LoadWhere(String.Format("ArticleCode = '{0}'", txtItemCode.Text.Trim()));
                //    if (product != null)
                //    {
                //        errMsg += Environment.NewLine + "Product Code is in use.";
                //        result = false;
                //    }
                //}
                #endregion
            }

            #region validate Color
            if (cboColor.Text == String.Empty)
            {
                errMsg += Environment.NewLine + "Color cannot be blank.";
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
            return xPort5.Controls.Utility.Product.DeleteRec(_ProductId);
        }

        private bool ApproveItem()
        {
            return xPort5.Controls.Utility.Product.ApproveRec(_ProductId);
        }

        private void DuplicatePackage(string sql, Guid newProductId)
        {
            ArticlePackageCollection packageList = ArticlePackage.LoadCollection(sql);
            foreach (ArticlePackage package in packageList)
            {
                ArticlePackage newPackage = new ArticlePackage();
                newPackage.ArticleId = newProductId;
                newPackage.PackageId = package.PackageId;
                newPackage.DefaultRec = package.DefaultRec;
                newPackage.UomId = package.UomId;
                newPackage.InnerBox = package.InnerBox;
                newPackage.OuterBox = package.OuterBox;
                newPackage.CUFT = package.CUFT;
                newPackage.SizeHeight_cm = package.SizeHeight_cm;
                newPackage.SizeHeight_in = package.SizeHeight_in;
                newPackage.SizeWidth_cm = package.SizeWidth_cm;
                newPackage.SizeWidth_in = package.SizeWidth_in;
                newPackage.SizeLength_cm = package.SizeLength_cm;
                newPackage.SizeLength_in = package.SizeLength_in;
                newPackage.WeightGross_kg = package.WeightGross_kg;
                newPackage.WeightGross_lb = package.WeightGross_lb;
                newPackage.WeightNet_kg = package.WeightNet_kg;
                newPackage.WeightNet_lb = package.WeightNet_lb;
                newPackage.ContainerQty = package.ContainerQty;
                newPackage.ContainerSize = package.ContainerSize;
                newPackage.CreatedBy = package.CreatedBy;
                newPackage.CreatedOn = package.CreatedOn;
                newPackage.ModifiedBy = package.ModifiedBy;
                newPackage.ModifiedOn = package.ModifiedOn;
                newPackage.Retired = package.Retired;
                newPackage.RetiredBy = package.RetiredBy;
                newPackage.RetiredOn = package.RetiredOn;

                newPackage.Save();
            }
        }

        private void DuplicateSuppier(string sql, Guid newProductId)
        {
            ArticleSupplierCollection supplierList = ArticleSupplier.LoadCollection(sql);
            foreach (ArticleSupplier supplier in supplierList)
            {
                ArticleSupplier newSupplier = new ArticleSupplier();
                newSupplier.ArticleId = newProductId;
                newSupplier.SupplierId = supplier.SupplierId;
                newSupplier.DefaultRec = supplier.DefaultRec;
                newSupplier.SuppRef = supplier.SuppRef;
                newSupplier.CurrencyId = supplier.CurrencyId;
                newSupplier.FCLCost = supplier.FCLCost;
                newSupplier.LCLCost = supplier.LCLCost;
                newSupplier.UnitCost = supplier.UnitCost;
                newSupplier.Notes = supplier.Notes;
                newSupplier.CreatedBy = supplier.CreatedBy;
                newSupplier.CreatedOn = supplier.CreatedOn;
                newSupplier.ModifiedBy = supplier.ModifiedBy;
                newSupplier.ModifiedOn = supplier.ModifiedOn;
                newSupplier.Retired = supplier.Retired;
                newSupplier.RetiredBy = supplier.RetiredBy;
                newSupplier.RetiredOn = supplier.RetiredOn;

                newSupplier.Save();
            }
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
                        MessageBox.Show(String.Format("Product Code {0} is saved!", _ProductCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            //cmdSmartSKU.Visible = false;
                            //cmdSmartProductCode.Visible = false;
                            //txtItemSKU.Width = txtItemSKU.Width + 25;
                            //txtItemCode.Width = txtItemCode.Width + 25;
                            //tabArticle.Enabled = true;
                            //imgProduct.ImageName = xPort5.Controls.Utility.Resources.PictureFilePath(_ProductId, xPort5.Controls.Utility.Product.KeyPicture(_ProductId));
                            //this.Update();
                            this.Init();
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
                        MessageBox.Show(String.Format("Item Code {0} is saved!", _ProductCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Product Code {0} is saved!", _ProductCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _Dup_ProductId = this.ProductId;
                            _EditMode = Common.Enums.EditMode.Add;
                            _ProductId = System.Guid.NewGuid();

                            this.SetAnsToolbar();
                            cmdSmartSKU.Visible = true;
                            cmdSmartProductCode.Visible = true;
                            txtItemSKU.Width = txtItemSKU.Width - 25;
                            txtItemCode.Width = txtItemCode.Width - 25;
                            txtItemSKU.Enabled = true;
                            txtItemCode.Enabled = true;
                            tabArticle.Enabled = false;
                            imgProduct.ImageName = xPort5.Controls.Utility.Resources.PictureFilePath(_ProductId, xPort5.Controls.Utility.Product.KeyPicture(_ProductId));
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
                        MessageBox.Show(String.Format("Product Code {0} is saved!", _ProductCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _ProductId = System.Guid.NewGuid();

                            this.SetAnsToolbar();
                            cmdSmartSKU.Visible = true;
                            cmdSmartProductCode.Visible = true;
                            txtItemSKU.Width = txtItemSKU.Width - 25;
                            txtItemCode.Width = txtItemCode.Width - 25;
                            txtItemSKU.Enabled = true;
                            txtItemCode.Enabled = true;
                            tabArticle.Enabled = false;
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
                        MessageBox.Show(String.Format("Item Code {0} is deleted.", _ProductCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdApprove_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (ApproveItem())
                    {
                        MessageBox.Show(String.Format("Item Code {0} is approved.", _ProductCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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

        private void cmdSmartSKU_Click(object sender, EventArgs e)
        {
            txtItemSKU.Text = xPort5.Controls.Utility.Product.NextSKU();
        }

        private void cmdSmartProductCode_Click(object sender, EventArgs e)
        {
            txtItemCode.Text = xPort5.Controls.Utility.Product.NextProductCode();
        }
    }
}
