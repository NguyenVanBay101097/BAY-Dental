using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountJournalViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Type { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
