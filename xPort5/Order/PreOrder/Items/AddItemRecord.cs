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

namespace xPort5.Order.PreOrder.Items
{
    public partial class AddItemRecord : Form
    {
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _OrderId = System.Guid.Empty;
        private Guid _OrderItemId = System.Guid.Empty;

        #region public properties
        public Common.Enums.EditMode EditMode
        {
            get
            {
                return _EditMode;
            }
            set
            {
                _EditMode = value;
            }
        }
        public Guid OrderId
        {
            get
            {
                return _OrderId;
            }
            set
            {
                _OrderId = value;
            }
        }
        public Guid OrderItemId
        {
            get
            {
                return _OrderItemId;
            }
            set
            {
                _OrderItemId = value;
            }
        }
        public int LineNumber { get; set; }
        #endregion

        public AddItemRecord()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetAttributes();
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format("{0} {1}", oDict.GetWord("add"), string.Format(oDict.GetWord("record"), oDict.GetWord("item")));

            this.lblQuotationNumber.Text = oDict.GetWordWithColon("qt_number");
            this.btnOK.Text = oDict.GetWord("ok");
            this.btnClear.Text = oDict.GetWord("clear");

            this.colArticleCode.Text = oDict.GetWord("product_code");
            this.colSupplier.Text = oDict.GetWord("supplier");
            this.colPackage.Text = oDict.GetWord("package");
            this.colColor.Text = oDict.GetWord("color");
            this.colCustRef.Text = oDict.GetWord("customer_ref");
            this.colResult.Visible = false;
        }

        private void BindList()
        {
            if (txtQuotationNumber.Text.Trim().Length > 0)
            {
                lvItemList.Items.Clear();

                string sql = @"
SELECT [QTNumber]           --0
      ,[OrderQTItemId]
      ,[LineNumber]
      ,[ArticleId]
      ,[ArticleCode]
      ,[SupplierId]         --5
      ,[SupplierName]
      ,[PackageId]
      ,[PackageName]
      ,[CustRef]
  FROM [dbo].[vwOrderQTItemList]";

                sql += string.Format(" WHERE [QTNumber] = '{0}'", txtQuotationNumber.Text.Trim());
                sql += " ORDER BY [LineNumber], [ArticleCode] ";

                SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
                while (reader.Read())
                {
                    Guid productId = reader.GetGuid(3);
                    String color = xPort5.Controls.Utility.Product.GetColor(productId);

                    ListViewItem lvItem = lvItemList.Items.Add(reader.GetGuid(1).ToString());
                    lvItem.SubItems.Add(reader.GetInt32(2).ToString());
                    lvItem.SubItems.Add(reader.GetString(4)); // Article Code
                    lvItem.SubItems.Add(reader.GetString(6)); // Supplier
                    lvItem.SubItems.Add(reader.GetString(8)); // Package
                    lvItem.SubItems.Add(color);                 // Color
                    lvItem.SubItems.Add(reader.GetString(9)); // CustRef
                    lvItem.SubItems.Add(string.Empty); // result
                }
            }
        }

        private void btnLookForQTNumber_Click(object sender, EventArgs e)
        {
            BindList();
        }

        private void txtQuotationNumber_EnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            BindList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lvItemList.CheckedItems.Count > 0)
            {
                foreach (ListViewItem lvItem in lvItemList.CheckedItems)
                {
                    if (Common.Utility.IsGUID(lvItem.Text))
                    {
                        string sql = "OrderQTItemId = '" + lvItem.Text + "' AND OrderPLId = '" + this.OrderId.ToString() + "'";
                        OrderPLItems item = OrderPLItems.LoadWhere(sql);
                        if (item == null)
                        {
                            item = new OrderPLItems();
                            item.OrderPLId = this.OrderId;
                            item.OrderQTItemId = new Guid(lvItem.Text);
                            item.LineNumber = LineNumber;
                            item.Save();

                            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, item.ToString());

                            lvItem.SubItems[5].Text = "Added";
                        }
                        else
                        {
                            lvItem.SubItems[5].Text = "Existed";
                        }
                    }

                    LineNumber++;
                }

                this.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtQuotationNumber.Clear();

            this.lvItemList.Items.Clear();
        }
    }
}
