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
            BuildSql();

            baseSqlSelect = @"SELECT CustomerId, CustName, CustRef, ArticleCode, ArtName,  
                                    Amount, SampleQty, Unit, SupplierName, QTNumber     
                              FROM  vwOSSample";
            baseSqlWhere = " WHERE (SampleQty <> 0) AND (QtyOUT < SampleQty) ";
            baseSqlOrder = " ORDER BY CustName, ArticleCode";
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

        private string BuildSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(baseSqlSelect + Environment.NewLine);
            sql.Append(curSqlWhere + Environment.NewLine);
            sql.Append(baseSqlOrder + Environment.NewLine);

            return sql.ToString();
        }

        /// <summary>
        /// Bind Date to ListView
        /// </summary>
        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            string sql = BuildSql();
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                ListViewItem objItems = this.lvwList.Items.Add(reader.GetGuid(0).ToString());  //CustomerId
                objItems.SubItems.Add(reader.GetString(1));                           //Customer                          
                objItems.SubItems.Add(reader.GetString(2));                           //CustRef
                objItems.SubItems.Add(reader.GetString(3));                           //ArticleCode
                objItems.SubItems.Add(reader.GetString(4));                           //ArticleName
                objItems.SubItems.Add(reader.GetDecimal(5).ToString("#,##0.0000"));   //UnitPrice
                objItems.SubItems.Add(reader.GetDecimal(6).ToString("##0.00"));       //SampleQty
                objItems.SubItems.Add(reader.GetString(7));                           //Unit
                objItems.SubItems.Add(reader.GetString(8));                           //Supplier
                objItems.SubItems.Add(reader.GetString(9));                           //PriceListNo

                iCount++;
            }
            reader.Close();
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
