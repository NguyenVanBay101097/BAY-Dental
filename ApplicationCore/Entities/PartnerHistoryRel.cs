using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerHistoryRel
    {
        public Guid PartnerId { get; set; }

        public Partner Partner { get; set; }

        public Guid HistoryId { get; set; }

        public History History { get; set; }
    }
}
