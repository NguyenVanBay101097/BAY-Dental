using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResPartnerBankBasic
    {
        public Guid Id { get; set; }

        public string AccountNumber { get; set; }

        public Guid BankId { get; set; }
        public ResBankSimple Bank { get; set; }
    }

    public class ResPartnerBankDisplay
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }

        public Guid BankId { get; set; }
        public ResBankSimple Bank { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }
    }

    public class ResPartnerBankPaged
    {
        public ResPartnerBankPaged()
        {
            Offset = 0;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid? PartnerId { get; set; }

        public Guid? BankId { get; set; }
    }
}
