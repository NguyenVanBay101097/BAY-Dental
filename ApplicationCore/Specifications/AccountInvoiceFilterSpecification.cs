using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class AccountInvoiceFilterSpecification : BaseSpecification<AccountInvoice>
    {
        public AccountInvoiceFilterSpecification(string filter = "", int skip = 0, int take = 20,
            bool isPagingEnabled = false, Expression<Func<AccountInvoice, object>> orderBy = null, string orderDirection = "asc")
            : base(i => i.Name.Contains(filter))
        {
            if (orderBy != null)
            {
                if (orderDirection == "desc")
                    ApplyOrderByDescending(orderBy);
                else
                    ApplyOrderBy(orderBy);
            }

            AddInclude(x => x.Partner);
            AddInclude(x => x.User);

            if (isPagingEnabled)
                ApplyPaging(skip, take);
        }
    }
}
