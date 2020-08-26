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
        Task<PagedResult2<HrPayslipDisplay>> GetPaged(HrPayslipPaged val);
        Task<HrPayslip> GetHrPayslipDisplay(Guid Id);
        Task<IEnumerable<HrPayslipLine>> ComputePayslipLine(Guid? EmployeeId, Guid? structureId, DateTime? DateFrom, DateTime? DateTo);
        Task ActionConfirm(IEnumerable<Guid> ids);
    }
}
