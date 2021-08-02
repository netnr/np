using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netnr.ResponseFramework.Data;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class TestController : Controller
    {
        public ContextBase db;
        public TestController(ContextBase cb)
        {
            db = cb;
        }

        public IActionResult Index()
        {
            return Content("Test");
        }
    }
}