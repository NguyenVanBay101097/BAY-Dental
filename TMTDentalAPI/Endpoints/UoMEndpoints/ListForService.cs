using ApplicationCore.Entities;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.UoMEndpoints
{
    [Authorize]
    public class ListForService : BaseAsyncEndpoint
    .WithRequest<ListForServiceRequest>
    .WithResponse<ListForServiceResponse>
    {
        private readonly IUoMService _uomService;
        private readonly IIRModelDataService _modelDataService;

        public ListForService(IUoMService uomService,
            IIRModelDataService modelDataService)
        {
            _uomService = uomService;
            _modelDataService = modelDataService;
        }

        [HttpGet("api/ServiceUoMs")]
        public override async Task<ActionResult<ListForServiceResponse>> HandleAsync([FromQuery] ListForServiceRequest request, CancellationToken cancellationToken = default)
        {
            var uomCategory = await _modelDataService.GetRef<UoMCategory>("product.product_uom_categ_service");
            if (uomCategory == null)
                return new ListForServiceResponse();
               
            var uoms = await _uomService.SearchQuery(x => x.CategoryId == uomCategory.Id && (string.IsNullOrEmpty(request.Search) || x.Name.Contains(request.Search)))
                .Select(x => new UoMSimple
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return new ListForServiceResponse()
            {
                Uoms = uoms
            };
        }
    }
}
