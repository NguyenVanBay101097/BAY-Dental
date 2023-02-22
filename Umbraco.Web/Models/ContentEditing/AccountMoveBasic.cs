using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountMoveBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string PartnerName { get; set; }

        public string Type { get; set; }

        public string InvoiceOrigin { get; set; }

        public decimal? AmountTotal { get; set; }

        public decimal? AmountTotalSigned { get; set; }

        public decimal? AmountResidual { get; set; }

        public decimal? AmountResidualSigned { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string InvoiceUserName { get; set; }
    }
}
