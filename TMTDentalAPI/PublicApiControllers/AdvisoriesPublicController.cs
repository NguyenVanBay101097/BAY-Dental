using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/Advisories")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class AdvisoriesPublicController : ControllerBase
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IMapper _mapper;
        public AdvisoriesPublicController(IAdvisoryService advisoryService, IMapper mapper)
        {
            _advisoryService = advisoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? partnerId = null)
        {
            var advisories = await _advisoryService.SearchQuery(x => (!partnerId.HasValue || x.CustomerId == partnerId))
                .Select(x => new AdvisoryPublicReponse
                {
                    Id = x.Id,
                    Date = x.Date,
                    EmployeeName = x.Employee.Name,
                    Note = x.Note,
                    Products = x.AdvisoryProductRels.Select(s => s.Product.Name),
                    ToothDiagnosis = x.AdvisoryToothDiagnosisRels.Select(s => s.ToothDiagnosis.Name),
                    Teeth = x.AdvisoryToothRels.Select(s => s.Tooth.Name),
                    ToothType = x.ToothType
                }).ToListAsync();

            return Ok(advisories);
        }
    }
}
