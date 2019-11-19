using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MailTrackingValue: BaseEntity
    {
        public MailTrackingValue()
        {
            TrackSequence = 100;
        }

        public string Field { get; set; }

        public string FieldDesc { get; set; }

        public string FieldType { get; set; }

        public int? OldValueInteger { get; set; }

        public decimal? OldValueDicimal { get; set; }

        public string OldValueText { get; set; }

        public DateTime? OldValueDateTime { get; set; }

        public int? NewValueInteger { get; set; }

        public decimal? NewValueDecimal { get; set; }

        public string NewValueText { get; set; }

        public DateTime? NewValueDateTime { get; set; }

        public Guid MailMessageId { get; set; }
        public MailMessage MailMessage { get; set; }

        public int TrackSequence { get; set; }
    }
}
