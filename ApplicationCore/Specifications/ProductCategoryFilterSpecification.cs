using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class ProductCategoryFilterSpecification : BaseSpecification<ProductCategory>
    {
        public ProductCategoryFilterSpecification(string filter = "", int skip = 0, int take = 20,
            bool isPagingEnabled = false, Expression<Func<ProductCategory, object>> orderBy = null, string orderDirection = "asc")
            : base(i => i.Name.Contains(filter))
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
