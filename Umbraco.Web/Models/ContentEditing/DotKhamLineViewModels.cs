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

        public int? Sequence { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }

        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();
    }

    public class DotKhamLineChangeRouting
    {
        public Guid Id { get; set; }

        public Guid RoutingId { get; set; }
    }
}
