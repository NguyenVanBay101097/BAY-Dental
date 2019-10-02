using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountMoveService : BaseService<AccountMove>, IAccountMoveService
    {
        public AccountMoveService(IAsyncRepository<AccountMove> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
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

        public async Task Unlink(IEnumerable<AccountMove> self)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();
            foreach (var move in self)
            {
                //moveLineObj.UpdateCheck(move.Lines);
                await moveLineObj.Unlink(move.Lines); //unlink already update_check
            }

            await DeleteAsync(self);
        }
    }
}
