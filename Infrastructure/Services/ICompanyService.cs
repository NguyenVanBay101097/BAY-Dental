using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICompanyService: IBaseService<Company>
    {
        Task SetupCompany(string companyName, string userName, string email, string password, string name = "");
        Task InsertModuleAccountData(Company main_company);
        Task InsertModuleStockData(Company main_company);
        Task SetupTenant(CompanySetupTenant val);
        Task<PagedResult2<CompanyBasic>> GetPagedResultAsync(CompanyPaged val);
        Task InsertSecurityData();
        Task InsertCompanyData(Company company);
        Task Unlink(Company self);
        Task InsertModuleProductData();
        Task InsertModuleDentalData();
        Task InsertIRulesIfNotExists();
        Task ActionArchive(IEnumerable<Guid> ids);
        Task ActionUnArchive(IEnumerable<Guid> ids);
    }
}
