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
        public class SingleOnlineFilter : IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                IdentityService.LoginCheck(context.HttpContext);
            }
        }

        /// <summary>
        /// 【注解】允许跨域
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        public class IsCors : Attribute, IActionFilter
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
        [AttributeUsage(AttributeTargets.All)]
        public class IsAdmin : Attribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (!IdentityService.IsAdmin(context.HttpContext))
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
        [AttributeUsage(AttributeTargets.All)]
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
    }
}
