using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Quy trình điều trị
    /// </summary>
    public class Routing: BaseEntity
    {
        public string Name { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public ICollection<RoutingLine> Lines { get; set; } = new List<RoutingLine>();
    }
}
