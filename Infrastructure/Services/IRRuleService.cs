using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRRuleService : BaseService<IRRule>, IIRRuleService
    {
        public IRRuleService(IAsyncRepository<IRRule> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        private async Task<IEnumerable<IRRule>> _SearchRulesAsync(string model, string mode)
        {
            var uid = UserId;
            switch (mode)
            {
                case "Read":
                    {
                        var rules = await SearchQuery(x => x.Model.Model == model && x.Active && x.PermRead &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToListAsync();
                        return rules;
                    }
                case "Create":
                    {
                        var rules = await SearchQuery(x => x.Model.Model == model && x.Active && x.PermCreate &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToListAsync();
                        return rules;
                    }
                case "Write":
                    {
                        var rules = await SearchQuery(x => x.Model.Model == model && x.Active && x.PermWrite &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToListAsync();
                        return rules;
                    }
                case "Unlink":
                    {
                        var rules = await SearchQuery(x => x.Model.Model == model && x.Active && x.PermUnlink &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToListAsync();
                        return rules;
                    }
                default:
                    throw new Exception("Invalid mode");
            }
        }

        public IEnumerable<IRRule> _SearchRules(string model, string mode)
        {
            var uid = UserId;
            switch (mode)
            {
                case "Read":
                    {
                        var rules = SearchQuery(x => x.Model.Model == model && x.Active && x.PermRead &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                case "Create":
                    {
                        var rules = SearchQuery(x => x.Model.Model == model && x.Active && x.PermCreate &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                case "Write":
                    {
                        var rules = SearchQuery(x => x.Model.Model == model && x.Active && x.PermWrite &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                case "Unlink":
                    {
                        var rules = SearchQuery(x => x.Model.Model == model && x.Active && x.PermUnlink &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                default:
                    throw new Exception("Invalid mode");
            }
        }

        public async Task<PagedResult2<IRRule>> GetPagedAsync(int offset = 0, int limit = 10, string filter = "")
        {
            ISpecification<IRRule> spec = new InitialSpecification<IRRule>(x => true);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                spec = spec.And(new InitialSpecification<IRRule>(x => x.Name.Contains(filter)));
            }

            // the implementation below using ForEach and Count. We need a List.
            var itemsOnPage = await SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name), limit: limit, offSet: offset).ToListAsync();
            var totalItems = await CountAsync(spec);
            return new PagedResult2<IRRule>(totalItems, offset, limit)
            {
                Items = itemsOnPage
            };
        }
    }
}
