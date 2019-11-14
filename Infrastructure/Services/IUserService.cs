using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public interface IUserService
    {
        IQueryable<ApplicationUser> GetQueryById(string id);
        void ClearSecurityCache(IEnumerable<string> ids);
        void ClearRuleCache(IEnumerable<string> ids);
    }
}
