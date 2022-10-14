namespace Netnr.Admin.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var uinfo = IdentityService.Get(HttpContext);
            if (uinfo == null)
            {
                return Content("please log in first");
            }

            return Redirect("/index.html");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Content(Activity.Current?.Id ?? HttpContext.TraceIdentifier);
        }
    }
}