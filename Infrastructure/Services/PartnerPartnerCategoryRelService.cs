using ApplicationCore.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PartnerPartnerCategoryRelService : IPartnerPartnerCategoryRelService
    {
        protected readonly CatalogDbContext _dbContext;

        public PartnerPartnerCategoryRelService(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        

        public IQueryable<PartnerPartnerCategoryRel> SearchQuery(Expression<Func<PartnerPartnerCategoryRel, bool>> domain = null)
        {
            var query = _dbContext.Set<PartnerPartnerCategoryRel>().AsQueryable();
            if (domain != null)
                query = query.Where(domain);

            return query;
        }


    }
}
