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
            ///lấy dũ liệu từ accountmoveline
            var phieuThuChiObj = GetService<IPhieuThuChiService>();
            var commSettlementObj = GetService<ICommissionSettlementService>();
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountIncome = await accountObj.GetAccountIncomeCurrentCompany();
            var accountCommAgent = await accountObj.GetAccountCommissionAgentCompany();

            var query_agent = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query_agent = query_agent.Where(x => x.Name.Contains(val.Search));

            //filter agent
            var agents = await query_agent.OrderByDescending(x => x.DateCreated).ToListAsync();

            ///lây ra doanh thu các khách hàng có người giới thiệu
            var query = movelineObj.SearchQuery(x => (x.CommissionSettlements.Any(x => x.PartnerId.HasValue && agents.Select(s => s.PartnerId).Contains(x.PartnerId.Value))) || (x.PhieuThuChiId.HasValue && x.PhieuThuChi.AgentId.HasValue && agents.Select(s=>s.Id).Contains(x.PhieuThuChi.AgentId.Value)));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);


            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);


            var movelines = await query.Include(x => x.PhieuThuChi).ThenInclude(x => x.Agent).Include(x => x.CommissionSettlements).ThenInclude(x => x.MoveLine).ToListAsync();
            var agent_dict = agents.ToDictionary(x => x.PartnerId, x => x);
            var totalItems = agents.Count();


            var sign = -1;

            ///group by chi hoa hồng cho người giới thiêu
            var commAgent_dict = movelines.Where(x => x.PhieuThuChiId.HasValue && agents.Select(x => x.Id).Contains(x.PhieuThuChi.AgentId.Value))
                .Select(x => x.PhieuThuChi).GroupBy(x => x.Agent.PartnerId).Select(x => new
                {
                    Id = x.Key,
                    Amount = x.Sum(s => s.Amount)
                }).ToDictionary(x => x.Id, x => x.Amount);

            ///group by doanh thu người giới thiêu
            var incomeAgent_dict = movelines.Where(x => x.AccountId == accountIncome.Id && x.CommissionSettlements.Any(s => s.PartnerId.HasValue && agents.Select(s=>s.PartnerId).Contains(s.PartnerId.Value)))
                .SelectMany(x => x.CommissionSettlements)
             .GroupBy(x => x.PartnerId.Value).Select(x => new
             {
                 Id = x.Key,
                 Amount = x.Sum(s => (s.MoveLine.Debit - s.MoveLine.Credit) * sign)
             }).ToDictionary(x => x.Id, x => x.Amount);


            var AgentCommissions = agents.Select(x => new CommissionAgentResult
            {
                Agent = new AgentBasic
                {
                    Id = agent_dict[x.PartnerId].Id,
                    Name = agent_dict[x.PartnerId].Name
                },
                AmountTotal = incomeAgent_dict.ContainsKey(x.PartnerId) ? incomeAgent_dict[x.PartnerId] : 0,
                AmountCommissionTotal = commAgent_dict.ContainsKey(x.PartnerId) ? commAgent_dict[x.PartnerId] : 0
            }).ToList();

            var paged = new PagedResult2<CommissionAgentResult>(totalItems, val.Offset, val.Limit)
            {
                Items = AgentCommissions
            };

            return paged;
        }

        public async Task<PagedResult2<CommissionAgentDetailResult>> GetCommissionAgentDetail(CommissionAgentDetailFilter val)
        {
            ///lấy dũ liệu từ accountmoveline
            var partnrtObj = GetService<IPartnerService>();
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountIncome = await accountObj.GetAccountIncomeCurrentCompany();
            var accountCommAgent = await accountObj.GetAccountCommissionAgentCompany();

            var agent = await SearchQuery(x => x.Id == val.AgentId).FirstOrDefaultAsync();

            ///lây ra doanh thu các khách hàng có người giới thiệu
            var query = movelineObj.SearchQuery(x => (x.AccountId == accountIncome.Id && x.CommissionSettlements.Any(x => x.PartnerId == agent.PartnerId)) || (x.PhieuThuChi.AgentId == agent.Id && x.AccountId == accountCommAgent.Id));

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Partner.Name.Contains(val.Search));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);


            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

            var sign = -1;
            var partnerIds = query.Where(x => x.CommissionSettlements.Any(s => s.PartnerId.HasValue && s.PartnerId == agent.PartnerId) && !x.PhieuThuChiId.HasValue).Select(x => x.PartnerId);
            var partner_dict = partnrtObj.SearchQuery(x => partnerIds.Contains(x.Id) && x.Customer).ToDictionary(x => x.Id, x => x.Name);
            var moveline_commAgents = await query.Where(x => x.PhieuThuChiId.HasValue && x.PhieuThuChi.AgentId.HasValue).Include(x => x.PhieuThuChi).ToListAsync();
            var commAgent_dict = moveline_commAgents.GroupBy(x => x.PhieuThuChi.PartnerId.Value).Select(x => new
            {
                Id = x.Key,
                Amount = x.Sum(s => s.PhieuThuChi.Amount)
            }).ToDictionary(x => x.Id, x => x.Amount);

            var items = await query.Where(x => partnerIds.Contains(x.PartnerId) && x.AccountId == accountIncome.Id)
                .GroupBy(x => x.PartnerId.Value).Select(x => new CommissionAgentDetailResult
                {

                    Partner = new PartnerSimple
                    {
                        Id = x.Key,
                        Name = partner_dict[x.Key]
                    },
                    AmountTotal = x.Sum(x => (x.Debit - x.Credit) * sign),
                    AmountCommissionTotal = commAgent_dict.ContainsKey(x.Key) ? commAgent_dict[x.Key] : 0
                })
                .ToListAsync();



            var totalItems = items.Count();


            var paged = new PagedResult2<CommissionAgentDetailResult>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }

        public async Task<PagedResult2<CommissionAgentDetailItemResult>> GetCommissionAgentDetailItem(CommissionAgentDetailItemFilter val)
        {
            ///lấy dũ liệu từ accountmoveline
            var movelineObj = GetService<IAccountMoveLineService>();

            var agent = await SearchQuery(x => x.Id == val.AgentId).FirstOrDefaultAsync();

            ///lây ra doanh thu các khách hàng có người giới thiệu
            var query = movelineObj.SearchQuery(x => x.CommissionSettlements.Any(x => x.PartnerId == agent.PartnerId));

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);


            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

            var totalItems = await query.CountAsync();

            var items = await query.Include(x => x.Move)
                .Select(x => new CommissionAgentDetailItemResult
                {
                    Date = x.Date.Value,
                    OrderName = x.Move.InvoiceOrigin,
                    ProductName = x.Product.Name,
                    Amount = x.PriceSubtotal ?? 0
                })
                .ToListAsync();

            var paged = new PagedResult2<CommissionAgentDetailItemResult>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }

        public async Task<decimal> GetCommissionAgentBalance(Guid id, Guid partnerId)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountIncome = await accountObj.GetAccountIncomeCurrentCompany();
            var accountCommAgent = await accountObj.GetAccountCommissionAgentCompany();

            var agent = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();

            var mountInComeTotal = await movelineObj.SearchQuery(x => x.CommissionSettlements.Any(x => x.PartnerId.HasValue && x.PartnerId == agent.PartnerId) && x.PartnerId == partnerId).SumAsync(x => (x.PriceSubtotal ?? 0));
            var commissionTotal = await movelineObj.SearchQuery(x => x.PhieuThuChiId.HasValue && x.AccountId == accountCommAgent.Id && x.PhieuThuChi.AgentId == id && x.PhieuThuChi.PartnerId == partnerId).Include(x => x.PhieuThuChi).SumAsync(x => x.PhieuThuChi.Amount);
            return mountInComeTotal - commissionTotal;
        }

        public async Task<decimal> GetCommissionAmountAgent(Guid id)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountCommAgent = await accountObj.GetAccountCommissionAgentCompany();
            var commissionTotal = await movelineObj.SearchQuery(x => x.PhieuThuChiId.HasValue && x.AccountId == accountCommAgent.Id && x.PhieuThuChi.AgentId == id).Include(x => x.PhieuThuChi).SumAsync(x => x.PhieuThuChi.Amount);
            return commissionTotal;
        }

        public async Task<decimal> GetInComeAmountAgent(Guid id)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountCommAgent = await accountObj.GetAccountCommissionAgentCompany();

            var agent = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var mountInComeTotal = await movelineObj.SearchQuery(x => x.CommissionSettlements.Any(x => x.PartnerId.HasValue && x.PartnerId == agent.PartnerId)).SumAsync(x => (x.PriceSubtotal ?? 0));
            return mountInComeTotal;
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
