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

        public async Task<SaleOrderLinePartnerCommission> _GetCommissionAmount(SaleOrderLinePartnerCommission self)
        {
            var productRuleObj = GetService<ICommissionProductRuleService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var orderLine = await saleLineObj.SearchQuery(x => x.Id == self.SaleOrderLineId).Include(x => x.Product).FirstOrDefaultAsync();
            var rule = await productRuleObj.SearchQuery(x => x.CommissionId == self.CommissionId &&
                (!x.ProductId.HasValue || x.ProductId == orderLine.ProductId) &&
                (!x.CategId.HasValue || x.CategId == orderLine.Product.CategId), orderBy: x => x.OrderBy(s => s.AppliedOn)).FirstOrDefaultAsync();

            if (rule == null)
            {
            }
            else
            {
            }
            
            return self;
        }

        public override async Task<IEnumerable<SaleOrderLinePartnerCommission>> CreateAsync(IEnumerable<SaleOrderLinePartnerCommission> entities)
        {
            await ComputeAmount(entities);
            return await base.CreateAsync(entities);
        }
    }
}
