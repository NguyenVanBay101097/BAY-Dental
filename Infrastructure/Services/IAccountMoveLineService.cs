using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountMoveLineService : IBaseService<AccountMoveLine>
    {
        void _AmountResidual(IEnumerable<AccountMoveLine> lines);
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
    }
}
