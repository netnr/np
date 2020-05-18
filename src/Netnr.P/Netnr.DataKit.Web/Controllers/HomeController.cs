using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Netnr.DataKit.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 5)]
        public IActionResult Index()
        {
            //先请求本地 /index.html，无效则从线上请求
            string ih;
            var uipath = Path.Combine(GlobalTo.WebRootPath, "index.html");
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
    }
}
