using Netnr.Blog.Data;

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
        /// Draw 图形
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View("/Views/Draw/_PartialGraphEditor.cshtml");
        }

        /// <summary>
        /// Draw 脑图
        /// </summary>
        /// <returns></returns>
        public IActionResult BPMN()
        {
            return View("/Views/Draw/_PartialBPMNEditor.cshtml");
        }

        /// <summary>
        /// Draw 脑图
        /// </summary>
        /// <returns></returns>
        public IActionResult Mind()
        {
            return View("/Views/Draw/_PartialMindEditor.cshtml");
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sid"></param>
        /// <param name="code">分享码</param>
        /// <param name="filename"></param>
        /// <param name="xml"></param>
        /// <param name="mof"></param>
        /// <returns></returns>
        public IActionResult Code([FromRoute] string id, [FromRoute] string sid, string code, string filename, string xml, Domain.Draw mof)
        {
            id ??= "";
            sid ??= "";

            var kid = string.Empty;
            if (id.Length == 20)
            {
                kid = id;
            }
            else if (sid.Length == 20)
            {
                kid = sid;
            }
            if (!string.IsNullOrEmpty(kid))
            {
                var sck = "SharedCode_" + kid;
                //有分享码
                if (!string.IsNullOrWhiteSpace(code))
                {
                    Response.Cookies.Append(sck, code);
                }
                else
                {
                    code = Request.Cookies[sck]?.ToString();
                }
            }

            var uinfo = Apps.LoginService.Get(HttpContext);

            if (!string.IsNullOrWhiteSpace(filename))
            {
                filename = filename.ToUrlDecode();
            }
            if (!string.IsNullOrWhiteSpace(xml))
            {
                xml = xml.ToUrlDecode();
            }

            //新增、编辑内容
            if (id == "open")
            {
                //编辑
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    var vm = new ResultVM();
                    var mo = db.Draw.Find(sid);

                    //分享码
                    var isShare = !string.IsNullOrWhiteSpace(mo?.Spare1) && mo?.Spare1 == code;
                    if (mo?.DrOpen == 1 || mo?.Uid == uinfo.UserId || isShare)
                    {
                        vm.Set(EnumTo.RTag.success);
                        vm.Data = mo;
                    }
                    else
                    {
                        vm.Set(EnumTo.RTag.unauthorized);
                    }
                    return Content(vm.ToJson());
                }
                return Ok();
            }
            //新增、编辑表单
            else if (id == "form")
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return Redirect("/account/login?ReturnUrl=" + Request.Path.ToString().ToUrlEncode());
                }

                if (!string.IsNullOrWhiteSpace(sid))
                {
                    var mo = db.Draw.Find(sid);
                    if (mo.Uid == uinfo.UserId)
                    {
                        return View("/Views/Draw/_PartialDrawForm.cshtml", mo);
                    }
                }

                return View("/Views/Draw/_PartialDrawForm.cshtml");
            }
            //保存标题等信息
            else if (id == "saveform")
            {
                var vm = Apps.LoginService.CompleteInfoValid(HttpContext);
                if (vm.Code == 200)
                {
                    int num = 0;
                    if (string.IsNullOrWhiteSpace(mof.DrId))
                    {
                        mof.DrId = mof.DrType[0] + Core.UniqueTo.LongId().ToString();
                        mof.DrCreateTime = DateTime.Now;
                        mof.Uid = uinfo.UserId;
                        mof.DrOrder = 100;
                        mof.DrStatus = 1;

                        db.Draw.Add(mof);
                        num = db.SaveChanges();
                    }
                    else
                    {
                        var newmo = db.Draw.Find(mof.DrId);
                        if (newmo.Uid != uinfo.UserId)
                        {
                            vm.Set(EnumTo.RTag.unauthorized);
                        }
                        else
                        {
                            newmo.DrRemark = mof.DrRemark;
                            newmo.DrName = mof.DrName;
                            newmo.DrOpen = mof.DrOpen;
                            newmo.Spare1 = mof.Spare1;

                            db.Draw.Update(newmo);
                            num = db.SaveChanges();
                        }
                    }

                    //推送通知
                    Application.PushService.PushAsync("网站消息（Draw）", $"{mof.DrName}");

                    vm.Set(num > 0);
                }

                if (vm.Code == 200)
                {
                    return Redirect("/draw/user/" + uinfo?.UserId);
                }
                else
                {
                    return Content(vm.Msg);
                }
            }
            //保存内容
            else if (id == "save")
            {
                var vm = Apps.LoginService.CompleteInfoValid(HttpContext);
                if (vm.Code == 200)
                {
                    //新增
                    if (string.IsNullOrWhiteSpace(sid))
                    {
                        var mo = new Domain.Draw
                        {
                            DrName = filename,
                            DrContent = xml,

                            DrId = mof.DrType[0] + Core.UniqueTo.LongId().ToString(),
                            DrType = mof.DrType,
                            DrCreateTime = DateTime.Now,
                            DrOpen = 1,
                            DrOrder = 100,
                            DrStatus = 1,
                            Uid = uinfo.UserId
                        };

                        db.Draw.Add(mo);

                        var num = db.SaveChanges();
                        vm.Set(num > 0);
                        vm.Data = mo.DrId;
                    }
                    else
                    {
                        var mo = db.Draw.Find(sid);
                        if (mo?.Uid == uinfo.UserId)
                        {
                            mo.DrName = filename;
                            mo.DrContent = xml;

                            db.Draw.Update(mo);

                            var num = db.SaveChanges();
                            vm.Set(num > 0);
                        }
                        else
                        {
                            vm.Set(EnumTo.RTag.unauthorized);
                        }
                    }
                }

                return Content(vm.ToJson());
            }
            //删除
            else if (id == "delete")
            {
                var vm = new ResultVM();

                if (User.Identity.IsAuthenticated)
                {
                    var mo = db.Draw.Find(sid);
                    if (mo.Uid == uinfo.UserId)
                    {
                        db.Remove(mo);
                        int num = db.SaveChanges();

                        vm.Set(num > 0);
                    }
                    else
                    {
                        vm.Set(EnumTo.RTag.unauthorized);
                    }
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                }

                if (vm.Code == 200)
                {
                    return Redirect("/draw/user/" + uinfo.UserId);
                }
                else
                {
                    return Content(vm.ToJson());
                }
            }
            //插入图片
            else if (id == "upload")
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                var errno = -1;
                var msg = "fail";
                var url = "";

                var subdir = GlobalTo.GetValue("StaticResource:DrawPath");
                var vm = api.APIController.UploadCheck(Request.Form.Files[0], null, "", subdir);
                if (vm.Code == 200)
                {
                    var jd = vm.Data.ToJson().ToJObject();
                    url = jd["server"].ToString() + jd["path"].ToString();
                    errno = 0;
                    msg = "ok";
                }

                //推送通知
                Application.PushService.PushAsync("网站消息（Upload）", $"{url}");

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

            ViewData["vid"] = id;

            var idAsType = "Graph";
            switch (id[0])
            {
                case 'm': idAsType = "Mind"; break;
                case 'b': idAsType = "BPMN"; break;
            }

            return View($"/Views/Draw/_Partial{idAsType}Editor.cshtml");
        }

        /// <summary>
        /// Draw 列表
        /// </summary>
        /// <param name="k"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 10)]
        public IActionResult Discover(string k, int page = 1)
        {
            var uinfo = Apps.LoginService.Get(HttpContext);

            var ps = Application.CommonService.DrawQuery(k, 0, uinfo.UserId, page);
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
        public IActionResult Id([FromRoute] string id, string k, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/draw");
            }

            int uid = Convert.ToInt32(id);

            var mu = db.UserInfo.Find(uid);
            if (mu == null)
            {
                return Content("Account is empty");
            }
            ViewData["Nickname"] = mu.Nickname;

            var uinfo = Apps.LoginService.Get(HttpContext);

            var ps = Application.CommonService.DrawQuery(k, uid, uinfo.UserId, page);
            ps.Route = Request.Path;
            return View("/Views/Draw/_PartialDrawList.cshtml", ps);
        }
    }
}