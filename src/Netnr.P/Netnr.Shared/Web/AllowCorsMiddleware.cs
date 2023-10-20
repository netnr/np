#if Full || Web

using System.Reflection.PortableExecutable;

namespace Netnr;

/// <summary>
/// 跨域中间件
/// </summary>
public class AllowCorsMiddleware
{
    /// <summary>
    /// 中间件配置
    /// </summary>
    public class MiddlewareOptions
    {
        /// <summary>
        /// 指定跨域源
        /// </summary>
        public string CustomOrigin { get; set; }
    }
    internal MiddlewareOptions Options { get; set; }

    private readonly RequestDelegate next;

    public AllowCorsMiddleware(RequestDelegate next, MiddlewareOptions options = null)
    {
        Options = options ?? new();

        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<DisableCorsAttribute>() == null)
        {
            var allowHeaders = "Authorization,Content-Type";
            if (context.Request.Path.Value.Contains("Tus"))
            {
                allowHeaders = "*";
            }

            if (context.Request.Method.Equals("OPTIONS"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
                context.Response.Headers.Add("Access-Control-Allow-Methods", context.Request.Headers["Access-Control-Request-Method"]);
                context.Response.Headers.Add("Access-Control-Allow-Headers", allowHeaders);

                //预检缓存
                context.Response.Headers.Add("Access-Control-Max-Age", "600");
                context.Response.StatusCode = StatusCodes.Status200OK;

                return;
            }
            else
            {
                string allowOrigin = null;

                // 未获取到源默认 *
                var origin = context.Request.Headers["Origin"];
                if (string.IsNullOrWhiteSpace(origin) || Options.CustomOrigin == "*")
                {
                    allowOrigin = "*";
                }
                else if (string.IsNullOrWhiteSpace(Options.CustomOrigin) || Options.CustomOrigin?.Contains(origin) == true)
                {
                    allowOrigin = origin;
                }

                if (allowOrigin != null)
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", allowOrigin);
                    context.Response.Headers.Add("Access-Control-Allow-Methods", context.Request.Method);
                    context.Response.Headers.Add("Access-Control-Allow-Headers", allowHeaders);
                    // 携带 cookie
                    if (allowOrigin != "*")
                    {
                        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    }
                }

                //读取 Response.Body 并重写
                //if (true)
                //{
                //    var originalBody = context.Response.Body;

                //    await using var memoryStream = new MemoryStream();
                //    context.Response.Body = memoryStream;

                //    await next(context);

                //    memoryStream.Seek(0, SeekOrigin.Begin);
                //    var readBody = await new StreamReader(memoryStream).ReadToEndAsync();
                //    memoryStream.Seek(0, SeekOrigin.Begin);

                //    context.Response.Body = originalBody;
                //    await context.Response.Body.WriteAsync(readBody.ToByte());
                //    return;
                //}
            }
        }

        await next(context);
    }
}

#endif