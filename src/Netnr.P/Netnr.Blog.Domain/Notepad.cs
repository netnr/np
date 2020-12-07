using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.Blog.Domain
{
    public partial class Notepad
    {
        [Key]
        public int NoteId { get; set; }
        public int? Uid { get; set; }
        [Column("NoteTItle")]
        [StringLength(100)]
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NoteCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NoteUpdateTime { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
    }
}
