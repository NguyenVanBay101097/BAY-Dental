using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerBasic Partner { get; set; }

        public string Avatar { get; set; }
    }
}
