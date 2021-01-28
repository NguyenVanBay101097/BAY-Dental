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
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyQuestionsController : BaseApiController
    {
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SurveyQuestionsController(
            ISurveyQuestionService surveyQuestionService,
            IMapper mapper,
            IUnitOfWorkAsync unitOfWork
            )
        {
            _surveyQuestionService = surveyQuestionService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SurveyQuestionPaged val)
        {
            var result = await _surveyQuestionService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var question = await _surveyQuestionService.SearchQuery(x=> x.Id == id).Include(x=> x.Answers.OrderBy(x=>x.Sequence)).FirstOrDefaultAsync();
            if (question == null)
                return NotFound();
            return Ok(_mapper.Map<SurveyQuestionDisplay>(question));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SurveyQuestionSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var question = _mapper.Map<SurveyQuestion>(val);
            SaveAnswers(val, question);
            var maxSequence = await _surveyQuestionService.SearchQuery().MaxAsync(x => x.Sequence);
            question.Sequence = maxSequence == null ? 1 : maxSequence + 1;
            await _surveyQuestionService.CreateAsync(question);

            _unitOfWork.Commit();
            return Ok(_mapper.Map<SurveyQuestionDisplay>(question));
        }

        private void SaveAnswers(SurveyQuestionSave val, SurveyQuestion question)
        {
            var toRemove = new List<SurveyAnswer>();
            toRemove = question.Answers.Where(x => !val.Answers.Any(y => y.Id == x.Id)).ToList();

            foreach (var item in toRemove)
            {
                question.Answers.Remove(item);
            }

            foreach (var ans in val.Answers)
            {
                if (ans.Id == Guid.Empty)
                {
                    var item = _mapper.Map<SurveyAnswer>(ans);
                    question.Answers.Add(item);
                }
                else
                {
                    var ansQ = question.Answers.SingleOrDefault(c => c.Id == ans.Id);
                    _mapper.Map(ans, ansQ);
                }

            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SurveyQuestionSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var question = await _surveyQuestionService.SearchQuery(x => x.Id == id).Include(x => x.Answers).FirstOrDefaultAsync();
            if (question == null)
                return NotFound();
            question = _mapper.Map(val, question);
            SaveAnswers(val, question);
            await _surveyQuestionService.UpdateAsync(question);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var question = await _surveyQuestionService.GetByIdAsync(id);

            if (question == null)
                return NotFound();
          
            await _surveyQuestionService.DeleteAsync(question);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> Duplicate(Guid id)
        {
            var question = await _surveyQuestionService.SearchQuery(x => x.Id == id).Include(x => x.Answers).FirstOrDefaultAsync();

            if (question == null)
                return NotFound();

             await _unitOfWork.BeginTransactionAsync();
            var maxSequence = await _surveyQuestionService.SearchQuery().MaxAsync(x => x.Sequence);
            var newQuestion = new SurveyQuestion()
            {
                Name = question.Name, 
                Type = question.Type,
                Sequence = maxSequence + 1,
            };
            foreach (var item in question.Answers)
            {
                newQuestion.Answers.Add(new SurveyAnswer() { 
                Score = item.Score,
                Sequence = item.Sequence
                });
            }
            await _surveyQuestionService.CreateAsync(newQuestion);
            _unitOfWork.Commit();
            return NoContent();
        }

    }
}
