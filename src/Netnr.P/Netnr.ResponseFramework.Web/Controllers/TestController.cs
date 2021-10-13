using Microsoft.AspNetCore.Authorization;
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
    }
}