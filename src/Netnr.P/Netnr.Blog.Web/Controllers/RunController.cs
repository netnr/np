namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// Run
    /// </summary>
    public class RunController : Controller
    {
        public ContextBase db;

        public RunController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// Run 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View("/Views/Run/_PartialRunEditor.cshtml");
        }

        /// <summary>
        /// 查询 一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Code([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/run/discover");
            }

            var uinfo = IdentityService.Get(HttpContext);

            //view
            var model = await db.Run.FirstOrDefaultAsync(x => x.RunCode == id && x.RunStatus == 1);
            if (model == null)
            {
                return NotFound();
            }
            else if (model.RunOpen != 1 && model.Uid != uinfo.UserId)
            {
                return Unauthorized(401);
            }
            else
            {
                var html = model.RunContent1;
                var totalDays = BaseTo.StartTime.ToUtcTotalDays();

                var injectJS = $"\n<script src='/file/run-oconsole.js?{totalDays}'></script>\n" +
                    (string.IsNullOrWhiteSpace(model.RunContent2)
                    ? "" : $"<script>\n{model.RunContent2}\n</script>\n");

                var injectCSS = string.IsNullOrWhiteSpace(model.RunContent3)
                    ? "" : $"\n<style>\n{model.RunContent3}\n</style>\n";

                if (model.RunContent1.Contains("</head>") && model.RunContent1.Contains("</body>"))
                {
                    html = html.Replace("</head>", $"{injectCSS}</head>").Replace("</body>", $"{injectJS}</body>");
                }
                else
                {
                    html = $"{html}{injectCSS}{injectJS}";
                }

                return new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "text/html; charset=utf-8",
                    Content = html
                };
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/run/discover");
            }

            var mo = await db.Run.FirstOrDefaultAsync(x => x.RunCode == id);
            if (mo != null)
            {
                return View("/Views/Run/_PartialRunEditor.cshtml", mo);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/run/discover");
            }

            var uinfo = IdentityService.Get(HttpContext);

            var num = await db.Run.Where(x => x.RunCode == id && x.Uid == uinfo.UserId).ExecuteDeleteAsync();
            if (num > 0)
            {
                return Redirect($"/run/user/{uinfo.UserId}");
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Run 保存
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public ResultVM Save([FromForm] Run mo)
        {
            var vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                var uinfo = IdentityService.Get(HttpContext);

                //add
                if (string.IsNullOrWhiteSpace(mo.RunCode))
                {
                    mo.RunId = Guid.NewGuid().ToString();
                    mo.RunCreateTime = DateTime.Now;
                    mo.RunStatus = 1;
                    mo.RunOpen = 1;
                    mo.Uid = uinfo.UserId;

                    mo.RunCode = UniqueTo.LongId().ToString();
                    db.Run.Add(mo);
                    int num = db.SaveChanges();

                    vm.Data = mo.RunCode;
                    vm.Set(num > 0);

                    //推送通知
                    _ = PushService.PushWeChat("网站消息（Run）", $"{mo.RunRemark}");
                }
                else
                {
                    var oldmo = db.Run.FirstOrDefault(x => x.RunCode == mo.RunCode);
                    if (oldmo?.Uid == uinfo.UserId)
                    {
                        oldmo.RunContent1 = mo.RunContent1;
                        oldmo.RunContent2 = mo.RunContent2;
                        oldmo.RunContent3 = mo.RunContent3;
                        oldmo.RunRemark = mo.RunRemark;
                        oldmo.RunTheme = mo.RunTheme;

                        db.Run.Update(oldmo);
                        int num = db.SaveChanges();

                        vm.Data = mo.RunCode;
                        vm.Set(num > 0);
                    }
                    else
                    {
                        vm.Set(EnumTo.RTag.failure);
                    }
                }
            }

            return vm;
        }

        /// <summary>
        /// Run 列表
        /// </summary>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> Discover(string k, int page = 1)
        {
            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.RunQuery(k, 0, userId, page);
            ps.Route = Request.Path;

            return View("/Views/Run/_PartialRunList.cshtml", ps);
        }

        /// <summary>
        /// Run 用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ActionName("User")]
        public async Task<IActionResult> Id([FromRoute] string id, string k, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/run/discover");
            }

            int uid = Convert.ToInt32(id);

            var mu = await db.UserInfo.FindAsync(uid);
            if (mu == null)
            {
                return Content("Account is empty");
            }
            ViewData["Nickname"] = mu.Nickname;

            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.RunQuery(k, uid, userId, page);
            ps.Route = Request.Path;

            return View("/Views/Run/_PartialRunList.cshtml", ps);
        }
    }
}
