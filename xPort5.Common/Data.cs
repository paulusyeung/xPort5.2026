using System;
using System.ComponentModel;
using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;

namespace xPort5.Common
{
    /// <summary>
    /// UI data population helper methods for menus and dropdowns.
    /// Migrated from xPort5.DAL.Common.cs
    /// Note: This class has dependencies on Gizmox VWG framework and nxStudio.
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// Appends the different Order Type with Icons to a ContextMenu
        /// </summary>
        /// <param name="ddlMenu">Context menu to populate</param>
        public static void AppendMenuItem_OrderType(ref ContextMenu ddlMenu)
        {
            ddlMenu.MenuItems.Add(new MenuItem("Upload File", string.Empty, "UploadFile"));
            ddlMenu.MenuItems.Add(new MenuItem("Direct Print", string.Empty, "DirectPrint"));
            ddlMenu.MenuItems.Add(new MenuItem("PS File", string.Empty, "PsFile"));
            ddlMenu.MenuItems.Add(new MenuItem("Others", string.Empty, "Others"));

            ddlMenu.MenuItems[0].Icon = new IconResourceHandle("JobOrder.UploadFile_16.png");
            ddlMenu.MenuItems[1].Icon = new IconResourceHandle("JobOrder.DirectPrint_16.png");
            ddlMenu.MenuItems[2].Icon = new IconResourceHandle("JobOrder.PsFile_16.png");
            ddlMenu.MenuItems[3].Icon = new IconResourceHandle("JobOrder.Others_16.png");
        }

        /// <summary>
        /// Appends application view options to a ContextMenu
        /// </summary>
        /// <param name="ddlViews">Context menu to populate</param>
        public static void AppendMenuItem_AppViews(ref ContextMenu ddlViews)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Config.CurrentWordDict, Config.CurrentLanguageId);

            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("icon_view"), string.Empty, "Icon"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("tile_view"), string.Empty, "Tile"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("list_view"), string.Empty, "List"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("details_view"), string.Empty, "Details"));

            ddlViews.MenuItems[0].Icon = new IconResourceHandle("16x16.appView_icons.png");
            ddlViews.MenuItems[1].Icon = new IconResourceHandle("16x16.appView_tile.png");
            ddlViews.MenuItems[2].Icon = new IconResourceHandle("16x16.appView_columns.png");
            ddlViews.MenuItems[3].Icon = new IconResourceHandle("16x16.appView_list.png");
        }

        /// <summary>
        /// Appends application preference options to a ContextMenu
        /// </summary>
        /// <param name="ddlViews">Context menu to populate</param>
        public static void AppendMenuItem_AppPref(ref ContextMenu ddlViews)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Config.CurrentWordDict, Config.CurrentLanguageId);

            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("save"), string.Empty, "Save"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("reset"), string.Empty, "Reset"));

            ddlViews.MenuItems[0].Icon = new IconResourceHandle("16x16.application_add.png");
            ddlViews.MenuItems[1].Icon = new IconResourceHandle("16x16.application_delete.png");
        }

        /// <summary>
        /// Appends image list size options to a ContextMenu
        /// </summary>
        /// <param name="ddlViews">Context menu to populate</param>
        public static void AppendMenuItem_AppImageList(ref ContextMenu ddlViews)
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Config.CurrentWordDict, Config.CurrentLanguageId);

            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("small_image"), string.Empty, "Small"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("medium_image"), string.Empty, "Medium"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("large_image"), string.Empty, "Large"));
            ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("details_image"), string.Empty, "Details"));

            ddlViews.MenuItems[0].Icon = new IconResourceHandle("16x16.imagelist_small_on_16.png");
            ddlViews.MenuItems[1].Icon = new IconResourceHandle("16x16.imagelist_medium_on_16.png");
            ddlViews.MenuItems[2].Icon = new IconResourceHandle("16x16.imagelist_large_on_16.png");
            ddlViews.MenuItems[3].Icon = new IconResourceHandle("16x16.imagelist_detail_on_16.png");
        }

        /// <summary>
        /// Loads credit limit options into a ComboBox
        /// </summary>
        /// <param name="comboBox">ComboBox to populate</param>
        public static void LoadCombo_CreditLimmit(ref ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("0");
            comboBox.Items.Add("30");
            comboBox.Items.Add("60");
            comboBox.Items.Add("90");
            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads language options into a ComboBox
        /// </summary>
        /// <param name="comboBox">ComboBox to populate</param>
        public static void LoadCombo_Language(ref ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("English");
            comboBox.Items.Add("Simpified Chinese");
            comboBox.Items.Add("Traditional Chinese");
            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads exchange rate base options into a ComboBox
        /// </summary>
        /// <param name="comboBox">ComboBox to populate</param>
        public static void LoadCombo_XchgBase(ref ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("Foreign --> Local");
            comboBox.Items.Add("Local --> Foreign");
            comboBox.SelectedIndex = 0;
        }
    }
}
