using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleOrderLineService : BaseService<SaleOrderLine>, ISaleOrderLineService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _dbContext;

        public SaleOrderLineService(IAsyncRepository<SaleOrderLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            CatalogDbContext dbContext)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
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
                    if (payment != null && payment.State != "draft")
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
                line.State = order.State;
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

        public void _GetInvoiceAmount(IEnumerable<SaleOrderLine> self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            foreach (var line in self)
            {
                decimal amountInvoiced = 0;
                var amls = amlObj.SearchQuery(x => x.SaleLineRels.Any(x => x.OrderLineId == line.Id)).Include(x => x.Move).ToList();
                foreach (var invoiceLine in amls)
                {
                    var move = invoiceLine.Move;
                    if (move.State != "cancel")
                    {
                        if (move.Type == "out_invoice")
                        {
                            amountInvoiced += (invoiceLine.PriceSubtotal ?? 0);
                        }
                        else if (move.Type == "out_refund")
                        {
                            amountInvoiced -= (invoiceLine.PriceSubtotal ?? 0);
                        }
                    }
                }

                line.AmountInvoiced = amountInvoiced;
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

        public async Task<PagedResult2<SaleOrderLineBasic>> GetPagedResultAsync(SaleOrderLinesPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.OrderPartner.Name.Contains(val.Search));
            if (val.OrderPartnerId.HasValue)
                query = query.Where(x => x.OrderPartnerId == val.OrderPartnerId);
            if (val.ProductId.HasValue)
                query = query.Where(x => x.ProductId == val.ProductId);
            if (val.OrderId.HasValue)
                query = query.Where(x => x.OrderId == val.OrderId);
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Contains(val.State));
            if (val.DateOrderFrom.HasValue)
                query = query.Where(x => x.DateCreated <= val.DateOrderFrom);
            if (val.DateOrderTo.HasValue)
                query = query.Where(x => x.DateCreated >= val.DateOrderTo);
            if (val.IsLabo == true)
                query = query.Where(x => x.Product.IsLabo);

            if (val.IsQuotation.HasValue)
            {
                query = query.Where(x => x.Order.IsQuotation == val.IsQuotation);
            }

            if (!string.IsNullOrEmpty(val.LaboState))
            {
                query = query.Where(x => x.Labos.Any(s => s.State == val.LaboState));
            }


            query = query.Include(x => x.OrderPartner).Include(x => x.Product).Include(x => x.Order).Include(x => x.Labos).Include(x => x.Employee).OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
            {
                query = query.Take(val.Limit).Skip(val.Offset);
            }
            var items = await _mapper.ProjectTo<SaleOrderLineBasic>(query).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleOrderLineBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override ISpecification<SaleOrderLine> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.sale_order_line_comp_rule":
                    return new InitialSpecification<SaleOrderLine>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var couponObj = GetService<ISaleCouponService>();
            var saleObj = GetService<ISaleOrderService>();
            var programObj = GetService<ISaleCouponProgramService>();
            var saleCardRelObj = GetService<ISaleOrderServiceCardCardRelService>();
            var serviceCardObj = GetService<IServiceCardCardService>();

            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Coupon).Include(x => x.SaleOrderLinePaymentRels)
                .Include(x => x.Order).Include(x => x.Promotions).ThenInclude(x => x.Lines)
                .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
                .Include(x => x.PaymentHistoryLines)
                .ToListAsync();


            if (self.Any(x => x.State != "draft" && x.State != "cancel" && x.State != "sale"))
                throw new Exception("Chỉ có thể xóa chi tiết ở trạng thái nháp hoặc hủy bỏ");

            if (self.Any(x => x.PaymentHistoryLines.Any()))
                throw new Exception("không thể xóa dịch vụ đã thanh toán");

            foreach (var line in self.Where(x => x.IsRewardLine))
            {
                var appliedCoupons = await couponObj.SearchQuery(x => x.SaleOrderId == line.OrderId)
                    .Include(x => x.Program).ToListAsync();

                var coupons_to_reactivate = appliedCoupons.Where(x => x.Program.DiscountLineProductId == line.ProductId).ToList();
                foreach (var coupon in coupons_to_reactivate)
                {
                    coupon.State = "new";
                    coupon.SaleOrderId = null;
                }

                await couponObj.UpdateAsync(coupons_to_reactivate);

                var related_program = await programObj.SearchQuery(x => x.DiscountLineProductId == line.ProductId).ToListAsync();
                if (related_program.Any())
                {
                    foreach (var program in related_program)
                    {
                        var promo_programs = await _dbContext.SaleOrderNoCodePromoPrograms.Where(x => x.ProgramId == program.Id).ToListAsync();
                        _dbContext.SaleOrderNoCodePromoPrograms.RemoveRange(promo_programs);
                        await _dbContext.SaveChangesAsync();

                        if (program.Id == line.Order.CodePromoProgramId)
                            line.Order.CodePromoProgramId = null;
                    }
                }

                await saleObj.UpdateAsync(line.Order);

                var card_rels_to_unlink = await saleCardRelObj.SearchQuery(x => x.SaleOrderId == line.OrderId && x.Card.CardType.ProductId == line.ProductId).ToListAsync();
                var card_ids = card_rels_to_unlink.Select(x => x.CardId).Distinct().ToList();
                await saleCardRelObj.DeleteAsync(card_rels_to_unlink);

                await serviceCardObj._ComputeResidual(card_ids);

                var promotionObj = GetService<ISaleOrderPromotionService>();
                var promotionIds = await promotionObj.SearchQuery(x => ids.Contains(x.SaleOrderLineId.Value) && x.SaleOrderId.HasValue).Select(x => x.Id).ToListAsync();
                if (promotionIds.Any())
                    await promotionObj.RemovePromotion(promotionIds);

            }

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

        public async Task UpdateDkByOrderLine(Guid key, SaleOrderLineDotKhamSave val)
        {
            var dkStepobj = GetService<IDotKhamStepService>();
            var productObj = GetService<IProductService>();
            var productStepObj = GetService<IProductStepService>();
            var line = await SearchQuery(x => x.Id == key).Include(x => x.DotKhamSteps).FirstOrDefaultAsync();
            var dksAdd = new List<DotKhamStep>();
            var dksRemove = new List<DotKhamStep>();

            foreach (var item in val.Steps.ToList())
            {
                if (!item.Id.HasValue)
                {
                    dksAdd.Add(new DotKhamStep
                    {
                        Id = GuidComb.GenerateComb(),
                        IsDone = item.IsDone,
                        Name = item.Name,
                        Order = item.Order,
                        ProductId = line.ProductId,
                        SaleLineId = line.Id,
                        SaleOrderId = line.OrderId
                    });

                    val.Steps.Remove(item);
                }
            }

            var valDict = val.Steps.ToDictionary(x => x.Id, x => x);

            foreach (var item in line.DotKhamSteps.ToList())
            {
                if (!val.Steps.Any(x => x.Id == item.Id))
                {
                    dksRemove.Add(item);
                    line.DotKhamSteps.Remove(item);
                }

                if (valDict.ContainsKey(item.Id))
                {
                    item.Name = valDict[item.Id].Name;
                    item.Order = valDict[item.Id].Order;
                }
            }


            await dkStepobj.CreateAsync(dksAdd);
            await dkStepobj.DeleteAsync(dksRemove);
            await dkStepobj.UpdateAsync(line.DotKhamSteps.ToList());
            if (val.Default)
            {
                var service = await productObj.SearchQuery(x => x.Id == line.ProductId).Include(x => x.Steps).FirstOrDefaultAsync();
                var stepsAdd = new List<ProductStep>();
                foreach (var item in val.Steps)
                {
                    stepsAdd.Add(new ProductStep
                    {
                        Id = item.Id.HasValue ? item.Id.Value : GuidComb.GenerateComb(),
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

        public async Task ComputeProductRequestedQuantity(IEnumerable<Guid> ids)
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
            await requestedObj.DeleteAsync(requesteds);

            var toCreateRequested = new List<SaleOrderLineProductRequested>();
            foreach (var item in requestLines)
            {
                var requested = new SaleOrderLineProductRequested
                {
                    SaleOrderLineId = item.SaleOrderLineId,
                    ProductId = item.ProductId,
                    RequestedQuantity = item.Total
                };
                toCreateRequested.Add(requested);
            }

            //save changes
            await requestedObj.CreateAsync(toCreateRequested);
        }



        public async Task ApplyDiscountOnOrderLine(ApplyDiscountViewModel val)
        {
            var orderPromotionObj = GetService<ISaleOrderPromotionService>();
            var orderObj = GetService<ISaleOrderService>();

            var orderLine = await SearchQuery(x => x.Id == val.Id).Include(x => x.Order)
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

        public async Task<SaleCouponProgramResponse> ApplyPromotionOnOrderLine(ApplyPromotionRequest val)
        {
            var programObj = GetService<ISaleCouponProgramService>();
            var couponObj = GetService<ISaleCouponService>();
            var orderLine = await SearchQuery(x => x.Id == val.Id)
              .Include(x => x.Product)
              .Include(x => x.Promotions).ThenInclude(x => x.SaleCouponProgram)
              .Include(x => x.Order).ThenInclude(x => x.Promotions)
              .Include(x => x.Order).ThenInclude(x => x.OrderLines)
              .Include(x => x.PromotionLines)
              .Include(x => x.SaleOrderLineInvoice2Rels)
              .FirstOrDefaultAsync();
            var program = await programObj.SearchQuery(x => x.Id == val.SaleProgramId).Include(x => x.DiscountSpecificProducts).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            if (program != null)
            {
                var error_status = await programObj._CheckPromotionApplySaleLine(program, orderLine);
                if (string.IsNullOrEmpty(error_status.Error))
                {
                    await _CreateRewardLine(orderLine, program);
                    return new SaleCouponProgramResponse { Error = null, Success = true, SaleCouponProgram = _mapper.Map<SaleCouponProgramDisplay>(program) };
                }
                else
                    return new SaleCouponProgramResponse { Error = error_status.Error, Success = false, SaleCouponProgram = null };
            }
            else
            {
                return new SaleCouponProgramResponse { Error = "Mã chương trình khuyến mãi không tồn tại", Success = false, SaleCouponProgram = null };

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
                .Include(x => x.SaleOrderLineInvoice2Rels)
                .FirstOrDefaultAsync();

            //Chương trình khuyến mãi sử dụng mã
            var program = await programObj.SearchQuery(x => x.PromoCode == couponCode).Include(x => x.DiscountSpecificProducts).FirstOrDefaultAsync();
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
                return new SaleCouponProgramResponse { Error = "Mã chương trình khuyến mãi không tồn tại", Success = false, SaleCouponProgram = null };

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

        public decimal _GetRewardValuesDiscountPercentagePerOrderLine(SaleCouponProgram program, SaleOrderLine line)
        {
            //discount_amount = so luong * don gia da giam * phan tram
            var price_reduce = (line.PriceUnit * (1 - line.Discount / 100)) *
                (1 - (program.DiscountPercentage ?? 0) / 100);
            var discount_amount = (line.PriceUnit - price_reduce) * line.ProductUOMQty;
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

            if (program.DiscountLineProduct == null)
            {
                var productObj = GetService<IProductService>();
                program.DiscountLineProduct = productObj.GetById(program.DiscountLineProductId);
            }

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

            return new SaleOrderPromotion();
        }


        public void RecomputePromotionLine(IEnumerable<SaleOrderLine> self)
        {
            //vong lap
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

            //lay ra nhung uu dai dang ap dung

            //tinh lai so tien promotion


        }
    }
}
