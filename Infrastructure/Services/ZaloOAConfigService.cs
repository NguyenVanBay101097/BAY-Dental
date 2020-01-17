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

        public async Task<ZaloOAConfig> SaveConfig(ZaloOAConfigSave val)
        {
            var accessToken = val.AccessToken;
            var zaloClient = new ZaloClient(accessToken);
            var profile = zaloClient.getProfileOfOfficialAccount().ToObject<GetProfileOAResponse>();
            if (profile.error != 0)
                throw new Exception("access token invalid");

            var config = await SearchQuery().FirstOrDefaultAsync();
            if (config != null)
            {
                config.Name = profile.data.name;
                config.Avatar = profile.data.avatar;
                config.AccessToken = accessToken;
                await UpdateAsync(config);
                return config;
            }
            else
            {
                config = new ZaloOAConfig
                {
                    AccessToken = accessToken,
                    Name = profile.data.name,
                    Avatar = profile.data.avatar
                };

                return await CreateAsync(config);
            }
        }
    }
}
