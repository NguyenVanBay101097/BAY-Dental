﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    //có thể từ nhiều phiếu
    public class ProductRequestLine : BaseEntity
    {
        /// <summary>
        /// meterial product
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? ProductUOMId { get; set; } 
        public UoM ProducUOM { get; set; }

        public Guid RequestId { get; set; }
        public ProductRequest Request { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        public decimal ProductQty { get; set; }

        public int Sequence { get; set; }
    }
}
