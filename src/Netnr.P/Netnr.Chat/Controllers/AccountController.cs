using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Netnr.Chat.Application.ViewModel;
using Netnr.SharedFast;
using Netnr.Core;
using chs = Netnr.Chat.Application.ChatHubService;

namespace Netnr.Chat.Controllers
{
    /// <summary>
    /// 账号
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly Data.ContextBase db;

        public AccountController(Data.ContextBase _db)
        {
            db = _db;
        }

        /// <summary>
        /// 获取用户授权信息
        /// </summary>
        /// <param name="access_token">授权Token</param>
        /// <returns></returns>
        [HttpGet]
        public SharedResultVM UserAuthInfo(string access_token)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua != null)
                {
                    if (chs.OnlineUser1.ContainsKey(ua.UserId))
                    {
                        ua.Conns = chs.OnlineUser1[ua.UserId].Conns;
                    }

                    vm.Data = ua;
                    vm.Set(SharedEnum.RTag.success);
                }

                return vm;
            });
        }

        /// <summary>
        /// 授权获取token
        /// </summary>
        /// <param name="chatLogin">登录信息</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM Token([FromBody] ChatLoginVM chatLogin)
        {
            return SharedResultVM.Try(vm =>
            {
                var isOk = true;

                //有效时间
                var now = DateTime.Now;
                var ae = GlobalTo.GetValue<int>("TokenManagement:AccessExpiration");
                var ed = now.AddSeconds(ae);

                var mo = new ChatUserTokenVM
                {
                    ExpireDate = ed.ToTimestamp(),
                    UserDevice = chatLogin.Device,
                    UserSign = chatLogin.Sign
                };

                //有账号、密码
                if (!string.IsNullOrWhiteSpace(chatLogin.UserName) && !string.IsNullOrWhiteSpace(chatLogin.Password))
                {
                    var pw = CalcTo.MD5(chatLogin.Password);

                    var uo = db.NChatUser.FirstOrDefault(x => x.CuUserName == chatLogin.UserName && x.CuPassword == pw);
                    if (uo != null)
                    {
                        mo.UserId = uo.CuUserId;
                        mo.UserName = uo.CuUserName;
                        mo.UserPhoto = uo.CuUserPhoto;
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.unauthorized);
                        vm.Msg = "账号或密码错误";
                        isOk = false;
                    }
                }
                //启用来宾用户
                else if (GlobalTo.GetValue<bool>("NetnrChat:EnableGuestUsers"))
                {
                    //新增
                    if (string.IsNullOrWhiteSpace(chatLogin.GuestId))
                    {
                        var uid = UniqueTo.LongId();

                        mo.UserId = "G-" + uid;
                        mo.UserName = "Guest-" + uid;
                    }
                    else
                    {

                    }
                }
                else
                {
                    vm.Set(SharedEnum.RTag.invalid);
                    vm.Msg = "账号或密码不能为空";
                    isOk = false;
                }

                if (isOk)
                {
                    SetAuth(mo, ed);

                    vm.Data = mo;
                    vm.Set(SharedEnum.RTag.success);
                }

                return vm;
            });
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="access_token">授权Token</param>
        /// <returns></returns>
        [HttpPost]
        public SharedResultVM RefreshToken(string access_token)
        {
            return SharedResultVM.Try(vm =>
            {
                var ua = chs.GetUserAuthInfo(HttpContext, access_token);
                if (ua == null)
                {
                    vm.Set(SharedEnum.RTag.invalid);
                }
                else
                {
                    var now = DateTime.Now;
                    //可用时间
                    var atime = ua.ExpireDate - now.ToTimestamp();
                    var ae = GlobalTo.GetValue<int>("TokenManagement:AccessExpiration");
                    var ed = now.AddSeconds(ae);

                    //失效时间小于，可刷新
                    var reSeconds = GlobalTo.GetValue<int>("TokenManagement:RefreshExpiration");
                    if (atime < reSeconds)
                    {
                        var mo = new ChatUserTokenVM()
                        {
                            UserId = ua.UserId,
                            UserName = ua.UserName,
                            UserPhoto = ua.UserPhoto,
                            UserDevice = ua.UserDevice,
                            UserSign = ua.UserSign,
                            ExpireDate = ed.ToTimestamp()
                        };

                        SetAuth(mo, ed);

                        vm.Data = mo;
                        vm.Set(SharedEnum.RTag.success);
                    }
                    else
                    {
                        vm.Set(SharedEnum.RTag.refuse);
                        vm.Msg = $"{atime - reSeconds} 秒以后才能刷新";
                    }
                }

                return vm;
            });
        }

        /// <summary>
        /// 写入授权
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="ed">过期时间</param>
        private void SetAuth(ChatUserTokenVM mo, DateTimeOffset ed)
        {
            //写入授权
            var claims = new List<Claim>();
            mo.GetType().GetProperties().ToList().ForEach(pi =>
            {
                claims.Add(new Claim(pi.Name, pi.GetValue(mo, null)?.ToString() ?? ""));
            });
            var cp = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = ed
            };
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, authProperties).Wait();

            //构建授权Token
            mo.BuildToken();
        }
    }
}