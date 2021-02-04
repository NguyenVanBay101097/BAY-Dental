using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SurveyUserInputService : BaseService<SurveyUserInput>, ISurveyUserInputService
    {
        readonly public IMapper _mapper;
        public SurveyUserInputService(IAsyncRepository<SurveyUserInput> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }



        public async Task<SurveyUserInputDisplay> GetDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<SurveyUserInputDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Khảo sát không tồn tại");
            res.Lines = res.Lines.Select(x => new SurveyUserInputLineDisplay
            {
                QuestionId = x.QuestionId,
                Question = new SurveyQuestionDisplay
                {
                    Id = x.Question.Id,
                    Name = x.Question.Name,
                    Sequence = x.Question.Sequence,
                    Type = x.Question.Type,
                    Answers = x.Question.Answers.Select(s => new SurveyAnswerDisplay
                    {
                        Id = s.Id,
                        Name = s.Name,
                        QuestionId = s.QuestionId,
                        Score = s.Score,
                        Sequence = s.Sequence
                    }).OrderByDescending(s => s.Score).ToList()
                },
                AnswerId = x.AnswerId.HasValue ? x.AnswerId : null,
                Answer = x.Answer != null ? new SurveyAnswerDisplay
                {
                    Id = x.Answer.Id,
                    Name = x.Answer.Name,
                    Score = x.Answer.Score,
                    Sequence = x.Answer.Sequence
                } : null,
                Score = x.Score,
                ValueText = x.ValueText
            }).ToList();

            return res;
        }

        public async Task<SurveyUserInputDisplay> DefaultGet(SurveyUserInputDefaultGet val)
        {

            if (!val.SurveyAssignmentId.HasValue)
                throw new Exception("");

            var questionObj = GetService<ISurveyQuestionService>();
            var questions = await questionObj.SearchQuery().Include(x => x.Answers).OrderBy(x => x.Sequence).ToListAsync();
            var userInputline = new List<SurveyUserInputLineDisplay>();
            var line = new SurveyUserInputLineDisplay();
            var res = new SurveyUserInputDisplay();

            foreach (var question in questions)
            {
                if (question.Type == "radio")
                {
                    var maxAnswer = question.Answers.Where(x => x.Score == question.Answers.Max(s => s.Score)).FirstOrDefault();
                    line = new SurveyUserInputLineDisplay
                    {
                        QuestionId = question.Id,
                        Question = new SurveyQuestionDisplay
                        {
                            Id = question.Id,
                            Name = question.Name,
                            Sequence = question.Sequence,
                            Type = question.Type,
                            Answers = question.Answers.Select(x => new SurveyAnswerDisplay
                            {
                                Id = x.Id,
                                Name = x.Name,
                                QuestionId = x.QuestionId,
                                Score = x.Score,
                                Sequence = x.Sequence
                            }).OrderByDescending(x => x.Score).ToList()
                        },
                        AnswerId = maxAnswer.Id,
                        Answer = new SurveyAnswerDisplay
                        {
                            Id = maxAnswer.Id,
                            Name = maxAnswer.Name,
                            Score = maxAnswer.Score,
                            Sequence = maxAnswer.Sequence
                        },
                        Score = maxAnswer.Score,
                    };
                }
                else if (question.Type == "text")
                {
                    line = new SurveyUserInputLineDisplay
                    {
                        QuestionId = question.Id,
                        Question = new SurveyQuestionDisplay
                        {
                            Id = question.Id,
                            Name = question.Name,
                            Sequence = question.Sequence,
                            Type = question.Type,
                        },
                    };
                }

                userInputline.Add(line);


            }

            res.Lines = userInputline;

            return res;
        }

        public async Task<SurveyUserInput> CreateUserInput(SurveyUserInputSave val)
        {
            var userInput = _mapper.Map<SurveyUserInput>(val);

            SaveLines(val, userInput);



            return await CreateAsync(userInput);
        }

        public override async Task<SurveyUserInput> CreateAsync(SurveyUserInput entity)
        {

            await base.CreateAsync(entity);
            await ComputeUserInputAsync(entity);
            return entity;
        }

        private void SaveLines(SurveyUserInputSave val, SurveyUserInput userinput)
        {
            //remove line
            var lineToRemoves = new List<SurveyUserInputLine>();
            foreach (var existLine in userinput.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                userinput.Lines.Remove(line);
            }

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var l = _mapper.Map<SurveyUserInputLine>(line);
                    userinput.Lines.Add(l);
                }
                else
                {
                    var l = userinput.Lines.SingleOrDefault(c => c.Id == line.Id);
                    if (l != null)
                    {
                        _mapper.Map(line, l);
                    }
                }
            }

        }

        private async Task ComputeUserInputAsync(SurveyUserInput userinput)
        {
            var questionObj = GetService<ISurveyQuestionService>();
            var rs = await SearchQuery(x => x.Id == userinput.Id).Include(x => x.Lines).Include("Lines.Answer").Include("Lines.Question").FirstOrDefaultAsync();
            var lineRaidos = userinput.Lines.Where(x => x.Question.Type == "radio").ToList();
            var questionIds = lineRaidos.Select(x => x.QuestionId).ToList();
            var questions = await questionObj.SearchQuery(x => questionIds.Contains(x.Id)).Include(x => x.Answers).ToListAsync();
            var maxNumber = lineRaidos.Max(x => x.Answer.Score.Value);
            var totalNumber = lineRaidos.Sum(x => x.Answer.Score.Value);
            var totalMax = 0M;
            foreach (var question in questions)
                totalMax += question.Answers.Max(x => x.Score.Value);



            userinput.MaxScore = 5;
            userinput.Score = Math.Round(((totalNumber * 5) / totalMax), 1);
        }

        public async Task UpdateUserInput(Guid id, SurveyUserInputSave val)
        {
            var userInput = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            if (userInput == null)
                throw new Exception("Khảo sát không tồn tại");

            userInput = _mapper.Map(val, userInput);

            await ComputeUserInputAsync(userInput);

            await UpdateAsync(userInput);
        }

        public async Task Unlink(Guid id)
        {
            var userInput = SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefault();
            if (userInput == null)
                throw new Exception("Khảo sát không tồn tại");

            await DeleteAsync(userInput);
        }

    }
}
