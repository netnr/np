namespace Netnr.Admin.Web.Controllers
{
    /// <summary>
    /// 账号
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : MainController
    {
        public ContextBase db;

        public AccountController(ContextBase cb)
        {
            db = cb;
        }

        /// <summary>
        /// 登录校验
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultVM> Login([FromForm] string account, [FromForm] string password, [FromForm] string code)
        {
            var vm = new ResultVM();

            if (string.IsNullOrWhiteSpace(code))
            {
                vm.Set(EnumTo.RTag.fail);
                vm.Msg = "验证码错误";
            }
            else
            {
                var pwd = CalcTo.MD5(password);

                var mo = db.BaseUser.FirstOrDefault(x => x.Account == account && x.Password == pwd);
                if (mo == null)
                {
                    vm.Set(EnumTo.RTag.error);
                    vm.Msg = "账号或密码错误";
                }
                else if (mo.Status != 1)
                {
                    vm.Set(EnumTo.RTag.refuse);
                    vm.Msg = "该账号已被限制";
                }
                else
                {
                    await IdentityService.Set(HttpContext, mo);
                    vm.Set(EnumTo.RTag.success);
                }
            }

            return vm;
        }

    }
}
