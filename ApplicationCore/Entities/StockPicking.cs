using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockPicking: BaseEntity
    {
        public Guid? PartnerId { get; set; }

        public Partner Partner { get; set; }

        public Guid PickingTypeId { get; set; }
        public StockPickingType PickingType { get; set; }

        public string Note { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        public string Name { get; set; }

        public ICollection<StockMove> MoveLines { get; set; } = new List<StockMove>();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Source Location Zone
        /// </summary>
        public Guid LocationId { get; set; }
        public StockLocation Location { get; set; }

        /// <summary>
        /// Destination Location Zone
        /// </summary>
        public Guid LocationDestId { get; set; }
        public StockLocation LocationDest { get; set; }
    }
}
