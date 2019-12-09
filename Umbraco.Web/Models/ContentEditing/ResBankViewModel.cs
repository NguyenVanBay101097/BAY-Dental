using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResBankBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BIC { get; set; }
    }

    public class ResBankSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ResBankPaged
    {
        public ResBankPaged()
        {
            Offset = 0;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }
    }
}
