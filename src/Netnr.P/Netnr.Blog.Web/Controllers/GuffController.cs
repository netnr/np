namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// Guff
    /// </summary>
    public class GuffController : Controller
    {
        public ContextBase db;

        public GuffController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// Guff 首页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            return View("/Views/Guff/_PartialGuffEditor.cshtml");
        }

        /// <summary>
        /// Guff 保存
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, FilterConfigs.IsCompleteInfo, HttpPost]
        public IActionResult Save([FromForm] GuffRecord mo)
        {
            var uinfo = IdentityService.Get(HttpContext);

            if (string.IsNullOrWhiteSpace(mo.GrContent) && string.IsNullOrWhiteSpace(mo.GrImage) && string.IsNullOrWhiteSpace(mo.GrAudio) && string.IsNullOrWhiteSpace(mo.GrVideo))
            {
                return BadRequest("内容不能为空（内容、图片、音频、视频 至少有一项有内容）");
            }
            else if (string.IsNullOrWhiteSpace(mo.GrTag))
            {
                return BadRequest("标签不能为空");
            }
            else if (IdentityService.CompleteInfoValid(HttpContext).Code != 200)
            {
                return BadRequest("请先完善个人信息");
            }
            else
            {
                var now = DateTime.Now;

                //add
                if (string.IsNullOrWhiteSpace(mo.GrId))
                {
                    mo.Uid = uinfo?.UserId;
                    mo.GrId = UniqueTo.LongId().ToString();
                    mo.GrCreateTime = now;
                    mo.GrUpdateTime = now;
                    mo.GrStatus = 1;
                    mo.GrReadNum = 0;
                    mo.GrLaud = 0;
                    mo.GrMark = 0;
                    mo.GrReplyNum = 0;
                    mo.GrOpen ??= 1;

                    mo.GrTypeName = ParsingTo.JsSafeJoin(mo.GrTypeName);
                    mo.GrTypeValue = ParsingTo.JsSafeJoin(mo.GrTypeValue);
                    mo.GrObject = ParsingTo.JsSafeJoin(mo.GrObject);
                    mo.GrImage = ParsingTo.JsSafeJoin(mo.GrImage);
                    mo.GrAudio = ParsingTo.JsSafeJoin(mo.GrAudio);
                    mo.GrVideo = ParsingTo.JsSafeJoin(mo.GrVideo);
                    mo.GrFile = ParsingTo.JsSafeJoin(mo.GrFile);
                    mo.GrTag = ParsingTo.JsSafeJoin(mo.GrTag);

                    db.GuffRecord.Add(mo);

                    int num = db.SaveChanges();
                    if (num > 0)
                    {
                        //推送通知
                        _ = PushService.PushWeChat("网站消息（Guff）", $"新增一条");

                        return Redirect($"/guff/code/{mo.GrId}");
                    }
                    else
                    {
                        return BadRequest("保存失败");
                    }
                }
                else
                {
                    var currMo = db.GuffRecord.Find(mo.GrId);
                    if (currMo == null || currMo.Uid != uinfo?.UserId)
                    {
                        return Unauthorized("Not Authorized");
                    }
                    else
                    {
                        currMo.GrTypeName = ParsingTo.JsSafeJoin(mo.GrTypeName);
                        currMo.GrTypeValue = ParsingTo.JsSafeJoin(mo.GrTypeValue);
                        currMo.GrObject = ParsingTo.JsSafeJoin(mo.GrObject);

                        currMo.GrContent = mo.GrContent;
                        currMo.GrContentMd = mo.GrContentMd;

                        currMo.GrImage = ParsingTo.JsSafeJoin(mo.GrImage);
                        currMo.GrAudio = ParsingTo.JsSafeJoin(mo.GrAudio);
                        currMo.GrVideo = ParsingTo.JsSafeJoin(mo.GrVideo);
                        currMo.GrFile = ParsingTo.JsSafeJoin(mo.GrFile);
                        currMo.GrRemark = mo.GrRemark;

                        currMo.GrTag = mo.GrTag;
                        currMo.GrUpdateTime = DateTime.Now;
                        currMo.GrOpen = mo.GrOpen ?? 1;

                        db.Update(currMo);

                        int num = db.SaveChanges();
                        if (num > 0)
                        {
                            return Redirect($"/guff/code/{mo.GrId}");
                        }
                        else
                        {
                            return BadRequest("保存失败");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Guff 列表
        /// </summary>
        /// <param name="id">子类</param>
        /// <param name="k">搜索</param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> Discover([FromRoute] string id, string k, int page = 1)
        {
            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var vm = await CommonService.GuffQuery(category: id, k, null, null, null, OwnerId: 0, UserId: userId, page);
            vm.Route = Request.Path;
            return View("/Views/Guff/_PartialGuffList.cshtml", vm);
        }

        /// <summary>
        /// Guff 用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="sid">子类</param>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ActionName("User")]
        public async Task<IActionResult> Id([FromRoute] int id, [FromRoute] string sid, string k, int page = 1)
        {
            if (id == 0)
            {
                return Redirect("/guff");
            }

            var mu = await db.UserInfo.FindAsync(id);
            if (mu == null)
            {
                return NotFound();
            }
            ViewData["Nickname"] = mu.Nickname;

            var userId = IdentityService.Get(HttpContext)?.UserId ?? 0;

            var vm = await CommonService.GuffQuery(category: sid, k, null, null, null, OwnerId: 0, UserId: userId, page);
            vm.Route = Request.Path;
            return View("/Views/Guff/_PartialGuffList.cshtml", vm);
        }

        /// <summary>
        /// Guff 一条
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="sid">命令</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Code([FromRoute] string id, [FromRoute] string sid)
        {
            var uinfo = IdentityService.Get(HttpContext);

            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/guff/discover");
            }
            else if (sid == "edit")
            {
                GuffRecord guffRecord = null;
                if (uinfo == null || (guffRecord = await db.GuffRecord.FindAsync(id)).Uid != uinfo.UserId)
                {
                    return Unauthorized("Not Authorized");
                }
                else
                {
                    return View("/Views/Guff/_PartialGuffEditor.cshtml", guffRecord);
                }
            }
            else if (sid == "delete")
            {
                if (uinfo == null)
                {
                    return Unauthorized("Not Authorized");
                }
                else
                {
                    var num = await db.GuffRecord.Where(x => x.GrId == id && x.Uid == uinfo.UserId && x.GrStatus != -1).ExecuteDeleteAsync();
                    if (num > 0)
                    {
                        return Redirect($"/guff/user/{uinfo.UserId}");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            else // view
            {
                var query1 = from a in db.GuffRecord
                             join b in db.UserInfo on a.Uid equals b.UserId
                             where a.GrId == id
                             select new
                             {
                                 a,
                                 b.Nickname
                             };
                var result1 = await query1.FirstOrDefaultAsync();
                var mo = result1.a;
                if (mo != null && (mo.GrOpen == 1 || uinfo?.UserId == mo.Uid))
                {
                    // 阅读 +1
                    mo.GrReadNum += 1;
                    db.Update(mo);
                    await db.SaveChangesAsync();

                    if (uinfo != null)
                    {
                        var ctype = ConnectionType.GuffRecord.ToString();
                        var result2 = await db.UserConnection.FirstOrDefaultAsync(x => x.UconnTargetType == ctype && x.UconnAction == 1 && x.Uid == uinfo.UserId && x.UconnTargetId == mo.GrId);

                        mo.Spare1 = string.IsNullOrEmpty(result2?.UconnTargetId) ? "" : "laud";
                        mo.Spare2 = (uinfo.UserId == mo.Uid) ? "owner" : "";
                    }
                    mo.Spare3 = result1.Nickname;

                    return View(mo);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        /// <summary>
        /// Guff 回复新增
        /// </summary>
        /// <param name="mo">内容，仅限内容字段必填，支持匿名回复</param>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpPost]
        public ResultVM ReplyAdd([FromForm] UserReply mo, [FromForm] string id)
        {
            var vm = new ResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    vm.Set(EnumTo.RTag.invalid);
                }
                else if (string.IsNullOrWhiteSpace(mo.UrContent))
                {
                    vm.Set(EnumTo.RTag.invalid);
                    vm.Msg = "回复内容不能为空";
                }
                else
                {
                    vm = IdentityService.CompleteInfoValid(HttpContext);
                    if (vm.Code == 200)
                    {
                        var uinfo = IdentityService.Get(HttpContext);

                        var guffmo = db.GuffRecord.Find(id);
                        if (guffmo == null)
                        {
                            vm.Set(EnumTo.RTag.invalid);
                        }
                        else
                        {
                            mo.Uid = uinfo?.UserId;
                            mo.UrTargetType = ConnectionType.GuffRecord.ToString();
                            mo.UrTargetId = id;
                            mo.UrCreateTime = DateTime.Now;
                            mo.UrStatus = 1;
                            mo.UrTargetPid = 0;

                            mo.UrAnonymousLink = ParsingTo.JsSafeJoin(mo.UrAnonymousLink);

                            db.UserReply.Add(mo);

                            guffmo.GrReplyNum += 1;
                            db.GuffRecord.Update(guffmo);

                            int num = db.SaveChanges();
                            vm.Set(num > 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                LoggingService.Write(HttpContext, ex);
            }

            return vm;
        }

        /// <summary>
        /// Guff 回复列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultVM> ReplyList(string id, int page = 1)
        {
            var vm = new ResultVM();

            try
            {
                var pag = new PaginationVM
                {
                    PageNumber = Math.Max(page, 1),
                    PageSize = 10
                };

                var list = await CommonService.ReplyOneQuery(ReplyType.GuffRecord, id, pag);
                //匿名用户，生成邮箱MD5加密用于请求头像
                foreach (var item in list)
                {
                    if (item.Uid == 0 && !string.IsNullOrWhiteSpace(item.UrAnonymousMail))
                    {
                        item.Spare3 = CalcTo.MD5(item.UrAnonymousMail);
                    }
                }

                var pvm = new PageVM()
                {
                    Rows = list,
                    Pag = pag
                };
                vm.Data = pvm;

                vm.Set(EnumTo.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
