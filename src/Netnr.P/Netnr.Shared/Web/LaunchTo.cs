#if Full || Web

namespace Netnr;

/// <summary>
/// 启动设定
/// </summary>
public static class LaunchTo
{
    /// <summary>
    /// 设置最大请求数据长度
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="maxLength">单位：B</param>
    public static void SetMaxRequestData(this WebApplicationBuilder builder, long? maxLength = null)
    {
        if (maxLength == null)
        {
            var maxSize = builder.Configuration.GetValue<int?>("StaticResource:MaxSize");
            if (!maxSize.HasValue)
            {
                maxSize = 50;
            }
            maxLength = maxSize * 1024 * 1024;
        }

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = maxLength.Value;
        });
        builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = maxLength.Value;
        });
    }

    /// <summary>
    /// 配置 JSON
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder SetJsonConfig(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true; //包含字段 如元组 Tuple
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // 原样输出，首字母不转小写
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DateTimeJsonConverter()); // 时间格式化
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //枚举字符串
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DataTableJsonConverter()); //数据表格式化
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DataSetJsonConverter()); //数据集格式化
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
                    ConsoleTo.Log(ex.Error);
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