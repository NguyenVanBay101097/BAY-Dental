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
    public class ToothDiagnosisController : BaseApiController
    {
        private readonly IToothDiagnosisService _toothDiagnosisService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ToothDiagnosisController(IToothDiagnosisService toothDiagnosisService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _toothDiagnosisService = toothDiagnosisService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ToothDiagnosisPaged val)
        {
            var result = await _toothDiagnosisService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var toothDiagnosis = await _toothDiagnosisService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (toothDiagnosis == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ToothDiagnosisDisplay>(toothDiagnosis));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Autocomplete(ToothDiagnosisPaged val)
        {
            var res = await _toothDiagnosisService.GetAutocompleteAsync(val);
            return Ok(res);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(ToothDiagnosisSave val)
        //{
        //    if (null == val || !ModelState.IsValid)
        //        return BadRequest();
        //    await _unitOfWork.BeginTransactionAsync();
        //    //var toothDiagnosis = await _toothDiagnosisService.CreateToothDiagnosis(val);
        //    _unitOfWork.Commit();
        //    var basic = _mapper.Map<ToothDiagnosisDisplay>(toothDiagnosis);
        //    return Ok(basic);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ToothDiagnosisSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();


            var result = await _toothDiagnosisService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            result = _mapper.Map(val, result);
            await _unitOfWork.BeginTransactionAsync();
            await _toothDiagnosisService.UpdateAsync(result);
            _unitOfWork.Commit();

            return NoContent();
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Remove(Guid id)
        //{
        //    await _unitOfWork.BeginTransactionAsync();
        //    await _toothDiagnosisService.RemoveToothDiagnosis(id);
        //    _unitOfWork.Commit();
        //    return NoContent();
        //}
    }
}
