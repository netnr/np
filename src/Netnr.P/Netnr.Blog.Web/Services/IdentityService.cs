using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using static Netnr.Blog.Web.Filters.FilterConfigs;

namespace Netnr.Blog.Web.Services
{
    /// <summary>
    /// 身份
    /// </summary>
    public class IdentityService
    {
        /// <summary>
        /// 获取授权用户
        /// </summary>
        /// <returns></returns>
        public static LoginUserVM Get(HttpContext context)
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                return new LoginUserVM
                {
                    UserId = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    UserName = user.FindFirst(ClaimTypes.Name)?.Value,
                    Nickname = user.FindFirst(ClaimTypes.GivenName)?.Value,
                    UserSign = user.FindFirst(ClaimTypes.UserData)?.Value,
                    UserPhoto = user.FindFirst("Gravatar")?.Value
                };
            }
            else
            {
                var accessToken = context.Request.Headers.Authorization.ToString();
                var vm = AccessTokenParse(accessToken);
                return vm;
            }
        }

        /// <summary>
        /// 设置授权用户
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <param name="remember">记住登录</param>
        /// <returns></returns>
        public static async Task Set(HttpContext context, UserInfo user, bool remember = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.GivenName, user.Nickname ?? ""),
                new Claim(ClaimTypes.UserData, user.UserSign ?? ""),
                new Claim("Gravatar", user.UserPhoto ?? "")
            };

            var cp = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            var ap = new AuthenticationProperties();
            if (remember)
            {
                ap.IsPersistent = true; //记住
                ap.ExpiresUtc = DateTimeOffset.Now.AddDays(10);
            }

            //写入授权
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, ap);
        }

        /// <summary>
        /// 授权码构建
        /// </summary>
        /// <param name="mo">登录用户信息</param>
        /// <returns></returns>
        public static string AccessTokenBuild(UserInfo mo)
        {
            //密钥
            var key = AppTo.GetValue("Common:GlobalKey");

            var vm = new LoginUserVM()
            {
                UserId = mo.UserId,
                UserName = mo.UserName,
                Nickname = mo.Nickname,
                UserSign = mo.UserSign,
                UserPhoto = mo.UserPhoto,
                Expired = DateTime.UtcNow.AddDays(1).Ticks
            };

            var accessToken = CalcTo.AESEncrypt(vm.ToJson(), key).ToBase64Encode();
            return accessToken;
        }

        /// <summary>
        /// 授权码解析
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static LoginUserVM AccessTokenParse(string accessToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    //密钥
                    var key = AppTo.GetValue("Common:GlobalKey");
                    var vm = CalcTo.AESDecrypt(accessToken.ToBase64Decode(), key).DeJson<LoginUserVM>();
                    if (vm.Expired >= DateTime.UtcNow.Ticks)
                    {
                        return vm;
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleTo.Title("accessToken 无效", accessToken);
                Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        /// 是管理员
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsAdmin(HttpContext context)
        {
            var uinfo = Get(context);
            return uinfo?.UserId == AppTo.GetValue<int>("Common:AdminId");
        }

        /// <summary>
        /// 单一在线，验证登录标记是最新，不是则注销登录（即同一用户不允许同时在线，按缓存时间生效）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static void SingleOnline(HttpContext context)
        {
            var uinfo = Get(context);
            //已登录、启用单一在线
            if (uinfo != null && AppTo.GetValue<bool>("Common:SingleOnline"))
            {
                var ckey = $"SingleOnline-{uinfo.UserId}";
                var cval = CacheTo.Get<string>(ckey);
                if (string.IsNullOrEmpty(cval))
                {
                    using var db = ContextBaseFactory.CreateDbContext();
                    var userInfo = db.UserInfo.Find(uinfo.UserId);
                    cval = userInfo.UserSign;
                    CacheTo.Set(ckey, cval, 5 * 60, false);
                }

                //登录标记不相同，退出登录
                if (uinfo.UserSign != cval)
                {
                    context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }

        /// <summary>
        /// 信息完整
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ResultVM CompleteInfoValid(HttpContext context)
        {
            var vm = new ResultVM();

            var uinfo = Get(context);
            if (uinfo == null)
            {
                vm.Log.Add("先登录");
            }
            else
            {
                if (AppTo.GetValue<bool>("Common:CompleteInfo"))
                {
                    if (string.IsNullOrWhiteSpace(uinfo.Nickname))
                    {
                        vm.Log.Add("填写昵称");
                    }

                    using var db = ContextBaseFactory.CreateDbContext();
                    var umo = db.UserInfo.Find(uinfo.UserId);

                    if (umo.UserId != AppTo.GetValue<int>("Common:AdminId"))
                    {
                        if (umo.UserMailValid != 1)
                        {
                            vm.Log.Add("验证邮箱");
                        }

                        if (string.IsNullOrWhiteSpace(umo.UserPhone) || umo.UserPhone.Trim().Length != 11)
                        {
                            vm.Log.Add("填写手机号码");
                        }

                        if (string.IsNullOrWhiteSpace(umo.OpenId1)
                            && string.IsNullOrWhiteSpace(umo.OpenId2)
                            && string.IsNullOrWhiteSpace(umo.OpenId3)
                            && string.IsNullOrWhiteSpace(umo.OpenId4)
                            && string.IsNullOrWhiteSpace(umo.OpenId5)
                            && string.IsNullOrWhiteSpace(umo.OpenId6))
                        {
                            vm.Log.Add("绑定一项授权关联");
                        }

                        if (umo.UserCreateTime.Value.AddDays(15) > DateTime.Now)
                        {
                            vm.Log.Add("新注册用户需 15 天以后才能操作");
                        }
                    }
                }
            }

            vm.Set(vm.Log.Count == 0);
            vm.Msg = string.Join("、", vm.Log);

            return vm;
        }
    }
}
