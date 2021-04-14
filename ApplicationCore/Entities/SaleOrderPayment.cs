using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderPayment : BaseEntity
    {
        public SaleOrderPayment()
        {
            State = "draft";
        }

        public string Name { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public ICollection<SaleOrderPaymentJournalLine> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLine>();

        public ICollection<SaleOrderPaymentHistoryLine> Lines { get; set; } = new List<SaleOrderPaymentHistoryLine>();

        /// <summary>
        /// Hóa đơn doanh thu , công nợ
        /// </summary>
        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }


        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Phiếu điều trị
        /// </summary>
        public Guid OrderId { get; set; }
        public SaleOrder Order { get; set; }

        /// <summary>
        /// draft : nháp
        /// posted : đã thanh toán
        /// cancel : hủy
        /// </summary>
        public string State { get; set; }

        public ICollection<AccountMoveLine> MoveLines { get; set; } = new List<AccountMoveLine>();

    }
}
