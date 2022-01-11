using ApplicationCore.Entities;
using ApplicationCore.Users;
using Ardalis.ApiEndpoints;
using AutoMapper;
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
using TMTDentalAPI.Endpoints.PartnerEndpoints.GetCustomerInfo;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints
{
    [Authorize]
    public class CreateSupplier : BaseAsyncEndpoint
      .WithRequest<CreateSupplierRequest>
      .WithResponse<CreateSupplierResponse>
    {
        private readonly IPartnerService _partnerService;
        private readonly IIRSequenceService _sequenceService;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CreateSupplier(IPartnerService partnerService,
            IIRSequenceService sequenceService,
            ICurrentUser currentUser,
            IUnitOfWorkAsync unitOfWork)
        {
            _partnerService = partnerService;
            _sequenceService = sequenceService;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("api/Partners/Suppliers")]
        [CheckAccess(Actions = "Basic.Partner.Create")]
        public override async Task<ActionResult<CreateSupplierResponse>> HandleAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
        {
            var partner = new Partner
            {
                Customer = false,
                Supplier = true,
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                Date = request.Date,
                Street = request.Street,
                Comment = request.Comment,
                CompanyId = _currentUser.CompanyId
            };

            partner.CityCode = request.City != null ? request.City.Code : null;
            partner.CityName = request.City != null ? request.City.Name : null;
            partner.DistrictCode = request.District != null ? request.District.Code : null;
            partner.DistrictName = request.District != null ? request.District.Name : null;
            partner.WardCode = request.Ward != null ? request.Ward.Code : null;
            partner.WardName = request.Ward != null ? request.Ward.Name : null;

            await _unitOfWork.BeginTransactionAsync();
            if (string.IsNullOrEmpty(partner.Ref))
            {
                var matchedCodes = await _partnerService.SearchQuery(x => x.Supplier && !string.IsNullOrEmpty(x.Ref)).Select(x => x.Ref).ToListAsync();
                foreach (var num in Enumerable.Range(1, 100))
                {
                    var supplierCode = await _sequenceService.NextByCode("supplier");
                    if (!matchedCodes.Contains(supplierCode))
                    {
                        partner.Ref = supplierCode;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(partner.Ref))
                    throw new Exception("Không thể phát sinh mã");
            }

            await _partnerService.CreateAsync(partner);
            _unitOfWork.Commit();

            return new CreateSupplierResponse()
            {
                Id = partner.Id,
                Name = partner.Name,
                DisplayName = partner.DisplayName,
            };
        }
    }
}
