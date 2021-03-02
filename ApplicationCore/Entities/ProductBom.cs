using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductBom : BaseEntity
    {
        /// <summary>
        /// Cha
        /// </summary>
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Product vat tu
        /// </summary>
        public Guid? MaterialProductId { get; set; }
        public Product MaterialProduct { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoM ProducUOM { get; set; }

        public decimal Quantity { get; set; }

        public int Sequence { get; set; }
    }
}
