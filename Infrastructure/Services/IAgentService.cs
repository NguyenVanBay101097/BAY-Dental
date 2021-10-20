using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAgentService : IBaseService<Agent>
    {
        Task<PagedResult2<AgentBasic>> GetPagedResultAsync(AgentPaged val);

        Task<PagedResult2<CommissionAgentResult>> GetCommissionAgent(CommissionAgentFilter val);

        Task<PagedResult2<CommissionAgentDetailResult>> GetCommissionAgentDetail(CommissionAgentDetailFilter val);

        Task<PagedResult2<CommissionAgentDetailItemResult>> GetCommissionAgentDetailItem(CommissionAgentDetailItemFilter val);

        Task<TotalAmountAgentResult> GetAmountCommissionAgentTotal(TotalAmountAgentFilter val);

        Task<decimal> GetAmountDebitTotalAgent(Guid id, Guid? companyId, DateTime? dateFrom, DateTime? dateTo);

        Task<AgentDisplay> GetDisplayById(Guid id);

        Task<PagedResult2<AgentInfo>> GetAgentPagedResult(AgentPaged val);

    }
}
