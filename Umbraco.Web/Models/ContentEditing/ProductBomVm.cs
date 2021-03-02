﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductBomBasic
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
        public UoMBasic ProducUOM { get; set; }

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
        public UoMBasic ProducUOM { get; set; }

        public decimal Quantity { get; set; }

        public int Sequence { get; set; }
    }

    public class ProductBomSave
    {
        public Guid ProductId { get; set; }

        /// <summary>
        /// Product vat tu
        /// </summary>
        public Guid? MaterialProductId { get; set; }


        public Guid ProductUOMId { get; set; }

        public decimal Quantity { get; set; }

        public int Sequence { get; set; }
    }
}
