using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISalaryPaymentService : IBaseService<SalaryPayment>
    {
        Task<SalaryPayment> CreateSalaryPayment(SalaryPaymentSave val);
        Task UpdateSalaryPayment(Guid id, SalaryPaymentSave val);
        Task InsertModelsIfNotExists();
        Task ActionConfirm(IEnumerable<GuidClass> ids);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task CreateAndConfirmMultiSalaryPayment(IEnumerable<MultiSalaryPaymentVm> vals);
        Task<IEnumerable<SalaryPaymentDisplay>> DefaulCreateBy(SalaryPaymentDefaultGetModel val);
    }
}
