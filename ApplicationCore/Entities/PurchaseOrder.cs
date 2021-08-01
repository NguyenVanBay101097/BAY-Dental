using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PurchaseOrder: BaseEntity
    {
        public PurchaseOrder()
        {
            Name = "/";
            State = "draft";
            DateOrder = DateTime.Now;
            Type = "order";
        }
        public string Name { get; set; }


        public string PartnerRef { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// ngày mua/trả hàng
        /// </summary>
        public DateTime DateOrder { get; set; }

        public DateTime? DateApprove { get; set; }

        public Guid PickingTypeId { get; set; }
        public StockPickingType PickingType { get; set; }

        /// <summary>
        /// phiếu nhập/xuất kho
        /// </summary>
        public Guid? PickingId { get; set; }
        public StockPicking Picking { get; set; }

        /// <summary>
        /// tiền thanh toán chỉ áp dụng lần thanh toán đầu
        /// </summary>
        public decimal? AmountPayment { get; set; }

        /// <summary>
        /// phương thức thanh toán chỉ áp dụng lần thanh toán đầu
        /// </summary>
        public Guid? JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? AmountTotal { get; set; }

        /// <summary>
        /// tiền còn nợ
        /// </summary>
        public decimal? AmountResidual { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTax { get; set; }

        public string Origin { get; set; }

        public ICollection<PurchaseOrderLine> OrderLines { get; set; } = new List<PurchaseOrderLine>();

        public DateTime? DatePlanned { get; set; }

        public string Notes { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string InvoiceStatus { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Dung de phan loai: don mua hang hay don tra hang
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// draft : nháp
        /// purchase : đơn hàng
        /// done : hoàn thành
        /// </summary>
        public string State { get; set; }

        public Guid? RefundOrderId { get; set; }
        public PurchaseOrder RefundOrder { get; set; }
    }
}
