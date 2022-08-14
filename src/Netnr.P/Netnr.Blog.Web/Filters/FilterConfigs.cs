using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Netnr.Blog.Web.Filters
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public class FilterConfigs
    {
        /// <summary>
        /// 【过滤器】全局过滤器
        /// </summary>
        public class GlobalFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                //日志

                var hc = context.HttpContext;

                if (string.IsNullOrWhiteSpace(hc.Request.Query["__nolog"].ToString()))
                {
                    string controller = context.RouteData.Values["controller"].ToString().ToLower();
                    string action = context.RouteData.Values["action"].ToString().ToLower();

                    //日志保存
                    var mo = LoggingService.Build(context.HttpContext);
                    mo.LogAction = controller + "/" + action;
                    if (LoggingService.DicDescription.ContainsKey(mo.LogAction))
                    {
                        mo.LogContent = LoggingService.DicDescription[mo.LogAction];
                    }

                    LoggingTo.Add(mo);
                }

                base.OnActionExecuting(context);
            }
        }

        /// <summary>
        /// 【过滤器】需要授权访问
        /// </summary>
        public class LoginSignValid : IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                //验证登录标记是最新，不是则注销登录（即同一用户不允许同时在线，按缓存时间生效）
                if (context.HttpContext.User.Identity.IsAuthenticated && AppTo.GetValue<bool>("Common:SingleSignOn"))
                {
                    var uinfo = IdentityService.Get(context.HttpContext);

                    string ServerSign = HelpFuncTo.GetLogonSign(uinfo.UserId);
                    if (uinfo.UserSign != ServerSign)
                    {
                        context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
            }
        }

        /// <summary>
        /// 【注解】允许跨域
        /// </summary>
        public class AllowCors : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var res = context.HttpContext.Response;

                var origin = context.HttpContext.Request.Headers["Origin"].ToString();

                var dicAk = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
                    { "Access-Control-Allow-Headers", "Accept, Authorization, Cache-Control, Content-Type, DNT, If-Modified-Since, Keep-Alive, Origin, User-Agent, X-Requested-With, Token, x-access-token" }
                };

                if (string.IsNullOrWhiteSpace(origin))
                {
                    dicAk.Add("Access-Control-Allow-Origin", "*");
                }
                else
                {
                    dicAk.Add("Access-Control-Allow-Origin", origin);
                    dicAk.Add("Access-Control-Allow-Credentials", "true");
                }

                foreach (var ak in dicAk.Keys)
                {
                    res.Headers.Remove(ak);
                    res.Headers.Add(ak, dicAk[ak]);
                }

                if (context.HttpContext.Request.Method == "OPTIONS")
                {
                    context.Result = new OkResult();
                }
            }
        }

        /// <summary>
        /// 【注解】是管理员
        /// </summary>
        public class IsAdmin : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                bool isv = false;

                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var uinfo = IdentityService.Get(context.HttpContext);
                    isv = uinfo.UserId == AppTo.GetValue<int>("Common:AdminId");
                }

                if (!isv)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "unauthorized",
                        StatusCode = 401
                    };
                }
            }
        }

        /// <summary>
        /// 【注解】完善信息
        /// </summary>
        public class IsCompleteInfo : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var vm = IdentityService.CompleteInfoValid(context.HttpContext);
                if (vm.Code != 200)
                {
                    context.Result = new RedirectResult("/home/completeinfo");
                }
            }
        }

        /// <summary>
        /// 【注解】有效授权（Cookie、Token）
        /// </summary>
        public class IsValidAuth : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var uinfo = IdentityService.Get(context.HttpContext);

                if (uinfo.UserId == 0)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "unauthorized",
                        StatusCode = 401
                    };
                }
            }
        }

        #region 辅助方法

        public class HelpFuncTo
        {
            /// <summary>
            /// 获取最新登录标记，用于对比本地，踢出下线
            /// </summary>
            /// <param name="UserId">登录的UserId</param>
            /// <param name="Cache">优先取缓存</param>
            /// <returns></returns>
            public static string GetLogonSign(int UserId, bool Cache = true)
            {
                string result = string.Empty;
                var usk = "UserSign_" + UserId;

                var us = CacheTo.Get<string>(usk);
                if (Cache && !string.IsNullOrEmpty(us))
                {
                    result = us;
                }
                else
                {
                    using var db = ContextBaseFactory.CreateDbContext();
                    var uiMo = db.UserInfo.Find(UserId);
                    if (uiMo != null)
                    {
                        result = uiMo.UserSign;
                        CacheTo.Set(usk, result, 5 * 60, false);
                    }
                }
                return result;
            }
        }

        #endregion
    }
}
