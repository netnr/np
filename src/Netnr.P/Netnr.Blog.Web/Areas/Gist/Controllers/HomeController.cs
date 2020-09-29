using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netnr.Blog.Data;

namespace Netnr.Web.Areas.Gist.Controllers
{
    [Area("Gist")]
    public class HomeController : Controller
    {
        public ContextBase db;

        public HomeController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// Gist首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View("_PartialMonacoEditor");
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResultVM SaveGist(Blog.Domain.Gist mo)
        {
            var vm = new ActionResultVM();

            var uinfo = new Blog.Application.UserAuthService(HttpContext).Get();

            //add
            if (string.IsNullOrWhiteSpace(mo.GistCode))
            {
                mo.GistId = Guid.NewGuid().ToString();
                mo.GistCreateTime = DateTime.Now;
                mo.GistUpdateTime = mo.GistCreateTime;
                mo.GistStatus = 1;
                mo.Uid = uinfo.UserId;

                mo.GistCode = Core.UniqueTo.LongId().ToString();
                db.Gist.Add(mo);
                db.SaveChanges();

                vm.Data = mo.GistCode;
                vm.Set(ARTag.success);
            }
            else
            {
                var oldmo = db.Gist.FirstOrDefault(x => x.GistCode == mo.GistCode);
                if (oldmo != null)
                {
                    if (oldmo.Uid == uinfo.UserId)
                    {
                        oldmo.GistRemark = mo.GistRemark;
                        oldmo.GistFilename = mo.GistFilename;
                        oldmo.GistLanguage = mo.GistLanguage;
                        oldmo.GistTheme = mo.GistTheme;
                        oldmo.GistContent = mo.GistContent;
                        oldmo.GistContentPreview = mo.GistContentPreview;
                        oldmo.GistRow = mo.GistRow;
                        oldmo.GistOpen = mo.GistOpen;
                        oldmo.GistUpdateTime = DateTime.Now;

                        db.Gist.Update(oldmo);
                        db.SaveChanges();

                        vm.Data = mo.GistCode;
                        vm.Set(ARTag.success);
                    }
                    else
                    {
                        vm.Set(ARTag.unauthorized);
                    }
                }
                else
                {
                    vm.Set(ARTag.invalid);
                }
            }

            return vm;
        }
    }
}
