namespace Netnr.ResponseFramework.Domain.Entities;

/// <summary>
/// 系统角色表
/// </summary>
public partial class SysRole
{
    public string SrId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string SrName { get; set; }

    /// <summary>
    /// 状态，1启用
    /// </summary>
    public int? SrStatus { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string SrDescribe { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public int? SrGroup { get; set; }

    /// <summary>
    /// 菜单
    /// </summary>
    public string SrMenus { get; set; }

    /// <summary>
    /// 按钮
    /// </summary>
    public string SrButtons { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? SrCreateTime { get; set; }
}
