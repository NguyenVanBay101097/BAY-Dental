using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PurchaseOrderLineService : BaseService<PurchaseOrderLine>, IPurchaseOrderLineService
    {
        private readonly IMapper _mapper;
        public PurchaseOrderLineService(IAsyncRepository<PurchaseOrderLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PurchaseOrderLineOnChangeProductResult> OnChangeProduct(PurchaseOrderLineOnChangeProduct val)
        {
            var res = new PurchaseOrderLineOnChangeProductResult();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId.Value).Include(x => x.UOM).Include(x => x.UOMPO).FirstOrDefaultAsync();
                res.PriceUnit = product.PurchasePrice ?? 0;
                res.Name = product.Name;
                res.ProductUOMId = product.UOMPOId;

                res.PriceUnit = Math.Round(res.PriceUnit * decimal.Parse((product.UOMPO.Factor != 0 ? 1 / product.UOMPO.Factor : 0).ToString()),1);
                res.ProductUOM = _mapper.Map<UoMDisplay>(product.UOMPO);
            }

            return res;
        }

        public async Task<PurchaseOrderLineOnChangeProductResult> OnChangeUoMProduct(PurchaseOrderLineOnChangeProduct val)
        {
            var res = new PurchaseOrderLineOnChangeProductResult();
            if (val.ProductId.HasValue && val.UOMId.HasValue)
            {

                var productObj = GetService<IProductService>();
                var uomOjb = GetService<IUoMService>();
                var uom = await uomOjb.SearchQuery(x => x.Id == val.UOMId.Value).FirstOrDefaultAsync();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId.Value)
                    .Include(x => x.UOM)
                    .Include(x => x.UOMPO)
                    .FirstOrDefaultAsync();
                res.PriceUnit = product.PurchasePrice ?? 0;
                res.Name = product.Name;
                res.ProductUOMId = uom.Id;
                res.PriceUnit = Math.Round(res.PriceUnit * decimal.Parse((uom.Factor != 0 ? 1 / uom.Factor : 0).ToString()),1);
                res.ProductUOM = _mapper.Map<UoMDisplay>(uom);
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
