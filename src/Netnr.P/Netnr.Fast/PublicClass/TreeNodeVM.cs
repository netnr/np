using Newtonsoft.Json;

/// <summary>
/// Tree JSON 节点
/// 推荐所有的JSON输出用此实体，保证一致性，即页面接收的JSON全是这种格式，方便维护。
/// 如果不够用，自己灵活追加。
/// </summary>
public class TreeNodeVM
{
    /// <summary>
    /// ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// 父ID
    /// </summary>
    [JsonProperty("pid")]
    public string Pid { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }

    /// <summary>
    /// 拓展
    /// </summary>
    [JsonProperty("ext1")]
    public string Ext1 { get; set; }

    /// <summary>
    /// 拓展
    /// </summary>
    [JsonProperty("ext2")]
    public string Ext2 { get; set; }

    /// <summary>
    /// 拓展
    /// </summary>
    [JsonProperty("ext3")]
    public string Ext3 { get; set; }

    /// <summary>
    /// 备用
    /// </summary>
    [JsonProperty("spare1")]
    public object Spare1 { get; set; }

    /// <summary>
    /// 备用
    /// </summary>
    [JsonProperty("spare2")]
    public object Spare2 { get; set; }

    /// <summary>
    /// 备用
    /// </summary>
    [JsonProperty("spare3")]
    public object Spare3 { get; set; }
}