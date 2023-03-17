using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Netnr.Demo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}