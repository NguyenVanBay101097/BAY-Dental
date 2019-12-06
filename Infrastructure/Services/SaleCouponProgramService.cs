using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class SaleCouponProgramService : BaseService<SaleCouponProgram>, ISaleCouponProgramService
    {
        private readonly IMapper _mapper;
        public SaleCouponProgramService(IAsyncRepository<SaleCouponProgram> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleCouponProgramBasic>> GetPagedResultAsync(SaleCouponProgramPaged val)
        {
            ISpecification<SaleCouponProgram> spec = new InitialSpecification<SaleCouponProgram>(x => x.Active);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => x.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.ProgramType))
                spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => x.ProgramType == val.ProgramType));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SaleCouponProgramBasic
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleCouponProgramBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SaleCouponProgram> CreateProgram(SaleCouponProgramSave val)
        {
            var program = _mapper.Map<SaleCouponProgram>(val);
            if (!program.CompanyId.HasValue)
                program.CompanyId = CompanyId;

            if (!program.DiscountLineProductId.HasValue)
            {
                var discountProduct = await GetOrCreateDiscountProduct(program);
                program.DiscountLineProductId = discountProduct.Id;
            }

            return await CreateAsync(program);
        }

        public async Task<SaleCouponProgramDisplay> GetDisplay(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Select(x => new SaleCouponProgramDisplay
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                DiscountFixedAmount = x.DiscountFixedAmount ?? 0,
                DiscountLineProductId = x.DiscountLineProductId,
                DiscountPercentage = x.DiscountPercentage ?? 0,
                DiscountType = x.DiscountType,
                Name = x.Name,
                RuleMinimumAmount = x.RuleMinimumAmount ?? 0,
                ValidityDuration = x.ValidityDuration ?? 0,
                CouponCount = x.Coupons.Count,
                OrderCount = x.SaleLines.Select(s => s.Order).Distinct().Count()
            }).FirstOrDefaultAsync();
        }

        public async Task UpdateProgram(Guid id, SaleCouponProgramSave val)
        {
            var program = await SearchQuery(x => x.Id == id)
                .Include(x => x.Coupons).FirstOrDefaultAsync();
            program = _mapper.Map(val, program);
            if (!program.DiscountLineProductId.HasValue)
            {
                var discountProduct = await GetOrCreateDiscountProduct(program);
                program.DiscountLineProductId = discountProduct.Id;
            }

            await UpdateAsync(program);

            if (program.Coupons.Any())
            {
                var couponObj = GetService<ISaleCouponService>();
                await couponObj.UpdateDateExpired(program.Coupons);
            }
        }

        private async Task<Product> GetOrCreateDiscountProduct(SaleCouponProgram self)
        {
            var discount_product_name = "";
            if (self.DiscountType == "fixed_amount")
                discount_product_name = "Giảm " + (self.DiscountFixedAmount ?? 0).ToString("n0") + " trên tổng tiền";
            else
                discount_product_name = "Giảm " + (self.DiscountPercentage ?? 0) + "% trên tổng tiền";
            var productObj = GetService<IProductService>();
            var product = await productObj.SearchQuery(x => x.Name == discount_product_name).FirstOrDefaultAsync();
            if (product == null)
            {
                var categObj = GetService<IProductCategoryService>();
                var categ = await categObj.DefaultCategory();
                var uomObj = GetService<IUoMService>();
                var uom = await uomObj.SearchQuery(x => x.Name == "Chiết khấu").FirstOrDefaultAsync();
                if (uom == null)
                {
                    var uomCategObj = GetService<IUoMCategoryService>();
                    var uom_categ = await uomCategObj.SearchQuery(x => x.MeasureType == "unit").FirstOrDefaultAsync();
                    uom = new UoM
                    {
                        Name = "Chiết khấu",
                        CategoryId = uom_categ.Id,
                        MeasureType = uom_categ.MeasureType,
                    };
                    await uomObj.CreateAsync(uom);
                }

                product = new Product()
                {
                    PurchaseOK = false,
                    SaleOK = false,
                    Type = "service",
                    Name = discount_product_name,
                    UOMId = uom.Id,
                    UOMPOId = uom.Id,
                    CategId = categ.Id
                };
                await productObj.CreateAsync(product);
            }

            return product;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            if (self.Any(x => x.Active))
                throw new Exception("Bạn không thể xóa chương trình khuyến mãi đang khả dụng");
            await DeleteAsync(self);
        }

        public async Task ToggleActive(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Coupons).ToListAsync();
            foreach (var program in self)
            {
                program.Active = !program.Active;
                if (!program.Active)
                {
                    var coupons = program.Coupons;
                    foreach(var coupon in coupons)
                    {
                        if (coupon.State != "used")
                            coupon.State = "expired";
                    }
                }
            }

            await UpdateAsync(self);
        }

        public async Task GenerateCoupons(SaleCouponProgramGenerateCoupons val)
        {
            var number = val.Number;
            var self = await GetByIdAsync(val.Id);
            if (number <= 0)
                return;
            var couponObj = GetService<ISaleCouponService>();
            for (var i = 0; i < number; i++)
            {
                await couponObj.CreateAsync(new SaleCoupon
                {
                    Code = StringUtils.RandomStringDigit(20),
                    ProgramId = self.Id,
                    Program = self,
                });
            }
        }

        public async Task Apply(SaleCouponProgram rule, SaleOrder order, decimal total_amount, decimal total_qty, SaleCoupon coupon = null)
        {
            if (rule.PromoApplicability == "on_current_order")
            {
                if (rule.RewardType == "discount")
                {
                    await UpdateDiscountLine(rule, order, total_amount, coupon: coupon);
                }
            }
            else if (rule.PromoApplicability == "on_next_order" && coupon == null)
            {
                if (rule.RewardType == "discount" && rule.DiscountLineProduct != null)
                {
                    var couponObj = GetService<ISaleCouponService>();
                    coupon = couponObj.SearchQuery(x => x.OrderId == order.Id).FirstOrDefault();
                    if (coupon == null)
                    {
                        coupon = new SaleCoupon
                        {
                            Code = StringUtils.RandomString(8).ToUpper(),
                            OrderId = order.Id,
                            PartnerId = order.PartnerId,
                            ProgramId = rule.Id,
                            Program = rule,
                            State = "reserved",
                        };
                        await couponObj.CreateAsync(coupon);
                    }
                    else if (coupon.State == "expired")
                    {
                        coupon.State = "new";
                    }
                }
            }
        }

        public async Task UpdateDiscountLine(SaleCouponProgram rule, SaleOrder order, decimal amount_total, decimal qty = 1, SaleCoupon coupon = null)
        {
            if (rule.RewardType == "discount" && rule.DiscountLineProduct == null)
                return;

            var soLineObj = GetService<ISaleOrderLineService>();
            decimal price_unit = 0;
            if (rule.RewardType == "discount" && rule.DiscountType == "percentage")
            {
                price_unit = amount_total * (rule.DiscountPercentage ?? 0) / 100;
                if ((rule.DiscountMaxAmount ?? 0) > 0)
                {
                    price_unit = Math.Min(price_unit, rule.DiscountMaxAmount ?? 0);
                }
            }
            else if (rule.RewardType == "discount" && rule.DiscountType == "fixed_amount")
            {
                price_unit = rule.DiscountFixedAmount ?? 0;
            }

            var product_discount_line =  rule.DiscountLineProduct;
            var coupon_id = coupon != null ? coupon.Id : (Guid?)null;
            var promo_line = order.OrderLines.Where(x => x.PromotionProgramId == rule.Id && (!coupon_id.HasValue || x.CouponId == coupon_id) && x.ProductId == product_discount_line.Id).FirstOrDefault();
            if (promo_line != null)
            {
                promo_line.ProductUOMQty = qty;
                promo_line.PriceUnit = -price_unit;
                soLineObj.ComputeAmount(new List<SaleOrderLine>() { promo_line });
            }
            else
            {
                await soLineObj.CreateAsync(PrepareDiscountLine(rule, order, price_unit, product: product_discount_line, qty: qty, coupon: coupon));
            }
        }

        private SaleOrderLine PrepareDiscountLine(SaleCouponProgram rule, SaleOrder order, decimal discount_amount, Product product = null, decimal qty = 1, SaleCoupon coupon = null)
        {
            return new SaleOrderLine
            {
                Name = $"Chiết khấu: {rule.Name}",
                Order = order,
                OrderId = order.Id,
                ProductUOMQty = qty,
                Product = product,
                ProductId = product.Id,
                ProductUOMId = product.UOMId,
                PriceUnit = -discount_amount,
                PromotionProgramId = rule.Id,
                CouponId = coupon != null ? coupon.Id : (Guid?)null
            };
        }
    }

    public class ApplyPromotionProductListItem
    {
        public Product Product { get; set; }

        public decimal Qty { get; set; }

        public decimal Amount { get; set; }
    }
}
