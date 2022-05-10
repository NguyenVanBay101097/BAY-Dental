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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetListCanPayOrReceive(AccountAccountGetListCanPayOrReceiveRequest val)
        {
            //Lấy danh sách những tài khoản mà có thể thu hoặc chi, xuất hiện trong sỗ quỹ
            var res = await _accountAccountService.GetListCanPayOrReceive(val);
            return Ok(_mapper.Map<IEnumerable<AccountAccountBasic>>(res));
        }
    }
}