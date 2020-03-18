using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookPageService : BaseService<FacebookPage>, IFacebookPageService
    {
        private readonly IMapper _mapper;
        private readonly FacebookAuthSettings _fbAuthSettings;
        public FacebookPageService(IAsyncRepository<FacebookPage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper , IOptions<FacebookAuthSettings> fbAuthSettingsAccessor)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _fbAuthSettings = fbAuthSettingsAccessor?.Value;
        }

        public override async Task<FacebookPage> CreateAsync(FacebookPage entity)
        {
            await _CheckConstraint(entity);
            return await base.CreateAsync(entity);
        }

        private async Task _CheckConstraint(FacebookPage self)
        {
            var exist = await SearchQuery(x => x.PageId == self.PageId).FirstOrDefaultAsync();
            if (exist != null)
                throw new Exception($"Bạn đã kết nối với page {exist.PageName}");
        }

        public async Task<PagedResult2<FacebookPageBasic>> GetPagedResultAsync(FacebookPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.UserName.Contains(val.Search) || x.PageName.Contains(val.Search));

            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookPageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<FacebookPageBasic>>(items)
            };
        }

        public async Task<string> GetFacebookAppAccessToken(string accesstoken)
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(_fbAuthSettings.AppId, _fbAuthSettings.AppSecret,accesstoken,FacebookApiVersions.V6_0);
            var getRequestUrl = $"oauth/access_token";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient,false);
            getRequest.AddQueryParameter("client_id", _fbAuthSettings.AppId);
            getRequest.AddQueryParameter("client_secret", _fbAuthSettings.AppSecret);
            getRequest.AddQueryParameter("grant_type", "client_credentials");
            var response = await getRequest.ExecuteAsync<dynamic>();          
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

        public async Task<string> GetLongTermAccessToken(string Accesstoken)
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(_fbAuthSettings.AppId, _fbAuthSettings.AppSecret, Accesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"oauth/access_token";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("grant_type", "fb_exchange_token");
            getRequest.AddQueryParameter("client_id", _fbAuthSettings.AppId);
            getRequest.AddQueryParameter("client_secret", _fbAuthSettings.AppSecret);
            getRequest.AddQueryParameter("fb_exchange_token", Accesstoken);
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

        public async Task<FacebookPage> GetFacebookPage(string accesstoken , string pageId) {
            string errorMaessage = null;
            var page = new FacebookPage();
            var a = new Object();
            var apiClient = new ApiClient(accesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"me?fields=id,name,accounts";
            var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);           
            getRequest.AddQueryParameter("access_token", accesstoken);
            var response = (await getRequest.ExecuteAsync<FacebookUserData>());
            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();
                page.UserId = result.Id;
                page.UserName = result.Name;
                page.UserAccesstoken = accesstoken;
                var fbpage = result.Accounts.Data.Where(x => x.Id == pageId).SingleOrDefault();
                page.PageId = fbpage.Id;
                page.PageName = fbpage.Name;
                page.PageAccesstoken = fbpage.PageAccesstoken;
                return page ;
            }
        }

        public async Task<FacebookPage> CheckFacebookPage(string userid , string pageid) {

            var facebookpage = await  SearchQuery(x => x.UserId == userid && x.PageId == pageid).FirstOrDefaultAsync();
            return facebookpage;
        }

        public async Task<FacebookPage> CreateFacebookPage(FacebookPageLinkSave val) {

            //generate an app access token
            var userAccessTokenData = await GetFacebookAppAccessToken(val.Accesstoken);
            if (userAccessTokenData == null)
            {
                throw new Exception("App facebook không tồn tại !");
            }
            //Get long term access token
            var longTermAccessToken = await GetLongTermAccessToken(val.Accesstoken);
            if (longTermAccessToken == null) {
                throw new Exception("Chưa lấy được access_token dài hạn");
            }
            
            //Get page facebook
            var page = await GetFacebookPage(longTermAccessToken,val.PageId);

            //check
            var check = await CheckFacebookPage(page.UserId, page.PageId);
            if (check != null)
            {
                throw new Exception("Fanpage đã được kết nối !");
            }
            var fbpage = new FacebookPage
            {
                UserId = page.UserId,
                UserName = page.UserName,
                UserAccesstoken = longTermAccessToken,
                PageId = page.PageId,
                PageName = page.PageName,
                PageAccesstoken = page.PageAccesstoken
            };
            await CreateAsync(fbpage);


            return fbpage;

        }


        public async Task LoadUserProfileFromConversations(Guid id)
        {
            var self = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            //Tải tất cả những conversations
            var limit = 100;
            var list = new List<FacebookUserProfile>();

            var result = await GetPageConversations(self.PageId, self.PageAccesstoken, limit);

            //Lấy ra 1 list psids duy nhất
            var psids = new List<string>().AsEnumerable();
            psids = psids.Union(ExtractPSIDsUserProfile(result, self.PageId));

            while (!string.IsNullOrEmpty(result.paging.next))
            {
                result = await GetPageConversations(self.PageId, self.PageAccesstoken, limit, after: result.paging.cursors.after);
                psids = psids.Union(ExtractPSIDsUserProfile(result, self.PageId));
            }

            var tasks = psids.Select(x => GetUserProfileFromPSID(x, self.PageAccesstoken));
            var all_profiles = await Task.WhenAll(tasks);
            foreach(var profile in all_profiles)
            {
                if (profile == null)
                    continue;
                list.Add(new FacebookUserProfile
                {
                    Name = profile.name,
                    FirstName = profile.first_name,
                    LastName = profile.last_name,
                    FbPageId = self.Id,
                    PSID = profile.id,
                    Gender = profile.gender,
                });
            }

            var userProfileObj = GetService<IFacebookUserProfileService>();
            await userProfileObj.CreateAsync(list);
        }

        public IEnumerable<string> ExtractPSIDsUserProfile(ApiPageConversationsResponse response, string pageId)
        {
            if (response.data == null)
                return new List<string>();

            var list = new List<string>().AsEnumerable();
            foreach(var item in response.data)
            {
                var ids = item.participants.data.Where(x => x.id != pageId).Select(x => x.id);
                list = list.Union(ids);
            }

            return list;
        }

        public async Task<ApiPageConversationsResponse> GetPageConversations(string page_id, string access_token, int limit, string after = "")
        {
            string errorMaessage = null;
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{page_id}/conversations";
            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("fields", "participants");
            getRequest.AddQueryParameter("limit", limit.ToString());
            if (!string.IsNullOrEmpty(after))
            {
                getRequest.AddQueryParameter("pretty", "0");
                getRequest.AddQueryParameter("after", after);
            }
            var response = (await getRequest.ExecuteAsync<ApiPageConversationsResponse>());

            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMaessage);
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }

        public async Task<ApiUserProfileResponse> GetUserProfileFromPSID(string psid, string access_token)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{psid}";
            var getRequest = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
            getRequest.AddQueryParameter("fields", "name,first_name,last_name,profile_pic,gender");
            var response = (await getRequest.ExecuteAsync<ApiUserProfileResponse>());

            if (response.GetExceptions().Any())
            {
                var error_message = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                return null;
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }
    }

    public class ApiPageConversationsResponse
    {
        public IEnumerable<ApiPageConversationsData> data { get; set; }
        public ApiPageConversationsPaging paging { get; set; }
    }

    public class ApiPageConversationsData
    {
        public ApiPageConversationsDataParticipants participants { get; set; }
        public string id { get; set; }
    }

    public class ApiPageConversationsDataParticipants
    {
        public IEnumerable<ApiPageConversationsDataParticipantData> data { get; set; }
    }

    public class ApiPageConversationsDataParticipantData
    {
        public string name { get; set; }
        public string email { get; set; }
        public string id { get; set; }
    }

    public class ApiPageConversationsPaging
    {
        public ApiPageConversationsPagingCursors cursors { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
    }

    public class ApiPageConversationsPagingCursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class ApiUserProfileResponse
    {
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string profile_pic { get; set; }
        public string gender { get; set; }
        public string id { get; set; }
    }
}
