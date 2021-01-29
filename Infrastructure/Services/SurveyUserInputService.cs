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

            return res;
        }

        public async Task<SurveyUserInputDisplay> DefaultGet()
        {
            var questionObj = GetService<ISurveyQuestionService>();
            var questions = await questionObj.SearchQuery().Include(x => x.Answers).OrderByDescending(x => x.Sequence).ToListAsync();
            var userInputline = new List<SurveyUserInputLineDisplay>();
            var line = new SurveyUserInputLineDisplay();
            var res = new SurveyUserInputDisplay();

            foreach (var question in questions)
            {
                if (question.Type == "radio")
                {
                    var maxAnswer = question.Answers.Where(x => x.Score == question.Answers.Max(s=>s.Score)).FirstOrDefault();
                    line = new SurveyUserInputLineDisplay
                    {
                        QuestionId = question.Id,
                        Question = new SurveyQuestionDisplay { 
                        Id = question.Id,
                        Name = question.Name,
                        Sequence = question.Sequence,
                        Type = question.Type,
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

            await ComputeUserInputAsync(userInput);

            return await CreateAsync(userInput);
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
            var lineRaidos = userinput.Lines.Where(x => x.Question.Type == "radio").ToList();
            var questions = await questionObj.SearchQuery(x => lineRaidos.Select(s => s.Id).ToList().Contains(x.Id)).Include(x => x.Answers).ToListAsync();
            var maxNumber = lineRaidos.Max(x => x.Answer.Score);
            var totalNumber = lineRaidos.Sum(x => x.Answer.Score);
            var totalMax = 0M;
            foreach (var question in questions)
            {

                totalMax += question.Answers.Max(x => x.Score.Value);

            }

            userinput.MaxScore = maxNumber;
            userinput.Score = totalNumber * maxNumber / totalMax;
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

    }
}
