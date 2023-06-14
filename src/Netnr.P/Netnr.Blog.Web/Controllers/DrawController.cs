namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// Draw
    /// </summary>
    public class DrawController : Controller
    {
        public ContextBase db;

        public DrawController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// Draw 新增表单
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            return View("/Views/Draw/_PartialDrawForm.cshtml");
        }

        /// <summary>
        /// 编辑表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var mo = await db.Draw.FirstOrDefaultAsync(x => x.DrId == id && x.Uid == uinfo.UserId);
            if (mo != null)
            {
                return View("/Views/Draw/_PartialDrawForm.cshtml", mo);
            }
            else
            {
                return Unauthorized("401");
            }
        }

        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var num = await db.Draw.Where(x => x.DrId == id && x.Uid == uinfo.UserId).ExecuteDeleteAsync();
            if (num > 0)
            {
                return Redirect($"/draw/user/{uinfo.UserId}");
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<IActionResult> Save([FromForm] Draw model)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                int num = 0;

                //新增
                if (string.IsNullOrWhiteSpace(model.DrId))
                {
                    model.DrId = model.DrType[0] + UniqueTo.LongId().ToString();
                    model.DrCreateTime = DateTime.Now;
                    model.Uid = uinfo?.UserId;
                    model.DrOrder = 100;
                    model.DrStatus = 1;

                    await db.Draw.AddAsync(model);
                    num = await db.SaveChangesAsync();

                    //推送通知
                    _ = PushService.PushWeChat("网站消息（Draw）", $"{model.DrName}");
                }
                else
                {
                    num = await db.Draw.Where(x => x.DrId == model.DrId && x.Uid == uinfo.UserId).ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.DrRemark, model.DrRemark)
                    .SetProperty(p => p.DrName, model.DrName)
                    .SetProperty(p => p.DrOpen, model.DrOpen)
                    .SetProperty(p => p.Spare1, model.Spare1));
                }

                vm.Set(num > 0);
            }

            if (vm.Code == 200)
            {
                return Redirect($"/draw/user/{uinfo.UserId}");
            }
            else
            {
                return BadRequest(vm);
            }
        }

        /// <summary>
        /// Draw 查看
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code">分享码</param>
        /// <returns></returns>
        public IActionResult Code([FromRoute] string id, string code)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/draw/discover");
            }
            else
            {
                var idAsType = "Graph";
                switch (id[0])
                {
                    case 'm': idAsType = "Mind"; break;
                    case 'b': idAsType = "BPMN"; break;
                }

                //有分享码（存储）
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var sharedCode = $"SharedCode_{id}"; //根据主键存储
                    Response.Cookies.Append(sharedCode, code);
                }

                return View($"/Views/Draw/_Partial{idAsType}Editor.cshtml");
            }
        }

        /// <summary>
        /// 编辑器打开
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultVM> EditorOpen([FromRoute] string id)
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);

                var sharedCode = $"SharedCode_{id}";
                var code = Request.Cookies[sharedCode]?.ToString();

                var mo = await db.Draw.FindAsync(id);
                if (mo == null)
                {
                    vm.Set(EnumTo.RTag.failure);
                }
                else if (mo.DrOpen == 1 || mo.Uid == uinfo?.UserId || (!string.IsNullOrWhiteSpace(mo.Spare1) && mo.Spare1 == code))
                {
                    vm.Data = mo;
                    vm.Set(EnumTo.RTag.success);
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 编辑器保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xml"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResultVM> EditorSave([FromRoute] string id, [FromForm] string xml, [FromForm] string filename)
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);

                var num = await db.Draw.Where(x => x.DrId == id && x.Uid == uinfo.UserId).ExecuteUpdateAsync(x => x
                .SetProperty(p => p.DrContent, xml)
                .SetProperty(p => p.DrName, filename));

                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// Mind Upload
        /// </summary>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<IActionResult> MindUpload()
        {
            var errno = -1;
            var msg = "fail";
            var url = "";

            var subdir = AppTo.GetValue("StaticResource:DrawPath");
            var vm = await api.APIController.UploadCheck(Request.Form.Files[0], null, "", subdir);
            if (vm.Code == 200)
            {
                var jd = vm.Data.ToJson().DeJson();
                url = Path.Combine(AppTo.GetValue("StaticResource:Server"), jd.GetValue("path"));
                errno = 0;
                msg = "ok";
            }

            return Content(new
            {
                errno,
                msg,
                data = new
                {
                    url
                }
            }.ToJson());
        }

        /// <summary>
        /// Draw 列表
        /// </summary>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> Discover(string k, int page = 1)
        {
            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.DrawQuery(k, 0, userId, page);
            ps.Route = Request.Path;
            return View("/Views/Draw/_PartialDrawList.cshtml", ps);
        }

        /// <summary>
        /// Draw 用户
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
                return Redirect("/draw");
            }

            int uid = Convert.ToInt32(id);

            var mu = await db.UserInfo.FindAsync(uid);
            if (mu == null)
            {
                return Content("Account is empty");
            }
            ViewData["Nickname"] = mu.Nickname;

            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.DrawQuery(k, uid, userId, page);
            ps.Route = Request.Path;
            return View("/Views/Draw/_PartialDrawList.cshtml", ps);
        }
    }
}