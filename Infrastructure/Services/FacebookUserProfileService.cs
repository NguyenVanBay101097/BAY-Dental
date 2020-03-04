using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class FacebookUserProfileService : BaseService<FacebookUserProfile>, IFacebookUserProfileService
    {
        public FacebookUserProfileService(IAsyncRepository<FacebookUserProfile> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
