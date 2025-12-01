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
            string sql = string.Empty;
            sql = @" SELECT OrderQTItemId ,ArticleCode ,SupplierCode ,PackageCode ,CustomerName,
                            CustRef ,QTDate ,QTNumber ,Margin ,PriceType ,Amount ,CurrencyCode,
                            FactoryCost ,CurrencyUsed ,InnerBox ,OuterBox ,CUFT ,Unit ,SKU
                     FROM vwQuoteHistory ";

            if (this.txtArticleCode.Text.Trim().Length > 0)
            {
                sql = sql + " WHERE ArticleCode LIKE '%" + txtArticleCode.Text + "%'";
            }

            if (this.txtCustArticleCode.Text.Trim().Length > 0)
            {
                sql = sql + " WHERE CustRef LIKE '%" + txtCustArticleCode.Text + "%'";
            }

            if (this.txtSupplierCode.Text.Trim().Length > 0)
            {
                sql = sql + " WHERE SupplierCode LIKE '%" + txtSupplierCode.Text + "%'";
            }

            return sql;
        }

        private void BindList()
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
                objItem.SubItems.Add(reader.GetString(4));          //CustomerName
                objItem.SubItems.Add(reader.GetString(5));          //CustRef
                objItem.SubItems.Add(reader.GetDateTime(6).ToString("dd MMM yyyy"));    //QTDate
                objItem.SubItems.Add(reader.GetString(7));                              //QTNumber
                objItem.SubItems.Add(reader.GetDecimal(8).ToString("N2"));              //Margin
                objItem.SubItems.Add(reader.GetString(9));                              //PriceType
                objItem.SubItems.Add(reader.GetDecimal(10).ToString("#,##0.0000"));     //Amount
                objItem.SubItems.Add(reader.GetString(11));                             //CurrencyCode
                objItem.SubItems.Add(reader.GetDecimal(12).ToString("#,##0.0000"));     //FactoryCost
                objItem.SubItems.Add(reader.GetString(13));                             //CurrencyUsed
                objItem.SubItems.Add(reader.GetDecimal(14).ToString("##0.00"));         //InnerBox
                objItem.SubItems.Add(reader.GetDecimal(15).ToString("##0.00"));         //OuterBox
                objItem.SubItems.Add(reader.GetDecimal(16).ToString("#,##0.0000"));     //CUFT
                objItem.SubItems.Add(reader.GetString(17));                             //Unit
                objItem.SubItems.Add(reader.GetString(18));                             //SKU

                iCount++;
            }
            reader.Close();
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
