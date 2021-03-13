using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(Uid), Name = "IDXNotepad_Uid")]
    public partial class Notepad
    {
        [Key]
        public int NoteId { get; set; }
        public int? Uid { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string NoteTitle { get; set; }
        [Column(TypeName = "longtext")]
        public string NoteContent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NoteCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NoteUpdateTime { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
    }
}
