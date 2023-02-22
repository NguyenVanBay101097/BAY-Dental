using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountJournalSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public string DisplayType
        {
            get
            {
                switch(Type)
                {
                    case "bank":
                        return "Ngân hàng";
                    case "cash":
                        return "Tiền mặt";
                    default:
                        return string.Empty;
                }
            }
        }
    }

    public class AccountJournalResBankSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ResPartnerBankBasic BankAccount { get;set;}
    }
}
