using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IDotKhamService: IBaseService<DotKham>
    {
        Task<PagedResult2<DotKhamBasic>> GetPagedResultAsync(DotKhamPaged val);
        Task<DotKhamDisplay> DefaultGet(DotKhamDefaultGet val);
        Task<IEnumerable<DotKham>> GetDotKhamsForSaleOrder(Guid saleOrderId);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);

        Task<IEnumerable<DotKhamBasic>> GetDotKhamBasicsForSaleOrder(Guid saleOrderId);

        Task<DotKhamDisplay> GetDotKhamDisplayAsync(Guid id);

        Task CreateDotKham(DotKhamSaveVm val);
        Task UpdateDotKham(Guid id, DotKhamSaveVm val);
    }
}
