using System.Text.Json.Serialization;

namespace Netnr.ResponseFramework.Domain.Models;

/// <summary>
/// 键值对，下拉列表等
/// </summary>
public class ValueTextVM
{
    /// <summary>
    /// 值
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// 文本
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
}