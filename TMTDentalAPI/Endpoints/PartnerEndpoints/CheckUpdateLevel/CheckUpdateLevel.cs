using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints.CheckUpdateLevel
{
    [Authorize]
    public class CheckUpdateLevel : BaseAsyncEndpoint
       .WithoutRequest
       .WithoutResponse
    {
        private readonly IPartnerService _partnerService;
        private readonly IIRPropertyService _propertyService;
        private readonly IMemberLevelService _memberLevelService;

        public CheckUpdateLevel(IPartnerService partnerService,
            IIRPropertyService propertyService,
            IMemberLevelService memberLevelService)
        {
            _partnerService = partnerService;
            _propertyService = propertyService;
            _memberLevelService = memberLevelService;
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

        [HttpPost("api/partners/checkupdatelevel")]
        [SwaggerOperation(
          Summary = "Creates a new Catalog Item",
          Description = "Creates a new Catalog Item",
          OperationId = "catalog-items.create",
          Tags = new[] { "CatalogItemEndpoints" })
      ]
        public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
        {
            var partners = _partnerService.SearchQuery(x => x.Customer).ToList();
            var partnerPointsPropsDict = _propertyService.get_multi("loyalty_points", "res.partner", partners.Select(x => x.Id.ToString()).ToList(), force_company: CompanyId);
            var memberLevels = _memberLevelService.SearchQuery().ToList();

            var levelValuesDict = new Dictionary<string, object>();
            foreach(var partnerPointsPropsItem in partnerPointsPropsDict)
            {
                var pointProp = partnerPointsPropsItem.Value;
                var partnerPoints = Convert.ToDecimal(pointProp == null ? 0 : pointProp);
                var type = memberLevels.Where(x => x.Point <= partnerPoints).OrderByDescending(x => x.Point).FirstOrDefault();
                if (type != null)
                    levelValuesDict.Add(string.Format("res.partner,{0}", partnerPointsPropsItem.Key), type.Id);
            }

            _propertyService.set_multi("member_level", "res.partner", levelValuesDict, force_company: CompanyId);
            return NoContent();
        }
    }
}
