using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundBooksController : ControllerBase
    {
        private readonly IVFundBookService _fundBookService;
        public FundBooksController(IVFundBookService fundBookService)
        {
            _fundBookService = fundBookService;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetMoney(VFundBookSearch val)
        {
            var res = await _fundBookService.GetMoney(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumary(VFundBookSearch val)
        {
            var res = await _fundBookService.GetSumary(val);
            return Ok(res);
        }
    }
}
