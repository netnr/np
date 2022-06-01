using Microsoft.AspNetCore.Authorization;
using Netnr.Blog.Data;
using Netnr.Core;

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
        [Authorize, Apps.FilterConfigs.IsCompleteInfo, HttpPost]
        public IActionResult Save([FromForm] Domain.GuffRecord mo)
        {
            var uinfo = Apps.LoginService.Get(HttpContext);

            if (string.IsNullOrWhiteSpace(mo.GrContent) && string.IsNullOrWhiteSpace(mo.GrImage) && string.IsNullOrWhiteSpace(mo.GrAudio) && string.IsNullOrWhiteSpace(mo.GrVideo))
            {
                return BadRequest("内容不能为空（内容、图片、音频、视频 至少有一项有内容）");
            }
            else if (string.IsNullOrWhiteSpace(mo.GrTag))
            {
                return BadRequest("标签不能为空");
            }
            else
            {
                var now = DateTime.Now;

                //add
                if (string.IsNullOrWhiteSpace(mo.GrId))
                {
                    mo.Uid = uinfo.UserId;
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
                        Application.PushService.PushAsync("网站消息（Guff）", $"a new record");

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
                    if (currMo == null || currMo.Uid != uinfo.UserId)
                    {
                        return Unauthorized();
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
        public IActionResult Discover([FromRoute] string id, string k, int page = 1)
        {
            var uinfo = Apps.LoginService.Get(HttpContext);

            var vm = Application.CommonService.GuffQuery(category: id, k, null, null, null, OwnerId: 0, UserId: uinfo.UserId, page);
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
        public IActionResult Id([FromRoute] int id, [FromRoute] string sid, string k, int page = 1)
        {
            if (id == 0)
            {
                return Redirect("/guff");
            }

            var mu = db.UserInfo.Find(id);
            if (mu == null)
            {
                return NotFound();
            }
            ViewData["Nickname"] = mu.Nickname;

            var uinfo = Apps.LoginService.Get(HttpContext);

            var vm = Application.CommonService.GuffQuery(category: sid, k, null, null, null, OwnerId: 0, UserId: uinfo.UserId, page);
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
        public IActionResult Code([FromRoute] string id, [FromRoute] string sid)
        {
            var uinfo = Apps.LoginService.Get(HttpContext);

            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/guff/discover");
            }
            else if (sid == "edit")
            {
                var mo = db.GuffRecord.Find(id);

                if (mo.Uid != uinfo.UserId)
                {
                    return Unauthorized();
                }
                else
                {
                    return View("/Views/Guff/_PartialGuffEditor.cshtml", mo);
                }
            }
            else if (sid == "delete")
            {
                if (uinfo.UserId != 0)
                {
                    var mo = db.GuffRecord.Find(id);

                    if (mo.Uid != uinfo.UserId)
                    {
                        return Unauthorized();
                    }
                    else if (mo.GrStatus == -1)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        db.Remove(mo);
                        int num = db.SaveChanges();
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
                else
                {
                    return Unauthorized();
                }
            }
            else // view
            {
                var ctype = Application.EnumService.ConnectionType.GuffRecord.ToString();

                var query = from a in db.GuffRecord
                            join b in db.UserInfo on a.Uid equals b.UserId
                            join c in db.UserConnection.Where(x => x.UconnTargetType == ctype && x.UconnAction == 1 && x.Uid == uinfo.UserId) on a.GrId equals c.UconnTargetId into cg
                            from c1 in cg.DefaultIfEmpty()
                            where a.GrId == id
                            select new
                            {
                                a,
                                UconnTargetId = c1 == null ? null : c1.UconnTargetId,
                                b.Nickname
                            };
                var qm = query.FirstOrDefault();
                var mo = qm?.a;
                if (mo == null)
                {
                    return NotFound();
                }
                else if (mo.GrOpen == 1 || uinfo.UserId == mo.Uid)
                {
                    // 阅读 +1
                    mo.GrReadNum += 1;
                    db.Update(mo);
                    db.SaveChanges();

                    mo.Spare1 = string.IsNullOrEmpty(qm.UconnTargetId) ? "" : "laud";
                    mo.Spare2 = (uinfo.UserId == mo.Uid) ? "owner" : "";
                    mo.Spare3 = qm.Nickname;

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
        public SharedResultVM ReplyAdd([FromForm] Domain.UserReply mo, [FromForm] string id)
        {
            var vm = new SharedResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    vm.Set(SharedEnum.RTag.invalid);
                }
                else if (string.IsNullOrWhiteSpace(mo.UrContent))
                {
                    vm.Set(SharedEnum.RTag.invalid);
                    vm.Msg = "回复内容不能为空";
                }
                else
                {
                    vm = Apps.LoginService.CompleteInfoValid(HttpContext);
                    if (vm.Code == 200)
                    {
                        var uinfo = Apps.LoginService.Get(HttpContext);

                        var guffmo = db.GuffRecord.Find(id);
                        if (guffmo == null)
                        {
                            vm.Set(SharedEnum.RTag.invalid);
                        }
                        else
                        {
                            mo.Uid = uinfo.UserId;
                            mo.UrTargetType = Application.EnumService.ConnectionType.GuffRecord.ToString();
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
                Apps.FilterConfigs.LogWrite(HttpContext, ex);
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
        public SharedResultVM ReplyList(string id, int page = 1)
        {
            return SharedResultVM.Try(vm =>
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var pag = new SharedPaginationVM
                {
                    PageNumber = Math.Max(page, 1),
                    PageSize = 10
                };

                var list = Application.CommonService.ReplyOneQuery(Application.EnumService.ReplyType.GuffRecord, id, pag);
                //匿名用户，生成邮箱MD5加密用于请求头像
                foreach (var item in list)
                {
                    if (item.Uid == 0 && !string.IsNullOrWhiteSpace(item.UrAnonymousMail))
                    {
                        item.Spare3 = CalcTo.MD5(item.UrAnonymousMail);
                    }
                }

                var pvm = new SharedPageVM()
                {
                    Rows = list,
                    Pag = pag
                };
                vm.Data = pvm;

                vm.Set(SharedEnum.RTag.success);

                return vm;
            });
        }
    }
}
