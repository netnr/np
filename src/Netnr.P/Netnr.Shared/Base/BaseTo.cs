#if Full || Base

using System.Collections.Generic;

namespace Netnr;

/// <summary>
/// 全局
/// </summary>
public partial class BaseTo
{
    /// <summary>
    /// 版本号
    /// </summary>
    public static string Version { get; set; } = $"{Environment.Version}";

    /// <summary>
    /// 编码注册
    /// </summary>
    public static void ReadyEncoding() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    /// <summary>
    /// 启用旧时间戳行为
    /// https://www.npgsql.org/efcore/release-notes/6.0.html
    /// </summary>
    public static void ReadyLegacyTimestamp() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    private static string _projectPath;
    /// <summary>
    /// 项目目录
    /// </summary>
    public static string ProjectPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_projectPath))
            {
                _projectPath = AppContext.BaseDirectory;

                var binDebug = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}";
                int binDebugIndex = _projectPath.IndexOf(binDebug);
                if (binDebugIndex > -1)
                {
                    _projectPath = _projectPath[..binDebugIndex];
                }
                else
                {
                    int binReleaseIndex = _projectPath.IndexOf(binDebug.Replace("Debug", "Release"));
                    if (binReleaseIndex > -1)
                    {
                        _projectPath = _projectPath[..binReleaseIndex];
                    }
                }
            }

            return _projectPath;
        }
        set
        {
            _projectPath = value;
        }
    }

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
    public static bool IsCmdArgs { get; set; } = CommandLineArgs.Count > 1;

    /// <summary>
    /// 命令行参数解析成键值对
    /// </summary>
    /// <param name="args">可选参数项，默认自取</param>
    /// <returns></returns>
    public static List<KeyValuePair<string, string>> CommandLineKeys(List<string> args = null)
    {
        args ??= CommandLineArgs;

        var result = new List<KeyValuePair<string, string>>();
        for (int i = 0; i < args.Count; i++)
        {
            var key = args[i];
            if (key.StartsWith('-'))
            {
                var val = i + 1 < args.Count ? args[i + 1] : "";
                result.Add(new KeyValuePair<string, string>(key, val.StartsWith('-') ? "" : val));
            }
        }
        return result;
    }
}

#endif