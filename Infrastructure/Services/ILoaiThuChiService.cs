using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILoaiThuChiService : IBaseService<LoaiThuChi>
    {
        Task<PagedResult2<LoaiThuChiBasic>> GetThuChiPagedResultAsync(LoaiThuChiPaged val);

        LoaiThuChiDisplay DefaultGet(LoaiThuChiDefault val);

        Task<LoaiThuChi> GetByIdThuChi(Guid id);
        Task<LoaiThuChi> CreateLoaiThuChi(LoaiThuChiSave val);
        Task UpdateLoaiThuChi(Guid id, LoaiThuChiSave val);
        Task RemoveLoaiThuChi(Guid id);
        Task InsertModelsIfNotExists();
    }
}
