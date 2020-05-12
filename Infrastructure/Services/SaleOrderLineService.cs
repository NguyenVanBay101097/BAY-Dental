using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
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
        public SaleOrderLineService(IAsyncRepository<SaleOrderLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override async Task<IEnumerable<SaleOrderLine>> CreateAsync(IEnumerable<SaleOrderLine> entities)
        {
            UpdateProps(entities);
            ComputeAmount(entities);
            _GetInvoiceQty(entities);
            _GetToInvoiceQty(entities);
            _ComputeInvoiceStatus(entities);
            return await base.CreateAsync(entities);
        }

        public void ComputeAmount(IEnumerable<SaleOrderLine> self)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {
                var discountType = line.DiscountType ?? "percentage";
                var price = discountType == "percentage" ? line.PriceUnit * (1 - line.Discount / 100) :
                    Math.Max(0, line.PriceUnit - (line.DiscountFixed ?? 0));
                line.PriceTax = 0;
                line.PriceSubTotal = price * line.ProductUOMQty;
                line.PriceTotal = line.PriceSubTotal + line.PriceTax;
            }
        }

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
                line.SalesmanId = order.UserId;
                line.OrderPartnerId = order.PartnerId;
                line.CompanyId = order.CompanyId;
                line.Order = order;
                line.State = order.State;
            }
        }

        public void UpdateOrderInfo(ICollection<SaleOrderLine> self, SaleOrder order)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {
                line.SalesmanId = order.UserId;
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

        public void _GetInvoiceQty(IEnumerable<SaleOrderLine> lines)
        {
            foreach (var line in lines)
            {
                decimal qtyInvoiced = 0;
                foreach (var rel in line.SaleOrderLineInvoice2Rels)
                {
                    var move = rel.InvoiceLine.Move;
                    var invoiceLine = rel.InvoiceLine;
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
                else if (line.QtyToInvoice != 0)
                    line.InvoiceStatus = "to invoice";
                else if (line.QtyToInvoice == 0)
                    line.InvoiceStatus = "invoiced";
                else
                    line.InvoiceStatus = "no";
            }
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
                Quantity = self.QtyToInvoice,
                Discount = self.Discount,
                PriceUnit = self.PriceUnit,
                DiscountType = self.DiscountType,
                DiscountFixed = self.DiscountFixed
            };

            res.SaleLineRels.Add(new SaleOrderLineInvoice2Rel { OrderLine = self });

            return res;
        }

        public async Task<PagedResult2<SaleOrderLine>> GetPagedResultAsync(SaleOrderLinesPaged val)
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

            var items = await query.Include(x => x.OrderPartner).Include(x => x.Product).Include(x => x.Order).OrderByDescending(x => x.DateCreated).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleOrderLine>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override ISpecification<SaleOrderLine> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.order.line_comp_rule":
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

            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Coupon)
                .Include(x => x.Order).Include("Order.AppliedCoupons").Include("Order.AppliedCoupons.Program")
                .Include("Order.NoCodePromoPrograms")
                .Include("Order.SaleOrderCardRels").Include("Order.SaleOrderCardRels.Card")
                .Include("Order.SaleOrderCardRels.Card.CardType").ToListAsync();

            if (self.Any(x => x.State != "draft" && x.State != "cancel"))
                throw new Exception("Chỉ có thể xóa chi tiết ở trạng thái nháp hoặc hủy bỏ");

            foreach(var line in self.Where(x => x.IsRewardLine))
            {
                var coupons_to_reactivate = line.Order.AppliedCoupons.Where(x => x.Program.DiscountLineProductId == line.ProductId).ToList();
                foreach (var coupon in coupons_to_reactivate)
                {
                    coupon.State = "new";
                    line.Order.AppliedCoupons.Remove(coupon);
                }

                await couponObj.UpdateAsync(coupons_to_reactivate);

                var related_program = await programObj.SearchQuery(x => x.DiscountLineProductId == line.ProductId).ToListAsync();
                if (related_program.Any())
                {
                    foreach(var program in related_program)
                    {
                        if (line.Order.NoCodePromoPrograms.Any(x => x.ProgramId == program.Id))
                            line.Order.NoCodePromoPrograms.Remove(line.Order.NoCodePromoPrograms.FirstOrDefault(x => x.ProgramId == program.Id));
                        if (program.Id == line.Order.CodePromoProgramId)
                            line.Order.CodePromoProgramId = null;
                    }
                }
                await saleObj.UpdateAsync(line.Order);

                var card_rels_to_unlink = line.Order.SaleOrderCardRels.Where(x => x.Card.CardType.ProductId == line.ProductId).ToList();
                var card_ids = line.Order.SaleOrderCardRels.Select(x => x.CardId).Distinct().ToList();
                await saleCardRelObj.DeleteAsync(card_rels_to_unlink);

                await serviceCardObj._ComputeResidual(card_ids);
            }

            await DeleteAsync(self);
        }
    }
}
