using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrPayslipRunService : IBaseService<HrPayslipRun>
    {
        Task<PagedResult2<HrPayslipRunBasic>> GetPagedResultAsync(HrPayslipRunPaged val);

        Task<HrPayslipRunDisplay> GetHrPayslipRunForDisplay(Guid id);
        Task<HrPayslipRunPrintVm> GetHrPayslipRunForPrint(Guid id);
        Task<HrPayslipRun> CreatePayslipRun(HrPayslipRunSave val);
        Task UpdatePayslipRun(Guid id, HrPayslipRunSave val);
        Task ActionConfirm(Guid id);

        Task ActionDone(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);

        Task<HrPayslipRunDisplay> CreatePayslipByRunId(Guid id);
        Task ComputeSalaryByRunId(Guid id);
        Task<HrPayslipRunDisplay> CheckExist(DateTime date);
    }
}
