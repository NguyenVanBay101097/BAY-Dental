using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMovesController : BaseApiController
    {

        [HttpPost]
        public async Task<IActionResult> Create(AccountInvoiceDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("AccountInvoice", "Create");
            var inv = _mapper.Map<AccountInvoice>(val);
            SaveInvoiceLines(val, inv);
            await _accountInvoiceService.CreateAsync(inv);

            val.Id = inv.Id;
            return Ok(val);
        }
    }
}