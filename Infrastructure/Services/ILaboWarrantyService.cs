using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILaboWarrantyService : IBaseService<LaboWarranty>
    {
        Task<PagedResult2<LaboWarrantyBasic>> GetPagedResultAsync(LaboWarrantyPaged val);
        Task<LaboWarrantyDisplay> GetDefault(LaboWarrantyGetDefault val);
        Task<LaboWarrantyDisplay> GetLaboWarrantyDisplay(Guid id);
        Task<LaboWarranty> CreateLaboWarranty(LaboWarrantySave val);
        Task UpdateLaboWarranty(Guid id, LaboWarrantySave val);
        Task Unlink(IEnumerable<Guid> ids);
        Task ButtonConfirm(IEnumerable<Guid> ids);
        Task ButtonCancel(IEnumerable<Guid> ids);
        Task ConfirmSendWarranty(Guid id, DateTime date);
        Task CancelSendWarranty(Guid id);
        Task ConfirmReceiptInspection(Guid id, DateTime date);
        Task CancelReceiptInspection(Guid id);
        Task ConfirmAssemblyWarranty(Guid id, DateTime date);
        Task CancelAssemblyWarranty(Guid id);
    }
}
