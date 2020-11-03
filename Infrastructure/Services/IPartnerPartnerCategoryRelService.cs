using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.Services
{
    public interface IPartnerPartnerCategoryRelService
    {
        IQueryable<PartnerPartnerCategoryRel> SearchQuery(Expression<Func<PartnerPartnerCategoryRel, bool>> domain = null);
    }
}
