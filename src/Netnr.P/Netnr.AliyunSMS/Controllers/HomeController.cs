using Microsoft.AspNetCore.Mvc;

namespace Netnr.AliyunSMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Application.TaskService.SmsNotSent();

            return Redirect("/swagger");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
