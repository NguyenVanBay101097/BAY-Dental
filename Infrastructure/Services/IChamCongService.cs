using ApplicationCore.Entities;
using ApplicationCore.Models;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
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
        ChamCongTinhCong TinhSoCongGioCong(ChamCong cc, IEnumerable<AttendanceInterval> attendanceIntervals, decimal hoursPerDay);

        Task ImportExcel(IFormFile file, Ex_ImportExcelDirect dir);
        //Task<IEnumerable<ChamCongDisplay>> ExportFile(employeePaged val);


    }
}
