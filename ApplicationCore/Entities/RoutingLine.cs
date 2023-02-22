using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Công đoạn quy trình
    /// </summary>
    public class RoutingLine: BaseEntity
    {
        public Guid RoutingId { get; set; }
        public Routing Routing { get; set; }

        public string Name { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public int Sequence { get; set; }

        public string Note { get; set; }
    }
}
