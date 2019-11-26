using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MailMessageFetch
    {
        public MailMessageFetch()
        {
            Limit = 10;
        }
        public int Limit { get; set; }
    }
}
