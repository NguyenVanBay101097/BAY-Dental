using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboBiteJointsPaged
    {
        public LaboBiteJointsPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }
        public int Limit { get; set; }

        public int Offset { get; set; }
    }

    public class LaboBiteJointBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LaboBiteJointDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class LaboBiteJointSave
    {
        public string Name { get; set; }
    }

    public class LaboBiteJointImportExcelRow
    {
        public string Name { get; set; }
    }
}
