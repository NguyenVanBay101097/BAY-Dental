using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotKhamStepsController : BaseApiController
    {
        private readonly IDotKhamStepService _dotKhamStepService;
        private readonly IMapper _mapper;
        public DotKhamStepsController(IDotKhamStepService dotKhamStepService,
            IMapper mapper)
        {
            _dotKhamStepService = dotKhamStepService;
            _mapper = mapper;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<DotKhamStepSave> stepSavePatch)
        {
            var task = await _dotKhamStepService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var taskSaveDto = _mapper.Map<DotKhamStepSave>(task);
            stepSavePatch.ApplyTo(taskSaveDto);

            task = _mapper.Map(taskSaveDto, task);
            await _dotKhamStepService.UpdateAsync(task);
            return NoContent();
        }

        [HttpPut("Reorder/{index}")]
        public async Task<IActionResult> Reorder(int index, List<DotKhamStepDisplay> list)
        {
            var productId = list[0].ProductId;
            var invoiceId = list[0].InvoicesId;
            for (var i=0; i< list.Count()-1; i++)
            {
                if(list[i].Order> list[i+1].Order)
                {
                    var a = list[i].Order;
                    var b = list[i+1].Order;
                    var query = await _dotKhamStepService.SearchQuery(x=> x.InvoicesId == invoiceId && x.ProductId == productId).ToListAsync();
                    if (list[index].Order == b)
                    {
                        //Dời xuống
                        var entity = query.Where(x=>x.Order==b).FirstOrDefault();
                        for (var j =b+1;j<=a; j++)
                        {
                            var record = query.Where(x => x.Order == j).FirstOrDefault();
                            record.Order = record.Order - 1;
                            await _dotKhamStepService.UpdateAsync(record);
                        }
                        entity.Order = a;
                        await _dotKhamStepService.UpdateAsync(entity);
                    }
                    else if (list[index].Order == a)
                    {
                        //Dời lên
                        var entity = query.Where(x => x.Order == a).FirstOrDefault();
                        for (var j = a - 1; j >= b; j--)
                        {
                            var record = query.Where(x => x.Order == j).FirstOrDefault();
                            record.Order = record.Order + 1;
                            await _dotKhamStepService.UpdateAsync(record);
                        }
                        entity.Order = b;
                        await _dotKhamStepService.UpdateAsync(entity);
                    }
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DotKhamStepDisplay val)
        {
            if(val==null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var res = _dotKhamStepService.SearchQuery(x => x.InvoicesId == val.InvoicesId && x.ProductId == val.ProductId).Max(x=>x.Order);
            if (res.HasValue)
            {
                val.Order = res + 1;
            }
            else
            {
                val.Order = 1;
            }
            
            var dkstep = _mapper.Map<DotKhamStep>(val);
            await _dotKhamStepService.CreateAsync(dkstep);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var step = await _dotKhamStepService.GetByIdAsync(id);
            if (step == null)
            {
                return NotFound();
            }
            var query = await _dotKhamStepService.SearchQuery(x => x.ProductId==step.ProductId && x.InvoicesId ==step.InvoicesId && x.Order > step.Order).ToListAsync();
            if (query.Count > 0)
            {
                foreach (var item in query)
                {
                    item.Order = item.Order - 1;
                    await _dotKhamStepService.UpdateAsync(item);
                }
            }
            

            await _dotKhamStepService.DeleteAsync(step);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AssignDotKham(DotKhamStepAssignDotKhamVM val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            await _dotKhamStepService.AssignDotKham(val);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToggleIsDone(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();

            await _dotKhamStepService.ToggleIsDone(ids);
            return NoContent();
        }
    }
}