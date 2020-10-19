using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class TCareMessageTemplatePaged
    {
        public TCareMessageTemplatePaged()
        {
            this.Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }
    public class TCareMessageTemplateBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }

    }

    public class TCareMessageTemplateSave
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public Guid? CouponProgramId { get; set; }
    }
    public class TCareMessageTemplateDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public Guid? CouponProgramId { get; set; }
        public SaleCouponProgramBasic CouponProgram { get; set; }
    }
}
