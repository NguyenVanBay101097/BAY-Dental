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
        public DateTime Date { get; set; }

        /// <summary>
        /// So tien
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// phuong thuc thanh toan
        /// </summary>
        public IEnumerable<SaleOrderPaymentPublicJournals> Journals { get; set; } = new List<SaleOrderPaymentPublicJournals>();

        public IEnumerable<SaleOrderPaymentPublicLines> Lines { get; set; } = new List<SaleOrderPaymentPublicLines>();

        public string Note { get; set; }

        public string State { get; set; }
    }

    public class SaleOrderPaymentPublicJournals
    {
        public string JournalName { get; set; }

        public decimal Amount { get; set; }
    }

    public class SaleOrderPaymentPublicLines
    {
        public string ProductName { get; set; }

        public decimal Amount { get; set; }
    }
}
