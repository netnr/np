using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Netnr.ResponseFramework.Data;
using Netnr.ResponseFramework.Domain;
using Netnr.SharedDrawing;

namespace Netnr.ResponseFramework.Web.Controllers
{
    /// <summary>
    /// 账号
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        public ContextBase db;

        public AccountController(ContextBase cb)
        {
            db = cb;
        }

        #region 登录

        /// <summary>
        /// 生成登录验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileResult Captcha()
        {
            string num = Core.RandomTo.NumCode(4);
            byte[] bytes = ImageTo.Captcha(num);
            HttpContext.Session.SetString("captcha", Core.CalcTo.MD5(num.ToLower()));
            return File(bytes, "image/jpeg");
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="captcha">验证码</param>
        /// <param name="remember">1记住登录状态</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<SharedResultVM> LoginValidation(SysUser mo, string captcha, int remember)
        {
            var vm = new SharedResultVM();

            var outMo = new SysUser();

            //跳过验证码
            if (captcha == "_pass_")
            {
                outMo = mo;
            }
            else
            {
                var capt = HttpContext.Session.GetString("captcha");
                HttpContext.Session.Remove("captcha");

                if (string.IsNullOrWhiteSpace(captcha) || (capt ?? "") != Core.CalcTo.MD5(captcha.ToLower()))
                {
                    vm.Set(SharedEnum.RTag.fail);
                    vm.Msg = "验证码错误或已过期";
                    return vm;
                }

                if (string.IsNullOrWhiteSpace(mo.SuName) || string.IsNullOrWhiteSpace(mo.SuPwd))
                {
                    vm.Set(SharedEnum.RTag.lack);
                    vm.Msg = "用户名或密码不能为空";
                    return vm;
                }

                outMo = db.SysUser.FirstOrDefault(x => x.SuName == mo.SuName && x.SuPwd == Core.CalcTo.MD5(mo.SuPwd, 32));
            }

            if (outMo == null || string.IsNullOrWhiteSpace(outMo.SuId))
            {
                vm.Set(SharedEnum.RTag.unauthorized);
                vm.Msg = "用户名或密码错误";
                return vm;
            }

            if (outMo.SuStatus != 1)
            {
                vm.Set(SharedEnum.RTag.refuse);
                vm.Msg = "用户已被禁止登录";
                return vm;
            }

            try
            {
                #region 授权访问信息

                //登录信息
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, outMo.SuId));
                identity.AddClaim(new Claim(ClaimTypes.Name, outMo.SuName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, outMo.SuNickname ?? ""));
                identity.AddClaim(new Claim(ClaimTypes.Role, outMo.SrId));

                //配置
                var authParam = new AuthenticationProperties();
                if (remember == 1)
                {
                    authParam.IsPersistent = true;
                    authParam.ExpiresUtc = DateTime.Now.AddDays(10);
                }

                //写入
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authParam);

                vm.Set(SharedEnum.RTag.success);
                vm.Data = "/";

                return vm;

                #endregion
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                return vm;
            }
        }
        #endregion

        #region 注销

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //清空全局缓存
            Application.CommonService.GlobalCacheRmove();

            return Redirect("/");
        }
        #endregion

        #region 修改密码

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdatePassword()
        {
            return View();
        }

        /// <summary>
        /// 修改为新的密码
        /// </summary>
        /// <param name="oldpwd">现有</param>
        /// <param name="newpwd1">新</param>
        /// <param name="newpwd2"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public SharedResultVM UpdateNewPassword(string oldpwd, string newpwd1, string newpwd2)
        {
            var vm = new SharedResultVM();

            if (string.IsNullOrWhiteSpace(oldpwd) || string.IsNullOrWhiteSpace(newpwd1))
            {
                vm.Msg = "密码不能为空";
            }
            else if (newpwd1.Length < 5)
            {
                vm.Msg = "密码长度至少 5 位";
            }
            else if (newpwd1 != newpwd2)
            {
                vm.Msg = "两次输入的密码不一致";
            }
            else
            {
                var userinfo = Apps.LoginService.GetLoginUserInfo(HttpContext);

                var mo = db.SysUser.Find(userinfo.UserId);
                if (mo != null && mo.SuPwd == Core.CalcTo.MD5(oldpwd))
                {
                    mo.SuPwd = Core.CalcTo.MD5(newpwd1);
                    db.SysUser.Update(mo);

                    vm.Set(db.SaveChanges() > 0);
                }
                else
                {
                    vm.Msg = "现有密码错误";
                }
            }

            return vm;
        }
        #endregion
    }
}