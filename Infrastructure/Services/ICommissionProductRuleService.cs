using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICommissionProductRuleService : IBaseService<CommissionProductRule>
    {
        Task<IEnumerable<CommissionProductRuleDisplay>> GetForCommission(Guid CommissionId);
        Task CreateUpdateCommissionProductRules(IEnumerable<CommissionProductRuleSave> vals);
    }
}
