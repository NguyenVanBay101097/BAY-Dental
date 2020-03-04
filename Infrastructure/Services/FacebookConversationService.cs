using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class FacebookConversationService : BaseService<FacebookConversation>, IFacebookConversationService
    {
        public FacebookConversationService(IAsyncRepository<FacebookConversation> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
