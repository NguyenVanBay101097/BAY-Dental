using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionProductRuleDisplay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 3_global: Áp dụng tất cả dịch vụ
        /// 2_product_category: Áp dụng trên 1 nhóm dịch vụ cụ thể
        /// 0_product_variant: Áp dụng trên 1 dịch vụ cụ thể
        /// </summary>
        public string AppliedOn { get; set; }

        /// <summary>
        /// dịch vụ
        /// </summary>
        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        /// <summary>
        /// nhóm dịch vụ
        /// </summary>
        public Guid? CategId { get; set; }
        public ProductCategorySimple Categ { get; set; }

        public decimal? PercentFixed { get; set; }


    }
}
