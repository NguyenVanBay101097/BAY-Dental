using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileSave
    {
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string PSID { get; set; }
        public Guid? PartnerId { get; set; }

        public IEnumerable<FacebookTagSimple> Tags { get; set; } = new List<FacebookTagSimple>();
    }
}
