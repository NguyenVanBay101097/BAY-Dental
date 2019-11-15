using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CompanySetupTenant
    {
        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }

    public class CompanyPaged
    {
        public CompanyPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }
    }

    public class CompanyBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CompanySimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CompanyDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Street { get; set; }

        public CitySimple City { get; set; }

        public DistrictSimple District { get; set; }

        public WardSimple Ward { get; set; }

        public string Logo { get; set; }
    }

    public class CitySimple
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
