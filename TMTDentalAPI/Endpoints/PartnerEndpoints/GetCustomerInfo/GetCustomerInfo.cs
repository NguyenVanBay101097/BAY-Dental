using Ardalis.ApiEndpoints;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints.GetCustomerInfo
{
    [Authorize]
    public class GetCustomerInfo : BaseAsyncEndpoint
       .WithRequest<GetCustomerInfoPartnerRequest>
       .WithResponse<GetCustomerInfoPartnerResponse>
    {
        private readonly IPartnerService _partnerService;
        private readonly IIRPropertyService _propertyService;
        private readonly IMemberLevelService _memberLevelService;
        private readonly IMapper _mapper;

        public GetCustomerInfo(IPartnerService partnerService,
            IIRPropertyService propertyService,
            IMemberLevelService memberLevelService,
            IMapper mapper)
        {
            _partnerService = partnerService;
            _propertyService = propertyService;
            _memberLevelService = memberLevelService;
            _mapper = mapper;
        }

        protected string UserId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return null;

                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        protected Guid CompanyId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        [HttpGet("api/partners/{PartnerId}/customerinfo")]
        [SwaggerOperation(
            Summary = "Creates a new Catalog Item",
            Description = "Creates a new Catalog Item",
            OperationId = "catalog-items.create",
            Tags = new[] { "CatalogItemEndpoints" })
        ]

        public override async Task<ActionResult<GetCustomerInfoPartnerResponse>> HandleAsync([FromRoute] GetCustomerInfoPartnerRequest request, CancellationToken cancellationToken)
        {
            var response = await _partnerService.SearchQuery(x => x.Id == request.PartnerId).Select(x => new GetCustomerInfoPartnerResponse
            {
                Id = x.Id,
                Avatar = x.Avatar,
                BirthDay = x.BirthDay,
                BirthMonth = x.BirthMonth,
                BirthYear = x.BirthYear,
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                WardName = x.WardName,
                Ref = x.Ref,
                Name = x.Name,
                Date = x.Date,
                Email = x.Email,
                Gender = x.Gender,
                Street = x.Street,
                JobTitle = x.JobTitle,
                Phone = x.Phone,
                MedicalHistory = x.MedicalHistory,
                Histories = x.PartnerHistoryRels.Select(x => new HistorySimple()
                {
                    Id = x.HistoryId,
                    Name = x.History.Name
                }),
                Categories = x.PartnerPartnerCategoryRels.Select(s => new PartnerCategoryBasic
                {
                    Id = s.CategoryId,
                    Name = s.Category.Name,
                    Color = s.Category.Color
                })
            }).FirstOrDefaultAsync();

            if (response == null)
                return NotFound();

            var partnerPointsProp = _propertyService.get("loyalty_points", "res.partner", res_id: $"res.partner,{request.PartnerId}", force_company: CompanyId);
            var partnerPoints = Convert.ToDecimal(partnerPointsProp == null ? 0 : partnerPointsProp);
            response.Point = partnerPoints;

            var partnerLevelProp = _propertyService.get("member_level", "res.partner", res_id: $"res.partner,{request.PartnerId}", force_company: CompanyId);
            var partnerLevelValue = partnerLevelProp == null ? string.Empty : partnerLevelProp.ToString();
            var partnerLevelId = !string.IsNullOrEmpty(partnerLevelValue) ? Guid.Parse(partnerLevelValue.Split(",")[1]) : (Guid?)null;
            if (partnerLevelId.HasValue)
            {
                var level = await _memberLevelService.GetByIdAsync(partnerLevelId);
                response.MemberLevel = _mapper.Map<MemberLevelBasic>(level);
            }

            return response;
        }
    }
}
