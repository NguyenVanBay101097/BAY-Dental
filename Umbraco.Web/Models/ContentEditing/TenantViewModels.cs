using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TenantBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateExpired { get; set; }
    }

    public class TenantDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }
    }

    public class TenantRegisterViewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class TenantPaged
    {
        public TenantPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}
