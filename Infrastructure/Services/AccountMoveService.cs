using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountMoveService : BaseService<AccountMove>, IAccountMoveService
    {
        public AccountMoveService(IAsyncRepository<AccountMove> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public override ISpecification<AccountMove> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.account_move_comp_rule":
                    return new InitialSpecification<AccountMove>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public override Task<AccountMove> CreateAsync(AccountMove self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            amlObj._AmountResidual(self.Lines);
            amlObj._StoreBalance(self.Lines);

            _ComputeAmount(new List<AccountMove>() { self });
            _ComputePartner(new List<AccountMove>() { self });
            return base.CreateAsync(self);
        }

        private void _ComputePartner(IEnumerable<AccountMove> self)
        {
            foreach (var move in self)
            {
                var partners = move.Lines.Select(x => x.PartnerId).Distinct().ToList();
                if (partners.Count == 1 && partners[0] != null)
                    move.PartnerId = partners[0];
            }
        }

        public void _ComputeAmount(IEnumerable<AccountMove> self)
        {
            foreach (var move in self)
            {
                decimal total = 0;
                foreach (var line in move.Lines)
                {
                    total += line.Debit;
                }
                move.Amount = total;
            }
        }

        public async Task Post(IEnumerable<AccountMove> moves, AccountInvoice invoice = null)
        {
            _PostValidate(moves);
            var seqObj = GetService<IIRSequenceService>();
            foreach (var move in moves)
            {
                if (move.Name == "/")
                {
                    var newName = "";
                    var journal = move.Journal;
                    if (invoice != null && !string.IsNullOrEmpty(invoice.MoveName) && invoice.MoveName != "/")
                        newName = invoice.MoveName;
                    else
                    {
                        if (journal.Sequence != null)
                        {
                            var sequence = journal.Sequence;
                            if (invoice != null && (invoice.Type == "out_refund" || invoice.Type == "in_refund") && journal.RefundSequence != null)
                                sequence = journal.RefundSequence;

                            newName = await seqObj.NextById(sequence.Id);
                        }
                        else
                            throw new Exception("Vui lòng định nghĩa trình tự cho nhật ký này");
                    }

                    if (!string.IsNullOrEmpty(newName))
                        move.Name = newName;
                }

                move.State = "posted";
            }

            await UpdateAsync(moves);
        }

        public async Task Write(IEnumerable<AccountMove> self)
        {
            _ComputePartner(self);
            _ComputeAmount(self);
            await UpdateAsync(self);
        }

        private void _PostValidate(IEnumerable<AccountMove> moves)
        {
            foreach (var move in moves)
            {
                if (move.Lines.Any())
                {
                    if (!move.Lines.All(x => x.CompanyId == move.CompanyId))
                        throw new Exception("Không thể tạo bút toán khác công ty.");
                }
            }

            AssertBalanced(moves);
            _CheckLockDate(moves);
        }

        public bool _CheckLockDate(IEnumerable<AccountMove> self)
        {
            foreach (var move in self)
            {
                var lock_date = move.Company.PeriodLockDate ?? DateTime.MinValue;
                if (move.Date <= lock_date)
                    throw new Exception(string.Format("Bạn không thể ghi/thay đổi sổ sách trước hoặc trong ngày khóa sổ {0}. Kiểm tra cấu hình hoặc yêu cầu quyền Kế toán/Cố vấn", lock_date.ToString("d")));
            }
            return true;
        }

        public void AssertBalanced(IEnumerable<AccountMove> moves)
        {
            //var ids = moves.Select(x => x.Id).ToList();
            //var parameters = new List<SqlParameter>();
            //var move_ids_params = new string[ids.Count];
            //for (var i = 0; i < ids.Count; i++)
            //{
            //    move_ids_params[i] = string.Format("@move{0}", i);
            //    parameters.Add(new SqlParameter(move_ids_params[i], ids[i]));
            //}

            //parameters.Add(new SqlParameter("lm", 0.00001));
            //var res = SqlQuery<long?>("SELECT MoveId " +
            //    "FROM AccountMoveLines " +
            //    "WHERE MoveId in (" + string.Join(", ", move_ids_params) + ") " +
            //    "GROUP BY MoveId " +
            //    "HAVING abs(sum(Debit) - sum(Creadit)) > @lm", parameters.ToArray()).ToList();

            //if (res.Count != 0)
            //    throw new Exception("Không thể tạo bút toán không cân đối.");
        }

        public async Task ButtonCancel(IEnumerable<Guid> ids)
        {
            await ButtonCancel(SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Journal).Include(x => x.Company).ToList());
        }

        public async Task ButtonCancel(IEnumerable<AccountMove> self)
        {
            foreach (var move in self)
            {
                if (!move.Journal.UpdatePosted)
                    throw new Exception("Bạn không thể thay đổi bút toán đã vào sổ trong nhật ký này. Đầu tiên bạn nên cho nhật ký này có thể hủy bỏ bút toán.");
                move.State = "draft";
            }

            await UpdateAsync(self);
            _CheckLockDate(self);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            await Unlink(SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).ToList());
        }

        public async Task Unlink(IEnumerable<AccountMove> self)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();
            foreach (var move in self)
            {
                //moveLineObj.UpdateCheck(move.Lines);
                await moveLineObj.Unlink(move.Lines.Select(x => x.Id).ToList()); //unlink already update_check
            }

            await DeleteAsync(self);
        }

        public async Task<AccountJournal> GetDefaultJournalAsync(Guid? default_journal_id = null, Guid? default_company_id = null, string default_type = "entry")
        {
            var move_type = default_type;
            var journal_type = "general";
            if (GetSaleTypes(include_receipts: true).Contains(move_type))
                journal_type = "sale";
            else if (GetPurchaseTypes(include_receipts: true).Contains(move_type))
                journal_type = "purchase";

            var journalObj = GetService<IAccountJournalService>();
            AccountJournal journal = null;
            if (default_journal_id.HasValue)
            {
                journal = await journalObj.GetByIdAsync(default_journal_id.Value);
                if (move_type != "entry" && journal.Type != journal_type)
                    throw new Exception($"Cannot create an invoice of type {move_type} with a journal having {journal.Type} as type.");
            }
            else
            {
                var company_id = default_company_id ?? CompanyId;
                journal = await journalObj.SearchQuery(x => x.CompanyId == company_id && x.Type == journal_type).FirstOrDefaultAsync();

                if (journal == null)
                {
                    var error_msg = "Please define an accounting miscellaneous journal in your company";
                    if (journal_type == "sale")
                        error_msg = "Please define an accounting sale journal in your company";
                    else if (journal_type == "purchase")
                        error_msg = "Please define an accounting purchase journal in your company";
                    throw new Exception(error_msg);
                }
            }

            return journal;
        }

        public async Task<IEnumerable<AccountMove>> CreateMoves(IEnumerable<AccountMove> vals_list)
        {
            vals_list = await _MoveAutocompleteInvoiceLinesCreate(vals_list);
            var moves = await CreateAsync(vals_list);
            return moves;
        }

        private async Task<IEnumerable<AccountMove>> _MoveAutocompleteInvoiceLinesCreate(IEnumerable<AccountMove> vals_list, string default_type = "")
        {
            // During the create of an account.move with only 'invoice_line_ids' set and not 'line_ids', this method is called
            // to auto compute accounting lines of the invoice. In that case, accounts will be retrieved and taxes, cash rounding
            // and payment terms will be computed. At the end, the values will contains all accounting lines in 'line_ids'
            // and the moves should be balanced.
            var new_vals_list = new List<AccountMove>();
            foreach(var vals in vals_list)
            {
                if (!vals.InvoiceLines.Any())
                {
                    new_vals_list.Add(vals);
                    continue;
                }
                   
                if (vals.Lines.Any())
                {
                    new_vals_list.Add(vals);
                    continue;
                }

                foreach (var l in vals.InvoiceLines)
                    vals.Lines.Add(l);

                new_vals_list.Add(await _MoveAutocompleteInvoiceLinesValues(vals));
            }

            return new_vals_list;
        }

        public async Task<AccountMove> _MoveAutocompleteInvoiceLinesValues(AccountMove self)
        {
            await _RecomputeDynamicLines(new List<AccountMove>() { self });
            return self;
        }

        public async Task _RecomputeDynamicLines(IEnumerable<AccountMove> self)
        {
            foreach(var invoice in self)
            {
                if (IsInvoice(invoice, include_receipts: true))
                {
                    //Compute payment terms.
                    await _RecomputePaymentTermsLines(invoice);
                }
            }
        }

        private async Task _RecomputePaymentTermsLines(AccountMove self)
        {
            var today = DateTime.Today;

            DateTime _GetPaymentTermsComputationDate() {
                return self.InvoiceDate ?? today;
            }

            AccountAccount _GetPaymentTermsAccount(IList<AccountMoveLine> existing_terms_lines)
            {
                if (existing_terms_lines.Any())
                    return existing_terms_lines.First().Account;
                else
                {
                    var accountObj = GetService<IAccountAccountService>();
                    var companyId = CompanyId;
                    var types = new List<string>() { "out_invoice", "out_refund", "out_receipt" };
                    var type = types.Contains(self.Type) ? "receivable" : "payable";
                    return accountObj.SearchQuery(x => x.CompanyId == companyId && x.InternalType == type).FirstOrDefault();
                }
            }

            async Task<IList<AccountMoveLine>> _ComputeDiffPaymentTermsLines(IList<AccountMoveLine> existing_terms_lines, AccountAccount account, decimal balance, DateTime date_maturity)
            {
                existing_terms_lines = existing_terms_lines.OrderBy(x => x.DateMaturity).ToList();
                var existing_terms_lines_index = 0;

                var new_terms_lines = new List<AccountMoveLine>();
                AccountMoveLine candidate = null;
                if (existing_terms_lines_index < existing_terms_lines.Count)
                {
                    candidate = existing_terms_lines[existing_terms_lines_index];
                    existing_terms_lines_index += 1;

                    candidate.DateMaturity = date_maturity;
                    candidate.Debit = balance < 0 ? -balance : 0;
                    candidate.Credit = balance > 0 ? balance : 0;
                }
                else
                {
                    var moveLineObj = GetService<IAccountMoveLineService>();
                    candidate = await moveLineObj.CreateAsync(new AccountMoveLine
                    {
                        Name = self.InvoicePaymentRef ?? "",
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        Quantity = 1,
                        DateMaturity = date_maturity,
                        MoveId = self.Id,
                        AccountId = account.Id,
                        PartnerId = self.PartnerId,
                        ExcludeFromInvoiceTab = true
                    });
                }

                new_terms_lines.Add(candidate);
                return new_terms_lines;
            }


            var accountTypes = new List<string>() { "receivable", "payable" };
            var existing_terms_lines = self.Lines.Where(x => accountTypes.Contains(x.Account.UserType.Type)).ToList();
            var others_lines = self.Lines.Where(x => !accountTypes.Contains(x.Account.UserType.Type)).ToList();
            var total_balance = others_lines.Sum(x => x.Balance);

            if (!others_lines.Any())
            {
                self.Lines = self.Lines.Except(existing_terms_lines).ToList();
                return;
            }

            var computation_date = _GetPaymentTermsComputationDate();
            var account = _GetPaymentTermsAccount(existing_terms_lines);
            var new_terms_lines = await _ComputeDiffPaymentTermsLines(existing_terms_lines, account, total_balance, computation_date);

            self.Lines = self.Lines.Except(existing_terms_lines).Concat(new_terms_lines).ToList();
        }

    

        private bool IsInvoice(AccountMove self, bool include_receipts = false)
        {
            return GetInvoiceTypes(include_receipts: include_receipts).Contains(self.Type);
        }

        private IEnumerable<string> GetInvoiceTypes(bool include_receipts = false)
        {
            var res = new List<string>() { "out_invoice", "out_refund", "in_invoice", "in_refund" };
            if (include_receipts)
                res = res.Concat(new List<string>() { "out_receipt", "in_receipt" }).ToList();
            return res;
        }

        private IEnumerable<string> GetSaleTypes(bool include_receipts = false)
        {
            var res = new List<string>() { "out_invoice", "out_refund" };
            if (include_receipts)
                res = res.Concat(new List<string>() { "out_receipt" }).ToList();
            return res;
        }

        private IEnumerable<string> GetPurchaseTypes(bool include_receipts = false)
        {
            var res = new List<string>() { "in_invoice", "in_refund" };
            if (include_receipts)
                res = res.Concat(new List<string>() { "in_receipt" }).ToList();
            return res;
        }
    }
}
