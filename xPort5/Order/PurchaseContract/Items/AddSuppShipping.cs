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

#endregion

namespace xPort5.Order.PurchaseContract.Items
{
    public partial class AddSuppShipping : Form
    {
        private string qty;
        public string Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public bool IsOkay { get; set; }

        public AddSuppShipping()
        {
            InitializeComponent();
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.Text = string.Format(oDict.GetWord("supplier")+oDict.GetWord("shipment_schedule"));
            this.lblSDate.Text = oDict.GetWord("date");
            this.lblSQty.Text = oDict.GetWord("Qty");

            this.txtSQty.Validator = TextBoxValidation.FloatValidator;

            if (xPort5.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PurchaseContract.Create") ||
                    xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PurchaseContract.Update"))
                {
                    this.btnOK.Visible = true;
                }
                else
                {
                    this.btnOK.Visible = false;
                }
            }
        }

        private void AddSuppShipping_Load(object sender, EventArgs e)
        {
            this.txtSQty.Text = qty;
            this.dtpDate.Value = Convert.ToDateTime(date);

            SetAttributes();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.Qty = this.txtSQty.Text;
            this.Date = this.dtpDate.Value.ToString("yyyy-MM-dd");

            base.OnClosed(e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtSQty.Text.Trim() != "")
            {
                this.IsOkay = true;
                this.Close();
            }
        }
    }
}
