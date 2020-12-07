using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(KeyName), Name = "KeyValueSynonym_KeyName")]
    public partial class KeyValueSynonym
    {
        [Key]
        [StringLength(50)]
        public string KsId { get; set; }
        [StringLength(255)]
        public string KeyName { get; set; }
        [StringLength(255)]
        public string KsName { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
