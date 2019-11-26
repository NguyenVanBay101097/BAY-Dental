using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountMoveLineService : BaseService<AccountMoveLine>, IAccountMoveLineService
    {

        public AccountMoveLineService(IAsyncRepository<AccountMoveLine> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public IQueryable<AccountMoveLine> _QueryGet(DateTime? dateTo = null, DateTime? dateFrom = null,
            IEnumerable<Guid> journalIds = null,
            string state = "all",
            Guid? companyId = null,
            bool initBal = false,
            IList<Guid> companyIds = null)
        {

            if (journalIds == null)
                journalIds = new List<Guid>();
            if (companyIds == null)
                companyIds = new List<Guid>();
            return SearchQuery(x => (!dateTo.HasValue || x.Date <= dateTo.Value) &&
                        (!dateFrom.HasValue || ((initBal && x.Date < dateFrom.Value) || (!initBal && x.Date >= dateFrom.Value))) &&
                        (!journalIds.Any() || journalIds.Contains(x.JournalId.Value)) &&
                        (!companyId.HasValue || x.CompanyId == companyId.Value) &&
                        (state == "all" || x.Move.State == state) &&
                         (!companyIds.Any() || companyIds.Contains(x.Company.Id)));
        }

        public ISpecification<AccountMoveLine> _QueryGetSpec(DateTime? dateTo = null, DateTime? dateFrom = null,
           IEnumerable<Guid> journalIds = null,
           string state = "all",
           Guid? companyId = null,
           bool initBal = false,
           IList<Guid> companyIds = null)
        {

            if (journalIds == null)
                journalIds = new List<Guid>();
            if (companyIds == null)
                companyIds = new List<Guid>();
            return new InitialSpecification<AccountMoveLine>(x => (!dateTo.HasValue || x.Date <= dateTo.Value) &&
                        (!dateFrom.HasValue || ((initBal && x.Date < dateFrom.Value) || (!initBal && x.Date >= dateFrom.Value))) &&
                        (!journalIds.Any() || journalIds.Contains(x.JournalId.Value)) &&
                        (!companyId.HasValue || x.CompanyId == companyId.Value) &&
                        (state == "all" || x.Move.State == state) &&
                         (!companyIds.Any() || companyIds.Contains(x.Company.Id)));
        }

        public ComputeAmountFieldsRes ComputeAmountFields(decimal amount)
        {
            var debit = amount > 0 ? amount : 0;
            var credit = amount < 0 ? -amount : 0;
            return new ComputeAmountFieldsRes { Debit = debit, Credit = credit };
        }

        public override Task<AccountMoveLine> CreateAsync(AccountMoveLine entity)
        {
            _StoreBalance(new List<AccountMoveLine>() { entity });
            _AmountResidual(new List<AccountMoveLine>() { entity });
            return base.CreateAsync(entity);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            await Unlink(SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Move).ToList());
        }

        public async Task Unlink(IEnumerable<AccountMoveLine> self)
        {
            UpdateCheck(self);
            await DeleteAsync(self);
        }

        public void UpdateCheck(IEnumerable<AccountMoveLine> lines)
        {
            var moves = new List<AccountMove>().AsEnumerable();
            foreach (var line in lines)
            {
                if (line.Move.State != "draft")
                    throw new Exception("Bạn không thể xóa phát sinh kế toán đã vào sổ.");
                if (line.Reconciled == true && !(line.Debit == 0 && line.Credit == 0))
                    throw new Exception("Bạn không thể xóa những phát sinh kế toán đã được đối soát.");
                if (line.Move != null)
                    moves = moves.Union(new List<AccountMove>() { line.Move });
            }

            if (moves.Any())
            {
                var moveObj = GetService<IAccountMoveService>();
                moveObj._CheckLockDate(moves);
            }
        }

        public IEnumerable<AccountMoveLine> _AmountResidual(IEnumerable<Guid> ids)
        {
            var self = SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Account).Include(x => x.MatchedDebits)
                .Include(x => x.MatchedCredits).ToList();
            return _AmountResidual(self);
        }

        public IEnumerable<AccountMoveLine> _AmountResidual(IEnumerable<AccountMoveLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.Account.Reconcile == false)
                {
                    line.Reconciled = false;
                    line.AmountResidual = 0;
                    continue;
                }

                var amount = Math.Abs(line.Debit - line.Credit);
                var sign = line.Debit - line.Credit > 0 ? 1 : -1;

                foreach (var partialLine in line.MatchedDebits.Concat(line.MatchedCredits))
                {
                    var signPartialLine = partialLine.CreditMoveId == line.Id ? sign : (-1 * sign);
                    amount += signPartialLine * partialLine.Amount;
                }

                bool reconciled = false;
                if (amount == 0)
                    reconciled = true;

                line.Reconciled = reconciled;
                line.AmountResidual = Math.Round(amount * sign);
            }

            return lines;
        }

        public void _StoreBalance(IEnumerable<AccountMoveLine> lines)
        {
            foreach (var line in lines)
            {
                line.Balance = line.Debit - line.Credit;
            }
        }

        public async Task<bool> Reconcile(IList<AccountMoveLine> self, long? writeoffJournalId = null, long? writeoffAccountId = null,
         string type = "auto", DateTime? dateP = null)
        {
            var companyIds = new HashSet<Guid>();
            var allAccounts = new HashSet<Guid>();
            var partners = new HashSet<Guid?>();
            foreach (var line in self)
            {
                companyIds.Add(line.Company.Id);
                allAccounts.Add(line.Account.Id);
                if (line.Account.InternalType == "receivable" || line.Account.InternalType == "payable")
                    partners.Add(line.PartnerId);
                if (line.Reconciled == true)
                    throw new Exception("Bạn đang đối soát bút toán đã được đối soát rồi!");
            }

            if (companyIds.Count > 1)
                throw new Exception("Tất cả bút toán đối soát phải thuộc cùng một công ty!");
            if (allAccounts.Count > 1)
                throw new Exception("Tất cả bút toán không cùng một tài khoản!");
            if (partners.Count > 1)
                throw new Exception("Tất cả bút toán không cùng thuộc một đối tác!");

            await AutoReconcileLines(self);

            await CheckFullReconcile(self);
            return true;
        }

        private async Task CheckFullReconcile(IList<AccountMoveLine> self)
        {
            //Get first all aml involved
            var partRecObj = GetService<IAccountPartialReconcileService>();
            var selfIds = self.Select(x => x.Id).ToList();
            var todo = await partRecObj.SearchQuery(x => selfIds.Contains(x.DebitMoveId) || selfIds.Contains(x.CreditMoveId))
                .Include(x => x.DebitMove).Include(x => x.CreditMove).ToListAsync();
            var amls = self.Distinct().ToList();
            var seen = new List<Guid>();
            var partial_recs = new List<AccountPartialReconcile>();
            while (todo.Any())
            {
                var aml_tmps = todo.Select(x => x.DebitMove).Union(todo.Select(x => x.CreditMove)).Distinct().ToList();
                amls = amls.Union(aml_tmps).ToList();
                seen = seen.Union(todo.Select(x => x.Id)).Distinct().ToList();
                partial_recs = partial_recs.Union(todo).ToList();

                todo = await partRecObj.SearchQuery(x => (selfIds.Contains(x.DebitMoveId) || selfIds.Contains(x.CreditMoveId)) &&
                !seen.Contains(x.Id))
              .Include(x => x.DebitMove).Include(x => x.CreditMove).ToListAsync();
            }

            decimal total_debit = 0;
            decimal total_credit = 0;
            foreach (var aml in amls)
            {
                total_debit += aml.Debit;
                total_credit += aml.Credit;
            }

            if (total_debit == total_credit)
            {
                var fullRecObj = GetService<IAccountFullReconcileService>();
                var fullRec = new AccountFullReconcile();
                fullRec.PartialReconciles = partial_recs;
                fullRec.ReconciledLines = amls;
                await fullRecObj.CreateAsync(fullRec);
            }
        }

        private async Task<IList<AccountMoveLine>> AutoReconcileLines(IList<AccountMoveLine> self)
        {
            // Create list of debit and list of credit move ordered by date-currency
            var debit_moves = self.Where(x => x.Debit != 0).ToList();
            var credit_moves = self.Where(x => x.Credit != 0).ToList();
            debit_moves = debit_moves.OrderBy(x => x.Date).ToList();
            credit_moves = credit_moves.OrderBy(x => x.Date).ToList();

            return await _ReconcileLines(debit_moves, credit_moves);

            //if (!self.Any())
            //    return;

            //var pairRes = _GetPairToReconcile(self);
            //var smDebitMove = pairRes.Debit;
            //var smCreditMove = pairRes.Credit;

            //if (smCreditMove == null || smDebitMove == null)
            //    return;

            //var amountReconcile = Math.Min(smDebitMove.AmountResidual, -smCreditMove.AmountResidual);
            //if (amountReconcile == smDebitMove.AmountResidual)
            //    self.Remove(smDebitMove);
            //if (amountReconcile == -smCreditMove.AmountResidual)
            //    self.Remove(smCreditMove);

            //var amountReconcile2 = Math.Min(smDebitMove.AmountResidual, -smCreditMove.AmountResidual);
            //var partialRecObj = GetService<IAccountPartialReconcileService>();

            //await partialRecObj.CreateAsync(new List<AccountPartialReconcile>()
            //{
            //    new AccountPartialReconcile
            //    {
            //        DebitMove = smDebitMove,
            //        DebitMoveId = smDebitMove.Id,
            //        CreditMove = smCreditMove,
            //        CreditMoveId = smCreditMove.Id,
            //        Amount = amountReconcile2,
            //        CompanyId = smDebitMove.CompanyId,
            //        Company = smDebitMove.Company,
            //    }
            //});

            ////matched_debit_ids, matched_credit_ids thay đổi update residual 
            //_AmountResidual(new List<AccountMoveLine>() { smDebitMove, smCreditMove });

            //await AutoReconcileLines(self);
        }

        private async Task<IList<AccountMoveLine>> _ReconcileLines(List<AccountMoveLine> debit_moves, List<AccountMoveLine> credit_moves)
        {
            var to_create = new List<AccountPartialReconcile>();
            while(debit_moves.Any() && credit_moves.Any())
            {
                var debit_move = debit_moves[0];
                var credit_move = credit_moves[0];
                var temp_amount_residual = Math.Min(debit_move.AmountResidual, -credit_move.AmountResidual);
                var amount_reconcile = Math.Min(debit_move.AmountResidual, -credit_move.AmountResidual);

                if (amount_reconcile == debit_move.AmountResidual)
                    debit_moves.Remove(debit_move);
                else
                    debit_moves[0].AmountResidual -= temp_amount_residual;

                if (amount_reconcile == -credit_move.AmountResidual)
                    credit_moves.Remove(credit_move);
                else
                    credit_moves[0].AmountResidual += temp_amount_residual;

                to_create.Add(new AccountPartialReconcile
                {
                    DebitMoveId = debit_move.Id,
                    CreditMoveId = credit_move.Id,
                    Amount = amount_reconcile,
                    CompanyId = debit_move.CompanyId
                });
            }

            var partRecObj = GetService<IAccountPartialReconcileService>();
            await partRecObj.CreateAsync(to_create);

            return debit_moves.Concat(credit_moves).ToList();
        }

        private PairToReconcileRes _GetPairToReconcile(IEnumerable<AccountMoveLine> moveLines)
        {
            var sortedMoves = moveLines.OrderBy(x => x.Date);
            AccountMoveLine credit = null;
            AccountMoveLine debit = null;
            foreach (var aml in sortedMoves)
            {
                if (credit != null && debit != null)
                    break;
                if (aml.AmountResidual > 0 && debit == null)
                    debit = aml;
                else if (aml.AmountResidual < 0 && credit == null)
                    credit = aml;
            }

            return new PairToReconcileRes { Credit = credit, Debit = debit };
        }

        public async Task RemoveMoveReconcile(IEnumerable<Guid> moveIds)
        {
            await RemoveMoveReconcile(SearchQuery(x => moveIds.Contains(x.Id)).Include(x => x.MatchedDebits)
                .Include(x => x.MatchedCredits).ToList());
        }

        public async Task RemoveMoveReconcile(IEnumerable<AccountMoveLine> self)
        {
            if (!self.Any())
                return;

            var recMoveObj = GetService<IAccountPartialReconcileService>();
            var invObj = GetService<IAccountInvoiceService>();
            var recMoves = new List<AccountPartialReconcile>().AsEnumerable();
            foreach (var moveLine in self)
            {
                recMoves = recMoves.Concat(moveLine.MatchedDebits);
                recMoves = recMoves.Concat(moveLine.MatchedCredits);
            }

            var amlsToRecompute = recMoves.Select(x => x.DebitMoveId).Concat(recMoves.Select(x => x.CreditMoveId)).ToList();
            await recMoveObj.Unlink(recMoves.Select(x => x.Id).ToList());

            //trigger update
            var amls = _AmountResidual(amlsToRecompute);
            await UpdateAsync(amls);

            var invoiceIds = amls.Where(x => x.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();
            if (invoiceIds.Any())
            {
                await invObj.UpdatePayments(invoiceIds);
                await invObj.UpdateResidual(invoiceIds);
            }
        }

        public override ISpecification<AccountMoveLine> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.account_move_line_comp_rule":
                    return new InitialSpecification<AccountMoveLine>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }

    public class ComputeAmountFieldsRes
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class PairToReconcileRes
    {
        public AccountMoveLine Credit { get; set; }

        public AccountMoveLine Debit { get; set; }
    }
}
