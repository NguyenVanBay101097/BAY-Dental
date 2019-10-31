using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountInvoiceLineService : IBaseService<AccountInvoiceLine>
    {
        void ComputePrice(IEnumerable<AccountInvoiceLine> self);
        void UpdateRelatedData(ICollection<AccountInvoiceLine> invoiceLines, AccountInvoice invoice);
        Task<AccountInvoiceLineDisplay> DefaultGet(AccountInvoiceLineDefaultGet val);
        Task<AccountInvoiceLineOnChangeProductResult> OnChangeProduct(AccountInvoiceLineOnChangeProduct val);
        Task<IEnumerable<AccountInvoiceLineSimple>> GetDotKhamInvoiceLine(Guid id);
        Task<AccountAccount> _DefaultAccount(Guid journalId, string type);
        Task<AccountAccount> _DefaultAccount(Guid? journalId = null, string type = "", AccountJournal journal = null);
    }
}
