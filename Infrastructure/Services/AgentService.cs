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

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

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
                .Include(x => x.Commission)
                .Include(x => x.Bank)
                .Include(x => x.Customer)
                .Include(x => x.Employee)
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

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);


            var items = await query.Include(x => x.Partners).ToListAsync();


            //tính tổng doanh thu và tổng chi hoa hồng trang 1 page
            var agentIds = items.Select(x => x.Id).ToList();


            ///group by chi hoa hồng cho người giới thiêu
            var commAgent_dict = await ComputeCommissionAgents(agentIds, val.DateFrom, val.DateTo, val.CompanyId);

            ///group by doanh thu người giới thiêu
            var incomeAgent_dict = ComputeIncomeAgents(agentIds, val.DateFrom, val.DateTo, val.CompanyId);


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

        public IDictionary<Guid, decimal> ComputeIncomeAgents(IEnumerable<Guid> ids, DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var query = movelineObj._QueryGet(companyId: companyId, state: "posted", dateFrom: dateFrom,
               dateTo: dateTo);

            query = query.Where(x => x.Account.Code == "5111" && x.Partner.AgentId.HasValue && ids.Contains(x.Partner.AgentId.Value));

            var incomAgent_dict = query.GroupBy(x => x.Partner.AgentId.Value).Select(x => new
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

            var totalItems = res.Count();

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

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

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
            var incomeAgent_dict = ComputeIncomeAgents(new List<Guid> { val.AgentId.Value }, null, null, val.CompanyId);

            var res = new TotalAmountAgentResult();
            res.AmountCommissionTotal = commAgent_dict.ContainsKey(val.AgentId.Value) ? commAgent_dict[val.AgentId.Value] : 0;
            res.AmountInComeTotal = incomeAgent_dict.ContainsKey(val.AgentId.Value) ? incomeAgent_dict[val.AgentId.Value] : 0;
            res.AmountBalanceTotal = res.AmountInComeTotal - res.AmountCommissionTotal;
            return res;
        }

        public async Task<PhieuThuChiDisplay> GetCommissionPaymentByAgentId(GetCommissionPaymentByAgentIdReq val)
        {
            var journalObj = GetService<IAccountJournalService>();
            var commissionSettlementObj = GetService<ICommissionSettlementService>();

            var totalDebitAgent = await GetAmountDebitTotalAgent(val.AgentId, CompanyId, null, null);
            var totalAmount = await commissionSettlementObj.SearchQuery(x => x.AgentId.HasValue && x.AgentId == val.AgentId && x.CompanyId == CompanyId).SumAsync(x => x.Amount ?? 0);
            var totalResidual = totalAmount - totalDebitAgent.AmountDebitTotal;

            if (totalAmount == 0)
                throw new Exception("Tiền hoa hồng bằng 0, không thể chi hoa hồng");
          
            if (totalResidual <= 0)
                throw new Exception("Tiền hoa hồng đã được thanh toán đủ");

            var res = new PhieuThuChiDisplay();
            res.Date = DateTime.Now;
            res.Type = val.Type;
            res.CompanyId = CompanyId;
            var journal = await journalObj.SearchQuery(x => x.CompanyId == CompanyId && x.Type == "cash").FirstOrDefaultAsync();
            res.Journal = _mapper.Map<AccountJournalSimple>(journal);
            res.IsAccounting = false;

            return res;
        }

        public async Task<AmountDebitTotalAgentReponse> GetAmountDebitTotalAgent(Guid id, Guid? companyId, DateTime? dateFrom, DateTime? dateTo)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();

            var agent = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var query = moveLineObj._QueryGet(dateTo: dateTo, dateFrom: dateFrom, companyId: companyId, state: "posted");

            query = query.Where(x => x.PartnerId == agent.PartnerId && x.Account.Code == "HHNGT");

            var res = new AmountDebitTotalAgentReponse();
            res.AmountDebitTotal = await query.SumAsync(s => s.Debit);

            return res;
        }

        public async Task<PagedResult2<AgentInfo>> GetAgentPagedResult(AgentPaged val)
        {
            var settlementObj = GetService<ICommissionSettlementService>();
            var phieuthuchiObj = GetService<IPhieuThuChiService>();

            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Select(x => new AgentInfo
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Classify = x.Classify,
                PartnerId = x.PartnerId,
            }).ToListAsync();

            var ids = items.Select(x => x.Id).ToList();
            var companyId = CompanyId;
            var commission_agents = await settlementObj.SearchQuery(x => x.AgentId.HasValue && ids.Contains(x.AgentId.Value) && x.CompanyId == companyId)
                .GroupBy(x => x.AgentId.Value)
                .Select(x => new { 
                    AgentId = x.Key,
                    BaseAmountTotal = x.Sum(s => s.BaseAmount),
                    TotalAmount = x.Sum(s => s.Amount),
                }).ToListAsync();
            var commissiont_dict = commission_agents.ToDictionary(x => x.AgentId, x => x);

            var phieuthuchis = await phieuthuchiObj.SearchQuery(x => x.AgentId.HasValue && ids.Contains(x.AgentId.Value) && x.State == "posted" && x.CompanyId == companyId)
                .GroupBy(x => x.AgentId.Value)
                .Select(x => new {
                    AgentId = x.Key,
                    TotalAmount = x.Sum(s => s.Amount),
                }).ToListAsync();

            var phieuthuchi_dict = phieuthuchis.ToDictionary(x => x.AgentId, x => x);

            foreach (var item in items)
            {
                item.BaseAmount = commissiont_dict.ContainsKey(item.Id) ? (commissiont_dict[item.Id].BaseAmountTotal ?? 0) : 0;
                item.Amount = commissiont_dict.ContainsKey(item.Id) ? (commissiont_dict[item.Id].TotalAmount ?? 0) : 0;
                item.AmountCommission = phieuthuchi_dict.ContainsKey(item.Id) ? phieuthuchi_dict[item.Id].TotalAmount : 0;
            }

            var paged = new PagedResult2<AgentInfo>(totalItems, val.Offset, val.Limit)
            {
                Items = items
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
            switch (rule.Code)
            {
                case "base.agent_comp_rule":
                    return new InitialSpecification<Agent>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
