using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IProductAppointmentRelService
    {
        IQueryable<ProductAppointmentRel> SearchQuery(Expression<Func<ProductAppointmentRel, bool>> domain = null);
        Task DeleteAsync(IEnumerable<ProductAppointmentRel> entities);
        Task CreateAsync(IEnumerable<ProductAppointmentRel> entities);

    }
}
