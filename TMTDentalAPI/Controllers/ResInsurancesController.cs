using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResInsurancesController : BaseApiController
    {
        private readonly IResInsuranceService _insuranceService;
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ResInsurancesController(IResInsuranceService insuranceService, IPartnerService partnerService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _insuranceService = insuranceService;
            _partnerService = partnerService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [CheckAccess(Actions = "Catalog.Insurance.Read")]
        public async Task<IActionResult> Get([FromQuery] ResInsurancePaged val)
        {
            var result = await _insuranceService.GetPagedResult(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.Insurance.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _insuranceService.GetDisplayById(id);
            var display = _mapper.Map<ResInsuranceDisplay>(res);
            return Ok(display);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.Insurance.Create")]
        public async Task<IActionResult> Create(ResInsuranceSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
          
            var insurance = _mapper.Map<ResInsurance>(val);
            insurance.CompanyId = CompanyId;         
            await _insuranceService.CreateAsync(insurance);

            var exist = await _insuranceService.CheckInsuranceNameExist(val.Name);
            if (exist)
                throw new Exception("Công ty bảo hiểm đã tồn tại");

            _unitOfWork.Commit();

            var simple = _mapper.Map<ResInsuranceSimple>(insurance);
            return Ok(simple);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AutoComplete(ResInsurancePaged val)
        {
            var insurances = await _insuranceService.GetAutoComplete(val);
            var itemSimples = _mapper.Map<IEnumerable<ResInsuranceSimple>>(insurances);
            return Ok(itemSimples);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.Insurance.Update")]
        public async Task<IActionResult> Update(Guid id, ResInsuranceSave val)
        {

            var insurance = await _insuranceService.SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();

            if (insurance == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            insurance = _mapper.Map(val, insurance);

            await UpdatePartnerToInsurance(insurance);

            await _insuranceService.UpdateAsync(insurance);

            var exist = await _insuranceService.CheckInsuranceNameExist(insurance.Name);
            if (exist)
                throw new Exception("Công ty bảo hiểm đã tồn tại");

            _unitOfWork.Commit();

            return NoContent();
        }

        private async Task UpdatePartnerToInsurance(ResInsurance insurance)
        {
            if (insurance.Partner == null)
                return;
            var pn = insurance.Partner;
            pn.Name = insurance.Name;
            pn.Phone = insurance.Phone;
            await _partnerService.UpdateAsync(pn);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Insurance.Update")]
        public async Task<IActionResult> ActionActive(InsuranceActionDeactiveRequest val)
        {
            var insurances = await _insuranceService.SearchQuery(x => val.Ids.Contains(x.Id)).ToListAsync();
            foreach (var insurance in insurances)
                insurance.IsActive = true;
            await _insuranceService.UpdateAsync(insurances);

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Insurance.Update")]
        public async Task<IActionResult> ActionDeactive(InsuranceActionDeactiveRequest val)
        {
            var insurances = await _insuranceService.SearchQuery(x => val.Ids.Contains(x.Id)).ToListAsync();
            foreach (var insurance in insurances)
                insurance.IsActive = false;
            await _insuranceService.UpdateAsync(insurances);

            return NoContent();
        }


        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.Insurance.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var insurance = await _insuranceService.GetByIdAsync(id);
            if (insurance == null)
                return NotFound();

            if (insurance.Partner != null)
                await _partnerService.DeleteAsync(insurance.Partner);

            await _insuranceService.DeleteAsync(insurance);
            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
