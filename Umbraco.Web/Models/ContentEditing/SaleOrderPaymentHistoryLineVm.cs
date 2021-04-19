using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPaymentHistoryLineBasic
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLineBasic SaleOrderLine { get; set; }

        public decimal Amount { get; set; }
    }

    public class SaleOrderPaymentHistoryLineDisplay
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLineDisplay SaleOrderLine { get; set; }

        public decimal Amount { get; set; }
    }

    public class RegisterSaleOrderPaymentHistoryLine
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public RegisterSaleOrderLinePayment SaleOrderLine { get; set; }

        public decimal Amount { get; set; }
    }

    public class SaleOrderPaymentHistoryLineSave
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }

        public decimal Amount { get; set; }
    }
}
