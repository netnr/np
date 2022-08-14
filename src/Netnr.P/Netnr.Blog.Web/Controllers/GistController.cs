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
        /// Gist 一条
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public IActionResult Code([FromRoute] string id, [FromRoute] string sid)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/gist");
            }

            //Auth
            var uinfo = IdentityService.Get(HttpContext);
            switch (sid?.ToLower())
            {
                case "edit":
                    {
                        if (!User.Identity.IsAuthenticated)
                        {
                            return Unauthorized();
                        }

                        var mo = db.Gist.Where(x => x.GistCode == id).FirstOrDefault();
                        //有记录且为当前用户
                        if (mo != null && mo.Uid == uinfo.UserId)
                        {
                            return View("/Views/Gist/_PartialGistEditor.cshtml", mo);
                        }
                        else
                        {
                            return Unauthorized();
                        }
                    }
                case "delete":
                    {
                        if (!User.Identity.IsAuthenticated)
                        {
                            return Unauthorized();
                        }

                        var mo = db.Gist.Where(x => x.GistCode == id && x.Uid == uinfo.UserId).FirstOrDefault();
                        db.Gist.Remove(mo);
                        int num = db.SaveChanges();
                        if (num > 0)
                        {
                            return Redirect("/gist/user/" + uinfo.UserId);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
            }

            //view
            var query = from a in db.Gist
                        join b in db.GistSync on a.GistCode equals b.GistCode into bg
                        from b in bg.DefaultIfEmpty()
                        join c in db.UserInfo on a.Uid equals c.UserId
                        where a.GistCode == id && a.GistStatus == 1 && a.GistOpen == 1
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
            var moout = query.FirstOrDefault();

            return View(moout);
        }

        /// <summary>
        /// Gist 保存
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public ResultVM Save([FromForm] Gist mo)
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
                    mo.Uid = uinfo.UserId;

                    mo.GistCode = UniqueTo.LongId().ToString();
                    db.Gist.Add(mo);
                    db.SaveChanges();

                    vm.Data = mo.GistCode;
                    vm.Set(EnumTo.RTag.success);
                }
                else
                {
                    var oldmo = db.Gist.FirstOrDefault(x => x.GistCode == mo.GistCode);
                    if (oldmo?.Uid == uinfo.UserId)
                    {
                        oldmo.GistRemark = mo.GistRemark;
                        oldmo.GistFilename = mo.GistFilename;
                        oldmo.GistLanguage = mo.GistLanguage;
                        oldmo.GistTheme = mo.GistTheme;
                        oldmo.GistContent = mo.GistContent;
                        oldmo.GistContentPreview = mo.GistContentPreview;
                        oldmo.GistRow = mo.GistRow;
                        oldmo.GistOpen = mo.GistOpen;
                        oldmo.GistUpdateTime = DateTime.Now;

                        db.Gist.Update(oldmo);
                        db.SaveChanges();

                        vm.Data = mo.GistCode;
                        vm.Set(EnumTo.RTag.success);
                    }
                    else
                    {
                        vm.Set(EnumTo.RTag.fail);
                    }
                }

                //推送通知
                PushService.PushAsync("网站消息（Gist）", $"{mo.GistRemark}\r\n{mo.GistFilename}");
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
        public IActionResult Discover(string k, string lang, int page = 1)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var ps = CommonService.GistQuery(k, lang, 0, uinfo.UserId, page);
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
        public IActionResult Id([FromRoute] string id, string k, string lang, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/gist");
            }

            int uid = Convert.ToInt32(id);

            var mu = db.UserInfo.Find(uid);
            if (mu == null)
            {
                return Content("Account is empty");
            }
            ViewData["Nickname"] = mu.Nickname;

            var uinfo = IdentityService.Get(HttpContext);

            var ps = CommonService.GistQuery(k, lang, uid, uinfo.UserId, page);
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
        public IActionResult Raw([FromRoute] string id, [FromRoute] string sid)
        {
            string result = string.Empty;

            string filename = string.Empty;
            if (!string.IsNullOrWhiteSpace(id))
            {
                var mo = db.Gist.FirstOrDefault(x => x.GistCode == id && x.GistStatus == 1 && x.GistOpen == 1);
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
