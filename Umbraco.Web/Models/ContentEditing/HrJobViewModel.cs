using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrJobSave
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
    }
    
    public class HrJobBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class HrJobPaged
    {
        public HrJobPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public string Search { get; set; }

    }
}
