using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutingsController : BaseApiController
    {
        private readonly IRoutingService _routingService;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;

        public RoutingsController(IRoutingService routingService,
            IMapper mapper, IIRModelAccessService modelAccessService)
        {
            _routingService = routingService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]RoutingPaged val)
        {
            _modelAccessService.Check("Routing", "Read");
            var result = await _routingService.GetPagedResultAsync(val);

            var paged = new PagedResult2<RoutingBasic>(result.TotalItems, val.Offset, val.Limit)
            {
                //Có thể dùng thư viện automapper
                Items = _mapper.Map<IEnumerable<RoutingBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _modelAccessService.Check("Routing", "Read");
            var routing = await _routingService.GetRoutingForDisplayAsync(id);
            if (routing == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<RoutingDisplay>(routing);
            res.Lines = res.Lines.OrderBy(x => x.Sequence); //order by after load data
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoutingDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("Routing", "Create");
            var routing = _mapper.Map<Routing>(val);
            SaveRoutingLines(val, routing);
            await _routingService.CreateAsync(routing);

            val.Id = routing.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RoutingDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("Routing", "Update");
            var routing = await _routingService.GetRoutingForDisplayAsync(id);
            if (routing == null)
                return NotFound();

            routing = _mapper.Map(val, routing);
            SaveRoutingLines(val, routing);

            await _routingService.UpdateAsync(routing);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("Routing", "Unlink");
            var routing = await _routingService.GetByIdAsync(id);
            if (routing == null)
                return NotFound();
            await _routingService.DeleteAsync(routing);

            return NoContent();
        }

        [HttpPost("AutocompleteSimple")]
        public async Task<IActionResult> AutocompleteSimple(RoutingPaged val)
        {
            var res = await _routingService.GetAutocompleteSimpleAsync(val);
            return Ok(res);
        }

        private void SaveRoutingLines(RoutingDisplay val, Routing routing)
        {
            //remove line
            var lineToRemoves = new List<RoutingLine>();
            foreach (var existLine in routing.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                routing.Lines.Remove(line);
            }

            int sequence = 1;
            foreach(var line in val.Lines)
            {
                line.Sequence = sequence++;
            }

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    routing.Lines.Add(_mapper.Map<RoutingLine>(line));
                }
                else
                {
                    _mapper.Map(line, routing.Lines.SingleOrDefault(c => c.Id == line.Id));
                }
            }
        }
    }
}