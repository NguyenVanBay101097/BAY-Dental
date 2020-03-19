using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookUserProfileService : IBaseService<FacebookUserProfile>
    {
        Task<PagedResult2<FacebookUserProfileBasic>> GetPagedResultAsync(FacebookUserProfilePaged val);
       
        Task<FacebookUserProfile> ActionConectPartner(ConnectPartner val);
        Task CheckPsid(string Psid);
    }
}
