using Netnr.Core;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 工具
    /// </summary>
    [Route("[controller]/[action]")]
    public class ToolController : Controller
    {
        #region 服务器信息

        /// <summary>
        /// 服务器信息
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ServerInfo()
        {
            return View();
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 10)]
        public SharedResultVM ServerStatus()
        {
            var vm = new SharedResultVM();

            try
            {
                var ckss = "Global_SystemStatus";
                if (CacheTo.Get(ckss) is not SystemStatusTo ss)
                {
                    ss = new SystemStatusTo();
                    CacheTo.Set(ckss, ss, 10, false);
                }

                vm.Log.Add(ss);
                vm.Data = ss.ToView();
                vm.Set(SharedEnum.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        #endregion

        #region 退出

        /// <summary>
        /// 退出应用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void Exit()
        {
            //Environment.Exit(0);
            //Process.GetCurrentProcess().Kill();
        }

        #endregion
    }
}