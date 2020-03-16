using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerMapPSIDFacebookPageBasic
    {
        public Guid id { get; set; }     
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }

        public string PartnerPhone { get; set; }

        public string PartnerEmail { get; set; }

    }
}
