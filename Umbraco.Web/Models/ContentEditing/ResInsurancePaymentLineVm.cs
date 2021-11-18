using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResInsurancePaymentLineSave
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }

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

    public class ResInsurancePaymentLineDisplay
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLineSimple SaleOrderLine { get; set; }

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

    public class ResInsurancePaymentLineRegisterDisplay
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public RegisterSaleOrderLinePayment SaleOrderLine { get; set; }

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
