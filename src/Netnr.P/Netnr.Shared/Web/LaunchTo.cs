#if Full || Web

using System.Text.Encodings.Web;

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
    /// JSON 序列化配置
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder SetJsonConfig(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            //编码
            options.JsonSerializerOptions.IncludeFields = true; //包含字段 如元组 Tuple
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // 原样输出，首字母不转小写
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping; //编码
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DateTimeJsonConverter()); // DateTime
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //Enum 枚举字符串
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DataTableJsonConverter()); //DataTable
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DataSetJsonConverter()); //DataSet
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.IPAddressJsonConverter()); //IPAddress
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.IPEndPointJsonConverter()); //IPEndPoint
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
                ConsoleTo.Title(errBody, ex.Error);
                errBody = $"{DateTime.Now}\r\n{ex.Error}";
            }

            await context.Response.WriteAsync(errBody);
        });
    }
}
#endif