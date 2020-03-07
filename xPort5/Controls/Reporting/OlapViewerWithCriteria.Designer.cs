namespace xPort5.Controls.Reporting
{
    partial class OlapViewerWithCriteria
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Visual WebGui UserControl Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new Gizmox.WebGUI.Forms.SplitContainer();
            this.panelTree = new Gizmox.WebGUI.Forms.Panel();
            this.tvList = new Gizmox.WebGUI.Forms.TreeView();
            this.panelTabs = new Gizmox.WebGUI.Forms.Panel();
            this.btnOK = new Gizmox.WebGUI.Forms.Button();
            this.btnClearSelection = new Gizmox.WebGUI.Forms.Button();
            this.tabOptions = new Gizmox.WebGUI.Forms.TabControl();
            this.tpOption = new Gizmox.WebGUI.Forms.TabPage();
            this.gbSelectionBy = new Gizmox.WebGUI.Forms.GroupBox();
            this.rbtnCode = new Gizmox.WebGUI.Forms.RadioButton();
            this.rbtnName = new Gizmox.WebGUI.Forms.RadioButton();
            this.tpPeriod = new Gizmox.WebGUI.Forms.TabPage();
            this.gbReportingPeriod = new Gizmox.WebGUI.Forms.GroupBox();
            this.cboCurrency = new Gizmox.WebGUI.Forms.ComboBox();
            this.dtpTo = new Gizmox.WebGUI.Forms.DateTimePicker();
            this.lblCurrency = new Gizmox.WebGUI.Forms.Label();
            this.lblTo = new Gizmox.WebGUI.Forms.Label();
            this.dtpFrom = new Gizmox.WebGUI.Forms.DateTimePicker();
            this.lblFrom = new Gizmox.WebGUI.Forms.Label();
            this.btnSelectionAll = new Gizmox.WebGUI.Forms.Button();
            this.viewerPane = new Gizmox.WebGUI.Forms.Panel();
            this.panelTree.SuspendLayout();
            this.panelTabs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabOptions)).BeginInit();
            this.tabOptions.SuspendLayout();
            this.tpOption.SuspendLayout();
            this.gbSelectionBy.SuspendLayout();
            this.tpPeriod.SuspendLayout();
            this.gbReportingPeriod.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.AutoValidate = Gizmox.WebGUI.Forms.AutoValidate.EnablePreventFocusChange;
            this.splitContainer.BorderStyle = Gizmox.WebGUI.Forms.BorderStyle.Clear;
            this.splitContainer.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panelTree);
            this.splitContainer.Panel1.Controls.Add(this.panelTabs);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.viewerPane);
            this.splitContainer.Size = new System.Drawing.Size(800, 600);
            this.splitContainer.SplitterDistance = 210;
            this.splitContainer.TabIndex = 0;
            // 
            // panelTree
            // 
            this.panelTree.Controls.Add(this.tvList);
            this.panelTree.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.panelTree.Location = new System.Drawing.Point(0, 170);
            this.panelTree.Name = "panelTree";
            this.panelTree.Size = new System.Drawing.Size(210, 430);
            this.panelTree.TabIndex = 1;
            // 
            // tvList
            // 
            this.tvList.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tvList.Location = new System.Drawing.Point(0, 0);
            this.tvList.Name = "tvList";
            this.tvList.Size = new System.Drawing.Size(210, 430);
            this.tvList.TabIndex = 1;
            this.tvList.Click += new System.EventHandler(this.tvList_Click);
            // 
            // panelTabs
            // 
            this.panelTabs.Controls.Add(this.btnOK);
            this.panelTabs.Controls.Add(this.btnClearSelection);
            this.panelTabs.Controls.Add(this.tabOptions);
            this.panelTabs.Controls.Add(this.btnSelectionAll);
            this.panelTabs.Dock = Gizmox.WebGUI.Forms.DockStyle.Top;
            this.panelTabs.Location = new System.Drawing.Point(0, 0);
            this.panelTabs.Name = "panelTabs";
            this.panelTabs.Size = new System.Drawing.Size(210, 170);
            this.panelTabs.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(140, 138);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(45, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnClearSelection
            // 
            this.btnClearSelection.Location = new System.Drawing.Point(80, 138);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(45, 23);
            this.btnClearSelection.TabIndex = 2;
            this.btnClearSelection.Text = "Clear";
            this.btnClearSelection.Click += new System.EventHandler(this.Button_Click);
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.tpOption);
            this.tabOptions.Controls.Add(this.tpPeriod);
            this.tabOptions.Dock = Gizmox.WebGUI.Forms.DockStyle.Top;
            this.tabOptions.Location = new System.Drawing.Point(0, 0);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.SelectedIndex = 0;
            this.tabOptions.Size = new System.Drawing.Size(210, 132);
            this.tabOptions.TabIndex = 1;
            // 
            // tpOption
            // 
            this.tpOption.Controls.Add(this.gbSelectionBy);
            this.tpOption.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tpOption.Location = new System.Drawing.Point(4, 22);
            this.tpOption.Name = "tpOption";
            this.tpOption.Size = new System.Drawing.Size(202, 106);
            this.tpOption.TabIndex = 0;
            this.tpOption.Text = "Option";
            // 
            // gbSelectionBy
            // 
            this.gbSelectionBy.Controls.Add(this.rbtnCode);
            this.gbSelectionBy.Controls.Add(this.rbtnName);
            this.gbSelectionBy.FlatStyle = Gizmox.WebGUI.Forms.FlatStyle.Flat;
            this.gbSelectionBy.Location = new System.Drawing.Point(10, 10);
            this.gbSelectionBy.Name = "gbSelectionBy";
            this.gbSelectionBy.Size = new System.Drawing.Size(182, 85);
            this.gbSelectionBy.TabIndex = 1;
            this.gbSelectionBy.TabStop = false;
            this.gbSelectionBy.Text = "Selection by";
            // 
            // rbtnCode
            // 
            this.rbtnCode.Location = new System.Drawing.Point(25, 31);
            this.rbtnCode.Name = "rbtnCode";
            this.rbtnCode.Size = new System.Drawing.Size(146, 24);
            this.rbtnCode.TabIndex = 1;
            this.rbtnCode.Text = "Customer Code";
            this.rbtnCode.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // rbtnName
            // 
            this.rbtnName.Checked = true;
            this.rbtnName.Location = new System.Drawing.Point(25, 56);
            this.rbtnName.Name = "rbtnName";
            this.rbtnName.Size = new System.Drawing.Size(146, 24);
            this.rbtnName.TabIndex = 2;
            this.rbtnName.Text = "Customer Name";
            this.rbtnName.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // tpPeriod
            // 
            this.tpPeriod.Controls.Add(this.gbReportingPeriod);
            this.tpPeriod.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tpPeriod.Location = new System.Drawing.Point(4, 22);
            this.tpPeriod.Name = "tpPeriod";
            this.tpPeriod.Size = new System.Drawing.Size(202, 106);
            this.tpPeriod.TabIndex = 0;
            this.tpPeriod.Text = "Period";
            // 
            // gbReportingPeriod
            // 
            this.gbReportingPeriod.Controls.Add(this.cboCurrency);
            this.gbReportingPeriod.Controls.Add(this.dtpTo);
            this.gbReportingPeriod.Controls.Add(this.lblCurrency);
            this.gbReportingPeriod.Controls.Add(this.lblTo);
            this.gbReportingPeriod.Controls.Add(this.dtpFrom);
            this.gbReportingPeriod.Controls.Add(this.lblFrom);
            this.gbReportingPeriod.FlatStyle = Gizmox.WebGUI.Forms.FlatStyle.Flat;
            this.gbReportingPeriod.Location = new System.Drawing.Point(10, 11);
            this.gbReportingPeriod.Name = "gbReportingPeriod";
            this.gbReportingPeriod.Size = new System.Drawing.Size(182, 85);
            this.gbReportingPeriod.TabIndex = 1;
            this.gbReportingPeriod.TabStop = false;
            this.gbReportingPeriod.Text = "Reporting Period";
            // 
            // cboCurrency
            // 
            this.cboCurrency.BorderStyle = Gizmox.WebGUI.Forms.BorderStyle.Fixed3D;
            this.cboCurrency.Location = new System.Drawing.Point(66, 52);
            this.cboCurrency.Name = "cboCurrency";
            this.cboCurrency.Size = new System.Drawing.Size(95, 20);
            this.cboCurrency.TabIndex = 1;
            this.cboCurrency.Visible = false;
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "dd/MM/yyyy";
            this.dtpTo.Format = Gizmox.WebGUI.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(66, 53);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(95, 20);
            this.dtpTo.TabIndex = 2;
            // 
            // lblCurrency
            // 
            this.lblCurrency.Location = new System.Drawing.Point(10, 52);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(52, 20);
            this.lblCurrency.TabIndex = 0;
            this.lblCurrency.Text = "Cny: ";
            this.lblCurrency.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblCurrency.Visible = false;
            // 
            // lblTo
            // 
            this.lblTo.Location = new System.Drawing.Point(10, 52);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(52, 20);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "To: ";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "dd/MM/yyyy";
            this.dtpFrom.Format = Gizmox.WebGUI.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(66, 26);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(95, 20);
            this.dtpFrom.TabIndex = 1;
            // 
            // lblFrom
            // 
            this.lblFrom.Location = new System.Drawing.Point(10, 26);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(52, 20);
            this.lblFrom.TabIndex = 1;
            this.lblFrom.Text = "From: ";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnSelectionAll
            // 
            this.btnSelectionAll.Location = new System.Drawing.Point(21, 138);
            this.btnSelectionAll.Name = "btnSelectionAll";
            this.btnSelectionAll.Size = new System.Drawing.Size(45, 23);
            this.btnSelectionAll.TabIndex = 1;
            this.btnSelectionAll.Text = "All";
            this.btnSelectionAll.Click += new System.EventHandler(this.Button_Click);
            // 
            // viewerPane
            // 
            this.viewerPane.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.viewerPane.Location = new System.Drawing.Point(0, 0);
            this.viewerPane.Name = "viewerPane";
            this.viewerPane.Size = new System.Drawing.Size(586, 600);
            this.viewerPane.TabIndex = 0;
            // 
            // OlapViewerWithCriteria
            // 
            this.Controls.Add(this.splitContainer);
            this.Size = new System.Drawing.Size(800, 600);
            this.Text = "OlapViewerWithCriteria";
            this.panelTree.ResumeLayout(false);
            this.panelTabs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabOptions)).EndInit();
            this.tabOptions.ResumeLayout(false);
            this.tpOption.ResumeLayout(false);
            this.gbSelectionBy.ResumeLayout(false);
            this.tpPeriod.ResumeLayout(false);
            this.gbReportingPeriod.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gizmox.WebGUI.Forms.SplitContainer splitContainer;
        private Gizmox.WebGUI.Forms.Panel panelTabs;
        private Gizmox.WebGUI.Forms.TabControl tabOptions;
        private Gizmox.WebGUI.Forms.TabPage tpOption;
        private Gizmox.WebGUI.Forms.TabPage tpPeriod;
        private Gizmox.WebGUI.Forms.Panel panelTree;
        private Gizmox.WebGUI.Forms.TreeView tvList;
        private Gizmox.WebGUI.Forms.Button btnSelectionAll;
        private Gizmox.WebGUI.Forms.Button btnClearSelection;
        private Gizmox.WebGUI.Forms.Button btnOK;
        private Gizmox.WebGUI.Forms.RadioButton rbtnName;
        private Gizmox.WebGUI.Forms.RadioButton rbtnCode;
        private Gizmox.WebGUI.Forms.GroupBox gbSelectionBy;
        private Gizmox.WebGUI.Forms.GroupBox gbReportingPeriod;
        private Gizmox.WebGUI.Forms.DateTimePicker dtpTo;
        private Gizmox.WebGUI.Forms.Label lblTo;
        private Gizmox.WebGUI.Forms.DateTimePicker dtpFrom;
        private Gizmox.WebGUI.Forms.Label lblFrom;
        private Gizmox.WebGUI.Forms.Panel viewerPane;
        private Gizmox.WebGUI.Forms.ComboBox cboCurrency;
        private Gizmox.WebGUI.Forms.Label lblCurrency;


    }
}