using Microsoft.AspNetCore.Mvc;

namespace Netnr.WeChat.Sample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
