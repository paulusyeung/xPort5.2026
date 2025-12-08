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

namespace xPort5.Order.PreOrder
{
    public partial class GenerateOrder : Form
    {
        private Guid _OrderPLId = System.Guid.Empty;

        #region public properties

        public Guid OrderPLId
        {
            get
            {
                return _OrderPLId;
            }
            set
            {
                _OrderPLId = value;
            }
        }

        #endregion

        public GenerateOrder()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttributes();
            ShowItem();
            BindList();
        }

        #region Set Attributes

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} - {1}", oDict.GetWord("pre_order"), oDict.GetWord("generate_order"));
            this.lblSalesContractNumber.Text = oDict.GetWordWithColon("sales_contract_num");
            this.lblPurchaseContractNumber.Text = oDict.GetWordWithColon("purchase_contract_num");
            this.lblCustomerName.Text = oDict.GetWordWithColon("name");
            this.btnOK.Text = oDict.GetWord("ok");
            this.btnClear.Text = oDict.GetWord("clear");
            this.boxCustomerInfo.Text = oDict.GetWord("customer_info");
            this.boxSupplierInfo.Text = oDict.GetWord("supplier_info");

            this.colSupplierCode.Text = oDict.GetWord("supplier_code");
            this.colSupplierName.Text = oDict.GetWord("supplier_name");
            this.colContractNumber.Text = oDict.GetWord("contract_num");

            toolTip1.SetToolTip(cmdNextContractNumber, string.Format(oDict.GetWord("generate_number"), oDict.GetWord("contract_num")));
        }

        #endregion

        private void BindList()
        {
            this.lvwSupplierList.Items.Clear();

            // Refactored to use ViewService
            string whereClause = string.Format("[OrderPLId] = '{0}'", _OrderPLId.ToString());
            DataSet ds = ViewService.Default.GetPreOrderItemList(whereClause, "");
            DataTable dt = ds.Tables[0];

            // Perform Distinct selection for Supplier info in memory
            var suppliers = dt.AsEnumerable()
                .Select(row => new
                {
                    SupplierId = row["SupplierId"] != DBNull.Value ? (Guid)row["SupplierId"] : Guid.Empty,
                    SupplierCode = row["SupplierCode"] != DBNull.Value ? row["SupplierCode"].ToString() : string.Empty,
                    SupplierName = row["SupplierName"] != DBNull.Value ? row["SupplierName"].ToString() : string.Empty
                })
                .Where(x => x.SupplierId != Guid.Empty)
                .Distinct()
                .OrderBy(x => x.SupplierCode)
                .ToList();

            foreach (var supp in suppliers)
            {
                ListViewItem objItem = this.lvwSupplierList.Items.Add(supp.SupplierId.ToString());  // Id
                objItem.SubItems.Add(supp.SupplierCode);             // code
                objItem.SubItems.Add(supp.SupplierName);             // name
                objItem.SubItems.Add(string.Empty);
            }
        }

        private void ShowItem()
        {
            xPort5.EF6.OrderPL order = xPort5.EF6.OrderPL.Load(this._OrderPLId);
            if (order != null)
            {
                txtCustomerName.Text = xPort5.Controls.Utility.Customer.GetCustomerName(order.CustomerId);
            }
        }

        private bool VerifyItem()
        {
            bool result = true;
            string errMsg = String.Empty;

            #region validate Contract Number
            if (txtSalesContractNumber.Text.Trim() == String.Empty)
            {
                errMsg += Environment.NewLine + "Contract No. cannot be blank.";
                result = false;
            }
            else
            {
                xPort5.EF6.OrderSC orderPL = xPort5.EF6.OrderSC.LoadWhere(String.Format("SCNumber = '{0}'", txtSalesContractNumber.Text.Trim()));
                if (orderPL != null)
                {
                    errMsg += Environment.NewLine + "Contract No. is in use.";
                    result = false;
                }
            }
            #endregion

            if (!(result))
            {
                MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (VerifyItem())
            {
                Guid orderSCId = GenerateSalesContract();
                if (orderSCId != System.Guid.Empty)
                {
                    GenerateSalesContractItems(orderSCId);

                    GeneratePurchaseContract(orderSCId);

                    MessageBox.Show("Contracts are generated successfully", "Result", MessageBoxButtons.OK, new EventHandler(cmdCloseClick));
                }
            }
        }

        private void cmdCloseClick(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtSalesContractNumber.Clear();
            this.txtPurchaseContractNumber.Clear();
            this.BindList();
        }

        private void cmdNextContractNumber_Click(object sender, EventArgs e)
        {
            string contractNumber = xPort5.Controls.Utility.OrderSC.NextSCNumber();

            FillPONumber(contractNumber);

            txtSalesContractNumber.Text = contractNumber;
            txtPurchaseContractNumber.Text = contractNumber;
        }

        #region Generate Order (Fill PO Number, Sales Contract & Purchase Contract)

        #region Fill PO Number
        private void FillPONumber(string ContractNumber)
        {
            int iCount = (int)'A';

            if (lvwSupplierList.Items.Count == 1)
            {
                // 祇有一個 Supplier 就不用加 suffix
                lvwSupplierList.Items[0].SubItems[3].Text = ContractNumber;
            }
            else
            {
                // 多於一個 Supplier 就加 suffix (A, B, C ... Z)
                foreach (ListViewItem item in this.lvwSupplierList.Items)
                {
                    item.SubItems[3].Text = ContractNumber + ((char)iCount).ToString();

                    iCount++;
                }
            }
        }
        #endregion

        #region Sales Contract
        private System.Guid GenerateSalesContract()
        {
            xPort5.EF6.OrderPL srcOrder = xPort5.EF6.OrderPL.Load(this._OrderPLId);
            if (srcOrder != null)
            {
                xPort5.EF6.OrderSC destOrder = new OrderSC();
                destOrder.SCNumber = this.txtSalesContractNumber.Text.Trim();
                destOrder.SCDate = DateTime.Now;
                destOrder.CustomerId = srcOrder.CustomerId;
                destOrder.StaffId = Common.Config.CurrentUserId;
                destOrder.YourOrderNo = srcOrder.PLNumber;
                destOrder.YourRef = string.Empty;
                destOrder.Carrier = string.Empty;
                destOrder.SendFrom = srcOrder.SendFrom;
                destOrder.SendTo = srcOrder.SendTo;
                destOrder.Remarks = srcOrder.Remarks;
                destOrder.Status = (int)Common.Enums.Status.Active;
                destOrder.CreatedBy = Common.Config.CurrentUserId;
                destOrder.CreatedOn = DateTime.Now;
                destOrder.ModifiedBy = Common.Config.CurrentUserId;
                destOrder.ModifiedOn = DateTime.Now;
                destOrder.AccessedBy = Common.Config.CurrentUserId;
                destOrder.AccessedOn = DateTime.Now;
                destOrder.Retired = false;
                destOrder.Save();

                return destOrder.OrderSCId;
            }

            return System.Guid.Empty;
        }

        private void GenerateSalesContractItems(Guid contractId)
        {
            string sql = string.Format("OrderPLId = '{0}'", this._OrderPLId.ToString());
            string[] orderBy = new string[] { "LineNumber" };
            OrderPLItemsCollection srcItemList = OrderPLItems.LoadCollection(sql, orderBy, true);
            foreach (OrderPLItems srcItem in srcItemList)
            {
                xPort5.EF6.OrderSCItems destItem = new OrderSCItems();
                destItem.OrderSCId = contractId;
                destItem.LineNumber = srcItem.LineNumber;
                destItem.OrderQTItemId = srcItem.OrderQTItemId;
                destItem.QtyOrdered = 0;
                destItem.QtyIN = 0;
                destItem.QtyOUT = 0;
                destItem.Save();
            }
        }
        #endregion

        #region Purchase Contract
        private void GeneratePurchaseContract(Guid salesContractId)
        {
            foreach (ListViewItem item in lvwSupplierList.Items)
            {
                if (Common.Utility.IsGUID(item.Text))
                {
                    System.Guid supplierId = new Guid(item.Text);
                    string pcNumber = item.SubItems[3].Text;
                    xPort5.EF6.OrderSC srcOrder = xPort5.EF6.OrderSC.Load(salesContractId);
                    if (srcOrder != null)
                    {
                        xPort5.EF6.OrderPC destOrder = new OrderPC();
                        destOrder.PCNumber = pcNumber;
                        destOrder.PCDate = DateTime.Now;
                        destOrder.SupplierId = supplierId;
                        destOrder.StaffId = Common.Config.CurrentUserId;
                        destOrder.YourRef = string.Empty;
                        destOrder.Carrier = string.Empty;
                        destOrder.SendFrom = srcOrder.SendFrom;
                        destOrder.SendTo = srcOrder.SendTo;
                        destOrder.Remarks = srcOrder.Remarks;
                        destOrder.Status = (int)Common.Enums.Status.Active;
                        destOrder.CreatedBy = Common.Config.CurrentUserId;
                        destOrder.CreatedOn = DateTime.Now;
                        destOrder.ModifiedBy = Common.Config.CurrentUserId;
                        destOrder.ModifiedOn = DateTime.Now;
                        destOrder.AccessedBy = Common.Config.CurrentUserId;
                        destOrder.AccessedOn = DateTime.Now;
                        destOrder.Retired = false;
                        destOrder.Save();

                        GeneratePurchaseContractItems(destOrder.OrderPCId, salesContractId, supplierId);
                    }
                }
            }
        }

        private void GeneratePurchaseContractItems(Guid contractId, Guid salesContractId, Guid supplierId)
        {
            string whereClause = string.Format("[OrderSCId] = '{0}' AND [SupplierId] = '{1}'", salesContractId.ToString(), supplierId.ToString());
            string orderBy = "[LineNumber]";

            DataSet ds = ViewService.Default.GetSalesContractItemList(whereClause, orderBy);
            DataTable dt = ds.Tables[0];

            int iCount = 1;

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrderSCItemsId"] != DBNull.Value)
                {
                    Guid scItemId = (Guid)row["OrderSCItemsId"];
                    xPort5.EF6.OrderSCItems srcItem = OrderSCItems.Load(scItemId);
                    if (srcItem != null)
                    {
                        xPort5.EF6.OrderPCItems destItem = new OrderPCItems();
                        destItem.OrderPCId = contractId;
                        destItem.LineNumber = iCount;
                        destItem.OrderSCItemsId = srcItem.OrderSCItemsId;
                        destItem.Qty = 0;
                        destItem.QtyIN = 0;
                        destItem.QtyOUT = 0;
                        destItem.Save();

                        iCount++;
                    }
                }
            }
        }
        #endregion

        #endregion

        private void txtSalesContractNumber_Enter(object sender, EventArgs e)
        {
            if (!(txtSalesContractNumber.Text.Trim() == String.Empty))
            {
                txtPurchaseContractNumber.Text = txtSalesContractNumber.Text;
            }
        }

        private void cmdFillPONumber_Click(object sender, EventArgs e)
        {
            if (!(String.IsNullOrEmpty(txtPurchaseContractNumber.Text.Trim())))
            {
                FillPONumber(txtPurchaseContractNumber.Text);
            }
        }

        private void txtPurchaseContractNumber_Enter(object sender, EventArgs e)
        {
            if (!(txtPurchaseContractNumber.Text.Trim() == String.Empty))
            {
                FillPONumber(txtPurchaseContractNumber.Text);
            }
        }

        private void txtSalesContractNumber_TextChanged(object sender, EventArgs e)
        {
            if (!(txtSalesContractNumber.Text.Trim() == String.Empty))
            {
                txtPurchaseContractNumber.Text = txtSalesContractNumber.Text;
            }
        }

        private void txtPurchaseContractNumber_TextChanged(object sender, EventArgs e)
        {
            if (!(txtPurchaseContractNumber.Text.Trim() == String.Empty))
            {
                FillPONumber(txtPurchaseContractNumber.Text);
            }
        }

        private void lvwSupplierList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwSupplierList.SelectedItem != null)
            {
                System.Guid plId = new Guid(this.lvwSupplierList.SelectedItem.SubItems[0].Text);
                String supplierCode = this.lvwSupplierList.SelectedItem.SubItems[1].Text;
                String supplierName = this.lvwSupplierList.SelectedItem.SubItems[2].Text;
                String contractNumber = this.lvwSupplierList.SelectedItem.SubItems[3].Text;

                GenerateOrder_EditItem editItem = new GenerateOrder_EditItem();
                editItem.SupplierCode = supplierCode;
                editItem.SupplierName = supplierName;
                editItem.ContractNumber = contractNumber;
                editItem.ShowDialog();

                editItem.Closed += new EventHandler(EditItem_Closed);
            }
        }

        private void EditItem_Closed(object sender, EventArgs e)
        {
            if (sender is GenerateOrder_EditItem)
            {
                GenerateOrder_EditItem editItem = sender as GenerateOrder_EditItem;
                if (editItem != null)
                {
                    if (editItem.IsOkay)
                    {
                        this.lvwSupplierList.SelectedItem.SubItems[3].Text = editItem.ContractNumber;
                    }
                }
            }
        }
    }
}
