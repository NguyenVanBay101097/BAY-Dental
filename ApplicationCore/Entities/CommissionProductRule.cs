using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// quy dinh % hoa hong theo dich vu
    /// </summary>
    public class CommissionProductRule : BaseEntity
    {
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
        public Product Product { get; set; }

        /// <summary>
        /// nhóm dịch vụ
        /// </summary>
        public Guid? CategId { get; set; }
        public ProductCategory Categ { get; set; }

        public decimal? PercentFixed { get; set; }


        public Guid CommissionId { get; set; }
        public Commission Commission { get; set; }

        /// <summary>
        /// chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
