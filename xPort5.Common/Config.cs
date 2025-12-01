using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using Gizmox.WebGUI.Forms;

namespace xPort5.Common
{
    public static class Config
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
                        if (Utility.IsGUID(HttpContext.Current.Request.Cookies["xPort3_LogonUser"].Value))
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
                        if (Utility.IsNumeric(HttpContext.Current.Request.Cookies["xPort3_LogonUserType"].Value))
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
                    if (ConfigurationManager.AppSettings["SqlQueryLimit"] != null)
                    {
                        sqlQueryLimit = ConfigurationManager.AppSettings["SqlQueryLimit"];
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
                    if (ConfigurationManager.AppSettings["CommandTimeOut"] != null)
                    {
                        commandTimeOut = ConfigurationManager.AppSettings["CommandTimeOut"];
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
}
