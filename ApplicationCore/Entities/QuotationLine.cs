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
        ///số % giảm giá
        /// </summary>
        public decimal? DiscountAmountPercent { get; set; }

        /// <summary>
        /// Số tiền Giảm giá 
        /// </summary>
        public decimal? DiscountAmountFixed { get; set; }

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
        /// Tổng đơn giá giảm của dịch vụ
        /// </summary>
        public double? AmountDiscountTotal { get; set; }


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

        public ICollection<QuotationPromotion> Promotions { get; set; } = new List<QuotationPromotion>();

        public ICollection<QuotationPromotionLine> PromotionLines { get; set; } = new List<QuotationPromotionLine>();

        /// <summary>
        /// Phiếu báo giá
        /// </summary>
        public Guid QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Phụ tá
        /// </summary>
        public Guid? AssistantId { get; set; }
        public Employee Assistant { get; set; }

        /// <summary>
        /// người tư vấn
        /// </summary>
        public Guid? CounselorId { get; set; }
        public Employee Counselor { get; set; }

        /// <summary>
        /// Tư vấn
        /// </summary>
        public Guid? AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }

        /// <summary>
        /// Loại răng: nguyên hàm, hàm trên hàm dưới
        /// </summary>
        public string ToothType { get; set; }
    }
}
