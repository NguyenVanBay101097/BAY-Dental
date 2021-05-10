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
