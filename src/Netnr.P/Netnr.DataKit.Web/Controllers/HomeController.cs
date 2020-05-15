using Microsoft.AspNetCore.Mvc;
using System;

namespace Netnr.DataKit.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 5)]
        public IActionResult Index()
        {
            var ih = Core.FileTo.ReadText(Environment.CurrentDirectory + "/wwwroot/", "index.html");
            return new ContentResult()
            {
                Content = ih,
                StatusCode = 200,
                ContentType = "text/html"
            };
        }
	}
}
