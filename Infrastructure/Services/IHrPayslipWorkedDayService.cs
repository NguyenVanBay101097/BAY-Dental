using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrPayslipWorkedDayService : IBaseService<HrPayslipWorkedDays>
    {
        Task<IEnumerable<HrPayslipWorkedDayDisplay>> GetPaged(HrPayslipWorkedDayPaged val);
        Task<HrPayslipWorkedDays> GetHrPayslipWorkedDayDisplay(Guid Id);
        Task Remove(IEnumerable<Guid> Ids);
    }
}
