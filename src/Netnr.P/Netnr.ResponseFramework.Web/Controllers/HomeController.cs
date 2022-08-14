using Microsoft.AspNetCore.Authorization;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 起始页
    /// 
    /// 注意：该页面勿写接口
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var vm = IdentityService.Get(HttpContext);
            return View(vm);
        }

        /// <summary>
        /// 桌面
        /// </summary>
        /// <returns></returns>
        public IActionResult Desk()
        {
            return View();
        }

        /// <summary>
        /// 请升级你的浏览器
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult UB()
        {
            return View();
        }

        /// <summary>
        /// 向导
        /// </summary>
        /// <returns></returns>
        public IActionResult Guide()
        {
            return View();
        }

        /// <summary>
        /// Swagger自定义样式
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
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
