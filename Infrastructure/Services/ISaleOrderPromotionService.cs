using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderPromotionService : IBaseService<SaleOrderPromotion>
    {
        Task<PagedResult2<SaleOrderPromotionDisplay>> GetPagedResultAsync(SaleOrderPromotionPaged val);
        Task RemovePromotion(IEnumerable<Guid> ids);
    }
}
