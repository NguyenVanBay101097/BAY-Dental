using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookConfigService : BaseService<FacebookConfig>, IFacebookConfigService
    {
        private readonly IMapper _mapper;
        public FacebookConfigService(IAsyncRepository<FacebookConfig> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<FacebookConfig> CreateConfig(FacebookConfigSave val)
        {
            //Tìm xem có config nào với facebook account id chưa?
            var config = await SearchQuery(x => x.FbAccountUserId == val.FbAccountUserId)
                .Include(x => x.ConfigPages).FirstOrDefaultAsync();

            if (config == null)
            {
                config = _mapper.Map<FacebookConfig>(val);
                SaveConfigPages(val, config);

                return await CreateAsync(config);
            }
            else
            {
                config = _mapper.Map(val, config);
                SaveConfigPages(val, config);

                await UpdateAsync(config);
                return config;
            }
        }

        private void SaveConfigPages(FacebookConfigSave val, FacebookConfig config)
        {
            var existLines = config.ConfigPages.ToList();
            var lineToRemoves = new List<FacebookConfigPage>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.ConfigPages)
                {
                    if (item.PageId == existLine.PageId)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
                config.ConfigPages.Remove(line);

            foreach (var line in val.ConfigPages)
            {
                var p = config.ConfigPages.FirstOrDefault(x => x.PageId == line.PageId);
                if (p != null)
                {
                    _mapper.Map(line, p);
                }
                else
                {
                    var cp = _mapper.Map<FacebookConfigPage>(line);
                    config.ConfigPages.Add(cp);
                }
            }
        }

        public async Task<FacebookConfig> CreateConfigConnect(FacebookConfigConnectSave val)
        {
            //Tìm xem có config nào với facebook account id chưa?
            var config = await SearchQuery(x => x.FbAccountUserId == val.UserId)
                .Include(x => x.ConfigPages).FirstOrDefaultAsync();

            if (config == null)
            {
                config = new FacebookConfig() { FbAccountUserId = val.UserId };

                var exchangeResult = await ExchangeLiveLongUserAccessToken(val.AccessToken);
                config.UserAccessToken = exchangeResult.access_token;

                var userInfo = GetFbUserInfo(config.FbAccountUserId, config.UserAccessToken);
                config.FbAccountName = userInfo.name;

                if (userInfo.accounts != null && userInfo.accounts.data != null)
                {
                    foreach (var page in userInfo.accounts.data)
                    {
                        config.ConfigPages.Add(new FacebookConfigPage
                        {
                            PageId = page.id,
                            PageName = page.name,
                            PageAccessToken = page.access_token
                        });
                    }
                } 

                return await CreateAsync(config);
            }

            return config;
        }

        public async Task<FacebookExchangeTokenResult> ExchangeLiveLongUserAccessToken(string access_token)
        {
            HttpClient client = new HttpClient();

            //var client_id = "652339415596520";
            var client_id = "327268081110321";

            //var client_secret = "4f3ebf28374019d1861a36f919997719";
            var client_secret = "84a0b91f1ca32eb6fa804bc11b28d0db";

            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("grant_type", "fb_exchange_token");
            queryBuilder.Add("client_id", client_id);
            queryBuilder.Add("client_secret", client_secret);
            queryBuilder.Add("fb_exchange_token", access_token);

            var requestUri = $"https://graph.facebook.com/v6.0/oauth/access_token{queryBuilder.ToQueryString()}";
            var stream = client.GetStreamAsync(requestUri);
            var result = await JsonSerializer.DeserializeAsync<FacebookExchangeTokenResult>(await stream);

            return result;
        }

        public GetFbUserInfoResponse GetFbUserInfo(string userId, string access_token)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/{userId}";

            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            getRequest.AddQueryParameter("fields", "name,accounts");

            var response = getRequest.Execute<GetFbUserInfoResponse>();
            if (response.GetExceptions().Any())
            {
                var errorMessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMessage);
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }

        public class GetFbUserInfoResponse
        {
            public string id { get; set; }
            public string name { get; set; }
            public GetFbUserInfoAccount accounts { get; set; }
        }

        public class GetFbUserInfoAccount
        {
            public IEnumerable<GetFbUserInfoAccountData> data { get; set; }
        }

        public class GetFbUserInfoAccountData
        {
            public string access_token { get; set; }
            public string category { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public IEnumerable<string> tasks { get; set; }
        }
    }
}
