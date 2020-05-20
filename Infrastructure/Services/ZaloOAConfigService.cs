using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class ZaloOAConfigService : BaseService<ZaloOAConfig>, IZaloOAConfigService
    {
        public ZaloOAConfigService(IAsyncRepository<ZaloOAConfig> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
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
                return page;
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

                return await pageObj.CreateAsync(page);
            }
        }
    }
}
