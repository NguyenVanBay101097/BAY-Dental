using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IEmployeeService: IBaseService<Employee>
    {
        Task<PagedResult2<EmployeeBasic>> GetPagedResultAsync(EmployeePaged val);
        Task<IEnumerable<EmployeeSimple>> GetAutocompleteAsync(EmployeePaged val);
    }
}
