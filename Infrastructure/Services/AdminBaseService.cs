using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AdminBaseService<TEntity> : IAdminBaseService<TEntity> where TEntity : AdminBaseEntity
    {
        private readonly IAsyncRepository<TEntity> _repository;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public AdminBaseService(IAsyncRepository<TEntity> repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _repository.GetByIdAsync(id);
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

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await CreateAsync(new List<TEntity>() { entity });
            return entity;
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

            await _repository.InsertAsync(entities);

            return entities;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await DeleteAsync(new List<TEntity>() { entity });
        }

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            await _repository.DeleteAsync(entities);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await UpdateAsync(new List<TEntity>() { entity });
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

            await _repository.UpdateAsync(entities);
        }

        public IQueryable<TEntity> SearchQuery(Expression<Func<TEntity, bool>> domain = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int offSet = 0, int limit = int.MaxValue)
        {
            return _repository.SearchQuery(domain: domain, orderBy: orderBy, offSet: offSet, limit: limit);
        }
    }
}
