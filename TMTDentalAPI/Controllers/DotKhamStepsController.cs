using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotKhamStepsController : BaseApiController
    {
        private readonly IDotKhamStepService _dotKhamStepService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMapper _mapper;
        public DotKhamStepsController(IDotKhamStepService dotKhamStepService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _dotKhamStepService = dotKhamStepService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<DotKhamStep> stepPatch)
        {
            var step = await _dotKhamStepService.GetByIdAsync(id);
            if (step == null)
                return NotFound();

            stepPatch.ApplyTo(step);
            await _dotKhamStepService.UpdateAsync(step);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var step = await _dotKhamStepService.GetByIdAsync(id);
            if (step == null)
                return NotFound();
            //var query = await _dotKhamStepService.SearchQuery(x => x.ProductId==step.ProductId && x.InvoicesId ==step.InvoicesId && x.Order > step.Order).ToListAsync();
            //if (query.Count > 0)
            //{
            //    foreach (var item in query)
            //    {
            //        item.Order = item.Order - 1;
            //        await _dotKhamStepService.UpdateAsync(item);
            //    }
            //}
            if (step.IsDone)
                throw new Exception("Không thể xóa công đoạn đã hoàn thành");
            await _dotKhamStepService.DeleteAsync(step);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CloneInsert(DotKhamStepCloneInsert val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var step = await _dotKhamStepService.CloneInsert(val);
            _unitOfWork.Commit();

            var display = await _dotKhamStepService.GetDisplay(step.Id);
            return Ok(display);
        }
    }
}