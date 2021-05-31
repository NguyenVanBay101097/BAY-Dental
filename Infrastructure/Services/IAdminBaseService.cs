using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAdminBaseService<T> where T : AdminBaseEntity
    {
        Task<T> GetByIdAsync(object id);

        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        Task UpdateAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);
        Task DeleteAsync(IEnumerable<T> entities);

        IQueryable<T> SearchQuery(Expression<Func<T, bool>> domain = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int offSet = 0,
            int limit = int.MaxValue);
    }
}
