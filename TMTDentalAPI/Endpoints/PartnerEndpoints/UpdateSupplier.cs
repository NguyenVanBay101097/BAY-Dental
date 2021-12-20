using ApplicationCore.Entities;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints
{
    [Authorize]
    public class UpdateSupplier : BaseAsyncEndpoint
      .WithRequest<UpdateSupplierRequest>
      .WithoutResponse
    {
        private readonly IPartnerService _partnerService;

        public UpdateSupplier(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpPut("api/Partners/Suppliers")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public override async Task<ActionResult> HandleAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default)
        {
            var partner = await _partnerService.SearchQuery(x => x.Id == request.Id).FirstOrDefaultAsync();

            partner.Name = request.Name;
            partner.Phone = request.Phone;
            partner.Email = request.Email;
            partner.Date = request.Date;
            partner.Street = request.Street;
            partner.Comment = request.Comment;
            partner.CityCode = request.City != null ? request.City.Code : null;
            partner.CityName = request.City != null ? request.City.Name : null;
            partner.DistrictCode = request.District != null ? request.District.Code : null;
            partner.DistrictName = request.District != null ? request.District.Name : null;
            partner.WardCode = request.Ward != null ? request.Ward.Code : null;
            partner.WardName = request.Ward != null ? request.Ward.Name : null;

            await _partnerService.UpdateAsync(partner);

            return NoContent();
        }
    }
}
