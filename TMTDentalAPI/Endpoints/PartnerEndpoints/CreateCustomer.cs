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
    public class CreateCustomer : BaseAsyncEndpoint
      .WithRequest<CreateCustomerRequest>
      .WithResponse<CreateCustomerResponse>
    {
        private readonly IPartnerService _partnerService;
        private readonly IIRSequenceService _sequenceService;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CreateCustomer(IPartnerService partnerService,
            IIRSequenceService sequenceService,
            ICurrentUser currentUser,
            IUnitOfWorkAsync unitOfWork)
        {
            _partnerService = partnerService;
            _sequenceService = sequenceService;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("api/Partners/Customers")]
        [CheckAccess(Actions = "Basic.Partner.Create")]
        public override async Task<ActionResult<CreateCustomerResponse>> HandleAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            var partner = new Partner
            { 
                Customer = true,
                Name = request.Name,
                Phone = request.Phone,
                Ref = request.Ref,
                BirthDay = request.BirthDay,
                BirthMonth = request.BirthMonth,
                BirthYear = request.BirthYear,
                Gender = request.Gender,
                TitleId = request.TitleId,
                Email = request.Email,
                JobTitle = request.JobTitle,
                AgentId = request.AgentId,
                Date = request.Date,
                Street = request.Street,
                SourceId = request.SourceId,
                Comment = request.Comment,
                Avatar = request.Avatar,
                MedicalHistory = request.MedicalHistory,
                CompanyId = _currentUser.CompanyId
            };

            partner.CityCode = request.City != null ? request.City.Code : null;
            partner.CityName = request.City != null ? request.City.Name : null;
            partner.DistrictCode = request.District != null ? request.District.Code : null;
            partner.DistrictName = request.District != null ? request.District.Name : null;
            partner.WardCode = request.Ward != null ? request.Ward.Code : null;
            partner.WardName = request.Ward != null ? request.Ward.Name : null;

            foreach (var historyId in request.HistoryIds)
                partner.PartnerHistoryRels.Add(new PartnerHistoryRel { HistoryId = historyId });

            await _unitOfWork.BeginTransactionAsync();
            if (string.IsNullOrEmpty(partner.Ref))
            {
                var customer_code_base = "KH";
                var matchedCodes = await _partnerService.SearchQuery(x => x.Ref.Contains(customer_code_base)).Select(x => x.Ref).ToListAsync();
                foreach (var num in Enumerable.Range(1, 100))
                {
                    var customerCode = await _sequenceService.NextByCode("customer");
                    if (!matchedCodes.Contains(customerCode))
                    {
                        partner.Ref = customerCode;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(partner.Ref))
                    throw new Exception("Không thể phát sinh mã");
            }    

            await _partnerService.CreateAsync(partner);
            _unitOfWork.Commit();

            return new CreateCustomerResponse()
            {
                Id = partner.Id
            };
        }
    }
}
