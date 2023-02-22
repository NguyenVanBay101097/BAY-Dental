using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockHistory
    {
        public Guid? product_categ_id { get; set; }
        public ProductCategory ProductCateg { get; set; }

        public Guid? move_id { get; set; }
        public StockMove Move { get; set; }

        public Guid? location_id { get; set; }
        public StockLocation Location { get; set; }

        public Guid? company_id { get; set; }
        public Company Company { get; set; }

        public Guid? product_id { get; set; }
        public Product Product { get; set; }

        public decimal quantity { get; set; }

        public DateTime? date { get; set; }

        public double? price_unit_on_quant { get; set; }
    }
}
