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
        public decimal StandardPrice { get; set; }

        public bool IsLabo { get; set; }
    }

    //ProductDisplay + DS các step
    public class ProductDisplayAndStep : ProductDisplay
    {
        public List<ProductStepDisplay> stepList { get; set; }
    }
}
