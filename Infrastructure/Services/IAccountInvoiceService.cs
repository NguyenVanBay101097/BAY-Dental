using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountInvoiceService: IBaseService<AccountInvoice>
    {
        Task<AccountInvoice> GetAccountInvoiceForDisplayAsync(Guid id);
        Task<AccountInvoiceDisplay> DefaultGet(AccountInvoiceDisplay val);
        Task ActionInvoiceOpen(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task ActionCancelDraft(IEnumerable<Guid> ids);
        Task RegisterPayment(IEnumerable<Guid> invoiceIds, AccountMoveLine paymentLine);
        Task<PagedResult<AccountInvoice>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20);
        Task<PagedResult2<AccountInvoiceBasic>> GetPagedResultAsync(AccountInvoicePaged val);
        Task UpdateInvoice(AccountInvoice inv);
        Task<IEnumerable<PaymentInfoContent>> _GetPaymentInfoJson(Guid id);
        Task<IEnumerable<AccountInvoiceCbx>> GetOpenPaid(string search = "");
        Task<AccountInvoicePrint> GetAccountInvoicePrint(Guid id);
        Task DeleteInvoice(Guid id);
    }
}
