using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IIRRuleService : IBaseService<IRRule>
    {
        IEnumerable<IRRule> _SearchRules(string model, string mode);
        Task<PagedResult2<IRRule>> GetPagedAsync(int offset = 0, int limit = 10, string filter = "");
    }
}
