using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class AccountPaymentsController : BaseApiController
    {
        private readonly IAccountPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public AccountPaymentsController(IAccountPaymentService paymentService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AccountPaymentPaged val)
        {
            var result = await _paymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var payment = await _paymentService.SearchQuery(x => x.Id == id).Include(x => x.Partner).Include(x => x.Journal)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<AccountPaymentDisplay>(payment);
            return Ok(res);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AccountPaymentSave val)
        {
            var payment = await _paymentService.CreateUI(val);
            var basic = _mapper.Map<AccountPaymentBasic>(payment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Post(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.Post(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.CancelAsync(ids);
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.ActionDraftUnlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaleDefaultGet(IEnumerable<Guid> ids)
        {
            var res = await _paymentService.OrderDefaultGet(ids);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ServiceCardOrderDefaultGet(IEnumerable<Guid> ids)
        {
            var res = await _paymentService.ServiceCardOrderDefaultGet(ids);
            return Ok(res);
        }

        [HttpGet("GetPaymentBasicList")]
        public async Task<IActionResult> GetPaymentBasicList([FromQuery]AccountPaymentFilter val)
        {
            var res = await _paymentService.GetPaymentBasicList(val);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _paymentService.GetPrint(id);
            return Ok(res);
        }
    }
}