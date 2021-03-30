using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AdvisoryToothRel
    {
        public Guid AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }

        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
