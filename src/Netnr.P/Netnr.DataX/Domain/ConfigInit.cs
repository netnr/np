namespace Netnr.DataX.Domain;

/// <summary>
/// 配置初始化
/// </summary>
public class ConfigInit
{
    public ConfigInit()
    {
        DXHub = Path.Combine(ReadyTo.ProjectRootPath, "ud/hub");
        if (!Directory.Exists(DXHub))
        {
            Directory.CreateDirectory(DXHub);
        }

        var configPath = Path.Combine(ReadyTo.ProjectRootPath, "ud/config.json");
        if (File.Exists(configPath))
        {
            DXConfig = File.ReadAllText(configPath).DeJson<ConfigJson>();
        }
    }

    /// <summary>
    /// 简称
    /// </summary>
    public const string ShortName = "NDX";

    /// <summary>
    /// 版本号（TDB）
    /// </summary>
    public const string Version = "1.0.1"; //2022-09

    /// <summary>
    /// 枢纽
    /// </summary>
    public string DXHub { get; set; }

    public ConfigJson DXConfig { get; set; }
}
