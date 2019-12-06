using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Chương trình khuyến mãi
    /// </summary>
    public class PromotionProgram: BaseEntity
    {
        public PromotionProgram()
        {
            Active = true;
        }

        public string Name { get; set; }
        public bool Active { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Số đơn hàng đầu tiên áp dụng
        /// </summary>
        public int? MaximumUseNumber { get; set; }

        /// <summary>
        /// Những chi nhánh được áp dụng, để trống thì sẽ áp dụng cho tất cả các chi nhánh
        /// </summary>
        public ICollection<PromotionProgramCompanyRel> ProgramCompanyRels { get; set; } = new List<PromotionProgramCompanyRel>();

        /// <summary>
        /// Danh sách điều kiện và phần thưởng
        /// </summary>
        public ICollection<PromotionRule> Rules { get; set; } = new List<PromotionRule>();

        public ICollection<SaleOrderLine> SaleLines { get; set; } = new List<SaleOrderLine>();
    }
}
