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
            return Redirect("/swagger");
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        /// <returns></returns>
        public IActionResult Status()
        {
            if (!(Core.CacheTo.Get("osi") is Fast.OSInfoTo osi))
            {
                osi = new Fast.OSInfoTo();
                Core.CacheTo.Set("osi", osi, 10, false);
            }
            return Content(osi.ToView());
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
