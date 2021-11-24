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

            var query = GetQueryableResInsurances(val);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.ToListAsync();

            var paged = new PagedResult2<ResInsuranceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }

        public IQueryable<ResInsuranceBasic> GetQueryableResInsurances(ResInsurancePaged val)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();

            IQueryable<ResInsurance> insurances = from isr in SearchQuery() select isr;

            if (!string.IsNullOrEmpty(val.Search))
                insurances = from ins in insurances.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search)) select ins;

            if (val.IsActive.HasValue)
                insurances = insurances.Union(from ins in SearchQuery().Where(x => x.IsActive == val.IsActive) select ins);


            var insuranceDebQr = from aml in movelineObj.SearchQuery(x => x.CompanyId == CompanyId)
                                 join acc in accObj.SearchQuery()
                                 on aml.AccountId equals acc.Id
                                 where acc.Code == "CNBH" && !aml.Reconciled
                                 group aml by aml.PartnerId into g
                                 select new
                                 {
                                     PartnerId = g.Key,
                                     TotalDebit = g.Sum(x => x.Balance)
                                 };

            var ResponseQr = from isr in insurances
                             from isrd in insuranceDebQr.Where(x => x.PartnerId == isr.PartnerId).DefaultIfEmpty()
                             orderby isr.DateCreated descending
                             select new ResInsuranceBasic
                             {
                                 Id = isr.Id,
                                 Name = isr.Name,
                                 Phone = isr.Phone,
                                 TotalDebt = isrd.TotalDebit,
                                 IsActive = isr.IsActive
                             };         

            if (val.IsDebt.HasValue)
                ResponseQr = ResponseQr.Where(x => val.IsDebt == true ? x.TotalDebt > 0 : x.TotalDebt <= 0);

            return ResponseQr;
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

        public override async Task UpdateAsync(ResInsurance entity)
        {
            await base.UpdateAsync(entity);

            var exist = await CheckInsuranceNameExist(entity.Name);
            if (exist)
                throw new Exception("Công ty bảo hiểm đã tồn tại");
        }

        private async Task<bool> CheckInsuranceNameExist(string name)
        {
            var count = await SearchQuery(x => x.Name == name).CountAsync();
            if (count > 1)
                return true;

            return false;
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
