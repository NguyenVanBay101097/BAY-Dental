using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PartnerMapPSIDFacebookPageService : BaseService<PartnerMapPSIDFacebookPage>, IPartnerMapPSIDFacebookPageService
    {
        private readonly IMapper _mapper;
        private readonly IPartnerService _partnerService;

        public PartnerMapPSIDFacebookPageService(IAsyncRepository<PartnerMapPSIDFacebookPage> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IPartnerService partnerService)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _partnerService = partnerService;
        }

        public async Task<PartnerMapPSIDFacebookPage> CheckPartnerMergeFBPage(Guid PartnerId , string PageId , string PSId)
        {
            var query = await SearchQuery(x => x.PartnerId == PartnerId && x.PageId == PageId && x.PSId == PSId).FirstOrDefaultAsync();
          
            return query;
        }

        public async Task<PartnerMapPSIDFacebookPage> CreatePartnerMapFBPage(CreatePartner val) {

            var result = new Partner
            {
                Name = val.Name,
                Phone = val.phone,
                Email = val.Email,
                CompanyId = CompanyId              
            };

            // Create Customer
            var partner = await _partnerService.CreateAsync(result);
            if(partner.Id == Guid.Empty)
            {
                throw new Exception(" khách hàng tạo không thành công !");
            }

            // Create Merge Partner for FacobookPage
            var map = new PartnerMapPSIDFacebookPage
            {
                PartnerId = partner.Id,
                PageId = val.PageId,
                PSId = val.PSId
            };
            var check = await CheckPartnerMergeFBPage(partner.Id, val.PageId, val.PSId);
            if (check != null)
                throw new Exception(" khách hàng đã được liên kết !");
            await CreateAsync(map);
            return _mapper.Map<PartnerMapPSIDFacebookPage>(map);
        }

        public async Task<PartnerMapPSIDFacebookPage> MergePartnerMapFBPage(CheckPartnerMapFBPage val)
        {
            var check = await CheckPartnerMergeFBPage(val.PartnerId , val.PageId , val.PSId);
            if (check != null) 
                throw new Exception(" khách hàng đã được liên kết !");
            
            var map = new PartnerMapPSIDFacebookPage
            {
                PartnerId = val.PartnerId,
                PageId = val.PageId,
                PSId = val.PSId
            };

            await CreateAsync(map);

            return _mapper.Map<PartnerMapPSIDFacebookPage>(map);
        }
    }
    public class CreatePartner { 

        public string Name { get; set; }

        public string phone { get; set; }

        public string Email { get; set; }

        public string PageId { get; set; }

        public string PSId { get; set; }
    
    }
}
