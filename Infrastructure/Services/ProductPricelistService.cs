using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductPricelistService : BaseService<ProductPricelist>, IProductPricelistService
    {
        public ProductPricelistService(IAsyncRepository<ProductPricelist> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<PagedResult2<ProductPricelistBasic>> GetPagedResultAsync(ProductPricelistPaged val)
        {
            ISpecification<ProductPricelist> spec = new InitialSpecification<ProductPricelist>(x => x.Active);
            if (!string.IsNullOrWhiteSpace(val.Search))
                spec = spec.And(new InitialSpecification<ProductPricelist>(x => x.Name.Contains(val.Search)));
            var items = await SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Sequence).ThenByDescending(s => s.DateCreated),
                limit: val.Limit, offSet: val.Offset)
                .Select(x => new ProductPricelistBasic
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            var totalItems = await CountAsync(spec);
            return new PagedResult2<ProductPricelistBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<ProductPricelist> GetPriceListForUpdate(Guid id)
        {
            return await SearchQuery(x => x.Id == id && x.Active)
                .Include(x => x.Items)
                .Include("Items.Product")
                .Include("Items.Categ")
                .Include("Items.PartnerCateg")
                .FirstOrDefaultAsync();
        }

        public async Task<IDictionary<Guid, ComputePriceRuleResValue>> _ComputePriceRule(IEnumerable<ProductQtyByPartner> products_qty_partner,
             DateTime? date = null)
        {
            if (!date.HasValue)
                date = DateTime.Now;
            var products = products_qty_partner.Select(x => x.Product);
            if (!products.Any())
                return new Dictionary<Guid, ComputePriceRuleResValue>();

            var categObj = GetService<IProductCategoryService>();
            var categ_ids = new HashSet<Guid>();
            foreach (var p in products)
            {
                var categ = p.Categ;
                while (categ != null)
                {
                    categ_ids.Add(categ.Id);
                    if (categ.ParentId.HasValue && categ.Parent == null)
                        categ.Parent = await categObj.SearchQuery(x => x.Id == categ.ParentId).Include(x => x.Parent).FirstOrDefaultAsync();
                    categ = categ.Parent;
                }
            }

            var prod_ids = products.Select(x => x.Id).ToList();

            var items = await _ComputePriceRuleGetItems(date.Value, prod_ids, categ_ids);
            var results = new Dictionary<Guid, ComputePriceRuleResValue>();
            foreach (var productByQtyByPartner in products_qty_partner)
            {
                var product = productByQtyByPartner.Product;
                var qty = productByQtyByPartner.Quantity;
                var partnerId = productByQtyByPartner.PartnerId;

                var qty_in_product_uom = qty;
                results.Add(product.Id, new ComputePriceRuleResValue());
                ProductPricelistItem suitable_rule = null;
                var price = product.ListPrice;
                foreach (var rule in items)
                {
                    if (rule.MinQuantity != 0 && qty_in_product_uom < rule.MinQuantity)
                        continue;
                    if (rule.ProductId.HasValue && product.Id != rule.ProductId.Value)
                        continue;
                    
                    if (partnerId.HasValue)
                    {
                        var partnerObj = GetService<IPartnerService>();
                        var partnerById = await partnerObj.GetPartnerForDisplayAsync(partnerId ?? Guid.Empty);

                        if (rule.PartnerCategId.HasValue && 
                            !partnerById.PartnerPartnerCategoryRels.Any(x => x.CategoryId== rule.PartnerCategId && x.PartnerId == partnerId))
                            continue;
                    }
                    

                    if (rule.Categ != null)
                    {
                        var cat = product.Categ;
                        while (cat != null)
                        {
                            if (cat.Id == rule.Categ.Id)
                                break;
                            cat = cat.Parent;
                        }

                        if (cat == null)
                            continue;
                    }

                    price = product.ListPrice;

                    if (price != 0)
                    {
                        if (rule.ComputePrice == "fixed")
                        {
                            price = rule.FixedPrice ?? 0;
                        }
                        else if (rule.ComputePrice == "percentage")
                        {
                            price = price - (price * (rule.PercentPrice ?? 0) / 100);
                        }

                        suitable_rule = rule;
                    }

                    break;
                }


                if (suitable_rule != null && suitable_rule.ComputePrice != "fixed" && suitable_rule.Base != "pricelist")
                    price = Math.Round(price);

                results[product.Id].Price = price;
                results[product.Id].SuitableRule = suitable_rule;
            }

            return results;
        }

        public async Task<IList<ProductPricelistItem>> _ComputePriceRuleGetItems(DateTime date, IEnumerable<Guid> productIds, IEnumerable<Guid> categIds)
        {
            var itemObj = GetService<IProductPricelistItemService>();
            var items = await itemObj.SearchQuery(x =>
                (!x.ProductId.HasValue || productIds.Contains(x.ProductId.Value)) &&
                (!x.CategId.HasValue || categIds.Contains(x.CategId.Value)) &&
                (!x.DateStart.HasValue || x.DateStart <= date) &&
                (!x.DateEnd.HasValue || x.DateEnd >= date)
                , orderBy: x => x.OrderBy(s => s.AppliedOn).ThenByDescending(s => s.MinQuantity).ThenByDescending(s => s.Categ.CompleteName).ThenByDescending(s => s.DateCreated)).ToListAsync();
            return items;
        }

        public override ISpecification<ProductPricelist> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "product.product_pricelist_comp_rule":
                    return new InitialSpecification<ProductPricelist>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }

    public class ProductQtyByPartner
    {
        public Product Product { get; set; }
        public Guid? PartnerId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class ComputePriceRuleResValue
    {
        public decimal Price { get; set; }

        public ProductPricelistItem SuitableRule { get; set; }
    }
}
