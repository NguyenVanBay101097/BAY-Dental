using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyReportController : BaseApiController
    {
        private readonly ISurveyReportService _surveyReportService;
        public SurveyReportController(ISurveyReportService surveyReportService)
        {
            _surveyReportService = surveyReportService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.UserInput.Read")]
        public async Task<IActionResult> ReportRatingScroreRate(ReportRatingScroreRateOfUserInputRequest val)
        {
            var res = await _surveyReportService.ReportRatingScroreRate(val);

            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.UserInput.Read")]
        public async Task<IActionResult> ReportNumberOfAssigmentByEmployee(ReportNumberOfAssigmentByEmployeeRequest val)
        {
            var res = await _surveyReportService.ReportNumberOfAssigmentByEmployee(val);

            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.UserInput.Read")]
        public async Task<IActionResult> ReportSatifyScoreRatingByQuestion(ReportSatifyScoreRatingByQuestionRequest val)
        {
            var res = await _surveyReportService.ReportSatifyScoreRatingByQuestion(val);

            return Ok(res);
        }
    }
}
