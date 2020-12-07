using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysDictionary
    {
        [Key]
        [StringLength(50)]
        public string SdId { get; set; }
        [StringLength(50)]
        public string SdPid { get; set; }
        [StringLength(200)]
        public string SdType { get; set; }
        [StringLength(200)]
        public string SdKey { get; set; }
        [StringLength(200)]
        public string SdValue { get; set; }
        public int? SdOrder { get; set; }
        public int? SdStatus { get; set; }
        [StringLength(200)]
        public string SdRemark { get; set; }
        [StringLength(50)]
        public string SdAttribute1 { get; set; }
        [StringLength(50)]
        public string SdAttribute2 { get; set; }
        [StringLength(50)]
        public string SdAttribute3 { get; set; }
    }
}
