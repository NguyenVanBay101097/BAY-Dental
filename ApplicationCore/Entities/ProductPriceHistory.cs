using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductPriceHistory: BaseEntity
    {
        public ProductPriceHistory()
        {
            DateTime = System.DateTime.Now;
        }

        public DateTime? DateTime { get; set; }

        public double Cost { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
