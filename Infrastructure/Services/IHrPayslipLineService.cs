using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrPayslipLineService : IBaseService<HrPayslipLine>
    {
        Task<PagedResult2<HrPayslipLineDisplay>> GetPaged(HrPayslipLinePaged val);
        Task<HrPayslipLine> GetHrPayslipLineDisplay(Guid Id);

        Task Remove(IEnumerable<Guid> Ids); 
        Task<HrPayslipDisplaySimple> GetAll(Guid payslipId);

    }
}
