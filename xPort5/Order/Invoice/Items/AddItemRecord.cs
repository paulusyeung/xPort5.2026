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
using System.Data.SqlClient;

#endregion

namespace xPort5.Order.Invoice.Items
{
    public partial class AddItemRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _OrderINId = System.Guid.Empty;
        private Guid _OrderINItemId = System.Guid.Empty;
        private DataTable shipmentTable = new DataTable();

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

        public AddItemRecord()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.SetAttributes();
            this.PrepareShipmentTable();
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", oDict.GetWord("add"), string.Format(oDict.GetWord("record"), oDict.GetWord("item")));

            this.lblSCNumber.Text = oDict.GetWordWithColon("sales_contract_num");
            this.btnOK.Text = oDict.GetWord("ok");
            this.btnClear.Text = oDict.GetWord("clear");

            this.colArticleCode.HeaderText = oDict.GetWord("product_code");
            this.colColor.HeaderText = oDict.GetWord("color");
            this.colCustRef.HeaderText = oDict.GetWord("customer_ref");
            this.colOutstandingQty.HeaderText = oDict.GetWord("outstanding_qty");
            this.colInvoicedQty.HeaderText = oDict.GetWord("invoiced_qty");

            this.colScheduledDate.HeaderText = oDict.GetWord("schedule_date");
            this.colOSQty.HeaderText = oDict.GetWord("outstanding_qty");
            this.colThisShipment.HeaderText = oDict.GetWord("this_shipment");

