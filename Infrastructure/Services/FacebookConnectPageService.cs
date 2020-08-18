using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookConnectPageService : BaseService<FacebookConnectPage>, IFacebookConnectPageService
    {
        private readonly TenantDbContext _tenantContext;
        private readonly AppTenant _tenant;
        private readonly AppSettings _appSettings;

        public FacebookConnectPageService(IAsyncRepository<FacebookConnectPage> repository, IHttpContextAccessor httpContextAccessor,
            TenantDbContext tenantContext, ITenant<AppTenant> tenant, IOptions<AppSettings> appSettings)
            : base(repository, httpContextAccessor)
        {
            _tenantContext = tenantContext;
            _tenant = tenant?.Value;
            _appSettings = appSettings?.Value;
        }

        public async Task AddConnect(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Connect).ToListAsync();
            var pageObj = GetService<IFacebookPageService>();
            foreach(var page in self)
            {
                var fbPage = await pageObj.SearchQuery(x => x.PageId == page.PageId).FirstOrDefaultAsync();
                if (fbPage != null)
                    continue;

                fbPage = await pageObj.CreateAsync(new FacebookPage
                {
                    PageId = page.PageId,
                    PageName = page.PageName,
                    PageAccesstoken = page.PageAccessToken,
                    UserAccesstoken = page.Connect.FbUserAccessToken,
                    UserId = page.Connect.FbUserId,
                    UserName = page.Connect.FbUserName,
                    Avatar = $"https://graph.facebook.com/{page.PageId}/picture?type=large",
                    Type = "facebook",
                });

                await SubcribeAppFacebookPage(fbPage);
                GetConnectWebhooksForPage(fbPage.PageId, fbPage.PageAccesstoken);
            }
        }

        private bool GetConnectWebhooksForPage(string pageId , string pageAccesstoken)
        {
            var host = _tenant != null ? _tenant.Hostname : "localhost";

            var content = new ConnectWebhooks
            {
                Host = $"{host}.{_appSettings.Domain}",
                FacebookId = pageId,
                FacebookToken = pageAccesstoken,
                FacebookName = null,
                FacebookAvatar = null,
                FacebookCover = null,
                FacebookLink = null,
                FacebookType = 2,
                CallbackUrl = $"http://{host}.{_appSettings.Domain}/api/FacebookWebHook",
                IsCallbackWithRaw = true
            };

            HttpResponseMessage response = null;
            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                response = client.PostAsJsonAsync("https://fba.tpos.vn/api/facebook/updatetoken", content).Result;
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception("Lỗi đăng ký fba.tpos.vn");
            
            return true;
        }

        private async Task<bool> SubcribeAppFacebookPage(FacebookPage fbPage)
        {
            var apiClient = new ApiClient(fbPage.PageAccesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{fbPage.PageId}/subscribed_apps?subscribed_fields=feed,messages,message_deliveries,message_reads";
            var postRequest = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, getRequestUrl, apiClient, false);
            var response = await postRequest.ExecuteAsync<dynamic>();
            if (response.GetExceptions().Any())
            {
                var errorMessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                throw new Exception(errorMessage);
            }

            return true;
        }

        private void AddFacebookPageToTenant(FacebookPage fbPage)
        {
            var host = _tenant != null ? _tenant.Hostname : "localhost";
            var tenant = _tenantContext.Tenants.Where(x => x.Hostname == host).FirstOrDefault();
            if (tenant == null && host == "localhost")
            {
                tenant = new AppTenant { Name = "Localhost", Hostname = host };
                _tenantContext.Tenants.Add(tenant);
                _tenantContext.SaveChanges();
            }

           

            if (tenant != null)
            {
                _tenantContext.TenantFacebookPages.Add(new TenantFacebookPage { TenantId = tenant.Id, PageId = fbPage.PageId });
                _tenantContext.SaveChanges();
            }
        }

        public async Task RemoveConnect(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Connect).ToListAsync();
            var pageObj = GetService<IFacebookPageService>();
            foreach (var page in self)
            {
                var item = await pageObj.SearchQuery(x => x.PageId == page.PageId).FirstOrDefaultAsync();
                if (item != null)
                {
                    await pageObj.DeleteAsync(item);
                    RemoveFacebookPageToTenant(item);
                }
            }
        }

        private void RemoveFacebookPageToTenant(FacebookPage item)
        {
            var host = _tenant != null ? _tenant.Hostname : "localhost";
            var tfps = _tenantContext.TenantFacebookPages.Where(x => x.Tenant.Hostname == host && x.PageId == item.PageId).ToList();
            _tenantContext.TenantFacebookPages.RemoveRange(tfps);
            _tenantContext.SaveChanges();
        }
    }
    public class ConnectWebhooks
    {
        [JsonProperty]
        public string Host { get; set; }
        [JsonProperty]
        public string FacebookId { get; set; }
        [JsonProperty]
        public string FacebookToken { get; set; }
        [JsonProperty]
        public string FacebookName { get; set; }
        [JsonProperty]
        public string FacebookAvatar { get; set; }
        [JsonProperty]
        public string FacebookCover { get; set; }
        [JsonProperty]
        public string FacebookLink { get; set; }
        [JsonProperty]
        public int FacebookType { get; set; }
        [JsonProperty]
        public string CallbackUrl { get; set; }
        [JsonProperty]
        public bool IsCallbackWithRaw { get; set; }
    }
}
