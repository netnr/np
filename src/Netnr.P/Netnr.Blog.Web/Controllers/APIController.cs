using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 公共接口
    /// </summary>
    [Route("api/v1/[action]")]
    [Filters.FilterConfigs.AllowCors]
    public partial class APIController : ControllerBase
    {
        /// <summary>
        /// 登录用户个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResultVM UserInfo()
        {
            var vm = new ActionResultVM();

            try
            {
                var uinfo = new Application.UserAuthService(HttpContext).Get();
                if (uinfo.UserId != 0)
                {
                    vm.Data = uinfo;

                    vm.Set(ARTag.success);
                }
                else
                {
                    vm.Set(ARTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Filters.FilterConfigs.WriteLog(HttpContext, ex);
            }

            return vm;
        }

        /// <summary>
        /// 公共上传
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="subdir">可选，自定义子路径，如：/static/draw</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM API98(IFormFile file, string subdir = null)
        {
            var vm = new ActionResultVM();

            if (Request?.Method == "OPTIONS")
            {
                vm.Set(ARTag.success);
                return vm;
            }

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
                        string filename = now.ToString("HHmmss") + Core.RandomTo.NumCode() + ext;

                        if (!string.IsNullOrWhiteSpace(subdir) && !Fast.ParsingTo.IsLinkPath(subdir))
                        {
                            vm.Set(ARTag.invalid);
                            vm.Msg = "subdir 仅为字母、数字";
                        }
                        else
                        {
                            var vpath = Fast.PathTo.Combine(GlobalTo.GetValue("StaticResource:RootDir"), subdir, now.ToString("yyyy'/'MM'/'dd"));
                            
                            var ppath = Fast.PathTo.Combine(GlobalTo.WebRootPath, vpath);

                            if (!Directory.Exists(ppath))
                            {
                                Directory.CreateDirectory(ppath);
                            }

                            using (var fs = new FileStream(Fast.PathTo.Combine(ppath, filename), FileMode.CreateNew))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            var jo = new
                            {
                                server = GlobalTo.GetValue("StaticResource:Server"),
                                path = Fast.PathTo.Combine(vpath, filename)
                            };

                            vm.Data = jo;

                            vm.Set(ARTag.success);
                        }
                    }
                }
                else
                {
                    vm.Set(ARTag.lack);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Console.WriteLine(ex);
                Filters.FilterConfigs.WriteLog(HttpContext, ex);
            }

            return vm;
        }

        /// <summary>
        /// 系统错误码说明
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResultVM API9999()
        {
            var vm = new ActionResultVM();

            try
            {
                var dic = new Dictionary<int, string>();
                foreach (ARTag item in Enum.GetValues(typeof(ARTag)))
                {
                    dic.Add((int)item, item.ToString());
                }
                vm.Data = dic;
                vm.Set(ARTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                Filters.FilterConfigs.WriteLog(HttpContext, ex);
            }

            return vm;
        }
    }
}