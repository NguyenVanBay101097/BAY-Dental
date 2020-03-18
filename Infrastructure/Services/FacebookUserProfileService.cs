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
    public class FacebookUserProfileService : BaseService<FacebookUserProfile>, IFacebookUserProfileService
    {
        public FacebookUserProfileService(IAsyncRepository<FacebookUserProfile> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public override async Task<IEnumerable<FacebookUserProfile>> CreateAsync(IEnumerable<FacebookUserProfile> entities)
        {
            var psids = entities.Select(x => x.PSID);
            var exists = await SearchQuery(x => psids.Contains(x.PSID)).Select(x => x.PSID).ToListAsync();
            entities = entities.Where(x => !exists.Contains(x.PSID));
            return await base.CreateAsync(entities);
        }
    }
}
