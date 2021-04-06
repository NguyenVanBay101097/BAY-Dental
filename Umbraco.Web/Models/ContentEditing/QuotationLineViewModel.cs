using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class QuotationLineBasic
    {
        public string Name { get; set; }

        /// <summary>
        /// Dịch vụ
        /// </summary>
        public ProductSimple Product { get; set; }
        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng dịch vụ
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public int PercentDiscount { get; set; }

        /// <summary>
        /// Tổng tiền trên 1 dịch vụ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Tổng tiền trên 1 dịch vụ
        /// </summary>
        public decimal? SubPrice { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        /// <summary>
        /// Loại răng: răng sữa, răng vĩnh viễn !!!
        /// </summary>
        public ToothCategoryBasic ToothCategory { get; set; }
        public Guid ToothCategoryId { get; set; }

        public IEnumerable<ToothBasic> QuotationLineToothRels { get; set; } = new List<ToothBasic>();

        public Guid QuotationId { get; set; }
        public QuotationSimple Quotation { get; set; }
    }

    public class QuotationLineDisplay
    {
        public string Name { get; set; }

        /// <summary>
        /// Dịch vụ
        /// </summary>
        public ProductSimple Product { get; set; }
        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng dịch vụ
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Tổng tiền trên 1 dịch vụ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Tien cua 1 dich vu
        /// </summary>
        public decimal? SubPrice { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        /// <summary>
        /// Loại răng: răng sữa, răng vĩnh viễn !!!
        /// </summary>
        public ToothCategoryBasic ToothCategory { get; set; }
        public Guid ToothCategoryId { get; set; }

        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
    }

    public class QuotationLineSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class QuotationLineSave
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Dịch vụ
        /// </summary>

        /// Ten dich vu
        public string Name { get; set; }

        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng dịch vụ
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Đơn giá
        /// </summary>
        public decimal? SubPrice { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        /// <summary>
        /// Loại răng: răng sữa, răng vĩnh viễn !!!
        /// </summary>
        public Guid ToothCategoryId { get; set; }

        /// <summary>
        /// Người tư vấn
        /// </summary>
        public Guid AdvisoryId { get; set; }

        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();
    }
}
