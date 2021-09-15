namespace Netnr.ResponseFramework.Domain
{
    /// <summary>
    /// 系统菜单表
    /// </summary>
    public partial class SysMenu
    {
        public string SmId { get; set; }
        public string SmPid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string SmName { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string SmUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SmOrder { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string SmIcon { get; set; }
        /// <summary>
        /// 状态，1启用
        /// </summary>
        public int? SmStatus { get; set; }
        /// <summary>
        /// 分组，默认1，比如移动端为2
        /// </summary>
        public int? SmGroup { get; set; }
    }
}
