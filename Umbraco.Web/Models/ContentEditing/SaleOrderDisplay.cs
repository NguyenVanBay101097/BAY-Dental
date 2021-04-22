using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderDisplay
    {
        public SaleOrderDisplay()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            Name = "/";
            AmountTotal = 0;
        }
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public decimal? AmountTax { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTotal { get; set; }
        public decimal? PaidTotal
        {
            get
            {
                return (this.AmountTotal ?? 0) - (this.Residual ?? 0);
            }
            set { }
        }

        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string State { get; set; }

        public string Name { get; set; }

        public IEnumerable<SaleOrderLineDisplay> OrderLines { get; set; } = new List<SaleOrderLineDisplay>();



        public Guid CompanyId { get; set; }

        /// <summary>
        /// Nhân viên, bác sĩ điều trị
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public decimal? Residual { get; set; }
        public decimal? Paid { get; set; }

        public Guid? PricelistId { get; set; }
        public ProductPricelistBasic Pricelist { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public bool? IsQuotation { get; set; }
        public bool? IsFast { get; set; }

        public SaleOrderBasic Quote { get; set; }

        public SaleOrderBasic Order { get; set; }

        public string InvoiceStatus { get; set; }

        public int InvoiceCount { get; set; }
    }

    public class SaleOrderDisplayVm
    {
        public SaleOrderDisplayVm()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            Name = "/";
            AmountTotal = 0;
        }
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerDisplay Partner { get; set; }

        public decimal? AmountTax { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTotal { get; set; }
        public decimal? PaidTotal
        {
            get
            {
                return (this.AmountTotal ?? 0) - (this.Residual ?? 0);
            }
            set { }
        }

        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string State { get; set; }

        public string Name { get; set; }

        public IEnumerable<SaleOrderLineDisplay> OrderLines { get; set; } = new List<SaleOrderLineDisplay>();

        public IEnumerable<DotKhamDisplayVm> DotKhams { get; set; } = new List<DotKhamDisplayVm>();

        public IEnumerable<SaleOrderPromotionBasic> Promotions { get; set; } = new List<SaleOrderPromotionBasic>();

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Nhân viên, bác sĩ điều trị
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public decimal? Residual { get; set; }
        public decimal? Paid { get; set; }

        public Guid? PricelistId { get; set; }
        public ProductPricelistBasic Pricelist { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public bool? IsQuotation { get; set; }
        public bool? IsFast { get; set; }

        public SaleOrderBasic Quote { get; set; }

        public SaleOrderBasic Order { get; set; }

        public string InvoiceStatus { get; set; }

        public int InvoiceCount { get; set; }
    }
}
