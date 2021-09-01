using ApplicationCore.Entities;
using ApplicationCore.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAppointmentService : IBaseService<Appointment>
    {
        Task<PagedResult2<AppointmentBasic>> GetPagedResultAsync(AppointmentPaged val);
        Task<AppointmentDisplay> DefaultGet(AppointmentDefaultGet val);
        Task<IEnumerable<AppointmentStateCount>> CountAppointment(DateFromTo dateFromTo);
        Task<AppointmentDisplay> GetAppointmentDisplayAsync(Guid id);
        Task<IEnumerable<AppointmentBasic>> SearchRead(AppointmentSearch val);
        Task<PagedResult2<AppointmentBasic>> SearchReadByDate(AppointmentSearchByDate val);
        Task<AppointmentBasic> GetBasic(Guid id);
        Task<long> GetCount(AppointmentGetCountVM val);
        Task<Appointment> CreateAsync(AppointmentDisplay val);
        Task UpdateAsync(Guid id,AppointmentDisplay val);
        Task<IEnumerable<AppointmentBasic>> GetExcelData(AppointmentPaged val);
        void ComputeDataExcel(ExcelWorksheet worksheet, IEnumerable<AppointmentBasic> data, Dictionary<string, string> stateDict);

        Task<IEnumerable<EmployeeSimple>> GetListDoctor(AppointmentDoctorReq val);
    }
}
