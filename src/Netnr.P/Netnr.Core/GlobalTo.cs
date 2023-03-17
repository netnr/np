using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
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
    public static List<string> StartArgs { get; set; } = Environment.GetCommandLineArgs().ToList();

    /// <summary>
    /// 启动带参数
    /// </summary>
    public static bool IsStartWithArgs { get; set; } = StartArgs.Count > 1;

    /// <summary>
    /// 获取启动参数值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetArgsVal(string key)
    {
        var keyIndex = StartArgs.IndexOf(key);
        if (keyIndex != -1 && ++keyIndex < StartArgs.Count)
        {
            var val = StartArgs[keyIndex];
            if (!val.StartsWith("-"))
            {
                return val;
            }
        }
        return null;
    }
}
