using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintPaperSizesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPrintPaperSizeService _printPaperSizeService;

        public PrintPaperSizesController(IPrintPaperSizeService printPaperSizeService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _printPaperSizeService = printPaperSizeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PrintPaperSizePaged val)
        {
            var result = await _printPaperSizeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _printPaperSizeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(PrintPaperSizeSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            var paperSize = await _printPaperSizeService.CreatePrintPaperSize(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<PrintPaperSizeBasic>(paperSize);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PrintPaperSizeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _printPaperSizeService.UpdatePrintPaperSize(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _printPaperSizeService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            await _printPaperSizeService.DeleteAsync(result);

            return NoContent();
        }
    }
}
