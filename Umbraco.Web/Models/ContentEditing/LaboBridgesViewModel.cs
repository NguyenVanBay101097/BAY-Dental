using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboBridgesPaged
    {
        public LaboBridgesPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }
        public int Limit { get; set; }

        public int Offset { get; set; }
    }

    public class LaboBridgeBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LaboBridgeDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class LaboBridgeSave
    {
        public string Name { get; set; }
    }

    public class LaboBridgeImportExcelRow
    {
        public string Name { get; set; }
    }
}
