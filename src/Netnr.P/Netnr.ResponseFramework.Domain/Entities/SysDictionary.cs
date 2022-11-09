namespace Netnr.ResponseFramework.Domain.Entities;

/// <summary>
/// 系统字典表
/// </summary>
public partial class SysDictionary
{
    public string SdId { get; set; }

    /// <summary>
    /// 上级ID
    /// </summary>
    public string SdPid { get; set; }

    /// <summary>
    /// 字典类别
    /// </summary>
    public string SdType { get; set; }

    /// <summary>
    /// 键
    /// </summary>
    public string SdKey { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string SdValue { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? SdOrder { get; set; }

    /// <summary>
    /// 状态：1正常，-1删除，2停用
    /// </summary>
    public int? SdStatus { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string SdRemark { get; set; }

    /// <summary>
    /// 特性
    /// </summary>
    public string SdAttribute1 { get; set; }

    /// <summary>
    /// 特性
    /// </summary>
    public string SdAttribute2 { get; set; }

    /// <summary>
    /// 特性
    /// </summary>
    public string SdAttribute3 { get; set; }
}
