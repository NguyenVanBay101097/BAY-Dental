using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    /// <summary>
    /// "There's some repetition here - couldn't we have some the sync methods call the async?"
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EfRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly IDbContext _dbContext;

        public EfRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, Func<IQueryable<T>, IOrderedQueryable<T>> sort = null,
            string includes = "",
            int offset = 0, int limit = int.MaxValue, bool isPagingEnabled = false)
        {
            var query = GetQuery(spec, sort: sort, includes: includes, offset: offset, limit: limit, isPagingEnabled: isPagingEnabled);
            return await query.ToListAsync();
        }

        public IQueryable<T> SearchQuery(ISpecification<T> spec, Func<IQueryable<T>, IOrderedQueryable<T>> sort = null,
          string includes = "",
          int offset = 0, int limit = int.MaxValue, bool isPagingEnabled = false)
        {
            return GetQuery(spec, sort: sort, includes: includes, offset: offset, limit: limit, isPagingEnabled: isPagingEnabled);
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            var query = GetQuery(spec);
            return await query.CountAsync();
        }

        public EntityEntry<T> GetEntry(T entity)
        {
            return _dbContext.Entry(entity);
        }


        public async Task<T> InsertAsync(T entity)
        {
            await InsertAsync(new List<T>() { entity });
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            await UpdateAsync(new List<T>() { entity });
        }

        public async Task DeleteAsync(T entity)
        {
            await DeleteAsync(new List<T>() { entity });
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }

        public T GetById(params object[] keyValues)
        {
            return _dbContext.Set<T>().Find(keyValues);
        }

        public T Insert(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<T> SearchQuery(Expression<Func<T, bool>> domain = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int offSet = 0, int limit = int.MaxValue)
        {
            var query = _dbContext.Set<T>().AsQueryable();
            if (domain != null)
                query = query.Where(domain);

            if (orderBy != null)
            {
                query = orderBy(query);
                query = query.Skip(offSet).Take(limit);
            }
            //else
            //{
            //    query = query.OrderBy(x => x.DateCreated);
            //}

            return query;
        }

        public virtual async Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities;
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return;

            foreach(var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec, Func<IQueryable<T>, IOrderedQueryable<T>> sort = null,
          string includes = "")
        {
            var query = GetQuery(spec, sort: sort, includes: includes);
            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<T> GetQuery(ISpecification<T> spec, Func<IQueryable<T>, IOrderedQueryable<T>> sort = null,
            string includes = "",
            int offset = 0, int limit = int.MaxValue, bool isPagingEnabled = false)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            if (spec.AsExpression() != null)
            {
                query = query.Where(spec.AsExpression());
            }

            if (!string.IsNullOrEmpty(includes))
            {
                var includeStrings = includes.Split(",");
                // Include any string-based include statements
                query = includeStrings.Aggregate(query,
                                        (current, include) => current.Include(include));
            }

            if (sort != null)
            {
                query = sort(query);
            }

            // Apply paging if enabled
            if (isPagingEnabled)
            {
                query = query.Skip(offset)
                             .Take(limit);
            }

            return query;
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            var query = GetQuery(spec);
            return await query.ToListAsync();
        }

        public Task<int> ExcuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return _dbContext.ExecuteSqlCommandAsync(sql, parameters);
        }
    }
}
