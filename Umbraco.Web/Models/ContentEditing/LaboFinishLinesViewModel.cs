using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboFinishLinesPaged
    {
        public LaboFinishLinesPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }
        public int Limit { get; set; }

        public int Offset { get; set; }
    }

    public class LaboFinishLinePageSimple
    {
        public string Search { get; set; }
        public int? Limit { get; set; }

        public int? Offset { get; set; }
    }

    public class LaboFinishLineBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LaboFinishLineDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
     public class LaboFinishLineSave
    {
        public string Name { get; set; }
    }

    public class LaboFinishLineImportExcelRow
    {
        public string Name { get; set; }
    }

    public class LaboFinishLineSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
