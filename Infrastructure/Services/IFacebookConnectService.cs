using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookConnectService: IBaseService<FacebookConnect>
    {
        Task<FacebookConnect> SaveFromUI(FacebookConnectSaveFromUI val);
        Task<FacebookUserData> GetUserAccounts(string access_token, string user_id);
        bool GetConnectWebhooksForPage(string pageId, string pageAccesstoken);
    }
}
