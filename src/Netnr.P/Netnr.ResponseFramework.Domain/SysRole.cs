using System;

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysRole
    {
        public string SrId { get; set; }
        public string SrName { get; set; }
        public int? SrStatus { get; set; }
        public string SrDescribe { get; set; }
        public int? SrGroup { get; set; }
        public string SrMenus { get; set; }
        public string SrButtons { get; set; }
        public DateTime? SrCreateTime { get; set; }
    }
}
