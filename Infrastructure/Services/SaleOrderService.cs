﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ApplicationCore.Utilities;

namespace Infrastructure.Services
{
    public class SaleOrderService : BaseService<SaleOrder>, ISaleOrderService
    {
        private readonly IMapper _mapper;

        public SaleOrderService(IAsyncRepository<SaleOrder> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<SaleOrder> CreateOrderAsync(SaleOrder order)
        {
            if (string.IsNullOrEmpty(order.Name) || order.Name == "/")
            {
                var sequenceService = GetService<IIRSequenceService>();
                if (order.IsQuotation == true)
                {
                    order.Name = await sequenceService.NextByCode("sale.quotation");
                    if (string.IsNullOrEmpty(order.Name))
                    {
                        await InsertSaleQuotationSequence();
                        order.Name = await sequenceService.NextByCode("sale.quotation");
                    }
                }
                else
                    order.Name = await sequenceService.NextByCode("sale.order");
            }

            var saleLineService = (ISaleOrderLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ISaleOrderLineService));
            saleLineService.UpdateOrderInfo(order.OrderLines, order);
            saleLineService.ComputeAmount(order.OrderLines);
            saleLineService._GetInvoiceQty(order.OrderLines);
            saleLineService._GetToInvoiceQty(order.OrderLines);
            saleLineService._ComputeInvoiceStatus(order.OrderLines);

            _ComputeResidual(new List<SaleOrder>() { order });
            _AmountAll(order);

            return await CreateAsync(order);
        }

        public IEnumerable<SaleOrderLine> _GetRewardLines(SaleOrder self)
        {
            return self.OrderLines.Where(x => x.IsRewardLine);
        }

        public async Task RecomputeResidual(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).Include("OrderLines.SaleOrderLineInvoiceRels")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice").ToListAsync();
            _ComputeResidual(self);
            await UpdateAsync(self);
        }

        public bool _IsRewardInOrderLines(SaleOrder self, SaleCouponProgram program)
        {
            return self.OrderLines.Where(x => x.ProductId == program.RewardProductId &&
            x.ProductUOMQty >= program.RewardProductQuantity).Any();
        }

        private async Task InsertSaleQuotationSequence()
        {
            var sequenceObj = GetService<IIRSequenceService>();
            await sequenceObj.CreateAsync(new IRSequence
            {
                Name = "Sale Quotation",
                Prefix = "SQ",
                Code = "sale.quotation",
                Padding = 5
            });
        }

        private void _AmountAll(SaleOrder order)
        {
            var totalAmountUntaxed = 0M;
            var totalAmountTax = 0M;

            foreach (var line in order.OrderLines)
            {
                totalAmountUntaxed += line.PriceSubTotal;
                totalAmountTax += line.PriceTax;
            }
            order.AmountTax = Math.Round(totalAmountTax);
            order.AmountUntaxed = Math.Round(totalAmountUntaxed);
            order.AmountTotal = order.AmountTax + order.AmountUntaxed;
        }

