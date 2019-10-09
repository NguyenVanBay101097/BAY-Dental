using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : BaseApiController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IAccountInvoiceService _accountInvoiceService;


        public PartnersController(IPartnerService partnerService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork,
            IPartnerCategoryService partnerCategoryService,
            IApplicationRoleFunctionService roleFunctionService,
            IAuthorizationService authorizationService,
            IIRModelAccessService modelAccessService,
            IAccountInvoiceService accountInvoiceService)
        {
            _partnerService = partnerService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerCategoryService = partnerCategoryService;
            _roleFunctionService = roleFunctionService;
            _authorizationService = authorizationService;
            _modelAccessService = modelAccessService;
            _accountInvoiceService = accountInvoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PartnerPaged val)
        {
            _modelAccessService.Check("Partner", "Read");
            var result = await _partnerService.GetPagedResultAsync(val);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _modelAccessService.Check("Partner", "Read");
            var partner = await _partnerService.GetPartnerForDisplayAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<PartnerDisplay>(partner);
            res.City = new CitySimple { Code = partner.CityCode, Name = partner.CityName };
            res.District = new DistrictSimple { Code = partner.DistrictCode, Name = partner.DistrictName };
            res.Ward = new WardSimple { Code = partner.WardCode, Name = partner.WardName };
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartnerDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("Partner", "Create");
            var partner = _mapper.Map<Partner>(val);
            partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);
            SaveCategories(val, partner);
            SaveHistories(val, partner);
            await _partnerService.CreateAsync(partner);

            val.Id = partner.Id;
            return CreatedAtAction(nameof(Get), new { id = partner.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PartnerDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("Partner", "Update");
            var partner = await _partnerService.GetPartnerForDisplayAsync(id);
            if (partner == null)
                return NotFound();
            partner = _mapper.Map(val, partner);

            partner.CityCode = val.City != null ? val.City.Code : string.Empty;
            partner.CityName = val.City != null ? val.City.Name : string.Empty;
            partner.DistrictCode = val.District != null ? val.District.Code : string.Empty;
            partner.DistrictName = val.District != null ? val.District.Name : string.Empty;
            partner.WardCode = val.Ward != null ? val.Ward.Code : string.Empty;
            partner.WardName = val.Ward != null ? val.Ward.Name : string.Empty;

            partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);
            partner.EmployeeId = val.EmployeeId;
            SaveCategories(val, partner);
            SaveHistories(val, partner);
            await _partnerService.UpdateAsync(partner);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("Partner", "Unlink");
            var partner = await _partnerService.GetByIdAsync(id);
            if (partner == null)
                return NotFound();
            await _partnerService.DeleteAsync(partner);

            return NoContent();
        }

        [HttpGet("Autocomplete")]
        public async Task<IActionResult> Autocomplete(string filter = "", bool? customer = null)
        {
            var res = await _partnerService.SearchAutocomplete(filter: filter, customer: customer);
            return Ok(res);
        }

        [HttpPost("Autocomplete2")]
        public async Task<IActionResult> Autocomplete2(PartnerPaged val)
        {
            var res = await _partnerService.SearchPartnersCbx(val);
            res = res.Skip(val.Offset).Take(val.Limit);
            return Ok(res);
        }

        [HttpPost("AutocompleteSimple")]
        public async Task<IActionResult> AutocompleteSimple(PartnerPaged val)
        {
            var res = await _partnerService.SearchPartnersCbx(val);
            return Ok(res);
        }

        [HttpPost("UploadImage/{id}")]
        public async Task<IActionResult> UploadImage(Guid id,IFormFile file)
        {
            var path = await _partnerService.UploadImage(file);

            var entity = await _partnerService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var patch = new JsonPatchDocument<PartnerPatch>();
            patch.Replace(x => x.Avatar, path);
            var entityMap = _mapper.Map<PartnerPatch>(entity);
            patch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            
            await _partnerService.UpdateAsync(entity);

            return Ok();
        }


        private void SaveCategories(PartnerDisplay val, Partner partner)
        {
            var toRemove = partner.PartnerPartnerCategoryRels.Where(x => !val.Categories.Any(s => s.Id == x.CategoryId)).ToList();
            foreach (var categ in toRemove)
            {
                partner.PartnerPartnerCategoryRels.Remove(categ);
            }
            if (val.Categories != null)
            {
                foreach (var categ in val.Categories)
                {
                    if (partner.PartnerPartnerCategoryRels.Any(x => x.CategoryId == categ.Id))
                        continue;
                    partner.PartnerPartnerCategoryRels.Add(_mapper.Map<PartnerPartnerCategoryRel>(categ));

                }
            }

        }

        private void SaveHistories(PartnerDisplay val, Partner partner)
        {
            var toRemove = partner.PartnerHistoryRels.Where(x => !val.Histories.Any(s => s.Id == x.HistoryId)).ToList();
            foreach (var hist in toRemove)
            {
                partner.PartnerHistoryRels.Remove(hist);
            }
            if (val.Histories != null)
            {
                foreach (var hist in val.Histories)
                {
                    if (partner.PartnerHistoryRels.Any(x => x.HistoryId == hist.Id))
                        continue;
                    partner.PartnerHistoryRels.Add(_mapper.Map<PartnerHistoryRel>(hist));

                }
            }

        }

        [HttpGet("{id}/GetInfo")]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            _modelAccessService.Check("Partner", "Read");
            var res = await _partnerService.GetInfo(id);
            return Ok(res);
        }

        //Lấy tất cả hóa đơn của KH 
        [HttpPost("GetCustomerInvoices")]
        public async Task<IActionResult> GetCustomerInvoice(AccountInvoicePaged val)
        {
            var res = await _partnerService.GetCustomerInvoices(val);
            return Ok(res);
        }
    }
}