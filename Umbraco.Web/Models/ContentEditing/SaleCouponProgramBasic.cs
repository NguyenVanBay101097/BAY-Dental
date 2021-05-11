using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponProgramBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Sản phẩm cho dòng chiết khấu
        /// </summary>
        public Guid? DiscountLineProductId { get; set; }
        public ProductSimple DiscountLineProduct { get; set; }

        public bool Active { get; set; }
        public string Status { get; set; }
        public DateTime? RuleDateFrom { get; set; }
        public DateTime? RuleDateTo { get; set; }
        public int? MaximumUseNumber { get; set; }
    }

}
