using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public Guid CompanyId { get; set; }
        public bool Active { get; set; }
        public bool IsUserRoot { get; set; }
        public Guid? FacebookPageId { get; set; }
    }
    public class ApplicationUserSimple
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class ApplicationUserBasic
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }
    }

    public class ApplicationUserDisplay
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Guid CompanyId { get; set; }
        public CompanyBasic Company { get; set; }

        public IEnumerable<CompanyBasic> Companies { get; set; } = new List<CompanyBasic>();

        public IEnumerable<ResGroupBasic> Groups { get; set; } = new List<ResGroupBasic>();

        public string Avatar { get; set; }
    }

    public class ApplicationUserPaged
    {
        public ApplicationUserPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string SearchNameUserName { get; set; }

        public string Search { get; set; }

        public bool? HasRoot { get; set; }
    }
}
