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

namespace xPort5.Admin.Coding.Category
{
    public partial class CategoryRecord : Form
    {
        nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _CategoryId = System.Guid.Empty;
        private string _CategoryCode = String.Empty;

        public CategoryRecord()
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
        public Guid CategoryId
        {
            get
            {
                return _CategoryId;
            }
            set
            {
                _CategoryId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("category")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            lblCode.Text = string.Format(oDict.GetWordWithColon("code_replace"), oDict.GetWord("category"));
            lblName.Text = string.Format(oDict.GetWordWithColon("name4others_replace"), oDict.GetWord("category"));
            lblName_Chs.Text = string.Format(oDict.GetWordWithColon("name_chs_replace"), oDict.GetWord("category"));
            lblName_Cht.Text = string.Format(oDict.GetWordWithColon("name_cht_replace"), oDict.GetWord("category"));
        }

        private void SetAttributes()
        {
            toolTip1.SetToolTip(cmdConvertToChs, "Convert to Chs");
            toolTip1.SetToolTip(cmdConvertToCht, "Convert to Cht");
            toolTip1.SetToolTip(cmdNextCategoryCode, "Next Category Code");

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    cmdNextCategoryCode.Visible = true;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    cmdNextCategoryCode.Visible = false;
                    break;
            }
        }

        private void SetDropdowns()
        {
            T_Dept.LoadCombo(ref cboDept, "DeptName", false);
            T_Class.LoadCombo(ref cboClass, "ClassName", false);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Admin", "Coding.Category");

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
            T_Category item = T_Category.Load(_CategoryId);
            if (item != null)
            {
                //cboDept.SelectedValue = item.DeptId;
                //cboClass.SelectedValue = item.ClassId;
                txtCode.Text = item.CategoryCode;
                txtName.Text = item.CategoryName;
                txtName_Chs.Text = item.CategoryName_Chs;
                txtName_Cht.Text = item.CategoryName_Cht;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    T_Category item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new T_Category();
                            item.CategoryId = _CategoryId;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = T_Category.Load(_CategoryId);
                            break;
                    }
                    //item.DeptId = (Guid)cboDept.SelectedValue;
                    //item.ClassId = (Guid)cboClass.SelectedValue;
                    item.CategoryCode = txtCode.Text.Trim();
                    item.CategoryName = txtName.Text.Trim();
                    item.CategoryName_Chs = txtName_Chs.Text.Trim();
                    item.CategoryName_Cht = txtName_Cht.Text.Trim();

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.CategoryId != _CategoryId)
                    {
                        _CategoryId = item.CategoryId;
                    }
                    _CategoryCode = item.CategoryCode;
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
                #region validate Code
                try
                {
                    sql = String.Format("CategoryCode = '{0}'", txtCode.Text.Trim());
                    T_Category item = T_Category.LoadWhere(sql);
                    if (item != null)
                    {
                        errMsg += Environment.NewLine + "Code is in use.";
                        result = false;
                    }
                }
                catch
                {
                    errMsg += Environment.NewLine + "Code is invalid.";
                    result = false;
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
            bool result = false;

            try
            {
                // log activity
                T_Category item = T_Category.Load(_CategoryId);
                if (item != null)
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());

                T_Category.Delete(_CategoryId);
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CategoryCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            cmdNextCategoryCode.Visible = false;

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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CategoryCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CategoryCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            cmdNextCategoryCode.Visible = true;
                            _CategoryId = System.Guid.NewGuid();
                            _CategoryCode = string.Empty;

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
                        MessageBox.Show(String.Format("Item {0} is saved!", _CategoryCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            cmdNextCategoryCode.Visible = true;
                            _CategoryId = System.Guid.NewGuid();

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
                        MessageBox.Show(String.Format("Item {0} is deleted.", _CategoryCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                    _CategoryId = System.Guid.NewGuid();
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

        private void cmdNextCategoryCode_Click(object sender, EventArgs e)
        {
            txtCode.Text = xPort5.Controls.Utility.Category.NextCategoryCode();
        }
    }
}