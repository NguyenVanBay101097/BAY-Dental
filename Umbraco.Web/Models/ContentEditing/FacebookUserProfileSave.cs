using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileSave
    {
        public Guid? PartnerId { get; set; }
        public IEnumerable<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
