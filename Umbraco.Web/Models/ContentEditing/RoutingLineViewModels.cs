using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RoutingLineDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public int Sequence { get; set; }

        public string Note { get; set; }
    }
}
