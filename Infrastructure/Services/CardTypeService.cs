using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
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
    public class CardTypeService : BaseService<CardType>, ICardTypeService
    {
        private readonly IMapper _mapper;
        public CardTypeService(IAsyncRepository<CardType> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<CardTypeBasic>> GetPagedResultAsync(CardTypePaged val)
        {
            ISpecification<CardType> spec = new InitialSpecification<CardType>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<CardType>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name));
            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.OrderByDescending(x => x.DateCreated).Select(x => new CardTypeBasic
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            return new PagedResult2<CardTypeBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<CardType> CreateCardType(CardTypeDisplay val)
        {
            var cardType = _mapper.Map<CardType>(val);
            await CreateAsync(cardType);
            var pricelist = await CreatePricelist(cardType);
            cardType.PricelistId = pricelist.Id;
            await UpdateAsync(cardType);
            return cardType;
        }

        public async Task UpdateCardType(Guid id, CardTypeDisplay val)
        {
            var cardType = await SearchQuery(x => x.Id == id).Include(x => x.Pricelist)
                .Include("Pricelist.Items").FirstOrDefaultAsync();
            cardType = _mapper.Map(val, cardType);
            var item = cardType.Pricelist.Items.FirstOrDefault(x => x.AppliedOn == "3_global" && x.ComputePrice == "percentage");
            if (item != null)
                item.PercentPrice = val.Discount;
            await UpdateAsync(cardType);
        }

        private async Task<ProductPricelist> CreatePricelist(CardType cardType)
        {
            var pricelistObj = GetService<IProductPricelistService>();
            var pricelist = new ProductPricelist()
            {
                Name = $"Bảng giá loại thẻ {cardType.Name}",
            };
            pricelist.Items.Add(new ProductPricelistItem
            {
                AppliedOn = "3_global",
                ComputePrice = "percentage",
                PercentPrice = cardType.Discount,
            });
            return await pricelistObj.CreateAsync(pricelist);
        }

        public DateTime GetPeriodEndDate(CardType self, DateTime? dStart = null)
        {
            if (dStart == null)
                dStart = DateTime.Today;
            var nb = self.NbPeriod;
            if (self.Period == "year")
                nb = nb * 12;
            return dStart.Value.AddMonths(nb);
        }

        public override async Task UpdateAsync(IEnumerable<CardType> entities)
        {
            await base.UpdateAsync(entities);
            foreach (var entity in entities)
            {
                await _CheckNameUnique(entity.Name);
                await _CheckBasicPointUnique(entity.BasicPoint.Value);
            }
        }

        public async Task<IEnumerable<CardType>> GetAutoComplete(CardTypePaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            var items = await query.OrderByDescending(x => x.DateCreated).ToListAsync();

            return items;
        }

        private async Task _CheckNameUnique(string name)
        {
            var count = await SearchQuery(x => x.Name == name).CountAsync();
            if (count >= 2)
                throw new Exception($"Trùng tên hạng thẻ khác");
        }

        private async Task _CheckBasicPointUnique(decimal point)
        {
            var count = await SearchQuery(x => x.BasicPoint.Value == point).CountAsync();
            if (count >= 2)
                throw new Exception($"Trùng điểm của hạng thẻ khác");
        }

        public override async Task<IEnumerable<CardType>> CreateAsync(IEnumerable<CardType> entities)
        {
            await base.CreateAsync(entities);
            foreach (var entity in entities)
            {
                await _CheckNameUnique(entity.Name);
                await _CheckBasicPointUnique(entity.BasicPoint.Value);
            }
            return entities;
        }

        public override ISpecification<CardType> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "member_card.card_type_comp_rule":
                    return new InitialSpecification<CardType>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        private void ValidateApplyPriceListItem(CardType self)
        {
            if (self == null)
                throw new Exception("Không tìm thấy hạng thẻ");
            if (self.Pricelist == null)
                throw new Exception("Không tìm thấy bảng giá của hạng thẻ");
        }
        public async Task<IEnumerable<ProductPricelistItem>> AddProductPricelistItem(Guid id, IEnumerable<Guid> productIds)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var productObj = GetService<IProductService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.Pricelist.Items).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);

            var products = await productObj.SearchQuery(x => productIds.Contains(x.Id)).ToListAsync();
            var priceListItems = self.Pricelist.Items;
            var toAdds = new List<ProductPricelistItem>();
            foreach (var pro in products)
            {
                if (priceListItems.Any(x => x.ProductId == pro.Id))
                    continue;
                toAdds.Add(new ProductPricelistItem()
                {
                    AppliedOn = "0_product_variant",
                    ComputePrice = "percentage",
                    PercentPrice = 0,
                    ProductId = pro.Id,
                    PriceListId = self.PricelistId
                });
            }

            await priceListItemObj.CreateAsync(toAdds);

            return toAdds;
        }

        public async Task UpdateProductPricelistItem(Guid id, IEnumerable<ProductPricelistItemCreate> items)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.Pricelist.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);

            var priceListItems = self.Pricelist.Items;
            var toUpdates = new List<ProductPricelistItem>();
            foreach (var item in items)
            {
                var existItem = priceListItems.FirstOrDefault(x => x.Id == item.Id);
                if (item == null)
                    continue;
                existItem.ComputePrice = item.ComputePrice;
                existItem.PercentPrice = item.PercentPrice;
                existItem.FixedAmountPrice = item.FixedAmountPrice;

                toUpdates.Add(existItem);
            }
            priceListItemObj.ValidateBase(toUpdates);// validate before CU
            await priceListItemObj.UpdateAsync(toUpdates);
        }

        public async Task ApplyServiceCategories(Guid id, IEnumerable<ApplyServiceCategoryReq> vals)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var productObj = GetService<IProductService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.Pricelist.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);
            var priceListItems = self.Pricelist.Items;
            //get list service
            var categIds = vals.Where(x => x.CategId != Guid.Empty).Select(x => x.CategId);
            var services = await productObj.SearchQuery(x => x.Active == true && x.Type2 == "service" && categIds.Contains(x.CategId.Value)).ToListAsync();
            // add and update pricelistitem
            var toAdds = new List<ProductPricelistItem>();
            var toUpdates = new List<ProductPricelistItem>();
            foreach (var service in services)
            {
                var cateVal = vals.FirstOrDefault(x => x.CategId == service.CategId);
                var existItem = priceListItems.FirstOrDefault(x => x.ProductId == service.Id);

                if (existItem != null)
                {
                    existItem.ComputePrice = cateVal.ComputePrice;
                    existItem.PercentPrice = cateVal.PercentPrice;
                    existItem.FixedAmountPrice = cateVal.FixedAmountPrice;
                    toUpdates.Add(existItem);
                }
                else
                {
                    toAdds.Add(new ProductPricelistItem()
                    {
                        AppliedOn = "0_product_variant",
                        ComputePrice = cateVal.ComputePrice,
                        PercentPrice = cateVal.PercentPrice,
                        FixedAmountPrice = cateVal.FixedAmountPrice,
                        ProductId = service.Id,
                        Product = service,
                        PriceListId = self.PricelistId
                    });
                }
            }

            if (toAdds.Any())
            {
                priceListItemObj.ValidateBase(toAdds);// validate before CU
                await priceListItemObj.CreateAsync(toAdds);
            }
            if (toUpdates.Any())
            {
                priceListItemObj.ValidateBase(toUpdates);// validate before CU
                await priceListItemObj.UpdateAsync(toUpdates);
            }
        }

        public async Task ApplyAllServices(Guid id, ApplyAllServiceReq val)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var productObj = GetService<IProductService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.Pricelist.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);
            var priceListItems = self.Pricelist.Items;
            //get list service
            var services = await productObj.SearchQuery(x => x.Active == true && x.Type2 == "service").ToListAsync();
            // add and update pricelistitem
            var toAdds = new List<ProductPricelistItem>();
            var toUpdates = new List<ProductPricelistItem>();
            foreach (var service in services)
            {
                var existItem = priceListItems.FirstOrDefault(x => x.ProductId == service.Id);

                if (existItem != null)
                {
                    existItem.ComputePrice = val.ComputePrice;
                    existItem.PercentPrice = val.PercentPrice;
                    existItem.FixedAmountPrice = val.FixedAmountPrice;
                    toUpdates.Add(existItem);
                }
                else
                {
                    toAdds.Add(new ProductPricelistItem()
                    {
                        AppliedOn = "0_product_variant",
                        ComputePrice = val.ComputePrice,
                        PercentPrice = val.PercentPrice,
                        FixedAmountPrice = val.FixedAmountPrice,
                        ProductId = service.Id,
                        Product = service,
                        PriceListId = self.PricelistId
                    });
                }
            }

            if (toAdds.Any())
            {
                priceListItemObj.ValidateBase(toAdds);// validate before CU
                await priceListItemObj.CreateAsync(toAdds);
            }
            if (toUpdates.Any())
            {
                priceListItemObj.ValidateBase(toUpdates);// validate before CU
                await priceListItemObj.UpdateAsync(toUpdates);
            }
        }
    }
}
