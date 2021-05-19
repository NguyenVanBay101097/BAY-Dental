using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    public class QuotationLineService : BaseService<QuotationLine>, IQuotationLineService
    {
        private readonly IMapper _mapper;

        public QuotationLineService(IMapper mapper, IAsyncRepository<QuotationLine> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }




        public void _ComputeAmountDiscountTotal(IEnumerable<QuotationLine> self)
        {
            //Trường hợp ưu đãi phiếu điều trị thì ko đúng, sum từ PromotionLines là đúng
            foreach (var line in self)
                line.AmountDiscountTotal = line.PromotionLines.Sum(x => x.PriceUnit);
        }

        public void ComputeAmount(IEnumerable<QuotationLine> self)
        {
            if (self == null)
                return;

            foreach (var line in self)
                line.Amount = Math.Round(line.Qty * ((line.SubPrice ?? 0) - (decimal)(line.AmountDiscountTotal ?? 0)));

        }

        public decimal _GetRewardValuesDiscountPercentagePerQuotationLine(SaleCouponProgram program, QuotationLine line)
        {
            //discount_amount = so luong * don gia da giam * phan tram
            var price_reduce = ((line.SubPrice ?? 0) * (1 - (program.DiscountPercentage ?? 0) / 100));
            var discount_amount = ((line.SubPrice ?? 0) - price_reduce) * line.Qty;
            return discount_amount;
        }

        public decimal _GetRewardValuesDiscountPercentagePerLine(SaleCouponProgram program, QuotationLine line)
        {
            var total = (line.SubPrice ?? 0) * line.Qty;
            //discount_amount = so luong * don gia da giam * phan tram        
            var discount_amount = (total  * ((program.DiscountPercentage ?? 0) / 100));
            return discount_amount;
        }

        private decimal _GetRewardValuesDiscountFixedAmountLine(QuotationLine self, SaleCouponProgram program)
        {
            var price_reduce = (self.SubPrice ?? 0) - (program.DiscountFixedAmount ?? 0);
            var fixed_amount = ((self.SubPrice ?? 0) - price_reduce) * self.Qty;
            return fixed_amount;
        }

        public async Task ApplyDiscountOnQuotationLine(ApplyDiscountViewModel val)
        {
            var quotationPromotionObj = GetService<IQuotationPromotionService>();
            var quotationObj = GetService<IQuotationService>();

            var quotationLine = await SearchQuery(x => x.Id == val.Id).Include(x => x.Quotation)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.PromotionLines)
                .FirstOrDefaultAsync();

            var total = (quotationLine.SubPrice ?? 0) * quotationLine.Qty;
            var price_reduce = val.DiscountType == "percentage" ? (quotationLine.SubPrice ?? 0) * (1 - val.DiscountPercent / 100) : (quotationLine.SubPrice ?? 0) - val.DiscountFixed;
            var discount_amount = ((quotationLine.SubPrice ?? 0) - price_reduce) * quotationLine.Qty;

            var promotion = quotationPromotionObj.SearchQuery(x => x.QuotationLineId == quotationLine.Id && x.Type == "discount" && x.QuotationId.HasValue).FirstOrDefault();
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
                    QuotationLineId = quotationLine.Id,
                    QuotationId = quotationLine.QuotationId
                };

                promotion.Lines.Add(new QuotationPromotionLine
                {
                    QuotationLineId = promotion.QuotationLineId.Value,
                    Amount = promotion.Amount,
                    PriceUnit = (double)(quotationLine.Qty != 0 ? (promotion.Amount / quotationLine.Qty) : 0),
                });

                await quotationPromotionObj.CreateAsync(promotion);
            }

            //tính lại tổng tiền ưu đãi 
           await quotationObj._ComputeAmountPromotionToQuotation(new List<Guid>(){ quotationLine.QuotationId});
        }

        public async Task<SaleCouponProgramResponse> ApplyPromotionOnQuotationLine(ApplyPromotionRequest val)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var quotationLine = await SearchQuery(x => x.Id == val.Id)
              .Include(x => x.Product)
              .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
              .Include(x => x.Quotation).ThenInclude(x => x.Promotions)
              .Include(x => x.Quotation).ThenInclude(x => x.Lines)
              .Include(x => x.PromotionLines)
              .FirstOrDefaultAsync();

            var program = await programObj.SearchQuery(x => x.Id == val.SaleProgramId).Include(x => x.DiscountSpecificProducts).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckPromotionApplyQuotationLine(program, quotationLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateRewardLine(quotationLine, program);
                    return new SaleCouponProgramResponse { Error = null, Success = true, SaleCouponProgram = _mapper.Map<SaleCouponProgramDisplay>(program) };
                }
                else
                    return new SaleCouponProgramResponse { Error = error_status.Error, Success = false, SaleCouponProgram = null };
            }
            else
            {
                return new SaleCouponProgramResponse { Error = "Mã chương trình khuyến mãi không tồn tại", Success = false, SaleCouponProgram = null };

            }
        }

        public async Task<SaleCouponProgramResponse> ApplyPromotionUsageCodeOnQuotationLine(ApplyPromotionUsageCode val)
        {
            var couponCode = val.CouponCode;
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var quotationLine = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.PromotionLines)
                .Include(x => x.Product)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.Quotation).ThenInclude(x => x.Promotions)
                .Include(x => x.Quotation).ThenInclude(x => x.Lines)
                .FirstOrDefaultAsync();

            //Chương trình khuyến mãi sử dụng mã
            var program = await programObj.SearchQuery(x => x.PromoCode == couponCode).Include(x => x.DiscountSpecificProducts).FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckPromotionApplyQuotationLine(program, quotationLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateRewardLine(quotationLine, program);

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

        private async Task _CreateRewardLine(QuotationLine self, SaleCouponProgram program)
        {
            var quotationObj = GetService<IQuotationService>();
            var quotationPromotionObj = GetService<IQuotationPromotionService>();
            var promotion = _GetRewardValuesDiscount(self, program);
            //foreach (var line in lines)
            //    line.Order = self;

            await quotationPromotionObj.CreateAsync(promotion);

            //tính lại tổng tiền ưu đãi 
            await quotationObj._ComputeAmountPromotionToQuotation(new List<Guid>() { self.QuotationId });
        }

        public QuotationPromotion _GetRewardValuesDiscount(QuotationLine self, SaleCouponProgram program)
        {
            var promotionObj = GetService<IQuotationPromotionService>();
            var programObj = GetService<ISaleCouponProgramService>();

            if (program.DiscountLineProduct == null)
            {
                var productObj = GetService<IProductService>();
                program.DiscountLineProduct = productObj.GetById(program.DiscountLineProductId);
            }

            if (program.DiscountType == "fixed_amount")
            {

                var discountAmount = _GetRewardValuesDiscountFixedAmountLine(self, program);
                var promotionLine = promotionObj.PreparePromotionToQuotationLine(self, program, discountAmount);

                return promotionLine;
            }


            var rewards = new List<QuotationPromotion>();
            if (program.DiscountApplyOn == "on_order")
                throw new Exception("Chương trình khuyến mãi không đúng định dạng");

            if (program.DiscountApplyOn == "specific_products")
            {
                var discount_specific_product_ids = program.DiscountSpecificProducts.Select(x => x.ProductId).ToList();
                //We should not exclude reward line that offer this product since we need to offer only the discount on the real paid product (regular product - free product)
                var free_product_lines = programObj.SearchQuery(x => x.RewardType == "product" && discount_specific_product_ids.Contains(x.RewardProductId.Value)).Select(x => x.DiscountLineProductId.Value).ToList();
                var tmp = discount_specific_product_ids.Union(free_product_lines);
                if (tmp.Contains(self.ProductId))
                {
                    var discount_amount = _GetRewardValuesDiscountPercentagePerQuotationLine(program, self);
                    var promotionLine = promotionObj.PreparePromotionToQuotationLine(self, program, discount_amount);

                    return promotionLine;
                }
            }

            return new QuotationPromotion();
        }


        public void RecomputePromotionLine(IEnumerable<QuotationLine> self)
        {
            //vong lap
            foreach (var line in self)
            {
                if (line.Promotions.Any())
                {
                    foreach (var promotion in line.Promotions)
                    {
                        var total = (line.SubPrice ?? 0) * line.Qty;
                        if (promotion.Type == "discount")
                        {
                            var price_reduce = promotion.DiscountType == "percentage" ? (line.SubPrice ?? 0) * (1 - (promotion.DiscountPercent ?? 0) / 100) : (line.SubPrice - promotion.DiscountFixed ?? 0);
                            var discount_amount = ((line.SubPrice ?? 0) - price_reduce) * line.Qty;
                            promotion.Amount = discount_amount;
                        }

                        if (promotion.SaleCouponProgramId.HasValue)
                        {
                            if (promotion.SaleCouponProgram.DiscountType == "fixed_amount")
                                promotion.Amount = promotion.SaleCouponProgram.DiscountFixedAmount ?? 0;
                            else
                                promotion.Amount = total * (promotion.SaleCouponProgram.DiscountPercentage ?? 0) / 100;
                        }

                        foreach (var child in promotion.Lines)
                        {
                            child.Amount = promotion.Amount;
                            child.PriceUnit = (double)(line.Qty != 0 ? (promotion.Amount / line.Qty) : 0);
                        }
                    }
                }
            }


        }

    }
}
