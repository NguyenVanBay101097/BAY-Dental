using Microsoft.AspNetCore.WebHooks.Filters;

namespace Microsoft.AspNetCore.WebHooks.Metadata
{
    public class FacebookMetadata : WebHookMetadata,
        IWebHookBodyTypeMetadata,
        IWebHookBodyTypeMetadataService,
        IWebHookGetHeadRequestMetadata
    {
        public FacebookMetadata() : base(FacebookConstants.ReceiverName)
        {
        }

        public override WebHookBodyType BodyType => WebHookBodyType.Json;

        public bool AllowHeadRequests => false;

        public string ChallengeQueryParameterName => null;

        public int SecretKeyMinLength => FacebookConstants.SecretKeyMinLength;

        public int SecretKeyMaxLength => FacebookConstants.SecretKeyMaxLength;

        WebHookBodyType? IWebHookBodyTypeMetadata.BodyType => WebHookBodyType.Json;
    }
}