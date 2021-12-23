using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleProductionLineBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// so luong dinh muc
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// so luong da yeu cau
        /// </summary>
        public decimal QuantityRequested { get; set; }
    }

    public class SaleProductionLineDisplay
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public UoMSimple ProductUOM { get; set; }

        /// <summary>
        /// so luong dinh muc
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// so luong da yeu cau
        /// </summary>
        public decimal QuantityRequested { get; set; }
    }
}
