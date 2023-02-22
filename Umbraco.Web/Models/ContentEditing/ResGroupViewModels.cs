using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResGroupBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class ResGroupPaged
    {
        public ResGroupPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }

    public class ResGroupDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<IRModelAccessDisplay> ModelAccesses { get; set; } = new List<IRModelAccessDisplay>();

        public IEnumerable<ApplicationUserSimple> Users { get; set; } = new List<ApplicationUserSimple>();
    }

    public class ResGroupByModulePar
    {
        public string ModuleName { get; set; }
    }
}
