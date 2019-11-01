using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountMoveLineService : IBaseService<AccountMoveLine>
    {
        IEnumerable<AccountMoveLine> _AmountResidual(IEnumerable<AccountMoveLine> lines);
        void _StoreBalance(IEnumerable<AccountMoveLine> lines);
        Task Unlink(IEnumerable<AccountMoveLine> lines);
        ComputeAmountFieldsRes ComputeAmountFields(decimal amount);
        Task<bool> Reconcile(IList<AccountMoveLine> moveLines, long? writeoffJournalId = null, long? writeoffAccountId = null,
         string type = "auto", DateTime? dateP = null);

        IQueryable<AccountMoveLine> _QueryGet(DateTime? dateTo = null, DateTime? dateFrom = null,
            IEnumerable<Guid> journalIds = null,
            string state = "all",
            Guid? companyId = null,
            bool initBal = false,
            IList<Guid> companyIds = null);

        ISpecification<AccountMoveLine> _QueryGetSpec(DateTime? dateTo = null, DateTime? dateFrom = null,
           IEnumerable<Guid> journalIds = null,
           string state = "all",
           Guid? companyId = null,
           bool initBal = false,
           IList<Guid> companyIds = null);

        Task RemoveMoveReconcile(IEnumerable<AccountMoveLine> self);
        Task RemoveMoveReconcile(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);
    }
}
