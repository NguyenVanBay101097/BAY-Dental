using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerTitleBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class PartnerTitlePaged
    {
        public PartnerTitlePaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }

    public class PartnerTitleSave
    {
        public string Name { get; set; }
    }
}
