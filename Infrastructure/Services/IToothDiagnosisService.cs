using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IToothDiagnosisService : IBaseService<ToothDiagnosis>
    {
        Task<PagedResult2<ToothDiagnosisBasic>> GetPagedResultAsync(ToothDiagnosisPaged val);
        Task<IEnumerable<ToothDiagnosisBasic>> GetAutocompleteAsync(ToothDiagnosisPaged val);
        Task<ToothDiagnosisDisplay> GetToothDiagnosisDisplay(Guid id);
        Task<ToothDiagnosis> CreateToothDiagnosis(ToothDiagnosisSave val);
        Task UpdateToothDiagnosis(Guid id, ToothDiagnosisSave val);
        Task RemoveToothDiagnosis(Guid id);
        Task<IEnumerable<ProductSimple>> GetProducts(IEnumerable<Guid> ids);
    }
}
