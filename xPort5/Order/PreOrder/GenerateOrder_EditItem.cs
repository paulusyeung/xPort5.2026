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

#endregion

namespace xPort5.Order.PreOrder
{
    public partial class GenerateOrder_EditItem : Form
    {
        private String _SupplierCode = String.Empty;
        private String _SupplierName = String.Empty;
        private String _ContractNumber = String.Empty;

        #region public properties
        public String SupplierCode
        {
            get { return _SupplierCode; }
            set { _SupplierCode = value; }
        }
        public String SupplierName
        {
            get { return _SupplierName; }
            set { _SupplierName = value; }
        }
        public String ContractNumber
        {
            get { return _ContractNumber; }
            set { _ContractNumber = value; }
        }
        public bool IsOkay { get; set; }
        #endregion

        public GenerateOrder_EditItem()
        {
            InitializeComponent();
        }

        private void GenerateOrder_EditItem_Load(object sender, EventArgs e)
        {
            SetCaptions();
            SetAttributes();
            LoadData();
            txtContractNumber.Focus();
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.lblSupplierCode.Text = oDict.GetWordWithColon("supplier_code");
            this.lblSupplierName.Text = oDict.GetWordWithColon("supplier_name");
            this.lblContractNumber.Text = oDict.GetWordWithColon("contract_num");
        }

        private void SetAttributes()
        {
            xPort5.Controls.Utility.TextBoxControl.SetToReadOnly(ref txtSupplierCode);
            xPort5.Controls.Utility.TextBoxControl.SetToReadOnly(ref txtSupplierName);
        }

        private void LoadData()
        {
            txtSupplierCode.Text = _SupplierCode;
            txtSupplierName.Text = _SupplierName;
            txtContractNumber.Text = _ContractNumber;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            _ContractNumber = txtContractNumber.Text.Trim();
            this.IsOkay = true;
            this.Close();
        }
    }
}
