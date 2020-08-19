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
                 await _GetCommissionAmount(line);
               
        }

        public async Task<SaleOrderLinePartnerCommission> _GetCommissionAmount(SaleOrderLinePartnerCommission val)
        {
            var productRuleObj = GetService<ICommissionProductRuleService>();
            var orderLine = val.SaleOrderLine;
            var commission = val.Commission;
            var rule = await productRuleObj.SearchQuery(x => x.CommissionId == commission.Id &&
                (!x.ProductId.HasValue || x.ProductId == orderLine.Product.Id) &&
                (!x.CategId.HasValue || x.CategId == orderLine.Product.CategId), orderBy: x => x.OrderBy(s => s.AppliedOn)).FirstOrDefaultAsync();

            if (rule == null)
            {
                val.Percentage = 0;
                val.Amount = 0;
            }
            else
            {
                val.Percentage = rule.PercentFixed ?? 0;
                val.Amount = orderLine.PriceTotal * ((rule.PercentFixed ?? 0) / 100);
            }
            
            return val;
        }
    }
}
