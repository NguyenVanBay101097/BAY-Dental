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
                    return new InitialSpecification<ProductPricelistItem>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }
        private decimal GetComputePrice(ProductPricelistItem self)
        {
            switch (self.ComputePrice)
            {
                case "percentage":
                    return Math.Round(((self.PercentPrice ?? 0) * self.Product.ListPrice) / 100);
                case "fixed_amount":
                    return self.FixedAmountPrice ?? 0;
                default:
                    return 0;
            }
        }
        public void ValidateBase(IEnumerable<ProductPricelistItem> selfs)
        {
            foreach (var self in selfs)
            {
                if (GetComputePrice(self) > self.Product.ListPrice)
                    throw new Exception("Ưu đãi vượt quá giá bán");
            }
        }
    }

}
