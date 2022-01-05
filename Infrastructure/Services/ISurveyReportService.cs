using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISurveyReportService
    {
        Task<IEnumerable<ReportNumberOfAssigmentByEmployeeResponse>> ReportNumberOfAssigmentByEmployee(ReportNumberOfAssigmentByEmployeeRequest val);
        Task<ReportRatingScroreRateOfUserInputResponse> ReportRatingScroreRate(ReportRatingScroreRateOfUserInputRequest val);
        Task<ReportSatifyScoreRatingByQuestionResponse> ReportSatifyScoreRatingByQuestion(ReportSatifyScoreRatingByQuestionRequest val);
    }
}
