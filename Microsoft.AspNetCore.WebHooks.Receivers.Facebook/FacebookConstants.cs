namespace Microsoft.AspNetCore.WebHooks
{
    public static class FacebookConstants
    {
        public static string ReceiverName => "facebook";
        public static string EventName => "change";
        public static int SecretKeyMinLength => 1;
        public static int SecretKeyMaxLength => 100;
        public static string VerifyTokenParameterName => "hub.verify_token";
        public static string ChallengeParameterName => "hub.challenge";
        public static string EventBodyPropertyPath => "$.entry[0].changes[0].field";
    }
}
