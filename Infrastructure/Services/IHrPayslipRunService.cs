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
        Task<HrPayslipRun> CreatePayslipRun(HrPayslipRunSave val);
        Task UpdatePayslipRun(Guid id, HrPayslipRunSave val);
        Task ActionConfirm(PaySlipRunConfirmViewModel val);

        Task ActionDone(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);

        Task CreatePayslipByRunId(Guid id);
        Task ComputeSalaryByRunId(Guid id);
    }
}
