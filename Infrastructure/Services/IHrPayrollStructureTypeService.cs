using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHrPayrollStructureTypeService : IBaseService<HrPayrollStructureType>
    {
        Task<PagedResult2<HrPayrollStructureTypeDisplay>> GetPaged(HrPayrollStructureTypePaged val);
        Task<HrPayrollStructureType> GetHrPayrollStructureTypeDisplay(Guid Id);

        Task<IEnumerable<HrPayrollStructureTypeSimple>> GetAutocompleteAsync(HrPayrollStructureTypePaged val);
    }
}
