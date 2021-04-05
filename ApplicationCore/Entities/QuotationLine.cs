﻿using System;
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
        public int Discount { get; set; }

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
        /// Bác sĩ
        /// </summary>
        public Employee Doctor { get; set; }
        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Phụ tá
        /// </summary>
        public Employee Assistant { get; set; }
        public Guid? AssistantId { get; set; }

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

        public Guid QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        public Guid? AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }
    }
}
