using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerMapPSIDFacebookPageService :  IBaseService<PartnerMapPSIDFacebookPage>
    {
        Task<PartnerMapPSIDFacebookPage> CheckPartnerMergeFBPage(Guid PartnerId, string PageId, string PSId);
        Task<PartnerMapPSIDFacebookPage> CreatePartnerMapFBPage(CreatePartner val);
        Task<PartnerMapPSIDFacebookPage> MergePartnerMapFBPage(CheckPartnerMapFBPage val);
    }
}
