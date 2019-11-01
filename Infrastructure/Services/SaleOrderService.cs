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

namespace Infrastructure.Services
{
    public class SaleOrderService: BaseService<SaleOrder>, ISaleOrderService
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
                var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                order.Name = await sequenceService.NextByCode("sale.order");
            }

            var saleLineService = (ISaleOrderLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ISaleOrderLineService));
            saleLineService.UpdateOrderInfo(order.OrderLines, order);
            saleLineService.ComputeAmount(order.OrderLines);

            _AmountAll(order);

            return await CreateAsync(order);
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

        public async Task<SaleOrder> GetSaleOrderForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.OrderLines)
                .Include("OrderLines.Product")
                .FirstOrDefaultAsync();
        }

        public async Task<SaleOrder> GetSaleOrderWithLines(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.OrderLines)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOrderAsync(SaleOrder order)
        {
            await UpdateAsync(order);
        }

        public async Task<SaleOrder> GetSaleOrderByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
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

        public async Task<SaleOrderDisplay> DefaultGet()
        {
            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            var res = new SaleOrderDisplay();
            res.CompanyId = CompanyId;
            res.UserId = UserId;
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            return res;
        }

        public override ISpecification<SaleOrder> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.order_comp_rule":
                    return new InitialSpecification<SaleOrder>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
