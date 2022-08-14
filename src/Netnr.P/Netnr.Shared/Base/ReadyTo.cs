#if Full || Base

namespace Netnr;

/// <summary>
/// 准备
/// </summary>
public class ReadyTo
{
    /// <summary>
    /// 编码注册
    /// </summary>
    public static void EncodingReg()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// 启用旧时间戳行为
    /// </summary>
    public static void LegacyTimestamp()
    {
        //https://www.npgsql.org/efcore/release-notes/6.0.html
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    /// <summary>
    /// 项目根目录（非 WEB 项目）
    /// </summary>
    public static string ProjectRootPath { get; set; } = AppContext.BaseDirectory.Split(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)[0].TrimEnd(Path.DirectorySeparatorChar);
}

#endif