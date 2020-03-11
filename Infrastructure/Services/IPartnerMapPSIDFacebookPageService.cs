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
        Task<PartnerMapPSIDFacebookPage> CheckPartnerMergeFBPage(Guid PartnerId ,string PageId, string PSId);
        Task<PartnerMapPSIDFacebookPage> CreatePartnerMapFBPage(PartnerMapPSIDFacebookPageSave val);
        Task<PartnerMapPSIDFacebookPage> MergePartnerMapFBPage(PartnerMapPSIDFacebookPageSave val);
        Task Unlink(IEnumerable<Guid> ids);
        Task<PartnerMapPSIDFacebookPageBasic> CheckPartner(string PageId, string PSId);


    }
}
