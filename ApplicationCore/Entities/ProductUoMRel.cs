using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductUoMRel
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid UoMId { get; set; }
        public UoM UoM { get; set; }
    }
}
