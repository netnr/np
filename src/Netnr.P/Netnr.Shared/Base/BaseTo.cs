#if Full || Base

namespace Netnr;

/// <summary>
/// 全局
/// </summary>
public partial class BaseTo
{
    /// <summary>
    /// 版本号
    /// </summary>
    public static string Version { get; set; } = "1.707.0";

    /// <summary>
    /// 编码注册
    /// </summary>
    public static void ReadyEncoding() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    /// <summary>
    /// 启用旧时间戳行为
    /// https://www.npgsql.org/efcore/release-notes/6.0.html
    /// </summary>
    public static void ReadyLegacyTimestamp() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    /// <summary>
    /// 项目根目录（非 WEB 项目）
    /// </summary>
    public static string ProjectRootPath { get; set; } = AppContext.BaseDirectory.Split(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)[0].TrimEnd(Path.DirectorySeparatorChar);

    /// <summary>
    /// 是 开发环境（需从 Web Program 赋值）
    /// </summary>
    public static bool IsDev { get; set; } = false;

    /// <summary>
    /// 启动时间
    /// </summary>
    public static DateTime StartTime { get; set; } = Process.GetCurrentProcess().StartTime;

    /// <summary>
    /// 启动参数
    /// </summary>
    public static List<string> CommandLineArgs { get; set; } = Environment.GetCommandLineArgs().ToList();

    /// <summary>
    /// 启动带参数
    /// </summary>
    public static bool IsWithArgs { get; set; } = CommandLineArgs.Count > 1;
}

#endif