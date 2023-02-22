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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrPayrollStructureTypesController : BaseApiController
    {

        private readonly IHrPayrollStructureTypeService _HrPayrollStructureTypeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayrollStructureTypesController(IHrPayrollStructureTypeService HrPayrollStructureTypeService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _HrPayrollStructureTypeService = HrPayrollStructureTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Salary.HrPayrollStructureType.Read")]
        public async Task<IActionResult> Get([FromQuery] HrPayrollStructureTypePaged val)
        {
            var res = await _HrPayrollStructureTypeService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.HrPayrollStructureType.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrPayrollStructureType = await _HrPayrollStructureTypeService.GetHrPayrollStructureTypeDisplay(id);
            if (HrPayrollStructureType == null)
                return NotFound();
            var res = _mapper.Map<HrPayrollStructureTypeDisplay>(HrPayrollStructureType);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.HrPayrollStructureType.Create")]
        public async Task<IActionResult> Create(HrPayrollStructureTypeSave val)
        {
            var type = new HrPayrollStructureType();
            type.CompanyId = CompanyId;

            type = _mapper.Map(val, type);

            await _unitOfWork.BeginTransactionAsync();
            await _HrPayrollStructureTypeService.CreateAsync(type);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrPayrollStructureTypeDisplay>(type));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Salary.HrPayrollStructureType.Update")]
        public async Task<IActionResult> Update(Guid id, HrPayrollStructureTypeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrPayrollStructureTypeService.GetHrPayrollStructureTypeDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);

            await _HrPayrollStructureTypeService.UpdateAsync(str);

            return NoContent();
        }
      
        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.HrPayrollStructureType.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var HrPayrollStructureType = await _HrPayrollStructureTypeService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (HrPayrollStructureType == null)
            {
                return NotFound();
            }
            await _HrPayrollStructureTypeService.DeleteAsync(HrPayrollStructureType);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayrollStructureType.Read")]
        public async Task<IActionResult> Autocomplete(HrPayrollStructureTypePaged val)
        {
            var res = await _HrPayrollStructureTypeService.GetAutocompleteAsync(val);
            return Ok(res);
        }
    }
}
