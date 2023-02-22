using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrderLineToothRel
    {
        public Guid LaboLineId { get; set; }
        public LaboOrderLine LaboLine { get; set; }

        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
