#if Full || Base

namespace Netnr;

/// <summary>
/// value-text
/// </summary>
public class ValueTextVM
{
    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 文本
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// 数据
    /// </summary>
    public object Data { get; set; }
}

#endif