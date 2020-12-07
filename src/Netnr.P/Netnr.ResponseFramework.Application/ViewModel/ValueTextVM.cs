using Newtonsoft.Json;

/// <summary>
/// 键值对，下拉列表等
/// </summary>
public class ValueTextVM
{
    /// <summary>
    /// 值
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; }

    /// <summary>
    /// 文本
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }
}