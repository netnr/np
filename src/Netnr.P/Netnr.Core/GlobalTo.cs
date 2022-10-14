using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Netnr;

/// <summary>
/// 全局
/// </summary>
public partial class GlobalTo
{
    /// <summary>
    /// 是 Windows
    /// </summary>
    public static bool IsWindows { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

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
    public static string[] StartArgs { get; set; } = Environment.GetCommandLineArgs();

    /// <summary>
    /// 启动带参数
    /// </summary>
    public static bool StartWithArgs { get; set; } = StartArgs.Length > 1;
}
