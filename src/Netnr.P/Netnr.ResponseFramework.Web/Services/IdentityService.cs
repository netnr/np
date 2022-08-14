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

            if (user.Identity.IsAuthenticated)
            {
                return new LoginUserVM
                {
                    UserId = context.User.FindFirst(ClaimTypes.PrimarySid)?.Value,
                    UserName = context.User.FindFirst(ClaimTypes.Name)?.Value,
                    Nickname = context.User.FindFirst(ClaimTypes.GivenName)?.Value,
                    RoleId = context.User.FindFirst(ClaimTypes.Role)?.Value
                };
            }
            else
            {
                var token = context.Request.Headers["Authorization"].ToString();
                var mo = TokenValid(token);
                if (mo == null)
                {
                    mo = new LoginUserVM();
                }
                return mo;
            }
        }

        /// <summary>
        /// 获取登录用户角色信息
        /// </summary>
        /// <param name="context"></param>
        public static SysRole Role(HttpContext context)
        {
            var lui = Get(context);
            if (!string.IsNullOrWhiteSpace(lui.RoleId))
            {
                return CommonService.QuerySysRoleEntity(x => x.SrId == lui.RoleId);
            }
            return null;
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="mo">授权用户信息</param>
        /// <returns></returns>
        public static string TokenMake(LoginUserVM mo)
        {
            var key = AppTo.GetValue("Common:GlobalKey");

            var token = CalcTo.AESEncrypt(new
            {
                mo,
                expired = DateTime.Now.AddDays(10).ToTimestamp()
            }.ToJson(), key);

            return token;
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static LoginUserVM TokenValid(string token)
        {
            LoginUserVM mo = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var key = AppTo.GetValue("Common:GlobalKey");

                    var jo = CalcTo.AESDecrypt(token, key).DeJson();

                    if (DateTime.Now.ToTimestamp() < jo.GetValue<long>("expired"))
                    {
                        mo = jo.GetValue("mo").DeJson<LoginUserVM>();
                    }
                }
            }
            catch (Exception)
            {

            }

            return mo;
        }
    }
}
