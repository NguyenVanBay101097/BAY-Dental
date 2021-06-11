using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.SaleOrders
{
    public class CreateSaleOrderRequest
    {
        public CreateSaleOrderRequest()
        {
            IsQuotation = false;
        }

        public Guid Id { get; set; }

        public DateTime DateOrder { get; set; }

        public Guid PartnerId { get; set; }
        public Guid? EmployeeId { get; set; }

        public string Note { get; set; }

        public IEnumerable<CreateSaleOrderRequestLine> OrderLines { get; set; } = new List<CreateSaleOrderRequestLine>();

        public Guid CompanyId { get; set; }

        public string UserId { get; set; }

        public Guid? PricelistId { get; set; }
        public Guid? JournalId { get; set; }

        public Guid? QuotationId { get; set; }

        public bool? IsQuotation { get; set; }
        public bool IsFast { get; set; }
    }

    public class CreateSaleOrderRequestLine
    {
        public Guid Id { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string Name { get; set; }

        public decimal Discount { get; set; }

        public Guid? ProductId { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public Guid? ToothCategoryId { get; set; }

        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();

        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manually :  thủ công
        /// </summary>
        public string ToothType { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? AssistantId { get; set; }

        public Guid? CounselorId { get; set; }

        public bool IsActive { get; set; }
    }
}
