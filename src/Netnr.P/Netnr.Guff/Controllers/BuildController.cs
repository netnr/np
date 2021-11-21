using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.Guff.Controllers
{
    public class BuildController : Controller
    {
        /// <summary>
        /// 构建静态文件
        /// </summary>
        /// <returns></returns>
        public SharedResultVM Index()
        {
            //设置是构建访问
            var cacheKey = GlobalTo.GetValue("Common:BuildHtmlKey");
            CacheTo.Set(cacheKey, true);

            var vm = new SharedApp.BuildTo(HttpContext).Html<HomeController>();

            CacheTo.Remove(cacheKey);

            return vm;
        }
    }
}
