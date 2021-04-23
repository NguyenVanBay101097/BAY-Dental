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
            ISpecification<SaleCouponProgram> spec = new InitialSpecification<SaleCouponProgram>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => x.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.ProgramType))
                spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => x.ProgramType == val.ProgramType));
            if (val.Active.HasValue)
                spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => x.Active == val.Active));
            if (val.Ids != null)
                spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => val.Ids.Contains(x.Id)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Sequence).ThenBy(s => s.RewardType));
            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }
            var items = await _mapper.ProjectTo<SaleCouponProgramBasic>(query).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleCouponProgramBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override ISpecification<SaleCouponProgram> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale_coupon.sale_coupon_program_comp_rule":
                    return new InitialSpecification<SaleCouponProgram>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public void _CheckRuleMinimumAmount(SaleCouponProgram self)
        {
            if (self.RuleMinimumAmount < 0)
                throw new Exception("Số tiền mua tối thiểu nên lớn hơn hoặc bằng 0");
        }

        public void _CheckRuleMinQuantity(SaleCouponProgram self)
        {
            if (self.RuleMinQuantity <= 0)
                throw new Exception("Số lượng tối thiểu nên lớn hơn 0");
        }

        public async Task<SaleCouponProgram> CreateProgram(SaleCouponProgramSave val)
        {
            var program = _mapper.Map<SaleCouponProgram>(val);
            if (!program.CompanyId.HasValue)
                program.CompanyId = CompanyId;

            await SaveDiscountSpecificProducts(program, val);

            if (!program.DiscountLineProductId.HasValue)
            {
                var discountProduct = await GetOrCreateDiscountProduct(program);
                program.DiscountLineProductId = discountProduct.Id;
            }

            _CheckDiscountPercentage(program);
            _CheckRuleMinimumAmount(program);
            _CheckRuleMinQuantity(program);
            await _CheckPromoCodeConstraint(program);
            return await CreateAsync(program);
        }

        public void _CheckDiscountPercentage(SaleCouponProgram self)
        {
            if (self.DiscountType == "percentage" && (self.DiscountPercentage < 0 || self.DiscountPercentage > 100))
                throw new Exception("Chiết khấu phần trăm phải từ 0-100");
        }

        public async Task<SaleCouponProgramDisplay> GetDisplay(Guid id)
        {
            var query = SearchQuery(x => x.Id == id);
            var res = await _mapper.ProjectTo<SaleCouponProgramDisplay>(query).FirstOrDefaultAsync();
            res.OrderCount = await _GetOrderCountAsync(id);
            return res;
        }

        public async Task UpdateProgram(Guid id, SaleCouponProgramSave val)
        {
            var program = await SearchQuery(x => x.Id == id)
                .Include(x => x.Coupons).Include(x => x.DiscountLineProduct)
                .Include(x => x.DiscountSpecificProducts).Include("DiscountSpecificProducts.Product").FirstOrDefaultAsync();
            program = _mapper.Map(val, program);
            if (!program.DiscountLineProductId.HasValue)
            {
                var discountProduct = await GetOrCreateDiscountProduct(program);
                program.DiscountLineProduct = discountProduct;
                program.DiscountLineProductId = discountProduct.Id;
            }

            if (!program.CompanyId.HasValue)
                program.CompanyId = CompanyId;

            await SaveDiscountSpecificProducts(program, val);

            var reward_name = await GetRewardDisplayName(program);
            if (program.DiscountLineProduct != null)
            {
                var productObj = GetService<IProductService>();
                program.DiscountLineProduct.Name = reward_name;
                program.DiscountLineProduct.NameNoSign = StringUtils.RemoveSignVietnameseV2(reward_name);
                await productObj.UpdateAsync(program.DiscountLineProduct);
            }

            _CheckDiscountPercentage(program);
            _CheckRuleMinimumAmount(program);
            _CheckRuleMinQuantity(program);
            await _CheckPromoCodeConstraint(program);
            await UpdateAsync(program);

            if (program.Coupons.Any())
            {
                var couponObj = GetService<ISaleCouponService>();
                await couponObj.UpdateDateExpired(program.Coupons);
            }
        }

        public IEnumerable<SaleCouponProgram> _FilterPromoProgramsWithCode(IEnumerable<SaleCouponProgram> self, string promo_code)
        {
            return self.Where(x => x.PromoCodeUsage == "code_needed" && x.PromoCode != promo_code);
        }

        private async Task SaveDiscountSpecificProducts(SaleCouponProgram program, SaleCouponProgramSave val)
        {
            var productObj = GetService<IProductService>();
            var to_remove = program.DiscountSpecificProducts.Where(x => !val.DiscountSpecificProductIds.Contains(x.ProductId)).ToList();
            foreach (var item in to_remove)
                program.DiscountSpecificProducts.Remove(item);

            var to_add = val.DiscountSpecificProductIds.Where(x => !program.DiscountSpecificProducts.Any(s => s.ProductId == x)).ToList();
            foreach (var productId in to_add)
            {
                var product = await productObj.GetByIdAsync(productId);
                program.DiscountSpecificProducts.Add(new SaleCouponProgramProductRel { ProductId = productId, Product = product });
            }
        }

        public async Task _CheckPromoCodeConstraint(SaleCouponProgram self)
        {
            if (self.PromoCodeUsage == "code_needed" && string.IsNullOrEmpty(self.PromoCode))
                throw new Exception("Vui lòng nhập mã khuyến mãi!");
            var exist = await SearchQuery(x => x.Id != self.Id && x.PromoCodeUsage == "code_needed" && x.PromoCode == self.PromoCode).FirstOrDefaultAsync();
            if (exist != null)
                throw new Exception("Mã khuyến mãi phải là duy nhất!");
        }

        private async Task<string> GetRewardDisplayName(SaleCouponProgram self)
        {
            var productObj = GetService<IProductService>();
            var reward_string = "";
            if (self.RewardType == "product")
            {
                if (self.RewardProduct == null || self.RewardProduct.Id != self.RewardProductId)
                    self.RewardProduct = await productObj.GetByIdAsync(self.RewardProductId);
                reward_string = $"Dịch vụ miễn phí - {self.RewardProduct.Name}";
            }
            else if (self.RewardType == "discount")
            {
                if (self.DiscountType == "percentage")
                {
                    var reward_percentage = (self.DiscountPercentage ?? 0).ToString();
                    if (self.DiscountApplyOn == "on_order")
                        reward_string = $"Giảm {reward_percentage}% trên tổng tiền";
                    else if (self.DiscountApplyOn == "specific_products")
                    {
                        if (!self.DiscountSpecificProducts.Any())
                            throw new Exception("Vui lòng chọn dịch vụ sẽ được chiết khấu");

                        if (self.DiscountSpecificProducts.Count > 1)
                            reward_string = $"Giảm {reward_percentage}% trên vài dịch vụ";
                        else
                            reward_string = $"Giảm {reward_percentage}% trên {self.DiscountSpecificProducts.First().Product.Name}";
                    }
                }
                else if (self.DiscountType == "fixed_amount")
                    reward_string = "Giảm " + (self.DiscountFixedAmount ?? 0).ToString("n0") + " trên tổng tiền";
            }

            return reward_string;
        }

        public IEnumerable<SaleCouponProgram> _KeepOnlyMostInterestingAutoAppliedGlobalDiscountProgram(IEnumerable<SaleCouponProgram> self)
        {
            /* Given a record set of programs, remove the less interesting auto
                applied global discount to keep only the most interesting one.
                We should not take promo code programs into account as a 10% auto
                applied is considered better than a 50% promo code, as the user might
                not know about the promo code. */
            var programs = self.Where(x => _IsGlobalDiscountProgram(x) && x.PromoCodeUsage == "no_code_needed").ToList();
            if (!programs.Any())
                return self;
            var most_interesting_program = programs.OrderByDescending(x => x.DiscountPercentage).First();
            return self.Except(programs.Except(new List<SaleCouponProgram>() { most_interesting_program }));
        }

        public IEnumerable<SaleCouponProgram> _FilterProgramsFromCommonRules(IEnumerable<SaleCouponProgram> self, SaleOrder order, bool next_order = false)
        {
            var programs = self;
            if (!next_order)
                programs = _FilterOnMinimumAmount(programs, order);
            programs = _FilterOnValidityDates(programs, order);
            programs = _FilterUnexpiredPrograms(programs);
            var programs_curr_order = programs.Where(x => x.PromoApplicability == "on_current_order");
            programs = programs.Where(x => x.PromoApplicability == "on_next_order");
            if (programs_curr_order.Any())
                programs = programs.Concat(_FilterNotOrderedRewardPrograms(programs_curr_order, order));
            return programs;
        }

        public IEnumerable<SaleCouponProgram> _FilterNotOrderedRewardPrograms(IEnumerable<SaleCouponProgram> self, SaleOrder order)
        {
            var programs = new List<SaleCouponProgram>().AsEnumerable();
            foreach(var program in self)
            {
                if (program.RewardType == "product" && !order.OrderLines.Any(x => x.ProductId == program.RewardProductId))
                    continue;
                else if (program.RewardType == "discount" && program.DiscountType == "percentage" && program.DiscountApplyOn == "specific_products" &&
                    !order.OrderLines.Any(x => program.DiscountSpecificProducts.Any(s => s.ProductId == x.ProductId)))
                    continue;
                programs = programs.Union(new List<SaleCouponProgram>() { program });
            }

            return programs;
        }

        private IEnumerable<SaleCouponProgram> _FilterUnexpiredPrograms(IEnumerable<SaleCouponProgram> self)
        {
            var dict = _GetOrderCountDict(self);
            return self.Where(x => !x.MaximumUseNumber.HasValue || x.MaximumUseNumber == 0 || dict[x.Id] <= x.MaximumUseNumber);
        }

        private IEnumerable<SaleCouponProgram> _FilterOnValidityDates(IEnumerable<SaleCouponProgram> self, SaleOrder order)
        {
            return self.Where(x => (x.RuleDateFrom.HasValue && x.RuleDateTo.HasValue && x.RuleDateFrom <= order.DateOrder && x.RuleDateTo >= order.DateOrder) ||
            !x.RuleDateFrom.HasValue || x.RuleDateTo.HasValue).ToList();
        }

        public async Task<CheckPromoCodeMessage> _CheckPromoCode(SaleCouponProgram self, SaleOrder order, string coupon_code) 
        {
            var message = new CheckPromoCodeMessage();

            var saleObj = GetService<ISaleOrderService>();
            var applicable_programs = await saleObj._GetApplicablePrograms(order);
            var order_count = (await _GetOrderCountDictAsync(new List<SaleCouponProgram>() { self }))[self.Id];
            if (self.MaximumUseNumber != 0 && order_count >= self.MaximumUseNumber)
                message.Error = $"Mã khuyến mãi {coupon_code} đã hết hạn.";
            else if (!_FilterOnMinimumAmount(new List<SaleCouponProgram>() { self }, order).Any())
                message.Error = $"Nên mua hàng tối thiểu {self.RuleMinimumAmount} để có thể nhận thưởng";
            else if (!string.IsNullOrEmpty(self.PromoCode) && (order.Promotions.Any(x=> x.SaleCouponProgramId == self.Id)))
                message.Error = "Mã khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (string.IsNullOrEmpty(self.PromoCode) && order.NoCodePromoPrograms.Select(x => x.Program).Contains(self))
                message.Error = "Ưu đãi khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (!self.Active)
                message.Error = "Mã khuyến mãi không có giá trị";
            else if ((self.RuleDateFrom.HasValue && self.RuleDateFrom > order.DateOrder) ||
                (self.RuleDateTo.HasValue && self.RuleDateTo < order.DateOrder))
                message.Error = "Mã khuyến mãi đã hết hạn";
            else if (order.CodePromoProgram != null && !string.IsNullOrEmpty(order.CodePromoProgram.PromoCode) && self.PromoCodeUsage == "code_needed")
                message.Error = "Mã khuyến mãi không thể cộng dồn.";
            //else if (_IsGlobalDiscountProgram(self) && saleObj._IsGlobalDiscountAlreadyApplied(order))
            //    message.Error = "Chiết khấu tổng không thể cộng dồn";
            else if (self.PromoApplicability == "on_current_order" && self.RewardType == "product" && !saleObj._IsRewardInOrderLines(order, self))
                message.Error = "Sản phẩm thưởng nên có trong chi tiết đơn hàng.";
            else
            {
                if (!applicable_programs.Contains(self) && self.PromoApplicability == "on_current_order")
                    message.Error = "Không đạt điều kiện nào để có thể nhận thưởng!";
            }

            return message;
        }

        public async Task<CheckPromoCodeMessage> _CheckPromotion(SaleCouponProgram self, SaleOrder order)
        {
            var message = new CheckPromoCodeMessage();
            var saleObj = GetService<ISaleOrderService>();
            var applicable_programs = await saleObj._GetApplicablePrograms(order);
            var order_count = (await _GetOrderCountDictAsync(new List<SaleCouponProgram>() { self }))[self.Id];
            if ((self.RuleDateFrom.HasValue && self.RuleDateFrom > order.DateOrder) || (self.RuleDateTo.HasValue && self.RuleDateTo < order.DateOrder))
                message.Error = $"Chương trình khuyến mãi {self.Name} đã hết hạn.";
            else if (self.ProgramType != "promotion_program" || self.PromoCodeUsage == "code_needed" || self.DiscountApplyOn != "on_order")
                message.Error = "Khuyến mãi Không áp dụng cho đơn hàng";
            else if (!_FilterOnMinimumAmount(new List<SaleCouponProgram>() { self }, order).Any())
                message.Error = $"Nên mua hàng tối thiểu {self.RuleMinimumAmount} để có thể nhận thưởng";
            else if (!string.IsNullOrEmpty(self.PromoCode) && (order.Promotions.Any(x => x.SaleCouponProgramId == self.Id)))
                message.Error = "Chương trình khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (string.IsNullOrEmpty(self.PromoCode) && order.NoCodePromoPrograms.Select(x => x.Program).Contains(self))
                message.Error = "Ưu đãi khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (!self.Active)
                message.Error = "Chương trình khuyến mãi không có giá trị";
            //else if (order.CodePromoProgram != null && !string.IsNullOrEmpty(order.CodePromoProgram.PromoCode) && self.PromoCodeUsage == "code_needed")
            //    message.Error = "Mã khuyến mãi không thể cộng dồn.";
            //else if (_IsGlobalDiscountProgram(self) && saleObj._IsGlobalDiscountAlreadyApplied(order))
            //    message.Error = "Chiết khấu tổng không thể cộng dồn";
            else if (self.PromoApplicability == "on_current_order" && self.RewardType == "product" && !saleObj._IsRewardInOrderLines(order, self))
                message.Error = "Sản phẩm thưởng nên có trong chi tiết đơn hàng.";
            else
            {
                if (!applicable_programs.Contains(self) && self.PromoApplicability == "on_current_order")
                    message.Error = "Không đạt điều kiện nào để có thể nhận thưởng!";
            }

            return message;
        }
        //public _FilterProgramsOnProducts(IEnumerable<SaleCouponProgram> self, SaleOrder order)
        //{
        //    //To get valid programs according to product list.
        //    //i.e Buy 1 imac + get 1 ipad mini free then check 1 imac is on cart or not
        //    //or  Buy 1 coke + get 1 coke free then check 2 cokes are on cart or not
        //    var saleObj = GetService<ISaleOrderService>();
        //    var order_lines = order.OrderLines.Where(x => x.ProductId.HasValue).Except(saleObj._GetRewardLines(order));
        //    var product
        //    var products_qties = order_lines.GroupBy(x => x.ProductId.Value).ToDictionary(x => x.Key, x => x.Sum(s => s.ProductUOMQty));
        //    foreach(var program in self)
        //    {
        //        var ordered_rule_products_qty = products_qties
        //    }
        //}

        public bool _IsGlobalDiscountProgram(SaleCouponProgram self)
        {
           
            return self.PromoApplicability == "on_current_order" && self.RewardType == "discount"
                && self.DiscountType == "percentage" && self.DiscountApplyOn == "on_order";
        }

        public IEnumerable<SaleCouponProgram> _FilterOnMinimumAmount(IEnumerable<SaleCouponProgram> self, SaleOrder order)
        {
            var untaxed_amount = order.AmountUntaxed;
            //Some lines should not be considered when checking if threshold is met like delivery
            return self.Where(x => x.RuleMinimumAmount <= untaxed_amount);
        }

        public IEnumerable<SaleCouponProgram> _FilterOnMinimumAmount(SaleCouponProgram self, SaleOrder order)
        {
            return _FilterOnMinimumAmount(new List<SaleCouponProgram>() { self }, order);
        }

        private async Task<Product> GetOrCreateDiscountProduct(SaleCouponProgram self)
        {
            var productObj = GetService<IProductService>();
            var reward_string = await GetRewardDisplayName(self);
            var categObj = GetService<IProductCategoryService>();
            var categ = await categObj.DefaultCategory();
            var uomObj = GetService<IUoMService>();
            var uom = await uomObj.SearchQuery().FirstOrDefaultAsync();

            var product = new Product()
            {
                PurchaseOK = false,
                SaleOK = false,
                Type = "service",
                Name = reward_string,
                UOMId = uom.Id,
                UOMPOId = uom.Id,
                CategId = categ.Id,
                ListPrice = 0,
            };
            await productObj.CreateAsync(product);

            return product;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            if (self.Any(x => x.Active))
                throw new Exception("Bạn không thể xóa chương trình đang hoạt động");
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

        public async Task ActionArchive(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var program in self)
            {
                if (program.Active == true)
                    program.Active = false;
            }

            await UpdateAsync(self);
        }

        public async Task ActionUnArchive(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var program in self)
            {
                if (program.Active == false)
                    program.Active = true;
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
                    Code = StringUtils.RandomStringDigit(13),
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
                            Code = StringUtils.RandomStringDigit(13),
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

        public async Task<IDictionary<Guid, int>> _GetOrderCountDictAsync(IEnumerable<SaleCouponProgram> self)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var discountLineProductIds = self.Where(x => x.DiscountLineProductId.HasValue).Select(x => x.DiscountLineProductId.Value).ToList();
            var mapped_data = await saleLineObj.SearchQuery(x => x.ProductId.HasValue && discountLineProductIds.Contains(x.ProductId.Value)).GroupBy(x => x.ProductId.Value)
                .Select(x => new
                {
                    ProductId = x.Key,
                    ProductIdCount = x.Count()
                }).ToDictionaryAsync(x => x.ProductId, x => x.ProductIdCount);
            var dict = self.ToDictionary(x => x.Id, x => 0);
            foreach(var program in self)
            {
                if (program.DiscountLineProductId.HasValue)
                    dict[program.Id] = mapped_data.ContainsKey(program.DiscountLineProductId.Value) ?
                        mapped_data[program.DiscountLineProductId.Value] : 0;
            }
            return dict;
        }

        public async Task<int> _GetOrderCountAsync(Guid id)
        {
            var discount_line_product_id = await SearchQuery(x => x.Id == id).Select(x => x.DiscountLineProductId).FirstOrDefaultAsync();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var res = await saleLineObj.SearchQuery(x => x.ProductId == discount_line_product_id)
                .Select(x => x.OrderId).Distinct().CountAsync();
            return res;
        }

        public IDictionary<Guid, int> _GetOrderCountDict(IEnumerable<SaleCouponProgram> self)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var discountLineProductIds = self.Where(x => x.DiscountLineProductId.HasValue).Select(x => x.DiscountLineProductId.Value).ToList();
            var mapped_data = saleLineObj.SearchQuery(x => x.ProductId.HasValue && discountLineProductIds.Contains(x.ProductId.Value)).GroupBy(x => x.ProductId.Value)
                .Select(x => new
                {
                    ProductId = x.Key,
                    //OrderIdCount = x.Select(s => s.OrderId).Distinct().Count(),
                    //ProductIdCount = x.Select(s => s.OrderId).Distinct().Count(),
                    ProductIdCount = x.Count()
                }).ToDictionary(x => x.ProductId, x => x.ProductIdCount);
            var dict = self.ToDictionary(x => x.Id, x => 0);
            foreach (var program in self)
            {
                if (program.DiscountLineProductId.HasValue)
                    dict[program.Id] = mapped_data.ContainsKey(program.DiscountLineProductId.Value) ?
                        mapped_data[program.DiscountLineProductId.Value] : 0;
            }
            return dict;
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

    public class CheckPromoCodeMessage
    {
        public string Error { get; set; }
    }
}
