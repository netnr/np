using Netnr.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 个人用户
    /// </summary>
    public class UserController : WebController
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
        public async Task<IActionResult> Message(int page = 1)
        {
            var uinfo = IdentityService.Get(HttpContext);

            var vm = await CommonService.MessageQuery(uinfo.UserId, MessageType.UserWriting, null, page);
            vm.Route = Request.Path;

            if (page == 1)
            {
                var mType = MessageType.UserWriting.ToString();
                await db.UserMessage.Where(x => x.UmType == mType && x.UmAction == 2 && x.UmStatus == 1)
                    .ExecuteUpdateAsync(x => x.SetProperty(p => p.UmStatus, 2));
            }

            return View(vm);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DelMessage([FromRoute] string id)
        {
            var vm = new ResultVM();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var uinfo = IdentityService.Get(HttpContext);

                var num = await db.UserMessage.Where(x => x.UmId == id && x.Uid == uinfo.UserId).ExecuteDeleteAsync();
                vm.Set(num > 0);
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
        public async Task<IActionResult> Id([FromRoute] int id)
        {
            if (id > 0)
            {
                var userInfo = await db.UserInfo.FindAsync(id);
                if (userInfo != null)
                {
                    return View(userInfo);
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
        public async Task<ResultVM> UpdateUserSay([FromForm] string UserSay)
        {
            var vm = IdentityService.CompleteInfoValid(HttpContext);
            if (vm.Code == 200)
            {
                var uinfo = IdentityService.Get(HttpContext);

                var num = await db.UserInfo.Where(x => x.UserId == uinfo.UserId).ExecuteUpdateAsync(x => x.SetProperty(p => p.UserSay, UserSay));
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
        public async Task<ResultVM> UpdateUserAvatar([FromForm] string type, [FromForm] string source)
        {
            var vm = new ResultVM();

            try
            {
                vm = IdentityService.CompleteInfoValid(HttpContext);
                if (vm.Code == 200)
                {
                    var uinfo = IdentityService.Get(HttpContext);

                    //物理根路径
                    var ppath = CommonService.StaticResourcePath("AvatarPath");
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

                                var userInfo = await db.UserInfo.FindAsync(uinfo.UserId);
                                userInfo.UserPhoto = npnew;
                                db.UserInfo.Update(userInfo);
                                int num = await db.SaveChangesAsync();
                                if (num > 0)
                                {
                                    //写入授权
                                    await IdentityService.Set(HttpContext, userInfo);
                                }

                                vm.Set(EnumTo.RTag.success);
                            }
                            break;
                        case "link":
                            {
                                var maxLength = 1024 * 1024 * 5;
                                var client = new HttpClient
                                {
                                    Timeout = TimeSpan.FromMinutes(1)
                                };
                                await client.DownloadAsync(source, PathTo.Combine(ppath, upname), (rlen, total) =>
                                {
                                    if (total > maxLength || rlen > maxLength)
                                    {
                                        throw new Exception($"{source} Size exceeds limit(max {ParsingTo.FormatByteSize(maxLength)})");
                                    }
                                });

                                var userInfo = await db.UserInfo.FindAsync(uinfo.UserId);
                                userInfo.UserPhoto = npnew;
                                db.UserInfo.Update(userInfo);
                                int num = await db.SaveChangesAsync();
                                if (num > 0)
                                {
                                    //写入授权
                                    await IdentityService.Set(HttpContext, userInfo);
                                }

                                vm.Set(EnumTo.RTag.success);
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
            var uinfo = IdentityService.Get(HttpContext);

            var mo = db.UserInfo.Find(uinfo.UserId);

            return View(mo);
        }

        /// <summary>
        /// 保存个人信息
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<ResultVM> SaveUserInfo([FromForm] UserInfo mo)
        {
            var vm = new ResultVM();

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
                vm.Set(EnumTo.RTag.refuse);
                vm.Msg = string.Join("<br/>", errMsg);

                return vm;
            }

            var uinfo = IdentityService.Get(HttpContext);

            var userInfo = await db.UserInfo.FindAsync(uinfo.UserId);
            var log = new List<object>() { new UserInfo().ToDeepCopy(userInfo) };

            //变更账号
            if (!string.IsNullOrWhiteSpace(mo.UserName) && userInfo.UserNameChange != 1 && userInfo.UserName != mo.UserName)
            {
                //账号重复
                if (await db.UserInfo.AnyAsync(x => x.UserName == mo.UserName))
                {
                    vm.Set(EnumTo.RTag.exist);
                    vm.Msg = "账号已经存在";

                    return vm;
                }
                else
                {
                    userInfo.UserName = mo.UserName;
                    userInfo.UserNameChange = 1;
                }
            }

            //变更邮箱
            if (mo.UserMail != userInfo.UserMail)
            {
                userInfo.UserMailValid = 0;

                //邮箱正则验证
                if (!string.IsNullOrWhiteSpace(mo.UserMail))
                {
                    if (!ParsingTo.IsMail(mo.UserMail))
                    {
                        vm.Set(EnumTo.RTag.invalid);
                        vm.Msg = "邮箱格式有误";

                        return vm;
                    }
                    else if (await db.UserInfo.AnyAsync(x => x.UserMail == mo.UserMail))
                    {
                        vm.Set(EnumTo.RTag.exist);
                        vm.Msg = "邮箱已经存在";

                        return vm;
                    }
                }
            }

            userInfo.UserMail = mo.UserMail;
            userInfo.Nickname = mo.Nickname;
            userInfo.UserPhone = mo.UserPhone;
            userInfo.UserUrl = mo.UserUrl;

            log.Add(userInfo);

            //留痕
            var logModel = LoggingService.Build(HttpContext);
            logModel.LogLevel = "T";
            logModel.LogGroup = "9";
            logModel.LogContent = log.ToJson();
            logModel.LogRemark = "修改个人信息";
            LoggingTo.Add(logModel);

            db.UserInfo.Update(userInfo);
            var num = await db.SaveChangesAsync();

            //更新授权信息
            await IdentityService.Set(HttpContext, userInfo);

            vm.Set(num > 0);

            return vm;
        }

        /// <summary>
        /// 绑定账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult OAuth([FromRoute] LoginWhich? id)
        {
            var url = ThirdLoginService.LoginLink(id, "bind");
            return Redirect(url);
        }

        /// <summary>
        /// 解绑账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> RidOAuth([FromRoute] LoginWhich? id)
        {
            var uinfo = IdentityService.Get(HttpContext);

            if (id.HasValue && ThirdLoginService.OpenIdMap.TryGetValue(id.Value, out string propertyName))
            {
                var query = db.UserInfo.Where(x => x.UserId == uinfo.UserId);
                switch (id)
                {
                    case LoginWhich.QQ:
                        await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.OpenId1, ""));
                        break;
                    case LoginWhich.Weibo:
                        await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.OpenId2, ""));
                        break;
                    case LoginWhich.GitHub:
                        await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.OpenId3, ""));
                        break;
                    case LoginWhich.Taobao:
                        await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.OpenId4, ""));
                        break;
                    case LoginWhich.Microsoft:
                        await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.OpenId5, ""));
                        break;
                    case LoginWhich.DingTalk:
                        await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.OpenId6, ""));
                        break;
                }
            }

            return Redirect("/user/setting");
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResultVM> UpdatePassword(string oldPassword, string newPassword)
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);

                var userInfo = await db.UserInfo.FindAsync(uinfo.UserId);
                if (userInfo.UserPwd == CalcTo.MD5(oldPassword))
                {
                    userInfo.UserPwd = CalcTo.MD5(newPassword);
                    //刷新登录标记
                    userInfo.UserSign = $"{CalcTo.MD5(userInfo.UserPwd, 16)}:{userInfo.UserSign.Split(':').Last()}";
                    db.UserInfo.Update(userInfo);
                    var num = await db.SaveChangesAsync();
                    if (num > 0)
                    {
                        //注销
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        //清空用户缓存
                        CacheTo.RemoveGroup(uinfo.UserId.ToString());
                    }

                    vm.Set(num > 0);
                }
                else
                {
                    vm.Set(EnumTo.RTag.unauthorized);
                    vm.Msg = "原密码错误";
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
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
        public async Task<IActionResult> Write([FromRoute] string id, int? page)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                int action = 1;
                if (id == "mark")
                {
                    action = 2;
                }

                var uinfo = IdentityService.Get(HttpContext);
                var vm = await CommonService.UserConnWritingQuery(uinfo.UserId, ConnectionType.UserWriting, action, page ?? 1);
                vm.Route = Request.Path;
                vm.Other = id;

                return View("~/Views/Home/_PartialWritingList.cshtml", vm);
            }

            return View();
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        [Authorize, HttpGet]
        public async Task<ResultVM> WriteList()
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);

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
                vm.Data = await query.ToListAsync();
                vm.Set(EnumTo.RTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 获取一篇文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResultVM> WriteOne(int id)
        {
            var vm = new ResultVM();

            var uinfo = IdentityService.Get(HttpContext);

            var mo = await db.UserWriting.FirstOrDefaultAsync(x => x.Uid == uinfo.UserId && x.UwId == id);
            var listTags = await db.UserWritingTags.Where(x => x.UwId == id).ToListAsync();

            vm.Data = new
            {
                item = mo,
                tags = listTags
            };
            vm.Set(EnumTo.RTag.success);

            return vm;
        }

        /// <summary>
        /// 保存一篇文章（编辑）
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="TagIds"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public async Task<ResultVM> WriteSave([FromForm] UserWriting mo, [FromForm] string TagIds)
        {
            var vm = new ResultVM();

            try
            {
                var lisTagId = new List<int>();
                TagIds.Split(',').ToList().ForEach(x => lisTagId.Add(Convert.ToInt32(x)));
                var lisTagName = (await CommonService.TagsQuery()).Where(x => lisTagId.Contains(x.TagId)).ToList();

                var uinfo = IdentityService.Get(HttpContext);

                //更新文章
                var num = await db.UserWriting
                    .Where(x => x.Uid == uinfo.UserId && x.UwId == mo.UwId && x.UwStatus != -1)
                    .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.UwTitle, mo.UwTitle)
                    .SetProperty(p => p.UwCategory, mo.UwCategory)
                    .SetProperty(p => p.UwContentMd, mo.UwContentMd)
                    .SetProperty(p => p.UwContent, mo.UwContent)
                    .SetProperty(p => p.UwUpdateTime, DateTime.Now));

                //删除标签
                num += await db.UserWritingTags.Where(x => x.UwId == mo.UwId).ExecuteDeleteAsync();

                //新增标签
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
                await db.UserWritingTags.AddRangeAsync(listwt);
                num += await db.SaveChangesAsync();

                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        /// <summary>
        /// 删除 一篇文章
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Authorize, HttpGet]
        public async Task<ResultVM> WriteDel(string ids)
        {
            var vm = new ResultVM();

            try
            {
                var uinfo = IdentityService.Get(HttpContext);
                var listKeyId = ids.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                var listKeys = listKeyId.Select(x => x.ToString()).ToList();

                var num = await db.UserWriting
                    .Where(x => x.Uid == uinfo.UserId && x.UwStatus != -1 && listKeyId.Contains(x.UwId))
                    .ExecuteDeleteAsync();
                num += await db.UserWritingTags.Where(x => listKeyId.Contains(x.UwId)).ExecuteDeleteAsync();
                num += await db.UserReply.Where(x => listKeys.Contains(x.UrTargetId)).ExecuteDeleteAsync();

                vm.Set(num > 0);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }

        #endregion

        #region 验证

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Verify([FromRoute] string id)
        {
            var vm = new ResultVM();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var vckey = AppTo.GetValue("Common:GlobalKey");
                var vcurl = AppTo.GetValue("Common:EmailVerificationLink");

                switch (id.ToLower())
                {
                    //发送验证邮箱
                    case "send":
                        {
                            if (User.Identity.IsAuthenticated)
                            {
                                var uinfo = IdentityService.Get(HttpContext);

                                var userInfo = await db.UserInfo.FindAsync(uinfo.UserId);
                                if (userInfo.UserMailValid == 1)
                                {
                                    vm.Msg = "邮箱已经完成验证";
                                }
                                else if (string.IsNullOrWhiteSpace(userInfo.UserMail))
                                {
                                    vm.Msg = "邮箱不能为空";
                                }
                                else
                                {
                                    var ckey = "VerifyMail";
                                    var gkey = userInfo.UserId.ToString();
                                    var issend = CacheTo.Get<bool>(ckey, gkey);
                                    if (issend == true)
                                    {
                                        vm.Msg = "5分钟内只能发送一次验证信息";
                                    }
                                    else
                                    {
                                        var mcs = await System.IO.File.ReadAllLinesAsync(Path.Combine(AppTo.WebRootPath, "file/mailchecker/list.txt"));
                                        if (mcs.Contains(userInfo.UserMail.Split('@').LastOrDefault().ToLower()))
                                        {
                                            vm.Msg = "该邮箱已被屏蔽";
                                        }
                                        else
                                        {
                                            //发送验证
                                            var toMail = userInfo.UserMail;

                                            var vjson = new
                                            {
                                                mail = toMail,
                                                ts = DateTime.Now.ToTimestamp()
                                            }.ToJson();

                                            var vcode = CalcTo.AESEncrypt(vjson, vckey).ToBase64Encode().ToUrlEncode();
                                            var verifyLink = string.Format(vcurl, vcode);
                                            var body = $"<div style='margin:1em auto;word-wrap:break-word'><div>验证您的邮箱：<b>{toMail}</b></div><br/>点击链接验证：<a href='{verifyLink}'>{verifyLink}</a></div>";

                                            vm = await PushService.SendEmail("验证您的邮箱", body, toMail);
                                            if (vm.Code == 200)
                                            {
                                                CacheTo.Set(ckey, true, 300, false, gkey);
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
                            var vjson = CalcTo.AESDecrypt(id.ToUrlDecode().ToBase64Decode(), vckey).DeJson();
                            if (DateTime.Now.ToTimestamp() - vjson.GetValue<int>("ts") < 60 * 5)
                            {
                                var mail = vjson.GetValue("mail");
                                if (string.IsNullOrWhiteSpace(mail))
                                {
                                    vm.Msg = "邮件地址有误";
                                }
                                else
                                {
                                    var userInfo = await db.UserInfo.FirstOrDefaultAsync(x => x.UserMail == mail);
                                    if (userInfo != null)
                                    {
                                        if (userInfo.UserMailValid == 1)
                                        {
                                            vm.Msg = "已验证，勿重复验证";
                                        }
                                        else
                                        {
                                            userInfo.UserMailValid = 1;
                                            db.UserInfo.Update(userInfo);

                                            int num = await db.SaveChangesAsync();

                                            vm.Set(num > 0);
                                            if (vm.Code == 200)
                                            {
                                                vm.Msg = "恭喜您，验证成功";
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