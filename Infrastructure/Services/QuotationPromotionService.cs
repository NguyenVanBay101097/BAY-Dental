using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class QuotationPromotionService : BaseService<QuotationPromotion>, IQuotationPromotionService
    {
        private readonly IMapper _mapper;
        public QuotationPromotionService(IAsyncRepository<QuotationPromotion> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<QuotationPromotionBasic>> GetPagedResultAsync(QuotationPromotionPaged val)
        {
            var query = SearchQuery();

            if (val.QuotationId.HasValue)
                query = query.Where(x => x.QuotationId == val.QuotationId.Value);

            if (val.QuotationLineId.HasValue)
                query = query.Where(x => x.QuotationLineId == val.QuotationLineId.Value && x.QuotationLineId.HasValue);

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.ToListAsync();

            var paged = new PagedResult2<QuotationPromotionBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<QuotationPromotionBasic>>(items)
            };

            return paged;
        }

        public QuotationPromotion PreparePromotionToQuotation(Quotation self, SaleCouponProgram program, decimal discountAmount)
        {
            var promotionLine = new QuotationPromotion
            {
                Name = program.Name,
                SaleCouponProgramId = program.Id,
                Amount = discountAmount,
                QuotationId = self.Id,
            };

            _ComputePromotionType(promotionLine, program);

            var total = self.Lines.Sum(x => (x.SubPrice ?? 0) * x.Qty);
            foreach (var line in self.Lines)
            {
                var subTotal = ((line.SubPrice ?? 0) * line.Qty);
                if (subTotal == 0)
                    continue;

                promotionLine.Lines.Add(new QuotationPromotionLine
                {
                    Amount = Math.Round(subTotal / total * discountAmount),
                    PriceUnit = (double)(line.Qty != 0 ? (subTotal / total * discountAmount / line.Qty) : 0),
                    QuotationLineId = line.Id,
                });
            }

            return promotionLine;
        }


        public QuotationPromotion PreparePromotionToQuotationLine(QuotationLine self, SaleCouponProgram program, decimal discountAmount)
        {
            var total = (self.SubPrice ?? 0) * self.Qty;
            var promotionLine = new QuotationPromotion
            {
                Name = program.Name,
                Amount = Math.Round(discountAmount),
                SaleCouponProgramId = program.Id,
                QuotationLineId = self.Id,
                QuotationId = self.QuotationId
            };

            _ComputePromotionType(promotionLine, program);

            promotionLine.Lines.Add(new QuotationPromotionLine
            {
                Amount = promotionLine.Amount,
                PriceUnit = (double)(promotionLine.Amount / self.Qty),
                QuotationLineId = self.Id,
            });

            return promotionLine;
        }

        public void _ComputePromotionType(QuotationPromotion self, SaleCouponProgram program)
        {
            if (program.ProgramType == "coupon_program" || (program.ProgramType == "promotion_program" && program.PromoCodeUsage == "code_needed" && !string.IsNullOrEmpty(program.PromoCode)))
            {
                self.Type = "code_usage_program";
            }
            else
            {
                self.Type = "promotion_program";
            }
        }

        public bool _IsRewardInQuotationLines(Quotation self, SaleCouponProgram program)
        {
            return self.Lines.Where(x => x.ProductId == program.RewardProductId &&
            x.Qty >= program.RewardProductQuantity).Any();
        }

        public async Task RemovePromotion(IEnumerable<Guid> ids)
        {
            var quotationObj = GetService<IQuotationService>();
            var quotationLineObj = GetService<IQuotationLineService>();
            var couponObj = GetService<ISaleCouponService>();
            var promotions = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines)
                .Include(x => x.SaleCouponProgram)
                .Include(x => x.Quotation)
                .Include(x => x.QuotationLine)
                .ToListAsync();
            //có thể là ưu đãi phiếu báo giá hoặc là ưu đãi dịch vụ
            //nếu là ưu đã 
            foreach (var promotion in promotions)
            {

                //Tìm những coupon áp dụng trên promotion để update lại state
                if (promotion.SaleCouponProgramId.HasValue && promotion.SaleCouponProgram.ProgramType == "coupon_program")
                {
                    var appliedCoupons = await couponObj.SearchQuery(x => x.SaleOrderId == promotion.QuotationId)
                        .Include(x => x.Program).ToListAsync();

                    var coupons_to_reactivate = appliedCoupons.Where(x => x.Program.Id == promotion.SaleCouponProgramId).ToList();
                    foreach (var coupon in coupons_to_reactivate)
                    {
                        coupon.State = "new";
                        coupon.SaleOrderId = null;
                    }

                    await couponObj.UpdateAsync(coupons_to_reactivate);
                }


            }

            await DeleteAsync(promotions);

            ///update AmountDiscountTotal lines to quotation      
            await quotationObj._ComputeAmountPromotionToQuotation(promotions.Select(x => x.QuotationId.Value).ToList());

        }
    }
}
