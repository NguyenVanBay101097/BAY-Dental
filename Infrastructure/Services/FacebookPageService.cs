using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly HttpClient _httpClient;
        private readonly FacebookAuthSettings _fbAuthSettings;
        public FacebookPageService(IAsyncRepository<FacebookPage> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper , IOptions<FacebookAuthSettings> fbAuthSettingsAccessor)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _fbAuthSettings = fbAuthSettingsAccessor.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v6.0/")
            };
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<FacebookUserAccessTokenData> GetFacebookAppAccessToken()
        {
            var response = await _httpClient.GetAsync($"oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials");
            if (!response.IsSuccessStatusCode)
                return default(FacebookUserAccessTokenData);

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserAccessTokenData>(result);

        }

        public async Task<LongTermAccessToken> GetLongTermAccessToken(string Accesstoken)
        {
            var response = await _httpClient.GetAsync($"oauth/access_token?grant_type=fb_exchange_token&client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&fb_exchange_token={Accesstoken}");
            if (!response.IsSuccessStatusCode)
                return default(LongTermAccessToken);

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LongTermAccessToken>(result);

        }

        public async Task<FacebookPageBasic> GetFacebookPage(string accesstoken , long pageId) {
            var PageInfoResponse = await _httpClient.GetAsync($"me/accounts?fields=id,access_token,picture,name&access_token={accesstoken}");
            var result = await PageInfoResponse.Content.ReadAsStringAsync();
            FacebookPageData datalist = JsonConvert.DeserializeObject<FacebookPageData>(result);
            var page = new FacebookPageBasic();
            foreach (var item in datalist.Data)
            {
                if (item.Id == pageId) {
                    page.Id = item.Id;
                    page.Name = item.Name;
                    page.PageAccesstoken = item.PageAccesstoken;
                }
              
               
            }
            return page;
        }

        public async Task<FacebookPage> CheckFacebookPage(long userid , long pageid) {

            var facebookpage = await  SearchQuery(x => x.UserId == userid && x.PageId == pageid).FirstOrDefaultAsync();
            return facebookpage;
        }
        public async Task<FacebookPage> CreateFacebookPage(FacebookPageLinkSave val) {

            //generate an app access token
            var userAccessTokenData = await GetFacebookAppAccessToken();

            //Get long term access token
            var longTermAccessToken = await GetLongTermAccessToken(val.Accesstoken);
            if (longTermAccessToken == null) {
                throw new Exception("Chưa lấy được access_token dài hạn");
            }
            //Get user facebook
            var userInfoResponse = await _httpClient.GetAsync($"me?fields=id,email,first_name,last_name,name,gender,locale&access_token={longTermAccessToken.AccessToken}");
            var result = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(result);

            //Get page facebook
            var page = await GetFacebookPage(longTermAccessToken.AccessToken,val.PageId);
            
            //check 
            var check = await CheckFacebookPage(userInfo.Id, page.Id);
            if (check != null)
            {
                throw new Exception("Fanpage đã được kết nối !");
            }
            var fbpage = new FacebookPage
            {
                UserId = userInfo.Id,
                UserName = userInfo.Name,
                UserAccesstoken = longTermAccessToken.AccessToken,
                PageId = page.Id,
                PageName = page.Name,
                PageAccesstoken = page.PageAccesstoken
            };
            await CreateAsync(fbpage);
            

            return fbpage;

        }


    }
}
