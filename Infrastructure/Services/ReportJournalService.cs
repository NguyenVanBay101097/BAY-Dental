using AutoMapper;
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
    public class ReportJournalService : IReportJournalService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportJournalService(IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public IEnumerable<AccountMoveLineBasic> Lines(IEnumerable<Guid> journal_ids, DateTime? date_from = null, DateTime? date_to = null, Guid? company_id = null)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var query = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: company_id,
                journalIds: journal_ids);

            var res = _mapper.ProjectTo<AccountMoveLineBasic>(query.OrderBy(x => x.Date).ThenBy(x => x.DateCreated)).ToList();
            return res;
        }

        public decimal _SumDebit(IEnumerable<Guid> journal_ids, DateTime? date_from = null, DateTime? date_to = null, Guid? company_id = null)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var query = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: company_id,
                journalIds: journal_ids);

            return query.Sum(x => x.Debit);
        }

        public decimal _SumCredit(IEnumerable<Guid> journal_ids, DateTime? date_from = null, DateTime? date_to = null, Guid? company_id = null)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var query = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: company_id,
                journalIds: journal_ids);

            return query.Sum(x => x.Credit);
        }

        public decimal _SumInitialBalance(IEnumerable<Guid> journal_ids, DateTime? date_from = null, Guid? company_id = null)
        {
            if (!date_from.HasValue)
                return 0;

            var amlObj = GetService<IAccountMoveLineService>();
            var query = amlObj._QueryGet(dateFrom: date_from, initBal: true, state: "posted", companyId: company_id,
                journalIds: journal_ids);

            return query.Sum(x => x.Debit - x.Credit);
        }

        public async Task<IEnumerable<ReportJournalItem>> GetReportValues(IEnumerable<Guid> journal_ids,
            DateTime? date_from = null, DateTime? date_to = null, Guid? company_id = null)
        {
            var res = new List<ReportJournalItem>();
            var journalObj = GetService<IAccountJournalService>();
            var journals = await _mapper.ProjectTo<AccountJournalBasic>(journalObj.SearchQuery(x => journal_ids.Contains(x.Id))).ToListAsync();
            foreach (var journal in journals)
            {
                res.Add(new ReportJournalItem
                {
                    Journal = journal,
                    Begin = _SumInitialBalance(date_from: date_from, company_id: company_id, journal_ids: new List<Guid>() { journal.Id }),
                    SumDebit = _SumDebit(date_from: date_from, date_to: date_to, company_id: company_id, journal_ids: new List<Guid>() { journal.Id }),
                    SumCredit = _SumCredit(date_from: date_from, date_to: date_to, company_id: company_id, journal_ids: new List<Guid>() { journal.Id }),
                    Lines = Lines(date_from: date_from, date_to: date_to, company_id: company_id, journal_ids: new List<Guid>() { journal.Id })
                });
            }

            return res;
        }

        public async Task<IEnumerable<ReportJournalItem>> GetCashBankReportValues(ReportCashBankJournalSearch val)
        {
            var journalObj = GetService<IAccountJournalService>();
            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            var journalIds = await journalObj.SearchQuery(x => types.Contains(x.Type)).Select(x => x.Id).ToListAsync();
            return await GetReportValues(journal_ids: journalIds, date_from: val.DateFrom, date_to: val.DateTo, company_id: val.CompanyId);
        }
    }
}
