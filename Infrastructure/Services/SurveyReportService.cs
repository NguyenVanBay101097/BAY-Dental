using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SurveyReportService : ISurveyReportService
    {
        private readonly ISurveyAssignmentService _surveyAssignmentService;
        private readonly ISurveyUserInputService _surveyUserInputService;
        private readonly IMapper _mapper;
        public SurveyReportService(ISurveyAssignmentService surveyAssignmentService,
            ISurveyUserInputService surveyUserInputService,
            IMapper mapper
            )
        {
            _surveyAssignmentService = surveyAssignmentService;
            _surveyUserInputService = surveyUserInputService;
            _mapper = mapper;
        }
        private IQueryable<SurveyAssignment> GetReportAssignmentQuery(GetReportAssignmentQueryRequest val)
        {
            var query = _surveyAssignmentService.SearchQuery();
            if (!string.IsNullOrEmpty(val.Status))
            {
                var statusArr = val.Status.Split(',');
                query = query.Where(x => statusArr.Contains(x.Status));
            }
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.AssignDate.Value >= val.DateFrom.Value.AbsoluteBeginOfDate());
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.AssignDate.Value < val.DateTo.Value.AbsoluteEndOfDate());
            return query;
        }
        public async Task<IEnumerable<ReportNumberOfAssigmentByEmployeeResponse>> ReportNumberOfAssigmentByEmployee(ReportNumberOfAssigmentByEmployeeRequest val)
        {
            var query = GetReportAssignmentQuery(val);
            var res = await query.GroupBy(x => new { x.EmployeeId, x.Employee.Name })
                            .Select(x => new ReportNumberOfAssigmentByEmployeeResponse
                            {
                                EmployeeId = x.Key.EmployeeId,
                                EmployeeName = x.Key.Name,
                                DoneCount = x.Sum(z => z.Status == "done" ? 1 : 0),
                                ContactCount = x.Sum(z => z.Status == "contact" ? 1 : 0)
                            }).ToListAsync();
            return res;
        }

        public async Task<ReportRatingScroreRateOfUserInputResponse> ReportRatingScroreRate(ReportRatingScroreRateOfUserInputRequest val)
        {
            var getReportAssignmentQuery = GetReportAssignmentQuery(val);
            getReportAssignmentQuery = getReportAssignmentQuery.Where(x => x.UserInput.Score.HasValue && x.UserInput.Score > 0);
            var resLines = await getReportAssignmentQuery.GroupBy(x => Math.Floor(x.UserInput.Score.Value)).OrderBy(x => x.Key).Select(x => new
            {
                Score = x.Key,
                Count = x.Count(),
            }).ToListAsync();

            var averageScore = await getReportAssignmentQuery.AverageAsync(x => x.UserInput.Score);
            var maxScore = await getReportAssignmentQuery.MaxAsync(x => x.UserInput.MaxScore);

            return new ReportRatingScroreRateOfUserInputResponse()
            {
                Lines = resLines.Select(x => new ReportRatingScroreRateOfUserInputResponseItem
                {
                    Score = x.Score,
                    Value = x.Count
                }),
                Score = averageScore,
                MaxScore = maxScore
            };
        }

        public async Task<ReportSatifyScoreRatingByQuestionResponse> ReportSatifyScoreRatingByQuestion(ReportSatifyScoreRatingByQuestionRequest val)
        {
            var query = GetReportAssignmentQuery(val).SelectMany(x => x.UserInput.Lines).Where(x => x.Question.Type == "radio");

            var res = new ReportSatifyScoreRatingByQuestionResponse();
            var listQuestions = await query.GroupBy(x => new { x.QuestionId, x.Question.Name }).Select(x => x.Key).ToListAsync();

            res.QuestionNames = listQuestions.Select(x => x.Name);

            var listCountByScoreQuestion = await query.OrderBy(x => x.Score).GroupBy(x => new { QuestionId = x.QuestionId, Score = x.Score.Value }).Select(x => new
            {
                QuestionId = x.Key.QuestionId,
                Score = x.Key.Score,
                Count = x.Count()
            }).ToListAsync();

            res.Data = listCountByScoreQuestion.GroupBy(x => x.Score).Select(x => new ReportSatifyScoreRatingByQuestionResponseItem()
            {
                Score = x.Key,
                Data = listQuestions.Select(z => x.Where(i => i.QuestionId == z.QuestionId).Sum(o => o.Count))
            });

            return res;
        }

    }
}
