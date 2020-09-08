using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ApplicationRolePaged
    {
        public ApplicationRolePaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }
    }

    public class ApplicationRoleBasic
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class ApplicationRoleDisplay
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Functions { get; set; } = new List<string>();

        public IEnumerable<ApplicationUserSimple> Users { get; set; } = new List<ApplicationUserSimple>();
    }

    public class ApplicationRoleSave
    {
        public string Name { get; set; }

        public IEnumerable<string> Functions { get; set; } = new List<string>();
    }
}
