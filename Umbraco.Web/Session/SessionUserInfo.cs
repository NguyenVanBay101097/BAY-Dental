using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Session
{
    public class SessionUserInfo
    {
        public string Name { get; set; }

        public SessionUserCompany UserCompanies { get; set; }

        /// <summary>
        /// Ngày tên miền hết hạn 
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        public bool IsAdmin { get; set; }

        public IEnumerable<string> Permissions { get; set; }

        public IEnumerable<string> Groups { get; set; }
    }

    public class SessionUserCompany
    {
        public CompanySimple CurrentCompany { get; set; }

        public IEnumerable<CompanySimple> AllowedCompanies { get; set; }
    }
}
