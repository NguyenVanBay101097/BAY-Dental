using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
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
            return await _repository.CountAsync(spec);
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
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            return await _repository.ListAsync(spec);
        }

        public IQueryable<TEntity> SearchQuery(Expression<Func<TEntity, bool>> domain = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int offSet = 0, int limit = int.MaxValue)
        {
            //CheckAccessRights(typeof(TEntity).Name, "Read");
            return _repository.SearchQuery(domain: domain, orderBy: orderBy, offSet: offSet, limit: limit);
        }

        public void Update(TEntity entity)
        {
            entity.WriteById = UserId;
            entity.LastUpdated = DateTime.Now;
            _repository.Update(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await _repository.UpdateAsync(new List<TEntity>() { entity });
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

            //CheckAccessRights(typeof(TEntity).Name, "Create");

            await _repository.InsertAsync(entities);

            return entities;
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return _repository.SqlQuery<TElement>(sql, parameters);
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

            //CheckAccessRights(typeof(TEntity).Name, "Write");

            await _repository.UpdateAsync(entities);
        }

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            //CheckAccessRights(typeof(TEntity).Name, "Unlink");

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
    }
}
