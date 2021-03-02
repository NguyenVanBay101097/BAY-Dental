using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductRequestLineBasic
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid? ProductUOMId { get; set; }
        public UoMBasic ProducUOM { get; set; }


        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLineBasic SaleOrderLine { get; set; }

        public decimal ProductQty { get; set; }

        public int Sequence { get; set; }
    }

    public class ProductRequestLineDisplay
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid? ProductUOMId { get; set; }
        public UoMSimple ProducUOM { get; set; }


        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLineSimple SaleOrderLine { get; set; }

        public decimal ProductQty { get; set; }
        public decimal ProductQtyMax { get; set; }

        public int Sequence { get; set; }
    }

    public class ProductRequestLineSave
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? ProductUOMId { get; set; }


        public Guid? SaleOrderLineId { get; set; }

        public decimal ProductQty { get; set; }

        public int Sequence { get; set; }
    }
}
