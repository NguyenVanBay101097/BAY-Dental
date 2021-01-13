using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Phân loại nhóm cho partner
    /// </summary>
    public class PartnerCategory: BaseEntity
    {
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public PartnerCategory Parent { get; set; }

        public string CompleteName { get; set; }

        public bool Active { get; set; }

        public int? ParentLeft { get; set; }

        public int? ParentRight { get; set; }

        public ICollection<PartnerPartnerCategoryRel> PartnerPartnerCategoryRels { get; set; } = new List<PartnerPartnerCategoryRel>();

        public string Color { get; set; }
    }
}
