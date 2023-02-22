using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountAccountType: BaseEntity
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Note { get; set; }

        public bool IncludeInitialBalance { get; set; }
    }
}
