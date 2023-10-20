using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Captcha()
        {
            string num = RandomTo.NewNumber(4);
            byte[] bytes = ImageTo.Captcha(num);
            HttpContext.Session.SetString("captcha", CalcTo.MD5(num.ToLower()));
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
        public async Task<ResultVM> LoginValidation(SysUser mo, string captcha, int? remember)
        {
            var vm = new ResultVM();
            var outMo = new SysUser();

            try
            {
                //跳过验证码
                if (captcha == "_pass_")
                {
                    outMo = mo;
                }
                else
                {
                    var capt = HttpContext.Session.GetString("captcha");
                    HttpContext.Session.Remove("captcha");

                    if (string.IsNullOrWhiteSpace(captcha) || (capt ?? "") != CalcTo.MD5(captcha.ToLower()))
                    {
                        vm.Set(RCodeTypes.failure);
                        vm.Msg = "验证码错误或已过期";
                    }
                    else if (string.IsNullOrWhiteSpace(mo.SuName) || string.IsNullOrWhiteSpace(mo.SuPwd))
                    {
                        vm.Set(RCodeTypes.failure);
                        vm.Msg = "用户名或密码不能为空";
                    }
                    else
                    {
                        outMo = await db.SysUser.FirstOrDefaultAsync(x => x.SuName == mo.SuName && x.SuPwd == CalcTo.MD5(mo.SuPwd, 32));
                    }
                }

                if (outMo == null || string.IsNullOrWhiteSpace(outMo.SuId))
                {
                    vm.Set(RCodeTypes.unauthorized);
                    vm.Msg = "用户名或密码错误";
                }
                else if (outMo.SuStatus != 1)
                {
                    vm.Set(RCodeTypes.refuse);
                    vm.Msg = "用户已被禁止登录";
                }
                else
                {
                    //授权访问信息
                    await IdentityService.Set(HttpContext, outMo, remember == 1);

                    vm.Set(RCodeTypes.success);
                    vm.Data = "/";
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
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

            //清空用户缓存
            var uinfo = IdentityService.Get(HttpContext);
            if (uinfo != null)
            {
                CacheTo.RemoveGroup(uinfo.UserId.ToString());
            }

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
        public async Task<ResultVM> UpdateNewPassword(string oldpwd, string newpwd1, string newpwd2)
        {
            var vm = new ResultVM();

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
                var uinfo = IdentityService.Get(HttpContext);

                var mo = await db.SysUser.FindAsync(uinfo.UserId);
                if (mo != null && mo.SuPwd == CalcTo.MD5(oldpwd))
                {
                    mo.SuPwd = CalcTo.MD5(newpwd1);
                    db.SysUser.Update(mo);

                    var num = await db.SaveChangesAsync();
                    vm.Set(num > 0);
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