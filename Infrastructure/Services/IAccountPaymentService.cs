using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountPaymentService: IBaseService<AccountPayment>
    {
        Task Post(IEnumerable<Guid> ids);
        Task<PagedResult2<AccountPaymentBasic>> GetPagedResultAsync(AccountPaymentPaged val);
        Task CancelAsync(IEnumerable<Guid> ids);
        Task UnlinkAsync(IEnumerable<Guid> ids);
        void _ComputeResidualPayment(IEnumerable<SaleOrder> saleOrders, decimal amount);
        Task<IEnumerable<AccountPaymentBasic>> GetPaymentBasicList(AccountPaymentFilter val);
        Task<AccountRegisterPaymentDisplay> OrderDefaultGet(IEnumerable<Guid> saleOrderIds);
        Task<AccountPayment> CreateUI(AccountPaymentSave val);
        Task<AccountRegisterPaymentDisplay> PartnerDefaultGet(Guid partnerId);
        Task ActionDraftUnlink(IEnumerable<Guid> ids);
        Task<AccountRegisterPaymentDisplay> ServiceCardOrderDefaultGet(IEnumerable<Guid> order_ids);

        Task<AccountPaymentPrintVM> GetPrint(Guid id);

     
    }
}
