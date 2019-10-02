using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamLineBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }
    }

    public class DotKhamLineDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid DotKhamId { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid? RoutingId { get; set; }
        public RoutingSimple Routing { get; set; }

        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinished { get; set; }

        /// <summary>
        /// Check xem đã có operation nào chưa? dùng để readonly product, routing
        /// </summary>
        public bool HasOps { get; set; }

        public IEnumerable<DotKhamLineOperationDisplay> Operations { get; set; } = new List<DotKhamLineOperationDisplay>();
    }

    public class DotKhamLineChangeRouting
    {
        public Guid Id { get; set; }

        public Guid RoutingId { get; set; }
    }
}
