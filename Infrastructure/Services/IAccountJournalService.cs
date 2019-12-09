using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountJournalService: IBaseService<AccountJournal>
    {
        Task<AccountJournal> GetJournalByTypeAndCompany(string type, Guid companyId);
        Task<AccountJournal> GetJournalWithDebitCreditAccount(Guid journalId);
        Task<IEnumerable<AccountJournalSimple>> GetAutocomplete(AccountJournalFilter val);
        Task CreateJournals(IEnumerable<AccountJournalSave> vals);
        Task<PagedResult2<AccountJournalBasic>> GetBankCashJournals(AccountJournalFilter val);
        Task UpdateJournalSave(Guid id, AccountJournalSave val);
    }
}
