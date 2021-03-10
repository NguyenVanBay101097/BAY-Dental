using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SurveyCallContentsController : BaseApiController
    {
        private readonly ISurveyCallContentService _surveyCallContentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public SurveyCallContentsController(ISurveyCallContentService surveyCallContentService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _surveyCallContentService = surveyCallContentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Survey.CallContent.Read")]
        public async Task<IActionResult> Get([FromQuery] SurveyCallContentPaged val)
        {
            var result = await _surveyCallContentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Survey.CallContent.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _surveyCallContentService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Survey.CallContent.Create")]
        public async Task<IActionResult> Create(SurveyCallContentSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var callcontent = await _surveyCallContentService.CreateSurveyCallContent(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<SurveyCallContentBasic>(callcontent);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Survey.CallContent.Update")]
        public async Task<IActionResult> Update(Guid id, SurveyCallContentSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _surveyCallContentService.UpdateSurveyCallContent(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Survey.CallContent.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var callcontent = await _surveyCallContentService.GetByIdAsync(id);
            if (callcontent == null)
                return NotFound();

            await _surveyCallContentService.DeleteAsync(callcontent);

            return NoContent();
        }
    }
}
