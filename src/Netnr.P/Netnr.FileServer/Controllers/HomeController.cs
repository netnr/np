using Microsoft.AspNetCore.Mvc;

namespace Netnr.FileServer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            Test();
            return Redirect("/swagger");
        }

        public IActionResult Test()
        {
            return Content("");
        }
    }
}
