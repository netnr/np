#if Full || Web

namespace Netnr;

/// <summary>
/// 跨域中间件
/// </summary>
public class AllowCorsMiddleware(RequestDelegate next, AllowCorsMiddleware.MiddlewareOptions options = null)
{
    /// <summary>
    /// 中间件配置
    /// </summary>
    public class MiddlewareOptions
    {
        /// <summary>
        /// 指定跨域源，默认 *
        /// </summary>
        public string CustomOrigin { get; set; }
    }
    internal MiddlewareOptions Options { get; set; } = options ?? new();

    private readonly RequestDelegate next = next;

    public async Task Invoke(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<DisableCorsAttribute>() == null)
        {
            var allowHeaders = "Authorization,Content-Type";
            //!! tus 访问头
            if (context.Request.Path.Value.Contains("Tus"))
            {
                allowHeaders = "*";
            }

            if (context.Request.Method.Equals("OPTIONS"))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", context.Request.Headers.Origin);
                context.Response.Headers.Append("Access-Control-Allow-Methods", context.Request.Headers.AccessControlRequestMethod);
                context.Response.Headers.Append("Access-Control-Allow-Headers", allowHeaders);

                //预检缓存，单位：秒
                context.Response.Headers.Append("Access-Control-Max-Age", "1800");
                context.Response.StatusCode = StatusCodes.Status204NoContent;

                return;
            }
            else
            {
                string allowOrigin = null;

                var origin = context.Request.Headers.Origin;
                if (string.IsNullOrWhiteSpace(Options.CustomOrigin) || Options.CustomOrigin == "*")
                {
                    allowOrigin = "*";
                }
                else if (Options.CustomOrigin.Contains(origin))
                {
                    allowOrigin = origin;
                }

                if (allowOrigin != null)
                {
                    context.Response.Headers.Append("Access-Control-Allow-Origin", allowOrigin);
                    context.Response.Headers.Append("Access-Control-Allow-Methods", context.Request.Method);
                    context.Response.Headers.Append("Access-Control-Allow-Headers", allowHeaders);
                    // 携带 cookie
                    if (allowOrigin != "*")
                    {
                        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
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