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

namespace xPort5.Order.PreOrder.Items
{
    public partial class AddCustShipping : Form
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

        public AddCustShipping()
        {
            InitializeComponent();
        }

        public bool IsKay { get; set; }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.Text = string.Format(oDict.GetWord("customer")+oDict.GetWord("shipment_schedule"));
            this.lblCDate.Text = oDict.GetWord("date");
            this.lblCQty.Text = oDict.GetWord("Qty");

            this.txtCQty.Validator = TextBoxValidation.FloatValidator;

            if (xPort5.Common.Config.UseNetSqlAzMan)
            {
                if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PreOrderList.Create") ||
                    xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PreOrderList.Update"))
                {
                    this.btnOK.Visible = true;
                }
                else
                {
                    this.btnOK.Visible = false;
                }
            }
        }

        private void AddCustShipping_Load(object sender, EventArgs e)
        {
            this.txtCQty.Text = qty;
            this.dtpDate.Value = Convert.ToDateTime(date);
            SetAttributes();
        }

        protected override void OnClosed(EventArgs e)
        {
            qty = this.txtCQty.Text;
            date = this.dtpDate.Value.ToString("yyyy-MM-dd");

            base.OnClosed(e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this.txtCQty.Text.Trim() != "")
            {
                IsKay = true;
                this.Close();
            }
        }
    }
}
