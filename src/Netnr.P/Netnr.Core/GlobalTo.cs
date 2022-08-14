using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Netnr;

/// <summary>
/// 全局
/// </summary>
public static partial class GlobalTo
{
    /// <summary>
    /// 全局内存
    /// </summary>
    public static ConcurrentDictionary<string, object> Memorys { get; set; } = new ConcurrentDictionary<string, object>();

    /// <summary>
    /// 是 Windows
    /// </summary>
    public static bool IsWindows { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}
