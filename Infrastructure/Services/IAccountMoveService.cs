using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountMoveService : IBaseService<AccountMove>
    {
        Task ButtonCancel(IEnumerable<AccountMove> self);
        Task ButtonCancel(IEnumerable<Guid> ids);
        bool _CheckLockDate(IEnumerable<AccountMove> self);
        Task Unlink(IEnumerable<AccountMove> self);
        Task Unlink(IEnumerable<Guid> ids);
        Task Write(IEnumerable<AccountMove> self);
        Task<AccountJournal> GetDefaultJournalAsync(Guid? default_journal_id = null, Guid? default_company_id = null, string default_type = "entry");
        Task<IEnumerable<AccountMove>> CreateMoves(IEnumerable<AccountMove> vals_list, string default_type = "");
        bool IsSaleDocument(AccountMove self, bool include_receipts = false);
        bool IsPurchaseDocument(AccountMove self, bool include_receipts = false);
        bool IsInvoice(AccountMove self, bool include_receipts = false);
        IEnumerable<string> GetOutboundTypes(bool include_receipts = true);
        IEnumerable<string> GetInboundTypes(bool include_receipts = true);
        Task ActionPost(IEnumerable<AccountMove> self);
        void _ComputeAmount(IEnumerable<AccountMove> self);
        Task _ComputeAmount(IEnumerable<Guid> ids);
        Task<IEnumerable<AccountMove>> _ComputePaymentsWidgetReconciledInfo(IEnumerable<Guid> ids);
        Task<IEnumerable<AccountMove>> ButtonDraft(IEnumerable<Guid> ids);
    }
}
