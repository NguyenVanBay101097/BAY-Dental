using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MemberLevelBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Point { get; set; }
        public string Color { get; set; }
    }

    public class MemberLevelSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Point { get; set; }
        public string Color { get; set; }
    }

    public class MemberLevelPaged
    {
        public MemberLevelPaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }
}
