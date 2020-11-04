using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToaThuocsController : BaseApiController
    {
        private readonly IToaThuocService _toaThuocService;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly ViewRender view;
        public ToaThuocsController(IToaThuocService toaThuocService, IMapper mapper, ViewRender view,
            IIRModelAccessService modelAccessService)
        {
            _toaThuocService = toaThuocService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
            this.view = view;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.ToaThuoc.Read")]
        public async Task<IActionResult> Get([FromQuery]ToaThuocPaged val)
        {
            var result = await _toaThuocService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.ToaThuoc.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var toaThuoc = await _toaThuocService.GetToaThuocForDisplayAsync(id);
            if (toaThuoc == null)
                return NotFound();

            return Ok(toaThuoc);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.ToaThuoc.Create")]
        public async Task<IActionResult> Create(ToaThuocSave val)
        {
            var order = _mapper.Map<ToaThuoc>(val);
            SaveOrderLines(val, order);
            await _toaThuocService.CreateAsync(order);

            var basic = _mapper.Map<ToaThuocBasic>(order);
            return Ok(basic);
        }



        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.ToaThuoc.Update")]
        public async Task<IActionResult> Update(Guid id, ToaThuocSave val)
        {
            var toathuoc = await _toaThuocService.SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            if (toathuoc == null)
                return NotFound();

            toathuoc = _mapper.Map(val, toathuoc);

            SaveOrderLines(val, toathuoc);

            await _toaThuocService.Write(toathuoc);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.ToaThuoc.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var toaThuoc = await _toaThuocService.GetByIdAsync(id);
            if (toaThuoc == null)
                return NotFound();
            await _toaThuocService.DeleteAsync(toaThuoc);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(ToaThuocDefaultGet val)
        {
            var res = await _toaThuocService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("LineDefaultGet")]
        public async Task<IActionResult> LineDefaultGet(ToaThuocLineDefaultGet val)
        {
            var res = await _toaThuocService.LineDefaultGet(val);
            return Ok(res);
        }

        [HttpGet("{id}/Print")]
        [CheckAccess(Actions = "Basic.ToaThuoc.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _toaThuocService.GetToaThuocPrint(id);

            var html = this.view.Render("ToathuocPrint", res);

            return Ok(new printData() { html = html });
        }

        private void SaveOrderLines(ToaThuocSave val, ToaThuoc order)
        {
            var lineToRemoves = new List<ToaThuocLine>();

            foreach (var existLine in order.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.Lines.Remove(line);
            }

            int sequence = 1;

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var l = _mapper.Map<ToaThuocLine>(line);
                    l.Sequence = sequence;
                    order.Lines.Add(l);
                }
                else
                {
                    var l = order.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                    l.Sequence = sequence;
                }

                sequence++;
            }
        }
    }
}