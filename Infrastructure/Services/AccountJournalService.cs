using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountJournalService : BaseService<AccountJournal>, IAccountJournalService
    {

        public AccountJournalService(IAsyncRepository<AccountJournal> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<AccountJournal> GetJournalByTypeAndCompany(string type, Guid companyId)
        {
            return await SearchQuery(x => x.Type == type && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<AccountJournal>> CreateAsync(IEnumerable<AccountJournal> self)
        {
            foreach(var journal in self)
            {
                var seq = await _CreateSequence(journal);
                journal.SequenceId = seq.Id;

                if (journal.DedicatedRefund && (journal.Type == "sale" || journal.Type == "purchase"))
                {
                    var refund_seq = await _CreateSequence(journal, refund: true);
                    journal.RefundSequenceId = refund_seq.Id;
                }
            }

            return await base.CreateAsync(self);
        }

        private async Task<IRSequence> _CreateSequence(AccountJournal journal, bool refund = false)
        {
            var seqObj = GetService<IIRSequenceService>();
            var prefix = _GetSequencePrefix(journal.Code, refund: refund);
            var seq = new IRSequence
            {
                Name = journal.Name,
                Prefix = prefix,
                Padding = 4,
                NumberIncrement = 1,
                NumberNext = 1,
                CompanyId = journal.CompanyId,
            };
            
            return await seqObj.CreateAsync(seq);
        }

        public string _GetSequencePrefix(string code, bool refund = false)
        {
            var prefix = code.ToUpper();
            if (refund)
                prefix = "R" + prefix;
            return prefix + "/{yyyy}/";
        }

        public async Task<AccountJournal> GetJournalWithDebitCreditAccount(Guid journalId)
        {
            return await SearchQuery(x => x.Id == journalId)
                .Include(x => x.DefaultCreditAccount)
                .Include(x => x.DefaultDebitAccount)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AccountJournalSimple>> GetAutocomplete(AccountJournalFilter val)
        {
            var types = val.Type.Split(',');
            var query = SearchQuery(domain: x => string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search), orderBy: x => x.OrderBy(s => s.Code));
            if (types.Any())
                query = query.Where(x => types.Contains(x.Type));

            var items = await query.Skip(0).Take(20)
                .Select(x => new AccountJournalSimple() {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            return items;
        }

        public override ISpecification<AccountJournal> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.journal_comp_rule":
                    return new InitialSpecification<AccountJournal>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
