namespace Netnr.DataKit.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(AppTo.WebRootPath, "index.html"), "text/html");
        }
    }
}
