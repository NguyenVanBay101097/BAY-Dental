using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ProductPricelistItemService : BaseService<ProductPricelistItem>, IProductPricelistItemService
    {
        public ProductPricelistItemService(IAsyncRepository<ProductPricelistItem> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public override ISpecification<ProductPricelistItem> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "product.product_pricelist_item_comp_rule":
                    return new InitialSpecification<ProductPricelistItem>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }

}
