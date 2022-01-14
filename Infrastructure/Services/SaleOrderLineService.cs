using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Kendo.DynamicLinqCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using Kendo.DynamicLinqCore;

namespace Infrastructure.Services
{
    public class SaleOrderLineService : BaseService<SaleOrderLine>, ISaleOrderLineService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        public SaleOrderLineService(IAsyncRepository<SaleOrderLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper,
             UserManager<ApplicationUser> userManager,
            CatalogDbContext dbContext)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public override async Task<IEnumerable<SaleOrderLine>> CreateAsync(IEnumerable<SaleOrderLine> entities)
        {
            UpdateProps(entities);
            ComputeAmount(entities);
            return await base.CreateAsync(entities);
        }

        public override async Task UpdateAsync(IEnumerable<SaleOrderLine> entities)
        {
            UpdateProps(entities);
            ComputeAmount(entities);
            await base.UpdateAsync(entities);
        }

        public void ComputeAmount(IEnumerable<SaleOrderLine> self)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {

                line.PriceSubTotal = Math.Round(line.ProductUOMQty * (line.PriceUnit - (decimal)(line.AmountDiscountTotal ?? 0)));
                line.PriceTax = 0;
                line.PriceTotal = line.PriceSubTotal + line.PriceTax;
            }
        }

        public void ComputeResidual(IEnumerable<SaleOrderLine> self)
        {
            foreach (var line in self)
            {
                if (line.State == "draft")
                {
                    line.AmountPaid = 0;
                    line.AmountResidual = 0;
                    continue;
                }

                decimal amountPaid = 0;
                foreach (var rel in line.SaleOrderLinePaymentRels)
                {
                    var payment = rel.Payment;
                    if (payment != null && payment.State != "draft" && payment.State != "cancel")
                        amountPaid += (rel.AmountPrepaid ?? 0);
                }

                line.AmountPaid = amountPaid;
                line.AmountResidual = line.PriceTotal - line.AmountPaid;
            }
        }

        //Get thong tin cua 1 dich vu
        public async Task<SaleOrderLineOnChangeProductResult> OnChangeProduct(SaleOrderLineOnChangeProduct val)
        {
            var res = new SaleOrderLineOnChangeProductResult();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId.Value).Include(x => x.Categ).FirstOrDefaultAsync();
                res.Name = product.Name;
                if (val.PricelistId.HasValue)
                {
                    var pricelistObj = GetService<IProductPricelistService>();
                    var pricelist = await pricelistObj.GetByIdAsync(val.PricelistId.Value);

                    var computePrice = await productObj._ComputeProductPrice(new List<Product>() { product }, val.PricelistId.Value, partnerId: val.PartnerId);
                    var price = computePrice[product.Id];
                    res.PriceUnit = price;

                    //tính giá của sản phẩm trước khi áp dụng bảng giá
                    if (pricelist.DiscountPolicy == "without_discount")
                    {
                        var new_list_price = await product.GetListPrice(GetService<IProductService>(), GetService<IIrConfigParameterService>(),
                        GetService<IIRPropertyService>());

                        res.PriceUnit = new_list_price;
                        if (new_list_price != 0)
                        {
                            var discount = (new_list_price - price) / new_list_price * 100;
                            //discount làm tròn 2 chữ số thập phân
                            if ((discount > 0 && new_list_price > 0) || (discount < 0 && new_list_price < 0))
                                res.Discount = (decimal)FloatUtils.FloatRound((double)discount, precisionDigits: 2);
                        }
                    }
                }
                else
                {
                    res.PriceUnit = await product.GetListPrice(GetService<IProductService>(), GetService<IIrConfigParameterService>(),
                        GetService<IIRPropertyService>());
                }
            }

            return res;
        }

        public void UpdateProps(IEnumerable<SaleOrderLine> self)
        {
            foreach (var line in self)
            {
                var order = line.Order;
                if (order == null)
                    continue;
                line.OrderPartnerId = order.PartnerId;
                line.CompanyId = order.CompanyId;
                //line.State = order.State;
            }
        }

        public void UpdateOrderInfo(ICollection<SaleOrderLine> self, SaleOrder order)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {
                //line.SalesmanId = order.UserId;
                line.OrderPartnerId = order.PartnerId;
                line.CompanyId = order.CompanyId;
                line.Order = order;
                line.State = order.State;
            }
        }

        public async Task<IEnumerable<ServiceCardCardBasic>> GetListServiceCardCardApplyable(Guid id)
        {
            var serviceCardCardObj = GetService<IServiceCardCardService>();
            var orderLine = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();

            var serviceCardCards = await serviceCardCardObj.SearchQuery(x => x.PartnerId == orderLine.OrderPartnerId
            && x.CardType.ProductPricelist.Items.Any(s => s.ProductId == orderLine.ProductId) && x.State == "in_use"
            && (!x.ActivatedDate.HasValue || orderLine.Date >= x.ActivatedDate.Value) && (!x.ExpiredDate.HasValue || orderLine.Date <= x.ExpiredDate.Value))
            .Include(x => x.CardType).ToListAsync();

            var res = _mapper.Map<IEnumerable<ServiceCardCardBasic>>(serviceCardCards);

            return res;
        }

        public async Task<IEnumerable<CardCardBasic>> GetListCardCardApplyable(Guid id)
        {
            var cardCardObj = GetService<ICardCardService>();
            var orderLine = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();

            var cardCards = await cardCardObj.SearchQuery(x => x.PartnerId == orderLine.OrderPartnerId
            && x.Type.Pricelist.Items.Any(s => s.ProductId == orderLine.ProductId) && x.State == "in_use")
            .Include(x => x.Type)
            .ToListAsync();

            var res = _mapper.Map<IEnumerable<CardCardBasic>>(cardCards);

            return res;
        }

        public void _GetToInvoiceQty(IEnumerable<SaleOrderLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.Order.State == "sale" || line.Order.State == "done")
                {
                    line.QtyToInvoice = line.ProductUOMQty - (line.QtyInvoiced ?? 0);
                }
                else
                {
                    line.QtyToInvoice = 0;
                }
            }
        }

        public void _GetToInvoiceAmount(IEnumerable<SaleOrderLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.Order.State == "sale" || line.Order.State == "done")
                {
                    line.AmountToInvoice = line.PriceSubTotal - (line.AmountInvoiced ?? 0);
                }
                else
                {
                    line.AmountToInvoice = 0;
                }
            }
        }

        public void _GetInvoiceQty(IEnumerable<SaleOrderLine> self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            foreach (var line in self)
            {
                decimal qtyInvoiced = 0;
                var amls = amlObj.SearchQuery(x => x.SaleLineRels.Any(x => x.OrderLineId == line.Id)).Include(x => x.Move).ToList();
                foreach (var invoiceLine in amls)
                {
                    var move = invoiceLine.Move;
                    if (move.State != "cancel")
                    {
                        if (move.Type == "out_invoice")
                        {
                            qtyInvoiced += (invoiceLine.Quantity ?? 0);
                        }
                        else if (move.Type == "out_refund")
                        {
                            qtyInvoiced -= (invoiceLine.Quantity ?? 0);
                        }
                    }
                }

                line.QtyInvoiced = qtyInvoiced;
            }
        }

        public void _GetInvoiceAmount(IEnumerable<Guid> ids)
        {
            var self = SearchQuery(x => ids.Contains(x.Id)).ToList();
            _GetInvoiceAmount(self);
        }

        public void _GetInvoiceAmount(IEnumerable<SaleOrderLine> self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var selfIds = self.Select(x => x.Id).ToList();
            var selfAmls = amlObj.SearchQuery(x => x.SaleLineRels.Any(x => selfIds.Contains(x.OrderLineId)))
                .Include(x => x.SaleLineRels)
                .Include(x => x.Move).ToList();

            foreach (var line in self)
            {
                decimal amountInvoiced = 0;
                decimal amountInsurancePaid = 0;
                Guid? insuranceId = null;

                var amls = selfAmls.Where(x => x.SaleLineRels.Any(s => s.OrderLineId == line.Id)).ToList();
                foreach (var invoiceLine in amls)
                {
                    var move = invoiceLine.Move;
                    if (move.State != "cancel")
                    {
                        if (move.Type == "out_invoice")
                        {
                            amountInvoiced += (invoiceLine.PriceSubtotal ?? 0);
                            amountInsurancePaid += invoiceLine.InsuranceId.HasValue ? (invoiceLine.PriceSubtotal ?? 0) : 0;
                        }
                        else if (move.Type == "out_refund")
                        {
                            amountInvoiced -= (invoiceLine.PriceSubtotal ?? 0);
                            amountInsurancePaid -= invoiceLine.InsuranceId.HasValue ? (invoiceLine.PriceSubtotal ?? 0) : 0;
                        }
                    }

                    insuranceId = invoiceLine.InsuranceId.HasValue ? invoiceLine.InsuranceId.Value : insuranceId;
                }

                line.AmountInvoiced = amountInvoiced;
                line.AmountInsurancePaidTotal = amountInsurancePaid;
                line.InsuranceId = insuranceId;
            }
        }

        public async Task _UpdateInvoiceQty(IEnumerable<Guid> ids)
        {
            var lines = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.SaleOrderLineInvoiceRels)
                .Include("SaleOrderLineInvoiceRels.InvoiceLine").Include("SaleOrderLineInvoiceRels.InvoiceLine.Invoice").ToListAsync();
            _GetInvoiceQty(lines);
            await UpdateAsync(lines);
        }

        public void _ComputeInvoiceStatus(IEnumerable<SaleOrderLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.State != "sale" && line.State != "done")
                    line.InvoiceStatus = "no";
                else if (line.QtyToInvoice != 0 || line.AmountToInvoice != 0)
                    line.InvoiceStatus = "to invoice";
                else if (line.QtyToInvoice == 0 && line.AmountToInvoice == 0)
                    line.InvoiceStatus = "invoiced";
                else
                    line.InvoiceStatus = "no";
            }
        }

        public async Task<IEnumerable<SaleOrderLine>> _ComputePaidResidual(IEnumerable<Guid> ids)
        {
            //tính toán lại số tiền đã thanh toán và còn nợ khi list SaleOrderLinePaymentRels thay đổi
            var self = SearchQuery(x => ids.Contains(x.Id)).Include(x => x.SaleOrderLinePaymentRels)
                .Include("SaleOrderLinePaymentRels.Payment").ToList();

            foreach (var line in self)
            {
                var amountPaid = 0M;
                foreach (var rel in line.SaleOrderLinePaymentRels)
                {
                    var payment = rel.Payment;
                    if (payment.State == "draft" || payment.State == "cancel")
                        continue;
                    amountPaid += (rel.AmountPrepaid ?? 0);
                }

                line.AmountPaid = amountPaid;
                line.AmountResidual = line.PriceSubTotal - amountPaid;
            }

            await UpdateAsync(self);
            return self;
        }

        public void _ComputeLinePaymentRels(IEnumerable<SaleOrderLine> self)
        {
            var self_ids = self.Select(x => x.Id).ToList();
            self = SearchQuery(x => self_ids.Contains(x.Id)).Include(x => x.SaleOrderLinePaymentRels).ToList();
            foreach (var line in self)
            {
                if (line.State != "draft")
                {
                    line.AmountPaid = line.AmountInvoiced;
                    line.AmountResidual = line.AmountToInvoice;
                }
                else
                {
                    line.AmountPaid = 0;
                    line.AmountResidual = 0;
                }
            }
        }

        public async Task _RemovePartnerCommissions(IEnumerable<Guid> ids)
        {
            var partnerCommission = GetService<ISaleOrderLinePartnerCommissionService>();
            var lines = await partnerCommission.SearchQuery(x => ids.Contains(x.SaleOrderLineId)).ToListAsync();
            await partnerCommission.DeleteAsync(lines);
        }

        public AccountInvoiceLine _PrepareInvoiceLine(SaleOrderLine line, decimal qty, AccountAccount account)
        {
            var res = new AccountInvoiceLine
            {
                Name = line.Name,
                Origin = line.Order.Name,
                AccountId = account.Id,
                PriceUnit = line.PriceUnit,
                Quantity = qty,
                Discount = line.Discount,
                UoMId = line.ProductUOMId,
                ProductId = line.ProductId,
            };

            return res;
        }

        public AccountMoveLine _PrepareInvoiceLine(SaleOrderLine self)
        {
            var res = new AccountMoveLine
            {
                Name = self.Name,
                ProductId = self.ProductId,
                ProductUoMId = self.ProductUOMId,
                Quantity = self.QtyToInvoice == 0 ? 1 : self.QtyToInvoice,
                Discount = self.Discount,
                PriceUnit = self.AmountToInvoice,
                DiscountType = self.DiscountType,
                DiscountFixed = self.DiscountFixed,
                SalesmanId = self.SalesmanId
            };

            res.SaleLineRels.Add(new SaleOrderLineInvoice2Rel { OrderLine = self });

            return res;
        }

        public void _ComputeAmountDiscountTotal(IEnumerable<SaleOrderLine> self)
        {
            //Trường hợp ưu đãi phiếu điều trị thì ko đúng, sum từ PromotionLines là đúng
            foreach (var line in self)
                line.AmountDiscountTotal = line.PromotionLines.Sum(x => x.PriceUnit);
        }
        // Check date is between range
        public async Task<PagedResult2<SaleOrderLineBasic>> GetPagedResultAsync(SaleOrderLinesPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.OrderPartner.Name.Contains(val.Search)
                                        || x.Product.NameNoSign.Contains(val.Search) || x.OrderPartner.NameNoSign.Contains(val.Search)
                                        || x.Order.Name.Contains(val.Search));
            if (val.OrderPartnerId.HasValue)
                query = query.Where(x => x.OrderPartnerId == val.OrderPartnerId);
            if (val.ProductId.HasValue)
                query = query.Where(x => x.ProductId == val.ProductId);
            if (val.OrderId.HasValue)
                query = query.Where(x => x.OrderId == val.OrderId);
            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                query = query.Where(x => states.Contains(x.State));
            }

            if (val.DateOrderFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateOrderFrom.Value.AbsoluteBeginOfDate());
            if (val.DateOrderTo.HasValue)
                query = query.Where(x => x.Date <= val.DateOrderTo.Value.AbsoluteEndOfDate());

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value.AbsoluteEndOfDate());

            if (val.IsLabo == true)
                query = query.Where(x => x.Product.IsLabo);

            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            if (val.IsQuotation.HasValue)
            {
                query = query.Where(x => x.Order.IsQuotation == val.IsQuotation);
            }

            if (!string.IsNullOrEmpty(val.LaboState))
            {
                query = query.Where(x => x.Labos.Any(s => s.State == val.LaboState));
            }

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.OrderPartnerId == val.PartnerId);

            var totalItems = await query.CountAsync();

            // Calculate the aggregates
            var aggregate = query.ProcessAggregates(val.Aggregate);

            query = query.OrderByDescending(x => x.DateCreated);
            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await _mapper.ProjectTo<SaleOrderLineBasic>(query).ToListAsync();

            return new PagedResult2<SaleOrderLineBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items,
                Aggregates = aggregate
            };
        }

        public override ISpecification<SaleOrderLine> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "sale.sale_order_line_comp_rule":
                    return new InitialSpecification<SaleOrderLine>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            //var couponObj = GetService<ISaleCouponService>();
            //var saleObj = GetService<ISaleOrderService>();
            //var programObj = GetService<ISaleCouponProgramService>();
            //var saleCardRelObj = GetService<ISaleOrderServiceCardCardRelService>();
            //var serviceCardObj = GetService<IServiceCardCardService>();


            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();

            if (self.Any(x => x.AmountInvoiced > 0))
                throw new Exception("Bạn không thể xóa dịch vụ đã thanh toán");

            //foreach (var line in self.Where(x => x.IsRewardLine))
            //{
            //    var appliedCoupons = await couponObj.SearchQuery(x => x.SaleOrderId == line.OrderId)
            //        .Include(x => x.Program).ToListAsync();

            //    var coupons_to_reactivate = appliedCoupons.Where(x => x.Program.DiscountLineProductId == line.ProductId).ToList();
            //    foreach (var coupon in coupons_to_reactivate)
            //    {
            //        coupon.State = "new";
            //        coupon.SaleOrderId = null;
            //    }

            //    await couponObj.UpdateAsync(coupons_to_reactivate);

            //    var related_program = await programObj.SearchQuery(x => x.DiscountLineProductId == line.ProductId).ToListAsync();
            //    if (related_program.Any())
            //    {
            //        foreach (var program in related_program)
            //        {
            //            var promo_programs = await _dbContext.SaleOrderNoCodePromoPrograms.Where(x => x.ProgramId == program.Id).ToListAsync();
            //            _dbContext.SaleOrderNoCodePromoPrograms.RemoveRange(promo_programs);
            //            await _dbContext.SaveChangesAsync();

            //            if (program.Id == line.Order.CodePromoProgramId)
            //                line.Order.CodePromoProgramId = null;
            //        }
            //    }

            //    await saleObj.UpdateAsync(line.Order);

            //    var card_rels_to_unlink = await saleCardRelObj.SearchQuery(x => x.SaleOrderId == line.OrderId && x.Card.CardType.ProductId == line.ProductId).ToListAsync();
            //    var card_ids = card_rels_to_unlink.Select(x => x.CardId).Distinct().ToList();
            //    await saleCardRelObj.DeleteAsync(card_rels_to_unlink);

            //    await serviceCardObj._ComputeResidual(card_ids);

            //    var promotionObj = GetService<ISaleOrderPromotionService>();
            //    var promotionIds = await promotionObj.SearchQuery(x => ids.Contains(x.SaleOrderLineId.Value) && x.SaleOrderId.HasValue).Select(x => x.Id).ToListAsync();
            //    if (promotionIds.Any())
            //        await promotionObj.RemovePromotion(promotionIds);

            //}

            await DeleteAsync(self);
        }

        public async Task CancelSaleOrderLine(IEnumerable<Guid> ids)
        {
            var orderObj = GetService<ISaleOrderService>();
            var dotkhamstepObj = GetService<IDotKhamStepService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var lines = await SearchQuery(x => ids.Contains(x.Id)).Include("DotKhamSteps").Include(x => x.Order)
                .Include(x => x.PartnerCommissions)
                .Include(x => x.Product)
                .Include(x => x.SaleOrderLinePaymentRels)
               .Include(x => x.SaleOrderLineInvoice2Rels)
               .Include("SaleOrderLineInvoice2Rels.InvoiceLine")
               .Include("SaleOrderLineInvoice2Rels.InvoiceLine.Move").ToListAsync();

            foreach (var line in lines)
            {
                if (line.SaleOrderLinePaymentRels.Any())
                    throw new Exception("Dịch vụ đã thanh toán không thể hủy");

                //Nếu có đợt khám step nào đã hoàn thành thì ko đc hủy
                if (line.DotKhamSteps.Any(x => x.IsDone))
                    throw new Exception("Không thể hủy dịch vụ đang được thực hiện");

                line.ProductUOMQty = 0;
                line.State = "cancel";
                if (line.DotKhamSteps.Any())
                {
                    await dotkhamstepObj.Unlink(line.DotKhamSteps);
                }

                line.PartnerCommissions.Clear(); //xóa hết commission
                line.IsCancelled = true;
            }

            await UpdateAsync(lines);

            ComputeAmount(lines);
            _GetInvoiceQty(lines);
            _GetToInvoiceQty(lines);
            _ComputeInvoiceStatus(lines);
            _ComputeLinePaymentRels(lines);
            await UpdateAsync(lines);

            var orderId = lines.Select(x => x.OrderId).FirstOrDefault();
            var order = await orderObj.GetSaleOrderWithLines(orderId);

            //tính lại tổng tiền phiếu điều trị
            orderObj._AmountAll(order);
            orderObj._GetInvoiced(new List<SaleOrder>() { order });
            await orderObj.UpdateAsync(order);

            // tính lại công nợ
            if (order.InvoiceStatus == "to invoice")
            {
                await orderObj.ActionInvoiceCreateV2(orderId);
            }
        }

        public async Task<IEnumerable<LaboOrderBasic>> GetLaboOrderBasics(Guid id)
        {
            var laboOrderLineObj = GetService<ILaboOrderLineService>();
            var order_ids = await laboOrderLineObj.SearchQuery(x => x.SaleOrderLineId == id).Select(x => x.OrderId).Distinct().ToListAsync();

            var laboOrderObj = GetService<ILaboOrderService>();
            var res = await _mapper.ProjectTo<LaboOrderBasic>(laboOrderObj.SearchQuery(x => order_ids.Contains(x.Id), orderBy: x => x.OrderByDescending(s => s.DateCreated))).ToListAsync();
            return res;
        }

        public async Task<IEnumerable<ToothBasic>> GetTeeth(Guid id)
        {
            var tooth_ids = await SearchQuery(x => x.Id == id).SelectMany(x => x.SaleOrderLineToothRels).Select(x => x.ToothId).ToListAsync();
            var toothObj = GetService<IToothService>();
            var res = await _mapper.ProjectTo<ToothBasic>(toothObj.SearchQuery(x => tooth_ids.Contains(x.Id), orderBy: x => x.OrderBy(s => s.Name))).ToListAsync();
            return res;
        }

        public async Task RecomputeCommissions(IEnumerable<SaleOrderLine> self)
        {
            //recompute thêm những dòng xác định người đc hưởng hoa hồng và bảng tính hoa hồng
            var self_ids = self.Select(x => x.Id).ToList();
            self = await SearchQuery(x => self_ids.Contains(x.Id)).Include(x => x.PartnerCommissions).ToListAsync();

            var commissionLineObj = GetService<ISaleOrderLinePartnerCommissionService>();
            var commission_lines = new List<SaleOrderLinePartnerCommission>();
            foreach (var line in self)
            {
                await commissionLineObj.DeleteAsync(line.PartnerCommissions);

                //add salesman commission
                var salesmanCommission = await _PrepareSalesmanCommission(line);
                if (salesmanCommission != null)
                    commission_lines.Add(salesmanCommission);
            }

            await commissionLineObj.CreateAsync(commission_lines);
        }

        public async Task<SaleOrderLinePartnerCommission> _PrepareSalesmanCommission(SaleOrderLine self)
        {
            if (!self.EmployeeId.HasValue)
                return null;

            var employeeObj = GetService<IEmployeeService>();
            var employee = await employeeObj.SearchQuery(x => x.Id == self.EmployeeId).FirstOrDefaultAsync();

            if (employee == null || !employee.CommissionId.HasValue)
                return null;

            var res = new SaleOrderLinePartnerCommission
            {
                SaleOrderLineId = self.Id,
                CommissionId = employee.CommissionId,
                EmployeeId = employee.Id
            };

            return res;
        }

        public async Task<IEnumerable<SaleOrderLineDisplay>> GetDisplayBySaleOrder(Guid Id)
        {
            var lines = await SearchQuery(x => x.OrderId == Id)
                .Include(x => x.Employee)
                .Include(x => x.Product)
                .Include("SaleOrderLineToothRels.Tooth")
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleOrderLineDisplay>>(lines);
        }

        public async Task<IEnumerable<SaleOrderLine>> GetOrderLinesBySaleOrderId(Guid orderId)
        {
            var lines = await SearchQuery(x => (orderId == null || x.OrderId == orderId))
             .Include(x => x.Advisory)
             .Include(x => x.Assistant)
             .Include(x => x.PromotionLines).ThenInclude(x => x.Promotion)
             .Include(x => x.Product)
             .Include(x => x.ToothCategory)
             .Include(x => x.SaleOrderLineToothRels).ThenInclude(x => x.Tooth)
             .Include(x => x.Employee)
             .Include(x => x.Counselor)
             .Include(x => x.OrderPartner).ToListAsync();

            return lines;
        }

        public async Task UpdateDkByOrderLine(Guid key, SaleOrderLineDotKhamSave val)
        {
            var dkStepobj = GetService<IDotKhamStepService>();
            var productObj = GetService<IProductService>();
            var productStepObj = GetService<IProductStepService>();
            var line = await SearchQuery(x => x.Id == key).Include(x => x.DotKhamSteps).FirstOrDefaultAsync();
            var dksAdd = new List<DotKhamStep>();
            var dksRemove = new List<DotKhamStep>();
            var dksUpdate = new List<DotKhamStep>();

            foreach (var item in line.DotKhamSteps.ToList())
            {
                if (!val.Steps.Any(x => x.Id == item.Id))
                    dksRemove.Add(item);
            }

            var order = 1;
            foreach (var item in val.Steps.ToList())
            {
                if (!item.Id.HasValue || item.Id.Value == Guid.Empty)
                {
                    dksAdd.Add(new DotKhamStep
                    {
                        Name = item.Name,
                        Order = order++,
                        ProductId = line.ProductId,
                        SaleLineId = line.Id,
                        SaleOrderId = line.OrderId
                    });
                }
                else
                {
                    var step = line.DotKhamSteps.Where(x => x.Id == item.Id).FirstOrDefault();
                    step.Name = item.Name;
                    step.Order = order++;
                    dksUpdate.Add(step);
                }
            }

            await dkStepobj.CreateAsync(dksAdd);
            await dkStepobj.Unlink(dksRemove);
            await dkStepobj.UpdateAsync(dksUpdate);

            if (val.Default)
            {
                var service = await productObj.SearchQuery(x => x.Id == line.ProductId).Include(x => x.Steps).FirstOrDefaultAsync();
                var stepsAdd = new List<ProductStep>();
                foreach (var item in val.Steps)
                {
                    stepsAdd.Add(new ProductStep
                    {
                        Name = item.Name,
                        Order = item.Order,
                        Active = true,
                        Default = true,
                        ProductId = service.Id,
                    });
                }
                await productStepObj.DeleteAsync(service.Steps.ToList());
                await productStepObj.CreateAsync(stepsAdd);
            }
        }

        public async Task CreateSaleProduction(IEnumerable<SaleOrderLine> self)
        {
            var saleProductObj = GetService<ISaleProductionService>();
            var bomObj = GetService<IProductBomService>();

            var products = self.GroupBy(x => x.ProductId.Value)
                .Select(x => new
                {
                    ProductId = x.Key,
                    Quantity = x.Sum(s => s.ProductUOMQty),
                    LineIds = x.Select(s => s.Id).ToList(),
                }).ToList();

            var productIds = products.Select(x => x.ProductId).ToList();
            var boms = await bomObj.SearchQuery(x => productIds.Contains(x.ProductId)).ToListAsync();
            var bom_dict = boms.GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.ToList());
            var saleProductions = new List<SaleProduction>();
            foreach (var item in products)
            {
                if (!bom_dict.ContainsKey(item.ProductId))
                    continue;

                var saleProduction = new SaleProduction
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    CompanyId = CompanyId
                };

                foreach (var line in bom_dict[item.ProductId])
                {
                    saleProduction.Lines.Add(new SaleProductionLine
                    {
                        ProductId = line.MaterialProductId.Value,
                        ProductUOMId = line.ProductUOMId,
                        Quantity = item.Quantity * line.Quantity
                    });
                }

                foreach (var lineId in item.LineIds)
                {
                    saleProduction.SaleOrderLineRels.Add(new SaleOrderLineSaleProductionRel { OrderLineId = lineId });
                }

                saleProductions.Add(saleProduction);
            }

            await saleProductObj.CreateAsync(saleProductions);
        }

        public async Task ComputeProductRequestedQuantity(IEnumerable<Guid> ids) //compute quantity after do something
        {
            var reqLineObj = GetService<IProductRequestLineService>();
            var requestedObj = GetService<ISaleOrderLineProductRequestedService>();

            var requestLines = await reqLineObj.SearchQuery(x => ids.Contains(x.SaleOrderLineId.Value) && x.Request.State != "draft")
                .GroupBy(x => new { SaleOrderLineId = x.SaleOrderLineId.Value, ProductId = x.ProductId.Value })
                .Select(x => new
                {
                    SaleOrderLineId = x.Key.SaleOrderLineId,
                    ProductId = x.Key.ProductId,
                    Total = x.Sum(s => s.ProductQty)
                })
                .ToListAsync();

            var requesteds = await requestedObj.SearchQuery(x => ids.Contains(x.SaleOrderLineId)).ToListAsync();
            //
            var toCreateRequested = new List<SaleOrderLineProductRequested>();
            var toDeleteRequested = new List<SaleOrderLineProductRequested>();
            var toUpdateRequested = new List<SaleOrderLineProductRequested>();

            foreach (var item in requesteds)
            {
                if (!requestLines.Any(x => x.SaleOrderLineId == item.SaleOrderLineId))
                    toDeleteRequested.Add(item);
            }

            foreach (var item in requestLines)
            {
                var existRequested = requesteds.FirstOrDefault(x => x.SaleOrderLineId == item.SaleOrderLineId);
                if (existRequested == null)
                {
                    var requested = new SaleOrderLineProductRequested
                    {
                        SaleOrderLineId = item.SaleOrderLineId,
                        ProductId = item.ProductId,
                        RequestedQuantity = item.Total
                    };
                    toCreateRequested.Add(requested);
                }
                else
                {
                    existRequested.RequestedQuantity = item.Total;
                    toUpdateRequested.Add(existRequested);
                }
            }

            //save changes
            if (toCreateRequested.Any())
                await requestedObj.CreateAsync(toCreateRequested);
            if (requesteds.Any())
                await requestedObj.UpdateAsync(requesteds);
            if (toDeleteRequested.Any())
                await requestedObj.DeleteAsync(toDeleteRequested);
        }



        public async Task ApplyDiscountOnOrderLine(ApplyDiscountViewModel val)
        {
            var orderPromotionObj = GetService<ISaleOrderPromotionService>();
            var orderObj = GetService<ISaleOrderService>();

            var orderLine = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.Order).ThenInclude(x => x.OrderLines)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.PromotionLines)
                .FirstOrDefaultAsync();

            var total = orderLine.PriceUnit * orderLine.ProductUOMQty;
            var price_reduce = val.DiscountType == "percentage" ? orderLine.PriceUnit * (1 - val.DiscountPercent / 100) : orderLine.PriceUnit - val.DiscountFixed;
            var discount_amount = (orderLine.PriceUnit - price_reduce) * orderLine.ProductUOMQty;

            var promotion = orderPromotionObj.SearchQuery(x => x.SaleOrderLineId == orderLine.Id && x.Type == "discount" && x.SaleOrderId.HasValue).FirstOrDefault();
            if (promotion != null)
            {
                promotion.Amount = (discount_amount ?? 0);
            }
            else
            {
                promotion = new SaleOrderPromotion
                {
                    Name = "Giảm tiền",
                    Amount = (discount_amount ?? 0),
                    DiscountType = val.DiscountType,
                    DiscountPercent = val.DiscountPercent,
                    DiscountFixed = val.DiscountFixed,
                    Type = "discount",
                    SaleOrderLineId = orderLine.Id,
                    SaleOrderId = orderLine.OrderId
                };

                promotion.Lines.Add(new SaleOrderPromotionLine
                {
                    SaleOrderLineId = promotion.SaleOrderLineId.Value,
                    Amount = promotion.Amount,
                    PriceUnit = (double)(orderLine.ProductUOMQty != 0 ? (promotion.Amount / orderLine.ProductUOMQty) : 0),
                });

                await orderPromotionObj.CreateAsync(promotion);
            }

            //tính lại tổng tiền ưu đãi saleorderlines
            _ComputeAmountDiscountTotal(new List<SaleOrderLine>() { orderLine });
            ComputeAmount(new List<SaleOrderLine>() { orderLine });

            orderObj._AmountAll(orderLine.Order);
            await orderObj.UpdateAsync(orderLine.Order);
        }

        public async Task ApplyPromotionOnOrderLine(ApplyPromotionRequest val)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var orderLine = await SearchQuery(x => x.Id == val.Id)
              .Include(x => x.Product)
              .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
              .Include(x => x.Order).ThenInclude(x => x.Promotions)
              .Include(x => x.Order).ThenInclude(x => x.OrderLines)
              .Include(x => x.Order).ThenInclude(x => x.Partner)
              .Include(x => x.PromotionLines)
              .Include(x => x.SaleOrderLineInvoice2Rels)
              .FirstOrDefaultAsync();

            var program = await programObj.SearchQuery(x => x.Id == val.SaleProgramId)
                .Include(x => x.DiscountSpecificProducts).ThenInclude(x => x.Product)
                .Include(x => x.DiscountSpecificProductCategories).ThenInclude(x => x.ProductCategory)
                .Include(x => x.DiscountSpecificPartners)
                .Include(x => x.DiscountCardTypes)
                .FirstOrDefaultAsync();

            if (program != null)
            {
                var error_status = await programObj._CheckPromotionApplySaleLine(program, orderLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateRewardLine(orderLine, program);

                }
                else
                    throw new Exception(error_status.Error);
            }
            else
            {
                throw new Exception("Mã khuyễn mãi không chính xác");
            }
        }

        public async Task ApplyServiceCardCard(ApplyServiceCardCardRequest val)
        {
            var serviceCardObj = GetService<IServiceCardCardService>();
            var orderLine = await SearchQuery(x => x.Id == val.Id)
                    .Include(x => x.Product)
                    .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                    .Include(x => x.Order).ThenInclude(x => x.Promotions)
                    .Include(x => x.Order).ThenInclude(x => x.OrderLines)
                    .Include(x => x.Order).ThenInclude(x => x.Partner)
                    .Include(x => x.PromotionLines)
                    .Include(x => x.SaleOrderLineInvoice2Rels)
                    .FirstOrDefaultAsync();

            var serviceCard = await serviceCardObj.SearchQuery(x => x.Id == val.ServiceCardId)
                .Include(x => x.CardType).ThenInclude(x => x.ProductPricelist).ThenInclude(x => x.Items)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();


            if (serviceCard != null)
            {
                var error_status = serviceCardObj._CheckServiceCardCardApplySaleLine(serviceCard, orderLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateServiceCardCardRewardLine(orderLine, serviceCard);

                }
                else
                    throw new Exception(error_status.Error);
            }
            else
            {
                throw new Exception("Không tìm thấy thẻ ưu đãi");
            }

        }

        public async Task ApplyCardCard(ApplyCardCardRequest val)
        {
            var cardCardObj = GetService<ICardCardService>();
            var orderLine = await SearchQuery(x => x.Id == val.Id)
                    .Include(x => x.Product)
                    .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                    .Include(x => x.Order).ThenInclude(x => x.Promotions)
                    .Include(x => x.Order).ThenInclude(x => x.OrderLines)
                    .Include(x => x.Order).ThenInclude(x => x.Partner)
                    .Include(x => x.PromotionLines)
                    .Include(x => x.SaleOrderLineInvoice2Rels)
                    .FirstOrDefaultAsync();

            var cardCard = await cardCardObj.SearchQuery(x => x.Id == val.CardId)
                .Include(x => x.Type).ThenInclude(x => x.Pricelist).ThenInclude(x => x.Items)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();


            if (cardCard != null)
            {
                var error_status = cardCardObj._CheckCardCardApplySaleLine(cardCard, orderLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateCardCardRewardLine(orderLine, cardCard);

                }
                else
                    throw new Exception(error_status.Error);
            }
            else
            {
                throw new Exception("Không tìm thấy thẻ thành viên");
            }

        }

        public async Task<SaleCouponProgramResponse> ApplyPromotionUsageCodeOnOrderLine(ApplyPromotionUsageCode val)
        {
            var couponCode = val.CouponCode;
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var orderLine = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.PromotionLines)
                .Include(x => x.Product)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.Order).ThenInclude(x => x.Promotions)
                .Include(x => x.Order).ThenInclude(x => x.OrderLines)
                .Include(x => x.Order).ThenInclude(x => x.Partner)
                .Include(x => x.SaleOrderLineInvoice2Rels)
                .FirstOrDefaultAsync();

            //Chương trình khuyến mãi sử dụng mã
            var program = await programObj.SearchQuery(x => x.PromoCode == val.CouponCode && x.Active)
                .Include(x => x.DiscountSpecificProducts).ThenInclude(x => x.Product)
                .Include(x => x.DiscountSpecificProductCategories).ThenInclude(x => x.ProductCategory)
                .Include(x => x.DiscountSpecificPartners)
                .Include(x => x.DiscountCardTypes)
                .FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckPromotionApplySaleLine(program, orderLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateRewardLine(orderLine, program);

                    return new SaleCouponProgramResponse { Error = null, Success = true, SaleCouponProgram = _mapper.Map<SaleCouponProgramDisplay>(program) };
                }
                else
                    //throw new Exception(error_status.Error);
                    return new SaleCouponProgramResponse { Error = error_status.Error, Success = false, SaleCouponProgram = null };
            }
            else
            {
                return new SaleCouponProgramResponse { Error = "Mã khuyến mãi không chính xác", Success = false, SaleCouponProgram = null };

            }
        }

        private async Task _CreateRewardLine(SaleOrderLine self, SaleCouponProgram program)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderPromotionObj = GetService<ISaleOrderPromotionService>();
            var promotion = _GetRewardLineValues(self, program);
            //foreach (var line in lines)
            //    line.Order = self;

            await orderPromotionObj.CreateAsync(promotion);

            //tính lại tổng tiền ưu đãi saleorderlines
            _ComputeAmountDiscountTotal(new List<SaleOrderLine>() { self });
            ComputeAmount(new List<SaleOrderLine>() { self });

            orderObj._AmountAll(self.Order);
            await orderObj.UpdateAsync(self.Order);
        }

        private async Task _CreateServiceCardCardRewardLine(SaleOrderLine self, ServiceCardCard serviceCard)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderPromotionObj = GetService<ISaleOrderPromotionService>();
            var promotion = _GetServiceCardValuesDiscount(self, serviceCard);
            //foreach (var line in lines)
            //    line.Order = self;

            await orderPromotionObj.CreateAsync(promotion);

            //tính lại tổng tiền ưu đãi saleorderlines
            _ComputeAmountDiscountTotal(new List<SaleOrderLine>() { self });
            ComputeAmount(new List<SaleOrderLine>() { self });

            orderObj._AmountAll(self.Order);
            await orderObj.UpdateAsync(self.Order);
        }

        //public async Task<bool> CheckCoditionApplyable(SaleOrderLine self)
        //{
        //    var res = false;
        //    var prmotion = 
        //}


        private async Task _CreateCardCardRewardLine(SaleOrderLine self, CardCard card)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderPromotionObj = GetService<ISaleOrderPromotionService>();
            var promotion = _GetCardValuesDiscount(self, card);


            await orderPromotionObj.CreateAsync(promotion);

            //tính lại tổng tiền ưu đãi saleorderlines
            _ComputeAmountDiscountTotal(new List<SaleOrderLine>() { self });
            ComputeAmount(new List<SaleOrderLine>() { self });

            orderObj._AmountAll(self.Order);
            await orderObj.UpdateAsync(self.Order);
        }

        public SaleOrderPromotion _GetServiceCardValuesDiscount(SaleOrderLine self, ServiceCardCard serviceCard)
        {
            var promotionObj = GetService<ISaleOrderPromotionService>();
            var programObj = GetService<ISaleCouponProgramService>();
            var productObj = GetService<IProductService>();

            var pricelistItem = serviceCard.CardType.ProductPricelist.Items.Where(x => x.ProductId == self.ProductId).FirstOrDefault();

            if (pricelistItem.ComputePrice == "fixed_amount")
            {
                var fixedAmount = pricelistItem.FixedAmountPrice ?? 0;
                var discountAmount = _GetValuesDiscountFixedAmountOrderLine(self, fixedAmount);
                var promotionLineFixed = promotionObj.PrepareServiceCardToOrderLine(self, serviceCard, discountAmount);

                return promotionLineFixed;
            }

            var percentNumber = pricelistItem.PercentPrice ?? 0;
            var discount_amount = _GetValuesDiscountPercentageOrderLine(self, percentNumber);
            var promotionLine = promotionObj.PrepareServiceCardToOrderLine(self, serviceCard, discount_amount);

            return promotionLine;

        }

        public SaleOrderPromotion _GetCardValuesDiscount(SaleOrderLine self, CardCard card)
        {
            var promotionObj = GetService<ISaleOrderPromotionService>();
            var programObj = GetService<ISaleCouponProgramService>();
            var productObj = GetService<IProductService>();

            var pricelistItem = card.Type.Pricelist.Items.Where(x => x.ProductId == self.ProductId).FirstOrDefault();

            if (pricelistItem.ComputePrice == "fixed_amount")
            {
                var fixedAmount = pricelistItem.FixedAmountPrice ?? 0;
                var discountAmount = _GetValuesDiscountFixedAmountOrderLine(self, fixedAmount);
                var promotionLineFixed = promotionObj.PrepareCardCardToOrderLine(self, card, discountAmount);

                return promotionLineFixed;
            }

            var percentNumber = pricelistItem.PercentPrice ?? 0;
            var discount_amount = _GetValuesDiscountPercentageOrderLine(self, percentNumber);
            var promotionLine = promotionObj.PrepareCardCardToOrderLine(self, card, discount_amount);

            return promotionLine;

        }

        private decimal _GetValuesDiscountFixedAmountOrderLine(SaleOrderLine self, decimal fixedAmount)
        {
            var price_reduce = self.PriceUnit - fixedAmount;
            var fixed_amount = (self.PriceUnit - price_reduce) * self.ProductUOMQty;
            return fixed_amount;
        }

        public decimal _GetValuesDiscountPercentageOrderLine(SaleOrderLine line, decimal percentNumber)
        {
            //discount_amount = so luong * don gia da giam * phan tram
            var price_reduce = (line.PriceUnit * (1 - line.Discount / 100)) *
                (1 - percentNumber / 100);
            var discount_amount = (line.PriceUnit - price_reduce) * line.ProductUOMQty;

            return discount_amount;
        }

        public decimal _GetRewardValuesDiscountPercentagePerOrderLine(SaleCouponProgram program, SaleOrderLine line)
        {
            //discount_amount = so luong * don gia da giam * phan tram
            var price_reduce = (line.PriceUnit * (1 - line.Discount / 100)) *
                (1 - (program.DiscountPercentage ?? 0) / 100);
            var discount_amount = (line.PriceUnit - price_reduce) * line.ProductUOMQty;

            if (program.IsApplyMaxDiscount && program.DiscountMaxAmount.HasValue && program.DiscountMaxAmount.Value > 0)
            {
                if (discount_amount >= program.DiscountMaxAmount)
                    discount_amount = program.DiscountMaxAmount.Value;
            }

            return discount_amount;
        }

        public decimal _GetRewardValuesDiscountPercentagePerLine(SaleCouponProgram program, SaleOrderLine line)
        {
            var total = line.PriceUnit * line.ProductUOMQty;
            //discount_amount = so luong * don gia da giam * phan tram        
            var discount_amount = (total * (1 - line.Discount / 100)) *
                ((program.DiscountPercentage ?? 0) / 100);

            return discount_amount;
        }

        private decimal _GetRewardValuesDiscountFixedAmountLine(SaleOrderLine self, SaleCouponProgram program)
        {
            var price_reduce = self.PriceUnit - (program.DiscountFixedAmount ?? 0);
            var fixed_amount = (self.PriceUnit - price_reduce) * self.ProductUOMQty;
            return fixed_amount;
        }


        public SaleOrderPromotion _GetRewardLineValues(SaleOrderLine self, SaleCouponProgram program)
        {
            if (program.RewardType == "discount")
                return _GetRewardValuesDiscount(self, program);

            return new SaleOrderPromotion();
        }

        public SaleOrderPromotion _GetRewardValuesDiscount(SaleOrderLine self, SaleCouponProgram program)
        {
            var promotionObj = GetService<ISaleOrderPromotionService>();
            var programObj = GetService<ISaleCouponProgramService>();
            var productObj = GetService<IProductService>();

            //if (program.DiscountLineProduct == null)
            //{
            //    var productObj = GetService<IProductService>();
            //    program.DiscountLineProduct = productObj.GetById(program.DiscountLineProductId);
            //}

            if (program.DiscountType == "fixed_amount")
            {

                var discountAmount = _GetRewardValuesDiscountFixedAmountLine(self, program);
                var promotionLine = promotionObj.PreparePromotionToOrderLine(self, program, discountAmount);

                return promotionLine;
            }


            var rewards = new List<SaleOrderPromotion>();
            if (program.DiscountApplyOn == "on_order")
                throw new Exception("Chương trình khuyến mãi không đúng định dạng");

            if (program.DiscountApplyOn == "specific_products")
            {
                var discount_specific_product_ids = program.DiscountSpecificProducts.Select(x => x.ProductId).ToList();
                //We should not exclude reward line that offer this product since we need to offer only the discount on the real paid product (regular product - free product)
                var free_product_lines = programObj.SearchQuery(x => x.RewardType == "product" && discount_specific_product_ids.Contains(x.RewardProductId.Value)).Select(x => x.DiscountLineProductId.Value).ToList();
                var tmp = discount_specific_product_ids.Union(free_product_lines);
                if (tmp.Contains(self.ProductId.Value))
                {
                    var discount_amount = _GetRewardValuesDiscountPercentagePerOrderLine(program, self);
                    var promotionLine = promotionObj.PreparePromotionToOrderLine(self, program, discount_amount);

                    return promotionLine;
                }
            }
            if (program.DiscountApplyOn == "specific_product_categories")
            {
                var discount_specific_categ_ids = program.DiscountSpecificProductCategories.Select(x => x.ProductCategoryId).ToList();
                var tmp = productObj.SearchQuery(x => discount_specific_categ_ids.Contains(x.CategId.Value)).Select(x => x.Id).ToList();
                var discount_amount = _GetRewardValuesDiscountPercentagePerOrderLine(program, self);
                var promotionLine = promotionObj.PreparePromotionToOrderLine(self, program, discount_amount);

                return promotionLine;
            }


            return new SaleOrderPromotion();
        }


        public void RecomputePromotionLine(IEnumerable<SaleOrderLine> self)
        {
            //vong lap chi tiet dieu tri
            foreach (var line in self)
            {
                if (line.Promotions.Any())
                {
                    foreach (var promotion in line.Promotions)
                    {
                        var total = line.PriceUnit * line.ProductUOMQty;
                        if (promotion.Type == "discount")
                        {
                            var price_reduce = promotion.DiscountType == "percentage" ? line.PriceUnit * (1 - (promotion.DiscountPercent ?? 0) / 100) : (line.PriceUnit - promotion.DiscountFixed ?? 0);
                            var discount_amount = (line.PriceUnit - price_reduce) * line.ProductUOMQty;
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
                            child.PriceUnit = (double)(line.ProductUOMQty != 0 ? (promotion.Amount / line.ProductUOMQty) : 0);
                        }
                    }
                }
            }



        }

        public async Task<PagedResult2<SmsCareAfterOrder>> GetPagedSmsCareAfterOrderAsync(SmsCareAfterOrderPaged val)
        {
            var query = SearchQuery(x => x.State == "done");
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.OrderPartner.Name.Contains(val.Search)
                    || x.OrderPartner.NameNoSign.Contains(val.Search)
                    || x.OrderPartner.Phone.Contains(val.Search));
            if (val.ProductId.HasValue)
                query = query.Where(x => x.ProductId == val.ProductId.Value);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Order.DateDone >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Order.DateDone <= dateTo);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var totalItems = await query.CountAsync();

            var items = await query.OrderByDescending(x => x.Order.DateDone).Skip(val.Offset).Take(val.Limit).Select(x => new SmsCareAfterOrder
            {
                PartnerId = x.OrderPartnerId,
                SaleOrderLineId = x.Id,
                PartnerName = x.OrderPartner.Name,
                PartnerPhone = x.OrderPartner.Phone,
                SaleOrderName = x.Order.Name,
                DoctorName = x.EmployeeId.HasValue ? x.Employee.Name : null,
                DateDone = x.Order.DateDone,
                ProductName = x.Product.Name
            }).ToListAsync();

            return new PagedResult2<SmsCareAfterOrder>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<ProductSimple>> GetProductSmsCareAfterOrder(DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var query = SearchQuery(x => x.State == "done");
            if (dateFrom.HasValue)
            {
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Order.DateDone >= dateFrom);
            }

            if (dateTo.HasValue)
            {
                dateTo = dateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Order.DateDone <= dateTo);
            }

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId);

            var result = await query.GroupBy(x => new
            {
                ProductName = x.Product.Name,
                ProductId = x.ProductId.Value,
                ProductNameNoSign = x.Product.NameNoSign
            }).Select(x => new ProductSimple
            {
                Id = x.Key.ProductId,
                Name = x.Key.ProductName,
                NameNoSign = x.Key.ProductNameNoSign
            }).ToListAsync();

            return result;
        }

        public async Task<Dictionary<DateTime, IEnumerable<SaleOrderLineHistoryRes>>> GetHistory(SaleOrderLineHistoryReq val)
        {
            var query = SearchQuery(x => x.Order.State == "sale" || x.Order.State == "done");

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.OrderPartnerId == val.PartnerId);
            var resList = await query.Include(x => x.Employee).Include(x => x.Order)
                .Include("SaleOrderLineToothRels.Tooth").ToListAsync();
            var resDict = resList.GroupBy(x => x.Order.DateOrder.Date, (key, val) => new
            {
                Key = key,
                Value = _mapper.Map<IEnumerable<SaleOrderLineHistoryRes>>(val)
            }).ToDictionary(x => x.Key, x => x.Value);
            return resDict;
        }

        public async Task UpdateOrderLine(Guid id, SaleOrderLineSave val)
        {
            var entity = await SearchQuery(x => x.Id == id).Include(x => x.SaleOrderLineToothRels).FirstOrDefaultAsync();
            if (entity == null)
                throw new Exception("Không tìm thấy dịch vụ!");
            _mapper.Map(val, entity);
            entity.SaleOrderLineToothRels.Clear();
            if (val.ToothType == "manual")
            {
                foreach (var toothId in val.ToothIds)
                {
                    entity.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                    {
                        ToothId = toothId
                    });
                }
            }

            //Tính toán lại thành tiền cho sale order line
            ComputeAmount(new List<SaleOrderLine>() { entity });
            await UpdateAsync(entity);

            //Tính toán lại tổng tiền ưu đãi phiếu điều trị nếu có
            var promotionLineObj = GetService<ISaleOrderPromotionLineService>();
            var orderPromotionIds = promotionLineObj.SearchQuery(x => x.SaleOrderLineId == entity.Id && x.Promotion.SaleOrderId.HasValue).Select(x => x.PromotionId).Distinct().ToList();
            if (orderPromotionIds.Any())
            {
                var promotionObj = GetService<ISaleOrderPromotionService>();
                await promotionObj.ComputeAmount(orderPromotionIds);
            }

            //Tính toán lại thành tiền, tổng giảm giá, tổng tiền, đã thanh toán, còn lại cho order
            var orderObj = GetService<ISaleOrderService>();
            var order = await orderObj.SearchQuery(x => x.Id == entity.OrderId)
               .Include(x => x.OrderLines)
               .FirstOrDefaultAsync();

            orderObj._AmountAll(order);
            await orderObj.UpdateAsync(order);

            if (order.AmountTotal < 0 || order.Residual < 0)
                throw new Exception("Không thể lưu phiếu điều trị khi số tiền còn lại bé hơn 0");
            await orderObj.UpdateAsync(order);
            //action done order
            var isOrderDone = (await SearchQuery(x => x.OrderId == entity.OrderId).AllAsync(x => x.State == "done" || x.State == "cancel"))
                && (await SearchQuery(x => x.OrderId == entity.OrderId).AnyAsync(x => x.State == "done"));
            if (isOrderDone)
                await orderObj.ActionDone(new List<Guid>() { entity.OrderId });
        }

        public async Task<SaleOrderLine> CreateOrderLine(SaleOrderLineSave val)
        {
            var mailMessageObj = GetService<IMailMessageService>();
            var orderObj = GetService<ISaleOrderService>();
            var order = await orderObj.SearchQuery(x => x.Id == val.OrderId)
               .Include(x => x.OrderLines).ThenInclude(x => x.OrderPartner)
               .FirstOrDefaultAsync();

            var saleLine = _mapper.Map<SaleOrderLine>(val);
            saleLine.Date = saleLine.Date ?? DateTime.Now;
            saleLine.State = order.State;

            if (val.ToothType == "manual")
            {
                foreach (var toothId in val.ToothIds)
                {
                    saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                    {
                        ToothId = toothId
                    });
                }
            }

            UpdateOrderInfo(new List<SaleOrderLine>() { saleLine }, order);
            _GetInvoiceQty(new List<SaleOrderLine>() { saleLine });
            _GetToInvoiceQty(new List<SaleOrderLine>() { saleLine });
            _GetInvoiceAmount(new List<SaleOrderLine>() { saleLine });
            _GetToInvoiceAmount(new List<SaleOrderLine>() { saleLine });
            _ComputeInvoiceStatus(new List<SaleOrderLine>() { saleLine });
            ComputeAmount(new List<SaleOrderLine>() { saleLine });

            //gán người giới thiệu nếu có
            if (saleLine.OrderPartnerId.HasValue)
            {
                var partnerObj = GetService<IPartnerService>();
                var partner = await partnerObj.GetByIdAsync(saleLine.OrderPartnerId.Value);
                saleLine.AgentId = partner.AgentId;
            }

            await CreateAsync(saleLine);

            orderObj._AmountAll(order);
            await orderObj.UpdateAsync(order);

            //action done order
            var isOrderDone = (await SearchQuery(x => x.OrderId == saleLine.OrderId).AllAsync(x => x.State == "done" || x.State == "cancel"))
                && (await SearchQuery(x => x.OrderId == saleLine.OrderId).AnyAsync(x => x.State == "done"));
            if (isOrderDone)
                await orderObj.ActionDone(new List<Guid>() { saleLine.OrderId });

            if (saleLine.State != "draft")
                await GenerateActionLogSaleOrderLine(saleLine, null, order.Name);

            return saleLine;
        }

        public async Task RemoveOrderLine(Guid id)
        {
            var saleLine = await SearchQuery(x => x.Id == id).Include(x => x.SaleOrderLineToothRels).Include(x => x.SaleProductionRels).FirstOrDefaultAsync();
            if (saleLine == null)
                throw new Exception("Không tìm thấy dịch vụ!");

            var dkStepObj = GetService<IDotKhamStepService>();
            var removeDkSteps = await dkStepObj.SearchQuery(x => x.SaleLineId.Value == id).ToListAsync();
            if (removeDkSteps.Any(x => x.IsDone))
                throw new Exception("Đã có công đoạn đợt khám hoàn thành, không thể hủy");

            await dkStepObj.DeleteAsync(removeDkSteps);

            //Xóa ưu đãi dịch vụ
            var promotionObj = GetService<ISaleOrderPromotionService>();
            var promotions = await promotionObj.SearchQuery(x => x.SaleOrderLineId.Value == id).ToListAsync();
            await promotionObj.DeleteAsync(promotions);

            //Xóa các dòng phân bổ từ ưu đãi phiếu điều trị
            var promotionLineObj = GetService<ISaleOrderPromotionLineService>();
            var promotionLines = await promotionLineObj.SearchQuery(x => x.SaleOrderLineId == id).ToListAsync();
            if (promotionLines.Any())
            {
                var recomputePromotionIds = promotionLines.Select(x => x.PromotionId).Distinct().ToList();
                await promotionLineObj.DeleteAsync(promotionLines);

                //Tính lại tổng tiền của ưu đãi phiếu điều trị
                await promotionObj.ComputeAmount(recomputePromotionIds);
            }


            await Unlink(new List<Guid>() { id });


            //compute sale order
            var orderObj = GetService<ISaleOrderService>();
            var order = await orderObj.SearchQuery(x => x.Id == saleLine.OrderId)
               .Include(x => x.OrderLines)
               .FirstOrDefaultAsync();

            orderObj._AmountAll(order);
            await orderObj.UpdateAsync(order);
        }

        public async Task UpdateState(Guid id, string state)
        {
            var line = await SearchQuery(x => x.Id == id).Include(x => x.Order).Include(x => x.OrderPartner).FirstOrDefaultAsync();
            var oldLineState = line.State;
            line.State = state;
            if (line.State == "done")
                line.DateDone = DateTime.Now;
            await UpdateAsync(line);

            //action done saleorder
            var isOrderDone = (await SearchQuery(x => x.OrderId == line.OrderId).AllAsync(x => x.State == "done" || x.State == "cancel"))
                && (await SearchQuery(x => x.OrderId == line.OrderId).AnyAsync(x => x.State == "done"));
            var orderObj = GetService<ISaleOrderService>();
            if (isOrderDone)
                await orderObj.ActionDone(new List<Guid>() { line.OrderId });

            await GenerateActionLogSaleOrderLine(line, oldLineState, line.Order.Name);
        }

        public async Task GenerateActionLogSaleOrderLine(SaleOrderLine line, string oldState = null, string orderName = null)
        {
            var threadMessageObj = GetService<IMailThreadMessageService>();
            var content = "";
            if ((oldState == "cancel" || oldState == "done") && line.State == "sale")
                content = "Tiếp tục điều trị dịch vụ";
            if ((oldState == "sale" || string.IsNullOrEmpty(oldState)) && line.State == "done")
                content = "Hoàn thành dịch vụ";
            if (oldState == "sale" && line.State == "cancel")
                content = "Ngừng điều trị dịch vụ";
            if ((oldState == "sale" || string.IsNullOrEmpty(oldState)) && line.State == "sale")
                content = "Sử dụng dịch vụ";

            var bodySaleOrderLine = string.Format("{0} <b>{1}</b> - phiếu điều trị <b>{2}</b>", content, line.Name, orderName);
            await threadMessageObj.MessagePost(line.OrderPartner, body: bodySaleOrderLine, subjectTypeId: "mail.subtype_sale_order_line");
        }


        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var line in self)
            {
                line.State = "done";
                line.DateDone = DateTime.Now;
            }

            var orderIds = self.Select(x => x.OrderId).Distinct().ToList();
            await _ComputeOrderState(orderIds);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var line in self)
                line.State = "cancel";

            await UpdateAsync(self);

            var orderIds = self.Select(x => x.OrderId).Distinct().ToList();
            await _ComputeOrderState(orderIds);
        }

        public async Task ActionUnlock(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var line in self)
                line.State = "sale";

            await UpdateAsync(self);

            var orderIds = self.Select(x => x.OrderId).Distinct().ToList();
            await _ComputeOrderState(orderIds);
        }

        private async Task _ComputeOrderState(IEnumerable<Guid> ids)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orders = await orderObj.SearchQuery(x => ids.Contains(x.Id))
               .Include(x => x.OrderLines)
               .ToListAsync();

            foreach (var order in orders)
            {
                if (order.OrderLines.All(x => x.State == "done" || x.State == "cancel"))
                {
                    if (order.OrderLines.Any(x => x.State == "done"))
                    {
                        order.State = "done";
                        order.DateDone = DateTime.Now;
                    }
                    else
                    {
                        order.State = "sale";
                    }
                }
                else
                {
                    order.State = "sale";
                }
            }

            await orderObj.UpdateAsync(orders);
        }
        public async Task<ServiceSaleReportPrint> SaleReportPrint(SaleOrderLinesPaged val)
        {
            val.Limit = 0;
            var resPage = await GetPagedResultAsync(val);
            var companyObj = GetService<ICompanyService>();
            var res = new ServiceSaleReportPrint()
            {
                data = resPage.Items,
                DateFrom = val.DateOrderFrom,
                DateTo = val.DateOrderTo,
                User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId))
            };
            if (val.CompanyId.HasValue)
            {
                var company = await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.Company = _mapper.Map<CompanyPrintVM>(company);
            }
            return res;
        }

        public async Task DebtPayment(Guid id)
        {
            var saleLine = await SearchQuery(x => x.Id == id).Include(x => x.Order).FirstOrDefaultAsync();
            if (saleLine == null)
                throw new Exception("Không tìm thấy dịch vụ");
            var amountToPay = saleLine.PriceTotal - saleLine.AmountInvoiced;
            //tạo saleorderpayment
            var journalObj = GetService<IAccountJournalService>();
            var soPaymentObj = GetService<ISaleOrderPaymentService>();
            var debtJournal = await journalObj.GetJournalByTypeAndCompany("debt", saleLine.CompanyId.Value);

            var soPaymentSave = new SaleOrderPaymentSave()
            {
                Amount = amountToPay.Value,
                CompanyId = saleLine.CompanyId.Value,
                Date = DateTime.Now,
                JournalLines = new List<SaleOrderPaymentJournalLineSave>()
                {
                    new SaleOrderPaymentJournalLineSave()
                    {
                        Amount = amountToPay.Value,
                        JournalId = debtJournal.Id
                    }
                },
                Lines = new List<SaleOrderPaymentHistoryLineSave>()
                {
                    new SaleOrderPaymentHistoryLineSave()
                    {
                        Amount = amountToPay.Value,
                        SaleOrderLineId = saleLine.Id
                    }
                },
                Note = saleLine.Order.Name + " - Khách hàng thanh toán",
                OrderId = saleLine.OrderId,
                State = "draft"
            };
            var soPayment = await soPaymentObj.CreateSaleOrderPayment(soPaymentSave);
            await soPaymentObj.ActionPayment(new List<Guid>() { soPayment.Id });
        }
    }
}
