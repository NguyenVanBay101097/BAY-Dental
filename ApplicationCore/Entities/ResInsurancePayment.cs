using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResInsurancePayment : BaseEntity
    {
        public ResInsurancePayment()
        {
            State = "draft";
        }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// Phiếu điều trị
        /// </summary>
        public Guid? OrderId { get; set; }
        public SaleOrder Order { get; set; }

        /// <summary>
        /// bảo hiểm
        /// </summary>
        public Guid? ResInsuranceId { get; set; }
        public ResInsurance ResInsurance { get; set; }

        /// <summary>
        /// thanh toán phiếu điều trị
        /// </summary>
        public Guid? SaleOrderPaymentId { get; set; }
        public SaleOrderPayment SaleOrderPayment { get; set; }

        public ICollection<ResInsurancePaymentLine> Lines { get; set; } = new List<ResInsurancePaymentLine>();

        /// <summary>
        /// Invoice
        /// </summary>
        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// draft : nháp
        /// posted : đã thanh toán
        /// </summary>
        public string State { get; set; }
    }
}
