using System;
using Microsoft.AspNetCore.WebHooks.Metadata;

namespace Microsoft.AspNetCore.WebHooks
{
    public class FacebookWebHookAttribute : WebHookAttribute
    {
        public FacebookWebHookAttribute()
            : base(FacebookConstants.ReceiverName)
        {
        }
    }
}
