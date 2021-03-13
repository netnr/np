using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class SysButton
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string SbId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SbPid { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string SbBtnText { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SbBtnId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SbBtnClass { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SbBtnIcon { get; set; }
        public int? SbBtnOrder { get; set; }
        public int? SbStatus { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string SbDescribe { get; set; }
        public int? SbBtnGroup { get; set; }
        public int? SbBtnHide { get; set; }
    }
}
