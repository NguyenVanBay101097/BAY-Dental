using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EmployeeAdminSave
    {
        public string Name { get; set; }
    }

    public class EmployeeAdminDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class EmployeeAdminUpdateViewModel
    {
        public string Name { get; set; }
    }

    public class EmployeeAdminPaged
    {
        public EmployeeAdminPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}
