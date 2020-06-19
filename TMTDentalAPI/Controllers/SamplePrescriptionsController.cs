﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Get([FromQuery]SamplePrescriptionPaged val)
        {
            var result = await _prescriptionService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _prescriptionService.GetPrescription(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SamplePrescriptionSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var prescription =  await _prescriptionService.CreatePrescription(val);

            _unitOfWork.Commit();
            var basic = _mapper.Map<SamplePrescriptionBasic>(prescription);

            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SamplePrescriptionSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();

            await _prescriptionService.UpdatePrescription(id,val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();

            await _prescriptionService.DeleteAsync(prescription);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Autocomplete(SamplePrescriptionPaged val)
        {
            var res = await _prescriptionService.GetAutocomplete(val);
            return Ok(res);
        }
    }
}