﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public void ComputeAmount(ICollection<SaleOrderLine> self)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {
                var price = line.PriceUnit * (1 - line.Discount / 100);
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
                var computePrice = await productObj._ComputeProductPrice(new List<Product>() { product }, val.PartnerId);
                res.PriceUnit = computePrice[product.Id];

            }

            return res;
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
                    line.QtyToInvoice = 0;
            }
        }

        public void _GetInvoiceQty(IEnumerable<SaleOrderLine> lines)
        {
            foreach (var line in lines)
            {
                decimal qtyInvoiced = 0;
                foreach (var rel in line.SaleOrderLineInvoiceRels)
                {
                    var invoice = rel.InvoiceLine.Invoice;
                    var invoiceLine = rel.InvoiceLine;
                    if (invoice.State != "cancel")
                    {
                        if (invoice.Type == "out_invoice")
                            qtyInvoiced += invoiceLine.Quantity;
                        else if (invoice.Type == "out_refund")
                            qtyInvoiced -= invoiceLine.Quantity;
                    }
                }

                line.QtyInvoiced = qtyInvoiced;
            }
        }

        public void _ComputeInvoiceStatus(IEnumerable<SaleOrderLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.State != "sale" && line.State != "done")
                    line.InvoiceStatus = "no";
                else if ((line.QtyToInvoice ?? 0) != 0)
                    line.InvoiceStatus = "to invoice";
                else if ((line.QtyInvoiced ?? 0) >= line.ProductUOMQty)
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
    }
}
