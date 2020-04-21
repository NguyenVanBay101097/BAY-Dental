using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileDisplay
    {
        public Guid Id { get; set; }

        public string PsId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerBasic Partner { get; set; }

        public IEnumerable<FacebookTagBasic> Tags { get; set; } = new List<FacebookTagBasic>();
    }
}
