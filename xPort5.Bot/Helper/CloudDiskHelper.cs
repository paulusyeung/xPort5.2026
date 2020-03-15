using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using ImageMagick;
using log4net;
using owncloudsharp;

using xPort5.EF6;

namespace xPort5.Bot.Helper
{
    public class CloudDiskHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string CLOUDDISK_EXTERNALURL = ConfigurationManager.AppSettings["CloudDisk_ServerExternalUri"];  // "http://cd.directoutput.com.hk/";
        static string CLOUDDISK_URL = ConfigurationManager.AppSettings["CloudDisk_ServerUri"];                  // "http://192.168.12.150/nextcloud/";
        static string CLOUDDISK_ADMIN = ConfigurationManager.AppSettings["CloudDisk_Admin"];                    // "nxstudio";
        static string CLOUDDISK_ADMINPASSWORD = ConfigurationManager.AppSettings["CloudDisk_AdminPassword"];    // "nx@82279602";

        /* 2020.03.15 paulus: 暫時取消有關 Client 嘅 functions
        public static bool IsClientExist(int clientId)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(clientId.ToString());
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        public static bool IsClientEnabled(int clientId)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(clientId.ToString());
                if (result)
                {
                    var user = c.GetUserAttributes(clientId.ToString());
                    result = user.Enabled;
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        /// <summary>
        /// 1. Create parent user using: client id + primary user password
        /// 2. Create group using: client id
        /// 3. Join parent user to group
        /// 4. Create child user using: primary user email + primary user password
        /// 5. Join child user to group
        /// 6. Create shared folders
        /// </summary>
        /// <param name="cuser"></param>
        /// <returns></returns>
        public static bool CreateClient(int clientId, int userId)
        {
            bool result = false;

            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var ctx = new xPort5Entities())
            {
                var cuser = ctx.vwClientUserList.Where(x => x.ClientId == clientId && x.PrimaryUser == true).SingleOrDefault();
                if (cuser != null)
                {
                    string group = clientId.ToString();
                    string parentId = clientId.ToString(), parentPassword = cuser.UserPassword;
                    string childId = cuser.Email, childPassword = cuser.UserPassword;
                    string cups = "/cups", vps = "/vps", cip3 = "/cip3", plate = "/plate", blueprint = "/blueprint", film = "/film", thumbnail = "/thumbnail", tools = "/tools";

                    try
                    {
                        var p = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);

                        #region Create primary user, group, child user
                        result = p.GroupExists(group) ? true : p.CreateGroup(group);
                        if (result) result = p.UserExists(parentId) ? true : p.CreateUser(parentId, parentPassword);
                        if (result) p.SetUserAttribute(parentId, OCSUserAttributeKey.DisplayName, cuser.ClientName);
                        if (result) result = p.IsUserInGroup(parentId, group) ? true : p.AddUserToGroup(parentId, group);
                        if (result) result = p.IsUserInSubAdminGroup(parentId, group) ? true : p.AddUserToSubAdminGroup(parentId, group);
                        if (result) result = p.UserExists(childId) ? true : p.CreateUser(childId, childPassword);
                        if (result) result = p.IsUserInGroup(childId, group) ? true : p.AddUserToGroup(childId, group);
                        if (result) p.SetUserAttribute(childId, OCSUserAttributeKey.DisplayName, cuser.UserFullName);
                        #endregion

                        #region Crate folders
                        var c = new owncloudsharp.Client(CLOUDDISK_URL, parentId, parentPassword);
                        
                        if (result) result = c.Exists(cups) ? true : c.CreateDirectory(cups);
                        if (result) c.ShareWithGroup(cups, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(vps) ? true : c.CreateDirectory(vps);
                        if (result) c.ShareWithGroup(vps, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(cip3) ? true : c.CreateDirectory(cip3);
                        if (result) c.ShareWithGroup(cip3, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(plate) ? true : c.CreateDirectory(plate);
                        if (result) c.ShareWithGroup(plate, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(blueprint) ? true : c.CreateDirectory(blueprint);
                        if (result) c.ShareWithGroup(blueprint, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(film) ? true : c.CreateDirectory(film);
                        if (result) c.ShareWithGroup(film, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(thumbnail) ? true : c.CreateDirectory(thumbnail);
                        if (result) c.ShareWithGroup(thumbnail, group, Convert.ToInt32(OcsPermission.All));
                        if (result) result = c.Exists(tools) ? true : c.CreateDirectory(tools);
                        if (result) c.ShareWithGroup(tools, group, Convert.ToInt32(OcsPermission.All));
                        #endregion

                        #region Copy default files from Admin and Modify PPD for this client
                        if (result)
                        {
                            #region 將 admin 嘅 /tools/*.* 抄去 client /tools/.
                            var items = p.List(tools);
                            if (items.Count > 0)
                            {
                                foreach (var item in items)
                                {
                                    var filename = item.Name.Replace("%20", " ");
                                    using (var stream = p.Download(Path.Combine(tools, filename)))
                                    {
                                        var localfile = @"C:\Temp\" + filename;
                                        using (FileStream fs = new FileStream(localfile, FileMode.OpenOrCreate))
                                        {
                                            stream.Seek(0, SeekOrigin.Begin);
                                            stream.CopyTo(fs);

                                            fs.Flush();
                                        }
                                        using (var fs = new FileStream(localfile, FileMode.Open, FileAccess.Read))
                                        {
                                            var uploaded = c.Upload(Path.Combine(tools, filename), fs, "application/octet-stream");
                                            if (uploaded)
                                            {
                                                File.Delete(localfile);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 改啲 PPD 入面嘅 CustomerNumber
                            var ppdCtp = "/tools/CTP.ppd";
                            var ppdFilm = "/tools/FILM.ppd";

                            if (c.Exists(ppdCtp))
                            {
                                using (var streamCtp = c.Download(ppdCtp))
                                {
                                    if (streamCtp != null)
                                    {
                                        var source = Encoding.ASCII.GetString(((MemoryStream)streamCtp).ToArray());
                                        var custom = source.Replace("999999", clientId.ToString());
                                        var memstream = new MemoryStream(ASCIIEncoding.Default.GetBytes(custom));
                                        c.Upload(ppdCtp, memstream, "text/plain");
                                    }
                                }
                            }
                            if (c.Exists(ppdFilm))
                            {
                                using (var streamFilm = c.Download(ppdFilm))
                                {
                                    if (streamFilm != null)
                                    {
                                        var source = Encoding.ASCII.GetString(((MemoryStream)streamFilm).ToArray());
                                        var custom = source.Replace("999999", clientId.ToString());
                                        var memstream = new MemoryStream(ASCIIEncoding.Default.GetBytes(custom));
                                        c.Upload(ppdFilm, memstream, "text/plain");
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region All done, send notifications
                        var msgTitle = "Create Cloud Disk Account";
                        var msgBody = String.Format("Your request is done. Time elapsed: {0:hh\\:mm\\:ss}. Client Id: {1}.", stopwatch.Elapsed, clientId.ToString());
                        var mobileIds = UserHelper.GetUserMobileDeviceTokens(userId);
                        var deviceIds = UserHelper.GetUserMobileDeviceIds(userId);

                        if (!String.IsNullOrEmpty(mobileIds))
                        {
                            FCMHelper.SendPushNotification(Config.FCM_ServerKey, mobileIds, Config.FCM_SenderId, msgTitle, msgBody);
                        }
                        var email = UserHelper.GetUserEmail(userId);
#if DEBUG
                        {
                            email = "paulusyeung@gmail.com";
                        }
#endif
                        if (!String.IsNullOrEmpty(email))
                        {
                            EmailHelper.EmailMessage(email, msgTitle, msgBody);
                        }

                        log.Info(String.Format("[bot, CloudDisk, CreateClient] \r\nHangfire job done.\r\nClient Id = {0}, User Id = {1}", clientId.ToString(), userId.ToString()));
                        #endregion

                        #region 有 userid，log it
                        var user = ctx.User.Where(x => x.UserId == userId).SingleOrDefault();
                        if (user != null)
                        {
                            var hst = new FCMHistory();
                            hst.MessageTitle = msgTitle;
                            hst.MessageBody = msgBody;
                            hst.DeliveredOn = DateTime.Now;
                            hst.Topic = "Device";
                            hst.RecipientList = mobileIds;
                            hst.UserIdList = user.UserSid.ToString(); ;

                            var okay = FCMHistoryHelper.WriteHistory(hst);
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        //
                    }
                }
            }
            stopwatch.Stop();
            return result;
        }

        public static bool CreateSubClient(int clientId, String login, String password)
        {
            bool result = false;

            using (var ctx = new xPort5Entities())
            {
                var puser = ctx.Client_User.Where(x => x.ClientID == clientId && x.PrimaryUser == true).SingleOrDefault();
                if (puser != null)
                {
                    string group = clientId.ToString();
                    string parentId = clientId.ToString(), parentPassword = puser.Password;
                    string childId = login, childPassword = password;

                    try
                    {
                        var p = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                        result = p.GroupExists(group) ? true : p.CreateGroup(group);
                        if (p.UserExists(parentId))
                        {
                            if (result) result = p.UserExists(childId) ? true : p.CreateUser(childId, childPassword);
                            if (result) result = p.IsUserInGroup(childId, group) ? true : p.AddUserToGroup(childId, group);
                        }
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                }
            }
            return result;
        }

        public static bool UpdateClient(String login, String password)
        {
            bool result = false;

            var p = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
            if (p.UserExists(login))
            {
                result = p.SetUserAttribute(login, OCSUserAttributeKey.Password, password);
            }

            return result;
        }

        public static bool DeleteClient(int clientId)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(clientId.ToString());
                if (result)
                {
                    result = c.DeleteUser(clientId.ToString());
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        public static bool DeleteClient(String login)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(login);
                if (result)
                {
                    result = c.DeleteUser(login);
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        public static bool DisableClient(int clientId)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(clientId.ToString());
                if (result)
                {
                    result = c.DisableUser(clientId.ToString());
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        public static bool DisableClient(String login)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(login);
                if (result)
                {
                    result = c.DisableUser(login);
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        public static bool EnableClient(int clientId)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(clientId.ToString());
                if (result)
                {
                    result = c.EnableUser(clientId.ToString());
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }

        public static bool EnableClient(String login)
        {
            bool result = false;

            try
            {
                var c = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                result = c.UserExists(login);
                if (result)
                {
                    result = c.EnableUser(login);
                }
            }
            catch (Exception ex)
            {
                //
            }

            return result;
        }
        */

