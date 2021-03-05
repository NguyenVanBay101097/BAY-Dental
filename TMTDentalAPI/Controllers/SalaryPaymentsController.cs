using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryPaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISalaryPaymentService _salaryPaymentService;
        private readonly IViewRenderService _view;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAccountJournalService _journalService;

        public SalaryPaymentsController(
            IMapper mapper,
            ISalaryPaymentService salaryPaymentService,
            IViewRenderService view,
            IUserService userService,
            IUnitOfWorkAsync unitOfWork,
            IAccountJournalService journalService
          )
        {
            _mapper = mapper;
            _salaryPaymentService = salaryPaymentService;
            _view = view;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _journalService = journalService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Salary.SalaryPayment.Read")]
        public async Task<IActionResult> Get([FromQuery] SalaryPaymentPaged val)
        {
            var results = await _salaryPaymentService.GetPagedResultAsync(val);
            return Ok(results);
        }


        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _mapper.ProjectTo<SalaryPaymentDisplay>(_salaryPaymentService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.SalaryPayment.Create")]
        public async Task<IActionResult> Post(SalaryPaymentSave model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var salaryPayment = await _salaryPaymentService.CreateSalaryPayment(model);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SalaryPaymentBasic>(salaryPayment);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Update")]
        public async Task<IActionResult> PUT(Guid id, SalaryPaymentSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _salaryPaymentService.UpdateSalaryPayment(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Update")]
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Create")]
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Update")]
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Read")]
        public async Task<IActionResult> GetPrint(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();

            var salaryPayments = await _mapper.ProjectTo<SalaryPaymentPrintVm>(_salaryPaymentService.SearchQuery(x => ids.Contains(x.Id))).ToListAsync();
            foreach (var print in salaryPayments)
                print.AmountString = AmountToText.amount_to_text(print.Amount);

            var html = _view.Render("SalaryPayment/Print", salaryPayments);
            return Ok(new PrintData() { html = html });
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var salaryPayment = await _salaryPaymentService.GetByIdAsync(id);
            if (salaryPayment.State == "done")
                throw new Exception("Bạn không thể xóa phiếu khi đã xác nhận");

            if (salaryPayment == null)
                return NotFound();

            await _salaryPaymentService.DeleteAsync(salaryPayment);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaulCreateBy(SalaryPaymentDefaultGetModel val)
        {
            var res = await _salaryPaymentService.DefaulCreateBy(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet([FromQuery]string type)
        {
            var res = new SalaryPaymentDisplay();
            res.Type = type;
            var journal = await _journalService.SearchQuery(x => x.CompanyId == CompanyId && x.Type == "cash").FirstOrDefaultAsync();
            res.Journal = _mapper.Map<AccountJournalSimple>(journal);
            return Ok(res);
        }
    }
}
