using Hangfire;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using xPort5.Bot.Helper;
using xPort5.EF6;

namespace xPort5.Bot.Controllers
{
    [RoutePrefix("CloudDisk")]
    public class CloudDiskController : ApiController
    {
        private DateTime _DateZero = new DateTime(2017, 1, 1);
        private int _PageSize = 50;

        #region log4net: Instead of naming my invoking class, I started using the following:
        //private static log4net.ILog Log { get; set; }
        //ILog log = log4net.LogManager.GetLogger(typeof(CloudDiskController));

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // In this way, I can use the same line of code in every class that uses log4net without having to remember to change code when I copy and paste.
        // Alternatively, i could create a logging class, and have every other class inherit from my logging class.
        // Refer: https://stackoverflow.com/questions/7089286/correct-way-of-using-log4net-logger-naming
        #endregion

        /// <summary>
        /// 將啲 xPort3 嘅 Product Images 同埋 Attachment 抄去 xPort5 嘅 NextCloud
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MigrateFile/{userId:guid}/")]
        public IHttpActionResult PostMigrateFile(Guid userId)
        {
            using (var ctx = new xPort5Entities())
            {
                var user = ctx.Staff.Where(x => x.StaffId == userId && x.Status > 0).SingleOrDefault();
                if (user != null)
                {
                    BackgroundJob.Enqueue(() => CloudDiskHelper.MigrateFiles(userId));

                    log.Info(String.Format("[bot, CloudDisk, MigrateFile] \r\nHangfire accepted the Job\r\nUser Id = {0}", userId.ToString()));

                    return StatusCode(HttpStatusCode.Accepted);     // 202 or use: return new StatusCodeResult(202);
                }
            }

            log.Info(String.Format("[bot, CloudDisk, MigrateFile] \r\nError found before submitting to Hangfire\r\nUser Id = {0}", userId.ToString()));

            return BadRequest();
        }

