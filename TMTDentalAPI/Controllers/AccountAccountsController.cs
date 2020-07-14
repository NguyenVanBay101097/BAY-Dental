using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountAccountsController : BaseApiController
    {
        private readonly IAccountAccountService _accountAccountService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public AccountAccountsController(IAccountAccountService accountAccountService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _accountAccountService = accountAccountService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //api get phan trang loai thu , chi
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AccountAccountPaged val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            var res = await _accountAccountService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _accountAccountService.GetById(id);
            if (res == null)
                return NotFound();
          
            return Ok(res);
        }

        //api get default loại thu , chi

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGetAsync(AccountAccountDefault val)
        {
            var res = await _accountAccountService.DefaultGet(val);         
            return Ok(res);
        }

        //api create
        [HttpPost]
        public async Task<IActionResult> Create(AccountAccountSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var accountaccount = await _accountAccountService.CreateAccountAccount(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<AccountAccountBasic>(accountaccount);
            return Ok(basic);
        }

        //api update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, AccountAccountSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _accountAccountService.UpdateAccountAccount(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        //api xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var accountaccount = await _accountAccountService.GetByIdAsync(id);
            if (accountaccount == null)
                return NotFound();

            await _accountAccountService.DeleteAsync(accountaccount);

            return NoContent();
        }
       

       
    }
}