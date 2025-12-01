#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using System.Data.SqlClient;

using xPort5.EF6;
using xPort5.Common;

#endregion

namespace xPort5.Order.Analysis
{
    public partial class ShipmentHistory2 : UserControl
    {
        string baseSqlSelect = string.Empty;
        string baseSqlWhere = string.Empty;
        string baseSqlOrder = string.Empty;
        string curSqlWhere = string.Empty;
        string id = string.Empty;

        public ShipmentHistory2()
        {
            InitializeComponent();
            SetLvwList();
            LoadTreeViewList();
            SetCaptions();
            SetAttributes();
        }

        private void SetLvwList()
        {
            this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.Dock = DockStyle.Fill;
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.tpName.Text = string.Format(oDict.GetWord("customer") + "/" + oDict.GetWord("supplier"));
            this.tpSearch.Text = oDict.GetWord("search");
            this.gbDate.Text = oDict.GetWord("select_date_range");
            this.gbByCode.Text = oDict.GetWord("select_by");
            this.lblDateFrom.Text = oDict.GetWord("from");
            this.lblDateTo.Text = oDict.GetWord("to");
            this.lblFromCode.Text = oDict.GetWord("from");
            this.lblToCode.Text = oDict.GetWord("to");
            this.rbtnArticleCode.Text = oDict.GetWord("product_code");
            this.rbtnCustRef.Text = oDict.GetWord("customer_ref_code");
            this.rbtnSuppRef.Text = oDict.GetWord("supplier_ref_code");
            this.rbtnByCustomer.Text = oDict.GetWord("customer");
            this.rbtnBySupplier.Text = oDict.GetWord("supplier");
            this.btnSearch.Text = oDict.GetWord("search");

            this.colArticleCode.Text = oDict.GetWord("product_code");
            this.colSupplierCode.Text = oDict.GetWord("supplier_code");
            this.colSupplierName.Text = oDict.GetWord("supplier_name");
            this.colSupplierRef.Text = oDict.GetWord("supplier_ref");
            this.colPackageCode.Text = oDict.GetWord("package");
            this.colCustomerName.Text = oDict.GetWord("customer_name");
            this.colCustomerRef.Text = oDict.GetWord("customer_ref");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colOSQty.Text = oDict.GetWord("osqty");
            this.colSalesContract.Text = oDict.GetWord("Sales_Contract");
            this.colScheduledDate.Text = oDict.GetWord("scheduled_date");
            this.colScheduledQty.Text = oDict.GetWord("scheduled_qty");
            this.colShippedQty.Text = oDict.GetWord("shipped_qty");
        }

        private void SetAttributes()
        {
            toolBar1.Height = 24;
            toolBar1.TextAlign = ToolBarTextAlign.Right;

            toolBar2.Height = 24;
            toolBar2.TextAlign = ToolBarTextAlign.Right;
        }

        /// <summary>
        /// Load Date to TreeView
        /// </summary>
        private void LoadTreeViewList()
        {
            xPort5.Controls.Utility.TreeViewControl.Load<Customer>(this.tvList.Nodes);
        }

        private string BindSql()
        {
            string sql = string.Empty;

            sql = @"SELECT OrderQTItemId, ArticleCode, SupplierCode ,PackageCode ,CustRef, SuppRef, CustName,
                                     SuppName, SCNumber, ScheduledDate, ScheduledQty, Unit, ShippedQty, OSQty, SKU 
                             FROM vwShipmentHistory 
                             WHERE CONVERT(NVARCHAR(10),ScheduledDate,126) >= '" + this.dtpDateFrom.Value.ToString("yyyy-MM-dd") + @"' AND 
                                    CONVERT(NVARCHAR(10),ScheduledDate,126) <= '" + this.dtpDateTo.Value.ToString("yyyy-MM-dd") + "'";

            if (rbtnArticleCode.Checked)
            {
                sql += " AND ArticleCode BETWEEN '" + txtFromCode.Text.Trim() + "' AND '" + txtToCode.Text.Trim() + "'";
            }

            if (rbtnCustRef.Checked)
            {
                sql += " AND CustRef BETWEEN '" + txtFromCode.Text.Trim() + "' AND '" + txtToCode.Text.Trim() + "'";
            }

            if (rbtnSuppRef.Checked)
            {
                sql += " AND SuppRef BETWEEN '" + txtFromCode.Text.Trim() + "' AND '" + txtToCode.Text.Trim() + "'";
            }

            if (id.Length > 0)
            {
                if (rbtnByCustomer.Checked)
                {
                    sql += " AND CustomerId ='" + id + "'";
                }
                else if (rbtnBySupplier.Checked)
                {
                    sql += " AND SupplierId ='" + id + "'";
                }
            }
            sql += " ORDER BY ArticleCode,ScheduledDate,SCNumber";

            return sql;
        }

        /// <summary>
        /// Bind Data to ListView Control
        /// </summary>
        private void BindList()
        {
            if (id.Length > 0)
            {
                this.lvwList.Items.Clear();

                int iCount = 1;
                string sql = BindSql();
                SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
                while (reader.Read())
                {
                    ListViewItem objItem = this.lvwList.Items.Add(reader.GetGuid(0).ToString()); //OrderQTItemId
                    objItem.SubItems.Add(reader.GetString(1));          //ArticleCode
                    objItem.SubItems.Add(reader.GetString(2));          //SupplierCode
                    objItem.SubItems.Add(reader.GetString(3));          //PackageCode
                    objItem.SubItems.Add(reader.GetString(4));          //CustRef
                    objItem.SubItems.Add(reader.GetString(5));          //SuppRef
                    objItem.SubItems.Add(reader.GetString(6));          //CustName
                    objItem.SubItems.Add(reader.GetString(7));          //SuppName
                    objItem.SubItems.Add(reader.GetString(8));          //SCNumber
                    objItem.SubItems.Add(reader.GetDateTime(9).ToString("dd MMM yyyy"));    //ScheduledDate
                    objItem.SubItems.Add(reader.GetDecimal(10).ToString("#,##0.00"));       //ScheduledQty
                    objItem.SubItems.Add(reader.GetString(11));                             //Unit
                    objItem.SubItems.Add(reader.GetDecimal(10).ToString("#,##0.00"));       //ShippedQty
                    objItem.SubItems.Add(reader.GetDecimal(10).ToString("#,##0.00"));       //OSQty

                    iCount++;
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("No Selected Item!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RadionButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                RadioButton btnCtrl = sender as RadioButton;
                if (btnCtrl != null)
                {
                    tvList.Nodes.Clear();
                    switch (btnCtrl.Name.ToLower())
                    {
                        case "rbtnbycustomer":
                            xPort5.Controls.Utility.TreeViewControl.Load<Customer>(this.tvList.Nodes);
                            break;
                        case "rbtnbysupplier":
                            xPort5.Controls.Utility.TreeViewControl.Load<Supplier>(this.tvList.Nodes);
                            break;
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindList();
        }

        private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (rbtnByCustomer.Checked && !(e.Node.HasNodes))
            {
                id = e.Node.Tag.ToString();
                //BindList();
            }
            else if (rbtnBySupplier.Checked && !(e.Node.HasNodes))
            {
                id = e.Node.Tag.ToString();
                //BindList();
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItem != null)
            {
                if (Common.Utility.IsGUID(this.lvwList.SelectedItem.Text))
                {
                    xPort5.Order.Analysis.ActualShipments actualShipment = new ActualShipments(new System.Guid(lvwList.SelectedItem.Text));
                    actualShipment.ShowDialog();
                }
            }
        }
    }
}
