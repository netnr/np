namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// Gist
    /// </summary>
    public class GistController : Controller
    {
        public ContextBase db;

        public GistController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// Gist 首页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            return View("/Views/Gist/_PartialGistEditor.cshtml");
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Code([FromRoute] string id)
        {
            var query = from a in db.Gist
                        join b in db.GistSync on a.GistCode equals b.GistCode into bg
                        from b in bg.DefaultIfEmpty()
                        join c in db.UserInfo on a.Uid equals c.UserId
                        where a.GistCode == id
                        select new Gist
                        {
                            GistId = a.GistId,
                            Uid = a.Uid,
                            GistCode = a.GistCode,
                            GistContent = a.GistContent,
                            GistCreateTime = a.GistCreateTime,
                            GistUpdateTime = a.GistUpdateTime,
                            GistFilename = a.GistFilename,
                            GistLanguage = a.GistLanguage,
                            GistOpen = a.GistOpen,
                            GistRemark = a.GistRemark,
                            GistRow = a.GistRow,
                            GistStatus = a.GistStatus,
                            GistTags = a.GistTags,
                            GistTheme = a.GistTheme,

                            Spare1 = b == null ? null : b.GsGitHubId,
                            Spare2 = b == null ? null : b.GsGiteeId,
                            Spare3 = c.Nickname
                        };

            var uinfo = IdentityService.Get(HttpContext);
            if (uinfo == null)
            {
                query = query.Where(x => x.GistOpen == 1 && x.GistStatus == 1);
            }

            var model = await query.FirstOrDefaultAsync();

            return View(model);
        }

        /// <summary>
        /// 编辑一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var mo = await db.Gist.FirstOrDefaultAsync(x => x.GistCode == id && x.Uid == uinfo.UserId);
            if (mo != null)
            {
                return View("/Views/Gist/_PartialGistEditor.cshtml", mo);
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

            var num = await db.Gist.Where(x => x.GistCode == id && x.Uid == uinfo.UserId).ExecuteDeleteAsync();
            if (num > 0)
            {
                return Redirect($"/gist/user/{uinfo.UserId}");
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gist 保存
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<ResultVM> Save([FromForm] Gist mo)
        {
            var vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                var uinfo = IdentityService.Get(HttpContext);

                //add
                if (string.IsNullOrWhiteSpace(mo.GistCode))
                {
                    mo.GistId = Guid.NewGuid().ToString();
                    mo.GistCreateTime = DateTime.Now;
                    mo.GistUpdateTime = mo.GistCreateTime;
                    mo.GistStatus = 1;
                    mo.Uid = uinfo?.UserId;

                    mo.GistCode = UniqueTo.LongId().ToString();
                    await db.Gist.AddAsync(mo);
                    var num = await db.SaveChangesAsync();

                    vm.Set(num > 0);
                    vm.Data = mo.GistCode;

                    //推送通知
                    _ = PushService.PushAsync("网站消息（Gist）", $"{mo.GistRemark}\r\n{mo.GistFilename}");
                }
                else
                {
                    var num = await db.Gist.Where(x => x.GistCode == mo.GistCode && x.Uid == uinfo.UserId)
                        .ExecuteUpdateAsync(x => x
                        .SetProperty(p => p.GistRemark, mo.GistRemark)
                        .SetProperty(p => p.GistFilename, mo.GistFilename)
                        .SetProperty(p => p.GistLanguage, mo.GistLanguage)
                        .SetProperty(p => p.GistTheme, mo.GistTheme)
                        .SetProperty(p => p.GistContent, mo.GistContent)
                        .SetProperty(p => p.GistContentPreview, mo.GistContentPreview)
                        .SetProperty(p => p.GistRow, mo.GistRow)
                        .SetProperty(p => p.GistOpen, mo.GistOpen)
                        .SetProperty(p => p.GistUpdateTime, DateTime.Now));

                    vm.Set(num > 0);
                    vm.Data = mo.GistCode;
                }
            }

            return vm;
        }

        /// <summary>
        /// Gist 列表
        /// </summary>
        /// <param name="k"></param>
        /// <param name="lang"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> Discover(string k, string lang, int page = 1)
        {
            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.GistQuery(k, lang, 0, userId, page);
            ps.Route = Request.Path;
            ViewData["lang"] = lang;
            return View("/Views/Gist/_PartialGistList.cshtml", ps);
        }

        /// <summary>
        /// Gist 用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="k"></param>
        /// <param name="lang"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ActionName("User")]
        public async Task<IActionResult> Id([FromRoute] string id, string k, string lang, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/gist");
            }

            int uid = Convert.ToInt32(id);

            var mu = await db.UserInfo.FindAsync(uid);
            if (mu == null)
            {
                return Content("Account is empty");
            }
            ViewData["Nickname"] = mu.Nickname;

            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var ps = await CommonService.GistQuery(k, lang, uid, userId, page);
            ps.Route = Request.Path;
            ViewData["lang"] = lang;
            return View("/Views/Gist/_PartialGistList.cshtml", ps);
        }

        /// <summary>
        /// Gist 原始数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Raw([FromRoute] string id, [FromRoute] string sid)
        {
            string result = string.Empty;
            string filename = string.Empty;

            if (!string.IsNullOrWhiteSpace(id))
            {
                var mo = await db.Gist.FirstOrDefaultAsync(x => x.GistCode == id && x.GistStatus == 1 && x.GistOpen == 1);
                if (mo != null)
                {
                    result = mo.GistContent;
                    filename = mo.GistFilename;
                }
            }

            if (sid == "download")
            {
                return File(result.ToByte(), "text/plain", filename);
            }
            else
            {
                return Content(result);
            }
        }
    }
}
