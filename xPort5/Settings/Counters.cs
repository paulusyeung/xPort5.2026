#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;

#endregion

namespace xPort5.Settings
{
    public partial class Counters : UserControl
    {
        public Counters()
        {
            InitializeComponent();
        }

        #region Set Attributes
        private void SetAttributes()
        {
            txtNextCutCode.Validator = TextBoxValidation.FloatValidator;
            txtNextSuppCode.Validator = TextBoxValidation.FloatValidator;
            txtNextUserCode.Validator = TextBoxValidation.FloatValidator;
            txtNextSKU.Validator = TextBoxValidation.FloatValidator;
            txtNextProdCode.Validator = TextBoxValidation.FloatValidator;

            txtNextQTNumber.Validator = TextBoxValidation.FloatValidator;
            txtNextPLNumber.Validator = TextBoxValidation.FloatValidator;
            txtNextSCNumber.Validator = TextBoxValidation.FloatValidator;
            txtNextPCNumber.Validator = TextBoxValidation.FloatValidator;
            txtNextINNumber.Validator = TextBoxValidation.FloatValidator;
            txtNextPKNumber.Validator = TextBoxValidation.FloatValidator;
            txtNextSPNumber.Validator = TextBoxValidation.FloatValidator;
        }

        private void SetTheme()
        {
            this.BackColor = Color.FromName("#ACC0E9");
        }
        #endregion

        #region Load SysInfo
        private void LoadSysInfo()
        {
            xPort5.Controls.Utility.SysInfo sys = xPort5.Controls.Utility.SysInfo.Load();

            txtOwnerName.Text = sys.OwnerName;
            txtFactoryGDocsAccount.Text = sys.FactoryGDocsAccount;
            txtFactoryGDocsPassword.Text = sys.FactoryGDocsPassword;
        }
        #endregion

        #region Load Labels
        private void LoadLabels()
        {
            LoadLabel();
        }

        private void LoadLabel()
        {
            X_CounterCollection counter = X_Counter.LoadCollection();
            if (counter.Count > 0)
            {
                this.txtNextCutCode.Text = counter[0].NextCustCode.ToString();
                this.txtNextSuppCode.Text = counter[0].NextSuppCode.ToString();
                this.txtNextUserCode.Text = counter[0].NextStaffCode.ToString();
                this.txtNextSKU.Text = counter[0].NextSKUCode.ToString();
                this.txtNextProdCode.Text = counter[0].NextArticleCode.ToString();
                this.txtNextQTNumber.Text = counter[0].NextQTNumber.ToString();
                this.txtNextPLNumber.Text = counter[0].NextPLNumber.ToString();
                this.txtNextSCNumber.Text = counter[0].NextSCNumber.ToString();
                this.txtNextPCNumber.Text = counter[0].NextPCNumber.ToString();
                this.txtNextINNumber.Text = counter[0].NextINNumber.ToString();
                this.txtNextPKNumber.Text = counter[0].NextPKNumber.ToString();
                this.txtNextSPNumber.Text = counter[0].NextSPNumber.ToString();
            }
        }
        #endregion

        #region Save Labels
        // Default, English
        private void SaveLabel()
        {
            X_CounterCollection counters = X_Counter.LoadCollection();
            if (counters.Count > 0)
            {
                X_Counter counter = counters[0];
                counter.NextCustCode = Convert.ToInt32(this.txtNextCutCode.Text.Trim());
                counter.NextSuppCode = Convert.ToInt32(this.txtNextSuppCode.Text.Trim());
                counter.NextStaffCode = Convert.ToInt32(this.txtNextUserCode.Text.Trim());
                counter.NextSKUCode = Convert.ToInt32(this.txtNextSKU.Text.Trim());
                counter.NextArticleCode = Convert.ToInt32(this.txtNextProdCode.Text.Trim());
                counter.NextQTNumber = Convert.ToInt32(this.txtNextQTNumber.Text.Trim());
                counter.NextPLNumber = Convert.ToInt32(this.txtNextPLNumber.Text.Trim());
                counter.NextSCNumber = Convert.ToInt32(this.txtNextSCNumber.Text.Trim());
                counter.NextPCNumber = Convert.ToInt32(this.txtNextPCNumber.Text.Trim());
                counter.NextINNumber = Convert.ToInt32(this.txtNextINNumber.Text.Trim());
                counter.NextPKNumber = Convert.ToInt32(this.txtNextPKNumber.Text.Trim());
                counter.NextSPNumber = Convert.ToInt32(this.txtNextSPNumber.Text.Trim());

                counter.Save();
            }
        }
        #endregion

        private void Counters_Load(object sender, EventArgs e)
        {
            SetAttributes();
            SetTheme();
            LoadLabels();
            LoadSysInfo();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            SaveLabel();
            MessageBox.Show("Counter saved!", "Message");
        }

        private void cmdSaveSysInfo_Click(object sender, EventArgs e)
        {
            xPort5.Controls.Utility.SysInfo sys = xPort5.Controls.Utility.SysInfo.Load();
            sys.OwnerName = txtOwnerName.Text.Trim();
            sys.FactoryGDocsAccount = txtFactoryGDocsAccount.Text.Trim();
            sys.FactoryGDocsPassword = txtFactoryGDocsPassword.Text.Trim();
            sys.Save();

            MessageBox.Show("SysInfo saved!", "Message");
        }
    }
}