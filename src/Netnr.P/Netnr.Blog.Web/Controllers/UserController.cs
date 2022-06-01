using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Netnr.Core;
using Netnr.Login;
using Netnr.Blog.Data;
using Netnr.Blog.Domain;
using Netnr.SharedFast;
using AgGrid.InfiniteRowModel;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 个人用户
    /// </summary>
    public class UserController : Controller
    {
        public ContextBase db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cb"></param>
        public UserController(ContextBase cb)
        {
            db = cb;
        }

        #region 消息

        /// <summary>
        /// 消息
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult Message(int page = 1)
        {
            var uinfo = Apps.LoginService.Get(HttpContext);

            var vm = Application.CommonService.MessageQuery(uinfo.UserId, Application.EnumService.MessageType.UserWriting, null, page);
            vm.Route = Request.Path;

            if (page == 1)
            {
                var listum = db.UserMessage.Where(x => x.UmType == Application.EnumService.MessageType.UserWriting.ToString() && x.UmAction == 2 && x.UmStatus == 1).ToList();
                if (listum.Count > 0)
                {
                    listum.ForEach(x => x.UmStatus = 2);
                    db.UserMessage.UpdateRange(listum);
                    db.SaveChanges();
                }
            }

            return View(vm);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult DelMessage([FromRoute] string id)
        {
            var vm = new SharedResultVM();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var um = db.UserMessage.Find(id);
                if (um == null)
                {
                    vm.Set(SharedEnum.RTag.lack);
                }
                else if (um?.Uid != uinfo.UserId)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else
                {
                    db.UserMessage.Remove(um);
                    int num = db.SaveChanges();

                    vm.Set(num > 0);
                }
            }

            if (vm.Code == 200)
            {
                return Redirect("/user/message");
            }
            else
            {
                return Content(vm.ToJson());
            }
        }

        #endregion

        #region 主页

        /// <summary>
        /// 我的主页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Id([FromRoute] int id)
        {
            if (id > 0)
            {
                var usermo = db.UserInfo.Find(id);
                if (usermo != null)
                {
                    return View(usermo);
                }
            }

            return NotFound();
        }

        /// <summary>
        /// 更新 Say
        /// </summary>
        /// <param name="UserSay"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public SharedResultVM UpdateUserSay([FromForm] string UserSay)
        {
            var vm = Apps.LoginService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var currmo = db.UserInfo.Find(uinfo.UserId);
                currmo.UserSay = UserSay;
                db.UserInfo.Update(currmo);

                int num = db.SaveChanges();

                vm.Set(num > 0);
            }

            return vm;
        }

        /// <summary>
        /// 更新 avatar
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public SharedResultVM UpdateUserAvatar([FromForm] string type, [FromForm] string source)
        {
            var vm = new SharedResultVM();

            try
            {
                vm = Apps.LoginService.CompleteInfoValid(HttpContext);
                if (vm.Code == 200)
                {
                    var uinfo = Apps.LoginService.Get(HttpContext);

                    //物理根路径
                    var prp = GlobalTo.GetValue("StaticResource:PhysicalRootPath");
                    var ppath = PathTo.Combine(GlobalTo.WebRootPath, prp, GlobalTo.GetValue("StaticResource:AvatarPath"));

                    if (!Directory.Exists(ppath))
                    {
                        Directory.CreateDirectory(ppath);
                    }

                    if (string.IsNullOrWhiteSpace(uinfo.UserPhoto))
                    {
                        uinfo.UserPhoto = UniqueTo.LongId() + ".jpg";
                    }
                    var upname = uinfo.UserPhoto.Split('?')[0];
                    var npnew = upname + "?" + DateTime.Now.ToTimestamp();

                    switch (type)
                    {
                        case "file":
                            {
                                source = source[(source.LastIndexOf(",") + 1)..];
                                byte[] bytes = Convert.FromBase64String(source);
                                System.IO.File.WriteAllBytes(PathTo.Combine(ppath, upname), bytes);

                                var usermo = db.UserInfo.Find(uinfo.UserId);
                                usermo.UserPhoto = npnew;
                                db.UserInfo.Update(usermo);
                                int num = db.SaveChanges();
                                if (num > 0)
                                {
                                    using var ac = new AccountController(db);
                                    ac.SetAuth(HttpContext, usermo);
                                }

                                vm.Set(SharedEnum.RTag.success);
                            }
                            break;
                        case "link":
                            {
                                HttpTo.DownloadSave(HttpTo.HWRequest(source), PathTo.Combine(ppath, upname));

                                var usermo = db.UserInfo.Find(uinfo.UserId);
                                usermo.UserPhoto = npnew;
                                db.UserInfo.Update(usermo);
                                int num = db.SaveChanges();
                                if (num > 0)
                                {
                                    using var ac = new AccountController(db);
                                    ac.SetAuth(HttpContext, usermo);
                                }

                                vm.Set(SharedEnum.RTag.success);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        #endregion

        #region 个人设置

        /// <summary>
        /// 个人设置页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Setting()
        {
            var uinfo = Apps.LoginService.Get(HttpContext);

            var mo = db.UserInfo.Find(uinfo.UserId);

            return View(mo);
        }

        /// <summary>
        /// 保存个人信息
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public SharedResultVM SaveUserInfo([FromForm] UserInfo mo)
        {
            var vm = new SharedResultVM();

            var errMsg = new List<string>();
            if (string.IsNullOrWhiteSpace(mo.UserName))
            {
                errMsg.Add("账号不能为空");
            }
            if (string.IsNullOrWhiteSpace(mo.Nickname))
            {
                errMsg.Add("昵称不能为空");
            }
            if (string.IsNullOrWhiteSpace(mo.UserMail))
            {
                errMsg.Add("邮箱不能为空");
            }
            if (string.IsNullOrWhiteSpace(mo.UserPhone))
            {
                errMsg.Add("手机不能为空");
            }

            if (errMsg.Count > 0)
            {
                vm.Set(SharedEnum.RTag.refuse);
                vm.Msg = string.Join("<br/>", errMsg);

                return vm;
            }

            var uinfo = Apps.LoginService.Get(HttpContext);

            var usermo = db.UserInfo.Find(uinfo.UserId);
            var log = new List<object>() { new UserInfo().ToRead(usermo) };

            //变更账号
            if (!string.IsNullOrWhiteSpace(mo.UserName) && usermo.UserNameChange != 1 && usermo.UserName != mo.UserName)
            {
                //账号重复
                if (db.UserInfo.Any(x => x.UserName == mo.UserName))
                {
                    vm.Set(SharedEnum.RTag.exist);
                    vm.Msg = "账号已经存在";

                    return vm;
                }
                else
                {
                    usermo.UserName = mo.UserName;
                    usermo.UserNameChange = 1;
                }
            }

            //变更邮箱
            if (mo.UserMail != usermo.UserMail)
            {
                usermo.UserMailValid = 0;

                //邮箱正则验证
                if (!string.IsNullOrWhiteSpace(mo.UserMail))
                {
                    if (!ParsingTo.IsMail(mo.UserMail))
                    {
                        vm.Set(SharedEnum.RTag.invalid);
                        vm.Msg = "邮箱格式有误";

                        return vm;
                    }
                    else if (db.UserInfo.Any(x => x.UserMail == mo.UserMail))
                    {
                        vm.Set(SharedEnum.RTag.exist);
                        vm.Msg = "邮箱已经存在";

                        return vm;
                    }
                }
            }

            usermo.UserMail = mo.UserMail;
            usermo.Nickname = mo.Nickname;
            usermo.UserPhone = mo.UserPhone;
            usermo.UserUrl = mo.UserUrl;

            log.Add(usermo);

            //留痕
            var logModel = Apps.FilterConfigs.LogBuild(HttpContext);
            logModel.LogLevel = "T";
            logModel.LogGroup = "9";
            logModel.LogContent = log.ToJson();
            logModel.LogRemark = "修改个人信息";
            SharedLogging.LoggingTo.Add(logModel);

            db.UserInfo.Update(usermo);
            var num = db.SaveChanges();

            //更新授权信息
            using (var ac = new AccountController(db))
            {
                ac.SetAuth(HttpContext, usermo, true);
            }

            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 绑定账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult OAuth([FromRoute] string id)
        {
            var url = Application.ThirdLoginService.LoginLink(id, "bind");
            return Redirect(url);
        }

        /// <summary>
        /// 解绑账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult RidOAuth([FromRoute] LoginBase.LoginType? id)
        {
            var uinfo = Apps.LoginService.Get(HttpContext);
            var mo = db.UserInfo.Find(uinfo.UserId);

            switch (id)
            {
                case LoginBase.LoginType.QQ:
                    mo.OpenId1 = "";
                    break;
                case LoginBase.LoginType.WeiBo:
                    mo.OpenId2 = "";
                    break;
                case LoginBase.LoginType.GitHub:
                    mo.OpenId3 = "";
                    break;
                case LoginBase.LoginType.TaoBao:
                    mo.OpenId4 = "";
                    break;
                case LoginBase.LoginType.MicroSoft:
                    mo.OpenId5 = "";
                    break;
                case LoginBase.LoginType.DingTalk:
                    mo.OpenId6 = "";
                    break;
            }

            db.UserInfo.Update(mo);
            db.SaveChanges();

            return Redirect("/user/setting");
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [Authorize]
        public SharedResultVM UpdatePassword(string oldPassword, string newPassword)
        {
            var vm = new SharedResultVM();

            var uinfo = Apps.LoginService.Get(HttpContext);

            var userinfo = db.UserInfo.Find(uinfo.UserId);
            if (userinfo.UserPwd == CalcTo.MD5(oldPassword))
            {
                userinfo.UserPwd = CalcTo.MD5(newPassword);
                db.UserInfo.Update(userinfo);
                var num = db.SaveChanges();

                vm.Set(num > 0);
            }
            else
            {
                vm.Set(SharedEnum.RTag.unauthorized);
                vm.Msg = "原密码错误";
            }

            return vm;
        }

        #endregion

        #region 文章管理

        /// <summary>
        /// 文章管理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult Write([FromRoute] string id, int? page)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                int action = 1;
                if (id == "mark")
                {
                    action = 2;
                }

                var uinfo = Apps.LoginService.Get(HttpContext);
                var vm = Application.CommonService.UserConnWritingQuery(uinfo.UserId, Application.EnumService.ConnectionType.UserWriting, action, page ?? 1);
                vm.Route = Request.Path;
                vm.Other = id;

                return View("_PartialWritingList", vm);
            }

            return View();
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        [Authorize, HttpGet]
        public SharedResultVM WriteList(string grp)
        {
            return SharedResultVM.Try(vm =>
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var query = from a in db.UserWriting
                            where a.Uid == uinfo.UserId
                            orderby a.UwCreateTime descending
                            select new
                            {
                                a.UwId,
                                a.UwTitle,
                                a.UwCreateTime,
                                a.UwUpdateTime,
                                a.UwReadNum,
                                a.UwReplyNum,
                                a.UwOpen,
                                a.UwStatus,
                                a.UwLaud,
                                a.UwMark,
                                a.UwCategory
                            };

                vm.Data = query.GetInfiniteRowModelBlock(grp);
                vm.Set(SharedEnum.RTag.success);

                return vm;
            });
        }

        /// <summary>
        /// 获取一篇文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public SharedResultVM WriteOne(int id)
        {
            var vm = new SharedResultVM();

            var uinfo = Apps.LoginService.Get(HttpContext);

            var mo = db.UserWriting.FirstOrDefault(x => x.Uid == uinfo.UserId && x.UwId == id);
            var listTags = db.UserWritingTags.Where(x => x.UwId == id).ToList();

            vm.Data = new
            {
                item = mo,
                tags = listTags
            };
            vm.Set(SharedEnum.RTag.success);

            return vm;
        }

        /// <summary>
        /// 保存一篇文章（编辑）
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="TagIds"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public SharedResultVM WriteSave([FromForm] UserWriting mo, [FromForm] string TagIds)
        {
            return SharedResultVM.Try(vm =>
            {
                var lisTagId = new List<int>();
                TagIds.Split(',').ToList().ForEach(x => lisTagId.Add(Convert.ToInt32(x)));
                var lisTagName = Application.CommonService.TagsQuery().Where(x => lisTagId.Contains(x.TagId)).ToList();

                var uinfo = Apps.LoginService.Get(HttpContext);

                var oldmo = db.UserWriting.FirstOrDefault(x => x.Uid == uinfo.UserId && x.UwId == mo.UwId);
                if (oldmo?.UwStatus == -1)
                {
                    vm.Set(SharedEnum.RTag.unauthorized);
                }
                else if (oldmo != null)
                {
                    oldmo.UwTitle = mo.UwTitle;
                    oldmo.UwCategory = mo.UwCategory;
                    oldmo.UwContentMd = mo.UwContentMd;
                    oldmo.UwContent = mo.UwContent;
                    oldmo.UwUpdateTime = DateTime.Now;

                    db.UserWriting.Update(oldmo);

                    var wt = db.UserWritingTags.Where(x => x.UwId == mo.UwId).ToList();
                    db.UserWritingTags.RemoveRange(wt);

                    var listwt = new List<UserWritingTags>();
                    foreach (var tag in lisTagId)
                    {
                        var wtmo = new UserWritingTags
                        {
                            UwId = mo.UwId,
                            TagId = tag,
                            TagName = lisTagName.Where(x => x.TagId == tag).FirstOrDefault().TagName
                        };

                        listwt.Add(wtmo);
                    }
                    db.UserWritingTags.AddRange(listwt);

                    int num = db.SaveChanges();
                    vm.Set(num > 0);
                }

                return vm;
            });
        }

        /// <summary>
        /// 删除 一篇文章
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Authorize, HttpGet]
        public SharedResultVM WriteDel(string ids)
        {
            return SharedResultVM.Try(vm =>
            {
                var uinfo = Apps.LoginService.Get(HttpContext);
                var listKeyId = ids.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                var listKeys = listKeyId.Select(x => x.ToString()).ToList();

                var listMo = db.UserWriting.Where(x => x.Uid == uinfo.UserId && x.UwStatus != -1 && listKeyId.Contains(x.UwId)).ToList();
                db.UserWriting.RemoveRange(listMo);

                var mo2 = db.UserWritingTags.Where(x => listKeyId.Contains(x.UwId)).ToList();
                db.UserWritingTags.RemoveRange(mo2);

                var mo3 = db.UserReply.Where(x => listKeys.Contains(x.UrTargetId)).ToList();
                db.UserReply.RemoveRange(mo3);

                var num = db.SaveChanges();
                vm.Set(num > 0);

                return vm;
            });
        }

        #endregion

        #region 验证

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Verify([FromRoute] string id)
        {
            var vm = new SharedResultVM();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var uinfo = Apps.LoginService.Get(HttpContext);

                var vcurl = GlobalTo.GetValue("VerifyCode:Url");
                var vckey = GlobalTo.GetValue("VerifyCode:Key");

                switch (id.ToLower())
                {
                    //发送验证邮箱
                    case "send":
                        {
                            if (User.Identity.IsAuthenticated)
                            {
                                var usermo = db.UserInfo.Find(uinfo.UserId);
                                if (usermo.UserMailValid == 1)
                                {
                                    vm.Msg = "邮箱已经完成验证";
                                }
                                else if (string.IsNullOrWhiteSpace(usermo.UserMail))
                                {
                                    vm.Msg = "邮箱不能为空";
                                }
                                else
                                {
                                    var cacheKey = "Global_VerifyMail_" + usermo.UserId;
                                    var issend = CacheTo.Get(cacheKey) as bool?;
                                    if (issend == true)
                                    {
                                        vm.Msg = "5分钟内只能发送一次验证信息";
                                    }
                                    else
                                    {
                                        var mcs = FileTo.ReadText(GlobalTo.WebRootPath + "/file/mailchecker/list.txt").Split(Environment.NewLine);
                                        if (mcs.Contains(usermo.UserMail.Split('@').LastOrDefault().ToLower()))
                                        {
                                            vm.Msg = "该邮箱已被屏蔽";
                                        }
                                        else
                                        {
                                            //发送验证

                                            var ToMail = usermo.UserMail;

                                            var vjson = new
                                            {
                                                mail = ToMail,
                                                ts = DateTime.Now.ToTimestamp()
                                            }.ToJson();

                                            var vcode = CalcTo.AESEncrypt(vjson, vckey).ToBase64Encode().ToUrlEncode();
                                            var VerifyLink = string.Format(vcurl, vcode);

                                            var txt = FileTo.ReadText(GlobalTo.WebRootPath + "/file/template/sendmailverify.html");
                                            txt = txt.Replace("@ToMail@", ToMail).Replace("@VerifyLink@", VerifyLink);

                                            vm = Application.MailService.Send(ToMail, $"[{GlobalTo.GetValue("Common:EnglishName")}] 验证你的邮箱", txt);

                                            if (vm.Code == 200)
                                            {
                                                vm.Msg = "已发送成功";
                                                CacheTo.Set(cacheKey, true, 300, false);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                vm.Msg = "请登录";
                            }
                        }
                        break;

                    //验证邮箱
                    default:
                        try
                        {
                            var vjson = CalcTo.AESDecrypt(id.ToUrlDecode().ToBase64Decode(), vckey).ToJObject();
                            if (DateTime.Now.ToTimestamp() - Convert.ToInt32(vjson["ts"]) < 60 * 5)
                            {
                                var mail = vjson["mail"].ToString();
                                if (string.IsNullOrWhiteSpace(mail))
                                {
                                    vm.Msg = "邮件地址有误";
                                }
                                else
                                {
                                    var usermo = db.UserInfo.FirstOrDefault(x => x.UserMail == mail);
                                    if (usermo != null)
                                    {
                                        if (usermo.UserMailValid == 1)
                                        {
                                            vm.Msg = "已验证，勿重复验证";
                                        }
                                        else
                                        {
                                            usermo.UserMailValid = 1;

                                            db.UserInfo.Update(usermo);

                                            int num = db.SaveChanges();

                                            vm.Set(num > 0);
                                            if (vm.Code == 200)
                                            {
                                                vm.Msg = "恭喜你，验证成功";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        vm.Msg = "邮件地址无效";
                                    }
                                }
                            }
                            else
                            {
                                vm.Msg = "链接已过期（5分钟内有效）";
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            vm.Msg = "链接已失效";
                        }
                        break;
                }
            }
            else
            {
                vm.Msg = "缺失验证码信息";
            }

            return View(vm);
        }

        #endregion
    }
}