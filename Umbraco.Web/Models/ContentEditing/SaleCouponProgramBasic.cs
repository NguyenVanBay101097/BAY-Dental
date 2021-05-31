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
        public DateTime? RuleDateFrom { get; set; }
        public DateTime? RuleDateTo { get; set; }
        public int? MaximumUseNumber { get; set; }

        public bool IsPaused { get; set; }

        public string StatusDisplay
        {
            get
            {
                if (!Active)
                    return "Lưu nháp";
                var now = DateTime.Today;
                if (now > RuleDateTo)
                    return "Hết hạn";
                if (IsPaused)
                    return "Tạm ngừng";
                if (now >= RuleDateFrom && now <= RuleDateTo)
                    return "Đang chạy";
              
                return "Chưa chạy";
            }
        }
    }

}
