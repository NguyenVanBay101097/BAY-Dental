using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class QuotationLine : BaseEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// Dịch vụ
        /// </summary>
        public Product Product { get; set; }
        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng dịch vụ
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Số tiền Giảm giá , số % giảm giá
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// Loại giảm giá : percent / cash 
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Tổng tiền trên 1 dịch vụ
        /// </summary>
        public decimal? Amount { get; set; }

        public decimal? SubPrice { get; set; }



        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        /// <summary>
        /// Loại răng: răng sữa, răng vĩnh viễn !!!
        /// </summary>
        public ToothCategory ToothCategory { get; set; }
        public Guid ToothCategoryId { get; set; }

        public ICollection<QuotationLineToothRel> QuotationLineToothRels { get; set; } = new List<QuotationLineToothRel>();

        /// <summary>
        /// Phiếu báo giá
        /// </summary>
        public Guid QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        /// <summary>
        /// Nhân viên tư vấn
        /// </summary>
        public string AdvisoryUserId { get; set; }
        public ApplicationUser AdvisoryUser { get; set; }

        /// <summary>
        /// Tư vấn
        /// </summary>
        public Guid? AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }
    }
}
