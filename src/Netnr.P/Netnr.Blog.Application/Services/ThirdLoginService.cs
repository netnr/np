using Netnr.Login;

namespace Netnr.Blog.Application.Services
{
    /// <summary>
    /// 第三方登录
    /// </summary>
    public class ThirdLoginService
    {
        /// <summary>
        /// 登录链接
        /// </summary>
        /// <param name="id">哪家</param>
        /// <param name="statePrefix">登录防伪追加标识，区分登录、注册</param>
        /// <returns></returns>
        public static string LoginLink(LoginWhich? id, string statePrefix = "")
        {
            string url = string.Empty;

            if (id.HasValue && OpenIdMap.ContainsKey(id.Value))
            {
                var loginType = id.Value;

                //默认构建请求链接
                DocModel authResult = LoginTo.EntryOfStep<object, object>(loginType, LoginStep.Authorize, stateCall: (state) => $"{statePrefix}{state}");
                if (!string.IsNullOrEmpty(authResult.Raw))
                {
                    return authResult.Raw;
                }
            }

            return "/account/login";
        }

        /// <summary>
        /// 类型的OpenId与列名映射
        /// </summary>
        public static Dictionary<LoginWhich, string> OpenIdMap { get; set; } = new()
        {
            { LoginWhich.QQ, "OpenId1" },
            { LoginWhich.Weibo, "OpenId2" },
            { LoginWhich.Taobao, "OpenId4" },
            { LoginWhich.DingTalk, "OpenId6" },
            { LoginWhich.GitHub, "OpenId3" },
            { LoginWhich.Microsoft, "OpenId5" }
        };
    }
}
