using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    }
}
