using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysMenu
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string SmId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SmPid { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SmName { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SmUrl { get; set; }
        public int? SmOrder { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SmIcon { get; set; }
        public int? SmStatus { get; set; }
        public int? SmGroup { get; set; }
    }
}
