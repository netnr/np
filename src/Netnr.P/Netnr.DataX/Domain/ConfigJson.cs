namespace Netnr.DataX.Domain;

/// <summary>
/// 配置
/// </summary>
public class ConfigJson
{
    /// <summary>
    /// 映射匹配模式（读写 表、列）：Same（相同） Similar（相似）
    /// </summary>
    public string MapingMatchPattern { get; set; }

    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public List<DataKitTransferVM.ConnectionInfo> ListConnectionInfo { get; set; }

    /// <summary>
    /// 作业
    /// </summary>
    public JsonNode Works { get; set; }
}
