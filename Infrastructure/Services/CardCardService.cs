﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
                spec = spec.And(new InitialSpecification<CardCard>(x => x.Name.Contains(val.Search) || x.Barcode.Contains(val.Search)
                || x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) || x.Partner.Phone.Contains(val.Search)
                || x.Type.Name.Contains(val.Search)));

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
                State = x.State,
                PartnerPhone = x.Partner.Phone
            }).Skip(val.Offset).Take(val.Limit).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<CardCardBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override async Task UpdateAsync(IEnumerable<CardCard> entities)
        {
            await base.UpdateAsync(entities);
            foreach (var card in entities)
            {
                await _CheckBarcodeUnique(card.Barcode);
                await _CheckPartnerUnique(card);
            }
        }

        private async Task _CheckPartnerUnique(CardCard self)
        {
            if (!self.PartnerId.HasValue)
                return;

            var count = await SearchQuery(x => x.PartnerId == self.PartnerId).CountAsync();
            if (count >= 2)
                throw new Exception($"Khách hàng đã có thẻ thành viên");
        }

        public override async Task<IEnumerable<CardCard>> CreateAsync(IEnumerable<CardCard> self)
        {
            var seqObj = GetService<IIRSequenceService>();
            foreach (var card in self)
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
            {
                await _CheckBarcodeUnique(card.Barcode);
                await _CheckPartnerUnique(card);
            }

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
            if (barcode.Length < 5 || barcode.Length > 15)
            {
                throw new Exception($"Số ID tối thiểu 5 và tối đa 15 ký tự");
            }

            var count = await SearchQuery(x => x.Barcode == barcode).CountAsync();
            if (count >= 2)
                throw new Exception($"Số ID thẻ không được trùng");
        }

        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id) && x.State == "draft").ToListAsync();
            foreach (var card in self)
                card.State = "confirmed";
            await UpdateAsync(self);
        }

        public CheckPromoCodeMessage _CheckCardCardApplySaleLine(CardCard self, SaleOrderLine line)
        {
            var message = new CheckPromoCodeMessage();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var date = line.Date;

            if (line.Promotions.Any(x => x.ServiceCardCardId == self.Id))
                message.Error = "Trùng thẻ thành viên đang áp dụng";
            else if (self.State != "in_use")
                message.Error = "Thẻ không khả dụng";
            else if (line.Promotions.Any(x => x.SaleCouponProgramId.HasValue && x.SaleCouponProgram.PromoCodeUsage == "no_code_needed") || line.Promotions.Any(x => x.ServiceCardCardId.HasValue))
                message.Error = "Không thể áp dụng";

            return message;
        }

        public async Task ButtonActive(IEnumerable<Guid> ids, bool check_basic_points = true)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Type).ToListAsync();
            foreach (var card in self)
            {
                if (!card.PartnerId.HasValue)
                    throw new Exception("Khách hàng đang trống, cần bổ sung khách hàng");

                //await CheckExisted(card);

                var active_date = card.State != "in_use" ? card.ActivatedDate : null;
                if (!active_date.HasValue)
                    active_date = DateTime.Today;

                var expiry_date = cardTypeObj.GetPeriodEndDate(card.Type);

                card.State = "in_use";
                card.PointInPeriod = 0;
                card.ActivatedDate = active_date;
                card.ExpiredDate = expiry_date;
            }

            //await _CheckUpgrade(self);  mở lại sau khi BA đã chốt
            await UpdateAsync(self);
        }

        public async Task CheckExisted(CardCard self)
        {
            //Kiểm tra xem đã có thẻ nào đã được cấp cho khách hàng hay chưa? nếu có thì báo lỗi
            var states = new string[] { "cancelled" };
            var existed = await SearchQuery(x => x.Id != self.Id && !states.Contains(x.State) && x.PartnerId == self.PartnerId).FirstOrDefaultAsync();
            if (existed != null)
                throw new Exception($"Thẻ thành viên với số {existed.Name} đã được cấp cho khách hàng này");
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft" };
            foreach (var card in self)
            {
                if (!states.Contains(card.State))
                    throw new Exception("Thẻ thành viên đã kích hoạt không thể xoá");
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

        public async Task ButtonRenew(IEnumerable<Guid> ids, bool check_basic_points = true)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var card in self)
                await AddHistory(card);
            await ButtonActive(ids, check_basic_points: check_basic_points);
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

        public async Task ButtonUpgradeCard(IEnumerable<CardCard> self)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var cards_check_upgrade = new List<CardCard>();
            foreach (var card in self)
            {
                if (!card.UpgradeTypeId.HasValue)
                    continue;
                cards_check_upgrade.Add(card);

                await AddHistory(card);

                card.TypeId = card.UpgradeTypeId.Value;
                card.ActivatedDate = DateTime.Today;
                card.ExpiredDate = cardTypeObj.GetPeriodEndDate(card.Type, dStart: card.ActivatedDate);
                card.PointInPeriod = 0;
            }

            await UpdateAsync(cards_check_upgrade);
            await _CheckUpgrade(cards_check_upgrade);
        }

        public async Task _CheckUpgrade(IEnumerable<CardCard> self, decimal? points = null)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var ids = self.Select(x => x.Id).ToList();
            self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Type).ToListAsync();
            foreach (var r in self)
            {
                if (!points.HasValue)
                    points = r.PointInPeriod ?? 0;
                var type = await cardTypeObj.SearchQuery(x => x.BasicPoint <= points && x.BasicPoint >= r.Type.BasicPoint && x.Id != r.TypeId, orderBy: x => x.OrderByDescending(s => s.BasicPoint))
                    .FirstOrDefaultAsync();

                r.UpgradeTypeId = type != null ? type.Id : (Guid?)null;
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

        public async Task<ImportExcelResponse> ActionImport(IFormFile formFile)
        {

            if (formFile == null || formFile.Length <= 0)
            {
                throw new Exception("Vui lòng chọn file để import");
            }

            var list = new List<CardCard>();
            var cardTypeObj = GetService<ICardTypeService>();

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                try
                {
                    var errors = new List<string>();
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var typeName = worksheet.Cells[row, 2].Text.Trim();
                            var type = await cardTypeObj.SearchQuery(x => x.Name == typeName).FirstOrDefaultAsync();
                            if (type == null)
                                errors.Add($"Dòng {row}: Không tìm thấy hạng thẻ");

                            var barcode = worksheet.Cells[row, 1].Text.Trim();
                            if (string.IsNullOrEmpty(barcode))
                            {
                                errors.Add($"Dòng {row}: Số ID thẻ không được bỏ trống");
                            }
                            else if (barcode.Length < 5 || barcode.Length > 15)
                            {
                                errors.Add($"Dòng {row}: Số ID tối thiểu 5 và tối đa 15 ký tự");
                            }
                            var exist = await SearchQuery(x => x.Barcode == barcode).AnyAsync();
                            if (exist)
                            {
                                errors.Add($"Dòng {row}: Số ID thẻ bị trùng");
                            }

                            if (!errors.Any())
                                list.Add(new CardCard
                                {
                                    Barcode = barcode,
                                    TypeId = type.Id
                                });
                        }
                    }
                    if (errors.Any())
                        return new ImportExcelResponse { Success = false, Errors = errors };
                }
                catch (Exception ex)
                {

                    throw new Exception("File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng");
                }

            }

            await CreateAsync(list);

            return new ImportExcelResponse { Success = true };
        }

        public async Task<CardCard> GetDefault()
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var type = await cardTypeObj.SearchQuery().OrderBy(x => x.BasicPoint).FirstOrDefaultAsync();
            return new CardCard()
            {
                Type = type
            };
        }

        public override ISpecification<CardCard> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "member_card.card_card_comp_rule":
                    return new InitialSpecification<CardCard>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
