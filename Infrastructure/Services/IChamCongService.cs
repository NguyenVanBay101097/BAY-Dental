using ApplicationCore.Entities;
using ApplicationCore.Models;
using Facebook.ApiClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IChamCongService : IBaseService<ChamCong>
    {
        Task CreateListChamcongs(IEnumerable<ChamCong> val);
        //Task<PagedResult2<EmployeeDisplay>> GetByEmployeePaged(employeePaged val);
        Task<IEnumerable<ChamCongDisplay>> GetAll(employeePaged val);
        Task<string> GetStatus(ChamCong val);
        Task<ChamCongDisplay> GetByEmployeeId(Guid id, DateTime date);
        Task<decimal> GetStandardWorkHour();
        Task CountChamCong(CountChamCong val);
        //Task<IEnumerable<ChamCongDisplay>> ExportFile(employeePaged val);
    }
}
