using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class PartnerAdvanceService : BaseService<PartnerAdvance>, IPartnerAdvanceService
    {
        private readonly IMapper _mapper;
        private readonly IPartnerService _partnerService;
        private readonly IAccountJournalService _journalService;

        public PartnerAdvanceService(IAsyncRepository<PartnerAdvance> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IPartnerService partnerService, IAccountJournalService journalService)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _partnerService = partnerService;
            _journalService = journalService;
        }

        public async Task<PagedResult2<PartnerAdvanceBasic>> GetPagedResultAsync(PartnerAdvancePaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }


            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Include(x => x.Journal).Include(x => x.Partner);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<PartnerAdvanceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<PartnerAdvanceBasic>>(items)
            };

            return paged;
        }

        public async Task<PartnerAdvancDefaultViewModel> DefaultGet(PartnerAdvanceDefaultFilter val)
        {
            var partner = await _partnerService.GetByIdAsync(val.PartnerId);
            var journal = await _journalService.SearchQuery(x => x.Type == "cash" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            var amount = await ComputeAmountTotal();

            if (val.Type == "refund" && amount <= 0)
                throw new Exception("Lỗi");

            var res = new PartnerAdvancDefaultViewModel();
            res.Type = val.Type;
            res.PartnerId = partner.Id;
            res.PartnerName = partner.Name;
            res.JournalId = journal.Id;
            res.Journal = _mapper.Map<AccountJournalSimple>(journal);
            res.AmounAdvanceTotal = amount;
            res.Date = DateTime.Now;
            res.State = "draft";

            return res;
        }

        public async Task<decimal> GetSummary(PartnerAdvanceSummaryFilter val)
        {
            var query = SearchQuery(x => x.State == "confirmed");

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (!string.IsNullOrEmpty(val.Type))
            {
                query = query.Where(x => x.Type == val.Type);
            }
            else
            {
                var amountBalance = await ComputeAmountTotal();
                return amountBalance;
            }



            return await query.SumAsync(x => x.Amount);
        }

        public async Task<decimal> ComputeAmountTotal()
        {
            var advanceAmount = await SearchQuery(x => x.Type == "advance").SumAsync(x => x.Amount);
            var refundAmount = await SearchQuery(x => x.Type == "refund").SumAsync(x => x.Amount);
            return advanceAmount - refundAmount;
        }

        public async Task<PartnerAdvanceDisplay> GetDisplayById(Guid id)
        {
            var res = await SearchQuery(x => x.Id == id).Select(x => new PartnerAdvanceDisplay
            {
                Id = x.Id,
                Name = x.Name,
                Amount = x.Amount,
                Date = x.Date,
                PartnerId = x.PartnerId,
                PartnerName = x.Partner != null ? x.Partner.Name : null,
                JournalId = x.JournalId.Value,
                Journal = x.Journal != null ? new AccountJournalSimple
                {
                    Id = x.Journal.Id,
                    Name = x.Journal.Name
                } : null,
                CompanyId = x.CompanyId.Value,
                Note = x.Note,
                Type = x.Type,
                State = x.State
            }).FirstOrDefaultAsync();

            return res;
        }

        public async Task<PartnerAdvance> CreatePartnerAdvance(PartnerAdvanceSave val)
        {
            var res = _mapper.Map<PartnerAdvance>(val);
            await CreateAsync(res);
            return res;
        }

        public async Task UpdatePartnerAdvance(Guid id, PartnerAdvanceSave val)
        {
            var res = await SearchQuery(x => x.Id == id)
                .Include(x => x.Journal)
                .FirstOrDefaultAsync();

            res = _mapper.Map(val, res);

            await UpdateAsync(res);
        }

        public override async Task<PartnerAdvance> CreateAsync(PartnerAdvance entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("partner.advance");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await _InsertPartnerAdvanceSequence();
                entity.Name = await sequenceService.NextByCode("partner.advance");
            }

            await base.CreateAsync(entity);

            return entity;
        }

        private async Task _InsertPartnerAdvanceSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Khách hàng tạm ứng",
                Code = "partner.advance",
                Prefix = "TUKH/{yyyy}/",
                Padding = 4
            });
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var res = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Journal).ThenInclude(s => s.DefaultCreditAccount)
                .Include(x => x.Journal).ThenInclude(s => s.DefaultDebitAccount)
                .ToListAsync();

            foreach (var item in res)
            {
                ///ghi so
                var moves = await _PreparePartnerAdvanceMovesAsync(res);

                var amlObj = GetService<IAccountMoveLineService>();
                foreach (var move in moves)
                    amlObj.PrepareLines(move.Lines);

                await moveObj.CreateMoves(moves);
                await moveObj.ActionPost(moves);

                foreach (var move in moves)
                    amlObj.ComputeMoveNameState(move.Lines);

                item.State = "confirmed";
            }

            await UpdateAsync(res);
        }

        private async Task<IList<AccountMove>> _PreparePartnerAdvanceMovesAsync(IList<PartnerAdvance> vals)
        {
            var accKHTU = await getAccountKHTU();
            var all_move_vals = new List<AccountMove>();
            foreach (var partnerAdvance in vals)
            {
                AccountAccount liquidity_line_account;
                decimal counterpart_amount;
                if (partnerAdvance.Type == "advance")
                {
                    counterpart_amount = -partnerAdvance.Amount;
                    liquidity_line_account = partnerAdvance.Journal.DefaultCreditAccount;
                }
                else
                {
                    counterpart_amount = partnerAdvance.Amount;
                    liquidity_line_account = partnerAdvance.Journal.DefaultDebitAccount;
                }

                var rec_pay_line_name = "";
                if (partnerAdvance.Type == "advance")
                    rec_pay_line_name = "Khách hàng đóng - tạm ứng";
                else if (partnerAdvance.Type == "refund")
                    rec_pay_line_name = "Khách hàng hoàn - tạm ứng";

                var balance = counterpart_amount;
                var liquidity_line_name = partnerAdvance.Name;

                var move_vals = new AccountMove
                {
                    Date = partnerAdvance.Date,
                    Ref = partnerAdvance.Note,
                    JournalId = partnerAdvance.JournalId.Value,
                    Journal = partnerAdvance.Journal,
                    CompanyId = partnerAdvance.CompanyId,
                    PartnerId = partnerAdvance.PartnerId,
                };

                var lines = new List<AccountMoveLine>()
                {
                    new AccountMoveLine
                    {
                        Name = !string.IsNullOrEmpty(rec_pay_line_name) ? rec_pay_line_name : "/",
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        DateMaturity = partnerAdvance.Date,
                        AccountId = accKHTU.Id,
                        Account = accKHTU,
                        Move = move_vals,
                        PartnerId = partnerAdvance.PartnerId,
                    },
                    new AccountMoveLine
                    {
                        Name = liquidity_line_name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        DateMaturity = partnerAdvance.Date,
                        AccountId = liquidity_line_account.Id,
                        Account = liquidity_line_account,
                        Move = move_vals,
                        PartnerId = partnerAdvance.PartnerId,
                    },
                };

                move_vals.Lines = lines;
                all_move_vals.Add(move_vals);
            }

            return all_move_vals;
        }

        public async Task<AccountAccount> getAccountKHTU()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountJournalObj = GetService<IAccountJournalService>();

            var currentLiabilities = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
            var accKHTU = new AccountAccount();
            accKHTU = await accountObj.SearchQuery(x => x.Code == "KHTU" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (accKHTU == null)
            {
                accKHTU = new AccountAccount
                {
                    Name = "Khách hàng tạm ứng",
                    Code = "KHTU",
                    InternalType = currentLiabilities.Type,
                    UserTypeId = currentLiabilities.Id,
                    CompanyId = CompanyId,
                };

                await accountObj.CreateAsync(accKHTU);
            }

            return accKHTU;

        }

        public async Task<PartnerAdvancePrint> GetPartnerAdvancePrint(Guid id)
        {
            var userObj = GetService<IUserService>();

            var partnerAdvance = await SearchQuery(x => x.Id == id).Select(x => new PartnerAdvancePrint
            {
                Id = x.Id,
                Name = x.Name,
                Date = x.Date,
                Company = x.Company != null ? new CompanyPrintVM
                {
                    Name = x.Company.Name,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Logo = x.Company.Logo,
                    PartnerCityName = x.Company.Partner.CityName,
                    PartnerDistrictName = x.Company.Partner.DistrictName,
                    PartnerWardName = x.Company.Partner.WardName,
                    PartnerStreet = x.Company.Partner.Street,
                } : null,
                Amount = x.Amount,
                PartnerName = x.Partner != null ? x.Partner.Name : null,
                JournalName = x.Journal != null ? x.Journal.Name : null,
                Note = x.Note,
                Type = x.Type,
                CreatedById = x.CreatedById
            }).FirstOrDefaultAsync();

            var user = await userObj.GetByIdAsync(partnerAdvance.CreatedById);
            partnerAdvance.UserName = user.Name;
            partnerAdvance.AmountText = AmountToText.amount_to_text(partnerAdvance.Amount);

            return partnerAdvance;
        }

        public override ISpecification<PartnerAdvance> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "partner.partner_advance_comp_rule":
                    return new InitialSpecification<PartnerAdvance>(x => companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
