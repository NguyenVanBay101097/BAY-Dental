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

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            var items = await _mapper.ProjectTo<PhieuThuChiBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
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
            var phieuThuChi = _mapper.Map<PhieuThuChi>(val);

            if (string.IsNullOrEmpty(phieuThuChi.Name))
            {
                var seqObj = GetService<IIRSequenceService>();
                if (phieuThuChi.Type == "thu")
                {
                    phieuThuChi.Name = await seqObj.NextByCode("phieu.thu");
                    if (string.IsNullOrEmpty(phieuThuChi.Name))
                    {
                        await _InsertPhieuThuSequence();
                        phieuThuChi.Name = await seqObj.NextByCode("phieu.thu");
                    }
                }
                else if (phieuThuChi.Type == "chi")
                {
                    phieuThuChi.Name = await seqObj.NextByCode("phieu.chi");
                    if (string.IsNullOrEmpty(phieuThuChi.Name))
                    {
                        await _InsertPhieuChiSequence();
                        phieuThuChi.Name = await seqObj.NextByCode("phieu.chi");
                    }
                }
                else
                    throw new Exception("Not support");
            }

            return await CreateAsync(phieuThuChi);
        }

        private async Task _InsertPhieuThuSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu thu",
                Code = "phieu.thu",
                Prefix = "THU/{yyyy}/",
                Padding = 4
            });
        }

        private async Task _InsertPhieuChiSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu chi",
                Code = "phieu.chi",
                Prefix = "CHI/{yyyy}/",
                Padding = 4
            });
        }

        public async Task UpdatePhieuThuChi(Guid id, PhieuThuChiSave val)
        {
            var phieuthuchi = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
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
                 .Include(x => x.Journal.DefaultDebitAccount)
                 .Include(x => x.Journal.DefaultCreditAccount)
                 .Include(x => x.LoaiThuChi)
                 .Include(x => x.LoaiThuChi.Account)
                 .Include(x => x.MoveLines)
                 .ToListAsync();

            var moveObj = GetService<IAccountMoveService>();

            foreach (var phieuThuChi in phieuThuChis)
            {
                if (phieuThuChi.State != "draft")
                    throw new Exception("Chỉ những phiếu nháp mới được vào sổ.");

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
                AccountAccount liquidity_line_account;
                decimal counterpart_amount;
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

                var rec_pay_line_name = "/";
                if (phieu.Type == "thu")
                    rec_pay_line_name = "Thu tiền";
                else if (phieu.Type == "chi")
                    rec_pay_line_name = "Chi tiền";

                var balance = counterpart_amount;
                var liquidity_line_name = phieu.Name;

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
                        Name = !string.IsNullOrEmpty(phieu.Reason) ? phieu.Reason : rec_pay_line_name,
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
                        AccountId = liquidity_line_account.Id,
                        Account = liquidity_line_account,
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

            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.MoveLines).ToListAsync();

            foreach (var phieuthuchi in self)
            {
                var move_ids = phieuthuchi.MoveLines.Select(x => x.MoveId).Distinct().ToList();
                await moveObj.ButtonCancel(move_ids);
                await moveObj.Unlink(move_ids);

                phieuthuchi.State = "draft";
            }

            await UpdateAsync(self);
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
                throw new Exception("Bạn không thể xóa phiếu khi đã xác nhận");

            await DeleteAsync(phieuthuchi);
        }

        public async Task InsertModelsIfNotExists()
        {
            var modelObj = GetService<IIRModelService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var model = await modelDataObj.GetRef<IRModel>("account.model_phieu_thu_chi");
            if (model == null)
            {
                model = new IRModel
                {
                    Name = "Phiếu thu chi",
                    Model = "PhieuThuChi",
                };

                modelObj.Sudo = true;
                await modelObj.CreateAsync(model);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "model_phieu_thu_chi",
                    Module = "sale",
                    Model = "ir.model",
                    ResId = model.Id.ToString()
                });
            }
        }

        public override ISpecification<PhieuThuChi> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "account.phieu_thu_chi_comp_rule":
                    return new InitialSpecification<PhieuThuChi>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<ReportPhieuThuChi>> ReportPhieuThuChi(PhieuThuChiSearch val)
        {
            var query = SearchQuery(x => x.State == "posted");
            if (!string.IsNullOrEmpty(val.Type) && (val.Type == "chi" || val.Type == "chi"))
                query = query.Where(x => x.Type.Equals(val.Type));
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            var list = await query.Select(x => new ReportPhieuThuChi
            {
                Id = x.Id,
                Name = x.Name,
                Amount = x.Amount,
                Type = x.Type
            }).ToListAsync();
            return list;
        }

        public async Task<List<PhieuThuChiExportExcel>> GetExportExcel(PhieuThuChiPaged val)
        {
            ISpecification<PhieuThuChi> spec = new InitialSpecification<PhieuThuChi>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PhieuThuChi>(x => x.Name.Contains(val.Search)));

            spec = spec.And(new InitialSpecification<PhieuThuChi>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            query = query.Skip(val.Offset).Take(val.Limit);

            var res = query.Select(x => new PhieuThuChiExportExcel
            {
                Date = x.Date,
                Name = x.Name,
                JournalName = x.Journal.Name,
                LoaiThuChiName = x.LoaiThuChi.Name,
                Amount = x.Amount,
                PayerReceiver = x.PayerReceiver,
                Reason = x.Reason,
                State = (x.State == "posted") ? "Đã xác nhận" : "Nháp",
            }).ToList();

            return res;
        }
    }
}
