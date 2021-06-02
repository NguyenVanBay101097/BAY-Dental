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

        Task<decimal> GetCommissionAgentBalance(Guid id, Guid partnerId);

        Task<decimal> GetInComeAmountAgent(Guid id);
        Task<decimal> GetCommissionAmountAgent(Guid id);

        Task<AgentDisplay> GetDisplayById(Guid id);

    }
}
