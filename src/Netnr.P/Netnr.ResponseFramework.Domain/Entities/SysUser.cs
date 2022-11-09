namespace Netnr.ResponseFramework.Domain.Entities;

/// <summary>
/// 系统用户表
/// </summary>
public partial class SysUser
{
    public string SuId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public string SrId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string SuName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string SuPwd { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string SuNickname { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? SuCreateTime { get; set; }

    /// <summary>
    /// 状态，1正常
    /// </summary>
    public int? SuStatus { get; set; }

    /// <summary>
    /// 登录标识
    /// </summary>
    public string SuSign { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public int? SuGroup { get; set; }
}
