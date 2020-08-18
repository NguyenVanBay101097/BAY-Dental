using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SaleOrderLinePartnerCommissionService : BaseService<SaleOrderLinePartnerCommission>, ISaleOrderLinePartnerCommissionService
    {
        private readonly IMapper _mapper;
        public SaleOrderLinePartnerCommissionService(IAsyncRepository<SaleOrderLinePartnerCommission> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task ComputeAmount(IEnumerable<SaleOrderLinePartnerCommission> self) 
        {
            foreach(var line in self)
            {
                var orderLine = line.SaleOrderLine;
                line.Amount = await _GetCommissionAmount(line.Commission, orderLine.PriceTotal, orderLine.Product);
                line.Percentage = await _GetCommissionPercentage(line.Commission, orderLine.Product);
            }
        }

        public async Task<decimal> _GetCommissionAmount(Commission commission, decimal? subtotal, Product product)
        {
            var productRuleObj = GetService<ICommissionProductRuleService>();
            var rule = await productRuleObj.SearchQuery(x => x.CommissionId == commission.Id &&
                (!x.ProductId.HasValue || x.ProductId == product.Id) &&
                (!x.CategId.HasValue || x.CategId == product.CategId), orderBy: x => x.OrderBy(s => s.AppliedOn)).FirstOrDefaultAsync();

            if (rule == null)
                return 0M;

            return (subtotal ?? 0) * ((rule.PercentFixed ?? 0) / 100);
        }

        public async Task<decimal> _GetCommissionPercentage(Commission commission, Product product)
        {
            var productRuleObj = GetService<ICommissionProductRuleService>();
            var rule = await productRuleObj.SearchQuery(x => x.CommissionId == commission.Id &&
                (!x.ProductId.HasValue || x.ProductId == product.Id) &&
                (!x.CategId.HasValue || x.CategId == product.CategId), orderBy: x => x.OrderBy(s => s.AppliedOn)).FirstOrDefaultAsync();

            if (rule == null)
                return 0M;

            return rule.PercentFixed ?? 0;
        }
    }
}
