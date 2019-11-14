using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMyCache _cache;
        public UserService(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IMyCache cache)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public IQueryable<ApplicationUser> GetQueryById(string id)
        {
            return _userManager.Users.Where(x => x.Id == id);
        }

        //public override ISpecification<ApplicationUser> RuleDomainGet(IRRule rule)
        //{
        //    var companyId = CompanyId;
        //    switch (rule.Code)
        //    {
        //        case "account.invoice_comp_rule":
        //            return new InitialSpecification<ApplicationUser>(x => x.CompanyId == companyId);
        //        default:
        //            return null;
        //    }
        //}

        public void ClearSecurityCache(IEnumerable<string> ids)
        {
            var tenant = _httpContextAccessor.HttpContext.GetTenant<AppTenant>();
            foreach (var id in ids)
            {
                _cache.RemoveByPattern(string.Format("{0}ir.model.access-{1}", tenant != null ? tenant.Hostname + "-" : "", id));
                _cache.RemoveByPattern(string.Format("{0}ir.rule-{1}", tenant != null ? tenant.Hostname + "-" : "", id));
            }
        }

        public void ClearRuleCache(IEnumerable<string> ids)
        {
            var tenant = _httpContextAccessor.HttpContext.GetTenant<AppTenant>();
            foreach (var id in ids)
            {
                _cache.RemoveByPattern(string.Format("{0}ir.rule-{1}", tenant != null ? tenant.Hostname + "-" : "", id));
            }
        }
    }
}
