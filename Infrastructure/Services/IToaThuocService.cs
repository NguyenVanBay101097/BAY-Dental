using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IToaThuocService: IBaseService<ToaThuoc>
    {
        Task<PagedResult2<ToaThuocBasic>> GetPagedResultAsync(ToaThuocPaged val);
        Task<ToaThuocDisplay> GetToaThuocForDisplayAsync(Guid id);
        Task<ToaThuocDisplay> DefaultGet(ToaThuocDefaultGet val);
        Task<ToaThuocLineDisplay> LineDefaultGet(ToaThuocLineDefaultGet val);
        Task<IEnumerable<ToaThuocBasic>> GetToaThuocsForDotKham(Guid dotKhamId);
        Task Write(ToaThuoc entity);
        Task<ToaThuocPrintViewModel> GetToaThuocPrint(Guid id);
        Task<ToaThuocDisplay> GetToaThuocFromUIAsync(Guid id);
        Task<ToaThuocBasic> CreateToaThuocFromUIAsync(ToaThuocSaveFromUI val);
        Task UpdateToaThuocFromUIAsync(Guid id, ToaThuocSaveFromUI val);
    }
}
