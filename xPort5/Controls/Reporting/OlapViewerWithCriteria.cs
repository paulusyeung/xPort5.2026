#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using xPort5.DAL;
using System.Web;
using System.Configuration;
using Gizmox.WebGUI.Common.Resources;

#endregion

namespace xPort5.Controls.Reporting
{
    public partial class OlapViewerWithCriteria : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapViewerWithCriteria"/> class.
        /// </summary>
        public OlapViewerWithCriteria()
        {
            InitializeComponent();

            tvList.BeforeCheck += new TreeViewCancelEventHandler(tvList_BeforeCheck);
        }

        private string aspxPagePath = string.Empty;
        private HashSet<Guid> selectedList = new HashSet<Guid>();
        private bool showPeriodTabPage = true;
        private bool withCurrencyList = false;
        private string treeviewData = "Customer";

        #region Properties

        /// <summary>
        /// Gets or sets the aspx page path.
        /// </summary>
        /// <value>The aspx page path.</value>
        public string AspxPagePath
        {
            get
            {
                return aspxPagePath;
            }
            set
            {
                aspxPagePath = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show period tab page].
        /// </summary>
        /// <value><c>true</c> if [show period tab page]; otherwise, <c>false</c>.</value>
        public bool ShowPeriodTabPage
        {
            get
            {
                return showPeriodTabPage;
            }
            set
            {
                showPeriodTabPage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [with currency list].
        /// </summary>
        /// <value><c>true</c> if [with currency list]; otherwise, <c>false</c>.</value>
        public bool WithCurrencyList
        {
            get
            {
                return withCurrencyList;
            }
            set
            {
                withCurrencyList = value;
            }
        }

        /// <summary>
        /// Gets or sets the treeview data.
        /// </summary>
        /// <value>The treeview data.</value>
        public string TreeviewData
        {
            get
            {
                return treeviewData;
            }
            set
            {
                treeviewData = value;
            }
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="E:Gizmox.WebGUI.Forms.UserControl.Load"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.SetAttributes();
            this.SetCaptions();

            this.LoadTreeViewControl(true);
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

            this.tpOption.Text = oDict.GetWord("option");
            this.gbSelectionBy.Text = oDict.GetWord("select_by");
            rbtnCode.Text = oDict.GetWord("customer_code");
            rbtnName.Text = oDict.GetWord("customer_name");
            this.tpPeriod.Text = oDict.GetWord("date_range");
            gbReportingPeriod.Text = oDict.GetWord("date_range");
            this.lblFrom.Text = oDict.GetWordWithColon("from");
            this.lblTo.Text = oDict.GetWordWithColon("to");
            this.lblCurrency.Text = oDict.GetWordWithColon("currency");
            this.btnSelectionAll.Text = oDict.GetWord("all");
            this.btnClearSelection.Text = oDict.GetWord("clear");
        }

        /// <summary>
        /// Sets the attributes.
        /// </summary>
        private void SetAttributes()
        {
            rbtnCode.Text = this.TreeviewData + " Code";
            rbtnName.Text = this.TreeviewData + " Name";

            // Tab Control
            if (this.ShowPeriodTabPage)
            {
                dtpFrom.Value = new DateTime(DateTime.Now.Year, 1, 1);
                tabOptions.SelectedIndex = 1;

                // Set the date period by selected value
                string[] period = xPort5.Controls.Utility.OlapAdmin.DatePeriod.Split(',');

                if (period.Length > 1)
                {
                    dtpFrom.Value = Convert.ToDateTime(period[0]);
                    dtpTo.Value = Convert.ToDateTime(period[1]);
                }

                // Show Currency List
                if (this.WithCurrencyList)
                {
                    lblCurrency.Visible = this.WithCurrencyList;
                    cboCurrency.Visible = this.WithCurrencyList;

                    lblTo.Visible = !this.WithCurrencyList;
                    dtpTo.Visible = !this.WithCurrencyList;

                    // Bind Currency list
                    T_Currency.LoadCombo(ref cboCurrency, "CurrencyCode", false);

                    if (xPort5.Controls.Utility.OlapAdmin.SelectedCurrency.Length > 0)
                    {
                        cboCurrency.Text = xPort5.Controls.Utility.OlapAdmin.SelectedCurrency;
                    }
                    else
                    {
                        cboCurrency.Text = ConfigurationManager.AppSettings["Currency"];
                    }
                }
            }
            else
            {
                tpPeriod.Visible = false;
            }

            // Treeview Control
            tvList.CheckBoxes = true;

            splitContainer.SplitterDistance = 210;
            splitContainer.SplitterWidth = 2;
        }

        /// <summary>
        /// Loads the tree view control.
        /// </summary>
        /// <param name="byNameOrCode">if set to <c>true</c> [by name] else [by code].</param>
        private void LoadTreeViewControl(bool byNameOrCode)
        {
            switch (this.TreeviewData.Trim().ToLower())
            {
                case "customer":
                    xPort5.Controls.Utility.TreeViewControl.Load<Customer>(tvList.Nodes, byNameOrCode);
                    break;
                case "supplier":
                    xPort5.Controls.Utility.TreeViewControl.Load<Supplier>(tvList.Nodes, byNameOrCode);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btnCtrl = sender as Button;
                if (btnCtrl != null)
                {
                    switch (btnCtrl.Name.ToLower())
                    {
                        case "btnselectionall":
                            SelectAllNodes();
                            break;
                        case "btnclearselection":
                            ClearTreeNodeSelection();
                            break;
                        case "btnok":
                            LoadOlapViewer();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the RadioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
                        case "rbtncode":
                            LoadTreeViewControl(false);
                            break;
                        case "rbtnname":
                            LoadTreeViewControl(true);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the olap viewer.
        /// </summary>
        private void LoadOlapViewer()
        {
            GetSelectedNodes();
            viewerPane.Controls.Clear();

            if (selectedList.Count > 0)
            {
                xPort5.Controls.Utility.OlapAdmin.DatePeriod = dtpFrom.Value.ToString("yyyy-MM-dd") + "," + dtpTo.Value.ToString("yyyy-MM-dd");
                VWGContext.Current.Session["CustomerList"] = selectedList;

                if (this.WithCurrencyList)
                {
                    xPort5.Controls.Utility.OlapAdmin.SelectedCurrency = cboCurrency.Text;
                }

                OlapViewer olapViewer = new OlapViewer();
                olapViewer.SetLoadingMessage("Loading...");
                olapViewer.AspxPagePath = this.AspxPagePath;
                olapViewer.Dock = DockStyle.Fill;
                viewerPane.Controls.Add(olapViewer);
            }
            else
            {
                MessageBox.Show("No Selected Item!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Selects all nodes.
        /// </summary>
        private void SelectAllNodes()
        {
            foreach (TreeNode node in tvList.Nodes)
            {
                if (node.HasNodes)
                {
                    node.ExpandAll();
                    node.Checked = true;

                    foreach (TreeNode endNode in node.Nodes)
                    {
                        endNode.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected nodes.
        /// </summary>
        private void GetSelectedNodes()
        {
            selectedList.Clear();

            foreach (TreeNode node in tvList.Nodes)
            {
                if (node.HasNodes)
                {
                    foreach (TreeNode endNode in node.Nodes)
                    {
                        if (endNode.Checked)
                        {
                            if (Common.Utility.IsGUID(endNode.Tag.ToString()))
                            {
                                selectedList.Add(new Guid(endNode.Tag.ToString()));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the tree node selection.
        /// </summary>
        private void ClearTreeNodeSelection()
        {
            foreach (TreeNode node in tvList.Nodes)
            {
                if (node.HasNodes)
                {
                    node.Collapse();
                    node.Checked = false;

                    foreach (TreeNode endNode in node.Nodes)
                    {
                        endNode.Checked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the tree node selection.
        /// </summary>
        /// <param name="node">The node.</param>
        private void SetTreeNodeSelection(TreeNode node)
        {
            if (!node.HasNodes)
            {
                if (node.Tag != null)
                {
                    if (Common.Utility.IsGUID(node.Tag.ToString()))
                    {
                        node.Checked = !node.Checked;
                    }
                }
            }
            else
            {
                node.Expand();

                foreach (TreeNode endNode in node.Nodes)
                {
                    endNode.Checked = !node.Checked;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the tvList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tvList_Click(object sender, EventArgs e)
        {
            if (tvList.SelectedNode != null)
            {
                SetTreeNodeSelection(tvList.SelectedNode);
                if (tvList.SelectedNode.HasNodes)
                {
                    tvList.SelectedNode.Checked = !tvList.SelectedNode.Checked;
                }
            }
        }

        /// <summary>
        /// Handles the BeforeCheck event of the tvList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Gizmox.WebGUI.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
        void tvList_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            /*
             * 2020.03.05 paulus: Avoid deadloop
             * https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.beforecheck?view=netframework-4.8
             */
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node != null)
                {
                    SetTreeNodeSelection(e.Node);
                }
            }
        }
    }
}