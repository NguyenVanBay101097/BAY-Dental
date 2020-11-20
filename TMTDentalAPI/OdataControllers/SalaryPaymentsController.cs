using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{

    public class SalaryPaymentsController : BaseController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly ConnectionStrings _connectionStrings;
        private readonly AppTenant _tenant;
        private readonly ISalaryPaymentService _salaryPaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SalaryPaymentsController(IPartnerService partnerService,
            IMapper mapper,
            IOptions<ConnectionStrings> connectionStrings,
            ITenant<AppTenant> tenant,
            ISalaryPaymentService salaryPaymentService,
            IUnitOfWorkAsync unitOfWork
          )
        {
            _partnerService = partnerService;
            _mapper = mapper;
            _connectionStrings = connectionStrings?.Value;
            _tenant = tenant?.Value;
            _salaryPaymentService = salaryPaymentService;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<SalaryPaymentVm>(_salaryPaymentService.SearchQuery());
            return Ok(results);
        }


        [EnableQuery]
        public SingleResult<SalaryPaymentVm> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<SalaryPaymentVm>(_salaryPaymentService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }


        [HttpPost]
        public async Task<IActionResult> Post(SalaryPaymentSave model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var salaryPayment = await _salaryPaymentService.CreateSalaryPayment(model);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SalaryPaymentVm>(salaryPayment);
            return Created(basic);
        }

        [HttpPut]
        public async Task<IActionResult> PUT([FromODataUri] Guid key, SalaryPaymentSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _salaryPaymentService.UpdateSalaryPayment(key, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _salaryPaymentService.ActionConfirm(ids);
            _unitOfWork.Commit();         

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMultiSalaryPayment(IEnumerable<SalaryPaymentSave> vals)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _salaryPaymentService.CreateAndConfirmMultiSalaryPayment(vals);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _salaryPaymentService.ActionCancel(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var salaryPayment = await _salaryPaymentService.GetByIdAsync(id);
            if (salaryPayment.State == "posted")
                throw new Exception("Bạn không thể xóa phiếu khi đã ghi sổ");
         
            if (salaryPayment == null)
                return NotFound();

            await _salaryPaymentService.DeleteAsync(salaryPayment);
            return NoContent();
        }



    }
}
