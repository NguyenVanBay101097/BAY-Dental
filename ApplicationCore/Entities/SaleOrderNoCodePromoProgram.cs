using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Lưu lại những chương trình khuyến mãi tự động áp dụng cho SaleOrder
    /// </summary>
    public class SaleOrderNoCodePromoProgram
    {
        public Guid OrderId { get; set; }
        public SaleOrder Order { get; set; }

        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }
    }
}
