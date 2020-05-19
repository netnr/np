using System.Linq;
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

        /// <summary>
        /// 系统字典表页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            var mo = db.SysLog.FirstOrDefault();
            var result = mo.ToJson();

            return Content(result);
        }

    }
}