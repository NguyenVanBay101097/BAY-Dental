using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsComposer : BaseEntity
    {
        public SmsComposer()
        {
            CompositionMode = "mass";
        }

        /// <summary>
        /// mass
        /// </summary>
        public string CompositionMode { get; set; }
        /// <summary>
        /// res.partner, appointment, campaign
        /// </summary>
        public string ResModel { get; set; }

        /// <summary>
        /// ID1,ID2
        /// </summary>
        public string ResIds { get; set; }

        /// <summary>
        /// Noi dung gui
        /// </summary>
        public string Body { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }

    }
}
