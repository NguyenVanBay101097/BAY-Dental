using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookPageService : IBaseService<FacebookPage>
    {
        Task<PagedResult2<FacebookPageBasic>> GetPagedResultAsync(FacebookPaged val);
        Task<string> GetFacebookAppAccessToken(string accesstoken);
        Task<FacebookPage> CreateFacebookPage(FacebookPageLinkSave val);
        Task<IEnumerable<string>> GetListPSId(Guid FBpageId);

        Task<IEnumerable<FacebookCustomer>> CheckCustomerNew(Guid FBpageId);
        Task<List<FacebookUserProfile>> CreateFacebookUser();
        Task<FacebookScheduleAppointmentConfigBasic> _GetAutoConfig();

        Task<FacebookPage> CreateAutoConfig(FacebookScheduleAppointmentConfigSave val);
        Task<FacebookPage> UpdateAutoConfig(FacebookScheduleAppointmentConfigSave val);
    }
}
