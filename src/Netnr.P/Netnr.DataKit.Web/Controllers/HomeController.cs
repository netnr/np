using Microsoft.AspNetCore.Mvc;

namespace Netnr.DataKit.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 5)]
        public IActionResult Index()
        {
            return Redirect("/lib/ndk/ndk.html");
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
