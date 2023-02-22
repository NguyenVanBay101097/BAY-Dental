using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class DotKhamLineToothRel
    {
        public Guid LineId { get; set; }
        public DotKhamLine Line { get; set; }

        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
