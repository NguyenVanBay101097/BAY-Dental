using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class ResInsuranceService : BaseService<ResInsurance> , IResInsuranceService
    {
        private readonly IMapper _mapper;

        public ResInsuranceService(IAsyncRepository<ResInsurance> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }


        public async Task<PagedResult2<ResInsuranceBasic>> GetAgentPagedResult(ResInsurancePaged val)
        {

            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search));

            if (val.IsActive.HasValue)
                query = query.Where(x => x.IsActive == val.IsActive);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);


            var items = await query.ToListAsync();

            var paged = new PagedResult2<ResInsuranceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResInsuranceBasic>>(items) 
            };

            return paged;
        }

        public async Task<ResInsurance> GetDisplayById(Guid id)
        {
            var insurance = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();

            return insurance;
        }

        public override async Task<ResInsurance> CreateAsync(ResInsurance entity)
        {
            var partnerObj = GetService<IPartnerService>();
            var partner = new Partner()
            {
                Name = entity.Name,
                IsAgent = false,
                Phone = entity.Phone,
                Email = entity.Email,
                CompanyId = entity.CompanyId,
                Customer = false,
                Supplier = false,
                Employee = false,
                IsInsurance = true
            };

            await partnerObj.CreateAsync(partner);
            entity.PartnerId = partner.Id;

            var insurance = await base.CreateAsync(entity);
            return insurance;
        }

        public override ISpecification<ResInsurance> RuleDomainGet(IRRule rule)
        {         
            switch (rule.Code)
            {
                case "base.res_insurance_comp_rule":
                    return new InitialSpecification<ResInsurance>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
