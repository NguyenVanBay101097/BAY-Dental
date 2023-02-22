using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IPartnerPartnerCategoryRelService
    {
        IQueryable<PartnerPartnerCategoryRel> SearchQuery(Expression<Func<PartnerPartnerCategoryRel, bool>> domain = null);
    }
}
