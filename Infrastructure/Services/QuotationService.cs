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
            var promotionObj = GetService<IQuotationPromotionService>();

            var model = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Employee)
                .Include(x => x.Payments)
                .Include(x => x.Promotions)
                .Include(x => x.Orders)
                .FirstOrDefaultAsync();

            var lines = await quotationLineObj.SearchQuery(x => x.QuotationId == id)
                .Include(x => x.QuotationLineToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.ToothCategory)
                .Include(x => x.Employee)
                .Include(x => x.Assistant)
                .Include(x => x.Counselor)
                .Include(x => x.PromotionLines).ThenInclude(x => x.Promotion)
                .Include(x => x.Promotions)
                .ToListAsync();

            model.Lines = lines;

            var display = _mapper.Map<QuotationDisplay>(model);

            display.Promotions = await promotionObj.SearchQuery(x => x.QuotationId.HasValue && x.QuotationId == display.Id && !x.QuotationLineId.HasValue).Select(x => new QuotationPromotionBasic
            {
                Id = x.Id,
                Name = x.Name,
                Amount = x.Amount,
                SaleCouponProgramId = x.SaleCouponProgramId,
                Type = x.Type
            }).ToListAsync();

            return display;
        }

        public async Task<QuotationDisplay> GetDefault(Guid partnerId)
        {
            var userObj = GetService<IUserService>();
            var partnerObj = GetService<IPartnerService>();
            var employeeObj = GetService<IEmployeeService>();



            var user = await userObj.GetCurrentUser();
            var employee = _mapper.Map<EmployeeSimple>(await employeeObj.SearchQuery(x => x.UserId == user.Id).FirstOrDefaultAsync());
            var partner = _mapper.Map<PartnerSimple>(await partnerObj.SearchQuery(x => x.Id == partnerId).FirstOrDefaultAsync());
            var quotation = new QuotationDisplay();
            quotation.Partner = partner;
            if (employee != null)
            {
                quotation.Employee = employee;
                quotation.EmployeeId = employee.Id;
            }
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
                query = query.Where(x => x.Name.Contains(val.Search) || x.Employee.Name.Contains(val.Search));
            var totalItem = await query.CountAsync();
            var items = await query.Include(x => x.Partner).Include(x => x.Employee).Include(x => x.Orders)
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
            var quotationLineObj = GetService<IQuotationLineService>();
            var quotation = await SearchQuery(x => x.Id == id).Include(x => x.Payments)
                .Include(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.Lines).ThenInclude(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.Lines).ThenInclude(x => x.Quotation).ThenInclude(x => x.Lines)
                .FirstOrDefaultAsync();

            _mapper.Map(val, quotation);
            await ComputeQuotationLine(val, quotation);
            await ComputePaymentQuotation(val, quotation);

            quotationLineObj.RecomputePromotionLine(quotation.Lines);

            ReComputePromotionOrder(quotation);
            await UpdateAsync(quotation);

            quotationLineObj._ComputeAmountDiscountTotal(quotation.Lines);
            quotationLineObj.ComputeAmount(quotation.Lines);

            ComputeAmountAll(quotation);
            await UpdateAsync(quotation);
        }

        public void ReComputePromotionOrder(Quotation quotation)
        {
            foreach (var promotion in quotation.Promotions.Where(x => !x.QuotationLineId.HasValue))
            {
                var total = promotion.Lines.Select(x => x.QuotationLine).Sum(x => ((x.SubPrice ?? 0) * x.Qty));
                if (promotion.Type == "discount")
                {

                    promotion.Amount = promotion.DiscountType == "percentage" ? total * (promotion.DiscountPercent ?? 0) / 100 : (promotion.DiscountFixed ?? 0);
                }

                if (promotion.SaleCouponProgramId.HasValue)
                {
                    if (promotion.SaleCouponProgram.DiscountType == "fixed_amount")
                        promotion.Amount = promotion.SaleCouponProgram.DiscountFixedAmount ?? 0;
                    else
                        promotion.Amount = total * (promotion.SaleCouponProgram.DiscountPercentage ?? 0) / 100;
                }

                foreach (var line in promotion.Lines)
                {
                    line.Amount = (((line.QuotationLine.SubPrice ?? 0) * line.QuotationLine.Qty) / total) * promotion.Amount;
                    line.PriceUnit = (double)(line.QuotationLine.Qty != 0 ?
                       ((line.QuotationLine.SubPrice * line.QuotationLine.Qty) / total) * promotion.Amount / line.QuotationLine.Qty : 0);
                }
            }
        }

        public async Task<QuotationBasic> CreateAsync(QuotationSave val)
        {
            var quotation = _mapper.Map<Quotation>(val);
            quotation = await CreateAsync(quotation);
            await ComputeQuotationLine(val, quotation);
            await ComputePaymentQuotation(val, quotation);
            ComputeAmountAll(quotation);
            await UpdateAsync(quotation);
            return _mapper.Map<QuotationBasic>(quotation);
        }

        public void ComputeAmountAll(Quotation quotation)
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
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineService = GetService<ISaleOrderLineService>();

            var today = DateTime.Today;
            var quotation = await SearchQuery(x => x.Id == id)
                .Include(x => x.Lines).ThenInclude(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.Lines).ThenInclude(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include("Lines.QuotationLineToothRels")
                .FirstOrDefaultAsync();

            var promotions = quotation.Promotions.Where(x => x.SaleCouponProgramId.HasValue).ToList();
            if (promotions.Any() && promotions.Any(x => (x.SaleCouponProgram.RuleDateFrom.HasValue && x.SaleCouponProgram.RuleDateFrom > today) || (x.SaleCouponProgram.RuleDateTo.HasValue && x.SaleCouponProgram.RuleDateTo < today)))
                throw new Exception("CTKM đã hết hạn. Vui lòng xóa các CTKM đã hết hạn để tiếp tục tạo phiếu điều trị");

            var saleOrder = new SaleOrder();

            saleOrder.CompanyId = quotation.CompanyId;
            saleOrder.State = "draft";
            saleOrder.PartnerId = quotation.PartnerId;
            saleOrder.QuotationId = quotation.Id;
            saleOrder.DateOrder = DateTime.Now;

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
                    saleLine.Diagnostic = line.Diagnostic;
                    saleLine.DiscountType = line.DiscountType;
                    saleLine.DiscountFixed = line.DiscountAmountFixed.HasValue ? line.DiscountAmountFixed : null;
                    saleLine.Discount = line.DiscountAmountPercent.HasValue ? line.DiscountAmountPercent.Value : 0;
                    saleLine.Name = line.Name;
                    saleLine.PriceUnit = line.SubPrice.HasValue ? line.SubPrice.Value : 0;
                    saleLine.ProductId = line.ProductId;
                    saleLine.ProductUOMQty = line.Qty;
                    saleLine.Order = saleOrder;
                    saleLine.Sequence = sequence++;
                    saleLine.ToothCategoryId = line.ToothCategoryId;
                    saleLine.ToothType = line.ToothType;
                    saleLine.EmployeeId = line.EmployeeId;
                    saleLine.AssistantId = line.AssistantId;
                    saleLine.CounselorId = line.CounselorId;

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

                    await saleLineService.CreateAsync(saleLine);

                    if (line.Promotions.Any())
                    {
                        foreach (var promotion in line.Promotions)
                        {
                            var saleLinePromotion = new SaleOrderPromotion();
                            saleLinePromotion.Amount = promotion.Amount;
                            saleLinePromotion.SaleOrderLineId = saleLine.Id;
                            saleLinePromotion.SaleOrderId = saleLine.OrderId;
                            saleLinePromotion.Type = promotion.Type;
                            if (promotion.Type == "discount")
                            {
                                saleLinePromotion.DiscountType = promotion.DiscountType;
                                saleLinePromotion.DiscountFixed = promotion.DiscountFixed;
                                saleLinePromotion.DiscountPercent = promotion.DiscountPercent;
                            }

                            if (promotion.Lines.Any())
                            {
                                foreach (var item in promotion.Lines)
                                {
                                    saleLinePromotion.Lines.Add(new SaleOrderPromotionLine
                                    {
                                        SaleOrderLine = saleLine,
                                        Amount = item.Amount,
                                        PriceUnit = item.PriceUnit,

                                    });
                                }
                            }

                            saleLine.Promotions.Add(saleLinePromotion);
                        }
                    }
                }



                if (quotation.Promotions.Any())
                {
                    foreach (var promotion in quotation.Promotions.Where(x => !x.QuotationLineId.HasValue))
                    {
                        var total = saleOrder.OrderLines.Sum(x => x.PriceUnit * x.ProductUOMQty);
                        var orderPromotion = new SaleOrderPromotion();
                        orderPromotion.Name = promotion.Name;
                        orderPromotion.Amount = promotion.Amount;
                        orderPromotion.SaleOrder = saleOrder;
                        orderPromotion.Type = promotion.Type;
                        if (promotion.Type == "discount")
                        {
                            orderPromotion.Name = "Giảm giá";
                            orderPromotion.DiscountType = promotion.DiscountType;
                            orderPromotion.DiscountFixed = promotion.DiscountFixed;
                            orderPromotion.DiscountPercent = promotion.DiscountPercent;
                        }

                        foreach (var line in saleOrder.OrderLines)
                        {
                            var amount = (((line.ProductUOMQty * line.PriceUnit) / total) * promotion.Amount);
                            orderPromotion.Lines.Add(new SaleOrderPromotionLine
                            {
                                SaleOrderLineId = line.Id,
                                Amount = amount,
                                PriceUnit = (double)(line.ProductUOMQty != 0 ? (amount / line.ProductUOMQty) : 0),
                            });
                        }

                        saleOrder.Promotions.Add(orderPromotion);
                    }
                }

                await saleOrderObj.UpdateAsync(saleOrder);
                await saleOrderObj._ComputeAmountPromotionToOrder(new List<Guid>() { saleOrder.Id });

            }

            return _mapper.Map<SaleOrderSimple>(saleOrder);
        }


        public async Task<QuotationPrintVM> Print(Guid id)
        {
            var quotation = await SearchQuery(x => x.Id == id)
               .Include(x => x.Partner)
               .Include(x => x.Employee)
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
            var sequenceService = GetService<IIRSequenceService>();
            entity.Name = await sequenceService.NextByCode("quotation");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await _InsertQuotationSequence();
                entity.Name = await sequenceService.NextByCode("quotation");
            }

            await base.CreateAsync(entity);

            return entity;
        }

        private async Task _InsertQuotationSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu báo giá",
                Code = "quotation",
                Prefix = "BG",
                Padding = 5
            });
        }

        public async Task _ComputeAmountPromotionToQuotation(IEnumerable<Guid> ids)
        {
            var quotationLineObj = GetService<IQuotationLineService>();
            var quotations = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines).ThenInclude(x => x.PromotionLines)
                .ToListAsync();
            foreach (var quotation in quotations)
            {
                quotationLineObj._ComputeAmountDiscountTotal(quotation.Lines);
                quotationLineObj.ComputeAmount(quotation.Lines);
                ComputeAmountAll(quotation);
            }

            await UpdateAsync(quotations);
        }

        public async Task ApplyDiscountOnQuotation(ApplyDiscountViewModel val)
        {
            var quotationPromotionObj = GetService<IQuotationPromotionService>();
            var quotationLineObj = GetService<IQuotationLineService>();

            var quotation = await SearchQuery(x => x.Id == val.Id).Include(x => x.Lines).ThenInclude(x => x.Promotions).ThenInclude(x => x.Lines).FirstOrDefaultAsync();
            var total = quotation.Lines.Sum(x => (x.SubPrice ?? 0) * x.Qty);
            var discount_amount = val.DiscountType == "percentage" ? total * val.DiscountPercent / 100 : val.DiscountFixed;

            var promotion = quotation.Promotions.Where(x => x.Type == "discount" && !x.QuotationLineId.HasValue).FirstOrDefault();
            if (promotion != null)
            {
                promotion.Amount = (discount_amount ?? 0);
            }
            else
            {
                promotion = new QuotationPromotion
                {
                    Name = "Giảm tiền",
                    Amount = (discount_amount ?? 0),
                    DiscountType = val.DiscountType,
                    DiscountPercent = val.DiscountPercent,
                    DiscountFixed = val.DiscountFixed,
                    Type = "discount",
                    QuotationId = quotation.Id
                };

                await quotationPromotionObj.CreateAsync(promotion);
            }

            if (quotation.Lines.Any())
            {
                foreach (var line in quotation.Lines)
                {
                    if (line.Qty == 0)
                        continue;

                    var amount = (((line.Qty * (line.SubPrice ?? 0)) / total) * promotion.Amount);
                    if (amount != 0)
                    {
                        promotion.Lines.Add(new QuotationPromotionLine
                        {
                            Amount = amount,
                            PriceUnit = (double)(line.Qty != 0 ? amount / line.Qty : 0),
                            QuotationLineId = line.Id,
                        });
                    }
                }
            }

            await quotationPromotionObj.UpdateAsync(promotion);

            //tính lại tổng tiền ưu đãi quotationlines
            quotationLineObj._ComputeAmountDiscountTotal(quotation.Lines);
            quotationLineObj.ComputeAmount(quotation.Lines);

            ComputeAmountAll(quotation);
            await UpdateAsync(quotation);
        }


        public async Task ApplyPromotionOnQuotation(ApplyPromotionRequest val)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var orderLineObj = GetService<ISaleOrderLineService>();
            var quotation = await SearchQuery(x => x.Id == val.Id)
              .Include(x => x.Promotions).ThenInclude(x => x.Lines)
              .Include(x => x.Lines).ThenInclude(x => x.Promotions)
              .Include("Lines.Product")
              .FirstOrDefaultAsync();

            var soLineObj = GetService<ISaleOrderLineService>();
            var product_list = new List<ApplyPromotionProductListItem>();
            var ruleObj = GetService<IPromotionRuleService>();
            var coupons = new List<SaleCoupon>();

            var program = await programObj.SearchQuery(x => x.Id == val.SaleProgramId).FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckQuotationPromotion(program, quotation);
                if (string.IsNullOrEmpty(error_status.Error))
                {

                    await _CreateRewardLine(quotation, program);

                    await UpdateAsync(quotation);

                }
                else
                    throw new Exception(error_status.Error);
            }
        }

        public async Task<SaleCouponProgramResponse> ApplyPromotionUsageCode(ApplyPromotionUsageCode val)
        {
            var couponCode = val.CouponCode;
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var quotation = await SearchQuery(x => x.Id == val.Id)
             .Include(x => x.Promotions).ThenInclude(x => x.Lines)
             .Include(x => x.Lines).ThenInclude(x => x.Promotions)
             .Include(x => x.Lines).ThenInclude(x => x.Product)
             .FirstOrDefaultAsync();

            //Chương trình khuyến mãi sử dụng mã
            var program = await programObj.SearchQuery(x => x.PromoCode == couponCode).FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckQuotationPromoCode(program, quotation, couponCode);
                if (string.IsNullOrEmpty(error_status.Error))
                {

                    await _CreateRewardLine(quotation, program);
                    await UpdateAsync(quotation);
                    return new SaleCouponProgramResponse { Error = null, Success = true, SaleCouponProgram = _mapper.Map<SaleCouponProgramDisplay>(program) };
                }
                else
                    //throw new Exception(error_status.Error);
                    return new SaleCouponProgramResponse { Error = error_status.Error, Success = false, SaleCouponProgram = null };
            }
            else
            {
                return new SaleCouponProgramResponse { Error = "Mã chương trình khuyến mãi không tồn tại", Success = false, SaleCouponProgram = null };
            }
        }

        private async Task _CreateRewardLine(Quotation self, SaleCouponProgram program)
        {
            var quotationLineObj = GetService<IQuotationLineService>();
            var quotationPromotionObj = GetService<IQuotationPromotionService>();
            var promotion = _GetRewardValuesDiscount(self, program);
            //foreach (var line in lines)
            //    line.Order = self;

            await quotationPromotionObj.CreateAsync(promotion);

            //tính lại tổng tiền ưu đãi saleorderlines
            quotationLineObj._ComputeAmountDiscountTotal(self.Lines);
            quotationLineObj.ComputeAmount(self.Lines);

            ComputeAmountAll(self);
            await UpdateAsync(self);
        }

        public QuotationPromotion _GetRewardValuesDiscount(Quotation self, SaleCouponProgram program)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var quotationLineObj = GetService<IQuotationLineService>();
            var promotionObj = GetService<IQuotationPromotionService>();

            if (program.DiscountLineProduct == null)
            {
                var productObj = GetService<IProductService>();
                program.DiscountLineProduct = productObj.GetById(program.DiscountLineProductId);
            }

            if (program.DiscountType == "fixed_amount")
            {

                var discountAmount = _GetRewardValuesDiscountFixedAmount(self, program);
                var promotionLine = promotionObj.PreparePromotionToQuotation(self, program, discountAmount);

                return promotionLine;
            }


            var reward = new QuotationPromotion();
            var lines = self.Lines;
            if (program.DiscountApplyOn == "specific_products" || program.DiscountApplyOn == "on_order")
            {
                if (program.DiscountApplyOn == "specific_products")
                {
                    var discount_specific_product_ids = program.DiscountSpecificProducts.Select(x => x.ProductId).ToList();
                    //We should not exclude reward line that offer this product since we need to offer only the discount on the real paid product (regular product - free product)
                    var free_product_lines = programObj.SearchQuery(x => x.RewardType == "product" && discount_specific_product_ids.Contains(x.RewardProductId.Value)).Select(x => x.DiscountLineProductId.Value).ToList();
                    var tmp = discount_specific_product_ids.Union(free_product_lines);
                    lines = lines.Where(x => tmp.Contains(x.ProductId)).ToList();
                }

                var total_discount_amount = 0M;

                foreach (var line in lines)
                {
                    var discount_line_amount = quotationLineObj._GetRewardValuesDiscountPercentagePerLine(program, line);
                    if (discount_line_amount > 0)
                        total_discount_amount += discount_line_amount;
                }

                reward = promotionObj.PreparePromotionToQuotation(self, program, total_discount_amount);


            }

            return reward;
        }

        private decimal _GetRewardValuesDiscountFixedAmount(Quotation self, SaleCouponProgram program)
        {
            var total_amount = self.Lines.Sum(x => (x.SubPrice ?? 0));
            var fixed_amount = program.DiscountFixedAmount ?? 0;
            if (total_amount < fixed_amount)
                return total_amount;
            return fixed_amount;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
              .Include(x => x.Lines)
              .Include(x => x.Promotions).ToListAsync();      

            var promotionObj = GetService<IQuotationPromotionService>();
            var promotionIds = await promotionObj.SearchQuery(x => ids.Contains(x.QuotationId.Value)).Select(x => x.Id).ToListAsync();
            if (promotionIds.Any())
                await promotionObj.RemovePromotion(promotionIds);

            await DeleteAsync(self);
        }
    }
}
