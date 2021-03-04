using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyAssignmentsController : BaseApiController
    {
        private readonly ISurveyAssignmentService _surveyAssignmentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IEmployeeService _employeeService;

        public SurveyAssignmentsController(
            ISurveyAssignmentService surveyAssignmentService,
            IMapper mapper,
            IUnitOfWorkAsync unitOfWork, ISaleOrderService saleOrderService,
            IEmployeeService employeeService
            )
        {
            _surveyAssignmentService = surveyAssignmentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _saleOrderService = saleOrderService;
            _employeeService = employeeService;
        }

        [HttpGet]
        [CheckAccess(Actions="Survey.Assignment.Read")]
        public async Task<IActionResult> Get([FromQuery] SurveyAssignmentPaged val)
        {
            var result = await _surveyAssignmentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _surveyAssignmentService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Survey.Assignment.Create")]
        public async Task<IActionResult> Create(SurveyAssignmentSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assignment = _mapper.Map<SurveyAssignment>(val);
            var saleOrder = await _saleOrderService.GetByIdAsync(val.SaleOrderId);
            assignment.CompanyId = saleOrder.CompanyId;
            assignment.PartnerId = saleOrder.PartnerId;

            var employee = await _employeeService.GetByIdAsync(val.EmployeeId);
            assignment.UserId = employee.UserId;
            await _surveyAssignmentService.CreateAsync(assignment);

            _unitOfWork.Commit();
            return Ok(_mapper.Map<SurveyAssignmentGridItem>(assignment));
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Create")]
        public async Task<IActionResult> CreateList(IEnumerable<SurveyAssignmentSave> vals)
        {
            if (vals.Count() == 0 || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assignments = _mapper.Map<IEnumerable<SurveyAssignment>>(vals);
            foreach(var assignment in assignments)
            {
                var saleOrder = await _saleOrderService.GetByIdAsync(assignment.SaleOrderId);
                assignment.CompanyId = saleOrder.CompanyId;
                assignment.PartnerId = saleOrder.PartnerId;

                var employee = await _employeeService.GetByIdAsync(assignment.EmployeeId);
                assignment.UserId = employee.UserId;
            }

            await _surveyAssignmentService.CreateAsync(assignments);

            _unitOfWork.Commit();
            return Ok(vals);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> Update(Guid id, SurveyAssignmentSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assign = await _surveyAssignmentService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (assign == null)
                return NotFound();

            if (val.EmployeeId != assign.EmployeeId) assign.AssignDate = DateTime.Now;

            assign = _mapper.Map(val, assign);

            await _surveyAssignmentService.UpdateAsync(assign);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> ActionContact(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();
           
            await _unitOfWork.BeginTransactionAsync();
            await _surveyAssignmentService.ActionContact(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.DeleteUserInput")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _surveyAssignmentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        //[HttpPost("[action]")]
        //[CheckAccess(Actions = "Survey.Assignment.Done")]
        //public async Task<IActionResult> ActionDone(AssignmentActionDone val)
        //{
        //    await _unitOfWork.BeginTransactionAsync();
        //    await _surveyAssignmentService.ActionDone(val);
        //    _unitOfWork.Commit();
        //    return NoContent();
        //}

        [HttpPatch("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> JsonPatchWithModelState(Guid id, [FromBody]JsonPatchDocument<SurveyAssignmentPatch> patchDoc)
        {
            var entity = await _surveyAssignmentService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var entityMap = _mapper.Map<SurveyAssignmentPatch>(entity);
            patchDoc.ApplyTo(entityMap, ModelState);
            _mapper.Map(entityMap, entity);
            await _unitOfWork.BeginTransactionAsync();
            await _surveyAssignmentService.UpdateAsync(entity);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var assign = await _surveyAssignmentService.GetByIdAsync(id);

            if (assign == null)
                return NotFound();

            await _surveyAssignmentService.DeleteAsync(assign);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> DefaultGetList(SurveyAssignmentDefaultGetPar val)
        {
            var result = await _surveyAssignmentService.DefaultGetList(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> GetSummary(SurveyAssignmentGetSummaryFilter val)
        {
            var result = await _surveyAssignmentService.GetSummary(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> UpdateEmployee(SurveyAssignmentUpdateEmployee val)
        {
            var assignment = await _surveyAssignmentService.GetByIdAsync(val.Id);
            if (assignment == null)
                return NotFound();
            if (assignment.Status == "done")
                throw new Exception("Phân việc ở trạng thái hoàn thành không thể đổi nhân viên");

            assignment.EmployeeId = val.EmployeeId;
            var employee = await _employeeService.GetByIdAsync(assignment.EmployeeId);
            assignment.UserId = employee.UserId;

            await _surveyAssignmentService.UpdateAsync(assignment);
            return NoContent();
        }
    }
}
