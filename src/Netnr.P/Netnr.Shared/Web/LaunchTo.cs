#if Full || Web

namespace Netnr;

/// <summary>
/// 启动设定
/// </summary>
public static class LaunchTo
{
    /// <summary>
    /// JSON 统一配置
    /// </summary>
    /// <param name="builder"></param>
    public static void SetJson(this IMvcBuilder builder)
    {
        builder.AddNewtonsoftJson(options =>
        {
            //Action原样输出JSON
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            //日期格式化
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";

            //swagger枚举显示名称
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        });
    }

    /// <summary>
    /// 异常处理
    /// </summary>
    /// <param name="options"></param>
    public static void SetExceptionHandler(this IApplicationBuilder options)
    {
        options.Run(async context =>
        {
            context.Response.StatusCode = 500;

            var errBody = "Server Error";

            var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (ex != null)
            {
                try
                {
                    errBody = $"{DateTime.Now}\r\n{ex.Error}";
                    Console.WriteLine(errBody);
                    Core.ConsoleTo.Log(ex.Error);
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"写入错误日志失败：{ex2.Message}");
                }
            }

            await context.Response.WriteAsync(errBody);
        });
    }
}
#endif