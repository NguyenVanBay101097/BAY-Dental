using ApplicationCore.Entities;
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

            _AmountAll(order);

            return await CreateAsync(order);
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

            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SaleOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                State = x.State,
                Residual = x.Residual,
                UserName = x.User != null ? x.User.Name : string.Empty
            }).ToListAsync();

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
            var self = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.OrderLines).Include("OrderLines.Product").FirstOrDefaultAsync();

            var couponObj = GetService<ISaleCouponService>();
            var soLineObj = GetService<ISaleOrderLineService>();
            var ruleObj = GetService<ISaleCouponProgramService>();

            var coupon = await couponObj.SearchQuery(x => x.Code == couponCode)
                .Include(x => x.Program).Include(x => x.Program.DiscountLineProduct).Include(x => x.Partner).FirstOrDefaultAsync();
            if (coupon == null)
                throw new Exception("Mã coupon " + couponCode + " không tồn tại");

            if (!coupon.Program.Active)
                throw new Exception("Chương trình coupon cho " + couponCode + " đã hết khả dụng.");

            var date = DateTime.Today;
            if (coupon.DateExpired.HasValue && date > coupon.DateExpired)
                throw new Exception("Mã coupon " + couponCode + " đã hết hạn sử dụng");

            if (coupon.State == "used" || coupon.State == "expired")
                throw new Exception("Mã coupon " + couponCode + " đã được sử dụng hoặc hết hạn sử dụng.");

            if (coupon.PartnerId.HasValue && coupon.PartnerId != self.PartnerId)
                throw new Exception("Mã coupon " + couponCode + " chỉ có thể sử dụng cho khách hàng " + coupon.Partner.Name);

            var rule = coupon.Program;
            decimal total_amount = 0;
            decimal total_qty = 0;
            if (rule.ProgramType == "coupon_program")
            {
                //Kiểm tra điều kiện để áp dụng coupon nếu nó thuộc một chương trình coupon
                var product_list = new List<ApplyPromotionProductListItem>();
                foreach (var line in self.OrderLines)
                {
                    if (line.PromotionProgramId.HasValue)
                        continue;
                    var qty = line.ProductUOMQty;
                    var price_unit = line.PriceUnit * (1 - line.Discount / 100);
                    var amount = price_unit * line.ProductUOMQty; //chưa bao gồm thuế
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

                var min_quantity = rule.RuleMinQuantity ?? 0;
                var min_amount = rule.RuleMinimumAmount ?? 0;

                foreach (var item in product_list)
                {
                    total_qty += item.Qty;
                    total_amount += item.Amount;
                }

                if (total_qty < min_quantity)
                {
                    throw new Exception("Bạn cần mua số lượng tối thiểu là " + min_quantity + ". Chương trình coupon: " + rule.Name);
                }

                if (total_amount < min_amount)
                    throw new Exception("Bạn cần mua số tiền tối thiểu là " + min_amount.ToString("n0") + ". Chương trình coupon: " + rule.Name);
            }

            await ruleObj.Apply(rule, self, total_amount, total_qty, coupon: coupon);

            coupon.SaleOrderId = self.Id;
            coupon.State = "used";
            await couponObj.UpdateAsync(coupon);

            _AmountAll(self);
            _GetInvoiced(new List<SaleOrder>() { self });
            await UpdateAsync(self);
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
