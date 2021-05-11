using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotKhamLinesController : BaseApiController
    {
        public readonly IDotKhamLineService _LineService;
        public readonly IUnitOfWorkAsync _unitOfWork;
        public DotKhamLinesController(IDotKhamLineService dotKhamLineService, IUnitOfWorkAsync unitOfWork)
        {
            _LineService = dotKhamLineService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DotKhamLinePaged val)
        {
            var res = await _LineService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _LineService.GetByIdAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var res = await _LineService.GetByIdAsync(id);
            if (res == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            await _LineService.DeleteAsync(res);
            _unitOfWork.Commit();

            return NoContent();
        }


    }
}
