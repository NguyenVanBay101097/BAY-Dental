using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerCategoryBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CompleteName { get; set; }
    }

    public class PartnerCategoryPaged
    {
        public PartnerCategoryPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? PartnerId { get; set; }
        public IEnumerable<Guid> Ids { get; set; }
    }

    public class PartnerCategoryViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class TagModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
