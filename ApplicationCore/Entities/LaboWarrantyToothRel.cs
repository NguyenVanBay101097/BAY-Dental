using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboWarrantyToothRel
    {
        public Guid LaboWarrantyId { get; set; }
        public LaboWarranty LaboWarranty { get; set; }

        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
