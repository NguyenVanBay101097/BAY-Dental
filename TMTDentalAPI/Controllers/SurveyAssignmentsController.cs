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
        public async Task<IActionResult> Get([FromQuery] SurveyAssignmentPaged val)
        {
            var result = await _SurveyAssignmentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(Guid id)
        //{
        //    var question = await _SurveyAssignmentService.SearchQuery(x=> x.Id == id).Include(x=> x.Answers.OrderBy(x=>x.Sequence)).FirstOrDefaultAsync();
        //    if (question == null)
        //        return NotFound();
        //    return Ok(_mapper.Map<SurveyAssignmentDisplay>(question));
        //}

        [HttpPost]
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
        public async Task<IActionResult> ActionContact(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
       
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPatch("{id}")]
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

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGetList()
        {
            var result = await _SurveyAssignmentService.DefaultGetList();
            return Ok(result);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetSummary([FromQuery]SurveyAssignmentPaged val)
        {
            var result = await _SurveyAssignmentService.GetSummary(val);
            return Ok(result);
        }

    }
}
