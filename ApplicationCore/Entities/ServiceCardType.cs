using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Loại thẻ dịch vụ
    /// </summary>
    public class ServiceCardType: BaseEntity
    {
        public ServiceCardType()
        {
            Type = "cash";
            Period = "month";
            Active = true;
        }

        public string Name { get; set; }

        /// <summary>
        /// Loại
        /// cash: Thẻ tiền mặt
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Số tiền trong thẻ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Thời hạn
        /// month: Tháng
        /// </summary>
        public string Period { get; set; }

        public int? NbrPeriod { get; set; }

        /// <summary>
        /// Product để ghi doanh thu và tạo discount line
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public bool Active { get; set; }
    }
}
