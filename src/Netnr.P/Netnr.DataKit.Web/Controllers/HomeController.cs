using Microsoft.AspNetCore.Mvc;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.DataKit.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 5)]
        public IActionResult Index()
        {
            string ih = FileTo.ReadText(PathTo.Combine(GlobalTo.WebRootPath, "lib/dk/dk.html"));
            return new ContentResult()
            {
                Content = ih,
                StatusCode = 200,
                ContentType = "text/html"
            };
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
