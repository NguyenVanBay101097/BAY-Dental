using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrPayslipService : IBaseService<HrPayslip>
    {
        Task<PagedResult2<HrPayslipBasic>> GetPaged(HrPayslipPaged val);
        Task<HrPayslipDisplay> GetHrPayslipDisplay(Guid Id);
        Task<HrPayslipOnChangeEmployeeResult> OnChangeEmployee(Guid? employeeId, DateTime? dateFrom, DateTime? dateTo);
        Task ComputeSheet(IEnumerable<Guid> ids);
        Task ActionDone(IEnumerable<Guid> ids);
        Task SaveWorkedDayLines(HrPayslipSave val, HrPayslip payslip);
        HrPayslipDefaultGetResult DefaultGet(HrPayslipDefaultGet val);
        Task Unlink(IEnumerable<Guid> ids);
        Task<IEnumerable<HrPayslipWorkedDayBasic>> GetWorkedDaysLines(Guid id);
        Task<IEnumerable<HrPayslipLineBasic>> GetLines(Guid id);
    }
}
