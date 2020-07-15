using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPhieuThuChiService : IBaseService<PhieuThuChi>
    {
        Task<PagedResult2<PhieuThuChiBasic>> GetPhieuThuChiPagedResultAsync(PhieuThuChiPaged val);
        Task<PhieuThuChi> GetByIdPhieuThuChi(Guid id);
        Task<PhieuThuChi> CreatePhieuThuChi(PhieuThuChiSave val);
        Task UpdatePhieuThuChi(Guid id, PhieuThuChiSave val);

        Task Unlink(Guid id);
        Task ActionConfirm(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> id);

    }
}
