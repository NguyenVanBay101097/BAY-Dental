using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerSaveAvatarVM
    {
        public Guid PartnerId { get; set; }

        public string ImageId { get; set; }
    }
}
