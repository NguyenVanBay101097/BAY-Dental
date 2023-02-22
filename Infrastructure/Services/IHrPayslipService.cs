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
        Task<HrPayslip> CreatePayslip(HrPayslipSave val);
        Task ComputeSheet(IEnumerable<Guid> ids);
        Task ActionDone(IEnumerable<Guid> ids);
        HrPayslipDefaultGetResult DefaultGet(HrPayslipDefaultGet val);
        Task Unlink(IEnumerable<Guid> ids);
        Task<IEnumerable<HrPayslipWorkedDayBasic>> GetWorkedDaysLines(Guid id);
        Task<IEnumerable<HrPayslipLineBasic>> GetLines(Guid id);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task UpdatePayslip(Guid id, HrPayslipSave val);
        Task<AccountJournal> InsertAccountJournalIfNotExists();
    }
}
