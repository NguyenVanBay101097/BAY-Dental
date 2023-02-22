using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EmployeeCategoryBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class EmployeeCategoryDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }

    public class EmployeeCategoryPaged
    {
        public EmployeeCategoryPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }
}
