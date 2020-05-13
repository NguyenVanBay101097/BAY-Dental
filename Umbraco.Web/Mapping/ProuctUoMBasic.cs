using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductUoMBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public decimal? PurchasePrice { get; set; }
        public UoMDisplay UOMPO { get; set; }
        public IEnumerable<UoMDisplay> UoMs { get; set; } = new List<UoMDisplay>();
    }
}
