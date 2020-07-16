using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountMoveLineBasic
    {
        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }
    }
}
