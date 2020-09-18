using Microsoft.AspNetCore.Mvc;

namespace Netnr.DataKit.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 5)]
        public IActionResult Index()
        {
            //先请求本地 /index.html，无效则从线上请求
            string ih;
            var uipath = Fast.PathTo.Combine(GlobalTo.WebRootPath, "index.html");
            ih = Core.FileTo.ReadText(uipath);
            if (string.IsNullOrEmpty(ih))
            {
                ih = Core.HttpTo.Get("https://ss.netnr.com/dk");
            }
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
