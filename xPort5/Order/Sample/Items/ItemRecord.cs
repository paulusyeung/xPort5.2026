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
using Gizmox.WebGUI.Common.Resources;
using System.Data.SqlClient;

#endregion

namespace xPort5.Order.Sample.Items
{
    public partial class ItemRecord : Form
    {
        private Guid _SampleId = System.Guid.Empty;
        private Common.Enums.EditMode _EditMode = Common.Enums.EditMode.Read;
        private Guid _SampleItemId = System.Guid.Empty;
        private string _SPNumber = string.Empty;
        private string errMsg = String.Empty;
        private decimal TotalQty = 0;

        public int NextLineNumber { get; set; }
        public string ExceptQTNumber { get; set; }

        #region Public Properties
        public Guid SampleId
        {
            get
            {
                return _SampleId;
            }
            set
            {
                _SampleId = value;
            }
        }

        public Guid SampleItemId
        {
            get
            {
                return _SampleItemId;
            }
            set
            {
                _SampleItemId = value;
            }
        }

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

        public string SPNumber
        {
            get
            {
                return _SPNumber;
            }
            set
            {
                _SPNumber = value;
            }
        }
        #endregion

        public ItemRecord()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.SetAttributes();
            this.SetListAns();
            this.SetCombo();
        }

        private void SetAttributes()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.Text = string.Format(oDict.GetWord("record"), oDict.GetWord("item"));

            this.lblLineNumber.Text = oDict.GetWordWithColon("line_no");
            this.lblArticleCode.Text = oDict.GetWordWithColon("product_code");
            this.lblQty.Text = oDict.GetWordWithColon("qty");
            this.lblUoM.Text = oDict.GetWordWithColon("unit");

            this.colQuotedDate.Text = oDict.GetWord("qt_date");
            this.colQTNumber.Text = oDict.GetWord("qt_number");
            this.colSupplierCode.Text = string.Format(oDict.GetWord("code_replace"), oDict.GetWord("supplier"));
            this.colSupplierName.Text = string.Format(oDict.GetWord("name4others_replace"), oDict.GetWord("supplier"));
            this.colPackageCode.Text = string.Format(oDict.GetWord("code_replace"), oDict.GetWord("package"));
            this.colPackageName.Text = string.Format(oDict.GetWord("name4others_replace"), oDict.GetWord("package"));
            this.colOutStandingSample.Text = oDict.GetWord("ost_sample");

