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
                string sql = BuildSql();

                SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
                while (reader.Read())
                {
                    ListViewItem objItem = this.lvwList.Items.Add(reader.GetGuid(0).ToString());    //CustomerId
                    objItem.SubItems.Add(reader.GetString(1));      //SuppName
                    objItem.SubItems.Add(reader.GetString(2));      //PCNumber
                    objItem.SubItems.Add(reader.GetString(3));      //CustName
                    objItem.SubItems.Add(reader.GetString(4));      //CustRef
                    objItem.SubItems.Add(reader.GetString(5));      //ArticleCode
                    objItem.SubItems.Add(reader.GetString(6));      //ArtName
                    objItem.SubItems.Add(reader.GetString(7));      //Packing
                    objItem.SubItems.Add(reader.GetDecimal(8).ToString("#,##0"));        //ScheduledQty
                    objItem.SubItems.Add(reader.GetString(9));                           //OrderedCny
                    objItem.SubItems.Add(reader.GetDecimal(10).ToString("#,##0.0000"));  //OrderedPrice
                    objItem.SubItems.Add(reader.GetString(11));                          //OrderedUnit
                    objItem.SubItems.Add(reader.GetString(12));                          //FactoryCny
                    objItem.SubItems.Add(reader.GetDecimal(13).ToString("#,##0.0000"));  //FactoryCost
                    objItem.SubItems.Add(reader.GetString(14));                          //FactoryUnit
                    objItem.SubItems.Add(reader.GetDateTime(15).ToString("dd MMM yyyy"));//ScheduledShipmentDate

                    iCount++;
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("No Selected Item!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string BuildSql()
        {
            string sql = string.Empty;
            sql = @"
SELECT	CustomerId, SuppName, PCNumber ,CustName, CustRef, ArticleCode ,ArtName ,Packing, ScheduledQty ,OrderedCny,
		OrderedPrice, OrderedUnit, FactoryCny, FactoryCost, FactoryUnit, ScheduledShipmentDate 
FROM	vwPurchaseHistory 
WHERE	LEN(PCNumber) > 0 AND CONVERT(NVARCHAR(10),ScheduledShipmentDate,126)>='" + dtpFrom.Value.ToString("yyyy-MM-dd") + @"' 
                          AND CONVERT(NVARCHAR(10),ScheduledShipmentDate,126)<='" + dtpTo.Value.ToString("yyyy-MM-dd") + @"'
";
            if (rbtnCustomer.Checked)
            {
                sql = sql + " AND CustomerId IN(" + selectedList + ")";
            }
            if (rbtnSupplier.Checked)
            {
                sql = sql + " AND SupplierId IN(" + selectedList + ")";
            }

            sql += " ORDER BY SuppName,CustName,ArticleCode,PCNumber,ScheduledShipmentDate";

            return sql;
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
