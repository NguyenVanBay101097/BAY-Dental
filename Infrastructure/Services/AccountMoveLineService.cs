using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
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

        public void _AmountResidual(IEnumerable<AccountMoveLine> lines)
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
        }

        public void _StoreBalance(IEnumerable<AccountMoveLine> lines)
        {
            foreach (var line in lines)
            {
                line.Balance = line.Debit - line.Credit;
            }
        }

        public async Task<bool> Reconcile(IList<AccountMoveLine> moveLines, long? writeoffJournalId = null, long? writeoffAccountId = null,
         string type = "auto", DateTime? dateP = null)
        {
            var companyIds = new HashSet<Guid>();
            var allAccounts = new HashSet<Guid>();
            var partners = new HashSet<Guid?>();
            foreach (var line in moveLines)
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

            await AutoReconcileLines(moveLines);
            return true;
        }

        private async Task AutoReconcileLines(IList<AccountMoveLine> moveLines)
        {
            if (!moveLines.Any())
                return;

            var pairRes = _GetPairToReconcile(moveLines);
            var smDebitMove = pairRes.Debit;
            var smCreditMove = pairRes.Credit;

            if (smCreditMove == null || smDebitMove == null)
                return;

            var amountReconcile = Math.Min(smDebitMove.AmountResidual, -smCreditMove.AmountResidual);
            if (amountReconcile == smDebitMove.AmountResidual)
                moveLines.Remove(smDebitMove);
            if (amountReconcile == -smCreditMove.AmountResidual)
                moveLines.Remove(smCreditMove);

            var amountReconcile2 = Math.Min(smDebitMove.AmountResidual, -smCreditMove.AmountResidual);
            var partialRecObj = GetService<IAccountPartialReconcileService>();

            await partialRecObj.CreateAsync(new List<AccountPartialReconcile>()
            {
                new AccountPartialReconcile
                {
                    DebitMove = smDebitMove,
                    DebitMoveId = smDebitMove.Id,
                    CreditMove = smCreditMove,
                    CreditMoveId = smCreditMove.Id,
                    Amount = amountReconcile2,
                    CompanyId = smDebitMove.CompanyId,
                    Company = smDebitMove.Company,
                }
            });

            //matched_debit_ids, matched_credit_ids thay đổi update residual 
            _AmountResidual(new List<AccountMoveLine>() { smDebitMove, smCreditMove });

            await AutoReconcileLines(moveLines);
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
