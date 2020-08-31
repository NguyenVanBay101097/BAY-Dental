using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "account.account_move_comp_rule":
                    return new InitialSpecification<AccountMove>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }

        //public override Task<AccountMove> CreateAsync(AccountMove self)
        //{
        //    var amlObj = GetService<IAccountMoveLineService>();
        //    amlObj._AmountResidual(self.Lines);
        //    amlObj._StoreBalance(self.Lines);

        //    _ComputeAmount(new List<AccountMove>() { self });
        //    _ComputePartner(new List<AccountMove>() { self });
        //    return base.CreateAsync(self);
        //}

        public async Task<IEnumerable<AccountMove>> _ComputePaymentsWidgetReconciledInfo(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines).ToListAsync();

            foreach(var move in self)
            {
                if (move.State != "posted" || !IsInvoice(move, include_receipts: true))
                {
                    move.InvoicePaymentsWidget = "";
                    continue;
                }

                var reconciled_vals = await _GetReconciledInfoJSONValues(move);
                if (reconciled_vals.Any())
                    move.InvoicePaymentsWidget = JsonConvert.SerializeObject(reconciled_vals);
                else
                    move.InvoicePaymentsWidget = "";
            }

            return self;
        }

        public async Task<IList<PaymentInfoContent>> _GetReconciledInfoJSONValues(AccountMove self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var reconciled_vals = new List<PaymentInfoContent>();
            var pay_term_line_ids = await amlObj.SearchQuery(x => x.MoveId == self.Id && (x.Account.InternalType == "receivable" || x.Account.InternalType == "payable"))
                .Include(x => x.MatchedDebits).Include(x => x.MatchedCredits).ToListAsync();

            var partials = pay_term_line_ids.SelectMany(x => x.MatchedDebits).Concat(pay_term_line_ids.SelectMany(x => x.MatchedCredits)).ToList();
            foreach(var partial in partials)
            {
                var counterpart_lines = await amlObj.SearchQuery(x => x.Id == partial.DebitMoveId || x.Id == partial.CreditMoveId)
                    .Include(x => x.Journal).Include(x => x.Move).ToListAsync();

                var counterpart_line = counterpart_lines.Where(x => !self.Lines.Contains(x)).FirstOrDefault();
                var amount = partial.Amount;

                if (amount == 0)
                    continue;

                var reference = counterpart_line.Move.Name;
                if (!string.IsNullOrEmpty(counterpart_line.Move.Ref))
                    reference += " (" + counterpart_line.Move.Ref +")";

                reconciled_vals.Add(new PaymentInfoContent
                {
                    Name = counterpart_line.Name,
                    JournalName = counterpart_line.Journal.Name,
                    Amount = amount,
                    Date = counterpart_line.Date,
                    PaymentId = counterpart_line.Id,
                    AccountPaymentId = counterpart_line.PaymentId,
                    MoveId = counterpart_line.MoveId,
                    Ref = reference
                });
            }

            return reconciled_vals;
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
                decimal total_untaxed = 0;
                decimal total_tax = 0;
                decimal total_residual = 0;
                decimal total = 0;

                foreach (var line in move.Lines)
                {
                    if (IsInvoice(move, include_receipts: true))
                    {
                        if (line.ExcludeFromInvoiceTab == false)
                        {
                            total_untaxed += line.Balance;
                            total += line.Balance;
                        }
                        else if (line.Account.InternalType == "receivable" || line.Account.InternalType == "payable")
                        {
                            total_residual += line.AmountResidual;
                        }
                    }
                    else
                    {
                        if (line.Debit != 0)
                        {
                            total += line.Balance;
                        }
                    }
                }

                var sign = -1;
                if (move.Type == "entry" || IsOutbound(move))
                    sign = 1;

                move.AmountUntaxed = sign * total_untaxed;
                move.AmountTax = sign * total_tax;
                move.AmountTotal = sign * total;
                move.AmountResidual = -sign * total_residual;
                move.AmountUntaxedSigned = -total_untaxed;
                move.AmountTaxSigned = -total_tax;
                move.AmountTotalSigned = move.Type == "entry" ? Math.Abs(total) : -total;
                move.AmountResidualSigned = total_residual;

                var is_paid = move.AmountResidual == 0;

                if (move.Type == "entry")
                    move.InvoicePaymentState = null;
                else if (move.State == "posted" && is_paid)
                    move.InvoicePaymentState = "paid";
                else
                    move.InvoicePaymentState = "not_paid";
            }
        }

        public async Task _ComputeAmount(IEnumerable<Guid> ids)
        {
            var self = SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines)
                .Include("Lines.Account").ToList();
            _ComputeAmount(self);
            await UpdateAsync(self);
        }

        public async Task ActionPost(IEnumerable<AccountMove> self)
        {
            await Post(self);

            //sau khi post name va state thay đổi nên cập nhật lại cho lines
            foreach(var move in self)
            {
                foreach(var line in move.Lines)
                {
                    line.ParentState = move.State;
                    line.MoveName = move.Name;
                }
            }

            await UpdateAsync(self);
        }

        public async Task Post(IEnumerable<AccountMove> self)
        {
            _PostValidate(self);
            var seqObj = GetService<IIRSequenceService>();
            foreach (var move in self)
            {
                if (!move.PartnerId.HasValue)
                {
                    if (IsSaleDocument(move))
                        throw new Exception("The field 'Customer' is required, please complete it to validate the Customer Invoice.");
                    else if (IsPurchaseDocument(move))
                        throw new Exception("The field 'Vendor' is required, please complete it to validate the Vendor Bill.");
                }

                if (IsInvoice(move, include_receipts: true) && move.AmountTotal < 0)
                    throw new Exception("You cannot validate an invoice with a negative total amount. You should create a credit note instead. Use the action menu to transform it into a credit note or refund.");

                if (!move.InvoiceDate.HasValue && IsInvoice(move, include_receipts: true))
                    move.InvoiceDate = DateTime.Today;


                if (move.Name == "/")
                {
                    var sequenceId = _GetSequence(move);
                    if (!sequenceId.HasValue)
                        throw new Exception("Please define a sequence on your journal.");

                    move.Name = await seqObj.NextById(sequenceId.Value);
                }

                move.State = "posted";

                //Compute 'ref' for 'out_invoice'.
                if (move.Type == "out_invoice" && string.IsNullOrEmpty(move.InvoicePaymentRef))
                {
                    var invoice_payment_ref = _GetInvoiceComputedReference(move);
                    foreach (var line in move.Lines.Where(x => x.Account.InternalType == "receivable" || x.Account.InternalType == "payable"))
                        line.Name = invoice_payment_ref;
                }
            }

            await UpdateAsync(self);
        }

        private string _GetInvoiceComputedReference(AccountMove self)
        {
            return self.Name;
        }

        public Guid? _GetSequence(AccountMove self)
        {
            var journal = self.Journal;
            var types = new string[] { "entry", "out_invoice", "in_invoice", "out_receipt", "in_receipt" };
            if (types.Contains(self.Type) || !journal.DedicatedRefund)
                return journal.SequenceId;
            return journal.RefundSequenceId;
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
            //_CheckLockDate(moves);
        }

        public bool _CheckLockDate(IEnumerable<AccountMove> self)
        {
            //foreach (var move in self)
            //{
            //    var lock_date = move.Company.PeriodLockDate ?? DateTime.MinValue;
            //    if (move.Date <= lock_date)
            //        throw new Exception(string.Format("Bạn không thể ghi/thay đổi sổ sách trước hoặc trong ngày khóa sổ {0}. Kiểm tra cấu hình hoặc yêu cầu quyền Kế toán/Cố vấn", lock_date.ToString("d")));
            //}
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
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Journal).Include(x => x.Company).ToListAsync();
            await ButtonCancel(self);
        }

        public async Task<IEnumerable<AccountMove>> ButtonDraft(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines).ToListAsync();

            var amlObj = GetService<IAccountMoveLineService>();
            var line_ids = self.SelectMany(x => x.Lines).Select(x => x.Id).ToList();
            await amlObj.RemoveMoveReconcile(line_ids);

            foreach (var move in self)
                move.State = "draft";

            await UpdateAsync(self);
            return self;
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

        public async Task<IEnumerable<AccountMove>> CreateMoves(IEnumerable<AccountMove> vals_list, string default_type = "")
        {
            vals_list = await _MoveAutocompleteInvoiceLinesCreate(vals_list);

            _ComputeAmount(vals_list);
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
            var moveLineObj = GetService<IAccountMoveLineService>();
            foreach (var vals in vals_list)
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

                var new_vals = await _MoveAutocompleteInvoiceLinesValues(vals);
                moveLineObj.PrepareLines(new_vals.Lines);

                new_vals_list.Add(new_vals);
            }

            return new_vals_list;
        }

        public async Task<AccountMove> _MoveAutocompleteInvoiceLinesValues(AccountMove self)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();
            foreach(var line in self.Lines)
            {
                if (line.ExcludeFromInvoiceTab == true)
                    continue;

                line.PartnerId = self.PartnerId;
                line.Date = self.Date;
                line.Move = self;
                line.Account = await moveLineObj._GetComputedAccount(line);

                if (line.Account == null)
                {
                    if (IsSaleDocument(self, include_receipts: true))
                        line.Account = self.Journal.DefaultCreditAccount;
                    else if (IsPurchaseDocument(self, include_receipts: true))
                        line.Account = self.Journal.DefaultDebitAccount;
                }

                if (line.Product != null)
                    line.Name = moveLineObj._GetComputedName(line);
            }
          
            moveLineObj._OnChangePriceSubtotal(self.Lines);
            moveLineObj._ComputeBalance(self.Lines);

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

            IList<AccountMoveLine> _ComputeDiffPaymentTermsLines(IList<AccountMoveLine> existing_terms_lines, AccountAccount account, decimal balance, DateTime date_maturity)
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
                    candidate = new AccountMoveLine
                    {
                        Name = self.InvoicePaymentRef ?? "/",
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        Quantity = 1,
                        DateMaturity = date_maturity,
                        MoveId = self.Id,
                        Move = self,
                        AccountId = account.Id,
                        Account = account,
                        PartnerId = self.PartnerId,
                        ExcludeFromInvoiceTab = true
                    };
                }

                new_terms_lines.Add(candidate);
                return new_terms_lines;
            }


            var accountTypes = new List<string>() { "receivable", "payable" };
            var existing_terms_lines = self.Lines.Where(x => accountTypes.Contains(x.Account.InternalType)).ToList();
            var others_lines = self.Lines.Where(x => !accountTypes.Contains(x.Account.InternalType)).ToList();
            var total_balance = others_lines.Sum(x => x.Balance);

            if (!others_lines.Any())
            {
                self.Lines = self.Lines.Except(existing_terms_lines).ToList();
                return;
            }

            var computation_date = _GetPaymentTermsComputationDate();
            var account = _GetPaymentTermsAccount(existing_terms_lines);
            var new_terms_lines = _ComputeDiffPaymentTermsLines(existing_terms_lines, account, total_balance, computation_date);

            self.Lines = self.Lines.Except(existing_terms_lines).Concat(new_terms_lines).ToList();
        }

        private IEnumerable<string> GetInvoiceTypes(bool include_receipts = false)
        {
            var res = new List<string>() { "out_invoice", "out_refund", "in_invoice", "in_refund" };
            if (include_receipts)
                res = res.Concat(new List<string>() { "out_receipt", "in_receipt" }).ToList();
            return res;
        }

        public bool IsInvoice(AccountMove self, bool include_receipts = false)
        {
            return GetInvoiceTypes(include_receipts: include_receipts).Contains(self.Type);
        }

        public bool IsOutbound(AccountMove self, bool include_receipts = true)
        {
            return GetOutboundTypes(include_receipts: include_receipts).Contains(self.Type);
        }

        public bool IsSaleDocument(AccountMove self, bool include_receipts = false)
        {
            return GetSaleTypes(include_receipts: include_receipts).Contains(self.Type);
        }

        public bool IsPurchaseDocument(AccountMove self, bool include_receipts = false)
        {
            return GetPurchaseTypes(include_receipts: include_receipts).Contains(self.Type);
        }

        private IEnumerable<string> GetSaleTypes(bool include_receipts = false)
        {
            var res = new List<string>() { "out_invoice", "out_refund" };
            if (include_receipts)
                res = res.Concat(new List<string>() { "out_receipt" }).ToList();
            return res;
        }

        public IEnumerable<string> GetOutboundTypes(bool include_receipts = true)
        {
            var res = new List<string>() { "in_invoice", "out_refund" };
            if (include_receipts)
                res = res.Concat(new List<string>() { "in_receipt" }).ToList();
            return res;
        }

        public IEnumerable<string> GetInboundTypes(bool include_receipts = true)
        {
            var res = new List<string>() { "out_invoice", "in_refund" };
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
