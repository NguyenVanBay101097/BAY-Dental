using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class PartnerFilterSpecification : BaseSpecification<Partner>
    {
        public PartnerFilterSpecification(string filter = "", int skip = 0, int take = 20,
            bool isPagingEnabled = false, Expression<Func<Partner, object>> orderBy = null, string orderDirection = "asc", bool? customer = null)
            : base(i => i.Name.Contains(filter) && (!customer.HasValue || i.Customer == customer))
        {
            if (orderBy != null)
            {
                if (orderDirection == "desc")
                    ApplyOrderByDescending(orderBy);
                else
                    ApplyOrderBy(orderBy);
            }

            if (isPagingEnabled)
                ApplyPaging(skip, take);
        }
    }
}
