using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Netnr.Admin.Web.Services
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
                var vm = new LoginUserVM
                {
                    UserId = Convert.ToInt64(user.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    Account = user.FindFirst(ClaimTypes.Name)?.Value
                };
                if (vm.UserId > 0 && !string.IsNullOrWhiteSpace(vm.Account))
                {
                    vm.Nickname = user.FindFirst(ClaimTypes.GivenName)?.Value;
                    vm.RolesId = user.FindFirst(ClaimTypes.Role)?.Value;
                    vm.OrgId = Convert.ToInt64(user.FindFirst(ClaimTypes.GroupSid)?.Value);

                    return vm;
                }
            }
            else
            {
                var accessToken = context.Request.Headers.Authorization.ToString();
                var vm = AccessTokenParse(accessToken);
                return vm;
            }

            return null;
        }

        /// <summary>
        /// 设置授权用户
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <param name="remember">记住登录</param>
        /// <returns></returns>
        public static async Task Set(HttpContext context, BaseUser user, bool remember = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Account ?? ""),
                new Claim(ClaimTypes.GivenName, user.Nickname ?? ""),
                new Claim(ClaimTypes.Role, user.RolesId ?? ""),
                new Claim(ClaimTypes.GroupSid, user.OrgId?.ToString() ?? "")
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
        public static string AccessTokenBuild(BaseUser mo)
        {
            //密钥
            var key = AppTo.GetValue("Common:GlobalKey");

            var vm = new LoginUserVM()
            {
                UserId = mo.UserId,
                Account = mo.Account,
                Nickname = mo.Nickname,
                OrgId = mo.OrgId,
                RolesId = mo.RolesId,
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
    }
}
