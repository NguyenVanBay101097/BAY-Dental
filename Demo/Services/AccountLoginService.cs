using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Demo.Models;

namespace Demo.Services
{
    public class AccountLoginService
    {
        TimeKeeperService timeKeeperObj = new TimeKeeperService();
        public async Task<AccountLogin> getAccount()
        {
            var account = new AccountLogin();
            string fileName = "AccountLogin.json";
            account = await timeKeeperObj.GetModelByJson<AccountLogin>(fileName);

            return account;
        }

        /// <summary>
        /// luu tai khoan dang nhap
        /// </summary>
        /// <param name="account"></param>
        public void AddAccount(AccountLogin account)
        {
            string fileName = "AccountLogin.json";
            timeKeeperObj.SetJson<AccountLogin>(fileName, account);
        }

        public async Task<AccountLogin> RefreshAccesstoken(AccountLogin account)
        {
            RefreshAccesstokenViewModel refresh = new RefreshAccesstokenViewModel();
            refresh.AccessToken = account.AccessToken;
            refresh.RefreshToken = account.RefeshToken;

            var response1 = await HttpClientConfig.client.PostAsJsonAsync("api/Account/Refresh", refresh);
            var rs = response1.Content.ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<RefreshAccesstokenViewModel>(rs);
            if (account.AccessToken != res.AccessToken && account.RefeshToken != res.RefreshToken)
            {
                account.AccessToken = res.AccessToken;
                account.RefeshToken = res.RefreshToken;
                AddAccount(account);
                var acc = await getAccount();
                return acc;
            }

            return account;

        }
    }
}
