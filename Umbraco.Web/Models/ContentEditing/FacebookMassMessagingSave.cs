using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookMassMessagingSave
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public Guid? FacebookPageId { get; set; }

        public string AudienceFilter { get; set; }
    }
}
