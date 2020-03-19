#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;

#endregion

namespace xPort5.NavPane
{
    public partial class SettingsNav : UserControl
    {
        public SettingsNav()
        {
            InitializeComponent();

            NavPane.NavMenu.FillNavTree("Settings", this.navSettings.Nodes);
        }

        private void navSettings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Control[] controls = this.Form.Controls.Find("wspPane", true);
            if (controls.Length > 0)
            {
                Panel wspPane = (Panel)controls[0];
                wspPane.Text = navSettings.SelectedNode.Text;
                //wspPane.BackColor = xPort5.Controls.Utility.Default.TopPanelBackgroundColor;
                wspPane.Controls.Clear();
                ShowWorkspace(ref wspPane, (string)navSettings.SelectedNode.Tag);
            }
        }

        private void ShowWorkspace(ref Panel wspPane, string Tag)
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                switch (Tag)
                {
                    case "Settings.SystemParameters":
                        xPort5.Settings.Counters counters = new xPort5.Settings.Counters();
                        counters.DockPadding.All = 6;
                        counters.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(counters);
                        break;
                    case "Coding.Division":
                        xPort5.Settings.Coding.Division.DivisionList divList = new xPort5.Settings.Coding.Division.DivisionList();
                        divList.DockPadding.All = 6;
                        divList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(divList);
                        break;
                    case "Coding.Group":
                        xPort5.Settings.Coding.Group.GroupList groupList = new xPort5.Settings.Coding.Group.GroupList();
                        groupList.DockPadding.All = 6;
                        groupList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(groupList);
                        break;
                    case "Coding.PhoneLabel":
                        xPort5.Settings.Coding.PhoneLabel.LabelList labelList = new xPort5.Settings.Coding.PhoneLabel.LabelList();
                        labelList.DockPadding.All = 6;
                        labelList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(labelList);
                        break;
                    case "Coding.MigrateProductPicture":
                        xPort5.Settings.Coding.MigrateProductPicture migrate = new xPort5.Settings.Coding.MigrateProductPicture();
                        migrate.DockPadding.All = 6;
                        migrate.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(migrate);
                        break;
                }
            }
        }
    }
}