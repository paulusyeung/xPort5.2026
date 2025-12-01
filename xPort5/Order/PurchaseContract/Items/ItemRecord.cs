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
using Gizmox.WebGUI.Common.Resources;
using System.Data.SqlClient;

#endregion

namespace xPort5.Order.PurchaseContract.Items
{
    public partial class ItemRecord : Form
    {
        private bool sbl = false;
        private bool cbl = false;
        private Guid _QtItemId = System.Guid.Empty;
        private Guid articleId = System.Guid.Empty;
        private Guid articleSupplierId = System.Guid.Empty;
        private Guid currencyId = System.Guid.Empty;

        private string PCNumber = string.Empty;

        private Common.Enums.EditMode editMode = Common.Enums.EditMode.Read;
        private Guid orderPCId = System.Guid.Empty;
        private Guid orderPCItemsId = System.Guid.Empty;
        private Guid orderQTSuppShippingId = System.Guid.Empty;
        private Guid orderQTCustShippingId = System.Guid.Empty;
        private Guid productId = System.Guid.Empty;
        private string productCode = String.Empty;

        public ItemRecord()
        {
            InitializeComponent();
        }

        #region public properties
        public Common.Enums.EditMode EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                editMode = value;
            }
        }
        public Guid OrderPCId
        {
            get
            {
                return orderPCId;
            }
            set
            {
                orderPCId = value;
            }
        }
        public Guid OrderPCItemsId
        {
            get
            {
                return orderPCItemsId;
            }
            set
            {
                orderPCItemsId = value;
            }
        }
        public Guid OrderQTSuppShippingId
        {
            get
            {
                return orderQTSuppShippingId;
            }
            set
            {
                orderQTSuppShippingId = value;
            }
        }
        public Guid OrderQTCustShippingId
        {
            get
            {
                return orderQTCustShippingId;
            }
            set
            {
                orderQTCustShippingId = value;
            }
        }

        #endregion

        private void ItemRecord_Load(object sender, EventArgs e)
        {
            this.Init();
        }

        private void Init()
        {
            SetAttributes();
            SetAnsToolbar();
            SetDropdowns();
            ShowOrder();

            switch ((int)editMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    orderPCItemsId = System.Guid.NewGuid();
                    txtProductCode.Width = txtProductCode.Width - 25;
                    cmdLookupProduct.Visible = true;
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    cmdLookupProduct.Visible = false;
                    ShowItem();
                    break;
            }
        }

        #region Configure Controls on Form Load
        private void SetDropdowns()
        {
            cboShippingMark.DataSource = null;
            T_ShippingMark.LoadCombo(ref cboShippingMark, "ShippingMarkCode", false);
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.Text = string.Format(oDict.GetWord("record"), oDict.GetWord("item"));
            //this.lblLineNumber.Text = oDict.GetWordWithColon("line_no");
            this.lblPCNumber.Text = oDict.GetWordWithColon("purchase_contract_num");
            this.lblProductCode.Text = oDict.GetWordWithColon("product_code");

            this.boxProduct.Text = oDict.GetWord("product_details");

            this.lblProductPicture.Text = oDict.GetWordWithColon("Picture");
            this.lblCurrency.Text = oDict.GetWordWithColon("Currency");
            this.lblUnitCost.Text = oDict.GetWordWithColon("unit_cost");
            this.lblFCLCost.Text = oDict.GetWordWithColon("FCL Cost");
            this.lblLCLCost.Text = oDict.GetWordWithColon("LCL Cost");

            this.lblShippingMark.Text = oDict.GetWordWithColon("shipping_mark");
            string schedule = "{0} {1}" + oDict.GetColon();
            this.lblSuppShippingSchedule.Text = string.Format(schedule, oDict.GetWord("supplier"), oDict.GetWord("shipping_schedule"));
            this.lblCustShippingSchedule.Text = string.Format(schedule, oDict.GetWord("customer"), oDict.GetWord("shipping_schedule"));

            this.colDateShipped.Text = oDict.GetWord("date");
            this.colCustSchedule.Text = oDict.GetWord("Qty");
            this.colShippedOn.Text = oDict.GetWord("date");
            this.colQtyOrdered.Text = oDict.GetWord("qty");

            txtPCNumber.Enabled = false;
            txtProductCode.Enabled = false;

            switch ((int)editMode)
            {
                case (int)Common.Enums.EditMode.Add:
                    break;
                case (int)Common.Enums.EditMode.Edit:
                    break;
            }

            productImage2.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void SetAnsToolbar()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            this.suppToolbar.MenuHandle = false;
            this.suppToolbar.DragHandle = false;

            this.custToolbar.MenuHandle = false;
            this.custToolbar.DragHandle = false;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            xPort5.Controls.Utility.ToolbarControl.LoadAnsBaseButtons(ref this.ansToolbar, editMode, "Order", "Order.PurchaseContract");

            // cmdSNew
            ToolBarButton cmdSNew = new ToolBarButton("New",oDict.GetWord("New"));
            cmdSNew.Tag = "New";
            cmdSNew.Image = new IconResourceHandle("16x16.ico_16_3.gif");

            // cmdSDelete
            ToolBarButton cmdSDelete = new ToolBarButton("Delete",oDict.GetWord("Delete"));
            cmdSDelete.Tag = "Delete";
            cmdSDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            // cmdCNew
            ToolBarButton cmdCNew = new ToolBarButton("New",oDict.GetWord("New"));
            cmdCNew.Tag = "New";
            cmdCNew.Image = new IconResourceHandle("16x16.ico_16_3.gif");

            // cmdCDelete
            ToolBarButton cmdCDelete = new ToolBarButton("Delete",oDict.GetWord("Delete"));
            cmdCDelete.Tag = "Delete";
            cmdCDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

            #region add cmdDNew, cmdCNew, cmdSDelete, cmdCDelete
            if (editMode != Common.Enums.EditMode.Read)
            {
                if (xPort5.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PurchaseContract.Create"))
                    {
                        this.suppToolbar.Buttons.Add(cmdSNew);
                        this.custToolbar.Buttons.Add(cmdCNew);
                    }
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.PurchaseContract.Delete"))
                    {
                        this.suppToolbar.Buttons.Add(cmdSDelete);
                        this.custToolbar.Buttons.Add(cmdCDelete);
                    }
                }
                else
                {
                    this.suppToolbar.Buttons.Add(cmdSNew);
                    this.suppToolbar.Buttons.Add(cmdSDelete);

                    this.custToolbar.Buttons.Add(cmdCNew);
                    this.custToolbar.Buttons.Add(cmdCDelete);
                }
            }
            #endregion

            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
            this.suppToolbar.ButtonClick += new ToolBarButtonClickEventHandler(suppToolbar_ButtonClick);
            this.custToolbar.ButtonClick += new ToolBarButtonClickEventHandler(custToolbar_ButtonClick);
        }

        void suppToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString().ToLower())
                {
                    case "new":
                        sbl = false;
                        AddSuppShipping supp = new AddSuppShipping();
                        supp.Date = DateTime.Now.ToString("yyyy-MM-dd");
                        supp.Closed += new EventHandler(supp_Closed);
                        supp.ShowDialog();
                        break;
                    case "delete":
                        if (lvSSSchedule.SelectedItem != null && lvSSSchedule.SelectedItem.SubItems[0].Text != new Guid().ToString())
                        {
                            if (lvSSSchedule.SelectedItem.SubItems[3].Text == String.Empty)
                            {
                                lvSSSchedule.SelectedItem.SubItems[3].Text = "D";
                                lvSSSchedule.SelectedItem.SubItems.Add(new IconResourceHandle("16x16.flag_dark.png").ToString());
                            }
                            else
                            {
                                lvSSSchedule.SelectedItem.SubItems[3].Text = String.Empty;
                                lvSSSchedule.SelectedItem.SubItems.Remove(lvSSSchedule.SelectedItem.SubItems[4]);
                            }
                        }
                        else
                        {
                            lvSSSchedule.SelectedItem.Remove();
                        }
                        break;
                }
            }
        }

        void custToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString().ToLower())
                { 
                    case "new":
                        cbl = false;
                        AddCustShipping cust = new AddCustShipping();
                        cust.Date = DateTime.Now.ToString("yyyy-MM-dd");
                        cust.Closed += new EventHandler(cust_Closed);
                        cust.ShowDialog();
                        break;
                    case "delete":
                        if (lvCSSchedule.SelectedItem != null && lvCSSchedule.SelectedItem.SubItems[0].Text != new Guid().ToString())
                        {
                            if (lvCSSchedule.SelectedItem.SubItems[3].Text == String.Empty)
                            {
                                lvCSSchedule.SelectedItem.SubItems[3].Text = "D";
                                lvCSSchedule.SelectedItem.SubItems.Add(new IconResourceHandle("16x16.flag_dark.png").ToString());
                            }
                            else
                            {
                                lvCSSchedule.SelectedItem.SubItems[3].Text = String.Empty;
                                lvCSSchedule.SelectedItem.SubItems.Remove(lvCSSchedule.SelectedItem.SubItems[4]);
                            }
                        }
                        else
                        {
                            lvCSSchedule.SelectedItem.Remove();
                        }
                        break;
                }
            }
        }

        void ansToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString().ToLower())
                {
                    case "save":
                        MessageBox.Show("Save Item?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSave_Click));
                        break;
                    case "save & close":
                        MessageBox.Show("Save Item And Close?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveClose_Click));
                        break;
                    case "save & dup":
                        MessageBox.Show("Save Item And Dup?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveDup_Click));
                        break;
                    case "save & new":
                        MessageBox.Show("Save Item And New?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdSaveNew_Click));
                        break;
                    case "delete":
                        MessageBox.Show("Delete Item?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, new EventHandler(cmdDelete_Click));
                        break;
                }
            }
        }
        #endregion

        #region ShowItem(), SaveItem(), VerifyItem(), DeleteItem()
        private void ShowOrder()
        {
            xPort5.EF6.OrderPC order = xPort5.EF6.OrderPC.Load(orderPCId);
            if (order != null)
            {
                txtPCNumber.Text = order.PCNumber;
                PCNumber = order.PCNumber;
                //string sql = string.Format("OrderPCId = '{0}'", OrderPCId);
                OrderPCItems pcItems = OrderPCItems.Load(orderPCItemsId);
                if (pcItems != null)
                {
                    OrderSCItems scItems = OrderSCItems.Load(pcItems.OrderSCItemsId);
                    if (scItems != null)
                    {
                        _QtItemId = scItems.OrderQTItemId;
                    }
                }
            }
        }

        
        private void ShowItem()
        {
            //            string sql = @"
            //SELECT TOP 100 PERCENT
            //        [OrderPCItemsId], 
            //        [PCNumber], 
            //        [ArticleCode],
            //        [CurrencyCode], 
            //        [FCLCost], 
            //        [LCLCost], 
            //        [UnitCost], 
            //        [ShippingMark], 
            //        [DateShipped], 
            //        [QtyOrdered], 
            //        [ShippedOn], 
            //        [CustSchedule]
            //FROM vwPurchaseContractItemList 
            //WHERE OrderPCItemsId ='" + orderPCItemsId + @"'
            // AND OrderQTCustShippingId = '" + OrderQTCustShippingId + @"'
            // AND OrderQTSuppShippingId = '" + OrderQTSuppShippingId + "'";

            //            SqlCommand cmd = new SqlCommand();
            //            cmd.CommandText = sql;
            //            cmd.CommandTimeout = Common.Config.CommandTimeOut;
            //            cmd.CommandType = CommandType.Text;

            //            SqlDataReader reader = SqlHelper.ExecuteReader(cmd);

            //            int i = 0;
            //            while (reader.Read())
            //            {
            //                txtProductCode.Text = reader.GetString(2);
            //                txtShippingMark.Text = reader.GetString(7);
            //                txtCurrency.Text = reader.GetString(3);
            //                txtFCLCost.Text = "$" + reader.GetDecimal(4).ToString("##0.0000");
            //                txtLCLCost.Text = "$" + reader.GetDecimal(5).ToString("##0.0000");
            //                txtUnitCost.Text = "$" + reader.GetDecimal(6).ToString("##0.0000");

            //                i++;
            //            }

            if (_QtItemId != null && _QtItemId != System.Guid.Empty)
            {
                
                OrderQTItems qtItem = OrderQTItems.Load(_QtItemId);
                if (qtItem != null)
                {
                    articleId = qtItem.ArticleId;
                    txtShippingMark.Text = qtItem.ShippingMark;

                    Article article = Article.Load(qtItem.ArticleId);
                    if (article != null)
                    {
                        txtProductCode.Text = article.ArticleCode;
                        productImage2.ImageName = xPort5.Controls.Utility.Resources.PictureFilePath(article.ArticleId, xPort5.Controls.Utility.Product.KeyPicture(article.ArticleId));

                        string suppSql = string.Format("ArticleId ='{0}'", qtItem.ArticleId);
                        ArticleSupplier articleSupp = ArticleSupplier.LoadWhere(suppSql);
                        if (articleSupp != null)
                        {
                            articleSupplierId = articleSupp.ArticleSupplierId;

                            txtFCLCost.Text = "$" + articleSupp.FCLCost.ToString("##0.0000");
                            txtLCLCost.Text = "$" + articleSupp.LCLCost.ToString("##0.0000");
                            txtUnitCost.Text = "$" + articleSupp.UnitCost.ToString("##0.0000");

                            if (articleSupp.CurrencyId.HasValue)
                            {
                                T_Currency curr = T_Currency.Load(articleSupp.CurrencyId.Value);
                                if (curr != null)
                                {
                                    currencyId = curr.CurrencyId;
                                    txtCurrency.Text = curr.CurrencyCode;
                                }
                            }
                        }
                    }
                }
            }

            BindShippingList();
        }

        private void BindShippingList()
        {
            string suppSql = string.Format("OrderQTItemId = '{0}'", _QtItemId);
            OrderQTSuppShippingCollection suppList = OrderQTSuppShipping.LoadCollection(suppSql);

            this.lvSSSchedule.Items.Clear();
            foreach (OrderQTSuppShipping supp in suppList)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.SubItems.Add(supp.OrderQTSuppShippingId.ToString());
                lvItem.SubItems.Add(supp.DateShipped.HasValue ? supp.DateShipped.Value.ToString("yyyy-MM-dd") : "");
                lvItem.SubItems.Add(supp.QtyOrdered.ToString("n0"));
                lvItem.SubItems.Add("");
                lvSSSchedule.Items.Add(lvItem);
            }


            string custSql = string.Format("OrderQTItemId = '{0}'", _QtItemId);
            OrderQTCustShippingCollection custList = OrderQTCustShipping.LoadCollection(custSql);

            this.lvCSSchedule.Items.Clear();
            foreach (OrderQTCustShipping cust in custList)
            {
                ListViewItem lvItem2 = new ListViewItem();
                lvItem2.SubItems.Add(cust.OrderQTCustShippingId.ToString());
                lvItem2.SubItems.Add(cust.ShippedOn.HasValue ? cust.ShippedOn.Value.ToString("yyyy-MM-dd") : "");
                lvItem2.SubItems.Add(cust.QtyOrdered.ToString("n0"));
                lvItem2.SubItems.Add("");
                lvCSSchedule.Items.Add(lvItem2);
            }

        }

        private bool SaveItem()
        {
            bool result = false;
            if (VerifyItem())
            {
                try
                {
                    OrderQTItems qtItem = null;
                    Article article = null;
                    ArticleSupplier artSupp = null;
                    T_Currency curr = null;
                    switch((int)editMode)
                    {
                        case (int)Common.Enums.EditMode.Add:
                            break;
                        case (int)Common.Enums.EditMode.Edit:
                            qtItem = OrderQTItems.Load(_QtItemId);
                            //article = Article.Load(articleId);
                            artSupp = ArticleSupplier.Load(articleSupplierId);
                            curr = T_Currency.Load(currencyId);
                            break;
                    }

                    qtItem.ShippingMark = this.txtShippingMark.Text;
                    qtItem.Save();

                    #region log activity
                    if (editMode == Common.Enums.EditMode.Add)
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Create, qtItem.ToString());
                    else
                        xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, qtItem.ToString());
                    #endregion

                    artSupp.FCLCost = Convert.ToDecimal(this.txtFCLCost.Text.Replace("$",""));
                    artSupp.LCLCost = Convert.ToDecimal(this.txtLCLCost.Text.Replace("$", ""));
                    artSupp.UnitCost = Convert.ToDecimal(this.txtUnitCost.Text.Replace("$", ""));
                    artSupp.Save();

                    curr.CurrencyCode = this.txtCurrency.Text;
                    curr.Save();

                    SaveShippingInfo();

                    result = true;
                }
                catch
                {
                    
                }
            }

            return result;
        }

        private void SaveShippingInfo()
        {
            foreach (ListViewItem lvItems in lvSSSchedule.Items)
            {
                if (lvItems.SubItems[0].Text != null)
                {
                    string SuppShippingId = lvItems.SubItems[0].Text;
                    decimal qty = 0;
                    decimal.TryParse(lvItems.SubItems[2].Text, out qty);
                    DateTime date = Convert.ToDateTime(lvItems.SubItems[1].Text);
                    if (qty >= 0)
                    {
                        OrderQTSuppShipping suppShipping = OrderQTSuppShipping.Load(new Guid(SuppShippingId));
                        if (suppShipping == null)
                        {
                            suppShipping = new OrderQTSuppShipping();
                            suppShipping.OrderQTItemId = _QtItemId;
                        }
                        suppShipping.DateShipped = date;
                        suppShipping.QtyOrdered = qty;

                        if (lvItems.SubItems[3].Text == "D")
                        {
                            suppShipping.Delete();
                            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, suppShipping.ToString());
                        }
                        else
                        {
                            suppShipping.Save();
                            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, suppShipping.ToString());
                        }

                    }
                }
            }

            foreach (ListViewItem lvItemc in lvCSSchedule.Items)
            {
                if (lvItemc.SubItems[0].Text != null)
                {
                    string custShippingId = lvItemc.SubItems[0].Text;
                    decimal qty = 0;
                    decimal.TryParse(lvItemc.SubItems[2].Text, out qty);
                    DateTime date = Convert.ToDateTime(lvItemc.SubItems[1].Text);


                    if (qty >= 0)
                    {
                        OrderQTCustShipping custShipping = OrderQTCustShipping.Load(new Guid(custShippingId));
                        if (custShipping == null)
                        {
                            custShipping = new OrderQTCustShipping();
                            custShipping.OrderQTItemId = _QtItemId;
                        }
                        custShipping.QtyOrdered = qty;
                        custShipping.ShippedOn = date;

                        if (lvItemc.SubItems[3].Text == "D")
                        {
                            custShipping.Delete();
                            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, custShipping.ToString());
                        }
                        else
                        {
                            custShipping.Save();
                            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Update, custShipping.ToString());
                        }

                    }
                }
            }
        }

        private bool VerifyItem()
        {
            bool result = true;
            string errMsg = string.Empty;

            decimal custShippingQty = 0;
            foreach (ListViewItem lvItem in lvCSSchedule.Items)
            {
                if (lvItem.SubItems[0].Text != null)
                {
                    decimal tmpQty = 0;
                    decimal.TryParse(lvItem.SubItems[2].Text, out tmpQty);

                    custShippingQty += tmpQty;
                }
            }

            // Supplier Shipping Info
            decimal suppShippingQty = 0;
            foreach (ListViewItem lvItem in lvSSSchedule.Items)
            {
                if (lvItem.SubItems[0].Text != null)
                {
                    decimal tmpQty = 0;
                    decimal.TryParse(lvItem.SubItems[2].Text, out tmpQty);

                    suppShippingQty += tmpQty;
                }
            }

            if (custShippingQty != suppShippingQty)
            {
                errMsg += Environment.NewLine + " Qty of customer shipping schedule does not equal to the Qty fo supplier shipping schedule.";
                result = false;
            }

            if (!(result))
            {
                MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        #endregion

        #region ans Button Clicks: Save, SaveClose, Delete
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Sales Contract {0} Details is saved!", PCNumber), "Save Result");
                        if (editMode == Common.Enums.EditMode.Add)
                        {
                            editMode = Common.Enums.EditMode.Edit;

                            this.Update();
                        }

                        this.Init();
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is ReadOnly...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdSaveClose_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Sales Contract {0} Details is saved!", PCNumber), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is Read Only...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdSaveDup_Click(object sender, EventArgs e)
        {

        }

        private void cmdSaveNew_Click(object sender, EventArgs e)
        {

        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {

        }

        private void cmdCloseForm(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void cboShippingMark_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;

            try
            {
                T_ShippingMark shippingMark = T_ShippingMark.Load((Guid)s.SelectedValue);
                if (shippingMark != null)
                {
                    txtShippingMark.Text = shippingMark.ShippingMarkName;
                }
            }
            catch { }
        }

        private void dgvSSSchedule_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[0].Value = DateTime.Now.ToString(Common.DateTimeHelper.GetDateFormat());
        }

        private void dgvCSSchedule_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[0].Value = DateTime.Now.ToString(Common.DateTimeHelper.GetDateFormat());
        }

        

        void supp_Closed(object sender, EventArgs e)
        {

            if (sender is AddSuppShipping)
            {
                AddSuppShipping supp = sender as AddSuppShipping;
                if (supp != null)
                {
                    if (supp.IsOkay)
                    {
                        if (sbl)
                        {
                            lvSSSchedule.SelectedItem.SubItems[2].Text = (Convert.ToDecimal(supp.Qty)).ToString("n0");
                            lvSSSchedule.SelectedItem.SubItems[1].Text = supp.Date;
                        }
                        else
                        {
                            ListViewItem lvItem = new ListViewItem();
                            lvItem.SubItems.Add(new Guid().ToString());
                            lvItem.SubItems.Add(supp.Date);
                            lvItem.SubItems.Add((Convert.ToDecimal(supp.Qty)).ToString("n0"));
                            lvItem.SubItems.Add("");
                            lvSSSchedule.Items.Add(lvItem);
                        }
                    }
                }
            }

        }

        void cust_Closed(object sender, EventArgs e)
        {
            if (sender is AddCustShipping)
            {
                AddCustShipping cust = sender as AddCustShipping;
                if (cust != null)
                {
                    if (cust.IsKay)
                    {
                        if (cbl)
                        {
                            lvCSSchedule.SelectedItem.SubItems[1].Text = cust.Date;
                            lvCSSchedule.SelectedItem.SubItems[2].Text = (Convert.ToDecimal(cust.Qty)).ToString("n0");
                        }
                        else
                        {
                            ListViewItem lvItem = new ListViewItem();
                            lvItem.SubItems.Add(new Guid().ToString());
                            lvItem.SubItems.Add(cust.Date);
                            lvItem.SubItems.Add(Convert.ToDecimal(cust.Qty).ToString("n0"));
                            lvItem.SubItems.Add("");
                            lvCSSchedule.Items.Add(lvItem);
                        }
                    }
                }
            }
        }

        private void lvSSSchedule_DoubleClick(object sender, EventArgs e)
        {
            if (lvSSSchedule.SelectedItem != null)
            {
                if (lvSSSchedule.SelectedItem.SubItems[3].Text == "D")
                {
                    lvSSSchedule.SelectedItem.SubItems[3].Text = "";
                }

                sbl = true;
                AddSuppShipping supp = new AddSuppShipping();
                supp.Qty = lvSSSchedule.SelectedItem.SubItems[2].Text.Replace(",", "");
                supp.Date = lvSSSchedule.SelectedItem.SubItems[1].Text;
                supp.Closed += new EventHandler(supp_Closed);
                supp.ShowDialog();
            }
        }

        private void lvCSSchedule_DoubleClick(object sender, EventArgs e)
        {
            if (lvCSSchedule.SelectedItem != null)
            {
                if (lvCSSchedule.SelectedItem.SubItems[3].Text == "D")
                {
                    lvCSSchedule.SelectedItem.SubItems[3].Text = "";
                }

                cbl = true;
                AddCustShipping cust = new AddCustShipping();
                cust.Date = lvCSSchedule.SelectedItem.SubItems[1].Text;
                cust.Qty = lvCSSchedule.SelectedItem.SubItems[2].Text.Replace(",", "");
                cust.Closed += new EventHandler(cust_Closed);
                cust.ShowDialog();
            }
        }
    }
}
