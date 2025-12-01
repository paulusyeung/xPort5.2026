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

namespace xPort5.Controls.Product
{
    public partial class FindProduct : Form
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Color { get; set; }

        public FindProduct()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttributes();

            Search();
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", oDict.GetWord("look_for"), oDict.GetWord("product"));

            this.lblProductCode.Text = oDict.GetWordWithColon("product_code");
            this.lblColor.Text = oDict.GetWordWithColon("color");
            this.btnSearch.Text = oDict.GetWord("search");

            string codeReplace = oDict.GetWord("code_replace");
            string nameReplace = oDict.GetWord("name4others_replace");

            this.colProductCode.Text = oDict.GetWord("product_code");
            this.colSupplierCode.Text = string.Format(codeReplace, oDict.GetWord("supplier"));
            this.colPackageCode.Text = string.Format(codeReplace, oDict.GetWord("package"));
            this.colProductName.Text = string.Format(nameReplace, oDict.GetWord("product"));
            this.colSupplierName.Text = string.Format(nameReplace, oDict.GetWord("supplier"));
            this.colSupplierRef.Text = oDict.GetWord("supplier_ref");
            this.colPackageName.Text = string.Format(nameReplace, oDict.GetWord("package"));
            this.colColor.Text = oDict.GetWord("color");
            this.colInnerBox.Text = oDict.GetWord("Inner Box");
            this.colOuterBox.Text = oDict.GetWord("Outer Box");
            this.colUnit.Text = oDict.GetWord("unit");
            //this.colCUFT.Text = oDict.GetWord("cuft");

            this.txtProductCode.Text = this.ProductCode;

            toolTip1.SetToolTip(this.lvResultList, oDict.GetWord("double_click_to_select_record"));
        }

        private void lvResultList_DoubleClick(object sender, EventArgs e)
        {
            if (lvResultList.SelectedItem != null)
            {
                if (Common.Utility.IsGUID(lvResultList.SelectedItem.Text.Trim()))
                {
                    this.ProductId = new Guid(lvResultList.SelectedItem.Text.Trim());
                    this.ProductCode = lvResultList.SelectedItem.SubItems[1].Text.Trim();
                    this.Color = lvResultList.SelectedItem.SubItems[8].Text.Trim();

                    this.Close();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private bool BuildQuery(out string query)
        {
            bool canQuery = true;
            query = @"SELECT [ArticleId],[SKU],[ArticleCode],[ArticleName],[ArticleName_Chs],[ArticleName_Cht]
                                ,[PackageCode],[PackageName],[SupplierName],[SuppRef],[SupplierCode],[ColorPattern]
                                ,[Unit],[InnerBox],[OuterBox],[CUFT]
                            FROM [dbo].[vwProductWithSupplierAndPackage]
                            {0}
                            ORDER BY [ArticleCode],[ArticleName],[ArticleName_Chs],[ArticleName_Cht]";

            string sqlWhere = " WHERE {0} ";
            if (txtProductCode.Text.Trim().Length > 0)
            {
                sqlWhere = string.Format(sqlWhere, string.Format(" SKU LIKE '{0}%' OR [ArticleCode] LIKE '{0}%' ", txtProductCode.Text.Trim()));
            }
            else if (txtColor.Text.Trim().Length > 0)
            {
                sqlWhere = string.Format(sqlWhere, string.Format(" [ColorPattern] LIKE '{0}%' ", txtColor.Text.Trim()));
            }
            else if (txtProductCode.Text.Trim().Length > 0 && txtColor.Text.Trim().Length > 0)
            {
                sqlWhere = string.Format(sqlWhere, string.Format(" SKU LIKE '{0}%' OR [ArticleCode] LIKE '{0}%' OR [ColorPattern] LIKE '{1}%'", txtProductCode.Text.Trim(), txtColor.Text.Trim()));
            }
            else
            {
                sqlWhere = string.Format(sqlWhere, string.Empty);
                canQuery = false;
            }

            query = string.Format(query, sqlWhere);

            return canQuery;
        }
            
        private void Search()
        {
            string sql = string.Empty;
            bool canQuery = BuildQuery(out sql);

            if (canQuery)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandTimeout = Common.Config.CommandTimeOut;
                cmd.CommandType = CommandType.Text;

                SqlDataReader reader = SqlHelper.Default.ExecuteReader(cmd);
                while (reader.Read())
                {
                    ListViewItem lvItem = lvResultList.Items.Add(reader.GetGuid(0).ToString()); // ProductId
                    lvItem.SubItems.Add(reader.GetString(2)); // Product Code
                    lvItem.SubItems.Add(reader.GetString(11)); // Color
                    lvItem.SubItems.Add(reader.GetString(10)); // Supplier Code
                    lvItem.SubItems.Add(reader.GetString(6)); // Package Code
                    lvItem.SubItems.Add(reader.GetString(3)); // Product Name
                    lvItem.SubItems.Add(reader.GetString(8)); // Supplier Name
                    lvItem.SubItems.Add(reader.GetString(9)); // Supplier Ref.
                    lvItem.SubItems.Add(reader.GetString(7)); // Package Name
                    lvItem.SubItems.Add(reader.GetString(12)); // Unit
                    lvItem.SubItems.Add(reader.GetDecimal(13).ToString("##0.00")); // Inner Box
                    lvItem.SubItems.Add(reader.GetDecimal(14).ToString("##0.00")); // Outer Box
                    lvItem.SubItems.Add(reader.GetDecimal(15).ToString("##0.00")); // CUFT
                }
            }
            else
            {
            }
        }

        private void txtColor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return || e.KeyData == Keys.Enter)
            {
                Search();
            }
        }
    }
}
