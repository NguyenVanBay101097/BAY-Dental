using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookConnectPageService : BaseService<FacebookConnectPage>, IFacebookConnectPageService
    {
        private readonly TenantDbContext _tenantContext;
        private readonly AppTenant _tenant;

        public FacebookConnectPageService(IAsyncRepository<FacebookConnectPage> repository, IHttpContextAccessor httpContextAccessor,
            TenantDbContext tenantContext, ITenant<AppTenant> tenant)
            : base(repository, httpContextAccessor)
        {
            _tenantContext = tenantContext;
            _tenant = tenant?.Value;
        }

        public async Task AddConnect(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Connect).ToListAsync();
            var pageObj = GetService<IFacebookPageService>();
            foreach(var page in self)
            {
                var fbPage = await pageObj.CreateAsync(new FacebookPage
                {
                    PageId = page.PageId,
                    PageName = page.PageName,
                    PageAccesstoken = page.PageAccessToken,
                    UserAccesstoken = page.Connect.FbUserAccessToken,
                    UserId = page.Connect.FbUserId,
                    UserName = page.Connect.FbUserName
                });

                await SubcribeAppFacebookPage(fbPage);
                AddFacebookPageToTenant(fbPage);
            }
        }

        private async Task<bool> SubcribeAppFacebookPage(FacebookPage fbPage)
        {
            var apiClient = new ApiClient(fbPage.PageAccesstoken, FacebookApiVersions.V6_0);
            var getRequestUrl = $"{fbPage.PageId}/subscribed_apps?subscribed_fields=feed,messages";
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
}
