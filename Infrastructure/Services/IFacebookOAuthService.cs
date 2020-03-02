using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IFacebookOAuthService
    {
        Task<FacebookExchangeTokenResult> ExchangeLiveLongUserAccessToken(string access_token);
    }
}
