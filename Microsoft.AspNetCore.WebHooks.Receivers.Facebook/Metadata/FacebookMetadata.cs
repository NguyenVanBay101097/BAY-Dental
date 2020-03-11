using Microsoft.AspNetCore.WebHooks.Filters;

namespace Microsoft.AspNetCore.WebHooks.Metadata
{
    public class FacebookMetadata : WebHookMetadata,
        IWebHookBodyTypeMetadata,
        IWebHookBodyTypeMetadataService,
        //IWebHookEventMetadata,
        IWebHookEventFromBodyMetadata,
        IWebHookFilterMetadata,
        IWebHookGetHeadRequestMetadata
    {
        private readonly FacebookVerifySignatureFilter _verifySignatureFilter;

        public FacebookMetadata(FacebookVerifySignatureFilter verifySignatureFilter) : base(FacebookConstants.ReceiverName)
        {
            _verifySignatureFilter = verifySignatureFilter;
        }

        public override WebHookBodyType BodyType => WebHookBodyType.Json;

        public bool AllowHeadRequests => false;

        public string ChallengeQueryParameterName => null;

        public int SecretKeyMinLength => FacebookConstants.SecretKeyMinLength;

        public int SecretKeyMaxLength => FacebookConstants.SecretKeyMaxLength;

        public string ConstantValue => FacebookConstants.EventName;

        public string HeaderName => null;

        public string QueryParameterName => null;

        public bool AllowMissing => true;

        public string BodyPropertyPath => FacebookConstants.EventBodyPropertyPath;

        WebHookBodyType? IWebHookBodyTypeMetadata.BodyType => WebHookBodyType.Json;

        public void AddFilters(WebHookFilterMetadataContext context)
        {
            context.Results.Add(_verifySignatureFilter);
        }
    }
}