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
    public partial class QuoteHistory : UserControl
    {
        public QuoteHistory()
        {
            InitializeComponent();
            SetLvwList();
            SetCaptions();
            SetAttributes();
        }

        private void SetLvwList()
        {
            this.lvwList.ListViewItemSorter = new ListViewItemSorter(this.lvwList);
            this.lvwList.Dock = DockStyle.Fill;
        }

        #region SetTheme...
        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.gbOurRef.Text = oDict.GetWord("select_by");
            this.groupBox1.Text = oDict.GetWord("select_by");
            this.gbSupplierCode.Text = oDict.GetWord("select_by");
            this.tabPage1.Text = oDict.GetWord("our_ref");
            this.lblArticleCode.Text = oDict.GetWordWithColon("product_code");
            this.btnFind.Text = oDict.GetWord("search");
            this.btnClear.Text = oDict.GetWord("clear");
            this.custRefPage.Text = oDict.GetWord("customer_ref");
            this.lblCustArticleCode.Text = oDict.GetWordWithColon("customer_code");
            this.btnCustomerFind.Text = oDict.GetWord("search");
            this.btnCustomerClear.Text = oDict.GetWord("clear");
            this.SupplierPage.Text = oDict.GetWord("supplier_ref");
            this.lblSupplierCode.Text = oDict.GetWordWithColon("supplier_code");
            this.btnSupplierFind.Text = oDict.GetWord("search");
            this.btnSupplierClear.Text = oDict.GetWord("clear");

            this.colArticleCode.Text = oDict.GetWord("product_code");
            this.colSupplierCode.Text = oDict.GetWord("supplier_code");
            this.colPackageCode.Text = oDict.GetWord("package");
            this.colCustomerName.Text = oDict.GetWord("customer_name");
            this.colCustomerRef.Text = oDict.GetWord("customer_ref");
            this.colQuoteDate.Text = oDict.GetWord("qt_date");
            this.colPriceListNO.Text = oDict.GetWord("price_list_no");
            this.colMargin.Text = oDict.GetWord("Margin");
            this.colType.Text = oDict.GetWord("type");
            this.colUnitPrice.Text = oDict.GetWord("unit_price");
            this.colUnitCost.Text = oDict.GetWord("unit_cost");
            this.colUnit.Text = oDict.GetWord("unit");
            this.colOuterBox.Text = oDict.GetWord("outer_box");
            this.colInnerBox.Text = oDict.GetWord("inner_box");
            this.colCurrency1.Text = oDict.GetWord("Currency");
            this.colCurrencyr2.Text = oDict.GetWord("Currency");
            
        }

        private void SetAttributes()
        {
            ansTree.Height = 24;
            ansTree.TextAlign = ToolBarTextAlign.Right;
            ansTree.Dock = DockStyle.Top;

            tbQuote.Dock = DockStyle.Fill;

            ansList.Height = 24;
            ansList.TextAlign = ToolBarTextAlign.Right;
            ansList.Dock = DockStyle.Top;

            lvwList.Dock = DockStyle.Fill;
        }

        private void SetAnsList()
        {
            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdButton
            this.ansList.Buttons.Add(new ToolBarButton("ExportToExcel", string.Empty));
            #endregion
        }
        #endregion

        private string BindSql()
        {
            string whereClause = "";

            if (this.txtArticleCode.Text.Trim().Length > 0)
            {
                whereClause = "ArticleCode LIKE '%" + txtArticleCode.Text + "%'";
            }

            if (this.txtCustArticleCode.Text.Trim().Length > 0)
            {
                whereClause = "CustRef LIKE '%" + txtCustArticleCode.Text + "%'";
            }

            if (this.txtSupplierCode.Text.Trim().Length > 0)
            {
                whereClause = "SupplierCode LIKE '%" + txtSupplierCode.Text + "%'";
            }

            return whereClause;
        }

        private void BindList()
        {
            this.lvwList.Items.Clear();

            int iCount = 1;
            
            // Use ViewService instead of direct SQL query
            string whereClause = BindSql();
            DataSet ds = ViewService.Default.GetQuoteHistory(whereClause, "");
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem objItem = this.lvwList.Items.Add(row["OrderQTItemId"].ToString()); //OrderQTItemId
                objItem.SubItems.Add(row["ArticleCode"] != DBNull.Value ? row["ArticleCode"].ToString() : "");          //ArticleCode
                objItem.SubItems.Add(row["SupplierCode"] != DBNull.Value ? row["SupplierCode"].ToString() : "");          //SupplierCode
                objItem.SubItems.Add(row["PackageCode"] != DBNull.Value ? row["PackageCode"].ToString() : "");          //PackageCode
                objItem.SubItems.Add(row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : "");          //CustomerName
                objItem.SubItems.Add(row["CustRef"] != DBNull.Value ? row["CustRef"].ToString() : "");          //CustRef
                objItem.SubItems.Add(row["QTDate"] != DBNull.Value ? Convert.ToDateTime(row["QTDate"]).ToString("dd MMM yyyy") : "");    //QTDate
                objItem.SubItems.Add(row["QTNumber"] != DBNull.Value ? row["QTNumber"].ToString() : "");                              //QTNumber
                objItem.SubItems.Add(row["Margin"] != DBNull.Value ? Convert.ToDecimal(row["Margin"]).ToString("N2") : "0.00");              //Margin
                objItem.SubItems.Add(row["PriceType"] != DBNull.Value ? row["PriceType"].ToString() : "");                              //PriceType
                objItem.SubItems.Add(row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]).ToString("#,##0.0000") : "0.0000");     //Amount
                objItem.SubItems.Add(row["CurrencyCode"] != DBNull.Value ? row["CurrencyCode"].ToString() : "");                             //CurrencyCode
                objItem.SubItems.Add(row["FactoryCost"] != DBNull.Value ? Convert.ToDecimal(row["FactoryCost"]).ToString("#,##0.0000") : "0.0000");     //FactoryCost
                objItem.SubItems.Add(row["CurrencyUsed"] != DBNull.Value ? row["CurrencyUsed"].ToString() : "");                             //CurrencyUsed
                objItem.SubItems.Add(row["InnerBox"] != DBNull.Value ? Convert.ToDecimal(row["InnerBox"]).ToString("##0.00") : "0.00");         //InnerBox
                objItem.SubItems.Add(row["OuterBox"] != DBNull.Value ? Convert.ToDecimal(row["OuterBox"]).ToString("##0.00") : "0.00");         //OuterBox
                objItem.SubItems.Add(row["CUFT"] != DBNull.Value ? Convert.ToDecimal(row["CUFT"]).ToString("#,##0.0000") : "0.0000");     //CUFT
                objItem.SubItems.Add(row["Unit"] != DBNull.Value ? row["Unit"].ToString() : "");                             //Unit
                objItem.SubItems.Add(row["SKU"] != DBNull.Value ? row["SKU"].ToString() : "");                             //SKU

                iCount++;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            this.lvwList.Items.Clear();

            this.txtCustArticleCode.Text = "";
            this.txtSupplierCode.Text = "";

            if (this.txtArticleCode.Text.Trim().Length > 0)
            {
                BindList();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtArticleCode.Text = "";
            this.lvwList.Items.Clear();
        }

        private void btnCustomerFind_Click(object sender, EventArgs e)
        {
            this.lvwList.Items.Clear();

            this.txtArticleCode.Text = "";
            this.txtSupplierCode.Text = "";

            if (this.txtCustArticleCode.Text.Trim().Length > 0)
            {
                BindList();
            }
        }

        private void btnCustomerClear_Click(object sender, EventArgs e)
        {
            this.txtCustArticleCode.Text = "";
            this.lvwList.Items.Clear();
        }

        private void btnSupplierFind_Click(object sender, EventArgs e)
        {
            this.lvwList.Items.Clear();

            this.txtArticleCode.Text = "";
            this.txtCustArticleCode.Text = "";

            if (this.txtSupplierCode.Text.Trim().Length > 0)
            {
                BindList();
            }
        }

        private void btnSupplierClear_Click(object sender, EventArgs e)
        {
            this.txtSupplierCode.Text = "";
            this.lvwList.Items.Clear();
        }

        private void ansList_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Button.Name))
            {
                switch (e.Button.Name.ToLower())
                {
                    case "exporttoexcel":
                        break;
                }
            }
        }
    }
}
