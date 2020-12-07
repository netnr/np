using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Blog.Web.Controllers
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
        /// 公共上传
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="subdir">可选，自定义子路径，如：/static/draw</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM API98(IFormFile file, string subdir = null)
        {
            var vm = new SharedResultVM();

            if (Request?.Method == "OPTIONS")
            {
                vm.Set(SharedEnum.RTag.success);
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
    }
}