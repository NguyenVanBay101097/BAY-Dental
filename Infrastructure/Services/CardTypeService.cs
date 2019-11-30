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

            var items = await query.Select(x => new CardTypeBasic
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            var totalItems = await query.CountAsync();
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
                CardTypeId = cardType.Id,
            };
            pricelist.Items.Add(new ProductPricelistItem
            {
                AppliedOn = "3_global",
                ComputePrice = "percentage",
                PercentPrice = cardType.Discount,
                CardTypeId = cardType.Id,
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
    }
}
