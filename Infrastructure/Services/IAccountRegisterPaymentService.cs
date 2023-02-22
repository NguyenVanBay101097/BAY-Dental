using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountRegisterPaymentService: IBaseService<AccountRegisterPayment>
    {
        Task<AccountRegisterPaymentDisplay> DefaultGet(IEnumerable<Guid> invoice_ids);
        Task<AccountPayment> CreatePayment(Guid Id);
        Task<AccountRegisterPaymentDisplay> OrderDefaultGet(IEnumerable<Guid> order_ids);
    }
}
