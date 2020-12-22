using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderBasic
    {
        public Guid Id { get; set; }

        public string PartnerDisplayName { get; set; }

        public string PartnerName { get; set; }

        public string PartnerRef { get; set; }

        public string CustomerDisplayName { get; set; }

        public string CustomerName { get; set; }

        public string CustomerRef { get; set; }

        public string ProductName { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// ngày gửi
        /// </summary>
        public DateTime DateOrder { get; set; }

        
        /// <summary>
        /// ngày dự kiến
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        /// <summary>
        /// Ngày nhận thực tế
        /// </summary>
        public DateTime? DateReceipt { get; set; }

        /// <summary>
        /// Ngày xuất labo 
        /// 7 kêu trường này đặt tên như vậy
        /// </summary>
        public DateTime? DateExport { get; set; }

        /// <summary>
        /// màu sắc
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// đơn giá
        /// </summary>
        public decimal PriceUnit { get; set; }

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public string SaleOrderLineName { get; set; }

        public string SaleOrderId { get; set; }
        public string SaleOrderName { get; set; }


        public DateTime? DateCreated { get; set; }

        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();


        public string State { get; set; }
    }
}
