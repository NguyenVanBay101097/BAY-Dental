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
        public AccountLogin getAccount()
        {
            var account = new AccountLogin();
            string fileName = "AccountLogin.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            //using (StreamReader sr = File.OpenText(path))
            //{
            //    account = JsonConvert.DeserializeObject<AccountLogin>(sr.ReadToEnd());

            //}

            if (!File.Exists(path))
            {
                File.Create(path);
                return null;
            }
            else
            {
                account = JsonConvert.DeserializeObject<AccountLogin>(File.ReadAllText(path));
                return account;
            }


        }

        /// <summary>
        /// luu tai khoan dang nhap
        /// </summary>
        /// <param name="account"></param>
        public void AddAccount(AccountLogin account)
        {
            string fileName = "AccountLogin.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(account));
        }

        public async Task<AccountLogin> RefreshAccesstokenAsync(AccountLogin account)
        {
            RefreshAccesstokenViewModel refresh = new RefreshAccesstokenViewModel();
            refresh.AccessToken = account.AccessToken;
            refresh.RefreshToken = account.RefeshToken;

            var response1 = await HttpClientConfig.client.PostAsync("api/Account/Refresh", new StringContent(JsonConvert.SerializeObject(refresh)));
            var rs = response1.Content.ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<RefreshAccesstokenViewModel>(rs);
            if (account.AccessToken != res.AccessToken && account.RefeshToken != res.RefreshToken)
            {
                account.AccessToken = res.AccessToken;
                account.RefeshToken = res.RefreshToken;
                AddAccount(account);
                var acc = getAccount();
                return acc;
            }

            return account;

        }
    }
}
