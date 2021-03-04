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

        public async Task CreateUserInput(SurveyUserInputCreate val)
        {
            //tao survey user input
            var questionObj = GetService<ISurveyQuestionService>();
            var questionIds = val.Questions.Select(x => x.QuestionId).ToList();
            var questions = await questionObj.SearchQuery(x => questionIds.Contains(x.Id))
                .Include(x => x.Answers)
                .ToListAsync();

            var questionDict = questions.ToDictionary(x => x.Id, x => x);

            var userInputline = new SurveyUserInputLine();
            var userInputlines = new List<SurveyUserInputLine>();
            var userinput = new SurveyUserInput();

            foreach (var line in val.Questions)
            {
                if (!questionDict.ContainsKey(line.QuestionId))
                    continue;

                var question = questionDict[line.QuestionId];
                if (question.Type == "radio")
                {
                    userInputline = new SurveyUserInputLine
                    {
                        QuestionId = question.Id,
                        AnswerId = Guid.Parse(line.AnswerValue),
                        Score = question.Answers.Where(x => x.Id == Guid.Parse(line.AnswerValue)).FirstOrDefault().Score
                    };
                }
                else if (question.Type == "text")
                {
                    userInputline = new SurveyUserInputLine
                    {
                        QuestionId = question.Id,
                        ValueText = line.AnswerValue
                    };
                }

                userInputlines.Add(userInputline);
            }

            if (val.SurveyTagIds != null)
            {
                foreach (var hist in val.SurveyTagIds)
                {
                    if (userinput.SurveyUserInputSurveyTagRels.Any(x => x.SurveyTagId == hist))
                        continue;

                    userinput.SurveyUserInputSurveyTagRels.Add(new SurveyUserInputSurveyTagRel
                    {
                        SurveyTagId = hist
                    });

                }
            }

            userinput.Lines = userInputlines;
            userinput.Note = val.Note;

            await CreateAsync(userinput);
            await ComputeUserInputAsync(userinput);

            //update UserInputId cho assignment va status
            var assignmentObj = GetService<ISurveyAssignmentService>();
            var assignment = await assignmentObj.SearchQuery(x => x.Id == val.AssignmentId).FirstOrDefaultAsync();

            if (assignment == null) 
                throw new Exception("Không tìm thấy Khảo sát!");

            var now = DateTime.Now;

            assignment.UserInputId = userinput.Id;
            assignment.Status = "done";
            assignment.CompleteDate = now;

            await assignmentObj.UpdateAsync(assignment);
        }

        private void SaveSurveyTags(SurveyUserInputSave val, SurveyUserInput userinput)
        {
            var toRemove = userinput.SurveyUserInputSurveyTagRels.Where(x => !val.SurveyTags.Any(s => s.Id == x.SurveyTagId)).ToList();
            foreach (var hist in toRemove)
            {
                userinput.SurveyUserInputSurveyTagRels.Remove(hist);
            }
            if (val.SurveyTags != null)
            {
                foreach (var hist in val.SurveyTags)
                {
                    if (userinput.SurveyUserInputSurveyTagRels.Any(x => x.SurveyTagId == hist.Id))
                        continue;

                    userinput.SurveyUserInputSurveyTagRels.Add(new SurveyUserInputSurveyTagRel
                    {
                        SurveyTagId = hist.Id
                    });

                }
            }

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
            userinput = await SearchQuery(x => x.Id == userinput.Id)
                .Include(x => x.Lines).ThenInclude(x => x.Answer)
                .Include(x => x.Lines).ThenInclude(x => x.Question).ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync();

            var totalScore = userinput.Lines.Where(x => x.Answer != null).Sum(x => x.Score ?? 0);
            var maxScore = userinput.Lines.Where(x => x.Answer != null).Sum(x => x.Question.Answers.Max(s => (s.Score ?? 0)));
            var numberScoreQuestions = userinput.Lines.Where(x => x.Answer != null).Count();

            userinput.MaxScore = numberScoreQuestions != 0 ? maxScore / numberScoreQuestions : 0;
            userinput.Score = maxScore != 0 ? Math.Round(totalScore / maxScore * (userinput.MaxScore ?? 0), 1) : 0;
            await UpdateAsync(userinput);
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