            this.lblShipmentSchedule.Text = oDict.GetWordWithColon("shipment_schedule");
        }

        #region Bind List

        private void PrepareShipmentTable()
        {
            shipmentTable.Clear();

            shipmentTable.Columns.Add("OrderSCItemId", typeof(Guid));
            shipmentTable.Columns.Add("LineNumber", typeof(int));
            shipmentTable.Columns.Add("QtyShipped", typeof(decimal));
        }

        private void BindList()
        {
            if (txtSCNumber.Text.Trim().Length > 0)
            {
                string sql = @"
SELECT [OrderSCItemsId]
      ,[SCLineNo]
      ,[ArticleCode]
      ,[OSQty]
      ,[InvoicedQty]
      ,[QTNumber]
      ,[QTLineNo]
      ,[SCNumber]
      ,[ArticleId]
      ,[Color]
      ,[CustRef]
  FROM [dbo].[vwOrderINQTList]";

                sql += string.Format(" WHERE [SCNumber] = '{0}' OR [QTNumber] = '{1}'", txtSCNumber.Text.Trim(), txtSCNumber.Text.Trim());
                sql += " ORDER BY [SCLineNo], [ArticleCode] ";

                DataSet dsQTList = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql);

                dgvQTList.AutoGenerateColumns = false;
                dgvQTList.DataSource = dsQTList.Tables[0];
            }
        }

        #endregion

        #region Save Item

        private bool SaveItem()
        {
            if (VerifyItem())
            {
                foreach (DataGridViewRow row in dgvQTList.SelectedRows)
                {
                    if (Common.Utility.IsGUID(row.Cells[colOrderQTItemId.Index].Value.ToString()))
                    {
                        int invoicedQty = 0;
                        int.TryParse(row.Cells[colInvoicedQty.Index].Value.ToString(), out invoicedQty);

                        this.OrderINItemId = SaveINItem(new Guid(row.Cells[0].Value.ToString()), (decimal)invoicedQty);

                        SaveShippingInfo(new Guid(row.Cells[colOrderQTItemId.Index].Value.ToString()), this.OrderINItemId);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private Guid SaveINItem(Guid orderSCItemId, decimal invoicedQty)
        {
            OrderINItems inItem = new OrderINItems();
            inItem.OrderINId = this.OrderINId;
            inItem.LineNumber = this.LineNumber;
            inItem.OrderSCItemsId = orderSCItemId;
            inItem.Qty = invoicedQty;
            inItem.Save();

            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, inItem.ToString());

            OrderSCItems scItem = OrderSCItems.Load(orderSCItemId);
            if (scItem != null)
            {
                scItem.QtyOUT += invoicedQty;
                scItem.Save();

                xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, scItem.ToString());
            }

            return inItem.OrderINItemsId;
        }

        private void SaveShippingInfo(Guid orderSCItemId, Guid orderINItemId)
        {
            DataRow[] selectedRows = shipmentTable.Select("OrderSCItemId = '" + orderSCItemId.ToString() + "'");
            for (int i = 0; i < selectedRows.Length; i++)
            {
                OrderINShipment inShipment = new OrderINShipment();
                inShipment.OrderINItemsId = orderINItemId;
                inShipment.ShipmentID = 0;
                inShipment.Qty = Convert.ToDecimal(selectedRows[i]["QtyShipped"].ToString());

                inShipment.Save();
            }
        }

        private bool VerifyItem()
        {
            bool result = true;
            string Msg = string.Empty;
            bool bAnyInput = false;
            decimal dblShippedQty;

            if (string.IsNullOrEmpty(txtSCNumber.Text.Trim()))
            {
                Msg += "The Sales Contract No. should no be blank, you must correct it.";
                Msg += Environment.NewLine;

                result = false;
            }

            if (dgvQTList.Rows.Count == 0)
            {
                Msg += "There is no selected item, operation must be aborted.";
                Msg += Environment.NewLine;

                result = false;
            }

            foreach (DataGridViewRow row in dgvQTList.SelectedRows)
            {
                decimal osQty = 0;
                int invoicedQty = 0;
                decimal.TryParse(row.Cells[colOutstandingQty.Index].Value.ToString(), out osQty);
                int.TryParse(row.Cells[colInvoicedQty.Index].Value.ToString(), out invoicedQty);

                if (invoicedQty > 0)
                {
                    bAnyInput = true;
                    if (osQty < (decimal)invoicedQty)
                    {
                        Msg += "Selected line number " + row.Cells[1].Value.ToString() + " has an Invoiced Qty great than O/S Qty.";
                        Msg += Environment.NewLine;

                        result = false;
                    }
                    else
                    {
                        dblShippedQty = 0;

                        DataRow[] selectedRows = shipmentTable.Select("OrderSCItemId = '" + row.Cells[0].Value.ToString() + "'");
                        for (int i = 0; i < selectedRows.Length; i++)
                        {
                            dblShippedQty += Convert.ToDecimal(selectedRows[i]["QtyShipped"].ToString());
                        }

                        if (dblShippedQty.CompareTo((decimal)invoicedQty) != 0)
                        {
                            Msg += "Selected line number " + row.Cells[1].Value.ToString() + " does not has a proper Shipment Qty." + Environment.NewLine +
                                  invoicedQty.ToString() + " vs " + dblShippedQty.ToString() + ".";
                            Msg += Environment.NewLine;

                            result = false;
                        }
                    }
                }
            }

            if (!bAnyInput)
            {
                Msg = "There is no Invoiced item," + Environment.NewLine +
                      "please check your entries.";

                MessageBox.Show(Msg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!result)
            {
                MessageBox.Show(Msg + Environment.NewLine + "Operation must be aborted.", "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindList();
        }

        private void txtSCNumber_TextChanged(object sender, EventArgs e)
        {
            BindList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!this.Text.Contains("ReadOnly"))
            {
                if (SaveItem())
                {
                    MessageBox.Show("Shipping info is saved!", "Save Result");

                    this.Close();
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSCNumber.Clear();
            dgvQTList.DataSource = null;
            dgvShipmentList.DataSource = null;
        }

        private void dgvQTList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQTList.SelectedRows.Count > 0 && Common.Utility.IsGUID(dgvQTList.SelectedRows[0].Cells[0].Value.ToString()))
            {
                string sql = @"
SELECT [ShippedOn]
      ,[OSQty]
      ,[ThisShipment]
 FROM [vwOrderINShipmentList] 
WHERE OrderSCItemsId = '" + dgvQTList.SelectedRows[0].Cells[colOrderQTItemId.Index].Value.ToString() + "'";
                sql += " ORDER BY ShippedOn";

                DataSet ds = SqlHelper.Default.ExecuteDataSet(CommandType.Text, sql);

                dgvShipmentList.AutoGenerateColumns = false;
                dgvShipmentList.DataSource = ds.Tables[0];

                DataRow[] existedRows = shipmentTable.Select("OrderSCItemId = '" + dgvQTList.SelectedRows[0].Cells[colOrderQTItemId.Index].Value.ToString() + "'");
                for (int i = 0; i < existedRows.Length; i++)
                {
                    dgvShipmentList.Rows[i].Cells[colThisShipment.Index].Value = existedRows[i]["QtyShipped"];
                }
            }
        }

        private void dgvShipmentList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvQTList.SelectedRows.Count > 0)
            {
                DataRow[] existedRows = shipmentTable.Select("OrderSCItemId = '" + dgvQTList.SelectedRows[0].Cells[0].Value.ToString() + "'");
                if (existedRows.Length == 0)
                {
                    DataRow newRow = shipmentTable.NewRow();
                    newRow["OrderSCItemId"] = dgvQTList.SelectedRows[0].Cells[0].Value;
                    newRow["LineNumber"] = dgvQTList.SelectedRows[0].Cells[1].Value;
                    newRow["QtyShipped"] = dgvShipmentList.Rows[e.RowIndex].Cells[2].Value;

                    shipmentTable.Rows.Add(newRow);
                }
                else
                {
                    for (int i = 0; i < shipmentTable.Rows.Count; i++)
                    {
                        if (shipmentTable.Rows[i]["OrderSCItemId"].Equals(dgvQTList.SelectedRows[0].Cells[0].Value))
                        {
                            shipmentTable.Rows[i].BeginEdit();

                            shipmentTable.Rows[i]["QtyShipped"] = dgvShipmentList.Rows[e.RowIndex].Cells[2].Value;

                            shipmentTable.Rows[i].EndEdit();
                            shipmentTable.AcceptChanges();
                        }
                    }
                }
            }
        }
    }
}
