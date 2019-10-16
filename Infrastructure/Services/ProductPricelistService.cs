using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductPricelistService : IProductPricelistService
    {
        private readonly IProductPricelistRepository _pricelistRepository;
        private readonly IProductPricelistItemRepository _priceListItemRepository;

        public ProductPricelistService(IProductPricelistRepository pricelistRepository,
            IProductPricelistItemRepository priceListItemRepository)
        {
            _pricelistRepository = pricelistRepository;
            _priceListItemRepository = priceListItemRepository;
        }

        public async Task<ProductPricelist> CreateAsync(ProductPricelist pricelist)
        {
            return await _pricelistRepository.InsertAsync(pricelist);
        }

        public async Task<ProductPricelist> GetByIdAsync(Guid id)
        {
            return await _pricelistRepository.GetByIdAsync(id);
        }

        public async Task DeleteAsync(ProductPricelist pricelist)
        {
            await _pricelistRepository.DeleteAsync(pricelist);
        }

        public async Task<PagedResult2<ProductPricelistBasic>> GetPagedResultAsync(ProductPricelistPaged val)
        {
            ISpecification<ProductPricelist> spec = new InitialSpecification<ProductPricelist>(x => x.Active);
            if (!string.IsNullOrWhiteSpace(val.Search))
                spec = spec.And(new InitialSpecification<ProductPricelist>(x => x.Name.Contains(val.Search)));
            var items = await _pricelistRepository.SearchQuery(spec, sort: x => x.OrderBy(s => s.Sequence).ThenByDescending(s => s.DateCreated),
                limit: val.Limit, offset: val.Offset, isPagingEnabled: true)
                .Select(x => new ProductPricelistBasic
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            var totalItems = await _pricelistRepository.CountAsync(spec);
            return new PagedResult2<ProductPricelistBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<ProductPricelist> GetPriceListForUpdate(Guid id)
        {
            var spec = new InitialSpecification<ProductPricelist>(x => x.Id == id && x.Active);
            return await _pricelistRepository.FirstOrDefaultAsync(spec, includes: "Items");
        }

        public async Task UpdateAsync(ProductPricelist pricelist)
        {
            await _pricelistRepository.UpdateAsync(pricelist);
        }

        public async Task<IDictionary<Guid, ComputePriceRuleResValue>> _ComputePriceRule(IEnumerable<ProductQtyByPartner> products_qty_partner,
             DateTime? date = null)
        {
            if (!date.HasValue)
                date = DateTime.Today;
            var products = products_qty_partner.Select(x => x.Product);
            if (!products.Any())
                return new Dictionary<Guid, ComputePriceRuleResValue>();

            var categ_ids = new HashSet<Guid>();
            foreach (var p in products)
            {
                var categ = p.Categ;
                while (categ != null)
                {
                    categ_ids.Add(categ.Id);
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
            var itemIds = await _priceListItemRepository.SearchQuery(new InitialSpecification<ProductPricelistItem>(x =>
                (!x.ProductId.HasValue || productIds.Contains(x.ProductId.Value)) &&
                (!x.CategId.HasValue || categIds.Contains(x.CategId.Value)) &&
                (!x.DateStart.HasValue || x.DateStart <= date) &&
                (!x.DateEnd.HasValue || x.DateStart >= date)
                ), sort: x => x.OrderBy(s => s.AppliedOn).ThenByDescending(s => s.MinQuantity).ThenByDescending(s => s.Categ.CompleteName).ThenByDescending(s => s.DateCreated))
                .Select(x => x.Id).ToListAsync();
            var items = await _priceListItemRepository.SearchQuery(new InitialSpecification<ProductPricelistItem>(x => itemIds.Contains(x.Id))).ToListAsync();
            return items;
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
