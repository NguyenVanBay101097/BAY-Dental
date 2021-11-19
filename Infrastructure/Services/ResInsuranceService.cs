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
    public class ResInsuranceService : BaseService<ResInsurance>, IResInsuranceService
    {
        private readonly IMapper _mapper;

        public ResInsuranceService(IAsyncRepository<ResInsurance> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }


        public async Task<PagedResult2<ResInsuranceBasic>> GetPagedResult(ResInsurancePaged val)
        {
            var movelineObj = GetService<IAccountMoveLineService>(); 
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search));

            if (val.IsActive.HasValue)
                query = query.Where(x => x.IsActive == val.IsActive);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

            var amlQuery = movelineObj._QueryGet(companyId: CompanyId, state: "posted");

            amlQuery = amlQuery.Where(x => x.Account.Code == "CNBH" && x.PartnerId.HasValue);

            var amls = await amlQuery.ToListAsync();

            var debt_dict = amls.Any() ? amls.GroupBy(x => x.PartnerId.Value).ToDictionary(x => x.Key, x => x.Sum(x => x.Balance)) : null;

            var items = await query.Select(x => new ResInsuranceBasic { 
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                TotalDebt = (debt_dict != null && debt_dict.ContainsKey(x.PartnerId.Value)) ? debt_dict[x.PartnerId.Value] : 0,
                IsActive = x.IsActive              
            }).ToListAsync();

            if (val.IsDebt.HasValue)
                items = items.Where(x => val.IsDebt == true ? x.TotalDebt > 0 : x.TotalDebt == 0).ToList();

            var list = items;

            var paged = new PagedResult2<ResInsuranceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = list
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

        public async Task<IEnumerable<ResInsurance>> GetAutoComplete(ResInsurancePaged val)
        {
            var query = SearchQuery(x => x.IsActive);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search));

            var items = await query.ToListAsync();

            return items;
        }

        public override async Task<ResInsurance> CreateAsync(ResInsurance entity)
        {
            var exist = await CheckInsuranceNameExist(entity.Name);
            if (exist)
                throw new Exception("Công ty bảo hiểm đã tồn tại");

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

        private async Task<bool> CheckInsuranceNameExist(string name)
        {
            var exist = await SearchQuery(x => x.Name == name).AnyAsync();
            return exist;
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
