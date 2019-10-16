using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IIRRuleService : IBaseService<IRRule>
    {
        IEnumerable<IRRule> _SearchRules(string model, string mode);
    }
}
