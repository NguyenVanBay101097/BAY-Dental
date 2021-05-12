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
                query = query.Where(x => x.SaleOrderLineId == val.SaleOrderLineId.Value && x.SaleOrderId.HasValue);

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

        public async Task<PagedResult2<HistoryPromotionReponse>> GetHistoryPromotionResult(HistoryPromotionRequest val)
        {
            
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.SearchOrder))
                query = query.Where(x => val.SearchOrder.Contains(x.SaleOrder.Name));

            //if (!string.IsNullOrEmpty(val.SearchOrderLine))
            //    query = query.Where(x => val.SearchOrderLine.Contains(x.SaleOrderLine.Name));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateCreated >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateCreated <= dateOrderTo);
            }

            if (val.SaleCouponProgramId.HasValue)
                query = query.Where(x => x.SaleCouponProgramId.HasValue && x.SaleCouponProgramId == val.SaleCouponProgramId);

            var query2 = query.GroupBy(x => new
            {
                Date = x.DateCreated,
                SaleOrderId = x.SaleOrderId
            }).Select(x => new
            {
                SaleOrderId = x.Key.SaleOrderId,
                Date = x.Key.Date,
                AmountPromotion = x.Sum(x => x.Amount)
            });

            var totalItems = await query2.CountAsync();
            var items = await query2.OrderByDescending(x => x.Date)
                .Skip(val.Offset).Take(val.Limit).ToListAsync();

            var orderObj = GetService<ISaleOrderService>();
            var saleOrderIds = items.Select(x => x.SaleOrderId).ToList();
            IList<SaleOrder> saleOrders = await orderObj.SearchQuery(x=> saleOrderIds.Contains(x.Id)).Include(x=>x.Partner).ToListAsync();
            var saleOrderDict = saleOrders.ToDictionary(x => x.Id, x => x);

            //query = query.OrderByDescending(x => x.DateCreated);

            //if (val.Limit > 0)
            //    query = query.Skip(val.Offset).Take(val.Limit);

           

            var paged = new PagedResult2<HistoryPromotionReponse>(totalItems, val.Offset, val.Limit)
            {
                Items = items.Select(x => new HistoryPromotionReponse
                {
                    Amount = saleOrderDict[x.SaleOrderId.Value].AmountTotal ?? 0,
                    AmountPromotion = x.AmountPromotion,
                    DatePromotion = x.Date,
                    PartnerName = saleOrderDict[x.SaleOrderId.Value].Partner.Name,
                    SaleOrderName = saleOrderDict[x.SaleOrderId.Value].Name, 
                    SaleOrderId = saleOrderDict[x.SaleOrderId.Value].Id,
                })
            };

            return paged;


        }

        public SaleOrderPromotion PreparePromotionToOrder(SaleOrder self, SaleCouponProgram program, decimal discountAmount)
        {
            var promotionLine = new SaleOrderPromotion
            {
                Name = program.Name,
                SaleCouponProgramId = program.Id,
                Amount = discountAmount,
                SaleOrderId = self.Id,
            };

            _ComputePromotionType(promotionLine, program);

            var total = self.OrderLines.Sum(x => x.PriceUnit * x.ProductUOMQty);
            foreach (var line in self.OrderLines)
            {
                var subTotal = line.PriceUnit * line.ProductUOMQty;
                if (subTotal == 0)
                    continue;

                promotionLine.Lines.Add(new SaleOrderPromotionLine
                {
                    Amount = Math.Round(subTotal / total * discountAmount),
                    PriceUnit = (double)(line.ProductUOMQty != 0 ? (subTotal / total * discountAmount / line.ProductUOMQty) : 0),
                    SaleOrderLineId = line.Id,
                });
            }

            return promotionLine;
        }

        public SaleOrderPromotion PreparePromotionToOrderLine(SaleOrderLine self, SaleCouponProgram program, decimal discountAmount)
        {
            var total = self.PriceUnit * self.ProductUOMQty;
            var promotionLine = new SaleOrderPromotion
            {
                Name = program.Name,
                Amount = Math.Round(discountAmount),
                SaleCouponProgramId = program.Id,
                SaleOrderLineId = self.Id,
                SaleOrderId = self.OrderId
            };

            _ComputePromotionType(promotionLine, program);

            promotionLine.Lines.Add(new SaleOrderPromotionLine
            {
                Amount = promotionLine.Amount,
                PriceUnit = (double)(promotionLine.Amount / self.ProductUOMQty),
                SaleOrderLineId = self.Id,
            });

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
                .Include(x => x.Lines)
                .Include(x => x.SaleCouponProgram)
                .Include(x => x.SaleOrder)
                .ToListAsync();
            //có thể là ưu đãi phiếu điều trị hoặc là ưu đãi dịch vụ
            //nếu là ưu đã 
            foreach (var promotion in promotions)
            {

                //Tìm những coupon áp dụng trên promotion để update lại state
                if (promotion.SaleCouponProgramId.HasValue && promotion.SaleCouponProgram.ProgramType == "coupon_program")
                {
                    var appliedCoupons = await couponObj.SearchQuery(x => x.SaleOrderId == promotion.SaleOrderId)
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

            ///update AmountDiscountTotal lines to order      
            await orderObj._ComputeAmountPromotionToOrder(promotions.Select(x => x.SaleOrderId.Value).ToList());

        }


    }
}
