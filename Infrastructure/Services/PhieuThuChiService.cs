using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class PhieuThuChiService : BaseService<PhieuThuChi>, IPhieuThuChiService
    {
        private readonly IMapper _mapper;

        public PhieuThuChiService(IAsyncRepository<PhieuThuChi> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<PhieuThuChiBasic>> GetPhieuThuChiPagedResultAsync(PhieuThuChiPaged val)
        {
       
            ISpecification<PhieuThuChi> spec = new InitialSpecification<PhieuThuChi>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PhieuThuChi>(x => x.Name.Contains(val.Search)));

            spec = spec.And(new InitialSpecification<PhieuThuChi>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name));

            var items = await query.Select(x => new PhieuThuChiBasic
            {
                Id = x.Id,
                Name = x.Name,
                Date = x.Date,
                PayerReceiver = x.PayerReceiver,
                JournalName = x.Journal.Name,
                TypeName = x.LoaiThuChi.Name,
                Amount = x.Amount,
                State = x.State
            }).Skip(val.Offset).Take(val.Limit).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<PhieuThuChiBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PhieuThuChiDisplay> GetByIdPhieuThuChi(Guid id)
        {
            return await _mapper.ProjectTo<PhieuThuChiDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<PhieuThuChi> CreatePhieuThuChi(PhieuThuChiSave val)
        {
            var journalObj = GetService<IAccountJournalService>();
            var loaithuchiObj = GetService<ILoaiThuChiService>();

            var phieuThuChi = _mapper.Map<PhieuThuChi>(val);
            string sequence_code = "";
            string name = "";
            string suffix = "";
            if (string.IsNullOrEmpty(phieuThuChi.Name))
            {
                if (phieuThuChi.Type == "thu")
                {
                    sequence_code = "payment.slip";
                    name = "Phiếu Thu";
                    suffix = "THU";
                }

                else if (phieuThuChi.Type == "chi")
                {
                    sequence_code = "receipt.slip";
                    name = "Phiếu Chi";
                    suffix = "CHI";
                }
                else
                    throw new Exception("Not support");

                phieuThuChi.Name = await CheckSequenceCode(sequence_code, name, suffix);
            }
            phieuThuChi.Journal = await journalObj.GetByIdAsync(val.JournalId);
            phieuThuChi.LoaiThuChi = await loaithuchiObj.GetByIdAsync(val.LoaiThuChiId);
            phieuThuChi.CompanyId = CompanyId;
            phieuThuChi.State = "draft";
            return await CreateAsync(phieuThuChi);
        }

        public async Task UpdatePhieuThuChi(Guid id, PhieuThuChiSave val)
        {
            var loaithuchiObj = GetService<ILoaiThuChiService>();
            var phieuthuchi = await SearchQuery(x => x.Id == id).Include(x => x.Company)
                .Include(x => x.Journal)
                .Include(x => x.LoaiThuChi)
                .Include(x => x.LoaiThuChi.Account)
                .Include(x => x.MoveLines)
                .FirstOrDefaultAsync();
            if (phieuthuchi == null)
                throw new Exception("Phiếu không tồn tại");

            phieuthuchi = _mapper.Map(val, phieuthuchi);
           

            await UpdateAsync(phieuthuchi);
        }

        /// <summary>
        /// xác nhận
        /// </summary>
        /// <returns></returns>
        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var phieuThuChis = await SearchQuery(x => ids.Contains(x.Id))
                 .Include(x => x.Company)
                 .Include(x => x.Journal)
                 .Include(x => x.LoaiThuChi)
                 .Include(x => x.LoaiThuChi.Account)
                 .Include(x => x.MoveLines)
                 .ToListAsync();


            var moveObj = GetService<IAccountMoveService>();

        foreach(var phieuThuChi in phieuThuChis)
            {
                if (phieuThuChi.State != "draft")
                    throw new Exception("Chỉ những phiếu nháp mới được vào sổ.");

                string sequence_code = "";
                string name = "";
                string suffix = "";
                if (string.IsNullOrEmpty(phieuThuChi.Name))
                {
                    if (phieuThuChi.Type == "thu")
                    {
                        sequence_code = "payment.slip";
                        name = "Phiếu Thu";
                        suffix = "THU";
                    }

                    else if (phieuThuChi.Type == "chi")
                    {
                        sequence_code = "receipt.slip";
                        name = "Phiếu Chi";
                        suffix = "CHI";
                    }
                    else
                        throw new Exception("Not support");

                    phieuThuChi.Name = await CheckSequenceCode(sequence_code, name, suffix);
                }

                var moves = _PreparePhieuThuChiMoves(phieuThuChis);

                var amlObj = GetService<IAccountMoveLineService>();
                foreach (var move in moves)
                    amlObj.PrepareLines(move.Lines);

                await moveObj.CreateMoves(moves);
                await moveObj.ActionPost(moves);

                foreach (var move in moves)
                    amlObj.ComputeMoveNameState(move.Lines);

                phieuThuChi.State = "posted";
            }

            await UpdateAsync(phieuThuChis);

        }

        public async Task<string> CheckSequenceCode(string code, string name, string suffix)
        {
            var seqObj = GetService<IIRSequenceService>();
            var namePhieu = await seqObj.NextByCode(code);
            if (string.IsNullOrEmpty(namePhieu))
            {
                var sequenceObj = GetService<IIRSequenceService>();
                await sequenceObj.CreateAsync(new IRSequence
                {
                    Name = name,
                    Prefix = suffix + "/{yyyy}/",
                    Code = code,
                    Padding = 4
                });

                namePhieu = await seqObj.NextByCode(code);
            }

            if (string.IsNullOrEmpty(namePhieu))
                throw new Exception($"You have to define a sequence for {code} in your company.");

            return namePhieu;
        }

        private IList<AccountMove> _PreparePhieuThuChiMoves(IList<PhieuThuChi> val)
        {
            var all_move_vals = new List<AccountMove>();
            foreach (var phieu in val)
            {
                decimal counterpart_amount = 0;
                AccountAccount liquidity_line_account = null;

                if (phieu.Type == "chi")
                {
                    counterpart_amount = phieu.Amount;
                    liquidity_line_account = phieu.Journal.DefaultDebitAccount;
                }
                else
                {
                    counterpart_amount = -phieu.Amount;
                    liquidity_line_account = phieu.Journal.DefaultCreditAccount;
                }

                var balance = counterpart_amount;
                var liquidity_amount = counterpart_amount;

                var rec_pay_line_name = "";
                if (phieu.Type == "thu")
                    rec_pay_line_name = "Phiếu Thu";
                else if (phieu.Type == "chi")
                    rec_pay_line_name = "Phiếu chi";

                var liquidity_line_name = "";
                liquidity_line_name = phieu.Name;

                var move_vals = new AccountMove
                {
                    Date = phieu.Date,
                    Ref = phieu.Communication,
                    JournalId = phieu.JournalId,
                    Journal = phieu.Journal,
                    CompanyId = phieu.CompanyId,
                };

                var lines = new List<AccountMoveLine>()
                {
                    
                    new AccountMoveLine
                    {
                        Name = rec_pay_line_name,
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        DateMaturity = phieu.Date,
                        AccountId = phieu.LoaiThuChi.AccountId.Value,
                        Account = phieu.LoaiThuChi.Account,
                        PhieuThuChiId = phieu.Id,
                        Move = move_vals,
                    },
                    new AccountMoveLine
                    {
                        Name = liquidity_line_name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        DateMaturity = phieu.Date,
                        AccountId = phieu.LoaiThuChi.AccountId.Value,
                        Account = phieu.LoaiThuChi.Account,
                        PhieuThuChiId = phieu.Id,
                        Move = move_vals,
                    },
                };

                move_vals.Lines = lines;

                all_move_vals.Add(move_vals);

            }


            return all_move_vals;
        }
        /// <summary>
        /// hủy phiếu
        /// </summary>
        /// <returns></returns>
        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            var phieuthuchis = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Company)
              .Include(x => x.Journal)
              .Include(x => x.LoaiThuChi)
              .Include(x => x.LoaiThuChi.Account)
              .Include(x => x.MoveLines)
              .Include("MoveLines.Move")
              .ToListAsync();
            foreach (var phieuthuchi in phieuthuchis)
            {
                foreach (var move in phieuthuchi.MoveLines.Select(x => x.Move).Distinct().ToList())
                {
                    await moveObj.ButtonCancel(new List<Guid>() { move.Id });
                    await moveObj.Unlink(new List<Guid>() { move.Id });
                }

                phieuthuchi.State = "draft";
            }

            await UpdateAsync(phieuthuchis);
        }

        public async Task Unlink(Guid id)
        {
            var phieuthuchi = await SearchQuery(x => x.Id == id).Include(x => x.Company)
               .Include(x => x.Journal)
               .Include(x => x.LoaiThuChi)
               .Include(x => x.LoaiThuChi.Account)
               .Include(x => x.MoveLines)
               .FirstOrDefaultAsync();
            if (phieuthuchi == null)
                throw new Exception("Phiếu không tồn tại");

            if (phieuthuchi.State == "posted")
                throw new Exception("Bạn không thể xóa phiếu khi đã ghi sổ");

            await DeleteAsync(phieuthuchi);
        }

        /// <summary>
        /// in phiếu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public async Task PrintPhieuThuChi(Guid id)
        //{

        //}


    }
}
