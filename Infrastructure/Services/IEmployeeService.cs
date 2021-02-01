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
        Task<Employee> GetByUserIdAsync(string userId);
        Task updateSalary(EmployeeDisplay val, Employee emp);
        Task<Boolean> ActionActive(Guid id, EmployeeActive val);

        Task UpdateResgroupForSurvey(Employee empl);
    }
}
