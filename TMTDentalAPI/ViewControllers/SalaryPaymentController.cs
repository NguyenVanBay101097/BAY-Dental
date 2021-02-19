using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class SalaryPaymentController : Controller
    {
        private readonly ISalaryPaymentService _salaryPaymentService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public SalaryPaymentController(ISalaryPaymentService salaryPaymentService, IMapper mapper, IUserService userService)
        {
            _salaryPaymentService = salaryPaymentService;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IActionResult> Print(IEnumerable<Guid> ids)
        {
            var salaries = await _salaryPaymentService.SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var salaryPayments = _mapper.ProjectTo<SalaryPaymentPrintVm>(_salaryPaymentService.SearchQuery(x => ids.Contains(x.Id))).ToList();
            foreach (var print in salaryPayments)
            {
                print.UserName = _userService.GetByIdAsync(print.CreatedById).Result.UserName;
                print.AmountString = AmountToText.amount_to_text(print.Amount);
            }

            return View(salaryPayments);
        }
    }
}
