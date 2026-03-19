using System;

namespace InvoiceManagement.Domain.Entities
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // snapshot of price at time of sale
    }
}
