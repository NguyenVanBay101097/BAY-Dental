using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToothBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CategoryId { get; set; }
    }

    public class ToothSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

    }

    public class ToothDisplay: ToothBasic
    {
        public string ViTriHam { get; set; }

        public string Position { get; set; }
    }

    public class ToothFilter
    {
        public Guid? CategoryId { get; set; }
    }
}
