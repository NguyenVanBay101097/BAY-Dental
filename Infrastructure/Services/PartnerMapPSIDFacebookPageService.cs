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

        public async Task<PartnerMapPSIDFacebookPage> CheckPartnerMergeFBPage(Guid PartnerId, string PageId , string PSId)
        {
            var query = await SearchQuery(x => x.PartnerId == PartnerId && x.PageId == PageId && x.PSId == PSId).FirstOrDefaultAsync();
          
            return query;
        }

        public async Task<PartnerMapPSIDFacebookPage> CreatePartnerMapFBPage(PartnerMapPSIDFacebookPageSave val) {

        

            // Create Merge Partner for FacobookPage
            var map = new PartnerMapPSIDFacebookPage
            {
                PartnerId = val.PartnerId,
                PageId = val.PageId,
                PSId = val.PSId
            };
            var check = await CheckPartnerMergeFBPage(val.PartnerId, val.PageId, val.PSId);
            if (check != null)
                throw new Exception(" khách hàng đã được liên kết !");
            await CreateAsync(map);
            return _mapper.Map<PartnerMapPSIDFacebookPage>(map);
        }

        public async Task<PartnerMapPSIDFacebookPage> MergePartnerMapFBPage(PartnerMapPSIDFacebookPageSave val)
        {
            var check = await CheckPartnerMergeFBPage(val.PartnerId,val.PageId , val.PSId);
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


        public async Task Unlink(IEnumerable<Guid> ids) { 

            var result = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            await DeleteAsync(result);      
        }

        public async Task<PartnerMapPSIDFacebookPageBasic> CheckPartner(string PageId , string PSId) {
            var partner = await SearchQuery(x => x.PageId == PageId && x.PSId == PSId).FirstOrDefaultAsync();
            var result = await _partnerService.SearchQuery(x => x.Id == partner.PartnerId && x.Active == true).FirstOrDefaultAsync();
            if (result == null)
            {
                return new PartnerMapPSIDFacebookPageBasic();
            }
            var query = _mapper.Map<PartnerInfoViewModel>(result);
            var basic = new PartnerMapPSIDFacebookPageBasic
            {
                id = partner.Id,
                PartnerName = result.Name,
                PartnerEmail = result.Email,
                PartnerPhone = result.Phone
            };
            return basic;
        }

    }
   

  
}
