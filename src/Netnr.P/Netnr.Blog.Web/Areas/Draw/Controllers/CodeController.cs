using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Netnr.Web.Areas.Draw.Controllers
{
    [Area("Draw")]
    public class CodeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="xml"></param>
        /// <param name="mof"></param>
        /// <returns></returns>
        public IActionResult Index(string filename, string xml, Blog.Domain.Draw mof)
        {
            var id = RouteData.Values["id"]?.ToString();
            var sid = RouteData.Values["sid"]?.ToString();

            var uinfo = new Blog.Application.UserAuthService(HttpContext).Get();

            if (!string.IsNullOrWhiteSpace(filename))
            {
                filename = filename.ToDecode();
            }
            if (!string.IsNullOrWhiteSpace(xml))
            {
                xml = xml.ToDecode();
            }


            //新增
            if (id == "open")
            {
                //编辑
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    var vm = new ActionResultVM();
                    using var db = new Blog.Data.ContextBase();
                    var mo = db.Draw.Find(sid);
                    if (mo?.DrOpen == 1 || mo?.Uid == uinfo.UserId)
                    {
                        vm.Set(ARTag.success);
                        vm.Data = mo;
                    }
                    else
                    {
                        vm.Set(ARTag.unauthorized);
                    }
                    return Content(vm.ToJson());
                }
                return Ok();
            }
            //新增、编辑表单
            else if (id == "form")
            {
                object model = null;
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrWhiteSpace(sid))
                    {
                        using var db = new Blog.Data.ContextBase();
                        var mo = db.Draw.Find(sid);
                        if (mo.Uid == uinfo.UserId)
                        {
                            model = mo;
                        }
                    }
                }

                return View("_PartialDrawForm", model);
            }
            //保存标题等信息
            else if (id == "saveform")
            {
                var vm = new ActionResultVM();
                if (User.Identity.IsAuthenticated)
                {
                    using var db = new Blog.Data.ContextBase();
                    int num = 0;
                    if (string.IsNullOrWhiteSpace(mof.DrId))
                    {
                        mof.DrId = mof.DrType[0] + Core.UniqueTo.LongId().ToString();
                        mof.DrCreateTime = DateTime.Now;
                        mof.Uid = uinfo.UserId;
                        mof.DrOrder = 100;

                        db.Draw.Add(mof);
                        num = db.SaveChanges();
                    }
                    else
                    {
                        var newmo = db.Draw.Find(mof.DrId);
                        if (newmo.Uid == uinfo.UserId)
                        {
                            newmo.DrRemark = mof.DrRemark;
                            newmo.DrName = mof.DrName;
                            newmo.DrOpen = mof.DrOpen;

                            db.Draw.Update(newmo);
                            num = db.SaveChanges();
                        }
                    }
                    vm.Set(num > 0);
                }
                else
                {
                    vm.Set(ARTag.unauthorized);
                }

                if (vm.Code == 200)
                {
                    return Redirect("/draw/user/" + uinfo?.UserId);
                }
                else
                {
                    return Content(vm.ToJson());
                }
            }
            //保存内容
            else if (id == "save")
            {
                var vm = new ActionResultVM();

                if (User.Identity.IsAuthenticated)
                {
                    using var db = new Blog.Data.ContextBase();
                    //新增
                    if (string.IsNullOrWhiteSpace(sid))
                    {
                        var mo = new Blog.Domain.Draw
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
                            vm.Set(ARTag.unauthorized);
                        }
                    }
                }
                else
                {
                    vm.Code = 1;
                    vm.Msg = "未登录";
                }

                return Content(vm.ToJson());
            }
            //删除
            else if (id == "del")
            {
                var vm = new ActionResultVM();

                if (User.Identity.IsAuthenticated)
                {
                    using var db = new Blog.Data.ContextBase();
                    var mo = db.Draw.Find(sid);
                    if (mo.Uid == uinfo.UserId)
                    {
                        db.Remove(mo);
                        int num = db.SaveChanges();

                        vm.Set(num > 0);
                    }
                    else
                    {
                        vm.Set(ARTag.unauthorized);
                    }
                }
                else
                {
                    vm.Set(ARTag.unauthorized);
                }

                if (vm.Code == 200)
                {
                    return Redirect("/draw/discover");
                }
                else
                {
                    return Content(vm.ToJson());
                }
            }
            //插入图片
            else if (id == "upload")
            {
                var errno = -1;
                var msg = "fail";
                var url = "";

                var subdir = GlobalTo.GetValue("StaticResource:DrawPath").Replace(GlobalTo.GetValue("StaticResource:RootDir"), "");
                var vm = new Blog.Web.Controllers.APIController().API98(Request.Form.Files[0], subdir);

                if (vm.Code == 200)
                {
                    var jd = vm.Data.ToJson().ToJObject();
                    url = jd["server"].ToString() + jd["path"].ToString();
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

            ViewData["vid"] = id;

            var vname = string.Format("_Partial{0}View", id.StartsWith('m') ? "Mind" : "Draw");
            return View(vname);
        }
    }
}