using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerSourceBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class PartnerSourceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
