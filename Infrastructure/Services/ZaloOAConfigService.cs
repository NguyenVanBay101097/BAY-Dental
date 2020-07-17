using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class ZaloOAConfigService : BaseService<ZaloOAConfig>, IZaloOAConfigService
    {
        private readonly TenantDbContext _tenantContext;
        private readonly AppTenant _tenant;
        private readonly AppSettings _appSettings;
        public ZaloOAConfigService(IAsyncRepository<ZaloOAConfig> repository, IHttpContextAccessor httpContextAccessor, TenantDbContext tenantContext, ITenant<AppTenant> tenant, IOptions<AppSettings> appSettings)
            : base(repository, httpContextAccessor)
        {
            _tenantContext = tenantContext;
            _tenant = tenant?.Value;
            _appSettings = appSettings?.Value;
        }

        public async Task<FacebookPage> SaveConfig(ZaloOAConfigSave val)
        {
            var accessToken = val.AccessToken;
            var zaloClient = new ZaloClient(accessToken);
            var profile = zaloClient.getProfileOfOfficialAccount().ToObject<GetProfileOAResponse>();
            if (profile.error != 0)
                throw new Exception("access token invalid");
            var pageObj = GetService<IFacebookPageService>();

            var oa_id = profile.data.oa_id.ToString();
            var page = await pageObj.SearchQuery(x => x.PageId == oa_id).FirstOrDefaultAsync();

            if (page != null)
            {
                page.PageName = profile.data.name;
                page.Avatar = profile.data.avatar;
                page.PageAccesstoken = accessToken;
                await pageObj.UpdateAsync(page);              
            }
            else
            {
                page = new FacebookPage
                {
                    PageAccesstoken = accessToken,
                    PageName = profile.data.name,
                    Avatar = profile.data.avatar,
                    PageId = oa_id,
                    Type = "zalo"
                };

                await pageObj.CreateAsync(page);
                GetConnectWebhooksForZalo(page.PageId, page.PageAccesstoken);

            }
            return page;
        }

        private bool GetConnectWebhooksForZalo(string pageId, string pageAccesstoken)
        {
            var host = _tenant != null ? _tenant.Hostname : "localhost";

            var content = new ConnectWebhookZalo
            {
                Host = host,
                ZaloId = pageId,
                ZaloToken = pageAccesstoken,
                ZaloName = null,
                ZaloAvatar = null,
                ZaloCover = null,
                ZaloLink = null,
                ZaloType = 1,
                CallbackUrl = $"{_appSettings.Schema}://{host}.{_appSettings.Domain}/api/ZaloWebHook",
                IsCallbackWithRaw = true
            };

            HttpResponseMessage response = null;
            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                response = client.PostAsJsonAsync("https://fba.tpos.vn/api/zalo/updatetoken", content).Result;
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception("Lỗi đăng ký fba.tpos.vn");

            return true;
        }
    }
    public class ConnectWebhookZalo
    {
        [JsonProperty]
        public string Host { get; set; }
        [JsonProperty]
        public string ZaloId { get; set; }
        [JsonProperty]
        public string ZaloToken { get; set; }
        [JsonProperty]
        public string ZaloName { get; set; }
        [JsonProperty]
        public string ZaloAvatar { get; set; }
        [JsonProperty]
        public string ZaloCover { get; set; }
        [JsonProperty]
        public string ZaloLink { get; set; }
        [JsonProperty]
        public int ZaloType { get; set; }
        [JsonProperty]
        public string CallbackUrl { get; set; }
        [JsonProperty]
        public bool IsCallbackWithRaw { get; set; }
    }
}
