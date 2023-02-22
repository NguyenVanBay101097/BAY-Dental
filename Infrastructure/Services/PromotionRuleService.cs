using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public class PromotionRuleService : BaseService<PromotionRule>, IPromotionRuleService
    {
        public PromotionRuleService(IAsyncRepository<PromotionRule> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public override ISpecification<PromotionRule> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.promotion_rule_comp_rule":
                    return new InitialSpecification<PromotionRule>(x => !x.Program.ProgramCompanyRels.Any() || x.Program.ProgramCompanyRels.Any(s => s.CompanyId == companyId));
                default:
                    return null;
            }
        }
    }
}
