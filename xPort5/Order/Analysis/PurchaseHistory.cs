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
    public partial class PurchaseHistory : UserControl
    {
        StringBuilder selectedList = new StringBuilder();

        public PurchaseHistory()
        {
            InitializeComponent();
            SetLvwList();
            SetCaptions();
            SetAttributes();

            tvList.BeforeCheck += new TreeViewCancelEventHandler(tvList_BeforeCheck);

            xPort5.Controls.Utility.TreeViewControl.Load<Customer>(this.tvList.Nodes);

            tabOptions.SelectedIndex = 1;
        }

        private void SetLvwList()
        {
            this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.Dock = DockStyle.Fill;
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.tpOption.Text = oDict.GetWord("option");
            this.tpPeriod.Text = oDict.GetWord("date_range");
            this.lblFrom.Text = oDict.GetWord("from");
            this.lblTo.Text = oDict.GetWord("to");
            this.rbtnCustomer.Text = oDict.GetWord("customer_name");
            this.rbtnSupplier.Text = oDict.GetWord("supplier_name");
            this.btnSelectionAll.Text = oDict.GetWord("all");
            this.btnClearSelection.Text = oDict.GetWord("clear");
            this.gbReportingPeriod.Text = oDict.GetWord("date_range");
            this.gbSelectionBy.Text = oDict.GetWord("select_by");

            this.colSupplier.Text = oDict.GetWord("supplier");
            this.colContractNo.Text = oDict.GetWord("contract_num");
            this.colCustomer.Text = oDict.GetWord("customer");
            this.colCustRef.Text = oDict.GetWord("customer_ref");
            this.colArticleCode.Text = oDict.GetWord("product_code");
            this.colDescription.Text = oDict.GetWord("description");
            this.colPacking.Text = oDict.GetWord("package");
            this.colScheduledQty.Text = oDict.GetWord("scheduled_qty");
            this.colCurrency.Text = oDict.GetWord("currency");
            this.colSellingPrice.Text = oDict.GetWord("selling_price");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colCurrency2.Text = oDict.GetWord("currency");
            this.colPurchaseCost.Text = oDict.GetWord("purchased_cost");
            this.colUnit2.Text = oDict.GetWord("unit");
            this.colScheduledDate.Text = oDict.GetWord("scheduled_date");
        }

        private void SetAttributes()
        {
            toolBar1.Height = 24;
            toolBar1.TextAlign = ToolBarTextAlign.Right;

            toolBar2.Height = 24;
            toolBar2.TextAlign = ToolBarTextAlign.Right;
        }

        /// <summary>
        /// Sets the tree node selection.
        /// </summary>
        /// <param name="node">The node.</param>
        private void SetTreeNodeSelection(TreeNode node)
        {
            if (!node.HasNodes)
            {
                if (node.Tag != null)
                {
                    if (Common.Utility.IsGUID(node.Tag.ToString()))
                    {
                        node.Checked = !node.Checked;
                    }
                }
            }
            else
            {
                node.Expand();

                foreach (TreeNode endNode in node.Nodes)
                {
                    endNode.Checked = !node.Checked;
                }
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the RadioButton control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                RadioButton btnCtrl = sender as RadioButton;
                if (btnCtrl != null)
                {
                    tvList.Nodes.Clear();
                    switch (btnCtrl.Name.ToLower())
                    {
                        case "rbtncustomer":
                            xPort5.Controls.Utility.TreeViewControl.Load<Customer>(tvList.Nodes);
                            break;
                        case "rbtnsupplier":
                            xPort5.Controls.Utility.TreeViewControl.Load<Supplier>(tvList.Nodes);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btnCtrl = sender as Button;
                if (btnCtrl != null)
                {
                    switch (btnCtrl.Name.ToLower())
                    {
                        case "btnselectionall":
                            SelectAllNodes();
                            break;
                        case "btnclearselection":
                            ClearTreeNodesSelection();
                            break;
                        case "btnok":
                            BindLvwList();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Selects all nodes.
        /// </summary>
        private void SelectAllNodes()
        {
            foreach (TreeNode node in tvList.Nodes)
            {
                if (node.HasNodes)
                {
                    node.Expand();
                    node.Checked = true;

                    foreach (TreeNode endNode in node.Nodes)
                    {
                        endNode.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the tree node selection.
        /// </summary>
        private void ClearTreeNodesSelection()
        {
            foreach (TreeNode node in tvList.Nodes)
            {
                if (node.HasNodes)
                {
                    node.Collapse();
                    node.Checked = false;

                    foreach (TreeNode endNode in node.Nodes)
                    {
                        endNode.Checked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected nodes.
        /// </summary>
        private void GetSelectedNodes()
        {
            selectedList.Remove(0, selectedList.Length);

            foreach (TreeNode node in tvList.Nodes)
            {
                if (node.HasNodes)
                {
                    foreach (TreeNode endNode in node.Nodes)
                    {
                        if (endNode.Checked)
                        {
                            if (Common.Utility.IsGUID(endNode.Tag.ToString()))
                            {
                                if (selectedList.Length > 0)
                                {
                                    selectedList.Append(",");
                                }

                                string guid = endNode.Tag.ToString();
                                selectedList.Append("'" + guid + "'");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bind Data to ListView Control
        /// </summary>
        private void BindLvwList()
        {
            GetSelectedNodes();

            if (selectedList.Length > 0)
            {
                this.lvwList.Items.Clear();

                int iCount = 1;
                string whereClause = BuildWhereClause();
                
                // Use ViewService instead of direct SQL query
                DataSet ds = ViewService.Default.GetPurchaseHistory(whereClause, "SuppName,CustName,ArticleCode,PCNumber,ScheduledShipmentDate");
                DataTable dt = ds.Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    ListViewItem objItem = this.lvwList.Items.Add(row["CustomerId"].ToString());    //CustomerId
                    objItem.SubItems.Add(row["SuppName"] != DBNull.Value ? row["SuppName"].ToString() : "");      //SuppName
                    objItem.SubItems.Add(row["PCNumber"] != DBNull.Value ? row["PCNumber"].ToString() : "");      //PCNumber
                    objItem.SubItems.Add(row["CustName"] != DBNull.Value ? row["CustName"].ToString() : "");      //CustName
                    objItem.SubItems.Add(row["CustRef"] != DBNull.Value ? row["CustRef"].ToString() : "");      //CustRef
                    objItem.SubItems.Add(row["ArticleCode"] != DBNull.Value ? row["ArticleCode"].ToString() : "");      //ArticleCode
                    objItem.SubItems.Add(row["ArtName"] != DBNull.Value ? row["ArtName"].ToString() : "");      //ArtName
                    objItem.SubItems.Add(row["Packing"] != DBNull.Value ? row["Packing"].ToString() : "");      //Packing
                    objItem.SubItems.Add(row["ScheduledQty"] != DBNull.Value ? Convert.ToDecimal(row["ScheduledQty"]).ToString("#,##0") : "0");        //ScheduledQty
                    objItem.SubItems.Add(row["OrderedCny"] != DBNull.Value ? row["OrderedCny"].ToString() : "");                           //OrderedCny
                    objItem.SubItems.Add(row["OrderedPrice"] != DBNull.Value ? Convert.ToDecimal(row["OrderedPrice"]).ToString("#,##0.0000") : "0.0000");  //OrderedPrice
                    objItem.SubItems.Add(row["OrderedUnit"] != DBNull.Value ? row["OrderedUnit"].ToString() : "");                          //OrderedUnit
                    objItem.SubItems.Add(row["FactoryCny"] != DBNull.Value ? row["FactoryCny"].ToString() : "");                          //FactoryCny
                    objItem.SubItems.Add(row["FactoryCost"] != DBNull.Value ? Convert.ToDecimal(row["FactoryCost"]).ToString("#,##0.0000") : "0.0000");  //FactoryCost
                    objItem.SubItems.Add(row["FactoryUnit"] != DBNull.Value ? row["FactoryUnit"].ToString() : "");                          //FactoryUnit
                    objItem.SubItems.Add(row["ScheduledShipmentDate"] != DBNull.Value ? Convert.ToDateTime(row["ScheduledShipmentDate"]).ToString("dd MMM yyyy") : "");//ScheduledShipmentDate

                    iCount++;
                }
            }
            else
            {
                MessageBox.Show("No Selected Item!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string BuildSql()
        {
            return BuildWhereClause();
        }

        private string BuildWhereClause()
        {
            string whereClause = "LEN(PCNumber) > 0 AND CONVERT(NVARCHAR(10),ScheduledShipmentDate,126)>='" + dtpFrom.Value.ToString("yyyy-MM-dd") + @"' 
                           AND CONVERT(NVARCHAR(10),ScheduledShipmentDate,126)<='" + dtpTo.Value.ToString("yyyy-MM-dd") + @"'";
            
            if (rbtnCustomer.Checked)
            {
                whereClause = whereClause + " AND CustomerId IN(" + selectedList + ")";
            }
            if (rbtnSupplier.Checked)
            {
                whereClause = whereClause + " AND SupplierId IN(" + selectedList + ")";
            }

            return whereClause;
        }

        /// <summary>
        ///  Handles the Click event of the tvList control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvList_Click(object sender, EventArgs e)
        {
            if (tvList.SelectedNode != null)
            {
                SetTreeNodeSelection(tvList.SelectedNode);
                if (tvList.SelectedNode.HasNodes)
                {
                    tvList.SelectedNode.Checked = !tvList.SelectedNode.Checked;
                }
            }
        }

        /// <summary>
        /// Handles the BeforeCheck event of the tvList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Gizmox.WebGUI.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
        void tvList_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node != null)
            {
                //SetTreeNodeSelection(e.Node);
            }
        }
    }
}
