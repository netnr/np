using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Netnr.ResponseFramework.Domain;
using Netnr.ResponseFramework.Application.ViewModel;
using Netnr.Core;
using Netnr.SharedFast;

namespace Netnr.ResponseFramework.Web.Apps
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginService
    {
        #region 获取登录用户信息

        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <returns></returns>
        public static LoginUserVM GetLoginUserInfo(HttpContext context)
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
        public static SysRole LoginUserRoleInfo(HttpContext context)
        {
            var lui = GetLoginUserInfo(context);
            if (!string.IsNullOrWhiteSpace(lui.RoleId))
            {
                return Application.CommonService.QuerySysRoleEntity(x => x.SrId == lui.RoleId);
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
            var key = GlobalTo.GetValue("VerifyCode:Key");

            var token = CalcTo.EnDES(new
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
                    var key = GlobalTo.GetValue("VerifyCode:Key");

                    var jo = CalcTo.DeDES(token, key).ToJObject();

                    if (DateTime.Now.ToTimestamp() < long.Parse(jo["expired"].ToString()))
                    {
                        mo = jo["mo"].ToString().ToEntity<LoginUserVM>();
                    }
                }
            }
            catch (Exception)
            {

            }

            return mo;
        }

        #endregion

    }
}
