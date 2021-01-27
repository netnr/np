using Microsoft.AspNetCore.Mvc;
using Netnr.Core;

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
            return View();
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        /// <returns></returns>
        public IActionResult Status()
        {
            if (CacheTo.Get("ss") is not SystemStatusTo ss)
            {
                ss = new SystemStatusTo();
                CacheTo.Set("ss", ss, 10, false);
            }
            return Content(ss.ToView());
        }

        /// <summary>
        /// Swagger自定义样式
        /// </summary>
        /// <returns></returns>
        public IActionResult SwaggerCustomStyle()
        {
            var txt = @".opblock-options{display:none}.download-contents{width:auto !important}";

            return new ContentResult()
            {
                Content = txt,
                StatusCode = 200,
                ContentType = "text/css"
            };
        }
    }
}
