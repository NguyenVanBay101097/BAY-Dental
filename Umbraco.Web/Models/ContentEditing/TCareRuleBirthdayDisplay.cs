using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareRuleBirthdayDisplay
    {
        public TCareRuleBirthdayDisplay()
        {
            BeforeDays = 0;
        }

        public Guid Id { get; set; }

        public string Type { get; set; }

        public int? BeforeDays { get; set; }
    }
}
