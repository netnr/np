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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 10)]
        public async Task<ResultVM> AboutServerStatus()
        {
            var vm = new ResultVM();

            try
            {
                var ckey = "SystemStatus";
                var cval = CacheTo.Get<string>(ckey);
                if (cval == null)
                {
                    var ss = new SystemStatusTo();
                    cval = await ss.ToView();
                    CacheTo.Set(ckey, cval, 10, false);
                }

                vm.Data = cval;
                vm.Set(EnumTo.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
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