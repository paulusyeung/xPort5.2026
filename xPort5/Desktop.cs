#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Common.Interfaces;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace xPort5
{
    public partial class Desktop : Form
    {
        private enum AtsStyle { Factory, Order, Admin, Settings };

        public Desktop()
        {
            InitializeComponent();

            SetTheme();
            SetCaptions();
            SetNavPanes();
            SetAppToolStrip(AtsStyle.Factory);

            SetCloseButton();
        }

        #region Close Button
        private void SetCloseButton()
        {
            Button cmdClose = new Button();
            cmdClose.Name = "cmdClose";
            cmdClose.Location = new System.Drawing.Point(this.Width - 43, 3);
            cmdClose.Size = new System.Drawing.Size(38, 38);
            cmdClose.Image = new IconResourceHandle("32x32.shutdown32.png");
            cmdClose.ImageAlign = ContentAlignment.MiddleCenter;
            cmdClose.TextImageRelation = Gizmox.WebGUI.Forms.TextImageRelation.ImageAboveText;
            cmdClose.Anchor = ((Gizmox.WebGUI.Forms.AnchorStyles)((Gizmox.WebGUI.Forms.AnchorStyles.Top | Gizmox.WebGUI.Forms.AnchorStyles.Right)));

            cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            this.Controls.Add(cmdClose);
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void Shutdown()
        {
            DAL.Common.Config.CurrentUserId = System.Guid.Empty;

            xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Logout, this.ToString());

            // set the IsLoggedOn to false will redirect to Logon Page.
            this.Context.Session.IsLoggedOn = false;
            VWGContext.Current.HttpContext.Session.Abandon();
            VWGContext.Current.Transfer(new Desktop());
        }
        #endregion

        private void SetTheme()
        {
            ImageResourceHandle bgImage = new ImageResourceHandle("logo_watermark.png");

            picBgImage.Image = bgImage;

            /**
             * 2020.03.20 paulus: 根據個 theme 改個 background color
             */
            Context.CurrentTheme    = xPort5.Controls.Utility.Default.CurrentTheme;
            wspPane.BackColor       = xPort5.Controls.Utility.Default.TopPanelBackgroundColor;
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(xPort5.Common.Config.CurrentWordDict, xPort5.Common.Config.CurrentLanguageId);

            tabCoding.Text = oDict.GetWord("factory");
            tabAdmin.Text = oDict.GetWord("admin");
            tabOrder.Text = oDict.GetWord("order");
            tabSettings.Text = oDict.GetWord("settings");

            amsFile.Text = oDict.GetWord("file");
            amsFileExit.Text = oDict.GetWord("exit");
            amsView.Text = oDict.GetWord("view");
            amsViewEn.Text = oDict.GetWord("english");
            amsViewChs.Text = oDict.GetWord("simplifiedchinese");
            amsViewCht.Text = oDict.GetWord("traditionalchinese");
            amsViewVista.Text = oDict.GetWord("vista");
            amsViewBlack.Text = oDict.GetWord("black");
            amsViewWinXP.Text = oDict.GetWord("winxp");
            amsHelp.Text = oDict.GetWord("help");
            amsHelpAbout.Text = oDict.GetWord("about");
        }

        private void SetAppToolStrip(AtsStyle index)
        {
            this.atsPane.Controls.Clear();

            switch (index)
            {
                case AtsStyle.Factory:
                    xPort5.AtsPane.CodingAts oAtsJO = new xPort5.AtsPane.CodingAts();
                    oAtsJO.Dock = DockStyle.Fill;
                    this.atsPane.Controls.Add(oAtsJO);
                    this.tabCoding.Select();
                    break;
                case AtsStyle.Order:
                    xPort5.AtsPane.OrderAts oAtsAcct = new xPort5.AtsPane.OrderAts();
                    oAtsAcct.Dock = DockStyle.Fill;
                    this.atsPane.Controls.Add(oAtsAcct);
                    this.tabOrder.Select();
                    break;
                case AtsStyle.Admin:
                    xPort5.AtsPane.AdminAts oAtsAdmin = new xPort5.AtsPane.AdminAts();
                    oAtsAdmin.Dock = DockStyle.Fill;
                    this.atsPane.Controls.Add(oAtsAdmin);
                    this.tabAdmin.Select();
                    break;
                case AtsStyle.Settings:
                    xPort5.AtsPane.SettingsAts oAtsSettings = new xPort5.AtsPane.SettingsAts();
                    oAtsSettings.Dock = DockStyle.Fill;
                    this.atsPane.Controls.Add(oAtsSettings);
                    this.tabSettings.Select();
                    break;
            }
        }

        #region Set Navigation Panes
        private void SetNavPanes()
        {
            SetNavCoding();
            SetNavOrder();
            SetNavAdmin();
            SetNavSettings();
        }

        private void SetNavCoding()
        {
            xPort5.NavPane.CodingNav navCoding = new xPort5.NavPane.CodingNav();

            TreeView tvwCoding = (TreeView)navCoding.Controls[0];
            if (tvwCoding.Nodes.Count == 0)
            {
                navTabs.TabPages.Remove(tabCoding);
            }
            else
            {
                navCoding.Dock = DockStyle.Fill;
                tabCoding.Controls.Add(navCoding);
            }
        }

        private void SetNavOrder()
        {
            xPort5.NavPane.OrderNav navOrder = new xPort5.NavPane.OrderNav();

            TreeView tvwOrder = (TreeView)navOrder.Controls[0];
            if (tvwOrder.Nodes.Count == 0)
            {
                navTabs.TabPages.Remove(tabOrder);
            }
            else
            {
                navOrder.Dock = DockStyle.Fill;
                tabOrder.Controls.Add(navOrder);
            }
        }

        private void SetNavAdmin()
        {
            xPort5.NavPane.AdminNav navAdmin = new xPort5.NavPane.AdminNav();

            TreeView tvwAdmin = (TreeView)navAdmin.Controls[0];
            if (tvwAdmin.Nodes.Count == 0)
            {
                navTabs.TabPages.Remove(tabAdmin);
            }
            else
            {
                navAdmin.Dock = DockStyle.Fill;
                tabAdmin.Controls.Add(navAdmin);
            }
        }

        private void SetNavSettings()
        {
            xPort5.NavPane.SettingsNav navSettings = new xPort5.NavPane.SettingsNav();

            TreeView tvwSettings = (TreeView)navSettings.Controls[0];
            if (tvwSettings.Nodes.Count == 0)
            {
                navTabs.TabPages.Remove(tabSettings);
            }
            else
            {
                navSettings.Dock = DockStyle.Fill;
                tabSettings.Controls.Add(navSettings);
            }
        }
        #endregion

        private void amsMain_MenuClick(object objSource, MenuItemEventArgs objArgs)
        {
            MenuItemEventArgs oArg = (MenuItemEventArgs)objArgs;
            string strAction = oArg.MenuItem.Tag as string;
            if (strAction != null)
            {
                switch (strAction)
                {
                    case "amsFileExit":
                        Shutdown();
                        break;
                    case "Print":
                        //MessageBox.Show(((Gizmox.WebGUI.Common.Interfaces.ISessionRegistry)this.Context.Session).Count.ToString());
                        break;
                    case "amsViewEn":   // English
//                        VWGContext.Current.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        System.Web.HttpContext.Current.Session["UserLanguage"] = "en-US";
                        VWGContext.Current.Transfer(new Desktop());
                        break;
                    case "amsViewChs":  // Simplified Chinese
//                        VWGContext.Current.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                        System.Web.HttpContext.Current.Session["UserLanguage"] = "zh-CHS";
                        VWGContext.Current.Transfer(new Desktop());
                        break;
                    case "amsViewCht":  // Tradictional Chinese
//                        VWGContext.Current.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                        System.Web.HttpContext.Current.Session["UserLanguage"] = "zh-CHT";
                        VWGContext.Current.Transfer(new Desktop());
                        break;
                    case "amsViewWinXP":
                        this.Context.CurrentTheme = "iOS";
                        xPort5.Controls.Utility.Default.TopPanelBackgroundColor = Color.FromName("#97B9CB");
                        xPort5.Controls.Utility.Default.CurrentTheme = "iOS";
                        break;
                    case "amsViewVista":
                        this.Context.CurrentTheme = "Vista";
                        xPort5.Controls.Utility.Default.TopPanelBackgroundColor = Color.FromName("#ACC0E9");
                        xPort5.Controls.Utility.Default.CurrentTheme = "Vista";
                        break;
                    case "amsViewBlack":
                        this.Context.CurrentTheme = "Graphite";
                        xPort5.Controls.Utility.Default.TopPanelBackgroundColor = Color.FromName("#333333");
                        xPort5.Controls.Utility.Default.CurrentTheme = "Graphite";
                        break;
                    case "amsHelpAbout":
                        Help.About oAbout = new xPort5.Help.About();
                        oAbout.ShowDialog();
                        break;
                    default:
                        //                        MessageBox.Show(strAction);
                        break;
                }
            }
        }

        #region Deselect selected TreeNodes on switching navTabs
        private void DeSelectTreeNodes()
        {
            Control[] coding = this.Form.Controls.Find("navCoding", true);
            if (coding.Length > 0)
            {
                TreeView tvJobOrder = (TreeView)coding[0];
                tvJobOrder.SelectedNode = null;
            }
            Control[] order = this.Form.Controls.Find("navOrder", true);
            if (order.Length > 0)
            {
                TreeView tvAcct = (TreeView)order[0];
                tvAcct.SelectedNode = null;
            }
            Control[] admin = this.Form.Controls.Find("navAdmin", true);
            if (admin.Length > 0)
            {
                TreeView tvAdmin = (TreeView)admin[0];
                tvAdmin.SelectedNode = null;
            }
            Control[] settings = this.Form.Controls.Find("navSettings", true);
            if (settings.Length > 0)
            {
                TreeView tvInvt = (TreeView)settings[0];
                tvInvt.SelectedNode = null;
            }
        }
        #endregion

        private void navTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeSelectTreeNodes();

            switch (navTabs.SelectedItem.TabIndex)
            {
                case (int)AtsStyle.Factory:
                    SetAppToolStrip(AtsStyle.Factory);
                    break;
                case (int)AtsStyle.Order:
                    SetAppToolStrip(AtsStyle.Order);
                    break;
                case (int)AtsStyle.Admin:
                    SetAppToolStrip(AtsStyle.Admin);
                    break;
                case (int)AtsStyle.Settings:
                    SetAppToolStrip(AtsStyle.Settings);
                    break;
            }
        }
    }
}
