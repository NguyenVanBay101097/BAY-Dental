﻿using System;
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
using NPOI.HSSF.Record.Chart;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrPayrollStructureController : BaseApiController
    {
        private readonly IHrPayrollStructureService _HrPayrollStructureService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayrollStructureController (IHrPayrollStructureService HrPayrollStructureService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _HrPayrollStructureService = HrPayrollStructureService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrPayrollStructurePaged val)
        {
            var res = await _HrPayrollStructureService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrPayrollStructure = await _HrPayrollStructureService.GetHrPayrollStructureDisplay(id);
            if (HrPayrollStructure == null)
                return NotFound();
            var res = _mapper.Map<HrPayrollStructureDisplay>(HrPayrollStructure);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayrollStructureSave val)
        {
            var entitys = _mapper.Map<HrPayrollStructure>(val);
            await _HrPayrollStructureService.SaveRules(val,entitys);
            await _unitOfWork.BeginTransactionAsync();
            await _HrPayrollStructureService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrPayrollStructureDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayrollStructureSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrPayrollStructureService.GetHrPayrollStructureDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);
           await _HrPayrollStructureService.SaveRules(val,str);
            
            await _HrPayrollStructureService.UpdateAsync(str);

            return NoContent();
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
           await _HrPayrollStructureService.Remove(id);
            return NoContent();
        }
    }
}
