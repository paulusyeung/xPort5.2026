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

using xPort5.EF6;
using xPort5.Common;

#endregion

namespace xPort5.Admin.Coding.User
{
    public partial class UserListAts : UserControl
    {
        public UserListAts()
        {
            InitializeComponent();

            SetPriceListAts();
        }

        private void SetPriceListAts()
        {
            this.atsUserList.MenuHandle = false;
            this.atsUserList.DragHandle = false;
            this.atsUserList.TextAlign = ToolBarTextAlign.Right;

            ToolBarButton sep = new ToolBarButton();
            sep.Style = ToolBarButtonStyle.Separator;

            #region cmdNew
            ContextMenu ddlNew = new ContextMenu();
            ddlNew.MenuItems.Add(new MenuItem("User", string.Empty, "User"));

            ToolBarButton cmdNew = new ToolBarButton("New", "New");
            cmdNew.Style = ToolBarButtonStyle.DropDownButton;
            cmdNew.Image = new IconResourceHandle("16x16.ico_16_3.gif");
            cmdNew.DropDownMenu = ddlNew;
            cmdNew.MenuClick += new MenuEventHandler(cmdMenuClick);
            #endregion

            #region cmdImport
            ContextMenu ddlImport = new ContextMenu();
            ddlImport.MenuItems.Add(new MenuItem("Goods Receive (Excel)", string.Empty, "Goods_Receive_Import_Xls"));
            ddlImport.MenuItems.Add(new MenuItem("Goods Receive (Text)", string.Empty, "Goods_Receive_Import_Txt"));
            ddlImport.MenuItems[0].Enabled = false;

            ToolBarButton cmdImport = new ToolBarButton("Import", "Import");
            cmdImport.Style = ToolBarButtonStyle.DropDownButton;
            cmdImport.Image = new IconResourceHandle("16x16.ico_16_4407.gif");
            cmdImport.DropDownMenu = ddlImport;
            cmdImport.MenuClick += new MenuEventHandler(cmdMenuClick);
            #endregion

            #region cmdExport
            ContextMenu ddlExport = new ContextMenu();
            ddlExport.MenuItems.Add(new MenuItem("Client Email List", string.Empty, "Client_Email_List"));

            ToolBarButton cmdExport = new ToolBarButton("Export", "Export");
            cmdExport.Style = ToolBarButtonStyle.DropDownButton;
            cmdExport.Image = new IconResourceHandle("16x16.export.png");
            cmdExport.DropDownMenu = ddlExport;
            cmdExport.MenuClick += new MenuEventHandler(cmdMenuClick);
            #endregion

            #region cmdReports
            ContextMenu ddlReports = new ContextMenu();
            ddlReports.MenuItems.Add(new MenuItem("Worksheet", string.Empty, "Worksheet"));
            ddlReports.MenuItems.Add(new MenuItem("History", string.Empty, "History"));
            ddlReports.MenuItems.Add(new MenuItem("Journal", string.Empty, "Journal"));

            ToolBarButton cmdReports = new ToolBarButton("Reports", "Reports");
            cmdReports.Style = ToolBarButtonStyle.DropDownButton;
            cmdReports.Image = new IconResourceHandle("16x16.16_reports.gif");
            cmdReports.DropDownMenu = ddlReports;
            cmdReports.MenuClick += new MenuEventHandler(cmdMenuClick);
            #endregion


            this.atsUserList.Buttons.Add(cmdNew);
//            this.atsClient.Buttons.Add(cmdImport);
//            this.atsClient.Buttons.Add(cmdExport);
//            this.atsClient.Buttons.Add(sep);
//            this.atsClient.Buttons.Add(cmdReports);
//            this.atsClient.Buttons.Add(sep);


        }

        void cmdMenuClick(object sender, MenuItemEventArgs e)
        {
            if (!(e.MenuItem.Tag == null))
            {
                switch (e.MenuItem.Tag.ToString().ToLower())
                {
                    case "user":
                        //UserRecord user = new UserRecord();
                        //user.EditMode = Common.Enums.EditMode.Add;
                        //user.ShowDialog();
                        break;
                }
            }
        }

        private void atsClient_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            Control[] controls = this.Form.Controls.Find("wspPane", true);
            if (controls.Length > 0)
            {
                Panel wspPane = (Panel)controls[0];
                wspPane.Text = (string)e.Button.Text;
            }
        }
    }
}