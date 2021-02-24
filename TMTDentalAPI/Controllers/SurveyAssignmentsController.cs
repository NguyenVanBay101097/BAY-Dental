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
        private readonly ISurveyAssignmentService _SurveyAssignmentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SurveyAssignmentsController(
            ISurveyAssignmentService SurveyAssignmentService,
            IMapper mapper,
            IUnitOfWorkAsync unitOfWork
            )
        {
            _SurveyAssignmentService = SurveyAssignmentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions="Survey.Assignment.Read")]
        public async Task<IActionResult> Get([FromQuery] SurveyAssignmentPaged val)
        {
            var result = await _SurveyAssignmentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _SurveyAssignmentService.GetDisplay(id);
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
            await _SurveyAssignmentService.CreateAsync(assignment);

            _unitOfWork.Commit();
            return Ok(assignment);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Create")]
        public async Task<IActionResult> CreateList(IEnumerable<SurveyAssignmentSave> vals)
        {
            if (vals.Count() == 0 || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assignments = _mapper.Map<IEnumerable<SurveyAssignment>>(vals);
            await _SurveyAssignmentService.CreateAsync(assignments);

            _unitOfWork.Commit();
            return Ok(assignments);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.UpdateEmployee")]
        public async Task<IActionResult> Update(Guid id, SurveyAssignmentSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var question = await _SurveyAssignmentService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (question == null)
                return NotFound();
            question = _mapper.Map(val, question);
            await _SurveyAssignmentService.UpdateAsync(question);

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
            await _SurveyAssignmentService.ActionContact(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _SurveyAssignmentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Done")]
        public async Task<IActionResult> ActionDone(AssignmentActionDone val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _SurveyAssignmentService.ActionDone(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> JsonPatchWithModelState(Guid id, [FromBody]JsonPatchDocument<SurveyAssignmentPatch> patchDoc)
        {
            var entity = await _SurveyAssignmentService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var entityMap = _mapper.Map<SurveyAssignmentPatch>(entity);
            patchDoc.ApplyTo(entityMap, ModelState);
            _mapper.Map(entityMap, entity);
            await _unitOfWork.BeginTransactionAsync();
            await _SurveyAssignmentService.UpdateAsync(entity);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var question = await _SurveyAssignmentService.GetByIdAsync(id);

            if (question == null)
                return NotFound();

            await _SurveyAssignmentService.DeleteAsync(question);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> DefaultGetList(SurveyAssignmentDefaultGetPar val)
        {
            var result = await _SurveyAssignmentService.DefaultGetList(val);
            return Ok(result);
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> GetSummary(SurveyAssignmentGetCountVM val)
        {
            var result = await _SurveyAssignmentService.GetCount(val);
            return Ok(result);
        }

    }
}
