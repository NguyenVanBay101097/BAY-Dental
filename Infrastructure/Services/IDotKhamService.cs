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
        Task<PagedResult2<DotKham>> GetPagedResultAsync(DotKhamPaged val);
        Task ActionConfirm(Guid id);
        Task<DotKhamDisplay> DefaultGet(DotKhamDefaultGet val);
        Task<DotKham> GetDotKhamForDisplayAsync(Guid id);
        Task<IEnumerable<DotKham>> GetDotKhamsForInvoice(Guid invoiceId);
        Task<IEnumerable<AccountInvoiceCbx>> GetCustomerInvoices(Guid customerId);
        Task<IEnumerable<DotKham>> GetDotKhamsForSaleOrder(Guid saleOrderId);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task<DotKhamDisplay> GetSearchedDotKham(DotKhamEntitySearchBy search);
    }
}
