namespace Netnr.DataKit.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 10)]
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(AppTo.WebRootPath, "index.html"), "text/html");
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
