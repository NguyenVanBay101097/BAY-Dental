using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductPricelistSave
    {
        public string Name { get; set; }

        public IEnumerable<ProductPricelistItemSave> Items { get; set; } = new List<ProductPricelistItemSave>();

        /// <summary>
        /// Gán datestart cho tất cả items
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// Gán dateend cho tất cả items
        /// </summary>
        public DateTime? DateEnd { get; set; }

        public Guid? PartnerCategId { get; set; }
        public PartnerCategoryBasic PartnerCateg { get; set; }

        public string DiscountPolicy { get; set; }

        public Guid? CompanyId { get; set; }
        public CompanyBasic Company { get; set; }
    }
}
