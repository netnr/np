using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Blog.Web.Controllers.api
{
    /// <summary>
    /// 公共接口
    /// </summary>
    [Route("api/v1/[action]")]
    [Apps.FilterConfigs.AllowCors]
    public partial class APIController : ControllerBase
    {
        /// <summary>
        /// 登录用户个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM UserInfo()
        {
            var vm = new SharedResultVM();

            try
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                if (uinfo.UserId != 0)
                {
                    vm.Data = uinfo;

                    vm.Set(SharedEnum.RTag.success);
                }
                else
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Apps.FilterConfigs.WriteLog(HttpContext, ex);
            }

            return vm;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="subdir">可选，默认 /static，自定义子路径，如：/static/draw</param>
        /// <returns></returns>
        [HttpPost]
        [HttpOptions]
        [Apps.FilterConfigs.AllowCors]
        public SharedResultVM Upload(IFormFile file, string subdir = "/static")
        {
            var vm = new SharedResultVM();

            try
            {
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (ext == ".exe" || ext == "")
                    {
                        vm.Code = 2;
                        vm.Msg = "Unsupported file format：" + ext;
                    }
                    else
                    {
                        var now = DateTime.Now;
                        string filename = now.ToString("HHmmss") + RandomTo.NumCode() + ext;

                        if (!string.IsNullOrWhiteSpace(subdir) && !ParsingTo.IsLinkPath(subdir))
                        {
                            vm.Set(SharedEnum.RTag.invalid);
                            vm.Msg = "subdir 仅为字母、数字";
                        }
                        else
                        {
                            var vpath = PathTo.Combine(GlobalTo.GetValue("StaticResource:RootDir"), subdir, now.ToString("yyyy'/'MM'/'dd"));

                            var ppath = PathTo.Combine(GlobalTo.WebRootPath, vpath);

                            if (!Directory.Exists(ppath))
                            {
                                Directory.CreateDirectory(ppath);
                            }

                            using (var fs = new FileStream(PathTo.Combine(ppath, filename), FileMode.CreateNew))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            var jo = new
                            {
                                server = GlobalTo.GetValue("StaticResource:Server"),
                                path = PathTo.Combine(vpath, filename)
                            };

                            vm.Data = jo;

                            vm.Set(SharedEnum.RTag.success);
                        }
                    }
                }
                else
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Console.WriteLine(ex);
                Apps.FilterConfigs.WriteLog(HttpContext, ex);
            }

            return vm;
        }

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="url">代理链接</param>
        /// <param name="charset">编码，默认utf-8，如：gb2312</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpHead]
        [HttpPatch]
        [HttpDelete]
        [HttpOptions]
        [Apps.FilterConfigs.AllowCors]
        public ActionResult Proxy(string url, string charset = "utf-8")
        {
            var result = string.Empty;

            try
            {
                var ci = new SharedApp.ClientTo(HttpContext);
                Console.WriteLine($"PROXY | {Request.Method} | {ci.IPv4} | {url} | {ci.UserAgent}");

                if (!GlobalTo.GetValue<bool>("ReadOnly"))
                {
                    return Ok();
                }

                GlobalTo.EncodingReg();

                var keywordBlacklist = new List<string> { ".m3u8", ".mpd", ".m4v" };
                if (string.IsNullOrWhiteSpace(url) || url.Length < 3)
                {
                    result = "url invalid";
                }
                else if (keywordBlacklist.Any(x => url.Contains(x)))
                {
                    result = $"The keyword '{string.Join(" ", keywordBlacklist)}' was blacklisted by the operator of this proxy.";
                }
                else
                {
                    if (!url.Contains("://"))
                    {
                        url = "http://" + url;
                    }

                    var ct = Request.Headers["Content-Type"].ToString();

                    string data = null;

                    if (Request.Method != "GET")
                    {
                        try
                        {
                            if (ct.StartsWith("application/x-www-form-urlencoded") || ct.StartsWith("multipart/form-data"))
                            {
                                var sb = new StringBuilder();
                                foreach (var key in Request.Form.Keys)
                                {
                                    sb.Append($"&{key}={Request.Form[key].ToString().ToEncode()}");
                                }
                                data = sb.Remove(0, 1).ToString();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    var hwr = HttpTo.HWRequest(url, Request.Method, data, charset);
                    var skipHeader = new List<string> { "Content-Length", "Content-Type", "Referer", "Cookie", "Host", "origin" };
                    foreach (var key in Request.Headers.Keys)
                    {
                        if (!skipHeader.Contains(key))
                        {
                            hwr.Headers[key] = Request.Headers[key];
                        }
                    }

                    result = HttpTo.Url(hwr, charset);
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                var cr = new ContentResult()
                {
                    Content = response.StatusDescription,
                    StatusCode = (int)response.StatusCode,
                    ContentType = response.Headers[HttpRequestHeader.ContentType]
                };
                return cr;
            }
            catch (Exception ex)
            {
                result = new
                {
                    code = -1,
                    msg = ex
                }.ToJson();
            }

            return Content(result);
        }

        #region 任务

        /// <summary>
        /// 任务项
        /// </summary>
        public enum TaskItem
        {
            /// <summary>
            /// 备份数据库
            /// </summary>
            BackupDataBase,
            /// <summary>
            /// 导出数据库
            /// </summary>
            ExportDataBase,
            /// <summary>
            /// 导出示例数据
            /// </summary>
            ExportSampleData,
            /// <summary>
            /// 代码片段同步到GitHub、Gitee
            /// </summary>
            GistSync,
            /// <summary>
            /// 处理操作记录
            /// </summary>
            HOR
        }

        /// <summary>
        /// 手动执行任务（管理员）
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 60)]
        [Apps.FilterConfigs.IsAdmin]
        public SharedResultVM ExecTask(TaskItem? ti)
        {
            var vm = new SharedResultVM();

            try
            {
                if (!ti.HasValue)
                {
                    ti = (TaskItem)Enum.Parse(typeof(TaskItem), RouteData.Values["id"]?.ToString(), true);
                }

                switch (ti)
                {
                    default:
                        vm.Set(SharedEnum.RTag.invalid);
                        break;

                    case TaskItem.BackupDataBase:
                        vm = Application.TaskService.BackupDataBase();
                        break;

                    case TaskItem.ExportDataBase:
                        vm = Application.TaskService.ExportDataBase();
                        break;

                    case TaskItem.ExportSampleData:
                        vm = Application.TaskService.ExportSampleData();
                        break;

                    case TaskItem.GistSync:
                        vm = Application.TaskService.GistSync();
                        break;

                    case TaskItem.HOR:
                        vm = Application.TaskService.HandleOperationRecord();
                        break;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
        #endregion
    }
}