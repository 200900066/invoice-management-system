using System;
using System.Collections.Generic;

namespace InvoiceManagement.Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedByUserId { get; set; }
        public decimal Total { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}
