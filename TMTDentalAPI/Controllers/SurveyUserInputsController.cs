using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SurveyUserInputsController : BaseApiController
    {
        private readonly ISurveyUserInputService _surveyUserInputService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public SurveyUserInputsController(ISurveyUserInputService surveyUserInputService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _surveyUserInputService = surveyUserInputService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Survey.UserInput.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _surveyUserInputService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.UserInput.Read")]
        public async Task<IActionResult> DefaultGet(SurveyUserInputDefaultGet val)
        {
            var res = await _surveyUserInputService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Survey.UserInput.Create")]
        public async Task<IActionResult> Create(SurveyUserInputCreate val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _surveyUserInputService.CreateUserInput(val);

            _unitOfWork.Commit();

            //var basic = _mapper.Map<SurveyUserInputBasic>(userinput);
            //return Ok(basic);
            return NoContent();
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Survey.UserInput.Update")]
        public async Task<IActionResult> Update(Guid id, SurveyUserInputSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _surveyUserInputService.UpdateUserInput(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        //api lấy kết quả
        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Survey.UserInput.Read")]
        public async Task<IActionResult> GetAnswer(Guid id)
        {
            //return SurveyUserInputAnswerResult
            var surveyInput = await _surveyUserInputService.SearchQuery(x => x.Id == id)
                .Include(x => x.Lines).ThenInclude(x => x.Question)
                .Include(x => x.SurveyUserInputSurveyTagRels).ThenInclude(x => x.SurveyTag)
                .FirstOrDefaultAsync();
            if (surveyInput == null)
                return NotFound();

            var res = new SurveyUserInputAnswerResult()
            {
                Note = surveyInput.Note,
                Questions = surveyInput.Lines.Select(x => new SurveyUserInputLineCreate
                {
                    QuestionId = x.QuestionId.Value,
                    AnswerValue = x.Question.Type == "radio" ? x.AnswerId.ToString() : x.ValueText
                }),
                SurveyTags = surveyInput.SurveyUserInputSurveyTagRels.Select(x => new SurveyTagBasic { 
                    Id = x.SurveyTagId,
                    Color = x.SurveyTag.Color,
                    Name = x.SurveyTag.Name
                })
            };

            return Ok(res);
        }
    }
}
