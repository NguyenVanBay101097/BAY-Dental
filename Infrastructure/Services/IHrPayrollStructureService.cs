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
        Task<PagedResult2<HrPayrollStructureDisplay>> GetPaged(HrPayrollStructurePaged val);
        Task<HrPayrollStructure> GetHrPayrollStructureDisplay(Guid Id);
    }
}
