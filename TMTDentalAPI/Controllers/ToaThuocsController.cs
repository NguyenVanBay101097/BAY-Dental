using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ToaThuocsController(IToaThuocService toaThuocService, IMapper mapper,
            IIRModelAccessService modelAccessService)
        {
            _toaThuocService = toaThuocService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ToaThuocPaged val)
        {
            var result = await _toaThuocService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var toaThuoc = await _toaThuocService.GetToaThuocForDisplayAsync(id);
            if (toaThuoc == null)
                return NotFound();

            return Ok(_mapper.Map<ToaThuocDisplay>(toaThuoc));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ToaThuocSave val)
        {
            var order = _mapper.Map<ToaThuoc>(val);
            SaveOrderLines(val, order);
            await _toaThuocService.CreateAsync(order);

            var basic = _mapper.Map<ToaThuocBasic>(order);
            return Ok(basic);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ToaThuocSave val)
        {
            var order = await _toaThuocService.GetToaThuocForDisplayAsync(id);
            if (order == null)
                return NotFound();

            order = _mapper.Map(val, order);

            SaveOrderLines(val, order);

            await _toaThuocService.Write(order);

            return NoContent();
        }

        [HttpDelete("{id}")]
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
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _toaThuocService.GetToaThuocPrint(id);
            res.Lines = res.Lines.OrderBy(x => x.Sequence);
            return Ok(res);
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