using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookConnectPageService : BaseService<FacebookConnectPage>, IFacebookConnectPageService
    {
        public FacebookConnectPageService(IAsyncRepository<FacebookConnectPage> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
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
                    await pageObj.DeleteAsync(item);
            }
        }
    }
}
