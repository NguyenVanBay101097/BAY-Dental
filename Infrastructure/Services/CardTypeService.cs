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

        public async Task<ProductPricelistItem> AddProductPricelistItem(AddProductPricelistItem val)
        {
            var self = await GetByIdAsync(val.Id);
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var exist = await priceListItemObj.SearchQuery(x => x.PriceListId == self.PricelistId && x.ProductId == val.ProductId).AnyAsync();
            if (exist)
                throw new Exception("Dịch vụ đã được thêm vào danh sách ưu đãi");

            var item = new ProductPricelistItem()
            {
                AppliedOn = "0_product_variant",
                ComputePrice = "percentage",
                PercentPrice = 0,
                ProductId = val.ProductId,
                PriceListId = self.PricelistId
            };

            await priceListItemObj.CreateAsync(item);
            return item;
        }

        public async Task UpdateProductPricelistItem(UpdateProductPricelistItem val)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var priceListItem = await priceListItemObj.SearchQuery(x => x.Id == val.Id)
                .Include(x => x.Product)
                .FirstOrDefaultAsync();

            priceListItem.ComputePrice = val.ComputePrice;
            priceListItem.PercentPrice = val.PercentPrice;
            priceListItem.FixedAmountPrice = val.FixedAmountPrice;

            //priceListItemObj.ValidateBase(new List<ProductPricelistItem>() { priceListItem });
            await priceListItemObj.UpdateAsync(priceListItem);
        }

        public async Task ApplyServiceCategories(ApplyServiceCategoryReq val)
        {
            var self = await GetByIdAsync(val.Id);

            var priceListItemObj = GetService<IProductPricelistItemService>();
            var priceListItems = await priceListItemObj.SearchQuery(x => x.PriceListId == self.PricelistId).ToListAsync();

            var categIds = val.ServiceCategoryApplyDetails.Select(x => x.CategId).ToList();

            var productObj = GetService<IProductService>();
            var services = await productObj.SearchQuery(x => x.Active == true && x.Type2 == "service" && categIds.Contains(x.CategId.Value)).ToListAsync();

            var toAdds = new List<ProductPricelistItem>();
            var toUpdates = new List<ProductPricelistItem>();

            foreach (var service in services)
            {
                var cateVal = val.ServiceCategoryApplyDetails.FirstOrDefault(x => x.CategId == service.CategId);
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

            //priceListItemObj.ValidateBase(toAdds.Concat(toUpdates));

            await priceListItemObj.CreateAsync(toAdds);
            await priceListItemObj.UpdateAsync(toUpdates);
        }

        public async Task ApplyAllServices(ApplyAllServiceReq val)
        {
            var self = await GetByIdAsync(val.Id);

            var priceListItemObj = GetService<IProductPricelistItemService>();
            var priceListItems = await priceListItemObj.SearchQuery(x => x.PriceListId == self.PricelistId)
                .Include(x => x.Product)
                .ToListAsync();

            var productObj = GetService<IProductService>();
            var services = await productObj.SearchQuery(x => x.Active == true && x.Type2 == "service").ToListAsync();

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

            //priceListItemObj.ValidateBase(toAdds.Concat(toUpdates));

            await priceListItemObj.CreateAsync(toAdds);
            await priceListItemObj.UpdateAsync(toUpdates);
        }
    }
}
