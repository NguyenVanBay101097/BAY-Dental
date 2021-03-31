using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
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
    public class QuotationService : BaseService<Quotation>, IQuotationService
    {
        private readonly IMapper _mapper;

        public QuotationService(IAsyncRepository<Quotation> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }



        public async Task<IEnumerable<QuotationDisplay>> GetDisplay(Guid id)
        {
            var model = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.Lines).ToListAsync();
            return _mapper.Map<IEnumerable<QuotationDisplay>>(model);
        }

        public async Task<QuotationDisplay> GetDefault(Guid partnerId)
        {
            var partnerObj = GetService<IPartnerService>();
            var partner = _mapper.Map<PartnerSimple>(await partnerObj.SearchQuery(x => x.Id == partnerId).FirstOrDefaultAsync());
            var quotation = new QuotationDisplay();
            quotation.Partner = partner;
            quotation.PartnerId = partner.Id;
            quotation.DateQuotation = DateTime.Today;
            quotation.DateApplies = 30;
            quotation.DateEndQuotation = DateTime.Today.AddDays(30);
            return quotation;
        }

        public async Task<PagedResult2<QuotationBasic>> GetPagedResultAsync(QuotationPaged val)
        {
            var query = SearchQuery();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateQuotation >= val.DateFrom.Value);
            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateQuotation <= val.DateTo.Value);
            }
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            var totalItem = await query.CountAsync();
            var items = await query
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Take(val.Limit)
                .Skip(val.Offset)
                .ToListAsync();
            return new PagedResult2<QuotationBasic>(totalItem, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<QuotationBasic>>(items)
            };
        }

        public async Task UpdateAsync(Guid id, QuotationSave val)
        {
            var quotation = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            _mapper.Map(val, quotation);
            await ComputeQuotationLine(val, quotation);
            await ComputeAmountAll(quotation);
            await UpdateAsync(quotation);
        }

        public async Task ComputeQuotationLine(QuotationSave val, Quotation quotation)
        {
            var listAdd = new List<QuotationLine>();
            var listRemove = new List<QuotationLine>();
            var listUpdate = new List<QuotationLine>();
            var quotationLineObj = GetService<IQuotationLineService>();
            foreach (var line in quotation.Lines)
            {
                if (!val.QuotationLines.Any(x => x.Id == line.Id))
                {
                    listRemove.Add(line);
                }
            }

            foreach (var line in val.QuotationLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var quoLine = _mapper.Map<QuotationLine>(line);
                    quoLine.QuotationId = quotation.Id;
                    quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) * (1 - (line.PercentDiscount.HasValue ? line.PercentDiscount.Value : 0) / 100);
                    foreach (var toothId in line.ToothIds)
                    {
                        quoLine.QuotationLineToothRels.Add(new QuotationLineToothRel
                        {
                            ToothId = toothId
                        });
                    }
                    listAdd.Add(quoLine);
                }
                else
                {
                    var quoLine = await quotationLineObj.SearchQuery(x => x.Id == line.Id).Include(x => x.QuotationLineToothRels).FirstOrDefaultAsync();
                    _mapper.Map(line, quoLine);
                    quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) * (1 - (line.PercentDiscount.HasValue ? line.PercentDiscount.Value : 0) / 100);

                    foreach (var item in quoLine.QuotationLineToothRels.ToList())
                    {
                        if (!line.ToothIds.Any(x => x == item.ToothId))
                        {
                            quoLine.QuotationLineToothRels.Remove(item);
                        }
                    }
                    foreach (var toothId in line.ToothIds)
                    {
                        if (!quoLine.QuotationLineToothRels.Any(x => x.ToothId == toothId))
                        {
                            quoLine.QuotationLineToothRels.Add(new QuotationLineToothRel
                            {
                                QuotationLineId = quoLine.Id,
                                ToothId = toothId
                            });
                        }
                    }

                    listUpdate.Add(quoLine);
                }
            }

            await quotationLineObj.CreateAsync(listAdd);
            await quotationLineObj.UpdateAsync(listUpdate);
            await quotationLineObj.DeleteAsync(listRemove);
        }

        public async Task<QuotationBasic> CreateAsync(QuotationSave val)
        {
            var quotation = _mapper.Map<Quotation>(val);
            quotation.State = "confirm";
            if (string.IsNullOrEmpty(quotation.Name) || quotation.Name == "/")
            {
                var sequenceService = GetService<IIRSequenceService>();
                quotation.Name = await sequenceService.NextByCode("quotation");
            }

            quotation = await CreateAsync(quotation);

            var lines = new List<QuotationLine>();

            foreach (var line in val.QuotationLines)
            {
                var quoLine = _mapper.Map<QuotationLine>(line);
                quoLine.QuotationId = quotation.Id;
                quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) * (1 - (line.PercentDiscount.HasValue ? line.PercentDiscount.Value : 0) / 100);
                foreach (var toothId in line.ToothIds)
                {
                    quoLine.QuotationLineToothRels.Add(new QuotationLineToothRel
                    {
                        ToothId = toothId
                    });
                }
                lines.Add(quoLine);
            }
            var quotationLineService = GetService<IQuotationLineService>();
            await quotationLineService.CreateAsync(lines);
            ComputeAmountAll(quotation);
            await UpdateAsync(quotation);
            return _mapper.Map<QuotationBasic>(quotation);
        }

        public async Task ComputeAmountAll(Quotation quotation)
        {
            var totalAmount = 0M;
            foreach (var line in quotation.Lines)
            {
                totalAmount += Math.Round(line.Amount.HasValue ? line.Amount.Value : 0);
            }
            quotation.TotalAmount = totalAmount;
        }

    }
}
