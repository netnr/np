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
        public ResultVM AboutServerStatus()
        {
            var vm = new ResultVM();

            try
            {
                var ckss = "Global_SystemStatus";
                var ss = CacheTo.Get<string>(ckss);
                if (ss == null)
                {
                    ss = new SystemStatusTo().ToView();
                    CacheTo.Set(ckss, ss, 10, false);
                }

                vm.Data = ss;
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

        /// <summary>
        /// 动态构建
        /// </summary>
        /// <returns></returns>
        public IActionResult Test()
        {
            var db = ContextBaseFactory.CreateDbContext();

            var filter1 = PredicateTo.Contains<UserInfo>("Nickname", "Contains", "用户"); // x.Nickname like '%用户%'
            var filter2 = PredicateTo.Contains<UserInfo>("Nickname", "StartsWith", "用户"); // x.Nickname like '用户%'
            var filter3 = PredicateTo.Contains<UserInfo>("Nickname", "EndsWith", "用户"); // x.Nickname like '%用户'

            var compare1 = PredicateTo.Compare<UserInfo>("UserId", "=", 1); // x.UserId = 1
            var compare2 = PredicateTo.Compare<UserInfo>("UserId", "!=", 1); // x.UserId != 1
            var compare3 = PredicateTo.Compare<UserInfo>("UserId", ">=", 2); // x.UserId >= 2
            var compare4 = PredicateTo.Compare<UserInfo>("UserId", "<", 20); // x.UserId < 20

            var select1 = PredicateTo.Field<UserInfo, string>("Nickname"); // x.Nickname
            var select2 = PredicateTo.Field<UserInfo, int>("UserId"); // x.UserId

            // x.Nickname like '%用户%' OR x.UserId = 1
            var inner1 = PredicateTo.False<UserInfo>();
            inner1 = inner1.Or(filter1);
            inner1 = inner1.Or(compare1);

            var filterResult1 = db.UserInfo.Where(filter1).Select(select1).ToList();
            Console.WriteLine(filterResult1.ToJson(true));
            var filterResult2 = db.UserInfo.Where(filter2).Select(select1).ToList();
            Console.WriteLine(filterResult2.ToJson(true));
            var filterResult3 = db.UserInfo.Where(filter3).Select(select1).ToList();
            Console.WriteLine(filterResult3.ToJson(true));

            var compareResult1 = db.UserInfo.Where(compare1).Select(select2).ToList();
            Console.WriteLine(compareResult1.ToJson(true));
            var compareResult2 = db.UserInfo.Where(compare2).Select(select2).ToList();
            Console.WriteLine(compareResult2.Take(10).ToJson(true));
            var compareResult3 = db.UserInfo.Where(compare3).Select(select2).ToList();
            Console.WriteLine(compareResult3.ToJson(true));
            var compareResult4 = db.UserInfo.Where(compare4).Select(select2).ToList();
            Console.WriteLine(compareResult4.ToJson(true));

            var innerResult1 = db.UserInfo.Where(inner1).Select(x => new { x.UserId, x.Nickname }).ToList();
            Console.WriteLine(innerResult1.ToJson(true));

            return Ok();
        }

    }
}