using Microsoft.AspNetCore.Mvc;

namespace Netnr.Web.Areas.Gist.Controllers
{
    [Area("Gist")]
    public class DiscoverController : Controller
    {
        /// <summary>
        /// Gist列表
        /// </summary>
        /// <param name="q"></param>
        /// <param name="lang"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IActionResult Index(string q, string lang, int page = 1)
        {
            var uinfo = new Blog.Application.UserAuthService(HttpContext).Get();

            var ps = Blog.Application.CommonService.GistQuery(q, lang, 0, uinfo.UserId, page);
            ps.Route = Request.Path;
            ViewData["lang"] = lang;
            ViewData["q"] = q;
            return View("_PartialGistList", ps);
        }
    }
}