        public static bool MigrateFiles(Guid userId)
        {
            bool result = false;

            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var ctx = new xPort5Entities())
            {
                try
                {
                    String serverUri = "", userName = "", userPassword = "", sourcePath = "", wildcard = "";

                    /**

                    #region Migrate Cups files
                    serverUri = ConfigurationManager.AppSettings["Cups_ServerUri"];
                    userName = ConfigurationManager.AppSettings["Cups_UserName"];
                    userPassword = ConfigurationManager.AppSettings["Cups_UserPassword"];
                    sourcePath = String.Format("{0}{1}", serverUri, ConfigurationManager.AppSettings["Cups_SourcePath"]);

                    wildcard = String.Format("*.{0}.*", clientId.ToString());
                    using (new Impersonation(serverUri, userName, userPassword))
                    {
                        var items = from file in Directory.GetFiles(sourcePath, wildcard, SearchOption.TopDirectoryOnly).OrderBy(x => x) select file;

                        foreach (var item in items)
                        {
                            var filename = Path.GetFileName(item);
                            UploadCupsFile(filename);
                        }
                    }
                    #endregion

                    #region Migrate VPS files
                    serverUri = ConfigurationManager.AppSettings["Vps_ServerUri"];
                    userName = ConfigurationManager.AppSettings["Vps_UserName"];
                    userPassword = ConfigurationManager.AppSettings["Vps_UserPassword"];
                    sourcePath = String.Format("{0}{1}", serverUri, ConfigurationManager.AppSettings["Vps_SourcePath"]);

                    wildcard = String.Format("{0}.*", clientId.ToString());
                    using (new Impersonation(serverUri, userName, userPassword))
                    {
                        var items = from file in Directory.GetFiles(sourcePath, wildcard, SearchOption.TopDirectoryOnly).OrderBy(x => x) select file;

                        foreach (var item in items)
                        {
                            var filename = Path.GetFileName(item);
                            UploadVpsFile(filename);
                        }
                    }
                    #endregion

                    #region Migrate Cip3 files
                    serverUri = ConfigurationManager.AppSettings["Cip3_ServerUri"];
                    userName = ConfigurationManager.AppSettings["Cip3_UserName"];
                    userPassword = ConfigurationManager.AppSettings["Cip3_UserPassword"];
                    sourcePath = String.Format("{0}{1}", serverUri, ConfigurationManager.AppSettings["Cip3_SourcePath"]);

                    wildcard = String.Format("{0}.*", clientId.ToString());
                    using (new Impersonation(serverUri, userName, userPassword))
                    {
                        var items = from file in Directory.GetFiles(sourcePath, wildcard, SearchOption.TopDirectoryOnly).OrderBy(x => x) select file;

                        foreach (var item in items)
                        {
                            var filename = Path.GetFileName(item);
                            UploadCip3File(filename);
                        }
                    }
                    #endregion

                    #region Migrate Tiff (plate) files
                    serverUri = ConfigurationManager.AppSettings["Plate_ServerUri"];
                    userName = ConfigurationManager.AppSettings["Plate_UserName"];
                    userPassword = ConfigurationManager.AppSettings["Plate_UserPassword"];
                    sourcePath = String.Format("{0}{1}", serverUri, ConfigurationManager.AppSettings["Plate_SourcePath"]);

                    wildcard = String.Format("*.{0}.*", clientId.ToString());
                    using (new Impersonation(serverUri, userName, userPassword))
                    {
                        var items = from file in Directory.GetFiles(sourcePath, wildcard, SearchOption.TopDirectoryOnly).OrderBy(x => x) select file;

                        foreach (var item in items)
                        {
                            //var filename = Path.GetFileName(item);
                            UploadPlateFile(item);
                        }
                    }
                    #endregion

                    #region Migrate Tiff (blueprint) files
                    serverUri = ConfigurationManager.AppSettings["Blueprint_ServerUri"];
                    userName = ConfigurationManager.AppSettings["Blueprint_UserName"];
                    userPassword = ConfigurationManager.AppSettings["Blueprint_UserPassword"];
                    sourcePath = String.Format("{0}{1}", serverUri, ConfigurationManager.AppSettings["Blueprint_SourcePath"]);

                    wildcard = String.Format("*.{0}.*", clientId.ToString());
                    using (new Impersonation(serverUri, userName, userPassword))
                    {
                        var items = from file in Directory.GetFiles(sourcePath, wildcard, SearchOption.TopDirectoryOnly).OrderBy(x => x) select file;

                        foreach (var item in items)
                        {
                            //var filename = Path.GetFileName(item);
                            UploadBlueprintFile(item);
                        }
                    }
                    #endregion

                    #region Migrate ps (film) files
                    serverUri = ConfigurationManager.AppSettings["Film_ServerUri"];
                    userName = ConfigurationManager.AppSettings["Film_UserName"];
                    userPassword = ConfigurationManager.AppSettings["Film_UserPassword"];
                    sourcePath = String.Format("{0}{1}", serverUri, ConfigurationManager.AppSettings["Film_SourcePath"]);

                    wildcard = String.Format("{0}.*", clientId.ToString());
                    using (new Impersonation(serverUri, userName, userPassword))
                    {
                        var items = from file in Directory.GetFiles(sourcePath, wildcard, SearchOption.TopDirectoryOnly).OrderBy(x => x) select file;

                        foreach (var item in items)
                        {
                            //var filename = Path.GetFileName(item);
                            UploadFilmFile(item);
                        }
                    }
                    #endregion

                    #region All done, send notifications
                    var msgTitle = "Sync Cloud Disk";
                    var msgBody = String.Format("Your request is done. Time elapsed: {0:hh\\:mm\\:ss}. Client Id: {1}.", stopwatch.Elapsed, clientId.ToString());
                    var mobileIds = UserHelper.GetUserMobileDeviceTokens(userId);
                    var deviceIds = UserHelper.GetUserMobileDeviceIds(userId);

                    if (!String.IsNullOrEmpty(mobileIds))
                    {
                        FCMHelper.SendPushNotification(Config.FCM_ServerKey, mobileIds, Config.FCM_SenderId, msgTitle, msgBody);
                    }
                    var email = UserHelper.GetUserEmail(userId);
#if DEBUG
                    {
                        email = "paulusyeung@gmail.com";
                    }
#endif
                    if (!String.IsNullOrEmpty(email))
                    {
                        EmailHelper.EmailMessage(email, msgTitle, msgBody);
                    }

                    log.Info(String.Format("[bot, CloudDisk, MigrateFiles] \r\nHangfire job done.\r\nClient Id = {0}, User Id = {1}", clientId.ToString(), userId.ToString()));
                    #endregion

                    #region 有 userid，log it
                    var user = ctx.User.Where(x => x.UserId == userId).SingleOrDefault();
                    if (user != null)
                    {
                        var hst = new FCMHistory();
                        hst.MessageTitle = msgTitle;
                        hst.MessageBody = msgBody;
                        hst.DeliveredOn = DateTime.Now;
                        hst.Topic = "Device";
                        hst.RecipientList = mobileIds;
                        hst.UserIdList = user.UserSid.ToString(); ;

                        var okay = FCMHistoryHelper.WriteHistory(hst);
                    }
                    #endregion

                    */
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        public static List<String> GetSubAdminUsers()
        {
            // 2018.09.12 paulus: 試下用 MemoryCache 改善個 response 速度
            List<string> result = CacheHelper.GetObjectFromCache<List<string>>("getSubAdminUsers", 60, () => {

                #region if cache not exist or is expired, create here
                var list = new List<String>();
                var p = new owncloudsharp.Client(CLOUDDISK_URL, CLOUDDISK_ADMIN, CLOUDDISK_ADMINPASSWORD);
                var items = p.GetUsers();
                foreach (var item in items)
                {
                    if (p.IsUserInSubAdminGroup(item, item))
                    {
                        list.Add(item);
                    }
                }

                return list;
                #endregion
            });
            return result;
        }

        #region handy private objects

        /// <summary>
        /// example: N10549.202856.CTP50.525x459.SM-CHQ-form.ps
        ///          MA6102.202706.CTP50_15459075.510x400.op180612-A03-Chiyu-Namecard-129571-1.ps
        ///          A11963.203080.CTP50.745x605.Nongs_coupon_90x55mnm_op.pdf
        ///          
        ///          JobId.ClientId.PQueue.PSize.PFileName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static FileNaming ParseCupsFileName(string source)
        {
            var result = new FileNaming();

            var parts = source.Split('.');

            result.Raw = source;
            result.ClientId = int.Parse(parts[1]);
            result.JobId = parts[0];
            result.PQueue = parts[2];
            result.PSize = parts[3];
            result.PFileName = source.Substring(parts[0].Length + parts[1].Length + parts[2].Length + parts[3].Length + 4);
            result.PFileExtension = parts[parts.Length - 1];

            return result;
        }

        /// <summary>
        /// example: 202926.N10657-LemonCR.p1(C).VPS
        ///          202204.A12203-封面新會商會、陳呂重德2018出菲林_f.p1(K).VPS
        ///          202706.MA6176-op180620-W05-UOB-CA-79_13.p1(PMS_485_C).VPS
        ///          
        ///          ClientId.JobId-PFileName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static FileNaming ParseVpsFileName(string source)
        {
            var result = new FileNaming();

            try
            {
                var parts = source.Split('.');

                result.Raw = source;
                result.ClientId = int.Parse(parts[0]);
                var part2 = parts[1].Split('-');
                result.JobId = part2[0];
                result.PQueue = "";
                result.PSize = "";
                result.PFileName = source.Substring(parts[0].Length + part2[0].Length + 2);
                result.PFileExtension = "VPS";
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// example: 202926.N10657-LemonCR.p1(CMYK).tif
        ///          202204.A12203-封面新會商會、陳呂重德2018出菲林_f.p1(CMYK).tif
        ///          202706.MA6176-op180620-W05-UOB-CA-79_13.p1(CMYK).tif
        ///          
        ///          ClientId.JobId-PFileName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static FileNaming ParseLowResTiffFileName(string source)
        {
            var result = new FileNaming();

            try
            {
                var parts = source.Split('.');

                result.Raw = source;
                result.ClientId = int.Parse(parts[0]);
                var part2 = parts[1].Split('-');
                result.JobId = part2[0];
                result.PQueue = "";
                result.PSize = "";
                result.PFileName = source.Substring(parts[0].Length + part2[0].Length + 2);
                result.PFileExtension = "tif";
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// example: 202204.A12198-新會商會2018出鋅_x3.p1.ppf
        ///          203080.A12192-ACUVUE_A4_PriceList_op_new.p1.ppf
        ///          202706.MA6179-op180620-B06-BANK_OF_COMMUNICATIONS-129378-2.p1.ppf
        ///          
        ///          ClientId.JobId-PFileName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static FileNaming ParseCip3FileName(string source)
        {
            var result = new FileNaming();

            var parts = source.Split('.');

            result.Raw = source;
            result.ClientId = int.Parse(parts[0]);
            var part2 = parts[1].Split('-');
            result.JobId = part2[0];
            result.PQueue = "";
            result.PSize = "";
            result.PFileName = source.Substring(parts[0].Length + part2[0].Length + 2);
            result.PFileExtension = "ppf";

            return result;
        }

        private class FileNaming
        {
            public string Raw { get; set; }
            public string JobId { get; set; }
            public int ClientId { get; set; }
            public string PQueue { get; set; }
            public string PSize { get; set; }
            public string PFileName { get; set; }
            public string PFileExtension { get; set; }
        }
        #endregion
    }
}
