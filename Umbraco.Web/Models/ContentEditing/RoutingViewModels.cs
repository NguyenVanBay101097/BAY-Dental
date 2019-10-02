using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RoutingBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }
    }

    public class RoutingSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class RoutingDisplay: RoutingBasic
    {
        public string NameGet {
            get {
                var name = Product != null ? Product.Name : string.Empty;
                if (!string.IsNullOrEmpty(Name))
                    name = Name + ": " + name;
                return name;
            }
            set { }
        }
        public IEnumerable<RoutingLineDisplay> Lines { get; set; } = new List<RoutingLineDisplay>();
    }

    public class RoutingPaged
    {
        public RoutingPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Search { get; set; }

        public Guid? ProductId { get; set; }
    }
}
