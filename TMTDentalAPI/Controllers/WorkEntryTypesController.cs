using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.FileSystem;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkEntryTypesController : BaseApiController
    {

        private readonly IWorkEntryTypeService _workEntryTypeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public WorkEntryTypesController(IWorkEntryTypeService workEntryTypeService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _workEntryTypeService = workEntryTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] WorkEntryTypePaged val)
        {
            var res = await _workEntryTypeService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var wet = await _workEntryTypeService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (wet == null)
                return NotFound();
            var res = _mapper.Map<WorkEntryTypeDisplay>(wet);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkEntryTypeSave val)
        {
            var wet = _mapper.Map<WorkEntryType>(val);
           var entity= await _workEntryTypeService.CreateAsync(wet);
            return Ok(_mapper.Map<WorkEntryTypeDisplay>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, WorkEntryTypeSave val)
        {
            var entity = await _workEntryTypeService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }
            entity = _mapper.Map(val, entity);
            await _workEntryTypeService.UpdateAsync(entity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var entity = await _workEntryTypeService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }
            await _workEntryTypeService.DeleteAsync(entity);
            return NoContent();
        }

    }
}
