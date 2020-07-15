using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountFinancialReportsController : ControllerBase
    {
        private readonly IReportFinancialService _reportFinancialService;
        public AccountFinancialReportsController(IReportFinancialService reportFinancialService)
        {
            _reportFinancialService = reportFinancialService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AccountFinancialReportSave val)
        {
            var model = new AccountFinancialReport();
            model.Name = val.Name;
            model.Type = val.Type;
            model.DisplayDetail = val.DisplayDetail;
            var res = await _reportFinancialService.CreateAsync(model);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAccountMoveLines(AccountingReport val)
        {
            var res = await _reportFinancialService.GetAccountLines(val);
            return Ok(res);
        }
    }
}