using Microsoft.AspNetCore.Mvc;

namespace Netnr.Blog.Web.Areas.Doc.Controllers
{
    [Area("Doc")]
    public class DiscoverController : Controller
    {
        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="q"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public IActionResult Index(string q, int page = 1)
        {
            var uinfo = new Application.UserAuthService(HttpContext).Get();

            var ps = Application.CommonService.DocQuery(q, 0, uinfo.UserId, page);
            ps.Route = Request.Path;
            ViewData["q"] = q;
            return View("_PartialDocList", ps);
        }
    }
}