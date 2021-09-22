using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PrintTemplateDefault
    {
        public string Type { get; set; }
    }

    public class PrintTemplateBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// html
        /// </summary>
        public string Content { get; set; }
    }
}
