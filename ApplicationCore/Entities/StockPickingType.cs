using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockPickingType: BaseEntity
    {
        public StockPickingType()
        {
            Active = true;
            UseCreateLots = true;
            UseExistingLots = true;
        }

        public string Code { get; set; }

        public int Sequence { get; set; }

        public Guid? DefaultLocationDestId { get; set; }
        public StockLocation DefaultLocationDest { get; set; }
        public Guid? WarehouseId { get; set; }
        public StockWarehouse Warehouse { get; set; }
        public Guid IRSequenceId { get; set; }
        public IRSequence IRSequence { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }
        public Guid? DefaultLocationSrcId { get; set; }

        public StockLocation DefaultLocationSrc { get; set; }

        public Guid? ReturnPickingTypeId { get; set; }
        public StockPickingType ReturnPickingType { get; set; }

        public bool? UseCreateLots { get; set; }

        public bool? UseExistingLots { get; set; }
    }
}
