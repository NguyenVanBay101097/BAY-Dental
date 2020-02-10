using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Infrastructure.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        private readonly IAsyncRepository<TEntity> _repository;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(IAsyncRepository<TEntity> repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        protected string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        protected bool IsUserRoot
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return false;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "user_root");
                return claim != null ? bool.Parse(claim.Value) : false;
            }
        }

        public async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            CheckAccessRights(typeof(TEntity).Name, "Read");
            spec = _ApplyIRRules(spec, "Read");
            var query = _repository.SearchQuery(domain: spec.AsExpression());
            return await query.CountAsync();
        }

        public TEntity Create(TEntity entity)
        {
            entity.CreatedById = UserId;
            entity.WriteById = UserId;
            _repository.Insert(entity);
            return entity;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await CreateAsync(new List<TEntity>() { entity });
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _repository.Delete(entity);
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await DeleteAsync(new List<TEntity>() { entity });
        }

        public TEntity GetById(object id)
        {
            return _repository.GetById(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            if (id is string)
                id = Guid.Parse(id.ToString());
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            return await _repository.ListAsync(spec);
        }

        public IQueryable<TEntity> SearchQuery(Expression<Func<TEntity, bool>> domain = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int offSet = 0, int limit = int.MaxValue)
        {
            CheckAccessRights(typeof(TEntity).Name, "Read");
            ISpecification<TEntity> spec = new InitialSpecification<TEntity>(x => true);
            if (domain != null)
                spec = new InitialSpecification<TEntity>(domain);

            spec = _ApplyIRRules(spec, "Read");
            return _repository.SearchQuery(domain: spec.AsExpression(), orderBy: orderBy, offSet: offSet, limit: limit);
        }

        public void Update(TEntity entity)
        {
            entity.WriteById = UserId;
            entity.LastUpdated = DateTime.Now;
            _repository.Update(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await UpdateAsync(new List<TEntity>() { entity });
        }

        public virtual async Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
                return entities;

            if (!string.IsNullOrEmpty(UserId))
            {
                foreach (var entity in entities)
                {
                    entity.CreatedById = UserId;
                    entity.WriteById = UserId;
                }
            }

            CheckAccessRights(typeof(TEntity).Name, "Create");
            CheckAccessRules(entities, "Create");

            await _repository.InsertAsync(entities);

            return entities;
        }

        public IEnumerable<TEntity> SqlQuery(string sql, params object[] parameters)
        {
            return _repository.SqlQuery(sql, parameters);
        }

        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
                return;

            if (!string.IsNullOrEmpty(UserId))
            {
                foreach (var entity in entities)
                {
                    entity.WriteById = UserId;
                    entity.LastUpdated = DateTime.Now;
                }
            }

            CheckAccessRights(typeof(TEntity).Name, "Write");
            CheckAccessRules(entities, "Write");

            await _repository.UpdateAsync(entities);
        }

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            CheckAccessRights(typeof(TEntity).Name, "Unlink");
            CheckAccessRules(entities, "Unlink");

            await _repository.DeleteAsync(entities);
        }

        public EntityEntry<TEntity> GetEntry(TEntity entity)
        {
            return _repository.GetEntry(entity);
        }

        public void CheckAccessRights(string model, string operation)
        {
            var accessObj = GetService<IIRModelAccessService>();
            accessObj.Check(model, mode: operation);
        }

        private void CheckAccessRules(IEnumerable<TEntity> entities, string operation)
        {
            if (IsUserRoot)
                return;
            if (string.IsNullOrEmpty(UserId))
                return;

            var domain = DomainRuleGet(typeof(TEntity).Name, mode: operation);
            if (domain == null)
                return;

            if (entities.Any(x => !domain.AsExpression().Compile()(x)))
                throw new Exception(string.Format("Không thể hoàn tất thao tác được yêu cầu do hạn chế về bảo mật. Xin vui lòng liên hệ với quản trị hệ thống của bạn.\n\n(Document type: {0}, Operation: {1})", typeof(TEntity).Name, operation));
        }

        private ISpecification<TEntity> DomainRuleGet(string model_name, string mode)
        {
            if (string.IsNullOrEmpty(UserId))
                return null;
            string DOMAIN_RULE_CACHE_KEY = "ir.rule-{0}-{1}-{2}";

            var cache = GetService<IMyCache>();
            var userObj = GetService<IUserService>();
            var key = string.Format(DOMAIN_RULE_CACHE_KEY, UserId, model_name, mode);
            var tenant = _httpContextAccessor.HttpContext.GetTenant<AppTenant>();
            if (tenant != null)
                key = tenant.Hostname + "-" + key;
            var res = cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var rules = _SearchRules(model_name, mode);
                if (!rules.Any())
                    return null;
                var groupDomains = new Dictionary<Guid, IList<ISpecification<TEntity>>>();
                var globalDomains = new List<ISpecification<TEntity>>();
                var uid = UserId;
                var user = userObj.GetQueryById(uid).Include(x => x.ResGroupsUsersRels).FirstOrDefault();
                foreach (var rule in rules)
                {
                    var dom = RuleDomainGet(rule);
                    if (dom == null)
                        continue;

                    foreach (var ruleGroupRel in rule.RuleGroupRels)
                    {
                        if (user.ResGroupsUsersRels.Any(x => x.GroupId == ruleGroupRel.GroupId))
                        {
                            if (!groupDomains.ContainsKey(ruleGroupRel.GroupId))
                                groupDomains.Add(ruleGroupRel.GroupId, new List<ISpecification<TEntity>>());
                            groupDomains[ruleGroupRel.GroupId].Add(dom);
                        }
                    }

                    if (!rule.RuleGroupRels.Any())
                        globalDomains.Add(dom);
                }

                ISpecification<TEntity> groupDomain = null;
                if (groupDomains.Any())
                {
                    ISpecification<TEntity> tmp = null;
                    foreach (var item in groupDomains.Values)
                    {
                        ISpecification<TEntity> tmp2 = null;
                        foreach (var item2 in item)
                        {
                            if (tmp2 == null)
                            {
                                tmp2 = item2;
                                continue;
                            }
                            tmp2 = tmp2.And(item2);
                        }

                        if (tmp == null)
                        {
                            tmp = tmp2;
                            continue;
                        }
                        tmp = tmp.Or(tmp2);
                    }

                    groupDomain = tmp;
                }

                ISpecification<TEntity> domain = null;
                foreach (var d in globalDomains)
                {
                    if (domain == null)
                    {
                        domain = d;
                        continue;
                    }

                    domain = domain.And(d);
                }

                if (groupDomain != null && domain != null)
                    return domain.And(groupDomain);
                if (groupDomain != null)
                    return groupDomain;
                if (domain != null)
                    return domain;

                return null;
            });

            return res;
        }

        private IEnumerable<IRRule> _SearchRules(string model, string mode)
        {
            var ruleRepository = GetService<IAsyncRepository<IRRule>>();
            var uid = UserId;
            switch (mode)
            {
                case "Read":
                    {
                        var rules = ruleRepository.SearchQuery(x => x.Model.Model == model && x.Active && x.PermRead &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                case "Create":
                    {
                        var rules = ruleRepository.SearchQuery(x => x.Model.Model == model && x.Active && x.PermCreate &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                case "Write":
                    {
                        var rules = ruleRepository.SearchQuery(x => x.Model.Model == model && x.Active && x.PermWrite &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                case "Unlink":
                    {
                        var rules = ruleRepository.SearchQuery(x => x.Model.Model == model && x.Active && x.PermUnlink &&
                            ((x.RuleGroupRels.Any(s => s.Group.ResGroupsUsersRels.Any(m => m.UserId == uid))) || x.Global))
                            .Include(x => x.RuleGroupRels).ToList();
                        return rules;
                    }
                default:
                    throw new Exception("Invalid mode");
            }
        }

        private ISpecification<TEntity> _ApplyIRRules(ISpecification<TEntity> spec, string mode = "Read")
        {
            var domain = DomainRuleGet(typeof(TEntity).Name, mode: mode);
            if (domain != null)
                spec = spec.And(domain);
            return spec;
        }

        public virtual ISpecification<TEntity> RuleDomainGet(IRRule rule)
        {
            return null;
        }

        public Task<int> ExcuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return _repository.ExcuteSqlCommandAsync(sql, parameters);
        }
    }
}
