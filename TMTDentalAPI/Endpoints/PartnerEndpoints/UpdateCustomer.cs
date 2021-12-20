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
    public class UpdateCustomer : BaseAsyncEndpoint
      .WithRequest<UpdateCustomerRequest>
      .WithoutResponse
    {
        private readonly IPartnerService _partnerService;
        private readonly IIRSequenceService _sequenceService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public UpdateCustomer(IPartnerService partnerService,
          IIRSequenceService sequenceService,
          IUnitOfWorkAsync unitOfWork)
        {
            _partnerService = partnerService;
            _sequenceService = sequenceService;
            _unitOfWork = unitOfWork;
        }

        [HttpPut("api/Partners/Customers")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public override async Task<ActionResult> HandleAsync(UpdateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            var partner = await _partnerService.SearchQuery(x => x.Id == request.Id)
                .Include(x => x.PartnerHistoryRels)
                .FirstOrDefaultAsync();

            partner.Name = request.Name;
            partner.Phone = request.Phone;
            partner.Ref = request.Ref;
            partner.BirthDay = request.BirthDay;
            partner.BirthMonth = request.BirthMonth;
            partner.BirthYear = request.BirthYear;
            partner.Gender = request.Gender;
            partner.TitleId = request.TitleId;
            partner.Email = request.Email;
            partner.JobTitle = request.JobTitle;
            partner.AgentId = request.AgentId;
            partner.Date = request.Date;
            partner.Street = request.Street;
            partner.SourceId = request.SourceId;
            partner.Comment = request.Comment;
            partner.Avatar = request.Avatar;
            partner.CityCode = request.City != null ? request.City.Code : null;
            partner.CityName = request.City != null ? request.City.Name : null;
            partner.DistrictCode = request.District != null ? request.District.Code : null;
            partner.DistrictName = request.District != null ? request.District.Name : null;
            partner.WardCode = request.Ward != null ? request.Ward.Code : null;
            partner.WardName = request.Ward != null ? request.Ward.Name : null;
            partner.MedicalHistory = request.MedicalHistory;

            var historyRelToDelete = new List<PartnerHistoryRel>();
            foreach(var rel in partner.PartnerHistoryRels.ToList())
            {
                if (!request.HistoryIds.Any(x => x == rel.HistoryId))
                    historyRelToDelete.Add(rel);
            }

            foreach (var item in historyRelToDelete)
                partner.PartnerHistoryRels.Remove(item);

            foreach(var historyId in request.HistoryIds)
            {
                if (!partner.PartnerHistoryRels.Any(x => x.HistoryId == historyId))
                    partner.PartnerHistoryRels.Add(new PartnerHistoryRel { HistoryId = historyId });
            }

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

            await _partnerService.UpdateAsync(partner);

            _unitOfWork.Commit();
        
            return NoContent();
        }
    }
}
