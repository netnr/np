using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Netnr.ResponseFramework.Domain
{
    public partial class TempExample
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string Id { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string TableName { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string ColField { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string DvTitle { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string ColTitle { get; set; }
        public int? ColWidth { get; set; }
        public int? ColAlign { get; set; }
        public int? ColHide { get; set; }
        public int? ColOrder { get; set; }
        public int? ColFrozen { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string ColFormat { get; set; }
        public int? ColSort { get; set; }
        public int? ColExport { get; set; }
        public int? ColQuery { get; set; }
        [Column(TypeName = "text")]
        public string FormUrl { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string FormType { get; set; }
        public int? FormArea { get; set; }
        public int? FormSpan { get; set; }
        public int? FormHide { get; set; }
        public int? FormOrder { get; set; }
        public int? FormRequired { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string FormPlaceholder { get; set; }
        [Column(TypeName = "text")]
        public string FormValue { get; set; }
        [Column(TypeName = "text")]
        public string FormText { get; set; }
    }
}
