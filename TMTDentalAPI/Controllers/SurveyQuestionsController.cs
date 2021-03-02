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
using TMTDentalAPI.JobFilters;
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
        [CheckAccess(Actions = "Survey.Question.Read")]
        public async Task<IActionResult> Get([FromQuery] SurveyQuestionPaged val)
        {
            var result = await _surveyQuestionService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Survey.Question.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var question = await _surveyQuestionService.SearchQuery(x => x.Id == id).Include(x => x.Answers).FirstOrDefaultAsync();
            if (question == null)
                return NotFound();
            question.Answers.OrderBy(x => x.Sequence);
            return Ok(_mapper.Map<SurveyQuestionDisplay>(question));
        }

        [HttpPost]
        [CheckAccess(Actions = "Survey.Question.Create")]
        public async Task<IActionResult> Create(SurveyQuestionSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var question = _mapper.Map<SurveyQuestion>(val);
            SaveAnswers(val, question);

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

            var sequence = 0;
            foreach (var ans in val.Answers)
            {
                if (ans.Id == Guid.Empty)
                {
                    var item = _mapper.Map<SurveyAnswer>(ans);
                    item.Sequence = sequence++;
                    question.Answers.Add(item);
                }
                else
                {
                    var ansQ = question.Answers.SingleOrDefault(c => c.Id == ans.Id);
                    _mapper.Map(ans, ansQ);
                    ansQ.Sequence = sequence++;
                }

            }
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Survey.Question.Update")]
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Question.Update")]
        public async Task<IActionResult> Resequence(SurveyQuestionResequenceVM val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var questions = await _surveyQuestionService.SearchQuery(x => val.Ids.Contains(x.Id)).ToListAsync();
            var questionDict = questions.ToDictionary(x => x.Id, x => x);
            var i = 0;
            foreach (var id in val.Ids)
            {
                var question = questionDict[id];
                question.Sequence = i + val.Offset;
                i++;
            }

            await _surveyQuestionService.UpdateAsync(questions);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Survey.Question.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var question = await _surveyQuestionService.GetByIdAsync(id);

            if (question == null)
                return NotFound();
            await _unitOfWork.BeginTransactionAsync();
            await _surveyQuestionService.DeleteAsync(question);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        [CheckAccess(Actions = "Survey.Question.Create")]
        public async Task<IActionResult> Duplicate(Guid id)
        {
            var question = await _surveyQuestionService.SearchQuery(x => x.Id == id)
                .Include(x => x.Answers).FirstOrDefaultAsync();

            if (question == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            var newQuestion = new SurveyQuestion()
            {
                Name = question.Name,
                Type = question.Type,
                Sequence = question.Sequence
            };

            foreach (var item in question.Answers)
            {
                newQuestion.Answers.Add(new SurveyAnswer()
                {
                    Score = item.Score,
                    Sequence = item.Sequence,
                    Name = item.Name
                });
            }

            await _surveyQuestionService.CreateAsync(newQuestion);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Question.Update")]
        public async Task<IActionResult> Swap(SwapPar val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var from = await _surveyQuestionService.GetByIdAsync(val.IdFrom);
            var to = await _surveyQuestionService.GetByIdAsync(val.IdTo);
            if (from == null || to == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            var temp = from.Sequence;
            from.Sequence = to.Sequence;
            to.Sequence = temp;
            await _surveyQuestionService.UpdateAsync(new List<SurveyQuestion>() { from, to });
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Survey.Question.Read")]
        public async Task<IActionResult> GetListForSurvey(Guid id)
        {
            var questions = await _surveyQuestionService.SearchQuery()
                .OrderBy(x => x.Sequence)
                .Include(x => x.Answers).ToListAsync();

            foreach(var question in questions)
                question.Answers.OrderBy(x => x.Sequence);

            return Ok(_mapper.Map<IEnumerable<SurveyQuestionDisplay>>(questions));
        }
    }
}
