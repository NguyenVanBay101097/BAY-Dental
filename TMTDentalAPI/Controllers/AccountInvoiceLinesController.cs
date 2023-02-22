using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInvoiceLinesController : BaseApiController
    {
        private readonly IAccountInvoiceLineService _invLineService;
        public AccountInvoiceLinesController(IAccountInvoiceLineService invLineService)
        {
            _invLineService = invLineService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AccountInvoiceLinesPaged val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            var res = await _invLineService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(AccountInvoiceLineDefaultGet val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            var res = await _invLineService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("OnChangeProduct")]
        public async Task<IActionResult> OnChangeProduct(AccountInvoiceLineOnChangeProduct val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();
            var res = await _invLineService.OnChangeProduct(val);
            return Ok(res);
        }

        [HttpGet("GetDotKhamInvoiceLine/{id}")]
        public async Task<IActionResult> GetDotKhamInvoiceLine(Guid id)
        {
            var res = await _invLineService.GetDotKhamInvoiceLine(id);
            return Ok(res);
        }
    }
}