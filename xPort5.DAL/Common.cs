using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using System.Web;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;
using System.ComponentModel;

namespace xPort5.DAL
{
    public class Common
    {
        #region Enum
        public class Enums
        {
            public enum Status
            {
                Inactive = -1,
                Draft = 0,
                Active,
                Power
            }

            public enum Workflow
            {
                Cancelled = 1,
                Queuing,
                Retouch,
                Printing,
                ProofingOutgoing,
                ProofingIncoming,
                Ready,
                Dispatch,
                InTransit,
                Completed
            }

            public enum OrderType
            {
                UploadFile = 1,
                DirectPrint,
                PsFile,
                Others
            }

            public enum Platform
            {
                PC = 1,
                Mac
            }

            public enum Priority
            {
                Rush = 1,
                Express,
                Regular
            }

            public enum Software
            {
                PageMaker = 1,
                FreeHand = 3,
                Illustrator = 5,
                PhotoShop = 7,
                QuarkXpress = 9,
                CorelDraw = 11,
                MsWord = 13,
                Others = 15
            }

            public enum DeliveryMethod
            {
                PickUp = 1,
                DeliverTo
            }

            public enum EditMode
            {
                Add,
                Edit,
                Read
            }

            public enum ContentType
            {
                Image,      // Supports *.Jpg/*.jpeg
                PdfFile,
                PlainText,  // Supports *.txt
                MSExcel,    // Supports *.xls/*.xlsx
                MSWord,     // Supports *.doc/*.docx
                Video       // Supports *.mp4
            }

            public enum UserGroup { Owner, Administrator, Manager, Supervisor, Senior, Junior, Guest }

            public enum UserType { Staff, Customer, Supplier }
        }
        #endregion

        #region Config
        public class Config
        {
            private static string ConnectionString4Excel03
            {
                get
                {
                    return ConfigurationManager.ConnectionStrings["OleConn4Excel03"].ConnectionString;
                }
            }

            private static string ConnectionString4Excel07
            {
                get
                {
                    return ConfigurationManager.ConnectionStrings["OleConn4Excel07"].ConnectionString;
                }
            }

