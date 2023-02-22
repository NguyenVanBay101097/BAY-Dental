using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareRuleService: IBaseService<TCareRule>
    {
        Task<object> GetDisplay(Guid id);
        Task<TCareRule> CreateRule(TCareRuleSave val);
        Task UpdateBirthdayRule(Guid id, TCareRuleBirthdayUpdate val);
    }
}
