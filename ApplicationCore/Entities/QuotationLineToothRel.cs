using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class QuotationLineToothRel
    {
        public QuotationLine QuotationLine { get; set; }
        public Guid QuotationLineId { get; set; }

        public Tooth Tooth { get; set; }
        public Guid ToothId { get; set; }
    }
}