            public static OleDbConnection GetOleDbConnection(string dataSource)
            {
                string connString = string.Empty;

                if (dataSource.Length > 0)
                {
                    string ext = dataSource.Remove(0, dataSource.LastIndexOf('.') + 1);
                    switch (ext.ToLower().Trim())
                    {
                        case "xls":
                            connString = string.Format(ConnectionString4Excel03, dataSource);
                            break;
                        case "xlsx":
                            connString = string.Format(ConnectionString4Excel07, dataSource);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(connString))
                {
                    OleDbConnection oConn = new OleDbConnection(connString);

                    if (oConn.State == ConnectionState.Open)
                    {
                        oConn.Close();
                    }

                    return oConn;
                }
                else
                {
                    return null;
                }
            }

            public static string ConnectionString
            {
                get
                {
                    return WebConfigurationManager.ConnectionStrings["SysDb"].ConnectionString;
                }
            }

            public static Guid CurrentUserId
            {
                get
                {
                    Guid cookieUserId = Guid.Empty;
                    if (HttpContext.Current.Request.Cookies["xPort3_LogonUser"] != null)
                    {
                        if (Common.Utility.IsGUID(HttpContext.Current.Request.Cookies["xPort3_LogonUser"].Value))
                        {
                            cookieUserId = new Guid(HttpContext.Current.Request.Cookies["xPort3_LogonUser"].Value);
                        }
                    }
                    return cookieUserId;
                }
                set
                {
                    System.Web.HttpCookie oCookie = new System.Web.HttpCookie("xPort3_LogonUser");

                    if (value != Guid.Empty)
                    {
                        // create the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddYears(1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                    else
                    {
                        // destory the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddDays(-1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                }
            }

            public static int CurrentUserType
            {
                get
                {
                    int cookieUserType = (int)Enums.UserType.Staff;
                    if (HttpContext.Current.Request.Cookies["xPort3_LogonUserType"] != null)
                    {
                        if (Common.Utility.IsNumeric(HttpContext.Current.Request.Cookies["xPort3_LogonUserType"].Value))
                        {
                            cookieUserType = Convert.ToInt32(HttpContext.Current.Request.Cookies["xPort3_LogonUserType"].Value);
                        }
                    }
                    return cookieUserType;
                }
                set
                {
                    System.Web.HttpCookie oCookie = new System.Web.HttpCookie("xPort3_LogonUserType");

                    if (value >= 0)
                    {
                        // create the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddYears(1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                    else
                    {
                        // destory the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddDays(-1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                }
            }

            public static int CurrentLanguageId
            {
                get
                {
                    int result = 1;
                    string sLang = (string)System.Web.HttpContext.Current.Session["UserLanguage"];
                    if (sLang == null) sLang = System.Web.HttpContext.Current.Request.UserLanguages[0];

                    switch (sLang.ToLower())
                    {
                        case "chs":
                        case "zh-chs":
                        case "zh-cn":
                            result = 2;
                            break;
                        case "cht":
                        case "zh-cht":
                        case "zh-hk":
                        case "zh-tw":
                            result = 3;
                            break;
                        case "en":
                        case "en-us":
                        default:
                            result = 1;
                            break;
                    }
                    return result;
                }
            }

            public static string CurrentWordDict
            {
                get
                {
                    return Path.Combine(VWGContext.Current.Config.GetDirectory("UserData"), "WordDict.xml");
                }
            }

            /// <summary>
            /// Maximum records allowed in SQL Query
            /// Default = 500 records
            /// </summary>
            public static int SqlQueryLimit
            {
                get
                {
                    string sqlQueryLimit = "500";
                    if (ConfigurationSettings.AppSettings["SqlQueryLimit"] != null)
                    {
                        sqlQueryLimit = ConfigurationSettings.AppSettings["SqlQueryLimit"];
                    }
                    return Convert.ToInt32(sqlQueryLimit);
                }
            }

            /// <summary>
            /// Maximum time out in SQL command
            /// Default = 600 seconds
            /// </summary>
            public static int CommandTimeOut
            {
                get
                {
                    string commandTimeOut = "600";
                    if (ConfigurationSettings.AppSettings["CommandTimeOut"] != null)
                    {
                        commandTimeOut = ConfigurationSettings.AppSettings["CommandTimeOut"];
                    }
                    return Convert.ToInt32(commandTimeOut);
                }
            }

            public static IFormatProvider DefaultCultureInfo
            {
                get
                {
                    CultureInfo defaultCultureInfo = new CultureInfo("en-US");
                    return defaultCultureInfo;
                }
            }

            public static void SetCultureInfo(string selectedLanguage)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);
            }

            public static string InBox
            {
                get
                {
                    string result = @"C:\xFilm\InBox";

                    if (ConfigurationManager.AppSettings["InBox"] != null)
                    {
                        result = (string)ConfigurationManager.AppSettings["InBox"];
                    }

                    return result;
                }
            }

            public static string OutBox
            {
                get
                {
                    string result = @"C:\xFilm\OutBox";

                    if (ConfigurationManager.AppSettings["OutBox"] != null)
                    {
                        result = (string)ConfigurationManager.AppSettings["OutBox"];
                    }

                    return result;
                }
            }

            public static string DropBox
            {
                get
                {
                    string result = @"C:\xFilm\DropBox";

                    if (ConfigurationManager.AppSettings["DropBox"] != null)
                    {
                        result = (string)ConfigurationManager.AppSettings["DropBox"];
                    }

                    return result;
                }
            }

            public static string GsWorkFolder
            {
                get
                {
                    string result = @"C:\xPort5\WorkFolder";

                    if (ConfigurationManager.AppSettings["Gswin32_WorkFolder"] != null)
                    {
                        result = (string)ConfigurationManager.AppSettings["Gswin32_WorkFolder"];
                        if (!(Directory.Exists(result)))
                        {
                            Directory.CreateDirectory(result);
                        }
                    }

                    return result;
                }
            }

            public static int MaxFileSize
            {
                get
                {
                    int result = 1024 * 50;

                    if (ConfigurationManager.AppSettings["MaxFileSize"] != null)
                    {
                        result = Convert.ToInt32((string)ConfigurationManager.AppSettings["MaxFileSize"]);
                    }

                    return result;
                }
            }

            public static int MaxLength_SKU
            {
                get
                {
                    int result = 8;

                    if (ConfigurationManager.AppSettings["MaxLength_SKU"] != null)
                    {
                        result = Convert.ToInt32((string)ConfigurationManager.AppSettings["MaxLength_SKU"]);
                    }

                    return result;
                }
            }

            public static int MaxLength_ProductCode
            {
                get
                {
                    int result = 10;

                    if (ConfigurationManager.AppSettings["MaxLength_ProductCode"] != null)
                    {
                        result = Convert.ToInt32((string)ConfigurationManager.AppSettings["MaxLength_ProductCode"]);
                    }

                    return result;
                }
            }

            public static int MaxLength_CustomerCode
            {
                get
                {
                    int result = 10;

                    if (ConfigurationManager.AppSettings["MaxLength_CustomerCode"] != null)
                    {
                        result = Convert.ToInt32((string)ConfigurationManager.AppSettings["MaxLength_CustomerCode"]);
                    }

                    return result;
                }
            }

            public static int MaxLength_SupplierCode
            {
                get
                {
                    int result = 10;

                    if (ConfigurationManager.AppSettings["MaxLength_SupplierCode"] != null)
                    {
                        result = Convert.ToInt32((string)ConfigurationManager.AppSettings["MaxLength_SupplierCode"]);
                    }

                    return result;
                }
            }

            public static int MaxLength_QTNumber
            {
                get
                {
                    int result = 10;

                    if (ConfigurationManager.AppSettings["MaxLength_QTNumber"] != null)
                    {
                        result = Convert.ToInt32((string)ConfigurationManager.AppSettings["MaxLength_QTNumber"]);
                    }

                    return result;
                }
            }

            public static bool UseNetSqlAzMan
            {
                get
                {
                    bool result = false;

                    if (ConfigurationManager.AppSettings["UseNetSqlAzMan"] != null)
                    {
                        bool.TryParse((String)ConfigurationManager.AppSettings["UseNetSqlAzMan"], out result);
                    }

                    return result;
                }
            }
        }
        #endregion

        public class Client
        {
            public static string InBox(int clientId)
            {
                string result = String.Empty;
                string fullpath = Path.Combine(Config.InBox, clientId.ToString());

                try
                {
                    if (!(Directory.Exists(fullpath)))
                    {
                        Directory.CreateDirectory(fullpath);
                    }
                    result = fullpath;
                }
                catch { }

                return result;
            }
            public static string DropBox(int clientId)
            {
                string result = String.Empty;
                string fullpath = Path.Combine(Config.DropBox, clientId.ToString());

                try
                {
                    if (!(Directory.Exists(fullpath)))
                    {
                        Directory.CreateDirectory(fullpath);
                    }
                    result = fullpath;
                }
                catch { }

                return result;
            }
        }

        public class Data
        {
            /// <summary>
            /// Appends the different Order Type with Icons to a ContextMenu
            /// </summary>
            /// <param name="ddlMenu"></param>
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

            public static void AppendMenuItem_AppViews(ref ContextMenu ddlViews)
            {
                nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("icon_view"), string.Empty, "Icon"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("tile_view"), string.Empty, "Tile"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("list_view"), string.Empty, "List"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("details_view"), string.Empty, "Details"));

                ddlViews.MenuItems[0].Icon = new IconResourceHandle("16x16.appView_icons.png");
                ddlViews.MenuItems[1].Icon = new IconResourceHandle("16x16.appView_tile.png");
                ddlViews.MenuItems[2].Icon = new IconResourceHandle("16x16.appView_columns.png");
                ddlViews.MenuItems[3].Icon = new IconResourceHandle("16x16.appView_list.png");
            }

            public static void AppendMenuItem_AppPref(ref ContextMenu ddlViews)
            {
                nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("save"), string.Empty, "Save"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("reset"), string.Empty, "Reset"));

                ddlViews.MenuItems[0].Icon = new IconResourceHandle("16x16.application_add.png");
                ddlViews.MenuItems[1].Icon = new IconResourceHandle("16x16.application_delete.png");
            }

            public static void AppendMenuItem_AppImageList(ref ContextMenu ddlViews)
            {
                nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("small_image"), string.Empty, "Small"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("medium_image"), string.Empty, "Medium"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("large_image"), string.Empty, "Large"));
                ddlViews.MenuItems.Add(new MenuItem(oDict.GetWord("details_image"), string.Empty, "Details"));

