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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyTagsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISurveyTagService _surveyTagService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SurveyTagsController(IMapper mapper, ISurveyTagService surveyTagService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _surveyTagService = surveyTagService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.SurveyTag.Read")]
        public async Task<IActionResult> Get([FromQuery] SurveyTagPaged val)
        {
            var result = await _surveyTagService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.SurveyTag.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _surveyTagService.GetByIdAsync(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.SurveyTag.Create")]
        public async Task<IActionResult> Create(SurveyTagSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var surveyTag = _mapper.Map<SurveyTag>(val);
            await _unitOfWork.BeginTransactionAsync();
            surveyTag = await _surveyTagService.CreateAsync(surveyTag);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<SurveyTagBasic>(surveyTag));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.SurveyTag.Update")]
        public async Task<IActionResult> Update(Guid id, SurveyTagSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var surveyTag = await _surveyTagService.GetByIdAsync(id);
            if (surveyTag == null)
                return NotFound();

            surveyTag = _mapper.Map(val, surveyTag);
            await _unitOfWork.BeginTransactionAsync();
            await _surveyTagService.UpdateAsync(surveyTag);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.SurveyTag.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var surveyTag = await _surveyTagService.GetByIdAsync(id);
            if (surveyTag == null)
                return NotFound();

            await _surveyTagService.DeleteAsync(surveyTag);

            return NoContent();
        }
    }
}
