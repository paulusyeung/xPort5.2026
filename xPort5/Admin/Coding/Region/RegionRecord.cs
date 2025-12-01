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

namespace xPort5.Admin.Coding.Region
{
    public partial class RegionRecord : Form
    {
        nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _RegionId = System.Guid.Empty;
        private string _RegionCode = String.Empty;

        public RegionRecord()
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
        public Guid RegionId
        {
            get
            {
                return _RegionId;
            }
            set
            {
                _RegionId = value;
            }
        }
        #endregion

        #region Configure Controls on Form Load
        private void SetCaptions()
        {
            this.Text = string.Format("{0} {1}", string.Format(oDict.GetWord("record"), oDict.GetWord("region")), oDict.GetWordWithSquareBracket(this._EditMode.ToString() + " Mode"));

            lblCode.Text = string.Format(oDict.GetWordWithColon("code_replace"), oDict.GetWord("region"));
            lblName.Text = string.Format(oDict.GetWordWithColon("name4others_replace"), oDict.GetWord("region"));
            lblName_Chs.Text = string.Format(oDict.GetWordWithColon("name_chs_replace"), oDict.GetWord("region"));
            lblName_Cht.Text = string.Format(oDict.GetWordWithColon("name_cht_replace"), oDict.GetWord("region"));
        }

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

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, _EditMode, "Admin", "Coding.Region");

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
            T_Region item = T_Region.Load(_RegionId);
            if (item != null)
            {
                txtCode.Text = item.RegionCode;
                txtName.Text = item.RegionName;
                txtName_Chs.Text = item.RegionName_Chs;
                txtName_Cht.Text = item.RegionName_Cht;
            }
        }

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                try
                {
                    T_Region item = null;
                    switch ((int)_EditMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            item = new T_Region();
                            item.RegionId = _RegionId;
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            item = T_Region.Load(_RegionId);
                            break;
                    }
                    item.RegionCode = txtCode.Text.Trim();
                    item.RegionName = txtName.Text.Trim();
                    item.RegionName_Chs = txtName_Chs.Text.Trim();
                    item.RegionName_Cht = txtName_Cht.Text.Trim();

                    item.Save();

                    #region log activity
                    if (_EditMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, item.ToString());
                    #endregion

                    if (item.RegionId != _RegionId)
                    {
                        _RegionId = item.RegionId;
                    }
                    _RegionCode = item.RegionCode;
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
                    sql = String.Format("RegionCode = '{0}'", txtCode.Text.Trim());
                    T_Group group = T_Group.LoadWhere(sql);
                    if (group != null)
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
                T_Region item = T_Region.Load(_RegionId);
                if (item != null)
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());

                T_Region.Delete(_RegionId);
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _RegionCode), "Save Result");
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _RegionCode), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                        MessageBox.Show(String.Format("Item {0} is saved!", _RegionCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _RegionId = System.Guid.NewGuid();
                            _RegionCode = string.Empty;

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
                        MessageBox.Show(String.Format("Item {0} is saved!", _RegionCode), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Edit)
                        {
                            _EditMode = Common.Enums.EditMode.Add;
                            _RegionId = System.Guid.NewGuid();

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
                        MessageBox.Show(String.Format("Item {0} is deleted.", _RegionCode), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
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
                    _RegionId = System.Guid.NewGuid();
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