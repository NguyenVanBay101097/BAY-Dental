using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMTTimeKeeper.Models;

namespace TMTTimeKeeper.Services
{
    public class AccountLoginService
    {
        public AccountLogin getAccount()
        {
            var account = new AccountLogin();
            string fileName = "AccountLogin.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                account = JsonConvert.DeserializeObject<AccountLogin>(sr.ReadToEnd());
            }
            return account;
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
    }
}