        /**

        [HttpPost]
        [Route("CreateClient/{clientId:int}/{userId:int}/")]
        public IHttpActionResult PostCreateClient(int clientId, int userId)
        {
            using (var ctx = new xPort5Entities())
            {
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    BackgroundJob.Enqueue(() => CloudDiskHelper.CreateClient(clientId, userId));

                    log.Info(String.Format("[bot, CloudDisk, CreateClient] \r\nHangfire accepted the Job\r\nClient Id = {0}", clientId.ToString()));

                    return StatusCode(HttpStatusCode.Accepted);     // 202 or use: return new StatusCodeResult(202);
                }
            }

            log.Info(String.Format("[bot, CloudDisk, CreateClient] \r\nError found before submitting to Hangfire\r\nClient Id = {0}", clientId.ToString()));

            return BadRequest();
        }

        [HttpPost]
        [Route("ApiCupsUploadFile/{cupsJobTitle}/")]
        public IHttpActionResult PostApiCupsUploadFile(string cupsJobTitle)
        {
            using (var ctx = new xPort5Entities())
            {
                BackgroundJob.Enqueue(() => CloudDiskHelper.ApiCupsUploadFile(cupsJobTitle));

                log.Info(String.Format("[bot, CloudDisk, ApiCupsUploadFile] \r\nHangfire accepted the Job\r\nCupJobTitle = {0}", cupsJobTitle));

                return StatusCode(HttpStatusCode.Accepted);     // 202 or use: return new StatusCodeResult(202);
            }

            log.Info(String.Format("[bot, CloudDisk, ApiCupsUploadFile] \r\nError found before submitting to Hangfire\r\nCupJobTitle = {0}", cupsJobTitle));

            return BadRequest();
        }

        [HttpPost]
        [Route("ApiVpsUploadFile/{vpsFileName}/")]
        public IHttpActionResult PostApiVpsUploadFile(string vpsFileName)
        {
            using (var ctx = new xPort5Entities())
            {
                BackgroundJob.Enqueue(() => CloudDiskHelper.ApiVpsUploadFile(vpsFileName));

                log.Info(String.Format("[bot, CloudDisk, ApiVpsUploadFile] \r\nHangfire accepted the Job\r\nVpsFileName = {0}", vpsFileName));

                return StatusCode(HttpStatusCode.Accepted);     // 202 or use: return new StatusCodeResult(202);
            }

            log.Info(String.Format("[bot, CloudDisk, ApiVpsUploadFile] \r\nError found before submitting to Hangfire\r\nVpsFileName = {0}", vpsFileName));

            return BadRequest();
        }

        [HttpPost]
        [Route("ApiCip3UploadFile/{cip3FileName}/")]
        public IHttpActionResult PostApiCip3UploadFile(string cip3FileName)
        {
            using (var ctx = new xPort5Entities())
            {
                BackgroundJob.Enqueue(() => CloudDiskHelper.ApiCip3UploadFile(cip3FileName));

                log.Info(String.Format("[bot, CloudDisk, ApiCip3UploadFile] \r\nHangfire accepted the Job\r\nCip3FileName = {0}", cip3FileName));

                return StatusCode(HttpStatusCode.Accepted);     // 202 or use: return new StatusCodeResult(202);
            }

            log.Info(String.Format("[bot, CloudDisk, ApiCip3UploadFile] \r\nError found before submitting to Hangfire\r\nCip3FileName = {0}", cip3FileName));

            return BadRequest();
        }

        [HttpPost]
        [Route("ApiLowResTiffUploadFile/{tiffFileName}/")]
        public IHttpActionResult PostApiLowResTiffUploadFile(string tiffFileName)
        {
            using (var ctx = new xPort5Entities())
            {
                BackgroundJob.Enqueue(() => CloudDiskHelper.ApiLowResTiffUploadFile(tiffFileName));

                log.Info(String.Format("[bot, CloudDisk, ApiLowResTiffUploadFile] \r\nHangfire accepted the Job\r\nTiffFileName = {0}", tiffFileName));

                return StatusCode(HttpStatusCode.Accepted);     // 202 or use: return new StatusCodeResult(202);
            }

            log.Info(String.Format("[bot, CloudDisk, ApiLowResTiffUploadFile] \r\nError found before submitting to Hangfire\r\nTiffFileName = {0}", tiffFileName));

            return BadRequest();
        }

        #region Get Cups, Cip3, Vps, Blueprint, Plate, Film, thumbnail
        [HttpGet]
        [Route("cups/{clientId:int}/{page:int}/")]
        public  IHttpActionResult GetCups(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/cups";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path):                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);    // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("cups/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetCups(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/cups";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("cip3/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetCip3(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/cip3";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);    // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("cip3/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetCip3(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/cip3";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("vps/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetVps(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/vps";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);    // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("vps/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetVps(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/vps";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("blueprint/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetBlueprint(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/blueprint";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);    // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("blueprint/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetBlueprint(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/blueprint";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("blueprint/group/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetBlueprintGroup(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/blueprint";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var all = CloudDiskHelper.FileLst(clientId, 0, path);

                    // 切咗 page(color).VPS 去
                    for (int i = 0; i < all.Count; ++i)
                    {
                        all[i].Name = all[i].Name.Substring(0, all[i].Name.LastIndexOf('('));
                    }

                    // 將同名嘅抽一個 record 出嚟
                    var result = page == 0 ?
                        all.GroupBy(x => x.Name, (key, g) => g.OrderBy(e => e.Name).First()).OrderByDescending(x => x.LastModified).ToList() :
                        all.GroupBy(x => x.Name, (key, g) => g.OrderBy(e => e.Name).First()).OrderByDescending(x => x.LastModified).Take(_PageSize * page).ToList();

                    if (result != null)
                        return Json(result);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("blueprint/group/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetBlueprintGroup(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/blueprint";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var all = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();

                    // 切咗 page(color).VPS 去
                    for (int i = 0; i < all.Count; ++i)
                    {
                        all[i].Name = all[i].Name.Substring(0, all[i].Name.LastIndexOf('('));
                    }

                    // 將同名嘅抽一個 record 出嚟
                    var result = all.GroupBy(x => x.Name, (key, g) => g.OrderBy(e => e.Name).First()).OrderByDescending(x => x.LastModified).ToList();

                    if (result != null)
                        return Json(result);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("plate/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetPlate(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/plate";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);     // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("plate/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetPlate(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/plate";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("film/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetFilm(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/film";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);     // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("film/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetFilm(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/film";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("thumbnail/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetThumbnail(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/thumbnail";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);     // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("thumbnail/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetThumbnail(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/thumbnail";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("tools/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetTools(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/tools";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);    // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("tools/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetTools(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/tools";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("speedbox/{clientId:int}/{page:int}/")]
        public IHttpActionResult GetSpeedBox(int clientId, int page)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/speedbox";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = page == 0 ?
                        CloudDiskHelper.FileLst(clientId, 0, path) :                   // all
                        CloudDiskHelper.FileLst(clientId, page * _PageSize, path);    // pages
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }

        [HttpGet]
        [Route("speedbox/keyword/{clientId:int}/{keyword}/")]
        public IHttpActionResult GetSpeedBox(int clientId, String keyword)
        {
            using (var ctx = new xPort5Entities())
            {
                var path = "/speedbox";
                var client = ctx.Client.Where(x => x.ID == clientId && x.Status > 0).SingleOrDefault();
                if (client != null)
                {
                    var list = (CloudDiskHelper.FileLst(clientId, 0, path)).Where(x => x.Name.Contains(keyword)).ToList();
                    if (list != null)
                        return Json(list);
                }
            }
            return null;
        }
        #endregion

        [HttpGet]
        [Route("users/subadmin/{workshop}/")]
        public IHttpActionResult GetSubAdminUsers(string workshop)
        {
            var items = CloudDiskHelper.GetSubAdminUsers();
            if (items.Count > 0)
            {
                using (var ctx = new xPort5Entities())
                {
                    if (workshop.ToLower() == "all")
                    {
                        var all = ctx.vwClientList.Where(c => items.Contains(c.ID.ToString())).OrderBy(x => x.Name).ToList();
                        return Json(all);
                    }
                    else
                    {
                        var list = ctx.vwClientList.Where(c => items.Contains(c.ID.ToString()) && c.BranchName == workshop).OrderBy(x => x.Name).ToList();
                        return Json(list);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// if width or height = 0, return full size image
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("thumbnail/{clientId:int}/{filename}/{width:int}/{height:int}")]
        public HttpResponseMessage GetThumbnail(int clientId, string filename, int width = 100, int height = 100)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var ctx = new xPort5Entities())
            {
                //ctx.Configuration.LazyLoadingEnabled = false;
                var client = ctx.vwClientList.Where(x => x.ID == clientId).SingleOrDefault();
                if (client != null)
                {
                    var img = CloudDiskHelper.Thumbnail(clientId, filename);
                    if (img != null)
                    {
                        try
                        {
                            // 2018.12.05 paulus: 新 thumbnail 改用 LowRes Tiff
                            var isTiff = (filename.Substring(filename.LastIndexOf('.')).Contains("tif")) ? true : false;

                            if (isTiff)
                            {
                                #region 新 code，先將 Tiff 轉成 PNG，再改 size，送出
                                Byte[] pngBytes;

                                using (MemoryStream outStream = new MemoryStream())
                                {
                                    //System.Drawing.Bitmap.FromStream(img).Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                                    Image.FromStream(img).Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                                    //pngBytes = outStream.ToArray();

                                    var suffix = "png";
                                    var pngImage = Image.FromStream(outStream);
                                    var thumb = width == 0 || height == 0 ? pngImage : ImageHelper.FixedSize(pngImage, width, height);
                                    var buffer = ImageHelper.ImageToByteArray(thumb, suffix);

                                    var contentLength = buffer.Length;

                                    //200
                                    //successful
                                    var statuscode = HttpStatusCode.OK;
                                    response = Request.CreateResponse(statuscode);
                                    response.Content = new StreamContent(new MemoryStream(buffer));
                                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                                    response.Content.Headers.ContentLength = contentLength;

                                    ContentDispositionHeaderValue contentDisposition = null;
                                    if (ContentDispositionHeaderValue.TryParse("inline; filename=" + filename, out contentDisposition))
                                    {
                                        response.Content.Headers.ContentDisposition = contentDisposition;
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region 蕭 code，已經係 PNG，改 size，送出
                                var suffix = "png";
                                var image = Image.FromStream(img);

                                //var thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                                var thumb = width == 0 || height == 0 ? image : ImageHelper.FixedSize(image, width, height);
                                var buffer = ImageHelper.ImageToByteArray(thumb, suffix);

                                var contentLength = buffer.Length;

                                //200
                                //successful
                                var statuscode = HttpStatusCode.OK;
                                response = Request.CreateResponse(statuscode);
                                response.Content = new StreamContent(new MemoryStream(buffer));
                                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                                response.Content.Headers.ContentLength = contentLength;

                                ContentDispositionHeaderValue contentDisposition = null;
                                if (ContentDispositionHeaderValue.TryParse("inline; filename=" + filename, out contentDisposition))
                                {
                                    response.Content.Headers.ContentDisposition = contentDisposition;
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = String.Format("Exception throw. Resource \"{0}\" contains error.", filename);
                            HttpError err = new HttpError(message);
                            response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                        }
                    }
                    else
                    {
                        var message = String.Format("Unable to find resource. Resource \"{0}\" may not exist.", filename);
                        HttpError err = new HttpError(message);
                        response = Request.CreateErrorResponse(HttpStatusCode.NotFound, err);
                    }
                }
            }
            return response;
        }

        #region Action: Email, Reprint, Re-Output
        [HttpPost]
        [Route("Action/Email/{clientId:int}")]
        public IHttpActionResult PostActionEmail(int clientId)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Models.CloudDisk.ActionEmail>(json);

            if (data == null)
            {
                log.Error("[bot, CloudDisk, PostSenActionEmail] jsonData == null");
                //return NotFound();
            }
            else
            {
                using (var ctx = new xPort5Entities())
                {
                    var client = ctx.Client.Where(x => x.ID == clientId).SingleOrDefault();
                    if (client != null)
                    {
                        if (!(String.IsNullOrEmpty(data.Recipient)))
                        {
                            var result = CloudDiskHelper.ActionEmail(data, clientId);

                            if (result)
                            {
                                log.Info(String.Format("[bot, CloudDisk, PostActionEmail] \r\njsondata = {0}", json));
                                return Ok();
                            }
                            else
                            {
                                log.Error(String.Format("[bot, CloudDisk, PostActionEmail] \r\nEmailer returns false\r\njsondat = {0}", json));
                            }
                        }
                        else
                        {
                            log.Error("[bot, CloudDisk, PostActionEmail] \r\nInvaoid Recipient IsNullOrEmpty");
                            //return NotFound();
                        }
                    }
                    else
                    {
                        log.Error("[bot, CloudDisk, PostActionEmail] \r\nInvaoid Client Id: " + clientId.ToString());
                        //return NotFound();
                    }
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Route("Action/Reprint/{clientId:int}")]
        public IHttpActionResult PostActionReprint(int clientId)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Models.CloudDisk.ActionReprintEx>(json);

            if (data == null)
            {
                log.Error("[bot, CloudDisk, PostSActionReprint] jsonData == null");
                //return NotFound();
            }
            else
            {
                using (var ctx = new xPort5Entities())
                {
                    var client = ctx.Client.Where(x => x.ID == clientId).SingleOrDefault();
                    if (client != null)
                    {
                        //var result = CloudDiskHelper.ActionReprintCups(data, clientId);
                        // let Hangfire takes over the job
                        BackgroundJob.Enqueue(() => CloudDiskHelper.ActionReprintCups(data, clientId));
                        var result = true;

                        if (result)
                        {
                            log.Info(String.Format("[bot, CloudDisk, PostActionReprint] \r\njsondata = {0}", json));
                            return Ok();
                        }
                        else
                        {
                            log.Error(String.Format("[bot, CloudDisk, PostActionReprint] \r\nReprint returns false\r\njsondat = {0}", json));
                        }
                    }
                    else
                    {
                        log.Error("[bot, CloudDisk, PostActionReprint] \r\nInvaoid Client Id: " + clientId.ToString());
                        //return NotFound();
                    }
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Route("Action/Output/Blueprint/{userId:int}")]
        public IHttpActionResult PostActionReOutputBlueprint(int userId)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Models.CloudDisk.ActionOutputEx>(json);

            if (data == null)
            {
                log.Error("[bot, CloudDisk, PostActionReOutputBlueprint] jsonData == null");
                //return NotFound();
            }
            else
            {
                using (var ctx = new xPort5Entities())
                {
                    var client = ctx.Client.Where(x => x.ID == data.ClientId).SingleOrDefault();
                    if (client != null)
                    {
                        //var result = CloudDiskHelper.ActionOutputBlueprint(data, data.ClientId);
                        // let Hangfire takes over the job
                        BackgroundJob.Enqueue(() => CloudDiskHelper.ActionOutputBlueprint(data, data.ClientId));
                        var result = true;

                        if (result)
                        {
                            log.Info(String.Format("[bot, CloudDisk, PostActionReOutputBlueprint, done] \r\njsondata = {0}", json));
                            return Ok();
                        }
                        else
                        {
                            log.Error(String.Format("[bot, CloudDisk, PostActionReOutputBlueprint] \r\nRe-Output returns false\r\njsondat = {0}", json));
                        }
                    }
                    else
                    {
                        log.Error("[bot, CloudDisk, PostActionReOutputBlueprint] \r\nInvaoid Client Id: " + data.ClientId.ToString());
                        //return NotFound();
                    }
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Route("Action/Output/Plate/{userId:int}")]
        public IHttpActionResult PostActionReOutputPlate(int userId)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Models.CloudDisk.ActionOutputEx>(json);

            if (data == null)
            {
                log.Error("[bot, CloudDisk, PostActionReOutputPlate] jsonData == null");
                //return NotFound();
            }
            else
            {
                using (var ctx = new xPort5Entities())
                {
                    var client = ctx.Client.Where(x => x.ID == data.ClientId).SingleOrDefault();
                    if (client != null)
                    {
                        //var result = CloudDiskHelper.ActionOutputPlate(data, data.ClientId);
                        // let Hangfire takes over the job
                        BackgroundJob.Enqueue(() => CloudDiskHelper.ActionOutputPlate(data, data.ClientId));
                        var result = true;

                        if (result)
                        {
                            log.Info(String.Format("[bot, CloudDisk, PostActionReOutputPlate, done] \r\njsondata = {0}", json));
                            return Ok();
                        }
                        else
                        {
                            log.Error(String.Format("[bot, CloudDisk, PostActionReOutputPlate] \r\nRe-Output returns false\r\njsondat = {0}", json));
                        }
                    }
                    else
                    {
                        log.Error("[bot, CloudDisk, PostActionReOutputPlate] \r\nInvaoid Client Id: " + data.ClientId.ToString());
                        //return NotFound();
                    }
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Route("Action/Output/Film/{userId:int}")]
        public IHttpActionResult PostActionReOutputFilm(int userId)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Models.CloudDisk.ActionOutputEx>(json);

            if (data == null)
            {
                log.Error("[bot, CloudDisk, PostActionReOutputFilm] jsonData == null");
                //return NotFound();
            }
            else
            {
                using (var ctx = new xPort5Entities())
                {
                    var client = ctx.Client.Where(x => x.ID == data.ClientId).SingleOrDefault();
                    if (client != null)
                    {
                        //var result = CloudDiskHelper.ActionOutputFilm(data, data.ClientId);
                        // let Hangfire takes over the job
                        BackgroundJob.Enqueue(() => CloudDiskHelper.ActionOutputFilm(data, data.ClientId));
                        var result = true;

                        if (result)
                        {
                            log.Info(String.Format("[bot, CloudDisk, PostActionReOutputFilm, done] \r\njsondata = {0}", json));
                            return Ok();
                        }
                        else
                        {
                            log.Error(String.Format("[bot, CloudDisk, PostActionReOutputFilm] \r\nRe-Output returns false\r\njsondat = {0}", json));
                        }
                    }
                    else
                    {
                        log.Error("[bot, CloudDisk, PostActionReOutputFilm] \r\nInvaoid Client Id: " + data.ClientId.ToString());
                        //return NotFound();
                    }
                }
            }
            return NotFound();
        }
        #endregion

        */
    }
}
