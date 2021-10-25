using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class ServiceCardCardService : BaseService<ServiceCardCard>, IServiceCardCardService
    {
        private readonly IMapper _mapper;
        private static readonly Random _random = new Random();

        public ServiceCardCardService(IAsyncRepository<ServiceCardCard> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task ActionActive(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.CardType).ToListAsync();
            var cardTypeObj = GetService<IServiceCardTypeService>();
            foreach (var card in self)
            {
                if (DateTime.Now > card.ExpiredDate)
                {
                    throw new Exception($"Thẻ {card.Barcode} đã hết hạn, không thể tạm dừng");
                }

                if (card.State != "draft" && card.State != "locked")
                {
                    throw new Exception($"Thẻ {card.Barcode} không thể kích hoạt, chỉ kích hoạt các thẻ chưa kích hoạt hoặc tạm dừng");
                }

                if (!card.PartnerId.HasValue)
                {
                    throw new Exception("Khách hàng đang trống, cần bổ sung khách hàng");
                }

                if (!card.ActivatedDate.HasValue)
                    card.ActivatedDate = DateTime.Today;
                var active_date = card.ActivatedDate.Value;
                var expire_date = cardTypeObj.GetPeriodEndDate(card.CardType, active_date);

                card.State = "in_use";
                card.ExpiredDate = expire_date;
            }

            await UpdateAsync(self);
        }

        public async Task ActionLock(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var card in self)
            {
                if (card.State != "in_use")
                {
                    throw new Exception($"Thẻ {card.Barcode} không thể tạm dừng, chỉ tạm dừng các thẻ đã kích hoạt");
                }

                if (DateTime.Now > card.ExpiredDate)
                {
                    throw new Exception($"Thẻ {card.Barcode} đã hết hạn, không thể tạm dừng");
                }

                card.State = "locked";
            }

            await UpdateAsync(self);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var card in self)
            {

                if (DateTime.Now > card.ExpiredDate)
                {
                    throw new Exception($"Thẻ {card.Barcode} đã hết hạn, không thể hủy");
                }
                card.State = "cancelled";
            }
            await UpdateAsync(self);
        }

        public async Task ButtonConfirm(IEnumerable<ServiceCardCard> self)
        {
            foreach (var card in self)
                card.State = "confirmed";
            await UpdateAsync(self);
        }

        public override async Task UpdateAsync(IEnumerable<ServiceCardCard> entities)
        {
            await base.UpdateAsync(entities);
            foreach (var card in entities)
            {
                await _CheckBarcodeUnique(card.Barcode);
                await _CheckPartnerUnique(card);
            }
        }

        public override async Task<IEnumerable<ServiceCardCard>> CreateAsync(IEnumerable<ServiceCardCard> self)
        {
            var seqObj = GetService<IIRSequenceService>();
            foreach (var card in self)
            {
                if (string.IsNullOrEmpty(card.Name) || card.Name == "/")
                {
                    card.Name = await seqObj.NextByCode("sequence_seq_service_card_nb");
                    if (string.IsNullOrEmpty(card.Name))
                    {
                        await CreateCardSequence();
                        card.Name = await seqObj.NextByCode("sequence_seq_service_card_nb");
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

        private async Task _CheckBarcodeUnique(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return;

            var count = await SearchQuery(x => x.Barcode == barcode).CountAsync();
            if (count >= 2)
                throw new Exception($"Đã có thẻ dịch vụ với mã vạch {barcode}");
        }

        private async Task _CheckPartnerUnique(ServiceCardCard self)
        {
            if (!self.PartnerId.HasValue)
                return;

            var count = await SearchQuery(x => x.PartnerId == self.PartnerId && x.CardTypeId == self.CardTypeId).CountAsync();
            if (count >= 2)
                throw new Exception($"Khách hàng{(self.Partner != null ? $" {self.Partner.Name}" : "")} đã có thẻ");
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var card in self)
            {
                if (card.State == "in_use")
                    throw new Exception("Ưu đãi dịch vụ đã kích hoạt không thể xóa");
            }

            await DeleteAsync(self);
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

        public override ISpecification<ServiceCardCard> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "service_card.service_card_card_comp_rule":
                    return new InitialSpecification<ServiceCardCard>(x => !x.CardType.CompanyId.HasValue || x.CardType.CompanyId == companyId);
                default:
                    return null;
            }
        }

        private async Task CreateCardSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Prefix = "SC",
                Padding = 6,
                NumberIncrement = 1,
                NumberNext = 1,
                Code = "sequence_seq_service_card_nb",
                Name = "Service Card Sequence"
            });
        }

        public async Task<PagedResult2<ServiceCardCardBasic>> GetPagedResultAsync(ServiceCardCardPaged val)
        {
            ISpecification<ServiceCardCard> spec = new InitialSpecification<ServiceCardCard>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x =>
                x.Name.Contains(val.Search) ||
                x.Barcode.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.CardType.Name.Contains(val.Search)));

            if (val.OrderId.HasValue)
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.SaleLine.OrderId == val.OrderId.Value));
            if (!string.IsNullOrEmpty(val.state))
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.State == val.state));
            }

            if (val.ActivatedDate.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ActivatedDate == val.ActivatedDateFrom));
            }

            if (val.ActivatedDateFrom.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ActivatedDate >= val.ActivatedDateFrom));
            }
            if (val.ActivatedDateTo.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ActivatedDate <= val.ActivatedDateTo));
            }
            if (val.ExpiredDateFrom.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ExpiredDate >= val.ExpiredDateFrom));
            }
            if (val.ExpiredDateTo.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ExpiredDate <= val.ExpiredDateTo));
            }


            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            if (val.Limit <= 0)
                val.Limit = int.MaxValue;
            var res = await query.Include(x => x.CardType).Include(x => x.Partner).Skip(val.Offset).Take(val.Limit).ToListAsync();
            var items = _mapper.Map<IEnumerable<ServiceCardCardBasic>>(res);
            var totalItems = await query.CountAsync();

            return new PagedResult2<ServiceCardCardBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public void _ComputeResidual(IEnumerable<ServiceCardCard> self)
        {
            foreach (var card in self)
            {
                var total_apply_sale = card.SaleOrderCardRels.Sum(x => x.Amount);
                card.Residual = card.Amount - total_apply_sale;
            }
        }

        public async Task<IEnumerable<ServiceCardCard>> _ComputeResidual(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.SaleOrderCardRels).ToListAsync();
            _ComputeResidual(self);
            await UpdateAsync(self);

            return self;
        }

        public async Task<IEnumerable<ServiceCardCardResponse>> GetServiceCardCards(ServiceCardCardFilter val)
        {
            var today = DateTime.Today;
            var productPriceListItemObj = GetService<IProductPricelistItemService>();
            var query = SearchQuery();

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.CardType.ProductPricelist.Items.Any(s => s.ProductId == val.ProductId));

                query = query.Where(x => (!x.ActivatedDate.HasValue || today >= x.ActivatedDate.Value) && (!x.ExpiredDate.HasValue || today <= x.ExpiredDate.Value));
            }

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            var items = await query.Include(x => x.CardType).ThenInclude(x => x.ProductPricelist).ThenInclude(x => x.Items).ToListAsync();

            var res = _mapper.Map<IEnumerable<ServiceCardCardResponse>>(items);

            if (val.ProductId.HasValue)
            {
                var productPricelist = res.Select(x => x.CardType.ProductPricelistId.Value).Distinct().ToList();
                var pricelistItems = await productPriceListItemObj.SearchQuery(x => productPricelist.Contains(x.PriceListId.Value)).ToListAsync();
                foreach (var item in res)
                {
                    var pricelistItem = pricelistItems.Where(x => x.PriceListId == item.CardType.ProductPricelistId && x.ProductId == val.ProductId.Value).FirstOrDefault();
                    item.ProductPricelistItem = pricelistItem == null ? null : _mapper.Map<ProductPricelistItemDisplay>(pricelistItem);
                }

            }

            return res;
        }


        public CheckPromoCodeMessage _CheckServiceCardCardApplySaleLine(ServiceCardCard self, SaleOrderLine line)
        {
            var message = new CheckPromoCodeMessage();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var today = DateTime.Today;

            if (line.Promotions.Any(x => x.ServiceCardCardId == self.Id))
                message.Error = "Trùng thẻ ưu đãi đang áp dụng";
            else if (line.Promotions.Any(x => x.ServiceCardCardId.HasValue))
                message.Error = "Không thể dùng chung với thẻ ưu đãi dịch vụ khác";
            else if ((self.ActivatedDate.HasValue && today < self.ActivatedDate.Value) || today > self.ExpiredDate.Value)
                message.Error = $"Thẻ ưu đãi đã hết hạn";

            return message;
        }

        public async Task<ServiceCardCard> CheckCode(string code)
        {
            var card = await SearchQuery(x => x.Barcode == code).FirstOrDefaultAsync();
            if (card == null)
                throw new Exception($"Không tìm thấy thẻ nào với mã vạch {code}");

            if (card.State != "in_use")
                throw new Exception($"Thẻ không khả dụng");

            if (card.Residual == 0)
                throw new Exception($"Số dư của thẻ bằng 0");

            return card;
        }

        public async Task<ImportExcelResponse> ActionImport(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                return new ImportExcelResponse { Success = false, Errors = new List<string>() { "File không được trống" } };
            }

            if (!System.IO.Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return new ImportExcelResponse { Success = false, Errors = new List<string>() { "Chỉ import file excel" } };
            }

            var list = new List<ServiceCardCard>();
            var cardTypeObj = GetService<IServiceCardTypeService>();

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var typeName = worksheet.Cells[row, 2].Value.ToString().Trim();
                        var type = await cardTypeObj.SearchQuery(x => x.Name == typeName).FirstOrDefaultAsync();
                        if (type == null)
                            return new ImportExcelResponse { Success = false, Errors = new List<string>() { $@"dòng {row} không tìm thấy hạng thẻ" } };

                        list.Add(new ServiceCardCard
                        {
                            Barcode = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            CardTypeId = type.Id
                        });
                    }
                }
            }

            await CreateAsync(list);

            return new ImportExcelResponse { Success = true };
        }
    }
}
