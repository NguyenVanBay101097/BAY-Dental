using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class ProductFilterSpecification : BaseSpecification<Product>
    {
        public ProductFilterSpecification(string filter = "", int skip = 0, int take = 20,
            bool isPagingEnabled = false, Expression<Func<Product, object>> orderBy = null, string orderDirection = "asc")
            : base(i => i.Name.Contains(filter))
        {
            if (orderBy != null)
            {
                if (orderDirection == "desc")
                    ApplyOrderByDescending(orderBy);
                else
                    ApplyOrderBy(orderBy);
            }

            AddInclude(x => x.Categ);
            AddInclude(x => x.UOM);

            if (isPagingEnabled)
                ApplyPaging(skip, take);
        }
    }
}
