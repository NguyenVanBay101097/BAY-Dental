using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductDisplay: ProductBasic
    {
        public ProductDisplay()
        {
            Type = "consu";
            Active = true;
            SaleOK = true;
            PurchaseOK = true;
            LaboPrice = 0;
            PurchasePrice = 0;
        }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public double StandardPrice { get; set; }

        public IEnumerable<ProductStepDisplay> StepList { get; set; } = new List<ProductStepDisplay>();

        public IEnumerable<Guid> UoMIds { get; set; } = new List<Guid>();

        public UoMBasic UOM { get; set; }

        public UoMBasic UOMPO { get; set; }

        public ProductCategoryBasic Categ { get; set; }

        public string Firm { get; set; }
        /// <summary>
        /// list định mức vật tư
        /// </summary>
        public IEnumerable<ProductBomBasic> Boms { get; set; } = new List<ProductBomBasic>();
    }
}
