using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
    public class AccountRegisterPaymentsController : BaseApiController
    {
        private readonly IAccountRegisterPaymentService _registerPaymentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public AccountRegisterPaymentsController(IAccountRegisterPaymentService registerPaymentService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _registerPaymentService = registerPaymentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var payment = await _registerPaymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountRegisterPaymentDisplay>(payment));
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountRegisterPaymentDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var payment = _mapper.Map<AccountRegisterPayment>(val);
            if (val.InvoiceIds.Any())
            {
                foreach(var invoiceId in val.InvoiceIds)
                {
                    payment.AccountRegisterPaymentInvoiceRels.Add(new AccountRegisterPaymentInvoiceRel
                    {
                        InvoiceId = invoiceId,
                        PaymentId = payment.Id
                    });
                }
            }

            await _registerPaymentService.CreateAsync(payment);

            val.Id = payment.Id;
            return Ok(val);
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(AccountRegisterPaymentDefaultGet val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var res = await _registerPaymentService.DefaultGet(val.InvoiceIds);
            return Ok(res);
        }

        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment(AccountRegisterPaymentCreatePayment val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _registerPaymentService.CreatePayment(val.Id);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}