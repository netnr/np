namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 混合、综合、其它
    /// </summary>
    public class MixController : Controller
    {
        /// <summary>
        /// 关于页面
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 5)]
        public async Task<ResultVM> AboutServerStatus()
        {
            var vm = new ResultVM();

            try
            {
                var ckey = "SystemStatus";
                var cval = CacheTo.Get<SystemStatusTo>(ckey);
                if (cval == null)
                {
                    cval = new SystemStatusTo();
                    await cval.RefreshAll();

                    CacheTo.Set(ckey, cval);
                }
                else if ((DateTime.Now - cval.Now).TotalSeconds > 10)
                {
                    _ = cval.RefreshAll();
                    CacheTo.Set(ckey, cval);
                }

                vm.Data = cval.ToView();
                vm.Set(RCodeTypes.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 条款
        /// </summary>
        /// <returns></returns>
        public IActionResult Terms()
        {
            return View();
        }

        /// <summary>
        /// FAQ
        /// </summary>
        /// <returns></returns>
        public IActionResult FAQ()
        {
            return View();
        }
    }
}