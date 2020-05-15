using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ServiceCardOrderLineService : BaseService<ServiceCardOrderLine>, IServiceCardOrderLineService
    {
        public ServiceCardOrderLineService(IAsyncRepository<ServiceCardOrderLine> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public void PrepareLines(IEnumerable<ServiceCardOrderLine> self)
        {
            _UpdateProps(self);
            _ComputeAmount(self);
        }

        public AccountMoveLine PrepareInvoiceLine(ServiceCardOrderLine self)
        {
            var res = new AccountMoveLine
            {
                Name = self.CardType.Name,
                ProductId = self.CardType.ProductId,
                ProductUoMId = self.CardType.Product.UOMId,
                Quantity = self.ProductUOMQty,
                Discount = self.Discount,
                PriceUnit = self.PriceUnit,
                DiscountType = self.DiscountType,
                DiscountFixed = self.DiscountFixed
            };

            res.CardOrderLineRels.Add(new ServiceCardOrderLineInvoiceRel { OrderLine = self });

            return res;
        }

        public void _ComputeAmount(IEnumerable<ServiceCardOrderLine> self)
        {
            foreach (var line in self)
            {
                var discountType = line.DiscountType ?? "percentage";
                var price = discountType == "percentage" ? line.PriceUnit * (1 - line.Discount / 100) :
                    Math.Max(0, line.PriceUnit - (line.DiscountFixed ?? 0));
                line.PriceSubTotal = price * line.ProductUOMQty;
                line.PriceTotal = line.PriceSubTotal;
            }
        }

        public void _UpdateProps(IEnumerable<ServiceCardOrderLine> self)
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

        public ServiceCardCard PrepareCard(ServiceCardOrderLine self)
        {
            return new ServiceCardCard
            {
                CardTypeId = self.CardTypeId,
                CardType = self.CardType,
                Amount = self.CardType.Amount,
                Residual = self.CardType.Amount,
                SaleLineId = self.Id
            };
        }

        public async Task<ServiceCardOrderLineOnChangeCardTypeResponse> OnChangeCardType(ServiceCardOrderLineOnChangeCardType val)
        {
            decimal price_unit = 0M;
            if (val.CardTypeId.HasValue)
            {
                var cardTypeObj = GetService<IServiceCardTypeService>();
                var cardType = await cardTypeObj.GetByIdAsync(val.CardTypeId.Value);
                price_unit = cardType.Price ?? 0;
            }

            var res = new ServiceCardOrderLineOnChangeCardTypeResponse()
            {
                PriceUnit = price_unit
            };

            return res;
        }
    }
}
