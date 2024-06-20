using System.IO.Compression;

namespace Netnr;

/// <summary>
/// 配置初始化
/// </summary>
public class ConfigInit
{
    /// <summary>
    /// 构造
    /// </summary>
    public ConfigInit()
    {
        DXHub = Path.Combine(BaseTo.ProjectPath, "ud/hub");
        if (!Directory.Exists(DXHub))
        {
            Directory.CreateDirectory(DXHub);
        }

        var configPath = Path.Combine(BaseTo.ProjectPath, "ud/config.json");
        if (File.Exists(configPath))
        {
            DXConfig = File.ReadAllText(configPath).DeJson<ConfigOption>();

            DXConfig.ListConnectionInfo.ForEach(item =>
            {
                //设置连接对象以深拷贝构建新实例
                item.DeepCopyNewInstance = true;
                //解密
                item.ConnectionString = DbKitExtensions.SqlConnEncryptOrDecrypt(item.ConnectionString);
            });

            //修改配置
            if (DXConfig.BatchMaxRows > 0)
            {
                DataKitTo.BatchMaxRows = DXConfig.BatchMaxRows;
            }
            if (DXConfig.CompressLevel.HasValue)
            {
                DataKitTo.CompressLevel = DXConfig.CompressLevel.Value;
            }
        }
    }

    /// <summary>
    /// 简称
    /// </summary>
    public const string ShortName = "NDX";

    /// <summary>
    /// 枢纽
    /// </summary>
    public string DXHub { get; set; }

    public ConfigOption DXConfig { get; set; }
}

/// <summary>
/// 配置
/// </summary>
public class ConfigOption
{
    /// <summary>
    /// 映射匹配模式（读写 表、列）：Same（相同） Similar（相似）
    /// </summary>
    public string MapingMatchPattern { get; set; }

    /// <summary>
    /// 分批最大行数
    /// </summary>
    public int BatchMaxRows { get; set; }

    /// <summary>
    /// 导出压缩等级
    /// </summary>
    public CompressionLevel? CompressLevel { get; set; }

    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public List<DbKitConnectionOption> ListConnectionInfo { get; set; }

    /// <summary>
    /// 作业
    /// </summary>
    public JsonNode Works { get; set; }
}