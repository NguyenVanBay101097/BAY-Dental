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
                    throw new Exception($"Thẻ đã hết hạn, không thể kích hoạt");
                }

                if (card.State != "draft" && card.State != "locked")
                {
                    throw new Exception($"Thẻ không thể kích hoạt, chỉ kích hoạt các thẻ chưa kích hoạt hoặc tạm dừng");
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
                    throw new Exception($"Thẻ không thể tạm dừng, chỉ tạm dừng các thẻ đã kích hoạt");
                }

                if (DateTime.Now > card.ExpiredDate)
                {
                    throw new Exception($"Thẻ đã hết hạn, không thể tạm dừng");
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
                    throw new Exception($"Thẻ đã hết hạn, không thể hủy");
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
            if (barcode.Length < 5 || barcode.Length > 15)
            {
                throw new Exception($"Số ID tối thiểu 5 và tối đa 15 ký tự");
            }

            var count = await SearchQuery(x => x.Barcode == barcode).CountAsync();
            if (count >= 2)
                throw new Exception($"Số ID thẻ không được trùng");
        }

        private async Task _CheckPartnerUnique(ServiceCardCard self)
        {
            if (!self.PartnerId.HasValue)
                return;

            var count = await SearchQuery(x => x.PartnerId == self.PartnerId && x.CardTypeId == self.CardTypeId).CountAsync();
            if (count >= 2)
                throw new Exception($"Khách hàng đã tồn tại hạng thẻ này trên hệ thống");
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft" };
            foreach (var card in self)
            {
                if (!states.Contains(card.State))
                    throw new Exception("Chỉ có thể xóa thẻ ưu đãi dịch vụ ở trạng thái chưa kích hoạt");
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
                    return new InitialSpecification<ServiceCardCard>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
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

            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.PartnerId == val.PartnerId.Value));

            if (!string.IsNullOrEmpty(val.state))
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.State == val.state));
            }

            if (val.ActivatedDate.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ActivatedDate.Value.Date == val.ActivatedDate.Value.Date));
            }

            if (val.ActivatedDateFrom.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ActivatedDate.Value.Date >= val.ActivatedDateFrom.Value.Date));
            }

            if (val.ActivatedDateTo.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ActivatedDate.Value.Date <= val.ActivatedDateTo.Value.Date));
            }

            if (val.DateFrom.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ExpiredDate.Value.Date >= val.DateFrom.Value.Date));
            }
            if (val.DateTo.HasValue)
            {
                spec = spec.And(new InitialSpecification<ServiceCardCard>(x => x.ExpiredDate.Value.Date <= val.DateTo.Value.Date));
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


        public CheckPromoCodeMessage _CheckServiceCardCardApplySaleLine(ServiceCardCard self, SaleOrderLine line)
        {
            var message = new CheckPromoCodeMessage();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var date = line.Date;

            if (line.Promotions.Any(x => x.ServiceCardCardId == self.Id))
                message.Error = "Trùng thẻ ưu đãi đang áp dụng";
            else if (self.State != "in_use")
                message.Error = "Thẻ không khả dụng";
            else if (line.Promotions.Any(x => x.ServiceCardCardId.HasValue))
                message.Error = "Không thể dùng chung với thẻ ưu đãi dịch vụ khác";
            else if ((self.ActivatedDate.HasValue && date < self.ActivatedDate.Value) || date > self.ExpiredDate.Value)
                message.Error = $"Thẻ ưu đãi đã hết hạn";
            else if (line.Promotions.Any(x => x.SaleCouponProgramId.HasValue && x.SaleCouponProgram.PromoCodeUsage == "no_code_needed") || line.Promotions.Any(x => x.CardCardId.HasValue))
                message.Error = "Không thể áp dụng";

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
                throw new Exception("Vui lòng chọn file để import");
            }

            var list = new List<ServiceCardCard>();
            var cardTypeObj = GetService<IServiceCardTypeService>();

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
                                list.Add(new ServiceCardCard
                                {
                                    Barcode = barcode,
                                    CardTypeId = type.Id
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

        public async Task<ServiceCardCard> GetNewestCreatedRequest(GetServiceCardCardNewestCreatedRequest val)
        {
            var query = SearchQuery();
            if (val.PartnerId.HasValue)
            {
                query = query.Where(x => x.PartnerId == val.PartnerId);
            }

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            return await query.OrderByDescending(x=> x.DateCreated).Include(x => x.CardType).FirstOrDefaultAsync();
        }
    }
}
