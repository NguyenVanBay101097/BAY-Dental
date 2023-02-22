using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IEmployeeCategoryService: IBaseService<EmployeeCategory>
    {
        Task<PagedResult2<EmployeeCategoryBasic>> GetPagedResultAsync(EmployeeCategoryPaged val);
        Task<IEnumerable<EmployeeCategoryBasic>> GetAutocompleteAsync(EmployeeCategoryPaged val);
    }
}
