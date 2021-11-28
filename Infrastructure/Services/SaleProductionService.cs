using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleProductionService : BaseService<SaleProduction> , ISaleProductionService
    {
        private readonly IMapper _mapper;

        public SaleProductionService(IAsyncRepository<SaleProduction> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }


        public async Task ChangeQtySaleProduction(SaleProductionChangeQtyReq val)
        {

        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var saleProductions = await SearchQuery(x => ids.Contains(x.Id))
              .Include(x => x.SaleOrderLineRels)
              .Include(x => x.ProductRequestLineRels)
              .ToListAsync();

            await DeleteAsync(saleProductions);
        }


        public override ISpecification<SaleProduction> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "sale.sale_production_comp_rule":
                    return new InitialSpecification<SaleProduction>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
