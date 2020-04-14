﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PurchaseOrderLineService : BaseService<PurchaseOrderLine>, IPurchaseOrderLineService
    {
        public PurchaseOrderLineService(IAsyncRepository<PurchaseOrderLine> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<PurchaseOrderLineOnChangeProductResult> OnChangeProduct(PurchaseOrderLineOnChangeProduct val)
        {
            var res = new PurchaseOrderLineOnChangeProductResult();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.GetByIdAsync(val.ProductId.Value);
                res.PriceUnit = product.PurchasePrice ?? 0;
                res.Name = product.Name;
                res.ProductUOMId = product.UOMId;
            }

            return res;
        }

        public void _ComputeAmount(IEnumerable<PurchaseOrderLine> self)
        {
            foreach (var line in self)
            {
                line.PriceSubtotal = line.PriceUnit * (1 - (line.Discount ?? 0) / 100) * line.ProductQty;
                line.PriceTax = 0;
                line.PriceTotal = line.PriceSubtotal;
            }
        }

        public void _ComputeQtyInvoiced(IEnumerable<PurchaseOrderLine> self)
        {
            foreach (var line in self)
            {
                decimal? qty = 0;
                foreach (var inv_line in line.MoveLines)
                {
                    if (inv_line.Move.State != "cancel")
                    {
                        if (inv_line.Move.Type == "in_invoice")
                            qty += inv_line.Quantity;
                        else if (inv_line.Move.Type == "in_refund")
                            qty -= inv_line.Quantity;
                    }
                }

                line.QtyInvoiced = qty;
            }
        }

        public AccountMoveLine _PrepareAccountMoveLine(PurchaseOrderLine self, AccountMove move)
        {
            var qty = self.ProductQty - (self.QtyInvoiced ?? 0);
            if (qty < 0)
                qty = 0;

            return new AccountMoveLine
            {
                Name = $"{self.Order.Name}: {self.Name}",
                Move = move,
                PurchaseLineId = self.Id,
                ProductUoMId = self.ProductUOMId,
                ProductId = self.ProductId,
                PriceUnit = self.PriceUnit,
                Quantity = qty,
                PartnerId = move.PartnerId,
            };
        }
    }
}
