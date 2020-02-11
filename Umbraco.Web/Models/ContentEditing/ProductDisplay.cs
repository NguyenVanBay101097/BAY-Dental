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
        /// Có thể kê toa 
        /// </summary>
        public bool KeToaOK { get; set; }

        /// <summary>
        /// Ghi hướng dẫn sử dụng khi tạo toa thuốc
        /// </summary>
        public string KeToaNote { get; set; }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public double StandardPrice { get; set; }

        public bool IsLabo { get; set; }

        public string Type2 { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? LaboPrice { get; set; }

        public IEnumerable<ProductStepDisplay> StepList { get; set; } = new List<ProductStepDisplay>();
    }
}
