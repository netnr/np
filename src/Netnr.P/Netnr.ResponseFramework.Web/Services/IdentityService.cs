using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Netnr.ResponseFramework.Web.Services
{
    /// <summary>
    /// 身份
    /// </summary>
    public class IdentityService
    {
        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <returns></returns>
        public static LoginUserVM Get(HttpContext context)
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var vm = new LoginUserVM
                {
                    UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    UserName = context.User.FindFirst(ClaimTypes.Name)?.Value,
                    Nickname = context.User.FindFirst(ClaimTypes.GivenName)?.Value,
                    RoleId = context.User.FindFirst(ClaimTypes.Role)?.Value
                };

                return vm;
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
        public static async Task Set(HttpContext context, SysUser user, bool remember = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.SuId),
                new Claim(ClaimTypes.Name, user.SuName),
                new Claim(ClaimTypes.GivenName, user.SuNickname ?? ""),
                new Claim(ClaimTypes.Role, user.SrId ?? "")
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
        public static string AccessTokenBuild(SysUser mo)
        {
            //密钥
            var key = AppTo.GetValue("Common:GlobalKey");

            var vm = new LoginUserVM()
            {
                UserId = mo.SuId,
                UserName = mo.SuName,
                Nickname = mo.SuNickname,
                RoleId = mo.SrId,
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
                ConsoleTo.WriteCard("accessToken 无效", accessToken);
                Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        /// 获取登录用户角色信息
        /// </summary>
        /// <param name="context"></param>
        public static SysRole Role(HttpContext context)
        {
            var uinfo = Get(context);
            if (!string.IsNullOrWhiteSpace(uinfo?.RoleId))
            {
                return CommonService.QuerySysRoleEntity(x => x.SrId == uinfo.RoleId);
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
            return uinfo?.UserName == AppTo.GetValue("Common:AdminName");
        }

    }
}
