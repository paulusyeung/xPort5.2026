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
using System.Data.SqlClient;

#endregion

namespace xPort5.Order.Analysis
{
    public partial class OutstandingSample : UserControl
    {
        string baseSqlSelect = string.Empty;
        string baseSqlWhere = string.Empty;
        string baseSqlOrder = string.Empty;
        string curSqlWhere = string.Empty;

        public OutstandingSample()
        {
            InitializeComponent();
            SetLvwList();
            SetCaptions();
            SetAttributes();
            LoadTreeViewList();
            
            // ViewService handles selection and ordering
            baseSqlWhere = " (SampleQty <> 0) AND (QtyOUT < SampleQty) ";
            baseSqlOrder = "CustName, ArticleCode";
            curSqlWhere = baseSqlWhere;

            this.tabOSSample.SelectedIndex = 1;
        }

        private void SetLvwList()
        {
            this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.Dock = DockStyle.Fill;
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.tpCustomer.Text = oDict.GetWord("customer");
            this.tpArticle.Text = oDict.GetWord("product");

            this.colSupplier.Text = oDict.GetWord("supplier");
            this.colCustomer.Text = oDict.GetWord("customer");
            this.colCustRef.Text = oDict.GetWord("customer_ref");
            this.colArticleCode.Text = oDict.GetWord("product_code");
            this.colArticleName.Text = oDict.GetWord("article_name");
            this.colUnitPrice.Text = oDict.GetWord("unit_price");
            this.colSampleQty.Text = oDict.GetWord("sample_qty");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colPriceListN.Text = oDict.GetWord("price_list_no");
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
            xPort5.Controls.Utility.TreeViewControl.LoadCustomer(this.tvList.Nodes);
        }

        /// <summary>
        /// Bind Date to ListView
        /// </summary>
        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            
            // Refactored to use ViewService
            string whereClause = curSqlWhere;
            // Remove "WHERE" keyword if present (ViewService expects just the condition)
            // Actually ViewService.ApplyDynamicWhere handles it, but let's be clean.
            // My previous variable naming was baseSqlWhere which included " WHERE ".
            // I'll strip it here just in case.
            if (!string.IsNullOrWhiteSpace(whereClause))
            {
               whereClause = whereClause.Replace("WHERE ", "").Trim();
            }

            DataSet ds = ViewService.Default.GetSampleItemRecord(whereClause, baseSqlOrder);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem objItems = this.lvwList.Items.Add(row["CustomerId"].ToString());  //CustomerId
                objItems.SubItems.Add(row["CustName"].ToString());                           //Customer                          
                objItems.SubItems.Add(row["CustRef"].ToString());                           //CustRef
                objItems.SubItems.Add(row["ArticleCode"].ToString());                           //ArticleCode
                objItems.SubItems.Add(row["ArtName"].ToString());                           //ArticleName
                
                decimal amount = row["Amount"] != DBNull.Value ? (decimal)row["Amount"] : 0;
                objItems.SubItems.Add(amount.ToString("#,##0.0000"));   //UnitPrice
                
                decimal sampleQty = row["SampleQty"] != DBNull.Value ? (decimal)row["SampleQty"] : 0;
                objItems.SubItems.Add(sampleQty.ToString("##0.00"));       //SampleQty
                
                objItems.SubItems.Add(row["Unit"].ToString());                           //Unit
                objItems.SubItems.Add(row["SupplierName"].ToString());                           //Supplier
                objItems.SubItems.Add(row["QTNumber"].ToString());                           //PriceListNo

                iCount++;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (this.txtArticle.Text.Trim().Length > 0)
            {
                curSqlWhere = baseSqlWhere + string.Format(@" AND (ArticleCode like '%{0}%') ", txtArticle.Text.Trim());
                BindList();
            }
        }

        private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(e.Node.HasNodes))
            {
                string customerId = e.Node.Tag.ToString();
                curSqlWhere = baseSqlWhere + string.Format(@" AND (CustomerId = '{0}') ", customerId);
                BindList();
            }
        }

        private void tabOSSample_SelectedIndexChanged(object sender, EventArgs e)
        {
            tvList.CollapseAll();
        }
    }
}
