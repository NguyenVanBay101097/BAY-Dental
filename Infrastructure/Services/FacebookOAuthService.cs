using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookOAuthService: IFacebookOAuthService
    {
        public async Task<FacebookExchangeTokenResult> ExchangeLiveLongUserAccessToken(string access_token)
        {
            HttpClient client = new HttpClient();

            var client_id = "652339415596520";
            var client_secret = "4f3ebf28374019d1861a36f919997719";

            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("grant_type", "fb_exchange_token");
            queryBuilder.Add("client_id", client_id);
            queryBuilder.Add("client_secret", client_secret);
            queryBuilder.Add("fb_exchange_token", access_token);

            var requestUri = $"https://graph.facebook.com/v6.0/oauth/access_token{queryBuilder.ToQueryString()}";
            var stream = client.GetStreamAsync(requestUri);
            var result = await JsonSerializer.DeserializeAsync<FacebookExchangeTokenResult>(await stream);

            return result;
        }
    }

    public class FacebookExchangeTokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
    }
}
