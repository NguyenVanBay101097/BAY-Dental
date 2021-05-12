using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models
{
    public class SmsComposerSave
    {
        /// <summary>
        /// res.partner, res.appointment,
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
    }
}
