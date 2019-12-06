using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class PromotionRuleService : BaseService<PromotionRule>, IPromotionRuleService
    {
        public PromotionRuleService(IAsyncRepository<PromotionRule> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
