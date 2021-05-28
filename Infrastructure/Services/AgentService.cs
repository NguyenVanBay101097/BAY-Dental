using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class AgentService : BaseService<Agent>, IAgentService
    {
        private readonly IMapper _mapper;
        public AgentService(IAsyncRepository<Agent> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<AgentBasic>> GetPagedResultAsync(AgentPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));


            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var totalItems = await query.CountAsync();

            var items = await query.ToListAsync();

            var paged = new PagedResult2<AgentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<AgentBasic>>(items)
            };

            return paged;
        }

        public async Task<AgentDisplay> GetDisplayById(Guid id)
        {
            var agent = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();

            var display = _mapper.Map<AgentDisplay>(agent);

            return display;
        }

        public async Task<PagedResult2<CommissionAgentResult>> GetCommissionAgent(CommissionAgentFilter val)
        {
            ///lấy dũ liệu từ CommissionSettlement
            var phieuThuChiObj = GetService<IPhieuThuChiService>();
            var commSettlementObj = GetService<ICommissionSettlementService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountIncome = await accountObj.GetAccountIncomeCurrentCompany();
            var accountCommAgent = await accountObj.GetAccountCommissionAgentCompany();
            ///lây ra doanh thu các khách hàng có người giới thiệu
            var query = commSettlementObj.SearchQuery(x => x.PartnerId.HasValue && x.MoveLine.AccountId == accountIncome.Id);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Partner.Name.Contains(val.Search));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);


            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            var agent_dict = SearchQuery().ToDictionary(x => x.PartnerId, x => x);
            var commAgent_dict = phieuThuChiObj.SearchQuery(x => x.AccountType == "commission" && x.AccountId.HasValue && x.AccountId == accountCommAgent.Id && x.PartnerId.HasValue).GroupBy(x => x.PartnerId.Value).Select(x => new
            {
                Id = x.Key,
                Amount = x.Sum(s => s.Amount)
            }).ToDictionary(x=>x.Id, x => x.Amount);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var totalItems = await query.CountAsync();

            var items = await query.Include(x => x.MoveLine).ToListAsync();
            var sign = -1;
            var agents = items.GroupBy(x => x.PartnerId.Value).Select(x => new CommissionAgentResult
            {

                Agent = new AgentBasic
                {
                    Id = agent_dict[x.Key].Id,
                    Name = agent_dict[x.Key].Name
                },
                AmountTotal = x.Sum(x => (x.MoveLine.Debit - x.MoveLine.Credit) * sign),
                AmountCommissionTotal = commAgent_dict.ContainsKey(x.Key) ? commAgent_dict[x.Key] : 0
            }).ToList();

            var paged = new PagedResult2<CommissionAgentResult>(totalItems, val.Offset, val.Limit)
            {
                Items = agents
            };

            return paged;
        }

        public override async Task<Agent> CreateAsync(Agent entity)
        {
            var partnerObj = GetService<IPartnerService>();
            var partner = new Partner()
            {
                Name = entity.Name,
                IsAgent = true,
                Phone = entity.Phone,
                Email = entity.Email,
                CompanyId = entity.CompanyId,
                Customer = false,
                Supplier = false,
                Employee = false,
            };

            await partnerObj.CreateAsync(partner);
            entity.PartnerId = partner.Id;

            var agent = await base.CreateAsync(entity);
            return agent;
        }

        public override ISpecification<Agent> RuleDomainGet(IRRule rule)
        {
            //ra đc list company id ma nguoi dung dc phép
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "base.agent_comp_rule":
                    return new InitialSpecification<Agent>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
