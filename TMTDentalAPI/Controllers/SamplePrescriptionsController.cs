using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SamplePrescriptionsController : BaseApiController
    {
        private readonly ISamplePrescriptionService _prescriptionService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public SamplePrescriptionsController(ISamplePrescriptionService prescriptionService , IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _prescriptionService = prescriptionService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.SamplePrescription.Read")]
        public async Task<IActionResult> Get([FromQuery]SamplePrescriptionPaged val)
        {
            var result = await _prescriptionService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")][CheckAccess(Actions = "Catalog.SamplePrescription.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _prescriptionService.GetPrescriptionForDisplay(id);
            return Ok(res);
        }

        [HttpPost][CheckAccess(Actions = "Catalog.SamplePrescription.Create")]
        public async Task<IActionResult> Create(SamplePrescriptionSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var prescription =  await _prescriptionService.CreatePrescription(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<SamplePrescriptionBasic>(prescription);
            return Ok(basic);
        }

        [HttpPut("{id}")][CheckAccess(Actions = "Catalog.SamplePrescription.Update")]
        public async Task<IActionResult> Update(Guid id, SamplePrescriptionSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _prescriptionService.UpdatePrescription(id,val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")][CheckAccess(Actions = "Catalog.SamplePrescription.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();

            await _prescriptionService.DeleteAsync(prescription);

            return NoContent();
        }

       
    }
}