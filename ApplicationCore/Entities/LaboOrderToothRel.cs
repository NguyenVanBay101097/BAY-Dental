using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrderToothRel
    {
        public Guid LaboOrderId { get; set; }
        public LaboOrder LaboOrder { get; set; }
        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
