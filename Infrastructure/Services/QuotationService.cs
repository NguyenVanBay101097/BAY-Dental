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

        public async Task<QuotationDisplay> GetDisplay(Guid id)
        {
            var quotationLineObj = GetService<IQuotationLineService>();
            var model = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.Payments)
                .Include(x => x.Orders)
                .FirstOrDefaultAsync();

            var lines = await quotationLineObj.SearchQuery(x => x.QuotationId == id)
                .Include(x => x.QuotationLineToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.ToothCategory)
                .Include(x => x.AdvisoryUser).ToListAsync();

            model.Lines = lines;
            return _mapper.Map<QuotationDisplay>(model);
        }

        public async Task<QuotationDisplay> GetDefault(Guid partnerId)
        {
            var userObj = GetService<IUserService>();
            var partnerObj = GetService<IPartnerService>();
            var user = await userObj.GetCurrentUser();
            var partner = _mapper.Map<PartnerSimple>(await partnerObj.SearchQuery(x => x.Id == partnerId).FirstOrDefaultAsync());
            var quotation = new QuotationDisplay();
            quotation.Partner = partner;
            quotation.User = _mapper.Map<ApplicationUserSimple>(user);
            quotation.UserId = user.Id;
            quotation.PartnerId = partner.Id;
            quotation.DateQuotation = DateTime.Today;
            quotation.DateApplies = 30;
            quotation.DateEndQuotation = DateTime.Today.AddDays(30);
            quotation.CompanyId = CompanyId;
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
                query = query.Where(x => x.Name.Contains(val.Search) || x.User.Name.Contains(val.Search));
            var totalItem = await query.CountAsync();
            var items = await query
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.Orders)
                .Skip(val.Offset)
                .Take(val.Limit)
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();
            return new PagedResult2<QuotationBasic>(totalItem, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<QuotationBasic>>(items)
            };
        }

        public async Task UpdateAsync(Guid id, QuotationSave val)
        {
            var quotation = await SearchQuery(x => x.Id == id).Include(x => x.Lines).Include(x => x.Payments).FirstOrDefaultAsync();
            _mapper.Map(val, quotation);
            await ComputeQuotationLine(val, quotation);
            await ComputePaymentQuotation(val, quotation);
            await ComputeAmountAll(quotation);
            await UpdateAsync(quotation);
        }

        public async Task<QuotationBasic> CreateAsync(QuotationSave val)
        {
            var quotation = _mapper.Map<Quotation>(val);
            quotation = await CreateAsync(quotation);
            await ComputeQuotationLine(val, quotation);
            await ComputePaymentQuotation(val, quotation);
            await ComputeAmountAll(quotation);
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

        public async Task ComputePaymentQuotation(QuotationSave val, Quotation quotation)
        {
            var listAdd = new List<PaymentQuotation>();
            var listRemove = new List<PaymentQuotation>();
            var listUpdate = new List<PaymentQuotation>();
            var paymentQuotationObj = GetService<IPaymentQuotationService>();
            foreach (var line in quotation.Payments)
            {
                if (!val.Payments.Any(x => x.Id == line.Id))
                {
                    listRemove.Add(line);
                }
            }

            foreach (var payment in val.Payments)
            {
                if (payment.Id == Guid.Empty)
                {
                    var payQuot = _mapper.Map<PaymentQuotation>(payment);
                    payQuot.QuotationId = quotation.Id;
                    listAdd.Add(payQuot);
                }
                else
                {
                    var payQuot = await paymentQuotationObj.SearchQuery(x => x.Id == payment.Id).FirstOrDefaultAsync();
                    _mapper.Map(payment, payQuot);
                    listUpdate.Add(payQuot);
                }
            }
            await paymentQuotationObj.CreateAsync(listAdd);
            await paymentQuotationObj.UpdateAsync(listUpdate);
            await paymentQuotationObj.DeleteAsync(listRemove);

        }

        public async Task ComputeQuotationLine(QuotationSave val, Quotation quotation)
        {
            var listAdd = new List<QuotationLine>();
            var listRemove = new List<QuotationLine>();
            var listUpdate = new List<QuotationLine>();
            var quotationLineObj = GetService<IQuotationLineService>();
            foreach (var line in quotation.Lines)
            {
                if (!val.Lines.Any(x => x.Id == line.Id))
                {
                    listRemove.Add(line);
                }
            }

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var quoLine = _mapper.Map<QuotationLine>(line);
                    quoLine.QuotationId = quotation.Id;
                    if (line.DiscountType == "fixed")
                        quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) - line.Discount;

                    else if (line.DiscountType == "percentage")
                        quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) * (1 - (line.Discount.HasValue ? line.Discount.Value : 0) / 100);

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

                    if (line.DiscountType == "fixed")
                        quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) - line.Discount;
                    else if (line.DiscountType == "percentage")
                        quoLine.Amount = line.Qty * (line.SubPrice.HasValue ? line.SubPrice.Value : 0) * (1 - (line.Discount.HasValue ? line.Discount.Value : 0) / 100);

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

        public async Task<SaleOrderSimple> CreateSaleOrderByQuotation(Guid id)
        {
            var quotation = await SearchQuery(x => x.Id == id)
                .Include(x => x.Lines)
                .Include("Lines.QuotationLineToothRels")
                .FirstOrDefaultAsync();
            var saleOrderDefaultGet = new SaleOrderDefaultGet()
            {
                PartnerId = quotation.PartnerId
            };
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrderDisplay = await saleOrderObj.DefaultGet(saleOrderDefaultGet);
            var saleOrder = new SaleOrder();

            saleOrder.CompanyId = saleOrderDisplay.CompanyId;
            saleOrder.DateOrder = saleOrderDisplay.DateOrder;
            saleOrder.State = saleOrderDisplay.State;
            saleOrder.PartnerId = saleOrderDisplay.PartnerId;
            saleOrder.QuotationId = quotation.Id;

            saleOrder = await saleOrderObj.CreateAsync(saleOrder);
            if (quotation.Lines.Any())
            {
                var lines = quotation.Lines.ToList();
                var sequence = 0;
                var SaleOrderLines = new List<SaleOrderLine>();
                foreach (var line in lines)
                {
                    var saleLine = new SaleOrderLine();
                    saleLine.State = "draft";
                    saleLine.AmountPaid = 0;
                    saleLine.Diagnostic = line.Diagnostic;
                    saleLine.DiscountType = line.DiscountType;
                    saleLine.DiscountFixed = line.DiscountType == "fixed" ? line.Discount : null;
                    saleLine.Discount = line.DiscountType == "percentage" && line.Discount.HasValue ? line.Discount.Value : 0;
                    saleLine.Name = line.Name;
                    saleLine.PriceUnit = line.SubPrice.HasValue ? line.SubPrice.Value : 0;
                    saleLine.ProductId = line.ProductId;
                    saleLine.ProductUOMQty = line.Qty;
                    saleLine.Order = saleOrder;
                    saleLine.Sequence = sequence++;
                    saleLine.ToothCategoryId = line.ToothCategoryId;
                    saleLine.AmountResidual = saleLine.PriceSubTotal - saleLine.AmountPaid;
                    if (line.QuotationLineToothRels.Any())
                    {
                        var toothIds = line.QuotationLineToothRels.Select(x => x.ToothId);
                        foreach (var toothId in toothIds)
                        {
                            saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                            {
                                ToothId = toothId
                            });
                        }
                    }
                    SaleOrderLines.Add(saleLine);
                }


                var saleLineService = GetService<ISaleOrderLineService>();
                await saleLineService.CreateAsync(SaleOrderLines);

                saleOrderObj._AmountAll(saleOrder);
                await saleOrderObj.UpdateAsync(saleOrder);

            }

            return _mapper.Map<SaleOrderSimple>(saleOrder);
        }

        public async Task<QuotationPrintVM> Print(Guid id)
        {
            var quotation = await SearchQuery(x => x.Id == id)
               .Include(x => x.Partner)
               .Include(x => x.User)
               .Include(x => x.Lines)
               .Include(x => x.Company).ThenInclude(x => x.Partner)
               .Include(x => x.Payments).FirstOrDefaultAsync();

            if (quotation == null)
            {
                return null;
            }
            var result = _mapper.Map<QuotationPrintVM>(quotation);

            return result;
        }

        public async override Task<Quotation> CreateAsync(Quotation entity)
        {
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                var sequenceService = GetService<IIRSequenceService>();
                entity.Name = await sequenceService.NextByCode("quotation");
            }
            return await base.CreateAsync(entity);
        }
    }
}
