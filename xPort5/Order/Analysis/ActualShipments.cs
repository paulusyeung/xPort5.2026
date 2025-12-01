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
    public partial class ActualShipments : Form
    {
        public ActualShipments()
        {
            InitializeComponent();
        }

        public ActualShipments(Guid itemId)
        {
            InitializeComponent();

            this.orderQtItemId = itemId;
            LoadLvwList();
        }

        #region Properties
        private Guid itemId = System.Guid.Empty;
        public Guid orderQtItemId
        {
            get 
            { 
                return itemId; 
            }
            set 
            { 
                itemId = value; 
            }
        }
        #endregion

        private string BuildSql()
        {
            string sql = @"
SELECT	OrderQTItems.OrderQTItemId, OrderIN.ShipmentDate, OrderINShipment.Qty, OrderQTItems.Unit, OrderIN.INNumber,OrderINShipment.ShipmentID
FROM	OrderINShipment LEFT JOIN (((OrderINItems LEFT JOIN OrderSCItems ON (OrderINItems.OrderSCItemsId = OrderSCItems.OrderSCItemsId)) 
		LEFT JOIN OrderQTItems ON (OrderSCItems.OrderQTItemId = OrderQTItems.OrderQTItemId)) 
		LEFT JOIN OrderIN ON OrderINItems.OrderINId = OrderIN.OrderINId) 
			ON (OrderINShipment.OrderINItemsId = OrderINItems.OrderINItemsId)
		INNER JOIN Article ON Article.ArticleId = OrderQTItems.ArticleId
WHERE OrderQTItems.OrderQTItemId ='" + orderQtItemId.ToString() + @"'
ORDER BY OrderIN.ShipmentDate, OrderIN.INNumber
";
            return sql;
        }

        private void LoadLvwList()
        {
            int iCount = 1;
            string sql = BuildSql();
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
            while (reader.Read())
            {
                ListViewItem objItem = this.lvwList.Items.Add(reader.GetGuid(0).ToString());  //OrderQtItemId
                objItem.SubItems.Add(reader.GetDateTime(1).ToString("dd MMM yyyy"));  //ShipmentDate
                objItem.SubItems.Add(reader.GetDecimal(2).ToString("#,##0.00"));      //Qty
                objItem.SubItems.Add(reader.GetString(3));                            //Unit
                objItem.SubItems.Add(reader.GetString(4));                            //INNumber

                iCount++;
            }
            reader.Close();
        }
    }
}
