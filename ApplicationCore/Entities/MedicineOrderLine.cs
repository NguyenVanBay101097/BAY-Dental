﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MedicineOrderLine : BaseEntity
    {
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal AmountTotal { get; set; }

        public Guid MedicineOrderId { get; set; }
        public MedicineOrder MedicineOrder { get; set; }

        public Guid ToaThuocLineId { get; set; }
        public ToaThuocLine ToaThuocLine { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? ProductUoMId { get; set; }
        public UoM ProductUoM { get; set; }
    }
}
