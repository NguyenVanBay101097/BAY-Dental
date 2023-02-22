using ApplicationCore.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ProductAppointmentRelService : IProductAppointmentRelService
    {
        protected readonly CatalogDbContext _dbContext;

        public ProductAppointmentRelService(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAsync(IEnumerable<ProductAppointmentRel> entities)
        {
            _dbContext.ProductAppointmentRels.RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<ProductAppointmentRel> entities)
        {
            _dbContext.ProductAppointmentRels.AddRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<ProductAppointmentRel> SearchQuery(Expression<Func<ProductAppointmentRel, bool>> domain = null)
        {
            var query = _dbContext.Set<ProductAppointmentRel>().AsQueryable();
            if (domain != null)
                query = query.Where(domain);
            return query;
        }
    }
}
