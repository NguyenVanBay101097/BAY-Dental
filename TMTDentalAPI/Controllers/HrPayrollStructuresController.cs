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
using NPOI.HSSF.Record.Chart;
using TMTDentalAPI.JobFilters;
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
        [CheckAccess(Actions = "Salary.HrPayrollStructure.Read")]
        public async Task<IActionResult> Get([FromQuery] HrPayrollStructurePaged val)
        {
            var res = await _payrollStructureService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.HrPayrollStructure.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _payrollStructureService.GetHrPayrollStructureDisplay(id);
            if (res == null)
                return NotFound();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.HrPayrollStructure.Create")]
        public async Task<IActionResult> Create(HrPayrollStructureSave val)
        {
            if (val.RegularPay)
            {
                var existItem = await _payrollStructureService.ExistRegular(val.TypeId, Guid.Empty);
                if (existItem != null)
                {
                    throw new Exception("Loại mẫu lương này đã tồn tại 1 bản mẫu lương thông dụng khác: " + existItem.Name);
                }
            }
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
        [CheckAccess(Actions = "Salary.HrPayrollStructure.Update")]
        public async Task<IActionResult> Update(Guid id, HrPayrollStructureSave val)
        {

            if (val.RegularPay)
            {
                var existItem = await _payrollStructureService.ExistRegular(val.TypeId, id);
                if (existItem != null)
                {
                    throw new Exception("Loại mẫu lương này đã tồn tại 1 bản mẫu lương thông dụng khác: " + existItem.Name);
                }
            }
            var structure = await _payrollStructureService.SearchQuery(x => x.Id == id).Include(x => x.Rules).FirstOrDefaultAsync();
            if (structure == null)
                return NotFound();

            structure = _mapper.Map(val, structure);
            SaveRules(val, structure);

            await _payrollStructureService.UpdateAsync(structure);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.HrPayrollStructure.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _payrollStructureService.Remove(id);
            return NoContent();
        }

        [HttpGet("{id}/Rules")]
        public async Task<IActionResult> GetRules(Guid id)
        {
           var res= await _payrollStructureService.GetRules(id);
            return Ok(res);
        }

    }
}
