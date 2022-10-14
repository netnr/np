using Microsoft.AspNetCore.Mvc.Filters;

namespace Netnr.Admin.Web.Filters
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public class FilterConfigs
    {
        /// <summary>
        /// 【过滤器】全局过滤器
        /// </summary>
        public class ActionFilter : ActionFilterAttribute
        {
            readonly Stopwatch sw = new();

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                sw.Restart();
                base.OnActionExecuting(context);
            }

            public override void OnResultExecuted(ResultExecutedContext context)
            {
                try
                {
                    sw.Stop();

                    //日志
                    var model = LoggingService.Build(context);
                    model.LogTimeCost = sw.ElapsedMilliseconds;
                    LoggingService.Write(model);
                }
                catch (Exception ex)
                {
                    ConsoleTo.Title("写入日志错误", ex.ToJson(true));
                }

                base.OnResultExecuted(context);
            }
        }

        /// <summary>
        /// 特性：已授权
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        public class IsAuth : Attribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                var vm = IdentityService.Get(context.HttpContext);
                if (vm == null)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Not Authorized",
                        StatusCode = 401
                    };
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

        }

        /// <summary>
        /// 特性：跨域
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        public class IsCORS : Attribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                var res = context.HttpContext.Response;
                var origin = context.HttpContext.Request.Headers.Origin;

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

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }
        }

    }
}