                ddlViews.MenuItems[0].Icon = new IconResourceHandle("16x16.imagelist_small_on_16.png");
                ddlViews.MenuItems[1].Icon = new IconResourceHandle("16x16.imagelist_medium_on_16.png");
                ddlViews.MenuItems[2].Icon = new IconResourceHandle("16x16.imagelist_large_on_16.png");
                ddlViews.MenuItems[3].Icon = new IconResourceHandle("16x16.imagelist_detail_on_16.png");
            }

            public static void LoadCombo_CreditLimmit(ref ComboBox comboBox)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("0");
                comboBox.Items.Add("30");
                comboBox.Items.Add("60");
                comboBox.Items.Add("90");
                comboBox.SelectedIndex = 0;
            }

            public static void LoadCombo_Language(ref ComboBox comboBox)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("English");
                comboBox.Items.Add("Simpified Chinese");
                comboBox.Items.Add("Traditional Chinese");
                comboBox.SelectedIndex = 0;
            }

            public static void LoadCombo_XchgBase(ref ComboBox comboBox)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("Foreign --> Local");
                comboBox.Items.Add("Local --> Foreign");
                comboBox.SelectedIndex = 0;
            }
        }

        #region Utility
        public class Utility
        {
            public static bool IsGUID(string expression)
            {
                /*
                if (expression != null)
                {
                    Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
                    return guidRegEx.IsMatch(expression);
                }
                return false;
                */
                var guidOutput = new Guid();
                bool isValid = Guid.TryParse(expression, out guidOutput);
                return isValid;
            }

            // Matches any unsigned or signed floating point number/numeric string.
            public static bool IsNumeric(string expression)
            {
                if (expression != null)
                {
                    Regex numericRegEx = new Regex(@"^-?\d+(\.\d+)?$");
                    return numericRegEx.IsMatch(expression);
                }
                return false;
            }
        }
        #endregion

        public class DateTimeHelper
        {
            /// <summary>
            /// Convert the datetime value to string with time or without.
            /// If the value is equaled to 1900-01-01, it would return a emty value.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="withTime"></param>
            /// <returns></returns>
            public static string DateTimeToString(DateTime value, bool withTime)
            {
                string formatString = GetDateFormat();
                if (withTime)
                {
                    formatString = GetDateTimeFormat();
                }

                if (!value.Equals(new DateTime(1900, 1, 1)))
                {
                    return value.ToString(formatString);
                }
                else
                {
                    return string.Empty;
                }
            }
            public static string DateTimeToString(string value, bool withTime)
            {
                string result = String.Empty;
                string formatString = GetDateFormat();
                if (withTime)
                {
                    formatString = GetDateTimeFormat();
                }
                try
                {
                    DateTime source = DateTime.Parse(value);
                    if (!source.Equals(new DateTime(1900, 1, 1)))
                    {
                        result = source.ToString(formatString);
                    }
                }
                catch {}
                return result;
            }

            public static string GetDateFormat()
            {
                string result = String.Empty;

                switch (VWGContext.Current.CurrentUICulture.ToString())
                {
                    case "zh-CHS":
                        result = "yyyy-MM-dd";
                        break;
                    case "zh-CHT":
                        result = "dd/MM/yyyy";
                        break;
                    case "en-US":
                    default:
                        result = "dd/MM/yyyy";
                        break;
                }

                return result;
            }

            public static string GetDateTimeFormat()
            {
                string result = String.Empty;

                switch (VWGContext.Current.CurrentUICulture.ToString())
                {
                    case "zh-CHS":
                        result = "yyyy-MM-dd HH:mm";
                        break;
                    case "zh-CHT":
                        result = "dd/MM/yyyy HH:mm";
                        break;
                    case "en-US":
                    default:
                        result = "dd/MM/yyyy HH:mm";
                        break;
                }

                return result;
            }
        }

        #region Combo Box Item

        public class ComboItem
        {
            private string _code = string.Empty;
            private Guid _id = Guid.Empty;

            public ComboItem(string code, Guid id)
            {
                _code = code;
                _id = id;
            }

            public string Code
            {
                get
                {
                    return _code;
                }
                set
                {
                    _code = value;
                }
            }

            public Guid Id
            {
                get
                {
                    return _id;
                }
                set
                {
                    _id = value;
                }
            }
        }

        public class ComboList : BindingList<ComboItem>
        {
        }

        #endregion
    }
}
