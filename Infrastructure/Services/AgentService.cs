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

            //lấy danh sách agent phân trang
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));


            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var totalItems = await query.CountAsync();
            var items = await query.Include(x => x.Partners).ToListAsync();


            //tính tổng doanh thu và tổng chi hoa hồng trang 1 page
            var agentIds = items.Select(x => x.Id).ToList();


            ///group by chi hoa hồng cho người giới thiêu
            var commAgent_dict = await ComputeCommissionAgents(agentIds, val.DateFrom, val.DateTo, val.CompanyId);

            ///group by doanh thu người giới thiêu
            var incomeAgent_dict = await ComputeIncomeAgents(agentIds, val.DateFrom, val.DateTo, val.CompanyId);


            var AgentCommissions = items.Select(x => new CommissionAgentResult
            {
                Agent = new AgentBasic
                {
                    Id = x.Id,
                    Name = x.Name
                },
                AmountTotal = incomeAgent_dict.ContainsKey(x.Id) ? incomeAgent_dict[x.Id] : 0,
                AmountCommissionTotal = commAgent_dict.ContainsKey(x.Id) ? commAgent_dict[x.Id] : 0
            }).ToList();

            var paged = new PagedResult2<CommissionAgentResult>(totalItems, val.Offset, val.Limit)
            {
                Items = AgentCommissions
            };

            return paged;
        }

        public async Task<IDictionary<Guid, decimal>> ComputeIncomeAgents(IEnumerable<Guid> ids, DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var query = movelineObj._QueryGet(companyId: companyId, state: "posted", dateFrom: dateFrom,
               dateTo: dateTo);

            query = query.Where(x => x.Account.Code == "5111");

            if (ids.Any())
                query = query.Where(x => x.Partner.AgentId.HasValue && ids.Contains(x.Partner.AgentId.Value));

            var items = await query.Include(x => x.Partner).ToListAsync();

            var incomAgent_dict = items.GroupBy(x => x.Partner.AgentId.Value).Select(x => new
            {
                Id = x.Key,
                Amount = x.Sum(s => (s.PriceSubtotal ?? 0)),
            }).ToDictionary(x => x.Id, x => x.Amount);

            return incomAgent_dict;
        }

        public async Task<Dictionary<Guid, decimal>> ComputeCommissionAgents(IEnumerable<Guid> ids, DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var phieuthuchiObj = GetService<IPhieuThuChiService>();
            var query = phieuthuchiObj.SearchQuery();

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId);

            if (dateFrom.HasValue)
                query = query.Where(x => x.Date >= dateFrom.Value);


            if (dateTo.HasValue)
            {
                var datetimeTo = dateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= datetimeTo);
            }

            if (ids.Any())
                query = query.Where(x => x.AgentId.HasValue && ids.Contains(x.AgentId.Value));

            query = query.Where(x => x.Account.Code == "HHNGT");

            var items = await query.ToListAsync();



            var comissionAgent_dict = items.GroupBy(x => x.AgentId.Value).Select(x => new
            {
                Id = x.Key,
                Amount = x.Sum(s => s.Amount),
            }).ToDictionary(x => x.Id, x => x.Amount);

            return comissionAgent_dict;
        }

        public async Task<Dictionary<Guid, decimal>> ComputeCommissionPartners(IEnumerable<Guid> partnerids, DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var phieuthuchiObj = GetService<IPhieuThuChiService>();
            var query = phieuthuchiObj.SearchQuery();

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId);

            if (dateFrom.HasValue)
                query = query.Where(x => x.Date >= dateFrom.Value);


            if (dateTo.HasValue)
            {
                var datetimeTo = dateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= datetimeTo);
            }

            if (partnerids.Any())
                query = query.Where(x => x.AgentId.HasValue && partnerids.Contains(x.PartnerId.Value));

            query = query.Where(x => x.Account.Code == "HHNGT");

            var items = await query.ToListAsync();

            var comissionAgent_dict = items.GroupBy(x => x.PartnerId.Value).Select(x => new
            {
                Id = x.Key,
                Amount = x.Sum(s => s.Amount),
            }).ToDictionary(x => x.Id, x => x.Amount);

            return comissionAgent_dict;
        }

        public async Task<PagedResult2<CommissionAgentDetailResult>> GetCommissionAgentDetail(CommissionAgentDetailFilter val)
        {
            ///lấy dũ liệu từ accountmoveline
            var partnerObj = GetService<IPartnerService>();
            var movelineObj = GetService<IAccountMoveLineService>();
            var agent = await SearchQuery(x => x.Id == val.AgentId).Include(x => x.Partners).FirstOrDefaultAsync();

            var partnerIds = agent.Partners.Select(x => x.Id).ToList();



            ///lây ra doanh thu các khách hàng có người giới thiệu
            var query = movelineObj._QueryGet(companyId: val.CompanyId, state: "posted", dateFrom: val.DateFrom,
              dateTo: val.DateTo);

            query = query.Where(x => x.Account.Code == "5111" && x.Partner.AgentId.HasValue && x.Partner.AgentId == agent.Id);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Partner.Name.Contains(val.Search));

            if (partnerIds.Any())
                query = query.Where(x => partnerIds.Contains(x.PartnerId.Value));

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

            var totalItems = await query.CountAsync();

            var items = await query.ToListAsync();
            var partner_dict = await partnerObj.SearchQuery(x => partnerIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.Name);
            var sign = -1;
            var commAgent_dict = await ComputeCommissionPartners(partnerIds, val.DateFrom, val.DateTo, val.CompanyId);

            var res = items.GroupBy(x => x.PartnerId.Value).Select(x => new CommissionAgentDetailResult
            {

                Partner = new PartnerSimple
                {
                    Id = x.Key,
                    Name = partner_dict[x.Key]
                },
                AmountTotal = x.Sum(x => (x.Debit - x.Credit) * sign),
                AmountCommissionTotal = commAgent_dict.ContainsKey(x.Key) ? commAgent_dict[x.Key] : 0
            }).ToList();


            var paged = new PagedResult2<CommissionAgentDetailResult>(totalItems, val.Offset, val.Limit)
            {
                Items = res
            };

            return paged;
        }

        public async Task<PagedResult2<CommissionAgentDetailItemResult>> GetCommissionAgentDetailItem(CommissionAgentDetailItemFilter val)
        {
            ///lấy dũ liệu từ accountmoveline
            var movelineObj = GetService<IAccountMoveLineService>();
            ///lây ra doanh thu các khách hàng có người giới thiệu
            var query = movelineObj._QueryGet(companyId: val.CompanyId, state: "posted", dateFrom: val.DateFrom,
              dateTo: val.DateTo);

            query = query.Where(x => x.Account.Code == "5111");

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            if (val.AgentId.HasValue)
                query = query.Where(x => x.Partner.AgentId.HasValue && x.Partner.AgentId == val.AgentId);

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

        public async Task<TotalAmountAgentResult> GetAmountCommissionAgentTotal(TotalAmountAgentFilter val)
        {
            var commAgent_dict = await ComputeCommissionAgents(new List<Guid> { val.AgentId.Value }, null, null, val.CompanyId);
            var incomeAgent_dict = await ComputeIncomeAgents(new List<Guid> { val.AgentId.Value }, null, null, val.CompanyId);

            var res = new TotalAmountAgentResult();
            res.AmountCommissionTotal = commAgent_dict.ContainsKey(val.AgentId.Value) ? commAgent_dict[val.AgentId.Value] : 0;
            res.AmountInComeTotal = incomeAgent_dict.ContainsKey(val.AgentId.Value) ? incomeAgent_dict[val.AgentId.Value] : 0;
            res.AmountBalanceTotal = res.AmountInComeTotal - res.AmountCommissionTotal;
            return res;
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
