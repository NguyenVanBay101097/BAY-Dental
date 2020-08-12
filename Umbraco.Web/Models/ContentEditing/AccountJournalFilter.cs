using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountJournalFilter
    {
        public AccountJournalFilter()
        {
            Offset = 0;
            Limit = 20;
        }
        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public string Type { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
