namespace Netnr.DataX.Domain
{
    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public class DbConnObj
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public SharedEnum.TypeDB TDB { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Conn { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
