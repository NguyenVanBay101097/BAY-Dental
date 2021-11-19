using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResInsurancePaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IResInsurancePaymentService _insurancePaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ResInsurancePaymentsController(IMapper mapper, IResInsurancePaymentService insurancePaymentService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _insurancePaymentService = insurancePaymentService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResInsurancePaymentSave val)
        {
            var insurancePayment = await _insurancePaymentService.CreateResInsurancePayment(val);
            var basic = _mapper.Map<ResInsurancePaymentBasic>(insurancePayment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionPayment(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _insurancePaymentService.ActionPayment(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
