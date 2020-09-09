using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class AccountLogin
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string RefeshToken { get; set; }
        public string Email { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
