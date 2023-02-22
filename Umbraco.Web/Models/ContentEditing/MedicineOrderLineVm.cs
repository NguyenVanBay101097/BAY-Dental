using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MedicineOrderLineDisplay
    {
        public Guid Id { get; set; }
        public Guid ToaThuocLineId { get; set; }
        public ToaThuocLineDisplay ToaThuocLine { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal AmountTotal { get; set; }
        public Guid? ProductId { get; set; }
        public ProductBasic Product { get; set; }
        public Guid? ProductUoMId { get; set; }
        public UoMBasic ProductUoM { get; set; }
    }

    public class MedicineOrderLineSave
    {
        public Guid Id { get; set; }
        public Guid ToaThuocLineId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal AmountTotal { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductUoMId { get; set; }
    }
}
