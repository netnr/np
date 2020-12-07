#if Full || DataKit

namespace Netnr.SharedDataKit.Models
{
    /// <summary>
    /// 表信息
    /// </summary>
    public partial class DkTableName
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string TableComment { get; set; }
    }
}

#endif