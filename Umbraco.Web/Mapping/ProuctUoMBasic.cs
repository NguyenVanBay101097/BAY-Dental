using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProuctUoMBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public UoMDisplay UOMPO { get; set; }
    }
}