        public async Task<PagedResult<SaleOrder>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "")
        {
            Expression<Func<SaleOrder, object>> sort = null;
            switch (orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new SaleOrderFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new SaleOrderFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await base.ListAsync(filterPaginatedSpecification);
            var totalItems = await base.CountAsync(filterSpecification);

            return new PagedResult<SaleOrder>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<SaleOrderBasic>> GetPagedResultAsync(SaleOrderPaged val)
        {
            ISpecification<SaleOrder> spec = new InitialSpecification<SaleOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));
            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.PartnerId == val.PartnerId));
            if (val.DateOrderFrom.HasValue)
            {
                var dateFrom = val.DateOrderFrom.Value.AbsoluteBeginOfDate();
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.DateOrder >= dateFrom));
            }
            if (val.DateOrderTo.HasValue)
            {
                var dateTo = val.DateOrderTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.DateOrder <= dateTo));
            }
            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                spec = spec.And(new InitialSpecification<SaleOrder>(x => states.Contains(x.State)));
            }

            if (val.IsQuotation.HasValue)
                spec = spec.And(new InitialSpecification<SaleOrder>(x => (!x.IsQuotation.HasValue && val.IsQuotation == false) || x.IsQuotation == val.IsQuotation));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<SaleOrderBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include(x => x.DotKhams)
                .Include("OrderLines.Order")
                .Include("OrderLines.SaleOrderLineInvoiceRels")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
                .ToListAsync();

            var saleLineObj = GetService<ISaleOrderLineService>();
            var invoiceIds = new List<Guid>().AsEnumerable();
            var dotKhamIds = new List<Guid>().AsEnumerable();
            foreach (var sale in self)
            {
                if (sale.DotKhams.Any())
                    throw new Exception("Không thể hủy phiếu điều trị đã có đợt khám.");
                foreach (var line in sale.OrderLines)
                {
                    invoiceIds = invoiceIds.Union(line.SaleOrderLineInvoiceRels.Select(x => x.InvoiceLine.Invoice.Id).Distinct().ToList());
                }

                dotKhamIds = dotKhamIds.Union(sale.DotKhams.Select(x => x.Id).ToList());
            }

            await UpdateAsync(self);

            if (invoiceIds.Any())
            {
                var invObj = GetService<IAccountInvoiceService>();
                await invObj.ActionCancel(invoiceIds);
            }

            if (dotKhamIds.Any())
            {
                var dkObj = GetService<IDotKhamService>();
                await dkObj.ActionCancel(dotKhamIds);
            }

            foreach (var sale in self)
            {
                foreach (var line in sale.OrderLines)
                {
                    line.State = "cancel";
                }

                saleLineObj._GetInvoiceQty(sale.OrderLines);
                saleLineObj._GetToInvoiceQty(sale.OrderLines);
                saleLineObj._ComputeInvoiceStatus(sale.OrderLines);

                sale.State = "cancel";
                _GetInvoiced(new List<SaleOrder>() { sale });
            }

            await UpdateAsync(self);
        }

        public async Task ActionConvertToOrder(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
                .Include("OrderLines.SaleOrderLineToothRels").FirstOrDefaultAsync();
            if (self.IsQuotation != true)
                throw new Exception("Chỉ có phiếu tư vấn mới có thể tạo phiếu điều trị");
            var seqObj = GetService<IIRSequenceService>();
            var order = new SaleOrder(self);
            order.Name = await seqObj.NextByCode("sale.order");
            order.QuoteId = id;

            foreach(var line in self.OrderLines)
            {
                var l = new SaleOrderLine(line);
                l.OrderId = order.Id;
                foreach(var rel in line.SaleOrderLineToothRels)
                {
                    var r = new SaleOrderLineToothRel
                    {
                        ToothId = rel.ToothId
                    };
                    l.SaleOrderLineToothRels.Add(r);
                }
                order.OrderLines.Add(l);
            }

            await CreateAsync(order);

            self.OrderId = order.Id;
            await UpdateAsync(self);
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var cardObj = GetService<ICardCardService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).Include(x => x.Card).Include(x => x.Card.Type)
                .ToListAsync();

            foreach (var sale in self)
            {
                foreach (var line in sale.OrderLines)
                {
                    line.State = "done";
                }

                sale.State = "done";

                var card = await cardObj.GetValidCard(sale.PartnerId);
                if (card == null)
                    continue;
                var points = await cardObj.ConvertAmountToPoint(sale.AmountTotal);
                card.TotalPoint = (card.TotalPoint ?? 0) + points;
                card.PointInPeriod = (card.PointInPeriod ?? 0) + points;
                await cardObj.UpdateAsync(card);
            }

            await UpdateAsync(self);
        }

        public async Task ApplyCoupon(SaleOrderApplyCoupon val)
        {
            var couponCode = val.CouponCode;
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var order = await SearchQuery(x => x.Id == val.Id)
             .Include(x => x.OrderLines).Include("OrderLines.Product")
             .Include(x => x.AppliedCoupons).Include("AppliedCoupons.Program")
             .Include(x => x.GeneratedCoupons).Include("GeneratedCoupons.Program")
             .Include(x => x.CodePromoProgram).Include(x => x.NoCodePromoPrograms).Include("NoCodePromoPrograms.Program").FirstOrDefaultAsync();

            var program = await programObj.SearchQuery(x => x.PromoCode == couponCode).FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckPromoCode(program, order, couponCode);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    if (program.PromoApplicability == "on_next_order")
                    {
                        // # Avoid creating the coupon if it already exist
                        var discount_line_product_ids = order.GeneratedCoupons.Where(x => x.State == "new" || x.State == "reserved").Select(x => x.Program.DiscountLineProductId);
                        if (!discount_line_product_ids.Contains(program.DiscountLineProductId))
                        {
                            var coupon = await _CreateRewardCoupon(order, program);
                        }
                    }
                    else
                    {
                        await _CreateRewardLine(order, program);
                        order.CodePromoProgramId = program.Id;
                        await UpdateAsync(order);
                    }
                }
                else
                    throw new Exception(error_status.Error);
            }
            else
            {
                var coupon = await couponObj.SearchQuery(x => x.Code == couponCode)
             .Include(x => x.Program).Include(x => x.Program.DiscountLineProduct).Include(x => x.Partner).FirstOrDefaultAsync();

                if (coupon != null)
                {
                    var error_status = await couponObj._CheckCouponCode(coupon, order);
                    if (string.IsNullOrEmpty(error_status.Error))
                    {
                        await _CreateRewardLine(order, coupon.Program);

                        order.AppliedCoupons.Add(coupon);
                        await UpdateAsync(order);

                        coupon.State = "used";
                        await couponObj.UpdateAsync(coupon);
                    }
                    else
                        throw new Exception(error_status.Error);
                }
                else
                    throw new Exception($"Mã {couponCode} không có giá trị");
            }
        }

        private async Task _CreateRewardLine(SaleOrder self, SaleCouponProgram program)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var lines = _GetRewardLineValues(self, program);
            foreach (var line in lines)
                line.Order = self;

            saleLineObj.UpdateProps(lines);
            saleLineObj.ComputeAmount(lines);

            foreach (var line in lines)
                self.OrderLines.Add(line);

            _AmountAll(self);
            await UpdateAsync(self);
        }

        private IEnumerable<SaleOrderLine> _GetRewardLineValues(SaleOrder self, SaleCouponProgram program)
        {
            if (program.RewardType == "discount")
                return _GetRewardValuesDiscount(self, program);
            else if (program.RewardType == "product")
                return new List<SaleOrderLine>() { _GetRewardValuesProduct(self, program) };
            return new List<SaleOrderLine>();
        }

        private SaleOrderLine _GetRewardValuesProduct(SaleOrder self, SaleCouponProgram program)
        {
            var line = self.OrderLines.Where(x => program.RewardProductId == x.ProductId).First();
            var price_unit = line.PriceUnit * (1 - line.Discount / 100);
            var order_lines = self.OrderLines.Except(_GetRewardLines(self));
            var max_product_qty = order_lines.Any() ? order_lines.Sum(x => x.ProductUOMQty) : 1;
            var reward_product_qty = Math.Min(max_product_qty, self.OrderLines.Where(x => program.RewardProductId == x.ProductId).Select(x => x.ProductUOMQty).Min());
            //Remove needed quantity from reward quantity if same reward and rule product
            var reward_qty = Math.Min(((int)(max_product_qty / (program.RuleMinQuantity ?? 1)) * (program.RewardProductQuantity ?? 1)), reward_product_qty);

            var productObj = GetService<IProductService>();
            if (program.RewardProduct == null)
                program.RewardProduct = productObj.GetById(program.RewardProductId);

            if (program.DiscountLineProduct == null)
                program.DiscountLineProduct = productObj.GetById(program.DiscountLineProductId);

            return new SaleOrderLine()
            {
                ProductId = program.DiscountLineProductId,
                PriceUnit = -price_unit,
                ProductUOMQty = reward_qty,
                IsRewardLine = true,
                Name = $"Dịch vụ miễn phí - {program.RewardProduct.Name}",
                ProductUOMId = program.DiscountLineProduct.UOMId
            };
        }

        public async Task<bool> CheckHasPromotionCanApply(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id)
              .Include(x => x.OrderLines)
              .Include(x => x.NoCodePromoPrograms).Include("NoCodePromoPrograms.Program")
              .Include(x => x.AppliedCoupons).Include("AppliedCoupons.Program")
              .Include(x => x.CodePromoProgram).FirstOrDefaultAsync();
            var programObj = GetService<ISaleCouponProgramService>();
            var programs = await _GetApplicableNoCodePromoProgram(self);
            programs = programObj._KeepOnlyMostInterestingAutoAppliedGlobalDiscountProgram(programs);

            var applied_programs = _GetAppliedProgramsWithRewardsOnCurrentOrder(self).Concat(_GetAppliedProgramsWithRewardsOnNextOrder(self));
            return programs.Any(x => !applied_programs.Contains(x));
        }

        public async Task _RemoveInvalidRewardLines(SaleOrder self)
        {
            /*
            Find programs & coupons that are not applicable anymore.
            It will then unlink the related reward order lines.
            It will also unset the order's fields that are storing
            the applied coupons & programs.
            Note: It will also remove a reward line coming from an archive program.
             */
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var order = self;
            var applicable_programs = (await _GetApplicableNoCodePromoProgram(order)).Union(await _GetApplicablePrograms(self))
                .Union(_GetValidAppliedCouponProgram(self));
            applicable_programs = programObj._KeepOnlyMostInterestingAutoAppliedGlobalDiscountProgram(applicable_programs);
            var applied_programs = _GetAppliedProgramsWithRewardsOnCurrentOrder(order).Concat(_GetAppliedProgramsWithRewardsOnNextOrder(order));
            var programs_to_remove = applied_programs.Where(x => !applicable_programs.Contains(x)).ToList();
            var programs_to_remove_ids = programs_to_remove.Select(x => x.Id).ToList();
            var products_to_remove_ids = programs_to_remove.Select(x => x.DiscountLineProductId.Value);

            var applied_programs_discount_line_product_ids = applied_programs.Select(x => x.DiscountLineProductId.Value);
            //delete reward line coming from an archived coupon (it will never be updated/removed when recomputing the order)
            var invalid_lines = order.OrderLines.Where(x => x.IsRewardLine && !applied_programs_discount_line_product_ids.Contains(x.ProductId.Value));

            //Invalid generated coupon for which we are not eligible anymore ('expired' since it is specific to this SO and we may again met the requirements)
            var invalid_coupons = self.GeneratedCoupons.Where(x => products_to_remove_ids.Contains(x.Program.DiscountLineProductId.Value)).ToList();
            foreach (var coupon in invalid_coupons)
                coupon.State = "expired";
            await couponObj.UpdateAsync(invalid_coupons);

            //Reset applied coupons for which we are not eligible anymore ('valid' so it can be use on another )
            var coupons_to_remove = self.AppliedCoupons.Where(x => programs_to_remove_ids.Contains(x.Program.Id)).ToList();
            foreach (var coupon in coupons_to_remove)
                coupon.State = "new";
            await couponObj.UpdateAsync(coupons_to_remove);

            //Unbind promotion and coupon programs which requirements are not met anymore
            if (programs_to_remove.Any())
            {
                foreach(var program in programs_to_remove)
                {
                    var rel = order.NoCodePromoPrograms.FirstOrDefault(x => x.ProgramId == program.Id);
                    if (rel != null)
                        order.NoCodePromoPrograms.Remove(rel);

                    if (program.Id == order.CodePromoProgramId)
                        order.CodePromoProgramId = null;
                }
            }

            if (coupons_to_remove.Any())
            {
                foreach (var coupon in coupons_to_remove)
                {
                    order.AppliedCoupons.Remove(coupon);
                }
            }

            await UpdateAsync(order);

            //Remove their reward lines
            invalid_lines = invalid_lines.Union(order.OrderLines.Where(x => products_to_remove_ids.Contains(x.ProductId.Value)));
            await saleLineObj.Unlink(invalid_lines.Select(x => x.Id).ToList());
        }

        public IEnumerable<SaleCouponProgram> _GetAppliedProgramsWithRewardsOnCurrentOrder(SaleOrder self)
        {
            /*
            # Need to add filter on current order. Indeed, it has always been calculating reward line even if on next order (which is useless and do calculation for nothing)
            # This problem could not be noticed since it would only update or delete existing lines related to that program, it would not find the line to update since not in the order
            # But now if we dont find the reward line in the order, we add it (since we can now have multiple line per  program in case of discount on different vat), thus the bug
            # mentionned ahead will be seen now
            */
            var programs = self.NoCodePromoPrograms.Select(x => x.Program).Where(x => x.PromoApplicability == "on_current_order")
                .Concat(self.AppliedCoupons.Select(x => x.Program));
            if (self.CodePromoProgram != null && self.CodePromoProgram.PromoApplicability == "on_current_order")
                programs = programs.Concat(new List<SaleCouponProgram>() { self.CodePromoProgram });
            return programs;
        }

        public IEnumerable<SaleCouponProgram> _GetAppliedProgramsWithRewardsOnNextOrder(SaleOrder self)
        {
            var programs = self.NoCodePromoPrograms.Select(x => x.Program).Where(x => x.PromoApplicability == "on_next_order");
            if (self.CodePromoProgram != null && self.CodePromoProgram.PromoApplicability == "on_next_order")
                programs = programs.Concat(new List<SaleCouponProgram>() { self.CodePromoProgram });
            return programs;
        }

        public IEnumerable<SaleCouponProgram> _GetValidAppliedCouponProgram(SaleOrder self)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var programs = self.AppliedCoupons.Select(x => x.Program).Where(x => x.PromoApplicability == "on_next_order");
            programs = programObj._FilterProgramsFromCommonRules(programs, self, true);
            programs = programs.Concat(programObj._FilterProgramsFromCommonRules(self.AppliedCoupons.Select(x => x.Program).Where(x => x.PromoApplicability == "on_current_order"), self));
            return programs;
        }

        public async Task<IEnumerable<SaleCouponProgram>> _GetApplicableNoCodePromoProgram(SaleOrder self)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var programs = await programObj.SearchQuery(x => x.PromoCodeUsage == "no_code_needed")
                .Include(x => x.DiscountSpecificProducts).ToListAsync();
            programs = programObj._FilterProgramsFromCommonRules(programs, self).ToList();
            return programs;
        }

        public async Task<IEnumerable<SaleCouponProgram>> _GetApplicablePrograms(SaleOrder self)
        {
            //This method is used to return the valid applicable programs on given order.
            //param: order - The sale order for which method will get applicable programs.
            var programObj = GetService<ISaleCouponProgramService>();
            var programs = (await programObj.SearchQuery().Include(x => x.DiscountSpecificProducts).ToListAsync()).AsEnumerable();
            programs = programObj._FilterProgramsFromCommonRules(programs, self);
            //if (self.CodePromoProgram != null && !string.IsNullOrEmpty(self.CodePromoProgram.PromoCode))
            //    programs = programObj._FilterPromoProgramsWithCode(programs, self.CodePromoProgram.PromoCode);
            return programs;
        }

       

        public bool _IsGlobalDiscountAlreadyApplied(SaleOrder self)
        {
            var applied_programs = self.NoCodePromoPrograms.Select(x => x.Program)
                .Concat(self.AppliedCoupons.Select(x => x.Program));
            if (self.CodePromoProgram != null)
                applied_programs = applied_programs.Concat(new List<SaleCouponProgram>() { self.CodePromoProgram });
            var programObj = GetService<ISaleCouponProgramService>();
            return applied_programs.Where(x => programObj._IsGlobalDiscountProgram(x)).Any();
        }

        public void GetRewardLineValues(SaleOrder self, SaleCouponProgram program)
        {

        }

        public IList<SaleOrderLine> _GetRewardValuesDiscount(SaleOrder self, SaleCouponProgram program)
        {
            if (program.DiscountLineProduct == null)
            {
                var productObj = GetService<IProductService>();
                program.DiscountLineProduct = productObj.GetById(program.DiscountLineProductId);
            }

            if (program.DiscountType == "fixed_amount")
            {
                return new List<SaleOrderLine>()
                {
                    new SaleOrderLine()
                    {
                        Name = $"Chiết khấu: " + program.Name,
                        ProductId = program.DiscountLineProductId,
                        PriceUnit = -_GetRewardValuesDiscountFixedAmount(self, program),
                        ProductUOMQty = 1,
                        ProductUOMId = program.DiscountLineProduct.UOMId,
                        IsRewardLine = true,
                    }
                };
            }

            var programObj = GetService<ISaleCouponProgramService>();
            var rewards = new List<SaleOrderLine>();
            var lines = _GetPaidOrderLines(self);
            if (program.DiscountApplyOn == "specific_products" || program.DiscountApplyOn == "on_order")
            {
                if (program.DiscountApplyOn == "specific_products")
                {
                    var discount_specific_product_ids = program.DiscountSpecificProducts.Select(x => x.ProductId).ToList();
                    //We should not exclude reward line that offer this product since we need to offer only the discount on the real paid product (regular product - free product)
                    var free_product_lines = programObj.SearchQuery(x => x.RewardType == "product" && discount_specific_product_ids.Contains(x.RewardProductId.Value)).Select(x => x.DiscountLineProductId.Value).ToList();
                    var tmp = discount_specific_product_ids.Union(free_product_lines);
                    lines = lines.Where(x => tmp.Contains(x.ProductId.Value)).ToList();
                }

                var total_discount_amount = 0M;
                foreach(var line in lines)
                {
                    var discount_line_amount = _GetRewardValuesDiscountPercentagePerLine(self, program, line);
                    if (discount_line_amount > 0)
                        total_discount_amount += discount_line_amount;
                }

                rewards.Add(new SaleOrderLine
                {
                    Name = $"Chiết khấu: {program.Name}",
                    ProductId = program.DiscountLineProductId,
                    PriceUnit = -total_discount_amount,
                    ProductUOMQty = 1,
                    ProductUOMId = program.DiscountLineProduct.UOMId,
                    IsRewardLine = true,
                });
            }

            return rewards;
        }

        private decimal _GetRewardValuesDiscountPercentagePerLine(SaleOrder self, SaleCouponProgram program, SaleOrderLine line)
        {
            var discount_amount = line.ProductUOMQty * (line.PriceUnit * (1 - line.Discount / 100)) *
                ((program.DiscountPercentage ?? 0) / 100);
            return discount_amount;
        }

        private decimal _GetRewardValuesDiscountFixedAmount(SaleOrder self, SaleCouponProgram program)
        {
            var total_amount = _GetPaidOrderLines(self).Sum(x => x.PriceTotal);
            var fixed_amount = program.DiscountFixedAmount ?? 0;
            if (total_amount < fixed_amount)
                return total_amount;
            return fixed_amount;
        }

        private IList<SaleOrderLine> _GetPaidOrderLines(SaleOrder self)
        {
            return self.OrderLines.Where(x => !x.IsRewardLine).ToList();
        }

        public async Task ApplyPromotion(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
                .Include("OrderLines.Product").FirstOrDefaultAsync();
            var soLineObj = GetService<ISaleOrderLineService>();
            var couponObj = GetService<SaleCouponService>();
            var product_list = new List<ApplyPromotionProductListItem>();
            var ruleObj = GetService<IPromotionRuleService>();
            var programObj = GetService<IPromotionProgramService>();
            var coupons = new List<SaleCoupon>();

            //tạo product list
            foreach (var line in self.OrderLines)
            {
                if (line.PromotionProgramId.HasValue || line.PromotionId.HasValue)
                    continue;

                var qty = line.ProductUOMQty;
                var amount = line.PriceSubTotal; //chưa bao gồm thuế
                var item = product_list.FirstOrDefault(x => x.Product.Id == line.Product.Id);
                if (item != null)
                {
                    item.Qty += qty;
                    item.Amount += amount;
                }
                else
                {
                    product_list.Add(new ApplyPromotionProductListItem
                    {
                        Product = line.Product,
                        Qty = qty,
                        Amount = amount
                    });
                }
            }

            //tìm tất cả nhóm sản phẩm và sản phẩm trong order
            var products = self.OrderLines.Select(x => x.Product).Distinct().ToList();
            var categ_ids = new HashSet<Guid>();
            foreach (var p in products)
            {
                var categ = p.Categ;
                while (categ != null)
                {
                    categ_ids.Add(categ.Id);
                    categ = categ.Parent;
                }
            }

            var prod_ids = products.Select(x => x.Id).ToList();

            var date = DateTime.Today;
            var programs = await programObj.SearchQuery(x => x.Active &&
            (!x.DateFrom.HasValue || x.DateFrom <= date) &&
            (!x.DateTo.HasValue || x.DateTo >= date)).Select(x => new PromotionProgramDisplay
            {
                Id = x.Id,
                OrderCount = x.SaleLines.Select(s => s.Order).Distinct().Count(),
                MaximumUseNumber = x.MaximumUseNumber ?? 0,
            }).ToListAsync();

            foreach(var program in programs)
            {
                if (program.MaximumUseNumber > 0 && program.MaximumUseNumber <= program.OrderCount)
                    continue;
                //tìm rules có thể áp dụng
                var rules = await ruleObj.SearchQuery(x => x.ProgramId == program.Id).Select(x => new SaleOrderSearchPromotionRuleItem
                {
                    Id = x.Id,
                    DiscountApplyOn = x.DiscountApplyOn,
                    DiscountProductItems = x.RuleProductRels.Select(s => new PromotionRuleDiscountProductItem {
                        ProductId = s.ProductId,
                        DiscountLineProductId = s.DiscountLineProductId,
                        DiscountLineProductUOMId = s.DiscountLineProduct.UOMId
                    }),
                    DiscountCategoryItems = x.RuleCategoryRels.Select(s => new PromotionRuleDiscountCategoryItem {
                        CategId = s.CategId,
                        DiscountLineProductId = s.DiscountLineProductId,
                        DiscountLineProductUOMId = s.DiscountLineProduct.UOMId
                    }),
                    MinQuantity = x.MinQuantity ?? 0,
                    DiscountType = x.DiscountType,
                    DiscountFixedAmount = x.DiscountFixedAmount ?? 0,
                    DiscountPercentage = x.DiscountPercentage ?? 0,
                    ProgramId = x.ProgramId,
                    ProgramName = x.Program.Name,
                    DiscountLineProductId = x.DiscountLineProductId,
                    DiscountLineProductUOMId = x.DiscountLineProduct.UOMId
                }).ToListAsync();

                foreach (var rule in rules)
                {
                    var min_quantity = rule.MinQuantity; //số lượng này nên product
                    var dict = new Dictionary<Guid, PromotionQtyAmountDictValue>();
                    if (rule.DiscountApplyOn == "0_product_variant")
                    {
                        foreach (var item in product_list)
                        {
                            if (!rule.DiscountProductItems.Any(x => x.ProductId == item.Product.Id))
                                continue;
                            if (item.Qty < min_quantity)
                                continue;
                            await UpdateDiscountLine(rule, self, item.Amount, productId: item.Product.Id);
                        }
                    }
                    else if (rule.DiscountApplyOn == "2_product_category")
                    {
                        decimal total_amount = 0;
                        foreach (var item in product_list)
                        {
                            var categId = item.Product.CategId;
                            if (!rule.DiscountCategoryItems.Any(x => x.CategId == categId))
                                continue;
                            if (item.Qty < min_quantity)
                                continue;

                            if (dict.ContainsKey(categId))
                                dict.Add(categId, new PromotionQtyAmountDictValue());

                            total_amount += item.Amount;
                            await UpdateDiscountLine(rule, self, total_amount, categId: categId);
                        }
                    }
                    else
                    {
                        decimal total_amount = 0;
                        foreach (var item in product_list)
                        {
                            if (item.Qty < min_quantity)
                                continue;
                            total_amount += item.Amount;
                        }

                        await UpdateDiscountLine(rule, self, total_amount);
                    }
                }
            }

            _AmountAll(self);
            _GetInvoiced(new List<SaleOrder>() { self });
            await UpdateAsync(self);
        }

        public async Task UpdateDiscountLine(SaleOrderSearchPromotionRuleItem rule, SaleOrder order, decimal amount_total, decimal qty = 1,
            Guid? productId = null, Guid? categId = null)
        {
            Guid? discountProductId = null;
            Guid? discountProductUOMId = null;
            if (rule.DiscountApplyOn == "0_product_variant" && productId.HasValue)
            {
                var discountProductItem = rule.DiscountProductItems.FirstOrDefault(x => x.ProductId == productId);
                discountProductId = discountProductItem.DiscountLineProductId;
                discountProductUOMId = discountProductItem.DiscountLineProductUOMId;
            }
            else if (rule.DiscountApplyOn == "2_product_category" && categId.HasValue)
            {
                var discountProductItem = rule.DiscountCategoryItems.FirstOrDefault(x => x.CategId == categId);
                discountProductId = discountProductItem.DiscountLineProductId;
                discountProductUOMId = discountProductItem.DiscountLineProductUOMId;
            }
            else
            {
                discountProductId = rule.DiscountLineProductId;
                discountProductUOMId = rule.DiscountLineProductUOMId;
            }
           
            var soLineObj = GetService<ISaleOrderLineService>();
            decimal price_unit = 0;
            if (rule.DiscountType == "percentage")
                price_unit = Math.Round(amount_total * rule.DiscountPercentage / 100);
            else if (rule.DiscountType == "fixed_amount")
                price_unit = rule.DiscountFixedAmount;

            var promo_line = order.OrderLines.Where(x => x.PromotionId == rule.ProgramId && x.ProductId == discountProductId).FirstOrDefault();
            if (promo_line != null)
            {
                promo_line.ProductUOMQty = qty;
                promo_line.PriceUnit = -price_unit;
                soLineObj.ComputeAmount(new List<SaleOrderLine>() { promo_line });
                await soLineObj._UpdateInvoiceQty(new List<Guid>() { promo_line.Id });
                soLineObj._GetToInvoiceQty(new List<SaleOrderLine>() { promo_line });
                soLineObj._ComputeInvoiceStatus(new List<SaleOrderLine>() { promo_line });
            }
            else
            {
                await soLineObj.CreateAsync(PrepareDiscountLine(rule, order, discountProductId.Value, discountProductUOMId.Value, price_unit, qty: qty));
            }
        }

        private SaleOrderLine PrepareDiscountLine(SaleOrderSearchPromotionRuleItem rule, SaleOrder order, Guid productId, Guid uomId, decimal discount_amount, decimal qty = 1)
        {
            return new SaleOrderLine
            {
                Name = $"Chiết khấu: {rule.ProgramName}",
                Order = order,
                OrderId = order.Id,
                ProductUOMQty = qty,
                ProductId = productId,
                ProductUOMId = uomId,
                PriceUnit = -discount_amount,
                PromotionId = rule.ProgramId,
            };
        }

        public async Task<SaleOrder> GetSaleOrderForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Order)
                .Include(x => x.Quote)
                .Include(x => x.Pricelist)
                .Include(x => x.User)
                .Include(x => x.OrderLines)
                .Include("OrderLines.Product")
                .Include("OrderLines.ToothCategory")
                .Include("OrderLines.SaleOrderLineToothRels")
                .Include("OrderLines.SaleOrderLineToothRels.Tooth")
                .FirstOrDefaultAsync();
        }

        public async Task<SaleOrder> GetSaleOrderWithLines(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.OrderLines)
                .Include("OrderLines.SaleOrderLineToothRels")
                .Include("OrderLines.DotKhamSteps")
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOrderAsync(Guid id, SaleOrderSave val)
        {
            var order = await GetSaleOrderWithLines(id);
            order = _mapper.Map(val, order);

            await SaveOrderLines(val, order);

            var saleLineObj = GetService<ISaleOrderLineService>();
            saleLineObj.UpdateOrderInfo(order.OrderLines, order);
            saleLineObj.ComputeAmount(order.OrderLines);
            await UpdateAsync(order);

            var linesIds = order.OrderLines.Select(x => x.Id).ToList();
            var lines = await saleLineObj.SearchQuery(x => linesIds.Contains(x.Id))
                .Include(x => x.Order)
                .Include(x => x.Product)
               .Include(x => x.SaleOrderLineInvoiceRels)
               .Include("SaleOrderLineInvoiceRels.InvoiceLine")
               .Include("SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
               .ToListAsync();

            saleLineObj._GetInvoiceQty(lines);
            saleLineObj._GetToInvoiceQty(lines);
            saleLineObj._ComputeInvoiceStatus(lines);
            await saleLineObj.UpdateAsync(lines);

            _AmountAll(order);
            _GetInvoiced(new List<SaleOrder>() { order });

            await UpdateAsync(order);

            var self = new List<SaleOrder>() { order };
            await _GenerateDotKhamSteps(self);

            //if (order.InvoiceStatus == "to_invoice" && order.State == "sale")
            //{
            //    var invs = await ActionInvoiceCreateV2(order);
            //    var invObj = GetService<IAccountInvoiceService>();
            //    await invObj.ActionInvoiceOpen(invs.Select(x => x.Id).ToList());

            //    saleLineObj._GetInvoiceQty(lines);
            //    saleLineObj._GetToInvoiceQty(lines);
            //    saleLineObj._ComputeInvoiceStatus(lines);
            //    await saleLineObj.UpdateAsync(lines);

            //    _AmountAll(order);
            //    _GetInvoiced(new List<SaleOrder>() { order });
            //    await UpdateAsync(order);

            //    ////Những dòng tăng số lượng sẽ tạo hóa đơn tăng doanh thu
            //    ////Những dòng giảm số lượng sẽ tạo hóa đơn giảm doanh thu
            //    //var increaseLines = new List<SaleOrderLine>();
            //    //var decreaseLines = new List<SaleOrderLine>();
            //    //foreach (var line in order.OrderLines)
            //    //{
            //    //    var qtyInvoiced = line.QtyInvoiced ?? 0;
            //    //    var priceUnit = line.PriceUnit;
            //    //    var qtyToInvoice = line.ProductUOMQty - qtyInvoiced;
            //    //    if (qtyToInvoice > 0 && priceUnit > 0)
            //    //        increaseLines.Add(line);
            //    //    if (qtyToInvoice > 0 && priceUnit < 0)
            //    //        decreaseLines.Add(line);
            //    //    if (qtyToInvoice < 0 && priceUnit > 0)
            //    //        decreaseLines.Add(line);
            //    //    if (qtyToInvoice < 0 && priceUnit < 0)
            //    //        increaseLines.Add(line);
            //    //}

            //    //var invObj = GetService<IAccountInvoiceService>();
            //    //bool updateFlag = false;
            //    //if (increaseLines.Any())
            //    //{
            //    //    var increaseInv = await CreateInvoice(increaseLines, order, type: "out_invoice");
            //    //    await invObj.ActionInvoiceOpen(new List<Guid>() { increaseInv.Id });

            //    //    saleLineObj._GetInvoiceQty(increaseLines);
            //    //    saleLineObj._GetToInvoiceQty(increaseLines);
            //    //    saleLineObj._ComputeInvoiceStatus(increaseLines);
            //    //    await saleLineObj.UpdateAsync(increaseLines);

            //    //    updateFlag = true;
            //    //}

            //    //if (decreaseLines.Any())
            //    //{
            //    //    var decreaseInv = await CreateInvoice(decreaseLines, order, type: "out_refund");
            //    //    await invObj.ActionInvoiceOpen(new List<Guid>() { decreaseInv.Id });

            //    //    saleLineObj._GetInvoiceQty(decreaseLines);
            //    //    saleLineObj._GetToInvoiceQty(decreaseLines);
            //    //    saleLineObj._ComputeInvoiceStatus(decreaseLines);
            //    //    await saleLineObj.UpdateAsync(decreaseLines);

            //    //    updateFlag = true;
            //    //}

            //    //if (updateFlag)
            //    //{
            //    //    var self = new List<SaleOrder>() { order };
            //    //    foreach (var saleOrder in self)
            //    //    {
            //    //        saleLineObj._GetInvoiceQty(saleOrder.OrderLines);
            //    //        saleLineObj._GetToInvoiceQty(saleOrder.OrderLines);
            //    //        saleLineObj._ComputeInvoiceStatus(saleOrder.OrderLines);
            //    //    }

            //    //    _GetInvoiced(self);
            //    //    await UpdateAsync(self);

            //    //    await _GenerateDotKhamSteps(self);
            //    //}
            //}
        }

        private async Task<AccountInvoice> CreateInvoice(IList<SaleOrderLine> saleLines, SaleOrder order, string type = "out_invoice")
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var invLineObj = GetService<IAccountInvoiceLineService>();
            var invObj = GetService<IAccountInvoiceService>();

            var companyId = CompanyId;
            var journalObj = GetService<IAccountJournalService>();
            var journal = journalObj.SearchQuery(x => x.Type == "sale" && x.CompanyId == companyId)
                .Include(x => x.DefaultCreditAccount).Include(x => x.DefaultDebitAccount).FirstOrDefault();
            if (journal == null)
                throw new Exception("Vui lòng tạo nhật ký bán hàng cho công ty này.");

            var inv = await PrepareInvoice(order, journal);
            inv.Type = type;
            await invObj.CreateAsync(inv);

            var sign = type == "out_invoice" ? 1 : -1;
            var invLines = new List<AccountInvoiceLine>();
            foreach (var line in order.OrderLines)
            {
                var qtyInvoiced = line.QtyInvoiced ?? 0;
                var qtyToInvoice = (line.ProductUOMQty - qtyInvoiced) * sign;
                if (qtyToInvoice == 0)
                    continue;

                var invLine = saleLineObj._PrepareInvoiceLine(line, qtyToInvoice, journal.DefaultCreditAccount);
                invLine.InvoiceId = inv.Id;
                invLine.Invoice = inv;
                invLine.SaleLines.Add(new SaleOrderLineInvoiceRel { OrderLineId = line.Id });
                invLines.Add(invLine);
            }

            if (invLines.Any())
            {
                await invLineObj.CreateAsync(invLines);
                invObj._ComputeAmount(inv);
                await invObj.UpdateAsync(inv);
            }
            else
            {
                throw new Exception("Không có dòng nào có thể tạo hóa đơn");
            }

            return inv;
        }

        private async Task SaveOrderLines(SaleOrderSave val, SaleOrder order)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<SaleOrderLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.OrderLines)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            if (lineToRemoves.Any())
                await saleLineObj.Unlink(lineToRemoves.Select(x => x.Id).ToList());

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var saleLine = _mapper.Map<SaleOrderLine>(line);
                    saleLine.Sequence = sequence++;
                    foreach (var toothId in line.ToothIds)
                    {
                        saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                        {
                            ToothId = toothId
                        });
                    }
                    order.OrderLines.Add(saleLine);
                }
                else
                {
                    var saleLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (saleLine != null)
                    {
                        _mapper.Map(line, saleLine);
                        saleLine.Sequence = sequence++;
                        saleLine.SaleOrderLineToothRels.Clear();
                        foreach (var toothId in line.ToothIds)
                        {
                            saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                            {
                                ToothId = toothId
                            });
                        }
                    }
                }
            }
        }

        public async Task<SaleOrder> GetSaleOrderByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.DotKhams)
                .ToListAsync();
            var states = new string[] { "draft", "cancel" };
            foreach (var order in self)
            {
                if (!states.Contains(order.State))
                    throw new Exception("Bạn chỉ có thể xóa phiếu ở trạng thái nháp hoặc hủy bỏ");
                if (order.IsQuotation == true && order.OrderId.HasValue)
                    throw new Exception("Bạn không thể xóa phiếu tư vấn đã tạo phiếu điều trị");
            }

            var quotationIds = self.Where(x => x.QuoteId.HasValue).Select(x => x.QuoteId.Value).Distinct().ToList();
            if (quotationIds.Any())
            {
                var quotations = await SearchQuery(x => quotationIds.Contains(x.Id)).ToListAsync();
                foreach (var quote in quotations)
                    quote.QuoteId = null;
                await UpdateAsync(quotations);
            }

            var dkStepObj = GetService<IDotKhamStepService>();
            var dkSteps = await dkStepObj.SearchQuery(x => ids.Contains(x.SaleOrderId.Value)).ToListAsync();
            await dkStepObj.DeleteAsync(dkSteps);

            var couponObj = GetService<ISaleCouponService>();
            var coupons = await couponObj.SearchQuery(x => ids.Contains(x.SaleOrderId.Value)).ToListAsync();
            if (coupons.Any())
            {
                foreach(var coupon in coupons)
                {
                    coupon.SaleOrderId = null;
                    coupon.State = "new";
                }
                await couponObj.UpdateAsync(coupons);
            }

            await DeleteAsync(self);
        }


        public async Task UnlinkSaleOrderAsync(SaleOrder order)
        {
            await DeleteAsync(order);
        }

        public async Task<SaleOrderLineDisplay> DefaultLineGet(SaleOrderLineDefaultGet val)
        {
            var productObj = GetService<IProductService>();
            var product = await productObj.GetByIdAsync(val.ProductId);
            var res = new SaleOrderLineDisplay()
            {
                Name = product.Name,
                ProductUOMQty = 1,
                ProductId = product.Id,
                Product = _mapper.Map<ProductSimple>(product),
                PriceUnit = product.ListPrice,
                PriceSubTotal = product.ListPrice,
                PriceTotal = product.ListPrice,
            };
            return res;
        }

        public async Task<SaleOrderDisplay> DefaultGet(SaleOrderDefaultGet val)
        {
            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            var res = new SaleOrderDisplay();
            res.CompanyId = CompanyId;
            res.UserId = UserId;
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            if (val.IsQuotation.HasValue)
                res.IsQuotation = val.IsQuotation;
            if (val.PartnerId.HasValue)
            {
                var partnerObj = GetService<IPartnerService>();
                var partner = await partnerObj.GetByIdAsync(val.PartnerId);
                res.PartnerId = partner.Id;
                res.Partner = _mapper.Map<PartnerSimple>(partner);
            }
            return res;
        }

        public IEnumerable<Guid> DefaultGetInvoice(List<Guid> ids)
        {
            var invoiceIds = new List<Guid>();
            foreach (var id in ids)
            {
                var order = SearchQuery(x => x.Id == id)
                .Include(x => x.OrderLines)
                .Include("OrderLines.SaleOrderLineInvoiceRels")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
                .FirstOrDefault();
                invoiceIds.AddRange(order.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels).Select(x => x.InvoiceLine).Select(x => x.Invoice.Id).Distinct().ToList());
            }

            return invoiceIds;
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include("OrderLines.Order")
                .Include("OrderLines.DotKhamSteps")
                .ToListAsync();
            foreach (var order in self)
            {
                order.State = "sale";
                foreach (var line in order.OrderLines)
                {
                    line.State = "sale";
                }
                saleLineObj._GetToInvoiceQty(order.OrderLines);
                saleLineObj._ComputeInvoiceStatus(order.OrderLines);
            }

            _GetInvoiced(self);
            await UpdateAsync(self);

            var invoices = await ActionInvoiceCreate(self);
            var invObj = GetService<IAccountInvoiceService>();
            await invObj.ActionInvoiceOpen(invoices.Select(x => x.Id).ToList());

            foreach (var order in self)
            {
                saleLineObj._GetInvoiceQty(order.OrderLines);
                saleLineObj._GetToInvoiceQty(order.OrderLines);
                saleLineObj._ComputeInvoiceStatus(order.OrderLines);
            }

            _GetInvoiced(self);
            _ComputeResidual(self);
            await UpdateAsync(self);

            await _GenerateDotKhamSteps(self);
        }

        public async Task<IEnumerable<PaymentInfoContent>> _GetPaymentInfoJson(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
              .Include("OrderLines.SaleOrderLineInvoiceRels")
              .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine").FirstOrDefaultAsync();
            var invoiceIds = self.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels).Where(x => x.InvoiceLine.InvoiceId.HasValue).Select(x => x.InvoiceLine.InvoiceId.Value).ToList();
            var invoiceObj = GetService<IAccountInvoiceService>();
            var paymentInfos = await invoiceObj._GetPaymentInfoJson(invoiceIds);
            var dict = new Dictionary<Guid, PaymentInfoContent>();
            foreach(var paymentInfo in paymentInfos)
            {
                if (!dict.ContainsKey(paymentInfo.PaymentId))
                    dict.Add(paymentInfo.PaymentId, paymentInfo);
                else
                    dict[paymentInfo.PaymentId].Amount += paymentInfo.Amount;
            }

            return dict.Values;
        }

        private async Task _GenerateDotKhamSteps(IEnumerable<SaleOrder> self)
        {
            var dotKhamStepService = GetService<IDotKhamStepService>();
            var productStepObj = GetService<IProductStepService>();
            foreach (var order in self)
            {
                foreach (var line in order.OrderLines)
                {
                    if (!line.ProductId.HasValue || line.Product == null)
                        continue;
                    if (!(line.Product.SaleOK == true && line.Product.Type2 == "service"))
                        continue;
                    //nếu số lượng bằng 0, nếu có steps thì remove
                    if (line.ProductUOMQty == 0 && line.DotKhamSteps.Any())
                    {
                        await dotKhamStepService.Unlink(line.DotKhamSteps);
                        continue;
                    }

                    if (line.DotKhamSteps.Any())
                        continue;

                    var steps = await productStepObj.SearchQuery(x => x.ProductId == line.ProductId).ToListAsync();
                    var list = new List<DotKhamStep>();
                    if (steps.Any())
                    {
                        foreach (var step in steps)
                        {
                            list.Add(new DotKhamStep
                            {
                                Name = step.Name,
                                ProductId = line.ProductId.Value,
                                Order = step.Order,
                                SaleOrderId = order.Id,
                                SaleLineId = line.Id,
                            });
                        }
                    }
                    else
                    {
                        list.Add(new DotKhamStep
                        {
                            Name = line.Product.Name,
                            SaleLineId = line.Id,
                            ProductId = line.ProductId.Value,
                            Order = 0,
                            SaleOrderId = order.Id,
                        });
                    }
                    await dotKhamStepService.CreateAsync(list);
                }
            }
        }

        public void _GetInvoiced(IEnumerable<SaleOrder> orders)
        {
            foreach (var order in orders)
            {
                var lineInvoiceStatus = order.OrderLines.Select(x => x.InvoiceStatus);
                if (order.State != "sale" && order.State != "done")
                    order.InvoiceStatus = "no";
                else if (lineInvoiceStatus.Any(x => x == "to invoice"))
                    order.InvoiceStatus = "to invoice";
                else if (lineInvoiceStatus.All(x => x == "invoiced"))
                    order.InvoiceStatus = "invoiced";
                else if (lineInvoiceStatus.All(x => x == "invoiced" || x == "upselling"))
                    order.InvoiceStatus = "upselling";
                else
                    order.InvoiceStatus = "no";
            }
        }

        public async Task<IEnumerable<AccountInvoice>> ActionInvoiceCreate(IEnumerable<SaleOrder> orders)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var invLineObj = GetService<IAccountInvoiceLineService>();
            var invObj = GetService<IAccountInvoiceService>();

            var companyId = CompanyId;
            var journalObj = GetService<IAccountJournalService>();
            var journal = journalObj.SearchQuery(x => x.Type == "sale" && x.CompanyId == companyId)
                .Include(x => x.DefaultCreditAccount).Include(x => x.DefaultDebitAccount).FirstOrDefault();
            if (journal == null)
                throw new Exception("Vui lòng tạo nhật ký bán hàng cho công ty này.");

            var invoices = new Dictionary<Guid, AccountInvoice>();
            var orderToUpdate = new HashSet<SaleOrder>();

            foreach (var order in orders)
            {
                var groupKey = order.Id;
                var invLines = new List<AccountInvoiceLine>();
                foreach (var line in order.OrderLines)
                {
                    if (FloatUtils.FloatIsZero((double)(line.QtyToInvoice ?? 0), precisionDigits: 5))
                        continue;
                    if (!invoices.ContainsKey(groupKey))
                    {
                        var inv = await PrepareInvoice(order, journal);
                        await invObj.CreateAsync(inv);
                        invoices.Add(groupKey, inv);
                    }

                    if (line.QtyToInvoice > 0)
                    {
                        var invLine = saleLineObj._PrepareInvoiceLine(line, line.QtyToInvoice ?? 0, journal.DefaultCreditAccount);
                        invLine.InvoiceId = invoices[groupKey].Id;
                        invLine.Invoice = invoices[groupKey];
                        invLine.SaleLines.Add(new SaleOrderLineInvoiceRel { OrderLineId = line.Id });
                        invLines.Add(invLine);
                    }
                }

                if (invLines.Any() && invoices.ContainsKey(groupKey))
                {
                    await invLineObj.CreateAsync(invLines);
                    var inv = invoices[groupKey];
                    invObj._ComputeAmount(inv);
                    await invObj.UpdateAsync(inv);
                }
            }

            if (!invoices.Any())
                throw new Exception("Không có dòng nào có thể tạo hóa đơn");

            foreach (var invoice in invoices.Values)
            {
                if (!invoice.InvoiceLines.Any())
                    throw new Exception("Không có dòng nào có thể tạo hóa đơn");
            }

            return invoices.Values.ToList();
        }

        public async Task<IEnumerable<AccountInvoice>> ActionInvoiceCreateV2(Guid id)
        {
            var order = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
                .FirstOrDefaultAsync();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var invLineObj = GetService<IAccountInvoiceLineService>();
            var invObj = GetService<IAccountInvoiceService>();

            var companyId = CompanyId;
            var journalObj = GetService<IAccountJournalService>();
            var journal = journalObj.SearchQuery(x => x.Type == "sale" && x.CompanyId == companyId)
                .Include(x => x.DefaultCreditAccount).Include(x => x.DefaultDebitAccount).FirstOrDefault();
            if (journal == null)
                throw new Exception("Vui lòng tạo nhật ký bán hàng cho công ty này.");

            var invoices = new Dictionary<string, AccountInvoice>();
            var orderToUpdate = new HashSet<SaleOrder>();

            var invLines = new List<AccountInvoiceLine>();
            foreach (var line in order.OrderLines)
            {
                var amountToInvoice = line.AmountToInvoice ?? 0;
                var qtyToInvoice = line.QtyToInvoice ?? 0;
                if (amountToInvoice == 0)
                    continue;
                var invType = amountToInvoice > 0 ? "out_invoice" : "out_refund";
                var sign = amountToInvoice > 0 ? 1 : -1;
                if (!invoices.ContainsKey(invType))
                {
                    var inv = await PrepareInvoice(order, journal);
                    inv.Type = invType;
                    await invObj.CreateAsync(inv);
                    invoices.Add(invType, inv);
                }

                var invLine = saleLineObj._PrepareInvoiceLine(line, Math.Abs(qtyToInvoice), journal.DefaultCreditAccount);
                invLine.PriceUnit *= sign;
                if (qtyToInvoice == 0)
                {
                    invLine.Quantity = 1;
                    invLine.PriceUnit = amountToInvoice * sign;
                    invLine.Discount = 0;
                }
                invLine.InvoiceId = invoices[invType].Id;
                invLine.Invoice = invoices[invType];
                invLine.SaleLines.Add(new SaleOrderLineInvoiceRel { OrderLineId = line.Id });
                invLines.Add(invLine);
            }

            if (invLines.Any())
            {
                await invLineObj.CreateAsync(invLines);
                var invs = invoices.Values;
                invObj._ComputeAmount(invs);
                await invObj.UpdateAsync(invs);
            }

            await invObj.ActionInvoiceOpen(invoices.Values.Select(x => x.Id).ToList());

            await saleLineObj._UpdateInvoiceQty(order.OrderLines.Select(x => x.Id).ToList());
            saleLineObj._GetToInvoiceQty(order.OrderLines);
            saleLineObj._ComputeInvoiceStatus(order.OrderLines);
            await saleLineObj.UpdateAsync(order.OrderLines);

            var self = new List<SaleOrder>() { order };
            _GetInvoiced(self);
            _ComputeResidual(self);
            await UpdateAsync(self);

            return invoices.Values.ToList();
        }

        public async Task<SaleOrderPrintVM> GetPrint(Guid id)
        {
            var order = await SearchQuery(x => x.Id == id)
               .Include(x => x.Partner)
               .Include(x => x.Company)
               .Include(x => x.Company.Partner)
               .Include(x => x.OrderLines)
               .Include("OrderLines.Product")
               .FirstOrDefaultAsync();
            var res = _mapper.Map<SaleOrderPrintVM>(order);
            var partnerObj = GetService<IPartnerService>();
            res.CompanyAddress = partnerObj.GetFormatAddress(order.Company.Partner);
            res.PartnerAddress = partnerObj.GetFormatAddress(order.Partner);
            return res;
        }

        public override ISpecification<SaleOrder> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.sale_order_comp_rule":
                    return new InitialSpecification<SaleOrder>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task RecomputeCouponLines(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include(x => x.NoCodePromoPrograms).Include("NoCodePromoPrograms.Program")
                .Include(x => x.AppliedCoupons).Include("AppliedCoupons.Program")
                .Include(x => x.CodePromoProgram).ToListAsync();
            foreach(var order in self)
            {
                await _RemoveInvalidRewardLines(order);
                await _CreateNewNoCodePromoRewardLines(order);
                await _UpdateExistingRewardLines(order);
            }
        }

        public async Task _CreateNewNoCodePromoRewardLines(SaleOrder self)
        {
            //Apply new programs that are applicable
            var programObj = GetService<ISaleCouponProgramService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var order = self;
            var programs = await _GetApplicableNoCodePromoProgram(self);
            programs = programObj._KeepOnlyMostInterestingAutoAppliedGlobalDiscountProgram(programs);
            var line_product_ids = self.OrderLines.Select(x => x.ProductId).ToList();
            foreach(var program in programs)
            {
                var error_status = await programObj._CheckPromoCode(program, order, "");
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    if (program.PromoApplicability == "on_next_order")
                        await _CreateRewardCoupon(order, program);
                    else if (!line_product_ids.Contains(program.DiscountLineProductId))
                    {
                        var values = _GetRewardLineValues(self, program);
                        foreach (var value in values)
                            value.Order = self;

                        saleLineObj.UpdateProps(values);
                        saleLineObj.ComputeAmount(values);
                        foreach (var value in values)
                        {
                            self.OrderLines.Add(value);
                        }
                    }

                    order.NoCodePromoPrograms.Add(new SaleOrderNoCodePromoProgram { ProgramId = program.Id });
                }
            }

            _AmountAll(order);
            await UpdateAsync(order);
        }

        public async Task _UpdateExistingRewardLines(SaleOrder self)
        {
            void UpdateLine(SaleOrder o, IEnumerable<SaleOrderLine> lines, SaleOrderLine value) {
                if (value != null && value.PriceUnit != 0 && value.ProductUOMQty != 0)
                {
                    foreach(var line in lines)
                    {
                        line.PriceUnit = value.PriceUnit;
                        line.ProductUOMQty = value.ProductUOMQty;
                    }
                }
            }

            //Update values for already applied rewards
            var saleLineObj = GetService<ISaleOrderLineService>();
            var order = self;
            var applied_programs = _GetAppliedProgramsWithRewardsOnCurrentOrder(self);
            foreach(var program in applied_programs)
            {
                var values = _GetRewardLineValues(order, program);
                var lines = order.OrderLines.Where(x => x.ProductId == program.DiscountLineProductId);
                if (program.RewardType == "discount" && program.DiscountType == "percentage")
                {
                    foreach(var value in values)
                    {
                        foreach(var line in lines)
                        {
                            UpdateLine(order, new List<SaleOrderLine>() { line }, value);
                        }
                    }
                }
                else
                {
                    UpdateLine(order, lines, values.FirstOrDefault());
                }

                saleLineObj.ComputeAmount(lines);
                await saleLineObj.UpdateAsync(lines);
            }

            _AmountAll(order);
            await UpdateAsync(order);
        }

        private async Task<SaleCoupon> _CreateRewardCoupon(SaleOrder self, SaleCouponProgram program)
        {
            //if there is already a coupon that was set as expired, reactivate that one instead of creating a new one
            var couponObj = GetService<ISaleCouponService>();
            var coupon = await couponObj.SearchQuery(x => x.ProgramId == program.Id && x.State == "expired" && x.PartnerId == self.PartnerId &&
            x.OrderId == self.Id && x.Program.DiscountLineProductId == program.DiscountLineProductId).FirstOrDefaultAsync();
            if (coupon != null)
            {
                coupon.State = "reserved";
                await couponObj.UpdateAsync(coupon);
            }
            else
            {
                coupon = await couponObj.CreateAsync(new SaleCoupon()
                {
                    ProgramId = program.Id,
                    State = "reserved",
                    PartnerId = self.PartnerId,
                    OrderId = self.Id,
                });
            }

            self.GeneratedCoupons.Add(coupon);
            await UpdateAsync(self);

            return coupon;
        }

        public async Task<AccountInvoice> PrepareInvoice(SaleOrder order, AccountJournal journal)
        {
            var accountObj = GetService<IAccountAccountService>();
            var accountReceivable = await accountObj.GetAccountReceivableCurrentCompany();

            var vals = new AccountInvoice
            {
                Name = order.Name,
                Origin = order.Name,
                Type = "out_invoice",
                Reference = order.Name,
                PartnerId = order.PartnerId,
                Comment = order.Note,
                AccountId = accountReceivable.Id,
                JournalId = journal.Id,
                UserId = order.UserId,
                CompanyId = order.CompanyId,
                Company = order.Company,
            };

            return vals;
        }

        public void _ComputeResidual(IEnumerable<SaleOrder> self)
        {
            foreach (var order in self)
            {
                var invoices = order.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels)
                    .Select(x => x.InvoiceLine).Select(x => x.Invoice).Distinct().ToList();
                decimal residual = 0M;
                foreach (var invoice in invoices)
                {
                    if (invoice.Type != "out_invoice" && invoice.Type != "out_refund")
                        continue;
                    if (invoice.Type == "out_invoice")
                        residual += invoice.Residual;
                    else
                        residual -= invoice.Residual;
                }
                order.Residual = residual;
            }
        }

        public void _ComputeResidual(IEnumerable<AccountInvoice> invoices)
        {
            var invoiceObj = GetService<IAccountInvoiceService>();
            var queryInvoices = invoiceObj.SearchQuery(x => invoices.Any(y => y.Id == x.Id))
                .Include(x => x.InvoiceLines)
                .Include("InvoiceLines.SaleLines")
                .Include("InvoiceLines.SaleLines.OrderLine")
                .Include("InvoiceLines.SaleLines.OrderLine.Order").ToList();
            //Xác định ds hóa đơn thuộc phiếu điều trị nào
            var orders = queryInvoices.SelectMany(x => x.InvoiceLines).SelectMany(x => x.SaleLines).Select(x => x.OrderLine).Select(x => x.Order)
                .Distinct().OrderBy(x=>x.DateCreated);
            foreach(var order in orders)
            {
                var queryOrder = SearchQuery(x => x.Id == order.Id)
                .Include(x => x.OrderLines)
                .Include("OrderLines.SaleOrderLineInvoiceRels")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
                .FirstOrDefault();

                //Lấy tất cả invoices thuộc phiếu điều trị order
                var allInvoices = order.OrderLines.SelectMany(x => x.SaleOrderLineInvoiceRels).Select(x => x.InvoiceLine).Select(x => x.Invoice).Distinct().ToList();

                var residual = 0M;
                foreach (var inv in allInvoices)
                {
                    residual += inv.Residual;
                }
                order.Residual = residual;
                Update(order);
            }            
        }

        public async Task _UpdateLoyaltyPoint(IEnumerable<SaleOrder> self)
        {
            foreach(var order in self)
            {
                if (order.State != "done")
                    continue;
                if (order.Card == null)
                    continue;
                var cardObj = GetService<ICardCardService>();
                var card = order.Card;
                var points = await cardObj.ConvertAmountToPoint(order.AmountTotal);
                card.TotalPoint += points;
                await cardObj.UpdateAsync(card);
            }
        }
    }

    public class SaleOrderSearchPromotionRuleItem
    {
        public Guid Id { get; set; }
        public string DiscountApplyOn { get; set; }
        public IEnumerable<PromotionRuleDiscountProductItem> DiscountProductItems { get; set; }
        public IEnumerable<PromotionRuleDiscountCategoryItem> DiscountCategoryItems { get; set; }
        public decimal MinQuantity { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountFixedAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string ProgramName { get; set; }
        public Guid ProgramId { get; set; }

        public Guid? DiscountLineProductId { get; set; }
        public Guid? DiscountLineProductUOMId { get; set; }
    }

    public class PromotionRuleDiscountProductItem
    {
        public Guid ProductId { get; set; }
        public Guid? DiscountLineProductId { get; set; }
        public Guid? DiscountLineProductUOMId { get; set; }
    }

    public class PromotionRuleDiscountCategoryItem
    {
        public Guid CategId { get; set; }
        public Guid? DiscountLineProductId { get; set; }
        public Guid? DiscountLineProductUOMId { get; set; }
    }

    public class PromotionQtyAmountDictValue
    {
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
    }
}
