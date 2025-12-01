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
    public partial class ShipmentHistory : UserControl
    {
        public ShipmentHistory()
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

        private string BindSql()
        {
            string sql = string.Empty;
            sql = @"
SELECT OrderQTItemId, ArticleCode, SupplierCode ,PackageCode ,CustRef, SuppRef, CustName,
	   SuppName, SCNumber, ScheduledDate, ScheduledQty, Unit, ShippedQty, OSQty, SKU 
FROM vwShipmentHistory 
";
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
                sql = sql + " WHERE SuppRef LIKE '%" + txtSupplierCode.Text + "%'";
            }

            sql += " ORDER BY ArticleCode,ScheduledDate,SCNumber";

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

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItem != null)
            {
                if (Common.Utility.IsGUID(this.lvwList.SelectedItem.Text))
                {
                    xPort5.Order.Analysis.ActualShipments actualShipment = new ActualShipments(new System.Guid(this.lvwList.SelectedItem.Text));
                    actualShipment.ShowDialog();
                }
            }
        }
    }
}
