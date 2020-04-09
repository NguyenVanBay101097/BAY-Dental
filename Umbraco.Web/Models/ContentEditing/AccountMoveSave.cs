using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountMoveSave
    {
        public DateTime? Date { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string InvoiceUserId { get; set; }

        public Guid? JournalId { get; set; }

        public IEnumerable<AccountMoveLineSave> InvoiceLines { get; set; }

        public IEnumerable<AccountMoveLineSave> Lines { get; set; }
    }
}
