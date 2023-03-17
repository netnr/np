#if Full || Web

namespace Netnr;

/// <summary>
/// 特性：跨域
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class FilterAllowCORS : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        SetHeaders(context.HttpContext);

        if (context.HttpContext.Request.Method == "OPTIONS")
        {
            context.Result = new OkResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    /// <summary>
    /// 设置头部
    /// </summary>
    /// <param name="context"></param>
    public static void SetHeaders(HttpContext context)
    {
        var origin = context.Request.Headers.Origin;

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
            context.Response.Headers.Remove(ak);
            context.Response.Headers.Add(ak, dicAk[ak]);
        }
    }
}

#endif