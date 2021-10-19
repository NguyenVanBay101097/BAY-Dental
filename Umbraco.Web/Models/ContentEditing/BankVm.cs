using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class BankPaged
    {
        public BankPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }
      
    }

    public class BankBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class BankSave
    {
        public string Name { get; set; }
    }
}
