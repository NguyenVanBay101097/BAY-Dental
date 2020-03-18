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
        Task<IEnumerable<string>> GetListPSId(Guid FBpageId);
        Task<PagedResult2<FacebookUserProfileBasic>> GetPagedResultAsync(FacebookUserProfilePaged val);
        Task<List<FacebookCustomer>> CheckCustomerNew(Guid FBpageId);
        Task<List<FacebookUserProfile>> CreateFacebookUser(Guid FBpageId);
        Task<FacebookUserProfile> ActionConectPartner(ConnectPartner val);
    }
}
