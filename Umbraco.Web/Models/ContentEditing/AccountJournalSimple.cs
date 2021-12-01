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

        public string BankName { get; set; }

        public string BankBic { get; set; }

        public string AccountHolderName { get; set; }

        /// <summary>
        /// chi nhánh ngân hàng
        /// </summary>
        public string Branch { get; set; }

        public string DisplayName
        {
            get
            {
                var name = Name;
                if (!string.IsNullOrEmpty(BankBic))
                    name += " - " + BankBic;
                return name;
            }
        }

        public string SubDisplayName
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(BankBic))
                {
                    var name = BankBic;
                    if (!string.IsNullOrEmpty(Branch))
                        name += $" ({Branch})";
                    list.Add(name);
                }
                  
                if (!string.IsNullOrEmpty(AccountHolderName))
                    list.Add(AccountHolderName);
              
                return string.Join(" - ", list);
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
