using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPaymentPublicFilter
    {
        public Guid SaleOrderId { get; set; }
    }

    public class SaleOrderPaymentPublic
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// So tien
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// phuong thuc thanh toan
        /// </summary>
        public IEnumerable<SaleOrderPaymentJournalLineSimple> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineSimple>();

        public IEnumerable<SaleOrderLineSimple> Lines { get; set; } = new List<SaleOrderLineSimple>();

        public string Note { get; set; }

        /// <summary>
        /// draft : nháp
        /// posted : đã thanh toán
        /// cancel : hủy
        /// </summary>
        public string State { get; set; }
    }
}
