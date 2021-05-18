using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleCouponProgramService : BaseService<SaleCouponProgram>, ISaleCouponProgramService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        public SaleCouponProgramService(IAsyncRepository<SaleCouponProgram> repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            AppTenant tenant)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant;
        }

        public async Task<PagedResult2<SaleCouponProgramGetListPagedResponse>> GetListPaged(SaleCouponProgramGetListPagedRequest val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }
            if (!string.IsNullOrEmpty(val.ProgramType))
            {
                query = query.Where(x => x.ProgramType == val.ProgramType);
            }
            if (val.Active.HasValue)
            {
                query = query.Where(x => x.Active == val.Active);
            }
            if (!string.IsNullOrEmpty(val.Status))
            {
                var now = DateTime.Today;
                if (val.Status == "waiting")
                {
                    query = query.Where(x => x.RuleDateFrom > now);
                }
                if (val.Status == "paused")
                {
                    query = query.Where(x => now <= x.RuleDateTo && x.IsPaused);
                }
                if (val.Status == "running")
                {
                    query = query.Where(x => now >= x.RuleDateFrom && now <= x.RuleDateTo && !x.IsPaused);
                }
                if (val.Status == "expired")
                {
                    query = query.Where(x => now > x.RuleDateTo);
                }
            }

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var totalItems = await query.CountAsync();

            var programIds = await query.Select(x => x.Id).ToListAsync();
            var saleProgramAmountDict = await GetAmountPromotionDictAsync(programIds);

            var itemsResponse = _mapper.Map<List<SaleCouponProgramGetListPagedResponse>>(items);
            itemsResponse.ForEach(x => {
                if (saleProgramAmountDict.ContainsKey(x.Id))
                {
                    x.AmountTotal = saleProgramAmountDict[x.Id];
                }
            });

            var paged = new PagedResult2<SaleCouponProgramGetListPagedResponse>(totalItems, val.Offset, val.Limit)
            {
                Items = itemsResponse
            };

            return paged;
        }

        public async Task<IDictionary<Guid, decimal>> GetAmountPromotionDictAsync(IEnumerable<Guid> ids)
        {
            var promotionObj = GetService<ISaleOrderPromotionService>();
            var saleProgramAmountDict = await promotionObj.SearchQuery(x => ids.Contains(x.SaleCouponProgramId.Value))
                .GroupBy(x => x.SaleCouponProgramId)
                .Select(x => new { SaleCouponProgramId = x.Key, Amount = x.Sum(y => y.Amount) })
                .ToDictionaryAsync(x => x.SaleCouponProgramId.Value, x => x.Amount);

            return (IDictionary<Guid, decimal>)saleProgramAmountDict;
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
            if (!string.IsNullOrEmpty(val.Status))
            {
                var now = DateTime.Today;
                if (val.Status == "waiting")
                    spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => x.RuleDateFrom > now));
                if (val.Status == "paused")
                    spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => now <= x.RuleDateTo && x.IsPaused));
                if (val.Status == "running")
                    spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => now >= x.RuleDateFrom && now <= x.RuleDateTo && !x.IsPaused));
                if (val.Status == "expired")
                    spec = spec.And(new InitialSpecification<SaleCouponProgram>(x => now > x.RuleDateTo));
            }

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

        public async Task<IEnumerable<SaleCouponProgramBasic>> GetPromotionBySaleOrder()
        {
            var today = DateTime.Today;
            var promotions = await SearchQuery(x => x.Active && !x.IsPaused && x.ProgramType == "promotion_program"
            && x.PromoCodeUsage == "no_code_needed" && x.DiscountApplyOn == "on_order"
            && (!x.RuleDateFrom.HasValue || today >= x.RuleDateFrom.Value) && (!x.RuleDateTo.HasValue || today <= x.RuleDateTo.Value)
            && (string.IsNullOrEmpty(x.Days) || x.Days.Contains(((int)today.DayOfWeek).ToString()))
            ).ToListAsync();

            var basics = _mapper.Map<IEnumerable<SaleCouponProgramBasic>>(promotions);
            return basics;
        }

        public async Task<IEnumerable<SaleCouponProgramBasic>> GetPromotionBySaleOrderLine(Guid productId)
        {
            var today = DateTime.Today;
            var productObj = GetService<IProductService>();
            var product = await productObj.SearchQuery(x => x.Id == productId).FirstOrDefaultAsync();
            var promotions = await SearchQuery(x => x.Active && !x.IsPaused && x.ProgramType == "promotion_program"
            && x.PromoCodeUsage == "no_code_needed" && (x.DiscountApplyOn == "specific_products" || x.DiscountApplyOn == "specific_product_categories")
            && (!x.RuleDateFrom.HasValue || today >= x.RuleDateFrom.Value) && (!x.RuleDateTo.HasValue || today <= x.RuleDateTo.Value)
            && (x.DiscountSpecificProducts.Any(s => s.ProductId == product.Id) || x.DiscountSpecificProductCategories.Any(s => s.ProductCategoryId == product.CategId))
            && (string.IsNullOrEmpty(x.Days) || x.Days.Contains(((int)today.DayOfWeek).ToString())))
            .ToListAsync();

            var basics = _mapper.Map<IEnumerable<SaleCouponProgramBasic>>(promotions);
            return basics;
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

            if (program.PromoCodeUsage == "code_needed" && string.IsNullOrEmpty(program.PromoCode))
            {
                //SaleCouponProgram checkExist = null;
                //do
                //{
                //    program.PromoCode = GeneratePromoCode();
                //    checkExist = await SearchQuery(x => x.PromoCodeUsage == "code_needed" && x.PromoCode == program.PromoCode).FirstOrDefaultAsync();
                //} while (checkExist != null);
                program.PromoCode = await GeneratePromoCodeIfEmpty();
            }

            program.RuleDateFrom = program.RuleDateFrom.Value.AbsoluteBeginOfDate();
            program.RuleDateTo = program.RuleDateTo.Value.AbsoluteBeginOfDate();

            if (!program.CompanyId.HasValue)
            program.CompanyId = CompanyId;

            if (program.DiscountApplyOn == "specific_product_categories")
            {
                SaveDiscountSpecificProductCategories(program, val);
            }
            else
            {
                program.DiscountSpecificProductCategories.Clear();
            }

            if (program.DiscountApplyOn == "specific_products")
            {
                SaveDiscountSpecificProducts(program, val);
            }
            else
            {
                program.DiscountSpecificProducts.Clear();
            }

            //if (!program.DiscountLineProductId.HasValue)
            //{
            //    var discountProduct = await GetOrCreateDiscountProduct(program);
            //    program.DiscountLineProductId = discountProduct.Id;
            //}

            _CheckRuleDate(program);
            _CheckDiscountPercentage(program);
            _CheckRuleMinimumAmount(program);
            _CheckRuleMinQuantity(program);

            if (program.PromoCodeUsage == "code_needed" && !string.IsNullOrEmpty(program.PromoCode))
            {
                await CheckAndUpdatePromoCode(program.PromoCode);
            }
            await CreateAsync(program);
            await _CheckPromoCodeConstraint(program);
            return program;
        }

        public void _CheckDiscountPercentage(SaleCouponProgram self)
        {
            if (self.DiscountType == "percentage" && (self.DiscountPercentage < 0 || self.DiscountPercentage > 100))
                throw new Exception("Chiết khấu phần trăm phải từ 0-100");
        }

        public void _CheckRuleDate(SaleCouponProgram self)
        {
            if (self.RuleDateFrom > self.RuleDateTo)
                throw new Exception("Ngày kết thúc đang nhỏ hơn ngày bắt đầu!");
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
                .Include(x => x.DiscountSpecificProducts)
                .Include(x => x.DiscountSpecificProductCategories)
                .FirstOrDefaultAsync();

            program = _mapper.Map(val, program);

            if (program.PromoCodeUsage == "code_needed" && string.IsNullOrEmpty(program.PromoCode))
            {
                //SaleCouponProgram checkExist = null;
                //do
                //{
                //    program.PromoCode = GeneratePromoCode();
                //    checkExist = await SearchQuery(x => x.PromoCodeUsage == "code_needed" && x.PromoCode == program.PromoCode).FirstOrDefaultAsync();
                //} while (checkExist != null);
                await GeneratePromoCodeIfEmpty();
            }

            program.RuleDateFrom = program.RuleDateFrom.Value.AbsoluteBeginOfDate();
            program.RuleDateTo = program.RuleDateTo.Value.AbsoluteBeginOfDate();

            //if (!program.DiscountLineProductId.HasValue)
            //{
            //    var discountProduct = await GetOrCreateDiscountProduct(program);
            //    program.DiscountLineProduct = discountProduct;
            //    program.DiscountLineProductId = discountProduct.Id;
            //}

            if (!program.CompanyId.HasValue)
                program.CompanyId = CompanyId;

            if (program.DiscountApplyOn == "specific_product_categories")
            {
                SaveDiscountSpecificProductCategories(program, val);
            } 
            else
            {
                program.DiscountSpecificProductCategories.Clear();
            }

            if (program.DiscountApplyOn == "specific_products")
            {
                SaveDiscountSpecificProducts(program, val);
            }
            else
            {
                program.DiscountSpecificProducts.Clear();
            }

            //var reward_name = await GetRewardDisplayName(program);
            //if (program.DiscountLineProduct != null)
            //{
            //    var productObj = GetService<IProductService>();
            //    program.DiscountLineProduct.Name = reward_name;
            //    program.DiscountLineProduct.NameNoSign = StringUtils.RemoveSignVietnameseV2(reward_name);
            //    await productObj.UpdateAsync(program.DiscountLineProduct);
            //}

            _CheckRuleDate(program);
            _CheckDiscountPercentage(program);
            _CheckRuleMinimumAmount(program);
            _CheckRuleMinQuantity(program);
            if (program.PromoCodeUsage == "code_needed" && !string.IsNullOrEmpty(program.PromoCode))
            {
                await CheckAndUpdatePromoCode(program.PromoCode);
            }
            await UpdateAsync(program);
            await _CheckPromoCodeConstraint(program);

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

        private void SaveDiscountSpecificProducts(SaleCouponProgram program, SaleCouponProgramSave val)
        {
            var productObj = GetService<IProductService>();
            var to_remove = program.DiscountSpecificProducts.Where(x => !val.DiscountSpecificProductIds.Contains(x.ProductId)).ToList();
            foreach (var item in to_remove)
                program.DiscountSpecificProducts.Remove(item);

            var to_add = val.DiscountSpecificProductIds.Where(x => !program.DiscountSpecificProducts.Any(s => s.ProductId == x)).ToList();
            foreach (var productId in to_add)
            {
                program.DiscountSpecificProducts.Add(new SaleCouponProgramProductRel { ProductId = productId });
            }
        }

        private void SaveDiscountSpecificProductCategories(SaleCouponProgram program, SaleCouponProgramSave val)
        {
            var productCategoryObj = GetService<IProductCategoryService>();
            var to_remove = program.DiscountSpecificProductCategories.Where(x => !val.DiscountSpecificProductCategoryIds.Contains(x.ProductCategoryId)).ToList();
            foreach (var item in to_remove)
                program.DiscountSpecificProductCategories.Remove(item);
            var to_add = val.DiscountSpecificProductCategoryIds.Where(x => !program.DiscountSpecificProductCategories.Any(s => s.ProductCategoryId == x)).ToList();
            foreach (var productCategoryId in to_add)
            {
                program.DiscountSpecificProductCategories.Add(new SaleCouponProgramProductCategoryRel { ProductCategoryId = productCategoryId });
            }
        }

        public async Task _CheckPromoCodeConstraint(SaleCouponProgram self)
        {
            //if (self.PromoCodeUsage == "code_needed" && string.IsNullOrEmpty(self.PromoCode))
            //    throw new Exception("Vui lòng nhập mã khuyến mãi!");
            var exist = await SearchQuery(x => x.PromoCodeUsage == "code_needed" && x.PromoCode == self.PromoCode).CountAsync();
            if (exist > 1)
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
            foreach (var program in self)
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
            var countApplied = await _GetCountAppliedAsync(self);
            if (self.MaximumUseNumber != 0 && countApplied >= self.MaximumUseNumber)
                message.Error = $"Mã khuyến mãi {coupon_code} vượt quá hạn mức áp dụng.";
            if ((self.RuleDateFrom.HasValue && self.RuleDateFrom.Value > order.DateOrder) || (self.RuleDateTo.HasValue && self.RuleDateTo.Value < order.DateOrder))
                message.Error = $"Chương trình khuyến mãi {self.Name} đã hết hạn.";
            else if (!_FilterOnMinimumAmount(new List<SaleCouponProgram>() { self }, order).Any() && self.DiscountApplyOn == "on_order")
                message.Error = $"Nên mua hàng tối thiểu {self.RuleMinimumAmount} để có thể nhận thưởng";
            else if (!string.IsNullOrEmpty(self.PromoCode) && (order.Promotions.Any(x => x.SaleCouponProgramId == self.Id)))
                message.Error = "Mã khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (string.IsNullOrEmpty(self.PromoCode) && order.NoCodePromoPrograms.Select(x => x.Program).Contains(self))
                message.Error = "Ưu đãi khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (!self.Active)
                message.Error = "Mã khuyến mãi không có giá trị";
            else if ((self.NotIncremental.HasValue && self.NotIncremental.Value) && order.Promotions.Where(x => !x.SaleOrderLineId.HasValue && x.SaleCouponProgramId.HasValue).Any() || saleObj._IsGlobalDiscountAlreadyApplied(order))
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
            var countApplied = await _GetCountAppliedAsync(self);
            if (self.MaximumUseNumber != 0 && countApplied >= self.MaximumUseNumber)
                message.Error = $"Chương trình khuyến mãi vượt quá hạn mức áp dụng.";
            if ((self.RuleDateFrom.HasValue && self.RuleDateFrom.Value > order.DateOrder) || (self.RuleDateTo.HasValue && self.RuleDateTo.Value < order.DateOrder))
                message.Error = $"Chương trình khuyến mãi {self.Name} đã hết hạn.";
            else if (self.ProgramType != "promotion_program" || self.PromoCodeUsage == "code_needed" || self.DiscountApplyOn != "on_order")
                message.Error = "Khuyến mãi Không áp dụng cho đơn hàng";
            else if (!_FilterOnMinimumAmount(new List<SaleCouponProgram>() { self }, order).Any() && self.DiscountApplyOn == "on_order")
                message.Error = $"Nên mua hàng tối thiểu {self.RuleMinimumAmount} để có thể nhận thưởng";
            else if ((order.Promotions.Any(x => x.SaleCouponProgramId == self.Id)))
                message.Error = "Chương trình khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (self.IsPaused)
                message.Error = "Chương trình khuyến mãi đang tạm ngừng";
            else if (!self.Active)
                message.Error = "Chương trình khuyến mãi không có giá trị";
            else if ((self.NotIncremental.HasValue && self.NotIncremental.Value) && order.Promotions.Where(x => !x.SaleOrderLineId.HasValue && x.SaleCouponProgramId.HasValue).Any() || saleObj._IsGlobalDiscountAlreadyApplied(order))
                message.Error = "Chiết khấu tổng không thể cộng dồn";
            else if (self.PromoApplicability == "on_current_order" && self.RewardType == "product" && !saleObj._IsRewardInOrderLines(order, self))
                message.Error = "Sản phẩm thưởng nên có trong chi tiết đơn hàng.";
            else
            {
                if (!applicable_programs.Contains(self) && self.PromoApplicability == "on_current_order")
                    message.Error = "Không đạt điều kiện nào để có thể nhận thưởng!";
            }

            return message;
        }

        public async Task<CheckPromoCodeMessage> _CheckPromotionApplySaleLine(SaleCouponProgram self, SaleOrderLine line)
        {
            var message = new CheckPromoCodeMessage();
            var saleLineObj = GetService<ISaleOrderLineService>();
            //var applicable_programs = await saleObj._GetApplicablePrograms(order);
            var countApplied = await _GetCountAppliedAsync(self);
            if (self.MaximumUseNumber != 0 && countApplied >= self.MaximumUseNumber)
                message.Error = $"Chương trình khuyến mãi vượt quá hạn mức áp dụng.";
            if ((self.RuleDateFrom.HasValue && self.RuleDateFrom.Value > line.Order.DateOrder) || (self.RuleDateTo.HasValue && self.RuleDateTo.Value < line.Order.DateOrder))
                message.Error = $"Chương trình khuyến mãi {self.Name} đã hết hạn.";
            else if ((self.DiscountSpecificProducts.Any() && !self.DiscountSpecificProducts.Any(x => x.ProductId == line.ProductId)))                                                                                                                                                                                
                message.Error = "Khuyến mãi Không áp dụng cho dịch vụ này";
            else if (line.Order.Promotions.Where(x => x.SaleOrderId.HasValue && !x.SaleOrderLineId.HasValue).Any(x => x.SaleCouponProgramId == self.Id))
                message.Error = "Chương trình khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (line.Promotions.Any(x => x.SaleCouponProgramId == self.Id))
                message.Error = "Chương trình khuyến mãi đã được áp dụng cho dịch vụ này";
            else if (line.Order.NoCodePromoPrograms.Select(x => x.Program).Contains(self))
                message.Error = "Ưu đãi khuyến mãi đã được áp dụng cho đơn hàng này";
            else if (self.IsPaused)
                message.Error = "Chương trình khuyến mãi đang tạm ngừng";
            else if (!self.Active)
                message.Error = "Chương trình khuyến mãi không có giá trị";
            else if ((self.NotIncremental.HasValue && self.NotIncremental.Value && line.Promotions.Where(x => x.SaleOrderId.HasValue && x.SaleCouponProgramId.HasValue).Any()) || line.Promotions.Where(x => !x.SaleOrderLineId.HasValue && x.SaleCouponProgramId.HasValue && x.SaleCouponProgram.NotIncremental.Value).Any())
                message.Error = "Chiết khấu tổng không thể cộng dồn";
            //else
            //{
            //    if (!applicable_programs.Contains(self) && self.PromoApplicability == "on_current_order")
            //        message.Error = "Không đạt điều kiện nào để có thể nhận thưởng!";
            //}

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

        public async Task<SaleCouponProgramResponse> GetPromotionDisplayUsageCode(string code, Guid? productId)
        {
            var now = DateTime.Now;
            //Chương trình khuyến mãi sử dụng mã
            var program = await SearchQuery(x => x.PromoCode == code).Include(x => x.DiscountSpecificProducts).FirstOrDefaultAsync();
            if (program == null)
                return new SaleCouponProgramResponse { Error = "Mã chương trình khuyến mãi không tồn tại", Success = false, SaleCouponProgram = _mapper.Map<SaleCouponProgramDisplay>(program) };

            var res = new SaleCouponProgramResponse();
            res.SaleCouponProgram = _mapper.Map<SaleCouponProgramDisplay>(program);
            res.Success = true;
            if (!program.Active)
            {
                res.Error = "Mã khuyến mãi không có giá trị";
                res.Success = false;
                res.SaleCouponProgram = null;
            }
            else if (!productId.HasValue && program.DiscountApplyOn == "specific_products")
            {
                res.Error = "Khuyến mãi không áp dụng cho đơn hàng";
                res.Success = false;
                res.SaleCouponProgram = null;
            }
            else if (productId.HasValue && !program.DiscountSpecificProducts.Any(x => x.ProductId == productId))
            {
                res.Error = "Mã khuyến mãi không áp dụng cho dịch vụ này";
                res.Success = false;
                res.SaleCouponProgram = null;
            }
            else if ((program.RuleDateFrom.HasValue && program.RuleDateFrom > now) || (program.RuleDateTo.HasValue && program.RuleDateTo < now))
            {
                res.Error = "Mã khuyến mãi đã hết hạn";
                res.Success = false;
                res.SaleCouponProgram = null;
            }


            return res;
        }

        public async Task<decimal> GetAmountTotal(Guid id)
        {
            ///lay tu moveline journal amount advance used
            var promotionObj = GetService<ISaleOrderPromotionService>();
            var amounAdvance = await promotionObj.SearchQuery(x => x.SaleCouponProgramId == id).Select(x => x.Amount).SumAsync();
            return Math.Round(amounAdvance);
        }



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
                    foreach (var coupon in coupons)
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
            var now = DateTime.Today;
            foreach (var program in self)
            {
                if (program.RuleDateTo >= now)
                {
                    program.IsPaused = true;
                }
            }

            await UpdateAsync(self);
        }

        public async Task ActionUnArchive(IEnumerable<Guid> ids)
        {
            DateTime today = DateTime.UtcNow.Date;

            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var program in self)
            {
                program.Active = true;
                program.IsPaused = false;
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

            var product_discount_line = rule.DiscountLineProduct;
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

        public async Task<int> _GetCountAppliedAsync(SaleCouponProgram self)
        {
            var orderPromotionObj = GetService<ISaleOrderPromotionService>();
            var countApplied = await orderPromotionObj.SearchQuery(x => x.SaleCouponProgramId.HasValue && x.SaleCouponProgramId == self.Id).CountAsync();
            //var discountLineProductIds = self.Where(x => x.DiscountLineProductId.HasValue).Select(x => x.DiscountLineProductId.Value).ToList();
            //var mapped_data = await saleLineObj.SearchQuery(x => x.ProductId.HasValue && discountLineProductIds.Contains(x.ProductId.Value)).GroupBy(x => x.ProductId.Value)
            //    .Select(x => new
            //    {
            //        ProductId = x.Key,
            //        ProductIdCount = x.Count()
            //    }).ToDictionaryAsync(x => x.ProductId, x => x.ProductIdCount);
            //var dict = self.ToDictionary(x => x.Id, x => 0);
            //foreach (var program in self)
            //{
            //    if (program.DiscountLineProductId.HasValue)
            //        dict[program.Id] = mapped_data.ContainsKey(program.DiscountLineProductId.Value) ?
            //            mapped_data[program.DiscountLineProductId.Value] : 0;
            //}
            return countApplied;
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

        private string GeneratePromoCode()
        {
            Random _random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var month = DateTime.Now.ToString("MM");
            var year = DateTime.Now.ToString("yyyy");
            var code = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[_random.Next(s.Length)]).ToArray()) + month + year;
            return code;
        }

        private async Task<string> GeneratePromoCodeIfEmpty(string type = "promotion.code")
        {
            var sequenceObj = GetService<IIRSequenceService>();
            var promotionCode = await sequenceObj.NextByCode(type);
            if (string.IsNullOrEmpty(promotionCode) || promotionCode == "/")
            {
                await _InsertPromotionCodeSequence();
                promotionCode = await sequenceObj.NextByCode("promotion.code");
            }

            return promotionCode;
        }

        private async Task CheckAndUpdatePromoCode(string code)
        {
            string pattern = @"^CTKM\d{4}$";
            Regex rg = new Regex(pattern, RegexOptions.IgnoreCase);
            Regex rgx = new Regex(@"CTKM", RegexOptions.IgnoreCase);
            var isMatching = rg.IsMatch(code);
            if (isMatching)
            {
                var m = rg.Match(code);
                var result = rgx.Split(code, 2, m.Index);
                var numberCode = result[1];
                int number = Int32.Parse(numberCode.ToString());
                var sequenceObj =  GetService<IIRSequenceService>();                                                
                var sequence = await sequenceObj.SearchQuery(x => x.Code == "promotion.code").FirstOrDefaultAsync();
                if(sequence == null)
                {
                    sequence = await sequenceObj.CreateAsync(new IRSequence
                    {
                        Name = "Mã khuyến mãi",
                        Code = "promotion.code",
                        Prefix = "CTKM/",
                        Padding = 4
                    });
                }                

                if (number > sequence.NumberNext)
                {
                    sequence.NumberNext = number + sequence.NumberIncrement;
                    await sequenceObj.UpdateAsync(sequence);
                }
            }
            
        }

        private async Task _InsertPromotionCodeSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Mã khuyến mãi",
                Code = "promotion.code",
                Prefix = "CTKM/",
                Padding = 4
            });
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
