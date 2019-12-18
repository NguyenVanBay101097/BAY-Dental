using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class CardCardService : BaseService<CardCard>, ICardCardService
    {
        private readonly IMapper _mapper;
        private static readonly Random _random = new Random();

        public CardCardService(IAsyncRepository<CardCard> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<decimal?> ConvertAmountToPoint(decimal? amount)
        {
            var prate = await GetLoyaltyPointExchangeRate();
            if (prate < 0 || !amount.HasValue)
                return 0;
            var points = (amount ?? 0) / prate;
            var res = FloatUtils.FloatRound((double)points, precisionRounding: 1);
            return (decimal)res;
        }

        public async Task<decimal> GetLoyaltyPointExchangeRate()
        {
            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("loyalty.point_exchange_rate");
            if (!string.IsNullOrEmpty(value))
                return Convert.ToDecimal(value);
            return 1;
        }

        public async Task<PagedResult2<CardCardBasic>> GetPagedResultAsync(CardCardPaged val)
        {
            ISpecification<CardCard> spec = new InitialSpecification<CardCard>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<CardCard>(x => x.Name.Contains(val.Search) ||
                x.Barcode.Contains(val.Search) || x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search)));
            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<CardCard>(x => x.PartnerId == val.PartnerId));
            if (!string.IsNullOrEmpty(val.Barcode))
                spec = spec.And(new InitialSpecification<CardCard>(x => x.Barcode == val.Barcode));
            if (!string.IsNullOrEmpty(val.State))
                spec = spec.And(new InitialSpecification<CardCard>(x => x.State == val.State));
            if (val.IsExpired.HasValue)
            {
                var tday = DateTime.Today;
                if (val.IsExpired.Value)
                    spec = spec.And(new InitialSpecification<CardCard>(x => x.ExpiredDate.HasValue && tday > x.ExpiredDate));
                else
                    spec = spec.And(new InitialSpecification<CardCard>(x => !x.ExpiredDate.HasValue || tday <= x.ExpiredDate));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new CardCardBasic
            {
                Id = x.Id,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                TypeName = x.Type.Name,
                TotalPoint = x.TotalPoint ?? 0,
                PointInPeriod = x.PointInPeriod ?? 0,
                Barcode = x.Barcode,
                State = x.State
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<CardCardBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override async Task<IEnumerable<CardCard>> CreateAsync(IEnumerable<CardCard> self)
        {
            var seqObj = GetService<IIRSequenceService>();
            foreach(var card in self)
            {
                if (string.IsNullOrEmpty(card.Name) || card.Name == "/")
                {
                    card.Name = await seqObj.NextByCode("sequence_seq_card_nb");
                    if (string.IsNullOrEmpty(card.Name))
                    {
                        await CreateCardSequence();
                        card.Name = await seqObj.NextByCode("sequence_seq_card_nb");
                    }
                }
                    
                if (string.IsNullOrEmpty(card.Barcode))
                    card.Barcode = RandomBarcode();
            }
            await base.CreateAsync(self);

            foreach (var card in self)
                await _CheckBarcodeUnique(card.Barcode);

            return self;
        }

        private async Task CreateCardSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Prefix = "LC",
                Padding = 6,
                NumberIncrement = 1,
                NumberNext = 1,
                Code = "sequence_seq_card_nb",
                Name = "Card Sequence"
            });
        }

        private string RandomBarcode()
        {
            var size = 13;
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                builder.Append(_random.Next(0, 9));
            }
            return builder.ToString();
        }

        private async Task _CheckBarcodeUnique(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return;

            var count = await SearchQuery(x => x.Barcode == barcode).CountAsync();
            if (count >= 2)
                throw new Exception($"Đã có thẻ thành viên với mã vạch {barcode}");
        }

        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id) && x.State == "draft").ToListAsync();
            foreach (var card in self)
                card.State = "confirmed";
            await UpdateAsync(self);
        }

        public async Task ButtonActive(IEnumerable<Guid> ids)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var self = await SearchQuery(x => ids.Contains(x.Id) && x.State == "draft" || x.State == "confirmed")
                .Include(x => x.Type).ToListAsync();
            foreach (var card in self)
            {
                if (!card.PartnerId.HasValue)
                    throw new Exception("Thẻ thành viên không được để trống khách hàng");
                await CheckExisted(card);
                if (!card.ActivatedDate.HasValue)
                    card.ActivatedDate = DateTime.Today;
                if (!card.ExpiredDate.HasValue)
                    card.ExpiredDate = cardTypeObj.GetPeriodEndDate(card.Type);
                card.State = "in_use";
                card.PointInPeriod = 0;
            }
              
            await UpdateAsync(self);
        }

        public async Task CheckExisted(CardCard self)
        {
            //Kiểm tra xem đã có thẻ nào đã được cấp cho khách hàng hay chưa? nếu có thì báo lỗi
            var states = new string[] { "draft", "confirmed", "canceled" };
            var existed = await SearchQuery(x => x.Id != self.Id && !states.Contains(x.State) && x.PartnerId == self.PartnerId).FirstOrDefaultAsync();
            if (existed != null)
                throw new Exception($"Thẻ thành viên với số {existed.Name} đã được cấp cho khách hàng này");
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach(var card in self)
            {
                if (card.State != "draft" && card.State != "cancel")
                    throw new Exception("Chỉ có thể xóa thẻ thành viên ở trạng thái nháp hoặc hủy bỏ");
            }
            await DeleteAsync(self);
        }

        public async Task ButtonCancel(IEnumerable<Guid> ids)
        {
            var states = new string[] { "draft", "confirmed" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).ToListAsync();
            foreach (var card in self)
                card.State = "cancelled";
            await UpdateAsync(self);
        }

        public async Task ButtonReset(IEnumerable<Guid> ids)
        {
            var states = new string[] { "cancelled" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).ToListAsync();
            foreach (var card in self)
                card.State = "draft";
            await UpdateAsync(self);
        }

        public async Task ButtonLock(IEnumerable<Guid> ids)
        {
            var states = new string[] { "in_use" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).ToListAsync();
            foreach (var card in self)
                card.State = "locked";
            await UpdateAsync(self);
        }

        public async Task ButtonUnlock(IEnumerable<Guid> ids)
        {
            var states = new string[] { "locked" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).ToListAsync();
            foreach (var card in self)
                card.State = "in_use";
            await UpdateAsync(self);
        }

        public async Task<CardCard> GetValidCard(Guid partnerId)
        {
            var card = await _GetCard(partnerId);
            if (card == null || IsExpired(card))
                return null;
            return card;
        }

        private async Task<CardCard> _GetCard(Guid partnerId, string state = "in_use")
        {
            return await SearchQuery(x => x.PartnerId == partnerId && x.State == state)
                .Include(x => x.Type)
                .FirstOrDefaultAsync();
        }

        public bool IsExpired(CardCard self)
        {
            var today = DateTime.Today;
            if (!self.ExpiredDate.HasValue)
                return false;
            return today > self.ExpiredDate;
        }

        public async Task<CardType> FindTypeUpgrade(CardCard self)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var points = self.PointInPeriod ?? 0;
            var type = await cardTypeObj.SearchQuery(x => x.BasicPoint <= points, orderBy: x => x.OrderByDescending(s => s.BasicPoint))
                .FirstOrDefaultAsync();
            return type;
        }

        public async Task ButtonUpgradeCard(IEnumerable<CardCard> self)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            foreach(var card in self)
            {
                var upgradeType = await FindTypeUpgrade(card);
                if (upgradeType == null || upgradeType.Id == card.TypeId)
                    continue;

                await AddHistory(card);

                card.TypeId = upgradeType.Id;
                card.ActivatedDate = DateTime.Today;
                card.ExpiredDate = cardTypeObj.GetPeriodEndDate(card.Type, dStart: card.ActivatedDate);
                card.PointInPeriod = 0;
            }

            await UpdateAsync(self);
        }

        public async Task ButtonUpgradeCard(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Type).ToListAsync();
            await ButtonUpgradeCard(self);
        }

        private async Task AddHistory(CardCard self)
        {
            var end_date = DateTime.Today;
            if (self.ExpiredDate.HasValue && end_date > self.ExpiredDate)
                end_date = self.ExpiredDate.Value;
            var historyObj = GetService<ICardHistoryService>();
            await historyObj.CreateAsync(new CardHistory
            {
                CardId = self.Id,
                StartDate = self.ActivatedDate,
                EndDate = end_date,
                PointInPeriod = self.PointInPeriod,
                TotalPoint = self.TotalPoint,
                TypeId = self.TypeId
            });
        }
    }
}
