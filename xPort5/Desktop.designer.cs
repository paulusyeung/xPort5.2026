namespace xPort5
{
    partial class Desktop
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Desktop));
            this.amsMain = new Gizmox.WebGUI.Forms.MainMenu();
            this.amsFile = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsFileExit = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsView = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsViewEn = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsViewChs = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsViewCht = new Gizmox.WebGUI.Forms.MenuItem();
            this.menuItem1 = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsViewVista = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsViewBlack = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsViewWinXP = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsHelp = new Gizmox.WebGUI.Forms.MenuItem();
            this.amsHelpAbout = new Gizmox.WebGUI.Forms.MenuItem();
            this.atsPane = new Gizmox.WebGUI.Forms.Panel();
            this.navPane = new Gizmox.WebGUI.Forms.Panel();
            this.navTabs = new Gizmox.WebGUI.Forms.NavigationTabs();
            this.tabCoding = new Gizmox.WebGUI.Forms.NavigationTab();
            this.tabOrder = new Gizmox.WebGUI.Forms.NavigationTab();
            this.tabAdmin = new Gizmox.WebGUI.Forms.NavigationTab();
            this.tabSettings = new Gizmox.WebGUI.Forms.NavigationTab();
            this.splitter1 = new Gizmox.WebGUI.Forms.Splitter();
            this.wspPane = new Gizmox.WebGUI.Forms.Panel();
            this.picBgImage = new Gizmox.WebGUI.Forms.PictureBox();
            this.navPane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navTabs)).BeginInit();
            this.navTabs.SuspendLayout();
            this.wspPane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBgImage)).BeginInit();
            this.SuspendLayout();
            // 
            // amsMain
            // 
            this.amsMain.Dock = Gizmox.WebGUI.Forms.DockStyle.Top;
            this.amsMain.Location = new System.Drawing.Point(0, 0);
            this.amsMain.MenuItems.AddRange(new Gizmox.WebGUI.Forms.MenuItem[] {
            this.amsFile,
            this.amsView,
            this.amsHelp});
            this.amsMain.Name = "amsMain";
            this.amsMain.Size = new System.Drawing.Size(369, 22);
            this.amsMain.MenuClick += new Gizmox.WebGUI.Forms.MenuEventHandler(this.amsMain_MenuClick);
            // 
            // amsFile
            // 
            this.amsFile.Index = 0;
            this.amsFile.MenuItems.AddRange(new Gizmox.WebGUI.Forms.MenuItem[] {
            this.amsFileExit});
            this.amsFile.Tag = "amsFile";
            this.amsFile.Text = "File";
            // 
            // amsFileExit
            // 
            this.amsFileExit.Index = 0;
            this.amsFileExit.Tag = "amsFileExit";
            this.amsFileExit.Text = "Exit";
            // 
            // amsView
            // 
            this.amsView.Index = 1;
            this.amsView.MenuItems.AddRange(new Gizmox.WebGUI.Forms.MenuItem[] {
            this.amsViewEn,
            this.amsViewChs,
            this.amsViewCht,
            this.menuItem1,
            this.amsViewVista,
            this.amsViewBlack,
            this.amsViewWinXP});
            this.amsView.Tag = "amsView";
            this.amsView.Text = "View";
            // 
            // amsViewEn
            // 
            this.amsViewEn.Icon = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("amsViewEn.Icon"));
            this.amsViewEn.Index = 0;
            this.amsViewEn.Tag = "amsViewEn";
            this.amsViewEn.Text = "English";
            // 
            // amsViewChs
            // 
            this.amsViewChs.Icon = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("amsViewChs.Icon"));
            this.amsViewChs.Index = 1;
            this.amsViewChs.Tag = "amsViewChs";
            this.amsViewChs.Text = "Simplified Chinese";
            // 
            // amsViewCht
            // 
            this.amsViewCht.Icon = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("amsViewCht.Icon"));
            this.amsViewCht.Index = 2;
            this.amsViewCht.Tag = "amsViewCht";
            this.amsViewCht.Text = "Traditional Chinese";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 3;
            this.menuItem1.Text = "-";
            // 
            // amsViewVista
            // 
            this.amsViewVista.Index = 4;
            this.amsViewVista.Tag = "amsViewVista";
            this.amsViewVista.Text = "Vista Theme";
            // 
            // amsViewBlack
            // 
            this.amsViewBlack.Index = 5;
            this.amsViewBlack.Tag = "amsViewBlack";
            this.amsViewBlack.Text = "Black Theme";
            // 
            // amsViewWinXP
            // 
            this.amsViewWinXP.Index = 6;
            this.amsViewWinXP.Tag = "amsViewWinXP";
            this.amsViewWinXP.Text = "WinXP Theme";
            // 
            // amsHelp
            // 
            this.amsHelp.Index = 2;
            this.amsHelp.MenuItems.AddRange(new Gizmox.WebGUI.Forms.MenuItem[] {
            this.amsHelpAbout});
            this.amsHelp.Tag = "amsHelp";
            this.amsHelp.Text = "Help";
            // 
            // amsHelpAbout
            // 
            this.amsHelpAbout.Icon = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("amsHelpAbout.Icon"));
            this.amsHelpAbout.Index = 0;
            this.amsHelpAbout.Tag = "amsHelpAbout";
            this.amsHelpAbout.Text = "About";
            // 
            // atsPane
            // 
            this.atsPane.Dock = Gizmox.WebGUI.Forms.DockStyle.Top;
            this.atsPane.Location = new System.Drawing.Point(0, 0);
            this.atsPane.Name = "atsPane";
            this.atsPane.Size = new System.Drawing.Size(520, 28);
            this.atsPane.TabIndex = 0;
            // 
            // navPane
            // 
            this.navPane.Controls.Add(this.navTabs);
            this.navPane.Dock = Gizmox.WebGUI.Forms.DockStyle.Left;
            this.navPane.Location = new System.Drawing.Point(0, 28);
            this.navPane.Name = "navPane";
            this.navPane.Size = new System.Drawing.Size(150, 342);
            this.navPane.TabIndex = 1;
            // 
            // navTabs
            // 
            this.navTabs.Controls.Add(this.tabCoding);
            this.navTabs.Controls.Add(this.tabOrder);
            this.navTabs.Controls.Add(this.tabAdmin);
            this.navTabs.Controls.Add(this.tabSettings);
            this.navTabs.CustomStyle = "Navigation";
            this.navTabs.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.navTabs.Location = new System.Drawing.Point(0, 0);
            this.navTabs.Name = "navTabs";
            this.navTabs.SelectedIndex = 0;
            this.navTabs.Size = new System.Drawing.Size(150, 342);
            this.navTabs.TabIndex = 0;
            this.navTabs.SelectedIndexChanged += new System.EventHandler(this.navTabs_SelectedIndexChanged);
            // 
            // tabCoding
            // 
            this.tabCoding.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tabCoding.Extra = false;
            this.tabCoding.Image = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("tabCoding.Image"));
            this.tabCoding.Location = new System.Drawing.Point(4, 22);
            this.tabCoding.Name = "tabCoding";
            this.tabCoding.Size = new System.Drawing.Size(142, 312);
            this.tabCoding.TabIndex = 0;
            this.tabCoding.Tag = "Coding";
            this.tabCoding.Text = "Coding";
            // 
            // tabOrder
            // 
            this.tabOrder.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tabOrder.Extra = false;
            this.tabOrder.Image = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("tabOrder.Image"));
            this.tabOrder.Location = new System.Drawing.Point(4, 22);
            this.tabOrder.Name = "tabOrder";
            this.tabOrder.Size = new System.Drawing.Size(142, 316);
            this.tabOrder.TabIndex = 1;
            this.tabOrder.Tag = "Order";
            this.tabOrder.Text = "Order";
            // 
            // tabAdmin
            // 
            this.tabAdmin.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tabAdmin.Extra = false;
            this.tabAdmin.Image = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("tabAdmin.Image"));
            this.tabAdmin.Location = new System.Drawing.Point(4, 22);
            this.tabAdmin.Name = "tabAdmin";
            this.tabAdmin.Size = new System.Drawing.Size(142, 316);
            this.tabAdmin.TabIndex = 2;
            this.tabAdmin.Tag = "Admin";
            this.tabAdmin.Text = "Admin";
            // 
            // tabSettings
            // 
            this.tabSettings.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.tabSettings.Extra = false;
            this.tabSettings.Image = new Gizmox.WebGUI.Common.Resources.IconResourceHandle(resources.GetString("tabSettings.Image"));
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(142, 313);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Tag = "Settings";
            this.tabSettings.Text = "Settings";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(150, 28);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1, 342);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // wspPane
            // 
            this.wspPane.Controls.Add(this.picBgImage);
            this.wspPane.CustomStyle = "HeaderedPanel";
            this.wspPane.Dock = Gizmox.WebGUI.Forms.DockStyle.Fill;
            this.wspPane.Location = new System.Drawing.Point(151, 28);
            this.wspPane.Name = "wspPane";
            this.wspPane.PanelType = Gizmox.WebGUI.Forms.PanelType.Titled;
            this.wspPane.Size = new System.Drawing.Size(369, 342);
            this.wspPane.TabIndex = 3;
            // 
            // picBgImage
            // 
            this.picBgImage.Anchor = ((Gizmox.WebGUI.Forms.AnchorStyles)((Gizmox.WebGUI.Forms.AnchorStyles.Bottom | Gizmox.WebGUI.Forms.AnchorStyles.Right)));
            this.picBgImage.Location = new System.Drawing.Point(231, 202);
            this.picBgImage.Name = "picBgImage";
            this.picBgImage.Size = new System.Drawing.Size(128, 128);
            this.picBgImage.SizeMode = Gizmox.WebGUI.Forms.PictureBoxSizeMode.StretchImage;
            this.picBgImage.TabIndex = 0;
            this.picBgImage.TabStop = false;
            // 
            // Desktop
            // 
            this.Controls.Add(this.wspPane);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.navPane);
            this.Controls.Add(this.atsPane);
            this.Menu = this.amsMain;
            this.Size = new System.Drawing.Size(520, 370);
            this.Text = "xPort5";
            this.navPane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navTabs)).EndInit();
            this.navTabs.ResumeLayout(false);
            this.wspPane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBgImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Gizmox.WebGUI.Forms.MainMenu amsMain;
        private Gizmox.WebGUI.Forms.MenuItem amsFile;
        private Gizmox.WebGUI.Forms.MenuItem amsFileExit;
        private Gizmox.WebGUI.Forms.MenuItem amsView;
        private Gizmox.WebGUI.Forms.MenuItem amsHelp;
        private Gizmox.WebGUI.Forms.MenuItem amsHelpAbout;
        private Gizmox.WebGUI.Forms.Panel atsPane;
        private Gizmox.WebGUI.Forms.Panel navPane;
        private Gizmox.WebGUI.Forms.Splitter splitter1;
        private Gizmox.WebGUI.Forms.Panel wspPane;
        private Gizmox.WebGUI.Forms.NavigationTabs navTabs;
        private Gizmox.WebGUI.Forms.NavigationTab tabAdmin;
        private Gizmox.WebGUI.Forms.NavigationTab tabOrder;
        private Gizmox.WebGUI.Forms.NavigationTab tabCoding;
        private Gizmox.WebGUI.Forms.NavigationTab tabSettings;
        private Gizmox.WebGUI.Forms.PictureBox picBgImage;
        private Gizmox.WebGUI.Forms.MenuItem amsViewEn;
        private Gizmox.WebGUI.Forms.MenuItem amsViewCht;
        private Gizmox.WebGUI.Forms.MenuItem amsViewChs;
        private Gizmox.WebGUI.Forms.MenuItem menuItem1;
        private Gizmox.WebGUI.Forms.MenuItem amsViewVista;
        private Gizmox.WebGUI.Forms.MenuItem amsViewBlack;
        private Gizmox.WebGUI.Forms.MenuItem amsViewWinXP;


    }
}