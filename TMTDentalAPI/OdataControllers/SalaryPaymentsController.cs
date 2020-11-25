using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class SalaryPaymentsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ISalaryPaymentService _salaryPaymentService;
        private readonly IViewRenderService _view;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SalaryPaymentsController(
            IMapper mapper,
            ISalaryPaymentService salaryPaymentService,
            IViewRenderService view,
            IUserService userService,
            IUnitOfWorkAsync unitOfWork
          )
        {
            _mapper = mapper;
            _salaryPaymentService = salaryPaymentService;
            _view = view;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        [HttpGet]
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
        public async Task<IActionResult> ActionConfirm([FromBody] SalaryPaymentIds val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _salaryPaymentService.ActionConfirm(val.Ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMultiSalaryPayment([FromBody] SalaryPaymentSalary val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var ids = await _salaryPaymentService.CreateAndConfirmMultiSalaryPayment(val.MultiSalaryPayments);
            _unitOfWork.Commit();

            return Ok(ids);
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

        [HttpPost]
        public async Task<IActionResult> PrintSalaryPayment([FromBody] SalaryPaymentIds val)
        {
            var salaries = await _salaryPaymentService.SearchQuery(x => val.Ids.Contains(x.Id)).ToListAsync();
            var salaryPayments = _mapper.ProjectTo<SalaryPaymentPrintVm>(_salaryPaymentService.SearchQuery(x => val.Ids.Contains(x.Id))).ToList();
            foreach (var print in salaryPayments)
            {
                print.UserName = _userService.GetByIdAsync(print.CreatedById).Result.UserName;
                print.AmountString = StringUtils.ChuyenSo(Convert.ToInt32(print.Amount).ToString());
            }

            if (val.Ids != null && val.Ids.Any())
            {
                var html = _view.Render("SalaryPaymentPrint", salaryPayments);
                return Ok(new printData() { html = html });
            }
            else
            {
                return Ok(new printData() { html = null });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var salaryPayment = await _salaryPaymentService.GetByIdAsync(key);
            if (salaryPayment.State == "done")
                throw new Exception("Bạn không thể xóa phiếu khi đã xác nhận");

            if (salaryPayment == null)
                return NotFound();

            await _salaryPaymentService.DeleteAsync(salaryPayment);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DefaulCreateBy(SalaryPaymentDefaultGetModel val)
        {
            var res = await _salaryPaymentService.DefaulCreateBy(val);
            return Ok(res);
        }



    }
}
