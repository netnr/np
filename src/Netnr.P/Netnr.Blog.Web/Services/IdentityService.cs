using System.Security.Claims;

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
        public static UserInfo Get(HttpContext context)
        {
            var user = context.User;

            if (user.Identity.IsAuthenticated)
            {
                return new UserInfo
                {
                    UserId = Convert.ToInt32(user.FindFirst(ClaimTypes.PrimarySid)?.Value),
                    UserName = user.FindFirst(ClaimTypes.Name)?.Value,
                    Nickname = user.FindFirst(ClaimTypes.GivenName)?.Value,
                    UserSign = user.FindFirst(ClaimTypes.Sid)?.Value,
                    UserPhoto = user.FindFirst(ClaimTypes.UserData)?.Value
                };
            }
            else
            {
                var token = context.Request.Query["token"].ToString();
                var mo = TokenValid(token);
                if (mo == null)
                {
                    mo = new UserInfo();
                }
                return mo;
            }
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="mo">授权用户信息</param>
        /// <returns></returns>
        public static string TokenMake(UserInfo mo)
        {
            var key = AppTo.GetValue("Common:GlobalKey");

            var token = CalcTo.AESEncrypt(new
            {
                mo = new
                {
                    mo.UserId,
                    mo.UserName,
                    mo.Nickname,
                    mo.UserSign,
                    mo.UserPhoto
                },
                expired = DateTime.Now.AddDays(10).ToTimestamp()
            }.ToJson(), key);

            return token;
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static UserInfo TokenValid(string token)
        {
            UserInfo mo = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var key = AppTo.GetValue("Common:GlobalKey");

                    var jo = CalcTo.AESDecrypt(token, key).DeJson();

                    if (DateTime.Now.ToTimestamp() < jo.GetValue<long>("expired"))
                    {
                        mo = jo.GetValue("mo").DeJson<UserInfo>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return mo;
        }

        /// <summary>
        /// 信息完整
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ResultVM CompleteInfoValid(HttpContext context)
        {
            var vm = new ResultVM();

            if (!context.User.Identity.IsAuthenticated)
            {
                vm.Log.Add("先登录");
            }
            else
            {
                if (AppTo.GetValue<bool>("Common:CompleteInfo"))
                {
                    var uinfo = Get(context);

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
