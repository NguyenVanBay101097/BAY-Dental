using ApplicationCore.Constants;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class SurveyAssignmentController : Controller
    {
        private readonly ISurveyAssignmentService _surveyAssignmentService;


        public SurveyAssignmentController(ISurveyAssignmentService surveyAssignmentService)
        {
            _surveyAssignmentService = surveyAssignmentService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.DoneSurveyAssignmentReport)]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> PrintDoneSurveyAssignmentReport([FromBody] SurveyAssignmentPaged val)
        {
            var data = await _surveyAssignmentService.GetDoneSurveyAssignmentReportPrint(val);
            return View(data);
        }
    }
}
