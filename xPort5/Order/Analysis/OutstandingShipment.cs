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
    public partial class OutstandingShipment : UserControl
    {
        private string _BaseSqlSelect = string.Empty;
        private string _BaseSqlWhere = string.Empty;
        private string _BaseSqlOrderBy = string.Empty;
        private string _CurSqlWhere = string.Empty;
        private string _CustName = string.Empty;
        private string _SuppName = String.Empty;

        public OutstandingShipment()
        {
            InitializeComponent();
            SetLvwList();
            SetCaptions();
            SetAttributes();
            SetBaseSqlSelect();

            this.tbOsShipment.SelectedIndex = 1;
            xPort5.Controls.Utility.TreeViewControl.LoadCustomer(this.tvList.Nodes);
        }

        private void SetLvwList()
        {
            this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.Dock = DockStyle.Fill;
            this.lvwList.Margin = new Padding(0, 24, 0, 0);
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.tpCustSupp.Text = string.Format(oDict.GetWord("customer") + "/" + oDict.GetWord("supplier"));
            //this.lblCustSupp.Text = oDict.GetWord("");
            this.rbtCustomer.Text = oDict.GetWord("customer");
            this.rbtSupplier.Text = oDict.GetWord("supplier");
            this.tpArticle.Text = oDict.GetWord("product");
            this.tpOption.Text = oDict.GetWord("option");
            this.gbColor.Text = oDict.GetWord("color_by_number_of_days");
            this.lblRed.Text = oDict.GetWord("red");
            this.lblYellow.Text = oDict.GetWord("yellow");
            this.lblGreen.Text = oDict.GetWord("green");
            this.gbPeriod.Text = oDict.GetWord("date_range");
            this.lblFromDatetime.Text = oDict.GetWord("from");
            this.lblToDatetime.Text = oDict.GetWord("to");

            this.colSupplier.Text = oDict.GetWord("supplier");
            this.colCustomer.Text = oDict.GetWord("customer");
            this.colCustRef.Text = oDict.GetWord("customer_ref");
            this.colArticleNo.Text = oDict.GetWord("product_code");
            this.colArticleName.Text = oDict.GetWord("product_description");
            this.colUnitPrice.Text = oDict.GetWord("unit_price");
            this.colCurrency1.Text = oDict.GetWord("currency");
            this.colOrderQty.Text = oDict.GetWord("order_qty");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colShipmentDate.Text = oDict.GetWord("shipment_date");
            this.colScheduledQty.Text = oDict.GetWord("scheduled_qty");
            this.colShippedQty.Text = oDict.GetWord("shipped_qty");
            this.colOSQty.Text = oDict.GetWord("osqty");
            this.colOSAmount.Text = oDict.GetWord("amount");
            this.colSalesContract.Text = oDict.GetWord("sales_contract");
        }

        private void SetAttributes()
        {
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.SplitterDistance = 200;

            toolBar1.Height = 24;
            toolBar1.TextAlign = ToolBarTextAlign.Right;

            toolBar2.Height = 24;
            toolBar2.TextAlign = ToolBarTextAlign.Right;

            splitContainer1.SplitterDistance = 200;
        }

        /// <summary>
        /// Set Base Sql Select
        /// </summary>
        private void SetBaseSqlSelect()
        {
            _BaseSqlSelect = @"SELECT TOP 100 PERCENT CustomerId, CustName, CustRef, ArticleCode,  
                                     ArtName, CurrencyCode, Amount, Qty, Unit, ShipmentDate,
                                     QtyOrdered, QtyShipped, OSQty, OSAmount, SuppName, SCNumber 
                              FROM vwOSShipment ";
            _BaseSqlWhere = @" WHERE (QtyShipped < QtyOrdered OR QtyShipped IS NULL)";
            _BaseSqlOrderBy = " ORDER BY ShipmentDate, CustName, ArticleCode";
            _CurSqlWhere = _BaseSqlWhere;
        }

        private string BuildSql()
        {
            StringBuilder sql = new StringBuilder();
            _CurSqlWhere = _CurSqlWhere + @" AND CONVERT(NVARCHAR(10),ShipmentDate,126) >= '" + dtpFromDate.Value.ToString("yyyy-MM-dd") + @"' AND 
                                     CONVERT(NVARCHAR(10),ShipmentDate,126) <= '" + dtpToDate.Value.ToString("yyyy-MM-dd") + "'";

            sql.Append(_BaseSqlSelect + Environment.NewLine);
            sql.Append(_CurSqlWhere + Environment.NewLine);
            sql.Append(_BaseSqlOrderBy);

            return sql.ToString();
        }

        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            string sql = BuildSql();
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                ListViewItem objItem = this.lvwList.Items.Add(reader.GetGuid(0).ToString());  //CustomerId
                objItem.SubItems.Add(reader.GetString(1));                            //Customer
                objItem.SubItems.Add(reader.GetString(2));                            //CustRef
                objItem.SubItems.Add(reader.GetString(3));                            //ArticleNO
                objItem.SubItems.Add(reader.GetString(4));                            //ArticleName
                objItem.SubItems.Add(reader.GetString(5));                            //CurrencyCode

                objItem.SubItems.Add(reader.GetDecimal(6).ToString("#,##0.0000"));    //UnitPrice
                objItem.UseItemStyleForSubItems = false;
                objItem.SubItems[6].BackColor = Color.PaleTurquoise;

                objItem.SubItems.Add(reader.GetDecimal(7).ToString("##0.00"));        //OrderQty
                objItem.SubItems[7].BackColor = Color.PaleTurquoise;

                objItem.SubItems.Add(reader.GetString(8));                            //Unit
                objItem.SubItems.Add(reader.GetDateTime(9).ToString("dd MMM yyyy"));  //ShipmentDate

                objItem.SubItems.Add(reader.GetDecimal(10).ToString("##0.00"));       //ScheduledQty
                objItem.SubItems[10].BackColor = Color.PaleTurquoise;

                objItem.SubItems.Add(reader.GetDecimal(11).ToString("##0.00"));       //ShippedQty
                objItem.SubItems[11].BackColor = Color.PaleTurquoise;

                objItem.SubItems.Add(reader.GetDecimal(12).ToString("#,##0.00"));     //O/SQty
                objItem.SubItems[12].BackColor = Color.PaleTurquoise;

                objItem.SubItems.Add(reader.GetDecimal(13).ToString("#,##0.0000"));   //O/SAmount
                objItem.SubItems[13].BackColor = Color.PaleTurquoise;

                objItem.SubItems.Add(reader.GetString(14));                           //SupplierName
                objItem.SubItems.Add(reader.GetString(15));                           //SCNumber

                iCount++;
            }
            reader.Close();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (txtArticle.Text.Trim().Length > 0)
            {
                _CurSqlWhere = _BaseSqlWhere + string.Format(@" AND (ArticleCode like '%{0}%') ", txtArticle.Text.Trim());
                BindList();
            }
        }

        private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(e.Node.HasNodes))
            {
                if (rbtCustomer.Checked)
                {
                    _CustName = e.Node.Tag.ToString();
                    _CurSqlWhere = _BaseSqlWhere + string.Format(@" AND (CustomerId = '{0}') ", _CustName);
                }
                else if(rbtSupplier.Checked)
                {
                    _SuppName = e.Node.Tag.ToString();
                    _CurSqlWhere = _BaseSqlWhere + string.Format(@" AND (SupplierId = '{0}') ", _SuppName);
                }

                BindList();
            }
        }

        /// <summary>
        /// 
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
                        case "rbtcustomer":
                            xPort5.Controls.Utility.TreeViewControl.Load<Customer>(tvList.Nodes);
                            break;
                        case "rbtsupplier":
                            xPort5.Controls.Utility.TreeViewControl.Load<Supplier>(tvList.Nodes);
                            break;
                    }
                }
            }
        }
    }
}
