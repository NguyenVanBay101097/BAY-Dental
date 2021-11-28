using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
