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

namespace xPort5.Admin.Coding.AgeGrading
{
    public partial class AgeGradingRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _AgeGradingId = System.Guid.Empty;
        private string _AgeGradingCode = String.Empty;

        public AgeGradingRecord()
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
        public Guid AgeGradingId
        {
            get
            {
                return _AgeGradingId;
            }
            set
            {
                _AgeGradingId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetAttributes()
        {
            toolTip1.SetToolTip(cmdConvertToChs, "Convert to Chs");
            toolTip1.SetToolTip(cmdConvertToCht, "Convert to Cht");

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
//            T_Currency.LoadCombo(ref cboCurrency, "CurrencyCode", false);
        }

        private void SetAnsToolbar()
        {
            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            // cmdSave
            ToolBarButton cmdSave = new ToolBarButton("Save", "Save");
            cmdSave.Tag = "Save";
            cmdSave.Image = new IconResourceHandle("16x16.16_L_save.gif");

            // cmdSaveClose
            ToolBarButton cmdSaveClose = new ToolBarButton("Save & Close", "Save & Close");
            cmdSaveClose.Tag = "Save & Close";
            cmdSaveClose.Image = new IconResourceHandle("16x16.16_saveClose.gif");

            // cmdSaveDup
            ToolBarButton cmdSaveDup = new ToolBarButton("Save & Dup", "Save & Dup");
            cmdSaveDup.Tag = "Save & Dup";
            cmdSaveDup.Image = new IconResourceHandle("16x16.16_L_saveDup.gif");

            // cmdSaveNew
            ToolBarButton cmdSaveNew = new ToolBarButton("Save & Dup", "Save & New");
            cmdSaveNew.Tag = "Save & New";
            cmdSaveNew.Image = new IconResourceHandle("16x16.16_L_saveOpen.gif");

            // cmdDelete
            ToolBarButton cmdDelete = new ToolBarButton("Delete", "Delete");
            cmdDelete.Tag = "Delete";
            cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            if (_EditMode != Common.Enums.EditMode.Read)
            {
                this.ansToolbar.Buttons.Add(cmdSave);
                this.ansToolbar.Buttons.Add(cmdSaveClose);
                this.ansToolbar.Buttons.Add(cmdSaveDup);
// reserved
//                this.ansToolbar.Buttons.Add(cmdSaveNew);
                if (_EditMode != Common.Enums.EditMode.Add)
                {
                    this.ansToolbar.Buttons.Add(cmdDelete);
                }
            }

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
            T_AgeGrading item = T_AgeGrading.Load(_AgeGradingId);
            if (item != null)
            {
                txtCode.Text = item.AgeGradingCode;
                txtName.Text = item.AgeGradingName;
                txtName_Chs.Text = item.AgeGradingName_Chs;
                txtName_Cht.Text = item.AgeGradingName_Cht;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    T_AgeGrading item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new T_AgeGrading();
                            item.AgeGradingId = _AgeGradingId;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = T_AgeGrading.Load(_AgeGradingId);
                            break;
                    }
                    item.AgeGradingCode = txtCode.Text.Trim();
                    item.AgeGradingName = txtName.Text.Trim();
                    item.AgeGradingName_Chs = txtName_Chs.Text.Trim();
                    item.AgeGradingName_Cht = txtName_Cht.Text.Trim();

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.AgeGradingId != _AgeGradingId)
                    {
                        _AgeGradingId = item.AgeGradingId;
                    }
                    _AgeGradingCode = item.AgeGradingCode;
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
                    sql = String.Format("AgeGradingCode = '{0}'", txtCode.Text.Trim());
                    T_AgeGrading item = T_AgeGrading.LoadWhere(sql);
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
                T_AgeGrading item = T_AgeGrading.Load(_AgeGradingId);
                if (item != null)
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());

                T_AgeGrading.Delete(_AgeGradingId);
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _AgeGradingCode), "Save Result");
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _AgeGradingCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _AgeGradingCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _AgeGradingId = System.Guid.NewGuid();
                            _AgeGradingCode = string.Empty;

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
                        MessageBox.Show(String.Format("Item {0} is saved!", _AgeGradingCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _AgeGradingId = System.Guid.NewGuid();

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
                        MessageBox.Show(String.Format("Item {0} is deleted.", _AgeGradingCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
            SetDropdowns();

            switch ((int)_EditMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    _AgeGradingId = System.Guid.NewGuid();
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
    }
}