            this.txtLineNumber.Text = this.NextLineNumber.ToString();
        }

        private void SetListAns()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            this.ansToolbar.Buttons.Clear();
            this.ansToolbar.ButtonClick -= new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);

            this.ansToolbar.MenuHandle = false;
            this.ansToolbar.DragHandle = false;
            this.ansToolbar.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            // cmdSave
            ToolBarButton cmdSave = new ToolBarButton("Save", oDict.GetWord("Save"));
            cmdSave.Tag = "Save";
            cmdSave.Image = new IconResourceHandle("16x16.16_L_save.gif");

            // cmdSaveClose
            ToolBarButton cmdSaveClose = new ToolBarButton("Save & Close", oDict.GetWord("save_close").Replace("%26", "&"));
            cmdSaveClose.Tag = "Save & Close";
            cmdSaveClose.Image = new IconResourceHandle("16x16.16_saveClose.gif");

            if (_EditMode != Common.Enums.EditMode.Read)
            {
                if (xPort5.Common.Config.UseNetSqlAzMan)
                {
                    if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.Sample.Create") ||
                        xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized("Order", "Order.Sample.Update"))
                    {
                        this.ansToolbar.Buttons.Add(cmdSaveClose);
                    }
                }
                else
                {
                    this.ansToolbar.Buttons.Add(cmdSaveClose);
                }
            }

            this.ansToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ansToolbar_ButtonClick);
        }

        private void SetCombo()
        {
            T_UnitOfMeasures.LoadCombo(ref cboUoM, "UomName", false);
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
                }
            }
        }

        #region Save Item

        private bool SaveItem()
        {
            bool result = false;

            if (VerifyItem())
            {
                if (lvwItems.SelectedItem != null && Common.Utility.IsGUID(lvwItems.SelectedItem.Text))
                {
                    System.Guid orderQTItemId = new Guid(lvwItems.SelectedItem.Text);

                    if (this.EditMode == Common.Enums.EditMode.Add)
                    {
                        OrderSPItems spItem = new OrderSPItems();
                        spItem.OrderSPId = this.SampleId;
                        spItem.LineNumber = this.NextLineNumber;
                        spItem.OrderQTItemId = orderQTItemId;
                        spItem.Qty = Convert.ToDecimal(txtQty.Text.Trim());
                        spItem.Unit = cboUoM.Text;
                        spItem.Save();

                        result = true;
                    }
                }
            }

            return result;
        }

        private bool VerifyItem()
        {
            bool result = false;

            if (_EditMode == Common.Enums.EditMode.Add)
            {
                #region validate for outstanding Article sample requirement
                if (txtArticleCode.Text.Trim() != String.Empty)
                {
                    OrderSP item = OrderSP.Load(this._SampleId);
                    if (item != null)
                    {
                        Guid customerId = item.CustomerId;
                        if (CheckForOutstandingSamle(customerId))
                        {
                            result = true;
                        }
                        else
                        {
                            errMsg += Environment.NewLine + "This customer has no outstanding Sample required for Article Code " + txtArticleCode.Text + ".";
                            result = false;
                        }
                    }
                }
                else
                {
                    errMsg += Environment.NewLine + "Article Code cannot be blank.";
                    result = false;
                }
                #endregion

                #region validate Qty
                if (txtQty.Text.Trim() != String.Empty)
                {
                    try
                    {
                        Decimal credit = Convert.ToDecimal(txtQty.Text.Trim());
                    }
                    catch
                    {
                        errMsg += Environment.NewLine + "Qty must be decimal.";
                        result = false;
                    }
                }
                else
                {
                    errMsg += Environment.NewLine + "Qty cannot be blank.";
                    result = false;
                }
                #endregion
            }

            if (!(result))
            {
                MessageBox.Show(errMsg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        private bool CheckForOutstandingSamle(Guid customerId)
        {
            bool result = false;

            string sql = @"
SELECT [SampleQty]
  FROM [dbo].[vwOSSample]
WHERE [CustomerId] = '{0}' AND ArticleCode = '{1}'";

            sql = string.Format(sql, customerId.ToString(), this.txtArticleCode.Text.Trim());

            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
            while (reader.Read())
            {
                if (reader.GetDecimal(0) > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region ans Button Clicks: Save, SaveClose
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (((Form)sender).DialogResult == DialogResult.Yes)
            {
                if (!this.Text.Contains("ReadOnly"))
                {
                    if (SaveItem())
                    {
                        MessageBox.Show(String.Format("Sample {0} Details is saved!", _SPNumber), "Save Result");
                        if (_EditMode == Common.Enums.EditMode.Add)
                        {
                            _EditMode = Common.Enums.EditMode.Edit;
                            this.Update();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes." + Environment.NewLine + errMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show(String.Format("Sample {0} Details is saved!", _SPNumber), "Save Result", MessageBoxButtons.OK, MessageBoxIcon.Information, new EventHandler(cmdCloseForm));
                    }
                    else
                    {
                        MessageBox.Show("Error found...Job aborted!\nPlease review your changes." + Environment.NewLine + errMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Record is Read Only...Job aborted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cmdCloseForm(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void txtArticleCode_TextChanged(object sender, EventArgs e)
        {
            string sql = @"
SELECT [OrderQTItemId]
      ,[QTDate]
      ,[QTNumber]
      ,[SupplierCode]
      ,[SupplierName]
      ,[PackageCode]
      ,[PackageName]
      ,[SampleQty]
  FROM [dbo].[vwOSSample]
WHERE [CustomerId] = '{0}' AND [ArticleCode] = '{1}' AND [QTNumber] NOT IN ({2})";

            if (txtArticleCode.Text.Trim() != String.Empty)
            {
                OrderSP item = OrderSP.Load(this._SampleId);
                if (item != null)
                {
                    var customerIdValue = item.CustomerId;
                    Guid customerId = customerIdValue;
                    if (customerId == Guid.Empty)
                        return;
                    
                    string customerIdStr = customerId.ToString();
                    if (CheckForOutstandingSamle(customerId))
                    {
                        lvwItems.Items.Clear();
                        int iCount = 0;
                        sql = string.Format(sql, customerIdStr, this.txtArticleCode.Text.Trim(), this.ExceptQTNumber);

                        SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
                        while (reader.Read())
                        {
                            if (reader.GetDecimal(7) > 0)
                            {
                                ListViewItem lvItem = lvwItems.Items.Add(reader.GetGuid(0).ToString());
                                lvItem.SubItems.Add(Common.DateTimeHelper.DateTimeToString(reader.GetDateTime(1), false));
                                lvItem.SubItems.Add(reader.GetString(2));
                                lvItem.SubItems.Add(reader.GetString(3));
                                lvItem.SubItems.Add(reader.GetString(4));
                                lvItem.SubItems.Add(reader.GetString(5));
                                lvItem.SubItems.Add(reader.GetString(6));
                                lvItem.SubItems.Add(reader.GetDecimal(7).ToString("N0"));

                                iCount++;
                            }

                            TotalQty += reader.GetDecimal(7);
                        }

                        lvwItems.SelectedIndex = 0;

                        if (iCount > 1)
                        {
                            string msg = "There are {0} Quotations with outstanding Sample for this Customer." + Environment.NewLine + Environment.NewLine + "Please make your selection if the default is not what you want.";
                            MessageBox.Show(string.Format(msg, iCount.ToString()), "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("This customer has no outstanding Sample required for Article Code " + txtArticleCode.Text + ".", "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            decimal qty = Convert.ToDecimal(txtQty.Text.Trim());

            if (qty > this.TotalQty)
            {
                string msg = "You've enter more Qty than required:" + Environment.NewLine;
                msg += string.Format("     Qty Entered = {0}", qty.ToString("#,###0.00")) + Environment.NewLine;
                msg += string.Format("     Qty Outstanding = {0},", qty.ToString("#,###")) + Environment.NewLine;
                msg += "you should double check your entries.";

                MessageBox.Show(msg, "Error found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
