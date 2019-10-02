using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T>: IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<T> InsertAsync(T entity);
        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        Task UpdateAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteAsync(IEnumerable<T> entities);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
