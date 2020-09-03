using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrPayrollStructureService:IBaseService<HrPayrollStructure>
    {
        Task<PagedResult2<HrPayrollStructureBasic>> GetPaged(HrPayrollStructurePaged val);
        Task<HrPayrollStructureDisplay> GetHrPayrollStructureDisplay(Guid Id);
        Task Remove(Guid Id);
        Task<HrPayrollStructureBase> GetFirstOrDefault(Guid typeId);
        Task<IEnumerable< HrSalaryRuleDisplay>> GetRules(Guid structureId);
    }
}
