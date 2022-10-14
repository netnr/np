using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Netnr.Login;

namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 账号
    /// </summary>
    public class AccountController : Controller
    {
        public ContextBase db;

        public AccountController(ContextBase cb)
        {
            db = cb;
        }

        #region 注册

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 注册提交
        /// </summary>
        /// <param name="mo">账号、密码</param>
        /// <param name="RegisterCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register(UserInfo mo, string RegisterCode)
        {
            var vm = new ResultVM();
            if (string.IsNullOrWhiteSpace(RegisterCode) || HttpContext.Session.GetString("RegisterCode") != RegisterCode)
            {
                vm.Msg = "验证码错误或已过期";
            }
            else if (!(mo.UserName?.Length >= 5 && mo.UserPwd?.Length >= 5))
            {
                vm.Msg = "账号、密码长度至少 5 位数";
            }
            else
            {
                mo.UserPwd = CalcTo.MD5(mo.UserPwd);
                mo.UserCreateTime = DateTime.Now;

                //邮箱注册
                if (ParsingTo.IsMail(mo.UserName))
                {
                    mo.UserMail = mo.UserName;
                }
                vm = PublicRegister(mo);
            }

            ViewData["UserName"] = mo.UserName;

            HttpContext.Session.Remove("RegisterCode");
            return View(vm);
        }

        /// <summary>
        /// 注册验证码
        /// </summary>
        /// <returns></returns>
        public FileResult RegisterCode()
        {
            //生成验证码
            string num = RandomTo.NumCode(4);
            HttpContext.Session.SetString("RegisterCode", num);
            byte[] bytes = ImageTo.Captcha(num);
            return File(bytes, "image/jpeg");
        }

        #endregion

        #region 登录（本地）

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <param name="ReturnUrl">登录跳转链接</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            //记录登录跳转
            Response.Cookies.Append("ReturnUrl", ReturnUrl ?? "");

            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="mo">用户信息</param>
        /// <param name="remember">记住账号</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Login(UserInfo mo, int? remember)
        {
            var isRemember = remember == 1;
            var vm = PublicLogin(null, mo, isRemember);
            if (vm.Code == 200)
            {
                var rurl = Request.Cookies["ReturnUrl"];
                rurl = string.IsNullOrWhiteSpace(rurl) ? "/" : rurl;

                if (rurl.StartsWith("http"))
                {
                    rurl += "?cookie=ok";
                }

                return Redirect(rurl);
            }
            else
            {
                return View(vm);
            }
        }

        #endregion

        #region 登录（第三方）

        /// <summary>
        /// 第三方登录授权页面
        /// </summary>
        /// <param name="id">登录类型</param>
        /// <returns></returns>
        public IActionResult Auth([FromRoute] LoginWhich? id) => Redirect(ThirdLoginService.LoginLink(id, "login"));

        /// <summary>
        /// 登录授权回调
        /// </summary>
        /// <param name="id">哪家</param>
        /// <param name="authResult">接收授权码</param>
        /// <returns></returns>
        public IActionResult AuthCallback([FromRoute] LoginWhich? id, AuthorizeResult authResult)
        {
            try
            {
                if (id == null || !ThirdLoginService.OpenIdMap.ContainsKey(id.Value))
                {
                    throw new Exception($"不支持该方式授权 {RouteData.Values["id"]?.ToString()}");
                }
                else if (authResult.NoAuthCode())
                {
                    throw new Exception($"授权失败");
                }
                else
                {
                    var loginType = id.Value;
                    ConsoleTo.Title($"{loginType} login authorization callback");

                    var publicUser = LoginTo.Entry(loginType, authResult);
                    Console.WriteLine(publicUser.ToJson(true));

                    if (string.IsNullOrWhiteSpace(publicUser.UniqueId))
                    {
                        throw new Exception("登录失败，未能获取唯一标识");
                    }

                    //兼容历史版本取唯一标识
                    var uniqueId = publicUser.UniqueId;
                    if (loginType == LoginWhich.DingTalk && DingTalk.IsOld)
                    {
                        uniqueId = publicUser.OpenId;
                    }

                    var now = DateTime.Now;
                    //注册信息
                    var newUser = new UserInfo()
                    {
                        LoginLimit = 0,
                        UserCreateTime = now,
                        UserLoginTime = now,
                        Nickname = publicUser.Nickname ?? publicUser.Name,
                        UserSex = publicUser.Gender,
                        UserMail = publicUser.Email,
                        UserMailValid = publicUser.EmailVerified ? 1 : 0,
                        UserUrl = publicUser.Site,
                        UserSay = publicUser.Intro
                    };
                    if (string.IsNullOrWhiteSpace(newUser.Nickname))
                    {
                        newUser.Nickname = $"{loginType}用户";
                    }
                    newUser.UserName = uniqueId;
                    newUser.UserPwd = CalcTo.MD5(uniqueId);

                    //绑定用户
                    if (User.Identity.IsAuthenticated && authResult.State.StartsWith("bind"))
                    {
                        var bindResult = BindUser(loginType, uniqueId);
                        if (bindResult.Code == 200)
                        {
                            return Redirect("/user/setting");
                        }
                        else
                        {
                            throw new Exception($"绑定失败，{bindResult.Msg}");
                        }
                    }
                    else
                    {
                        UserInfo hasUser = null;
                        if (ThirdLoginService.OpenIdMap.ContainsKey(loginType))
                        {
                            var propertyName = ThirdLoginService.OpenIdMap[loginType];
                            //x.OpenId1 = UniqueId
                            var whereEqual = PredicateTo.Compare<UserInfo>(propertyName, "=", uniqueId);
                            hasUser = db.UserInfo.FirstOrDefault(whereEqual);
                            //newUser.OpenId1 = UniqueId
                            newUser.GetType().GetProperty(propertyName).SetValue(newUser, uniqueId);
                        }

                        //注册用户
                        if (hasUser == null)
                        {
                            if (!string.IsNullOrWhiteSpace(publicUser.Avatar) && loginType != LoginWhich.Microsoft)
                            {
                                newUser.UserPhoto = $"{UniqueTo.LongId()}.jpg";
                                try
                                {
                                    //物理根路径
                                    var ppath = CommonService.StaticResourcePath("AvatarPath");
                                    if (!Directory.Exists(ppath))
                                    {
                                        Directory.CreateDirectory(ppath);
                                    }

                                    HttpTo.DownloadSave(publicUser.Avatar, PathTo.Combine(ppath, newUser.UserPhoto));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"下载头像失败 {ex.Message}");
                                    LoggingService.Write(HttpContext, ex);
                                }
                            }

                            var ruResult = PublicRegister(newUser);
                            //注册成功
                            if (ruResult.Code == 200)
                            {
                                hasUser = newUser;
                            }
                            else
                            {
                                throw new Exception(ruResult.Msg);
                            }
                        }

                        //登录
                        if (hasUser != null)
                        {
                            var vlogin = PublicLogin(loginType, hasUser);
                            if (vlogin.Code == 200)
                            {
                                var rurl = Request.Cookies["ReturnUrl"];
                                rurl = string.IsNullOrWhiteSpace(rurl) ? "/" : rurl;

                                if (rurl.StartsWith("http"))
                                {
                                    rurl += "?cookie=ok";
                                }

                                return Redirect(rurl);
                            }
                            else
                            {
                                throw new Exception(vlogin.Msg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

            return BadRequest("授权失败");
        }

        #endregion

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout([FromRoute] string id)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //清空全局缓存
            CacheTo.RemoveAll();

            return Redirect("/" + (id ?? ""));
        }

        /// <summary>
        /// 用户绑定
        /// </summary>
        /// <param name="loginType"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        private ResultVM BindUser(LoginWhich? loginType, string openId)
        {
            var vm = new ResultVM();

            var uid = IdentityService.Get(HttpContext)?.UserId;

            var isBindOther = false;
            var queryIsBind = db.UserInfo.Where(x => x.UserId != uid);
            var userInfo = db.UserInfo.Find(uid);

            if (loginType.HasValue && ThirdLoginService.OpenIdMap.ContainsKey(loginType.Value))
            {
                var propertyName = ThirdLoginService.OpenIdMap[loginType.Value];
                //x.OpenId1 = UniqueId
                var whereEqual = PredicateTo.Compare<UserInfo>(propertyName, "=", openId);
                isBindOther = db.UserInfo.Where(whereEqual).Any();
                if (!isBindOther)
                {
                    //userInfo.OpenId1 = UniqueId;
                    userInfo.GetType().GetProperty(propertyName).SetValue(userInfo, openId);
                }
            }

            if (isBindOther)
            {
                vm.Set(EnumTo.RTag.exist);
                vm.Msg = "已绑定其它用户";
            }
            else
            {
                db.UserInfo.Update(userInfo);
                vm.Data = db.SaveChanges();
                vm.Set(EnumTo.RTag.success);
            }

            return vm;
        }

        /// <summary>
        /// 公共注册
        /// </summary>
        /// <param name="mo">个人用户信息</param>
        /// <returns></returns>
        private ResultVM PublicRegister(UserInfo mo)
        {
            var vm = new ResultVM();

            //邮箱注册
            if (!string.IsNullOrWhiteSpace(mo.UserMail)
                && db.UserInfo.Any(x => x.UserName == mo.UserName || x.UserMail == mo.UserMail))
            {
                vm.Set(EnumTo.RTag.exist);
                vm.Msg = "该邮箱已经注册";
            }
            else if (db.UserInfo.Any(x => x.UserName == mo.UserName))
            {
                vm.Set(EnumTo.RTag.exist);
                vm.Msg = "该账号已经注册";
            }
            else
            {
                db.UserInfo.Add(mo);
                int num = db.SaveChanges();
                vm.Set(num > 0);

                //推送通知
                _ = PushService.PushAsync("网站消息（注册）", $"{mo.UserId}");
            }

            return vm;
        }

        /// <summary>
        /// 公共登录
        /// </summary>
        /// <param name="loginType">登录类型</param>
        /// <param name="mo">用户信息</param>
        /// <param name="isRemember">记住账号</param>
        /// <returns></returns>
        private ResultVM PublicLogin(LoginWhich? loginType, UserInfo mo, bool isRemember = true)
        {
            var vm = new ResultVM();

            UserInfo loginUser = null;

            //第三方登录
            if (loginType.HasValue && ThirdLoginService.OpenIdMap.ContainsKey(loginType.Value))
            {
                loginUser = mo;
            }
            else if (!string.IsNullOrWhiteSpace(mo.UserName) && !string.IsNullOrWhiteSpace(mo.UserPwd))
            {
                mo.UserPwd = CalcTo.MD5(mo.UserPwd);

                //邮箱登录
                if (ParsingTo.IsMail(mo.UserName))
                {
                    loginUser = db.UserInfo.FirstOrDefault(x => x.UserMail == mo.UserName && x.UserPwd == mo.UserPwd);
                }
                else
                {
                    loginUser = db.UserInfo.FirstOrDefault(x => x.UserName == mo.UserName && x.UserPwd == mo.UserPwd);
                }
            }

            if (loginUser == null || loginUser.UserId == 0)
            {
                vm.Set(EnumTo.RTag.fail);
                vm.Msg = "用户名或密码错误";
            }
            else if (loginUser.LoginLimit == 1)
            {
                vm.Set(EnumTo.RTag.refuse);
                vm.Msg = "用户已被禁止登录";
            }
            else
            {
                //刷新登录标记
                if (!AppTo.GetValue<bool>("ReadOnly"))
                {
                    loginUser.UserLoginTime = DateTime.Now;
                    loginUser.UserSign = loginUser.UserLoginTime.Value.ToTimestamp().ToString();
                    db.UserInfo.Update(loginUser);
                    db.SaveChanges();
                }

                //登录标记 缓存5分钟，绝对过期
                if (AppTo.GetValue<bool>("Common:SingleSignOn"))
                {
                    var usk = "UserSign_" + loginUser.UserId;
                    CacheTo.Set(usk, loginUser.UserSign, 5 * 60, false);
                }

                //写入授权
                IdentityService.Set(HttpContext, loginUser, isRemember).Wait();

                //生成Token
                vm.Data = IdentityService.AccessTokenBuild(loginUser);

                vm.Set(EnumTo.RTag.success);
            }

            return vm;
        }
    }
}