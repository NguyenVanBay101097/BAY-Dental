using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string DefaultCode { get; set; }
        public decimal? PriceUnit { get; set; }

        public decimal? ListPrice { get; set; }

        /// <summary>
        /// hãng:  nếu là labo thì hãng nào ?
        /// </summary>
        public string Firm { get; set; }

        public decimal? LaboPrice { get; set; }

        public Guid UOMId { get; set; }
        public UoMBasic UOM { get; set; }
    }
}
