using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public SalaryPaymentsController(
            IMapper mapper,
            ISalaryPaymentService salaryPaymentService,
            IViewRenderService view,
            IUserService userService,
            IUnitOfWorkAsync unitOfWork,
            IAccountJournalService journalService, IPrintTemplateConfigService printTemplateConfigService,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService
          )
        {
            _mapper = mapper;
            _salaryPaymentService = salaryPaymentService;
            _view = view;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _journalService = journalService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
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
        public async Task<IActionResult> CreateMultiSalaryPayment(IEnumerable<PayslipCreateSalaryPaymentSave> vals)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var ids = await _salaryPaymentService.CreateAndConfirmMultiSalaryPayment(vals);
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

        [HttpPost("Print")]
        [CheckAccess(Actions = "Salary.SalaryPayment.Read")]
        public async Task<IActionResult> GetPrint(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();

            var res = await _salaryPaymentService.SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var type = res.GroupBy(z => z.Type).Select(x => x.Key).FirstOrDefault();
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == (type == "advance" ? "tmp_salary_advance" : "tmp_salary_employee") && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>(type == "advance" ? "base.print_template_salary_advance" : "base.print_template_salary_employee");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, ids, paperSize);

            return Ok(new PrintData() { html = result });
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
        public async Task<IActionResult> DefaulCreateBy(IEnumerable<Guid> payslipIds)
        {
            var res = await _salaryPaymentService.DefaulCreateBy(payslipIds);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet([FromQuery] string type)
        {
            var res = new SalaryPaymentDisplay();
            res.Type = type;
            var journal = await _journalService.SearchQuery(x => x.CompanyId == CompanyId && x.Type == "cash").FirstOrDefaultAsync();
            res.Journal = _mapper.Map<AccountJournalSimple>(journal);
            return Ok(res);
        }
    }
}
