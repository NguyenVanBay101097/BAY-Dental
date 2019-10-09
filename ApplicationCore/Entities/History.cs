using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class History : BaseEntity
    {
        public History()
        {
            Active = true;
        }
        public string Name { get; set; }

        public bool Active { get; set; }

        public ICollection<PartnerHistoryRel> PartnerHistoryRels { get; set; } = new List<PartnerHistoryRel>();
    }
}
