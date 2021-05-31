using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsTemplateSave
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
    }

    public class SmsTemplateBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public string Type { get; set; }
    }

    public class SmsTemplatePaged
    {
        public SmsTemplatePaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public string Type { get; set; }
    }
    public class SmsTemplateBody
    {
        public string Text { get; set; }
        public string TemplateType { get; set; }
    }

    public class SmsTemplateFilter
    {
        public string Search { get; set; }
        public string Type { get; set; }
    }
}
