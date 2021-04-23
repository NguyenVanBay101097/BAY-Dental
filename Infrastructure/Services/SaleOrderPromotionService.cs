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

        public async Task<PagedResult2<SaleOrderPromotionDisplay>> GetPagedResultAsync(SaleOrderPromotionPaged val)
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

            var items = await query.Include(x => x.SaleOrderPromotionChilds).ToListAsync();

            var paged = new PagedResult2<SaleOrderPromotionDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderPromotionDisplay>>(items)
            };

            return paged;
        }




        public async Task RemovePromotion(IEnumerable<Guid> ids)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderLineObj = GetService<ISaleOrderLineService>();
            var order_ids = new List<Guid>().AsEnumerable();
            var saleLineIds = new List<Guid>().AsEnumerable();
            var couponObj = GetService<ISaleCouponService>();
            var promotions = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.SaleCouponProgram)
                .Include(x => x.SaleOrderPromotionChilds).ThenInclude(x => x.Parent)
                .Include(x => x.SaleOrder).ThenInclude(x => x.AppliedCoupons)
                .Include(x => x.SaleOrderLine)
                .Include(x => x.Parent)
                .ToListAsync();
            //có thể là ưu đãi phiếu điều trị hoặc là ưu đãi dịch vụ
            //nếu là ưu đã 
            foreach (var promotion in promotions)
            {
                if (promotion.SaleOrderId.HasValue)
                    order_ids = order_ids.Union(new List<Guid>() { promotion.SaleOrderId.Value });

                if (promotion.SaleOrderLineId.HasValue)
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
            await orderObj._ComputeAmountPromotionToOrder(order_ids);

        }


    }
}
