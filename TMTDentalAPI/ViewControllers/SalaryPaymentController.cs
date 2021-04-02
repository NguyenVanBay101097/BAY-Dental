using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class SalaryPaymentController : Controller
    {
        private readonly ISalaryPaymentService _salaryPaymentService;
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public SalaryPaymentController(ISalaryPaymentService salaryPaymentService, IViewToStringRenderService viewToStringRenderService, IMapper mapper, IUserService userService)
        {
            _salaryPaymentService = salaryPaymentService;
            _viewToStringRenderService = viewToStringRenderService;
            _mapper = mapper;
            _userService = userService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.SalaryPaymentPaperCode)]
        public async Task<IActionResult> Print([FromBody] IEnumerable<Guid> ids)
        {
            var salaries = await _salaryPaymentService.SearchQuery(x => ids.Contains(x.Id)).ToListAsync();

            var salaryPayments = await _mapper.ProjectTo<SalaryPaymentPrintVm>(_salaryPaymentService.SearchQuery(x => ids.Contains(x.Id))).ToListAsync();
            foreach (var print in salaryPayments)
                print.AmountString = AmountToText.amount_to_text(print.Amount);      

            return View(salaryPayments);
        }
    }
}
