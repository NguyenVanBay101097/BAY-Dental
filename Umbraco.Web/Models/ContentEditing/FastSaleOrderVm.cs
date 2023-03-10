using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FastSaleOrderVm
    {
        public FastSaleOrderVm()
        {
            IsQuotation = false;
        }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }


        public Guid CompanyId { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Quotation
        /// sale: Sales Order
        /// done: Locked
        /// cancel: Cancelled
        /// </summary>
        public string State { get; set; }

        public IEnumerable<SaleOrderLineSave> OrderLines { get; set; } = new List<SaleOrderLineSave>();

        public decimal? AmountTotal { get; set; }
        public Guid? PricelistId { get; set; }

        public string Note { get; set; }

        public bool? IsQuotation { get; set; }

    }

    public class FastSaleOrderSave
    {
        public FastSaleOrderSave()
        {
            IsQuotation = false;
            IsFast = true;
        }

        public DateTime DateOrder { get; set; }

        public Guid PartnerId { get; set; }
        public Guid? EmployeeId { get; set; }

        public string Note { get; set; }

        public IEnumerable<SaleOrderLineSave> OrderLines { get; set; } = new List<SaleOrderLineSave>();

        /// <summary>
        /// danh sách chương trình ưu đãi
        /// </summary>
        public ICollection<SaleOrderPromotionSave> Promotions { get; set; } = new List<SaleOrderPromotionSave>();

        public Guid CompanyId { get; set; }

        public string UserId { get; set; }

        public Guid? PricelistId { get; set; }

        public Guid? QuotationId { get; set; }

        public bool? IsQuotation { get; set; }
        public bool IsFast { get; set; }
    }
}
