using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsTemplateSave
    {
        public string Name { get; set; }
        public string Body { get; set; }
    }

    public class SmsTemplateBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
    }

    public class SmsTemplatePaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }
}
