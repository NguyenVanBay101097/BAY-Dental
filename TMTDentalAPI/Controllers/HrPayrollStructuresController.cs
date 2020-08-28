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
    public class HrPayrollStructuresController : BaseApiController
    {
        private readonly IHrPayrollStructureService _payrollStructureService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayrollStructuresController(IHrPayrollStructureService payrollStructureService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _payrollStructureService = payrollStructureService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrPayrollStructurePaged val)
        {
            var res = await _payrollStructureService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrPayrollStructure = await _payrollStructureService.GetHrPayrollStructureDisplay(id);
            if (HrPayrollStructure == null)
                return NotFound();
            var res = _mapper.Map<HrPayrollStructureDisplay>(HrPayrollStructure);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayrollStructureSave val)
        {
            var structure = _mapper.Map<HrPayrollStructure>(val);
            SaveRules(val, structure);

            await _unitOfWork.BeginTransactionAsync();
            await _payrollStructureService.CreateAsync(structure);
            _unitOfWork.Commit();

            var basic = _mapper.Map<HrPayrollStructureBasic>(structure);
            return Ok(basic);
        }

        private void SaveRules(HrPayrollStructureSave val, HrPayrollStructure structure)
        {
            var rulesToRemove = new List<HrSalaryRule>();
            foreach (var rule in structure.Rules)
            {
                if (!val.Rules.Any(x => x.Id == rule.Id))
                    rulesToRemove.Add(rule);
            }

            foreach (var item in rulesToRemove)
                structure.Rules.Remove(item);

            var sequence = 0;
            foreach (var rule in val.Rules)
            {
                if (rule.Id == Guid.Empty)
                {
                    var r = _mapper.Map<HrSalaryRule>(rule);
                    r.Sequence = sequence++;
                    structure.Rules.Add(r);
                }
                else
                {
                    var r = structure.Rules.SingleOrDefault(c => c.Id == rule.Id);
                    if (r != null)
                    {
                        _mapper.Map(rule, r);
                        r.Sequence = sequence++;
                    }
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayrollStructureSave val)
        {
            var structure = await _payrollStructureService.SearchQuery(x => x.Id == id).Include(x => x.Rules).FirstOrDefaultAsync();
            if (structure == null)
                return NotFound();

            structure = _mapper.Map(val, structure);
            SaveRules(val, structure);

            await _payrollStructureService.UpdateAsync(structure);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _payrollStructureService.Remove(id);
            return NoContent();
        }
    }
}
