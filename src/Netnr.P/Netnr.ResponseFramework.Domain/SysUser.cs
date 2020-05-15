using System;

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysUser
    {
        public string SuId { get; set; }
        public string SrId { get; set; }
        public string SuName { get; set; }
        public string SuPwd { get; set; }
        public string SuNickname { get; set; }
        public DateTime? SuCreateTime { get; set; }
        public int? SuStatus { get; set; }
        public string SuSign { get; set; }
        public int? SuGroup { get; set; }
    }
}
