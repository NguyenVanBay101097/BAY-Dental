using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IToaThuocService: IBaseService<ToaThuoc>
    {
        Task<ToaThuoc> GetToaThuocForDisplayAsync(Guid id);
        Task<ToaThuocDisplay> DefaultGet(ToaThuocDefaultGet val);
        Task<ToaThuocLineDisplay> LineDefaultGet(ToaThuocLineDefaultGet val);
        Task<IEnumerable<ToaThuocBasic>> GetToaThuocsForDotKham(Guid dotKhamId);
        Task Write(ToaThuoc entity);
        Task<ToaThuocPrintViewModel> GetToaThuocPrint(Guid id);
        Task CopyToaThuoc(CopyToaThuoc val);
        Task UsedPrescription(UsedPrescription val);
    }
}
