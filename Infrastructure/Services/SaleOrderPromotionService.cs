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
    public class SaleOrderPromotionService : BaseService<SaleOrderPromotion>, ISaleOrderPromotionService
    {
        private readonly IMapper _mapper;

        public SaleOrderPromotionService(IAsyncRepository<SaleOrderPromotion> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleOrderPromotionBasic>> GetPagedResultAsync(SaleOrderPromotionPaged val)
        {
            var query = SearchQuery();

            if (val.SaleOrderId.HasValue)
                query = query.Where(x => x.SaleOrderId == val.SaleOrderId.Value);

            if (val.SaleOrderLineId.HasValue)
                query = query.Where(x => x.SaleOrderLineId == val.SaleOrderLineId.Value && !x.ParentId.HasValue);

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.ToListAsync();

            var paged = new PagedResult2<SaleOrderPromotionBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderPromotionBasic>>(items)
            };

            return paged;
        }

        public SaleOrderPromotion PreparePromotionToOrder(SaleOrder self, SaleCouponProgram program, decimal discountAmount)
        {
            var promotionLine = new SaleOrderPromotion
            {
                Name = program.Name,
                ProductId = program.DiscountLineProductId,
                Amount = discountAmount,
                SaleOrderId = self.Id,
            };

            _ComputePromotionType(promotionLine, program);

            foreach (var line in self.OrderLines)
            {
                var subTotal = line.PriceSubTotal;
                if (subTotal == 0)
                    continue;

                promotionLine.SaleOrderPromotionChilds.Add(new SaleOrderPromotion
                {
                    Name = promotionLine.Name,
                    Amount = (subTotal / (self.AmountTotal ?? 0)) * discountAmount / line.ProductUOMQty,
                    ProductId = promotionLine.ProductId,
                    SaleOrderLineId = line.Id,
                    Type = promotionLine.Type
                });
            }

            return promotionLine;
        }

        public SaleOrderPromotion PreparePromotionToOrderLine(SaleOrderLine self, SaleCouponProgram program, decimal discountAmount)
        {
            var promotionLine = new SaleOrderPromotion
            {
                Name = program.Name,
                Amount = Math.Round((self.PriceSubTotal / (self.Order.AmountTotal ?? 0)) * discountAmount / self.ProductUOMQty),
                ProductId = program.DiscountLineProductId,
                SaleOrderLineId = self.Id,
            };

            _ComputePromotionType(promotionLine, program);
         

            return promotionLine;
        }

        public void _ComputePromotionType(SaleOrderPromotion self, SaleCouponProgram program)
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


        public async Task RemovePromotion(IEnumerable<Guid> ids)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderLineObj = GetService<ISaleOrderLineService>();
            var order_ids = new List<Guid>().AsEnumerable();
            var saleLineIds = new List<Guid>().AsEnumerable();
            var couponObj = GetService<ISaleCouponService>();
            var promotions = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.SaleOrderPromotionChilds).ThenInclude(x => x.Parent)               
                .Include(x => x.Parent)
                .ToListAsync();
            //có thể là ưu đãi phiếu điều trị hoặc là ưu đãi dịch vụ
            //nếu là ưu đã 
            foreach (var promotion in promotions)
            {
                if (promotion.SaleOrderId.HasValue)
                    order_ids = order_ids.Union(new List<Guid>() { promotion.SaleOrderId.Value });

                if (promotion.SaleOrderLineId.HasValue && !promotion.ParentId.HasValue)
                    saleLineIds = saleLineIds.Union(new List<Guid>() { promotion.SaleOrderLineId.Value });

                //xóa ưu đãi phiếu điều trị: discount, coupon, promotion
                if (promotion.SaleOrderPromotionChilds.Any())
                {
                    //Xóa discount phân bổ
                    await DeleteAsync(promotion.SaleOrderPromotionChilds);
                }


                //Tìm những coupon áp dụng trên promotion để update lại state
                if (promotion.SaleOrderId.HasValue)
                {
                    var appliedCoupons = await couponObj.SearchQuery(x => x.SaleOrderId == promotion.SaleOrderId)
                        .Include(x => x.Program).ToListAsync();

                    var coupons_to_reactivate = appliedCoupons.Where(x => x.Program.DiscountLineProductId == promotion.ProductId).ToList();
                    foreach (var coupon in coupons_to_reactivate)
                    {
                        coupon.State = "new";
                        coupon.SaleOrderId = null;
                    }

                    await couponObj.UpdateAsync(coupons_to_reactivate);
                }


            }

            await DeleteAsync(promotions);

            ///update AmountDiscountTotal lines to order      

            if (order_ids.Any())
                await orderObj._ComputeAmountPromotionToOrder(order_ids);

            if (saleLineIds.Any())
            {
                var orderIds = await orderObj.SearchQuery(x => x.OrderLines.Any(s => saleLineIds.Contains(s.Id)))
                    .Select(x=>x.Id)
                    .ToListAsync();

                await orderObj._ComputeAmountPromotionToOrder(orderIds);
            }

        }


    }
}
