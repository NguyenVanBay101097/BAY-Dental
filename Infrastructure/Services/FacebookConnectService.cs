using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookConnectService : BaseService<FacebookConnect>, IFacebookConnectService
    {
        private readonly FacebookAuthSettings _fbAuthSettings;
        public FacebookConnectService(IAsyncRepository<FacebookConnect> repository, IHttpContextAccessor httpContextAccessor,
            IOptions<FacebookAuthSettings> fbAuthSettings)
            : base(repository, httpContextAccessor)
        {
            _fbAuthSettings = fbAuthSettings?.Value;
        }

        public async Task<FacebookConnect> SaveFromUI(FacebookConnectSaveFromUI val)
        {
            var connect = await SearchQuery().Include(x => x.Pages).FirstOrDefaultAsync();
            if (connect == null)
            {
                var access_token = val.FbUserAccessToken;
                access_token = await GetLongLivedAccessToken(access_token);

                var result = await GetUserAccounts(access_token, val.FbUserId);

                connect = new FacebookConnect()
                {
                    FbUserId = val.FbUserId,
                    FbUserAccessToken = access_token,
                    FbUserName = result.Name,
                };

                foreach (var account in result.Accounts.Data)
                {
                    connect.Pages.Add(new FacebookConnectPage
                    {
                        PageId = account.Id,
                        PageName = account.Name,
                        PageAccessToken = account.PageAccesstoken
                    });
                }

                await CreateAsync(connect);
            }
            else
            {
                var access_token = val.FbUserAccessToken;
                access_token = await GetLongLivedAccessToken(access_token);

                var result = await GetUserAccounts(access_token, val.FbUserId);

                connect.FbUserId = val.FbUserId;
                connect.FbUserAccessToken = access_token;
                connect.FbUserName = result.Name;

                connect.Pages.Clear();
                foreach (var account in result.Accounts.Data)
                {
                    connect.Pages.Add(new FacebookConnectPage
                    {
                        PageId = account.Id,
                        PageName = account.Name,
                        PageAccessToken = account.PageAccesstoken
                    });
                }

                await UpdateAsync(connect);
            }

            return connect;
        }

        public async Task<FacebookUserData> GetUserAccounts(string access_token, string user_id)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{user_id}?fields=id,name,accounts";
            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            var response = (await getRequest.ExecuteAsync<FacebookUserData>());

            string error_message = "";
            if (response.GetExceptions().Any())
            {
                error_message = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(error_message);
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }

        public async Task<string> GetLongLivedAccessToken(string access_token)
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"oauth/access_token";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("grant_type", "fb_exchange_token");
            getRequest.AddQueryParameter("client_id", _fbAuthSettings.AppId);
            getRequest.AddQueryParameter("client_secret", _fbAuthSettings.AppSecret);
            getRequest.AddQueryParameter("fb_exchange_token", access_token);

            var response = (await getRequest.ExecuteAsync<dynamic>());

            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();
                return (string)result["access_token"];
            }
        }
    }
}
