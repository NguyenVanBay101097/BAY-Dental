using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductBomBasic
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public ProductSimpleAutoComplete Product { get; set; }

        /// <summary>
        /// Product vat tu
        /// </summary>
        public Guid? MaterialProductId { get; set; }
        public ProductSimpleAutoComplete MaterialProduct { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoMSimple ProductUOM { get; set; }

        public decimal Quantity { get; set; }

        public int Sequence { get; set; }
    }

    public class ProductBomDisplay
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        /// <summary>
        /// Product vat tu
        /// </summary>
        public Guid? MaterialProductId { get; set; }
        public ProductSimple MaterialProduct { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }

        public decimal Quantity { get; set; }

        public int Sequence { get; set; }
    }

    public class ProductBomSave
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        /// <summary>
        /// Product vat tu
        /// </summary>
        public Guid? MaterialProductId { get; set; }


        public Guid ProductUOMId { get; set; }

        public decimal Quantity { get; set; }

        public int Sequence { get; set; }
    }

    public class ProductBomForSaleOrderLine
    {
        public Guid Id { get; set; }
        public string MaterialProductName { get; set; }
        public string ProductUOMName { get; set; }
        public decimal Quantity { get; set; }
        public int Sequence { get; set; }
    }
}
