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
        Task ActionConfirm(Guid id);
        Task<DotKhamDisplay> DefaultGet(DotKhamDefaultGet val);
        Task<DotKham> GetDotKhamForDisplayAsync(Guid id);
        Task<IEnumerable<DotKham>> GetDotKhamsForInvoice(Guid invoiceId);
        Task<IEnumerable<DotKham>> GetDotKhamsForSaleOrder(Guid saleOrderId);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);

        Task<IEnumerable<DotKhamBasic>> GetDotKhamBasicsForSaleOrder(Guid saleOrderId);

        Task<DotKhamDisplay> GetDotKhamDisplayAsync(Guid id);

        Task CreateOrUpdateDotKham(DotKhamVm val);
    }
}
