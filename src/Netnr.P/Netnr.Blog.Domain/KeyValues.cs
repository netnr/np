using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class KeyValues
    {
        [Key]
        [StringLength(50)]
        public string KeyId { get; set; }
        [StringLength(255)]
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public string KeyValue1 { get; set; }
        public string KeyValue2 { get; set; }
        public string KeyValue3 { get; set; }
        public string KeyValue4 { get; set; }
        public string KeyValue5 { get; set; }
        [StringLength(50)]
        public string KeyType { get; set; }
        [StringLength(50)]
        public string KeyRemark { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
