using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class FptAuthConfig
    {
        private string _clientId;
        private string _clientSecret;
        private string[] _scopes;
        public FptAuthConfig(string clientId, string clientSecret, string[] scopes)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scopes = scopes;
        }

        public object GetAuthData()
        {
            var sessionId = StringUtils.RandomString(32);
            return new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                scope = string.Join(" ", _scopes),
                session_id = sessionId,
                grant_type = "client_credentials"
            };
        }
    }
}
