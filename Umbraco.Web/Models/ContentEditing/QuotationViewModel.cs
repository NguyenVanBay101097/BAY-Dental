using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class QuotationBasic
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Mã báo giá
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public PartnerSimple Partner { get; set; }
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public EmployeeSimple Employee { get; set; }
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tổng tiền báo giá
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// trạng thái
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Danh sách thanh toán
        /// </summary>
        public IEnumerable<PaymentQuotationDisplay> Payments { get; set; } = new List<PaymentQuotationDisplay>();
        public IEnumerable<SaleOrderSimple> Orders { get; set; } = new List<SaleOrderSimple>();
    }

    public class QuotationDisplay
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Mã báo giá
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public PartnerSimple Partner { get; set; }
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public EmployeeSimple Employee { get; set; }
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tổng tiền báo giá
        /// </summary>
        public decimal? TotalAmount { get; set; }

        public Guid CompanyId { get; set; }

        public IEnumerable<SaleOrderSimple> Orders { get; set; } = new List<SaleOrderSimple>();

        public IEnumerable<QuotationLineDisplay> Lines { get; set; } = new List<QuotationLineDisplay>();
        public IEnumerable<PaymentQuotationDisplay> Payments { get; set; } = new List<PaymentQuotationDisplay>();

        public IEnumerable<QuotationPromotionBasic> Promotions { get; set; } = new List<QuotationPromotionBasic>();

        public decimal TotalAmountDiscount 
        {
            get
            {
                return Lines.Sum(x => (x.AmountDiscountTotal ?? 0) * x.Qty);
            }
        }

    }

    public class QuotationSave
    {

        public Guid Id { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public Guid CompanyId { get; set; }


        public IEnumerable<QuotationLineSave> Lines { get; set; } = new List<QuotationLineSave>();

        public IEnumerable<PaymentQuotationSave> Payments { get; set; } = new List<PaymentQuotationSave>();



    }

    public class QuotationPaged
    {
        public QuotationPaged()
        {
            Limit = 1;
            Offset = 0;
        }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public string Search { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public Guid? PartnerId { get; set; }

    }

    public class QuotationSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class QuotationPrintVM
    {
        public CompanyPrintVM Company { get; set; }
        public PartnerDisplay Partner { get; set; }
        public string Note { get; set; }
        public DateTime DateQuotation { get; set; }
        public EmployeeSimple Employee { get; set; }
        public int DateApplies { get; set; }
        public string Name { get; set; }
        public DateTime? DateEndQuotation { get; set; }
        public IEnumerable<QuotationLineDisplay> Lines { get; set; } = new List<QuotationLineDisplay>();
        public decimal? TotalAmount { get; set; }
        public IEnumerable<PaymentQuotationDisplay> Payments { get; set; } = new List<PaymentQuotationDisplay>();
    }
}


