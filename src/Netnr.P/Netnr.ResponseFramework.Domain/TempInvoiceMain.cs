using System;

namespace Netnr.ResponseFramework.Domain
{
    public partial class TempInvoiceMain
    {
        public string TimId { get; set; }
        public string TimNo { get; set; }
        public DateTime? TimDate { get; set; }
        public string TimStore { get; set; }
        public int? TimType { get; set; }
        public string TimSupplier { get; set; }
        public string TimUser { get; set; }
        public string TimRemark { get; set; }
        public string TimOwnerId { get; set; }
        public string TimOwnerName { get; set; }
        public DateTime? TimCreateTime { get; set; }
        public int? TimStatus { get; set; }
        public string Spare1 { get; set; }
        public string Spare2 { get; set; }
        public string Spare3 { get; set; }
    }
}
