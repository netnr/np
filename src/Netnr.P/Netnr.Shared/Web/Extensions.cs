#if Full || Web

using System.Text.Encodings.Web;

namespace Netnr;

/// <summary>
/// 扩展 web
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 获取文件 Content-Type
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetContentType(this string path)
    {
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        provider.TryGetContentType(path, out string contentType);
        return contentType ?? "application/octet-stream";
    }

    /// <summary>
    /// 文件下载
    /// </summary>
    /// <param name="fi"></param>
    /// <returns></returns>
    public static PhysicalFileResult ToDownloadResult(this FileInfo fi)
    {
        var result = new PhysicalFileResult(fi.FullName, fi.Name.GetContentType())
        {
            //FileDownloadName = fileInfo.Name, // 指定下载时客户端文件的名称
            EnableRangeProcessing = true,
            LastModified = fi.LastWriteTimeUtc,
            EntityTag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{fi.LastWriteTimeUtc.Ticks:x}\""),
        };
        return result;
    }

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

            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DataTableJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.DataSetJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.IPAddressJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.IPEndPointJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonConverterTo.TypeJsonConverter());
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

            var errBody = "Server-Error";

            var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (ex != null)
            {
                ConsoleTo.WriteCard(errBody, ex.Error);
                errBody = ex.Error.ToJson(true);
            }

            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Append("Access-Control-Allow-Methods", context.Request.Method);
            context.Response.Headers.Append("Access-Control-Allow-Headers", "*");
            await context.Response.WriteAsync(errBody);
        });
    }
}
#endif