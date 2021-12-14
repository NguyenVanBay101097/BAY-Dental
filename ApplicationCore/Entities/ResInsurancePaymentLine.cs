using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResInsurancePaymentLine : BaseEntity
    {
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        public Guid ResInsurancePaymentId { get; set; }
        public ResInsurancePayment ResInsurancePayment { get; set; }

        /// <summary>
        /// kiểu chi trả
        /// percent : giảm phần trăm
        /// fixed : giảm tiền
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// phần trăm
        /// </summary>
        public decimal? Percent { get; set; }

        /// <summary>
        /// tiền cố định
        /// </summary>
        public decimal? FixedAmount { get; set; }
    }
}
