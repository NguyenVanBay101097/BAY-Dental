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
    public class SaleOrderPaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPaymentService _saleOrderPaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleOrderPaymentsController(IMapper mapper, ISaleOrderPaymentService saleOrderPaymentService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _saleOrderPaymentService = saleOrderPaymentService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaymentPaged val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var saleOrderPayment = _saleOrderPaymentService.GetDisplay(id);

            _unitOfWork.Commit();
         
            return Ok(saleOrderPayment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleOrderPaymentSave val)
        {
            var saleOrderPayment = await _saleOrderPaymentService.CreateSaleOrderPayment(val);
            var basic = _mapper.Map<SaleOrderPaymentBasic>(saleOrderPayment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionPayment(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionPayment(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
