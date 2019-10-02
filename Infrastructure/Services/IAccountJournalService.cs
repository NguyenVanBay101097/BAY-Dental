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
    }
}
