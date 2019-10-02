using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountInvoiceReport
    {
        public string number { get; set; }

        public DateTime? date { get; set; }

        public Guid? product_id { get; set; }
        public Product Product { get; set; }

        public decimal product_qty { get; set; }

        public Guid? categ_id { get; set; }
        public ProductCategory Categ { get; set; }

        public Guid? journal_id { get; set; }
        public AccountJournal Journal { get; set; }

        public Guid? partner_id { get; set; }
        public Partner Partner { get; set; }

        public Guid? company_id { get; set; }
        public Company Company { get; set; }

        public string user_id { get; set; }
        public ApplicationUser User { get; set; }

        public decimal price_total { get; set; }

        public decimal discount_amount { get; set; }

        public Guid? invoice_id { get; set; }
        public AccountInvoice Invoice { get; set; }

        public string type { get; set; }

        public string state { get; set; }

        public Guid? account_id { get; set; }
        public AccountAccount Account { get; set; }

        public Guid? account_line_id { get; set; }
        public AccountAccount AccountLine { get; set; }

        public decimal residual { get; set; }

        public decimal amount_total { get; set; }
    }
}
