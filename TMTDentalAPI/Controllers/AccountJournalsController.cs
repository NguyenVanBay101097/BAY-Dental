using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountJournalsController : BaseApiController
    {
        private readonly IAccountJournalService _accountJournalService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMapper _mapper;

        public AccountJournalsController(IAccountJournalService accountJournalService, IUnitOfWorkAsync unitOfWork,
            IMapper mapper)
        {
            _accountJournalService = accountJournalService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(AccountJournalFilter val)
        {
            var result = await _accountJournalService.GetAutocomplete(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateJournalSave(AccountJournalSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _accountJournalService.CreateJournals(new List<AccountJournalSave>() { val });
            _unitOfWork.Commit();
            return Ok(_mapper.Map<IEnumerable<AccountJournalDisplay>>(res));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetBankJournals(string search)
        {
            var query = _accountJournalService.SearchQuery(x => x.Type == "bank");
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Name.Contains(search));
            var res = await query.Select(x => new AccountJournalBankJournalDto
                {
                    Id = x.Id,
                    AccountHolderName = x.BankAccount.AccountHolderName,
                    AccountNumber = x.BankAccount.AccountNumber,
                    Active = x.Active,
                    BankBic = x.BankAccount.Bank.BIC,
                    BankBranch = x.BankAccount.Branch,
                    Name = x.Name
                }).ToListAsync();

            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetBankJournal(Guid id)
        {
            var res = await _accountJournalService.SearchQuery(x => x.Id == id)
                .Select(x => new AccountJournalGetBankJournalVM {
                    Id = x.Id,
                    AccountHolderName = x.BankAccount.AccountHolderName,
                    AccountNumber = x.BankAccount.AccountNumber,
                    Active = x.Active,
                    Bank = x.BankAccount.Bank != null ? new ResBankSimple
                    {
                        Id = x.BankAccount.Bank.Id,
                        Name = x.BankAccount.Bank.Name
                    } : null,
                    BankBranch = x.BankAccount.Branch
                }).FirstOrDefaultAsync();
          
            return Ok(res);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBankJournal(AccountJournalCreateBankJournalVM val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var journal = await _accountJournalService.CreateBankJournal(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<AccountJournalBasic>(journal));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateBankJournal(AccountJournalUpdateBankJournalVM val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _accountJournalService.UpdateBankJournal(val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, AccountJournalSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _accountJournalService.UpdateJournalSave(id,val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetBankCashJournals([FromQuery]AccountJournalFilter val)
        {
            var res = await _accountJournalService.GetBankCashJournals(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var journal = await _accountJournalService.SearchQuery(x => x.Id.Equals(id))
                .Include(x => x.BankAccount)
                .ThenInclude(x => x.Bank)
                .FirstOrDefaultAsync();
            if (journal == null)
                return BadRequest();
            var res = _mapper.Map<AccountJournalDisplay>(journal);

            return Ok(res);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ActiveChangeJournals(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                var self = await _accountJournalService.GetByIdAsync(id);
                if (self == null)
                    return BadRequest();

                self.Active = !self.Active;
                await _accountJournalService.UpdateAsync(self);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _accountJournalService.Unlink(id);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}